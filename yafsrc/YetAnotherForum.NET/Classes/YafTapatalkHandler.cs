/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;

    using CookComputing.XmlRpc;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    [XmlRpcService(Name = "YAF.Tapatalk", Description = "Tapatalk Service for YAF.NET", UseIntTag = true)]
    public class YafTapatalkHandler : XmlRpcService
    {
        private enum ProcessModes { Normal, TextOnly, Quote }

        #region Forum API

        [XmlRpcMethod("get_config")]
        public XmlRpcStruct GetConfig()
        {
            var yafContext = YAFTapatalkBoardContext.Create(Context);

            if (yafContext == null)
            {
                {
                    throw new XmlRpcFaultException(100, "Invalid Context");
                }
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var rpcstruct = new XmlRpcStruct
                                {
                                    { "version", "dev" },
                                    { "is_open", !yafContext.BoardSettings.RequireLogin },
                                    { "api_level", "4" },
                                    { "guest_okay", !yafContext.BoardSettings.RequireLogin },
                                    { "disable_bbcode", "0" },
                                    { "min_search_length", "4" },
                                    { "reg_url", "register.aspx" },
                                    { "charset", "UTF-8" },
                                    { "subscribe_forum", "1" },
                                    { "multi_quote", "1" },
                                    { "goto_unread", "1" },
                                    { "goto_post", "1" },
                                    { "announcement", "1" },
                                    { "no_refresh_on_post", "1" },
                                    { "subscribe_load", "1" },
                                    { "user_id", "1" },
                                    { "avatar", "0" },
                                    { "disable_subscribe_forum", "0" },
                                    { "get_latest_topic", "1" },
                                    { "mark_read", "1" },
                                    { "mark_forum", "1" },
                                    { "get_forum_status", "1" },
                                    { "hide_forum_id", string.Empty },
                                    { "mark_topic_read", "1" },
                                    { "get_topic_status", "1" },
                                    { "get_participated_forum", "1" },
                                    { "get_forum", "1" },
                                    /* Not Yet Implemented */
                                    { "can_unread", "0" },
                                    { "conversation", "0" },
                                    { "inbox_stat", "0" },
                                    { "push", "0" },
                                    { "allow_moderate", "0" },
                                    { "report_post", "0" },
                                    { "report_pm", "0" },
                                    { "get_id_by_url", "0" },
                                    { "delete_reason", "0" },
                                    { "mod_approve", "0" },
                                    { "mod_delete", "0" },
                                    { "mod_report", "0" },
                                    { "pm_load", "0" },
                                    { "mass_subscribe", "0" },
                                    { "emoji", "0" },
                                    { "searchid", "0" },
                                    { "get_smiles", "0" },
                                    { "get_online_users", "0" },
                                    { "mark_pm_unread", "0" },
                                    { "advanced_search", "0" },
                                    { "get_alert", "0" },
                                    { "advanced_delete", "0" },
                                    { "default_smiles", "0" }
                                };

            return rpcstruct;
        }


        [XmlRpcMethod("get_forum")]
        public ForumStructure[] GetForums(params object[] parameters)
        {
            var includeDescription = (parameters == null || parameters.Length == 0) || parameters[0].ToType<bool>();

            if (parameters == null || parameters.Length <= 1)
            {
                return this.GetForums(includeDescription);
            }

            var forumId = parameters[1].ToType<string>();

            return this.GetForums(forumId, includeDescription);
        }

        private ForumStructure[] GetForums(bool includeDescription)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
            {
                throw new XmlRpcFaultException(100, "Invalid Context");
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var forumTable = LegacyDb.forum_listall(yafContext.BoardID, yafContext.UserId);
            //var forumSubscriptions = fc.GetSubscriptionsForUser(yafContext.ModuleSettings.ForumModuleId, yafContext.UserId, null, 0).ToList();

            var result = new List<ForumStructure>();

            // Note that all the fields in the DataTable are strings if they come back from the cache, so they have to be converted appropriately.

            // Get the distict list of groups
            var groups = forumTable.AsEnumerable()
                .Select(r => new
                {
                    ID = Convert.ToInt32(r["CategoryID"]),
                    Name = r["Category"].ToString(),
                    SortOrder = Convert.ToInt32(r["SortOrder"]),
                    //Active = Convert.ToBoolean(r["GroupActive"])
                }).Distinct().OrderBy(o => o.SortOrder);

            // Get all forums the user can read
            var visibleForums = forumTable.AsEnumerable()
                .Select(f => new
                {
                    ID = Convert.ToInt32(f["ForumId"]),
                    ForumGroupId = Convert.ToInt32(f["ForumGroupId"]),
                    Name = f["ForumName"].ToString(),
                    Description = f["ForumDesc"].ToString(),
                    ParentForumId = Convert.ToInt32(f["ParentForumId"]),
                    ReadRoles = f["CanRead"].ToString(),
                    SubscribeRoles = f["CanSubscribe"].ToString(),
                    LastRead = Convert.ToDateTime(f["LastRead"]),
                    LastPostDate = Convert.ToDateTime(f["LastPostDate"]),
                    SortOrder = Convert.ToInt32(f["ForumSort"]),
                    Active = Convert.ToBoolean(f["ForumActive"])
                })
                .OrderBy(o => o.SortOrder).ToList();

            foreach (var group in groups)
            {
                // Find any root level forums for this group
                var groupForums = visibleForums.Where(vf => vf.ParentForumId == 0 && vf.ForumGroupId == group.ID).ToList();

                if (!groupForums.Any())
                    continue;

                // Create the structure to represent the group
                var groupStructure = new ForumStructure
                {
                    ForumId = "G" + group.ID, // Append G to distinguish between forums and groups with the same id.
                    Name = group.Name.ToBytes(),
                    Description = null,
                    ParentId = "-1",
                    LogoUrl = null,
                    HasNewPosts = false,
                    IsProtected = false,
                    IsSubscribed = false,
                    CanSubscribe = false,
                    Url = null,
                    IsGroup = true,
                };

                // Add the Child Forums
                var groupChildren = new List<ForumStructure>();
                foreach (var groupForum in groupForums)
                {
                    var forumStructure = new ForumStructure
                    {
                        ForumId = groupForum.ID.ToString(),
                        Name = HtmlHelper.StripHtml(groupForum.Name).ToBytes(),
                        Description = includeDescription ? HtmlHelper.StripHtml(groupForum.Description).ToBytes() : string.Empty.ToBytes(),
                        ParentId = 'G' + group.ID.ToString(),
                        LogoUrl = null,
                        HasNewPosts = yafContext.UserId > 0 && groupForum.LastPostDate > groupForum.LastRead,
                        IsProtected = false,
                        //IsSubscribed = forumSubscriptions.Any(fs => fs.ForumId == groupForum.ID),
                        //CanSubscribe = ActiveForums.Permissions.HasPerm(groupForum.SubscribeRoles, yafContext.ForumUser.UserRoles),
                        Url = null,
                        IsGroup = false
                    };

                    // Add any sub-forums

                    var subForums = visibleForums.Where(vf => vf.ParentForumId == groupForum.ID).ToList();

                    if (subForums.Any())
                    {
                        var forumChildren = new List<ForumStructure>();

                        foreach (var subForum in subForums)
                        {
                            forumChildren.Add(new ForumStructure
                            {
                                ForumId = subForum.ID.ToString(),
                                Name = HtmlHelper.StripHtml(subForum.Name).ToBytes(),
                                Description = includeDescription ? HtmlHelper.StripHtml(subForum.Description).ToBytes() : String.Empty.ToBytes(),
                                ParentId = groupForum.ID.ToString(),
                                LogoUrl = null,
                                HasNewPosts = yafContext.UserId > 0 && subForum.LastPostDate > subForum.LastRead,
                                IsProtected = false,
                               // IsSubscribed = forumSubscriptions.Any(fs => fs.ForumId == subForum.ID),
                               // CanSubscribe = ActiveForums.Permissions.HasPerm(subForum.SubscribeRoles, yafContext.ForumUser.UserRoles),
                                Url = null,
                                IsGroup = false
                            });
                        }

                        forumStructure.Children = forumChildren.ToArray();
                    }

                    groupChildren.Add(forumStructure);
                }

                groupStructure.Children = groupChildren.ToArray();

                result.Add(groupStructure);
            }

            return result.ToArray();
        }

        private ForumStructure[] GetForums(string forumId, bool includeDescription)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
            {
                throw new XmlRpcFaultException(100, "Invalid Context");
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var forumTable = LegacyDb.forum_listall(yafContext.BoardID, yafContext.UserId);
            //var forumSubscriptions = fc.GetSubscriptionsForUser(yafContext.ModuleSettings.ForumModuleId, yafContext.UserId, null, 0).ToList();

            var result = new List<ForumStructure>();

            

            // Get all forums the user can read
            var visibleForums = forumTable.AsEnumerable()
                .Select(f => new
                {
                    ID = f["ForumID"].ToType<int>(),
                    ForumGroupId = f["CategoryID"].ToType<int>(),
                    Name = f["Name"].ToString(),
                    Description = f["Name"].ToString(),//f["ForumDesc"].ToString(),
                    ParentForumId = f["ParentID"].ToType<int>(),
                    //ReadRoles = f["CanRead"].ToString(),
                    //SubscribeRoles = f["CanSubscribe"].ToString(),
                    //LastRead = Convert.ToDateTime(f["LastRead"]),
                    //LastPostDate = Convert.ToDateTime(f["LastPostDate"]),
                    SortOrder = f["SortOrder"].ToType<int>()//,
                    //Active = Convert.ToBoolean(f["ForumActive"])
                })
                .OrderBy(o => o.SortOrder).ToList();


            if(forumId.StartsWith("G"))
            {
               /* var groupId = forumId.Substring(1).ToType<int>();
                
                foreach(var forum in visibleForums.Where(f => f.ForumGroupId == groupId && f.ParentForumId == 0))
                {
                    var forumStructure = new ForumStructure
                    {
                        ForumId = forum.ID.ToString(),
                        Name = HtmlHelper.StripHtml(forum.Name).ToBytes(),
                        Description = includeDescription ? HtmlHelper.StripHtml(forum.Description).ToBytes() : string.Empty.ToBytes(),
                        ParentId = forumId,
                        LogoUrl = null,
                        HasNewPosts = yafContext.UserId > 0 && forum.LastPostDate > forum.LastRead,
                        IsProtected = false,
                        IsSubscribed = forumSubscriptions.Any(fs => fs.ForumId == forum.ID),
                        CanSubscribe = ActiveForums.Permissions.HasPerm(forum.SubscribeRoles, yafContext.ForumUser.UserRoles),
                        Url = null,
                        IsGroup = false
                    };

                    result.Add(forumStructure);
                }*/
            }
            else
            {
                foreach (var forum in visibleForums.Where(f => f.ParentForumId == int.Parse(forumId)))
                {
                    var forumStructure = new ForumStructure
                    {
                        ForumId = forum.ID.ToString(),
                        Name = HtmlHelper.StripHtml(forum.Name).ToBytes(),
                        Description = includeDescription ? HtmlHelper.StripHtml(forum.Description).ToBytes() : string.Empty.ToBytes(),
                        ParentId = forumId,
                        LogoUrl = null,
                        HasNewPosts = false,//yafContext.UserId > 0 && forum.LastPostDate > forum.LastRead,
                        IsProtected = false,
                        //IsSubscribed = forumSubscriptions.Any(fs => fs.ForumId == forum.ID),
                        //CanSubscribe = ActiveForums.Permissions.HasPerm(forum.SubscribeRoles, yafContext.ForumUser.UserRoles),
                        Url = null,
                        IsGroup = false
                    };

                    result.Add(forumStructure);
                }
            }

            return result.ToArray();
        }


        /// <summary>
        /// Marks the forum as read.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="CookComputing.XmlRpc.XmlRpcFaultException">100;Invalid Context</exception>
        [XmlRpcMethod("mark_all_as_read")]
        public XmlRpcStruct MarkForumAsRead(params object[] parameters)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
            {
                throw new XmlRpcFaultException(100, "Invalid Context");
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var forumId = (parameters != null && parameters.Any()) ? parameters[0].ToType<int>() : 0;

            YafContext.Current.Get<IReadTrackCurrentUser>().SetForumRead(forumId.ToType<int>());

            return new XmlRpcStruct
            {
                {
                    "result", true
                }
            };
        }
/*
        [XmlRpcMethod("get_participated_forum")]
        public XmlRpcStruct GetParticipatedForums()
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
            {
                throw new XmlRpcFaultException(100, "Invalid Context");
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var subscribedForums = fc.GetParticipatedForums(portalId, forumModuleId, userId, forumIds).ToList();

            return new XmlRpcStruct
                       {
                           {"total_forums_num", subscribedForums.Count},
                           {"forums", subscribedForums.Select(f => new ListForumStructure
                                {
                                    ForumId = f.ForumId.ToString(),
                                    ForumName = f.ForumName.ToBytes(),
                                    IsProtected = false,
                                    HasNewPosts =  f.LastPostDate > f.LastAccessDate
                                }).ToArray()}
                       };
        }

        [XmlRpcMethod("get_forum_status")]
        public XmlRpcStruct GetForumStatus(params object[] parameters)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead") + string.Empty;

            // Clean up our forum id list before we split it up.
            forumIds = Regex.Replace(forumIds, @"\;+$", string.Empty);

            var forumIdList = !string.IsNullOrWhiteSpace(forumIds)
                                  ? forumIds.Split(';').Select(int.Parse).ToList()
                                  : new List<int>();

            // Intersect requested forums with avialable forums
            var requestedForumIds = (parameters != null && parameters.Any())
                                        ? ((object[]) parameters[0]).Select(Convert.ToInt32).Where(forumIdList.Contains)
                                        : new int[] {};

            // Convert the new list of forums back to a string for the proc.
            forumIds = requestedForumIds.Aggregate(string.Empty, (current, id) => current + (id.ToString() + ";"));

            var forumStatus = fc.GetForumStatus(portalId, forumModuleId, userId, forumIds).ToList();

            return new XmlRpcStruct
                       {
                           {"forums", forumStatus.Select(f => new ListForumStructure
                                {
                                    ForumId = f.ForumId.ToString(),
                                    ForumName = f.ForumName.ToBytes(),
                                    IsProtected = false,
                                    HasNewPosts =  f.LastPostDate > f.LastAccessDate
                                }).ToArray()}
                       };
        }
        */
        #endregion

        #region Topic API
        /*
        [XmlRpcMethod("get_topic")]
        public TopicListStructure  GetTopic(params object[] parameters)
        {
            if(parameters[0].ToString().StartsWith("G"))
            {
                var yafContext = YAFTapatalkBoardContext.Create(this.Context);

                if (yafContext == null)
                    { throw new XmlRpcFaultException(100, "Invalid Context"); }

                this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

                return new TopicListStructure
                {
                    CanPost = false,
                    ForumId = parameters[0].ToString(),
                    ForumName = string.Empty.ToBytes(),
                    TopicCount = 0
                };
            }


            if (parameters.Length == 3)
                return this.GetTopic(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
            
            if (parameters.Length == 4)
                return this.GetTopic(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), parameters[3].ToString());

            throw new XmlRpcFaultException(100, "Invalid Method Signature");
        }

        private TopicListStructure GetTopic(int forumId, int startIndex, int endIndex, string mode = null)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;

            var fc = new AFTForumController();

            var fp = fc.GetForumPermissions(forumId);

            if (!ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanRead))
                throw new XmlRpcFaultException(100, "No Read Permissions");

            var maxRows = endIndex + 1 - startIndex;

            var forumTopicsSummary = fc.GetForumTopicSummary(portalId, forumModuleId, forumId, yafContext.UserId, mode);
            var forumTopics = fc.GetForumTopics(portalId, forumModuleId, forumId, yafContext.UserId, startIndex, maxRows, mode);

            var canSubscribe = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanSubscribe);

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var profilePath = string.Format("{0}://{1}{2}", this.Context.Request.Url.Scheme, this.Context.Request.Url.Host, VirtualPathUtility.ToAbsolute("~/profilepic.ashx"));

            var forumTopicsStructure = new TopicListStructure
                                           {
                                               CanPost = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanCreate),
                                               ForumId = forumId.ToString(),
                                               ForumName = forumTopicsSummary.ForumName.ToBytes(),
                                               TopicCount = forumTopicsSummary.TopicCount,
                                               Topics = forumTopics.Select(t => new TopicStructure{ 
                                                   TopicId = t.TopicId.ToString(),
                                                   AuthorAvatarUrl = string.Format("{0}?userId={1}&w=64&h=64", profilePath, t.AuthorId),
                                                   AuthorName = GetAuthorName(mainSettings, t).ToBytes(),
                                                   CanSubscribe = canSubscribe,
                                                   ForumId = forumId.ToString(),
                                                   HasNewPosts =  (t.LastReplyId < 0 && t.TopicId > t.UserLastTopicRead) || t.LastReplyId > t.UserLastReplyRead,
                                                   IsLocked = t.IsLocked,
                                                   IsSubscribed = t.SubscriptionType > 0,
                                                   LastReplyDate = t.LastReplyDate,
                                                   ReplyCount = t.ReplyCount,
                                                   Summary = GetSummary(t.Summary, t.Body).ToBytes(),
                                                   ViewCount = t.ViewCount,
                                                   Title = HttpUtility.HtmlDecode(t.Subject + string.Empty).ToBytes()
                                               }).ToArray()
                                           };
                                             
                             

            return forumTopicsStructure;
        }

        [XmlRpcMethod("new_topic")]
        public XmlRpcStruct NewTopic(params object[] parameters)
        {
            if (parameters.Length < 3)
                throw new XmlRpcFaultException(100, "Invalid Method Signature");

            var forumId = Convert.ToInt32(parameters[0]);
            var subject = Encoding.UTF8.GetString((byte[])parameters[1]);
            var body = Encoding.UTF8.GetString((byte[])parameters[2]);

            var prefixId = parameters.Length >= 4 ? Convert.ToString(parameters[3]) : null;
            var attachmentIdObjArray = parameters.Length >= 5 ? (object[])parameters[4] : null;
            var groupId = parameters.Length >= 6 ? Convert.ToString(parameters[5]) : null;

            var attachmentIds = (attachmentIdObjArray != null)
                        ? attachmentIdObjArray.Select(Convert.ToString)
                        : new string[] { }; 

            return this.NewTopic(forumId, subject, body, prefixId, attachmentIds, groupId);

        }

        private XmlRpcStruct NewTopic(int forumId, string subject, string body, string prefixId, IEnumerable<string> attachmentIds, string groupId)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);
            
            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;

            var fc = new AFTForumController();

            var forumInfo = fc.GetForum(portalId, forumModuleId, forumId);

            // Verify Post Permissions
            if(!ActiveForums.Permissions.HasPerm(forumInfo.Security.Create, yafContext.ForumUser.UserRoles))
            {
                return new XmlRpcStruct
                                {
                                    {"result", "false"}, //"true" for success
                                    {"result_text", "Not Authorized to Post".ToBytes()}, 
                                };
            }

            // Build User Permissions
            var canModApprove = ActiveForums.Permissions.HasPerm(forumInfo.Security.ModApprove, yafContext.ForumUser.UserRoles);
            var canTrust = ActiveForums.Permissions.HasPerm(forumInfo.Security.Trust, yafContext.ForumUser.UserRoles);
            var userProfile =  yafContext.UserId > 0 ? yafContext.ForumUser.Profile : new UserProfileInfo { TrustLevel = -1 };
            var userIsTrusted = Utilities.IsTrusted((int)forumInfo.DefaultTrustValue, userProfile.TrustLevel, canTrust, forumInfo.AutoTrustLevel, userProfile.PostCount);

            // Determine if the post should be approved
            var isApproved = !forumInfo.IsModerated || userIsTrusted || canModApprove;

            var mainSettings = new SettingsInfo {MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId)};

            var dnnUser = Entities.Users.UserController.GetUserById(portalId, yafContext.UserId);

            var authorName = GetAuthorName(mainSettings, dnnUser);

            var themePath = string.Format("{0}://{1}{2}", this.Context.Request.Url.Scheme, this.Context.Request.Url.Host, VirtualPathUtility.ToAbsolute("~/DesktopModules/activeforums/themes/" + mainSettings.Theme + "/"));

            subject = Utilities.CleanString(portalId, subject, false, EditorTypes.TEXTBOX, forumInfo.UseFilter, false, forumModuleId, themePath, false);
            body = Utilities.CleanString(portalId, TapatalkToHtml(body), forumInfo.AllowHTML, EditorTypes.HTMLEDITORPROVIDER, forumInfo.UseFilter, false, forumModuleId, themePath, forumInfo.AllowEmoticons);

            // Create the topic

            var ti = new TopicInfo();
            var dt = DateTime.Now;
            ti.Content.DateCreated = dt;
            ti.Content.DateUpdated = dt;
            ti.Content.AuthorId = yafContext.UserId;
            ti.Content.AuthorName = authorName;
            ti.Content.IPAddress = this.Context.Request.UserHostAddress;
            ti.TopicUrl = string.Empty;
            ti.Content.Body = body;
            ti.Content.Subject = subject;
            ti.Content.Summary = string.Empty;

            ti.IsAnnounce = false;
            ti.IsPinned = false;
            ti.IsLocked = false;
            ti.IsDeleted = false;
            ti.IsArchived = false;

            ti.StatusId = -1;
            ti.TopicIcon = string.Empty;
            ti.TopicType = 0;

            ti.IsApproved = isApproved;

            // Save the topic
            var tc = new TopicsController();
            var topicId = tc.TopicSave(portalId, ti);
            ti = tc.Topics_Get(portalId, forumModuleId, topicId, forumId, -1, false);

            if(ti == null)
            {
                return new XmlRpcStruct
                                {
                                    {"result", "false"}, //"true" for success
                                    {"result_text", "Error Creating Post".ToBytes()}, 
                                };
            }

            // Update stats
            tc.Topics_SaveToForum(forumId, topicId, portalId, forumModuleId);
            if (ti.IsApproved && ti.Author.AuthorId > 0)
            {
                var uc = new ActiveForums.Data.Profiles();
                uc.Profile_UpdateTopicCount(portalId, ti.Author.AuthorId);
            }


            try
            {
                // Clear the cache
                var cachekey = string.Format("AF-FV-{0}-{1}", portalId, forumModuleId);
                DataCache.CacheClearPrefix(cachekey);

                // Subscribe the user if they have auto-subscribe set.
                if (userProfile.PrefSubscriptionType != SubscriptionTypes.Disabled && !(Subscriptions.IsSubscribed(portalId, forumModuleId, forumId, topicId, SubscriptionTypes.Instant, yafContext.UserId)))
                {
                    new SubscriptionController().Subscription_Update(portalId, forumModuleId, forumId, topicId, (int)userProfile.PrefSubscriptionType, yafContext.UserId, yafContext.ForumUser.UserRoles);
                }

                if(isApproved)
                {
                    // Send User Notifications
                    Subscriptions.SendSubscriptions(portalId, forumModuleId, yafContext.ModuleSettings.ForumTabId, forumInfo, topicId, 0, ti.Content.AuthorId);

                    // Add Journal entry
                    var forumTabId = yafContext.ModuleSettings.ForumTabId;
                    var fullURL = fc.BuildUrl(forumTabId, forumModuleId, forumInfo.ForumGroup.PrefixURL, forumInfo.PrefixURL, forumInfo.ForumGroupId, forumInfo.ForumID, topicId, ti.TopicUrl, -1, -1, string.Empty, 1, forumInfo.SocialGroupId);
                    new Social().AddTopicToJournal(portalId, forumModuleId, forumId, topicId, ti.Author.AuthorId, fullURL, ti.Content.Subject, string.Empty, ti.Content.Body, forumInfo.ActiveSocialSecurityOption, forumInfo.Security.Read, forumInfo.SocialGroupId);
                }
                else
                {
                    // Send Mod Notifications
                    var mods = Utilities.GetListOfModerators(portalId, forumId);
                    var notificationType = NotificationsController.Instance.GetNotificationType("AF-ForumModeration");
                    var notifySubject = Utilities.GetSharedResource("NotificationSubjectTopic");
                    notifySubject = notifySubject.Replace("[DisplayName]", dnnUser.DisplayName);
                    notifySubject = notifySubject.Replace("[TopicSubject]", ti.Content.Subject);
                    var notifyBody = Utilities.GetSharedResource("NotificationBodyTopic");
                    notifyBody = notifyBody.Replace("[Post]", ti.Content.Body);
                    var notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", yafContext.ModuleSettings.ForumTabId, forumModuleId, forumId, topicId, 0);

                    var notification = new Notification
                    {
                        NotificationTypeID = notificationType.NotificationTypeId,
                        Subject = notifySubject,
                        Body = notifyBody,
                        IncludeDismissAction = false,
                        SenderUserID = dnnUser.UserID,
                        Context = notificationKey
                    };

                    NotificationsController.Instance.SendNotification(notification, portalId, null, mods);
                }
 
            }
            catch(Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex); 
            }


            var result = new XmlRpcStruct
            {
                {"result", true}, //"true" for success
               // {"result_text", "OK".ToBytes()}, 
                {"topic_id", ti.TopicId.ToString()},
            };

            if(!isApproved)
                result.Add("state", 1);

            return result;

        }

        [XmlRpcMethod("get_unread_topic")]
        public XmlRpcStruct GetUnreadTopics(params object[] parameters)
        {
            var startIndex = parameters.Any() ? Convert.ToInt32(parameters[0]) : 0;
            var endIndex = parameters.Count() > 1 ? Convert.ToInt32(parameters[1]) : startIndex + 49;
            var searchId = parameters.Count() > 2 ? Convert.ToString(parameters[2]) : null;
            var filters = parameters.Count() > 3 ? parameters[3] : null;

            return this.GetUnreadTopics(startIndex, endIndex, searchId, filters);
        }

        private XmlRpcStruct GetUnreadTopics(int startIndex, int endIndex, string searchId, object filters)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // If user is not signed in, don't return any unread topics
            if(userId <= 0)
                return new XmlRpcStruct
                       {
                           {"total_topic_num", 0},
                           {"topics", new object[]{}}
                       };  


            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var maxRows = endIndex + 1 - startIndex;

            var unreadTopics = fc.GetUnreadTopics(portalId, forumModuleId, userId, forumIds, startIndex, maxRows).ToList();

            return new XmlRpcStruct
                       {
                           {"total_topic_num", unreadTopics.Count > 0 ? unreadTopics[0].TopicCount : 0},
                           {"topics", unreadTopics.Select(t => new ExtendedTopicStructure{ 
                                                   TopicId = t.TopicId.ToString(),
                                                   AuthorAvatarUrl = GetAvatarUrl(t.LastReplyAuthorId),
                                                   AuthorId = t.LastReplyAuthorId.ToString(),
                                                   AuthorName = GetLastReplyAuthorName(mainSettings, t).ToBytes(),
                                                   ForumId = t.ForumId.ToString(),
                                                   ForumName = t.ForumName.ToBytes(),
                                                   HasNewPosts =  (t.LastReplyId < 0 && t.TopicId > t.UserLastTopicRead) || t.LastReplyId > t.UserLastReplyRead,
                                                   IsLocked = t.IsLocked,
                                                   IsSubscribed = t.SubscriptionType > 0,
                                                   CanSubscribe = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fc.GetForumPermissions(t.ForumId).CanSubscribe), // GetforumPermissions uses cache so it shouldn't be a performance issue
                                                   ReplyCount = t.ReplyCount,
                                                   Summary = GetSummary(null, t.LastReplyBody).ToBytes(),
                                                   ViewCount = t.ViewCount,
                                                   DateCreated = t.LastReplyDate,
                                                   Title = HttpUtility.HtmlDecode(t.Subject + string.Empty).ToBytes()
                                               }).ToArray()}
                       };  
        }

        [XmlRpcMethod("get_participated_topic")]
        public XmlRpcStruct GetParticipatedTopics(params object[] parameters)
        {
            var userName = parameters.Any() ?  Encoding.UTF8.GetString((byte[])parameters[0]) : null;
            var startIndex = parameters.Count() > 1 ? Convert.ToInt32(parameters[1]) : 0;
            var endIndex = parameters.Count() > 2 ? Convert.ToInt32(parameters[2]) : startIndex + 49;
            var searchId = parameters.Count() > 3 ? Convert.ToString(parameters[3]) : null;
            var userId = parameters.Count() > 4 ? Convert.ToInt32(parameters[4]) : 0;

            return this.GetParticipatedTopics(userName, startIndex, endIndex, searchId, userId);
        }

        private XmlRpcStruct GetParticipatedTopics(string participantUserName, int startIndex, int endIndex, string searchId, int participantUserId)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Lookup the user id for the username if needed
            if(participantUserId <= 0)
            {
                var forumUser = new ActiveForums.UserController().GetUser(yafContext.Portal.PortalID, yafContext.ModuleSettings.ForumModuleId, participantUserName);

                if (forumUser != null)
                    participantUserId = forumUser.UserId;
            }

            // If we don't have a valid participant user id at this point, return invalid result
            if(participantUserId <= 0)
            {
                return new XmlRpcStruct
                           {
                               {"result", false},
                               {"result_text", "User Not Found".ToBytes()},
                               {"total_topic_num", 0},
                               {"total_unread_num", 0}
                           };
            }


            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var maxRows = endIndex + 1 - startIndex;

            var participatedTopics = fc.GetParticipatedTopics(portalId, forumModuleId, userId, forumIds, participantUserId, startIndex, maxRows).ToList();

            return new XmlRpcStruct
                       {
                           {"result", true},
                           {"total_topic_num", participatedTopics.Count > 0 ? participatedTopics[0].TopicCount : 0},
                           {"total_unread_num", participatedTopics.Count > 0 ? participatedTopics[0].UnreadTopicCount : 0},
                           {"topics", participatedTopics.Select(t => new ExtendedTopicStructure{ 
                                                   TopicId = t.TopicId.ToString(),
                                                   AuthorAvatarUrl = GetAvatarUrl(t.LastReplyAuthorId),
                                                   AuthorId = t.LastReplyAuthorId.ToString(),
                                                   AuthorName = GetLastReplyAuthorName(mainSettings, t).ToBytes(),
                                                   ForumId = t.ForumId.ToString(),
                                                   ForumName = t.ForumName.ToBytes(),
                                                   HasNewPosts =  (t.LastReplyId < 0 && t.TopicId > t.UserLastTopicRead) || t.LastReplyId > t.UserLastReplyRead,
                                                   IsLocked = t.IsLocked,
                                                   IsSubscribed = t.SubscriptionType > 0,
                                                   CanSubscribe = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fc.GetForumPermissions(t.ForumId).CanSubscribe), // GetforumPermissions uses cache so it shouldn't be a performance issue
                                                   ReplyCount = t.ReplyCount,
                                                   Summary = GetSummary(null, t.LastReplyBody).ToBytes(),
                                                   ViewCount = t.ViewCount,
                                                   DateCreated = t.LastReplyDate,
                                                   Title = HttpUtility.HtmlDecode(t.Subject + string.Empty).ToBytes()
                                               }).ToArray()}
                       };
        }


        [XmlRpcMethod("get_new_topic")]
        public XmlRpcStruct GetNewTopics()
        {
            return this.GetLatestTopics(0, 100, null, null);
        }


        [XmlRpcMethod("get_latest_topic")]
        public XmlRpcStruct GetLatestTopics(params object[] parameters)
        {
            var startIndex = parameters.Any() ? Convert.ToInt32(parameters[0]) : 0;
            var endIndex = parameters.Count() > 1 ? Convert.ToInt32(parameters[1]) : startIndex + 49;
            var searchId = parameters.Count() > 2 ? Convert.ToString(parameters[2]) : null;
            var filters = parameters.Count() > 3 ? parameters[3] : null;

            return this.GetLatestTopics(startIndex, endIndex, searchId, filters);
        }

        private XmlRpcStruct GetLatestTopics(int startIndex, int endIndex, string searchId, object filters)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var maxRows = endIndex + 1 - startIndex;

            var latestTopics = fc.GetLatestTopics(portalId, forumModuleId, userId, forumIds, startIndex, maxRows).ToList();

            return new XmlRpcStruct
                       {
                           {"result", true},
                           {"total_topic_num", latestTopics.Count > 0 ? latestTopics[0].TopicCount : 0},
                           {"topics", latestTopics.Select(t => new ExtendedTopicStructure{ 
                                                   TopicId = t.TopicId.ToString(),
                                                   AuthorAvatarUrl = GetAvatarUrl(t.LastReplyAuthorId),
                                                   AuthorId = t.LastReplyAuthorId.ToString(),
                                                   AuthorName = GetLastReplyAuthorName(mainSettings, t).ToBytes(),
                                                   ForumId = t.ForumId.ToString(),
                                                   ForumName = t.ForumName.ToBytes(),
                                                   HasNewPosts =  (t.LastReplyId < 0 && t.TopicId > t.UserLastTopicRead) || t.LastReplyId > t.UserLastReplyRead,
                                                   IsLocked = t.IsLocked,
                                                   IsSubscribed = t.SubscriptionType > 0,
                                                   CanSubscribe = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fc.GetForumPermissions(t.ForumId).CanSubscribe), // GetforumPermissions uses cache so it shouldn't be a performance issue
                                                   ReplyCount = t.ReplyCount,
                                                   Summary = GetSummary(null, t.LastReplyBody).ToBytes(),
                                                   ViewCount = t.ViewCount,
                                                   DateCreated = t.LastReplyDate,
                                                   Title = HttpUtility.HtmlDecode(t.Subject + string.Empty).ToBytes()
                                               }).ToArray()}
                       };
        }


        [XmlRpcMethod("get_topic_status")]
        public XmlRpcStruct GetTopicStatus(params object[] parameters)
        {
            var topicIds = parameters.Any() ? ((object[])parameters[0]).Select(Convert.ToInt32) : new int[]{};

            return this.GetTopicStatus(topicIds);
        }

        private XmlRpcStruct GetTopicStatus(IEnumerable<int> topicIds)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var topicIdsString = topicIds.Aggregate(string.Empty, (current, topicId) => current + (topicId.ToString() + ";"));

            var unreadTopics = fc.GetTopicStatus(portalId, forumModuleId, userId, forumIds, topicIdsString).ToList();

            return new XmlRpcStruct
                       {
                           {"result", true},
                           {"status", unreadTopics.Select(t => new TopicStatusStructure(){ 
                                                   TopicId = t.TopicId.ToString(),
                                                   HasNewPosts =  (t.LastReplyId < 0 && t.TopicId > t.UserLastTopicRead) || t.LastReplyId > t.UserLastReplyRead,
                                                   IsLocked = t.IsLocked,
                                                   IsSubscribed = t.SubscriptionType > 0,
                                                   CanSubscribe = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fc.GetForumPermissions(t.ForumId).CanSubscribe), // GetforumPermissions uses cache so it shouldn't be a performance issue
                                                   ReplyCount = t.ReplyCount,
                                                   ViewCount = t.ViewCount,
                                                   LastReplyDate = t.LastReplyDate
                                               }).ToArray()}
                       };
        }

        [XmlRpcMethod("mark_topic_read")]
        public XmlRpcStruct MarkTopicsRead(params object[] parameters)
        {
            var topicIds = ((object[]) parameters[0]).Select(Convert.ToInt32);

            return this.MarkTopicsRead(topicIds);
        }

        public XmlRpcStruct MarkTopicsRead(IEnumerable<int> topicIds)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();

            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");
            var topicIdsStr = topicIds.Aggregate(string.Empty, (current, topicId) => current + (topicId.ToString() + ";"));

            fc.MarkTopicsRead(portalId, forumModuleId, userId, forumIds, topicIdsStr);

            return new XmlRpcStruct
            {
                {"result", true}
            };
        }

        */
        #endregion

        #region Post API
        /*
        [XmlRpcMethod("get_thread")]
        public PostListStructure GetThread(params object[] parameters)
        {
            if (parameters.Length == 3)
                return this.GetThread(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), false);

            if (parameters.Length == 4)
                return this.GetThread(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToBoolean(parameters[3]));

            throw new XmlRpcFaultException(100, "Invalid Method Signature");
        }

        private PostListStructure GetThread(int topicId, int startIndex, int endIndex, bool returnHtml)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;

            var fc = new AFTForumController();

            var forumId = fc.GetTopicForumId(topicId);

            if(forumId <= 0)
                throw new XmlRpcFaultException(100, "Invalid Topic");

            var fp = fc.GetForumPermissions(forumId);

            if (!ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanRead))
                throw new XmlRpcFaultException(100, "No Read Permissions");

            var maxRows = endIndex + 1 - startIndex;

            var forumPostSummary = fc.GetForumPostSummary(yafContext.Module.PortalID, yafContext.ModuleSettings.ForumModuleId, forumId, topicId, yafContext.UserId);
            var forumPosts = fc.GetForumPosts(yafContext.Module.PortalID, yafContext.ModuleSettings.ForumModuleId, forumId, topicId, yafContext.UserId, startIndex, maxRows);

            var breadCrumbs = new List<BreadcrumbStructure>
                                  {
                                      new BreadcrumbStructure
                                          {
                                              ForumId = 'G' + forumPostSummary.ForumGroupId.ToString(),
                                              IsCategory = true,
                                              Name = forumPostSummary.GroupName.ToBytes()
                                          },
                                  };

            // If we're in a sub forum, add the parent to the breadcrumb
            if(forumPostSummary.ParentForumId > 0)
                breadCrumbs.Add(new BreadcrumbStructure
                {
                    ForumId = forumPostSummary.ParentForumId.ToString(),
                    IsCategory = false,
                    Name = forumPostSummary.ParentForumName.ToBytes()
                });

            breadCrumbs.Add(new BreadcrumbStructure
            {
                ForumId = forumId.ToString(),
                IsCategory = false,
                Name = forumPostSummary.ForumName.ToBytes()
            });

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var profilePath = string.Format("{0}://{1}{2}", this.Context.Request.Url.Scheme, this.Context.Request.Url.Host, VirtualPathUtility.ToAbsolute("~/profilepic.ashx"));

            var result = new PostListStructure
                             { 
                                 PostCount = forumPostSummary.ReplyCount + 1,
                                 CanReply = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanReply),
                                 CanSubscribe = ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanSubscribe),
                                 ForumId = forumId,
                                 ForumName = forumPostSummary.ForumName.ToBytes(),
                                 IsLocked = forumPostSummary.IsLocked,
                                 IsSubscribed = forumPostSummary.SubscriptionType > 0,
                                 Subject = forumPostSummary.Subject.ToBytes(),
                                 TopicId = topicId,
                                 Posts = forumPosts.Select(p => new PostStructure
                                    {
                                          PostID = p.ContentId.ToString(),
                                          AuthorAvatarUrl = string.Format("{0}?userId={1}&w=64&h=64", profilePath, p.AuthorId),
                                          AuthorName = GetAuthorName(mainSettings, p).ToBytes(),
                                          Body =  HtmlToTapatalk(p.Body, returnHtml).ToBytes(),
                                          CanEdit = false, // TODO: Fix this
                                          IsOnline = p.IsUserOnline,
                                          PostDate = p.DateCreated,
                                          Subject = p.Subject.ToBytes()
                                    }).ToArray(),
                                 Breadcrumbs = breadCrumbs.ToArray()
                              
                             };

            return result;
        }

        [XmlRpcMethod("get_thread_by_unread")]
        public PostListStructure GetThreadByUnread(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature");

            var topicId = Convert.ToInt32(parameters[0]);
            var postsPerRequest = parameters.Length >= 2 ? Convert.ToInt32(parameters[1]) : 20;
            var returnHtml = parameters.Length >= 3 && Convert.ToBoolean(parameters[2]);

            return this.GetThreadByUnread(topicId, postsPerRequest, returnHtml);
        }

        private PostListStructure GetThreadByUnread(int topicId, int postsPerRequest, bool returnHtml)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            var postIndex = new AFTForumController().GetForumPostIndexUnread(topicId, yafContext.UserId);

            var pageIndex = (postIndex - 1)/postsPerRequest;
            var startIndex = pageIndex * postsPerRequest;
            var endIndex = startIndex + postsPerRequest - 1;

            var result = this.GetThread(topicId, startIndex, endIndex, returnHtml);

            result.Position = postIndex;

            return result;
        }

        [XmlRpcMethod("get_thread_by_post")]
        public PostListStructure GetThreadByPost(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature");

            var postId = Convert.ToInt32(parameters[0]);
            var postsPerRequest = parameters.Length >= 2 ? Convert.ToInt32(parameters[1]) : 20;
            var returnHtml = parameters.Length >= 3 && Convert.ToBoolean(parameters[2]);

            return this.GetThreadByPost(postId, postsPerRequest, returnHtml);
        }

        private PostListStructure GetThreadByPost(int postId, int postsPerRequest, bool returnHtml)
        {
            // PostId = ContentId for our purposes

            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            var postIndexResult = new AFTForumController().GetForumPostIndex(postId);
                
            if(postIndexResult == null)
                throw new XmlRpcFaultException(100, "Post Not Found");

            var pageIndex = (postIndexResult.RowIndex - 1) / postsPerRequest;
            var startIndex = pageIndex * postsPerRequest;
            var endIndex = startIndex + postsPerRequest - 1;

            var result = GetThread(postIndexResult.TopicId, startIndex, endIndex, returnHtml);

            result.Position = postIndexResult.RowIndex;

            return result;
        }

        [XmlRpcMethod("get_quote_post")]
        public XmlRpcStruct GetQuote(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature");

            var postIds = Convert.ToString(parameters[0]);

            return this.GetQuote(postIds);
        }

        private XmlRpcStruct GetQuote(string postIds)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;

            var fc = new AFTForumController();

            // Load our forum settings
            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            // Get our quote template info
            var postedByTemplate = Utilities.GetSharedResource("[RESX:PostedBy]") + " {0} {1} {2}";
            var sharedOnText = Utilities.GetSharedResource("On.Text");

            var contentIds = postIds.Split('-').Select(int.Parse).ToList();

            if(contentIds.Count > 25) // Let's be reasonable
                throw new XmlRpcFaultException(100, "Bad Request");


            var postContent = new StringBuilder();

            foreach (var contentId in contentIds)
            {
                // Retrieve the forum post
                var forumPost = fc.GetForumPost(portalId, forumModuleId, contentId);

                if (forumPost == null)
                    throw new XmlRpcFaultException(100, "Bad Request");

                // Verify read permissions - Need to do this for every content id as we can not assume they are all from the same forum (even though they probably should be)
                var fp = fc.GetForumPermissions(forumPost.ForumId);

                if (!ActiveForums.Permissions.HasPerm(yafContext.ForumUser.UserRoles, fp.CanRead))
                    continue;


                // Build our sanitized quote
                var postedBy = string.Format(postedByTemplate, GetAuthorName(mainSettings, forumPost), sharedOnText,
                                             GetServerDateTime(mainSettings, forumPost.DateCreated));

                postContent.Append(HtmlToTapatalkQuote(postedBy, forumPost.Body));
                postContent.Append("\r\n");
                // add the result

            }

            return new XmlRpcStruct
            {
                {"post_id", postIds},
                {"post_title", string.Empty.ToBytes()},
                {"post_content", postContent.ToString().ToBytes()}
            };
        }

        [XmlRpcMethod("reply_post")]
        public XmlRpcStruct Reply(params object[] parameters)
        {
            if (parameters.Length < 4)
                throw new XmlRpcFaultException(100, "Invalid Method Signature");

            var forumId = Convert.ToInt32(parameters[0]);
            var topicId = Convert.ToInt32(parameters[1]);
            var subject = Encoding.UTF8.GetString((byte[]) parameters[2]);
            var body = Encoding.UTF8.GetString((byte[])parameters[3]);

            var attachmentIdObjArray = parameters.Length >= 5 ? (object[]) parameters[4] : null;
            var groupId = parameters.Length >= 6 ? Convert.ToString(parameters[5]) : null;
            var returnHtml = parameters.Length >= 7 && Convert.ToBoolean(parameters[6]);

            var attachmentIds = (attachmentIdObjArray != null)
                                    ? attachmentIdObjArray.Select(Convert.ToString)
                                    : new string[] {}; 

            return this.Reply(forumId, topicId, subject, body, attachmentIds, groupId, returnHtml);
        }

        private XmlRpcStruct Reply(int forumId, int topicId, string subject, string body, IEnumerable<string> attachmentIds, string groupID, bool returnHtml)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;

            var fc = new AFTForumController();

            var forumInfo = fc.GetForum(portalId, forumModuleId, forumId);

            // Verify Post Permissions
            if (!ActiveForums.Permissions.HasPerm(forumInfo.Security.Reply, yafContext.ForumUser.UserRoles))
            {
                return new XmlRpcStruct
                                {
                                    {"result", "false"}, //"true" for success
                                    {"result_text", "Not Authorized to Reply".ToBytes()}, 
                                };
            }

            // Build User Permissions
            var canModApprove = ActiveForums.Permissions.HasPerm(forumInfo.Security.ModApprove, yafContext.ForumUser.UserRoles);
            var canTrust = ActiveForums.Permissions.HasPerm(forumInfo.Security.Trust, yafContext.ForumUser.UserRoles);
            var canDelete = ActiveForums.Permissions.HasPerm(forumInfo.Security.Delete, yafContext.ForumUser.UserRoles);
            var canModDelete = ActiveForums.Permissions.HasPerm(forumInfo.Security.ModDelete, yafContext.ForumUser.UserRoles);
            var canEdit = ActiveForums.Permissions.HasPerm(forumInfo.Security.Edit, yafContext.ForumUser.UserRoles);
            var canModEdit = ActiveForums.Permissions.HasPerm(forumInfo.Security.ModEdit, yafContext.ForumUser.UserRoles);

            var userProfile = yafContext.UserId > 0 ? yafContext.ForumUser.Profile : new UserProfileInfo { TrustLevel = -1 };
            var userIsTrusted = Utilities.IsTrusted((int)forumInfo.DefaultTrustValue, userProfile.TrustLevel, canTrust, forumInfo.AutoTrustLevel, userProfile.PostCount);

            // Determine if the post should be approved
            var isApproved = !forumInfo.IsModerated || userIsTrusted || canModApprove;

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var dnnUser = Entities.Users.UserController.GetUserById(portalId, yafContext.UserId);

            var authorName = GetAuthorName(mainSettings, dnnUser);

            var themePath = string.Format("{0}://{1}{2}", this.Context.Request.Url.Scheme, this.Context.Request.Url.Host, VirtualPathUtility.ToAbsolute("~/DesktopModules/activeforums/themes/" + mainSettings.Theme + "/"));

            subject = Utilities.CleanString(portalId, subject, false, EditorTypes.TEXTBOX, forumInfo.UseFilter, false, forumModuleId, themePath, false);
            body = Utilities.CleanString(portalId, TapatalkToHtml(body), forumInfo.AllowHTML, EditorTypes.HTMLEDITORPROVIDER, forumInfo.UseFilter, false, forumModuleId, themePath, forumInfo.AllowEmoticons);

            var dt = DateTime.Now;

            var ri = new ReplyInfo();
            ri.Content.DateCreated = dt;
            ri.Content.DateUpdated = dt;
            ri.Content.AuthorId = yafContext.UserId;
            ri.Content.AuthorName = authorName;
            ri.Content.IPAddress = this.Context.Request.UserHostAddress;
            ri.Content.Subject = subject;
            ri.Content.Summary = string.Empty;
            ri.Content.Body = body;
            ri.TopicId = topicId;
            ri.IsApproved = isApproved;
            ri.IsDeleted = false;
            ri.StatusId = -1;

            // Save the topic
            var rc = new ReplyController();
            var replyId = rc.Reply_Save(portalId, ri);
            ri = rc.Reply_Get(portalId, forumModuleId, topicId, replyId);

            if (ri == null)
            {
                return new XmlRpcStruct
                                {
                                    {"result", "false"}, //"true" for success
                                    {"result_text", "Error Creating Post".ToBytes()}, 
                                };
            }

            try
            {
                // Clear the cache
                var cachekey = string.Format("AF-FV-{0}-{1}", portalId, forumModuleId);
                DataCache.CacheClearPrefix(cachekey);

                // Subscribe the user if they have auto-subscribe set.
                if (userProfile.PrefSubscriptionType != SubscriptionTypes.Disabled && !(Subscriptions.IsSubscribed(portalId, forumModuleId, forumId, topicId, SubscriptionTypes.Instant, yafContext.UserId)))
                {
                    new SubscriptionController().Subscription_Update(portalId, forumModuleId, forumId, topicId, (int)userProfile.PrefSubscriptionType, yafContext.UserId, yafContext.ForumUser.UserRoles);
                }

                if (isApproved)
                {
                    // Send User Notifications
                    Subscriptions.SendSubscriptions(portalId, forumModuleId, yafContext.ModuleSettings.ForumTabId, forumInfo, topicId, ri.ReplyId, ri.Content.AuthorId);

                    // Add Journal entry
                    var forumTabId = yafContext.ModuleSettings.ForumTabId;
                    var ti = new TopicsController().Topics_Get(portalId, forumModuleId, topicId, forumId, -1, false);
                    var fullURL = fc.BuildUrl(forumTabId, forumModuleId, forumInfo.ForumGroup.PrefixURL, forumInfo.PrefixURL, forumInfo.ForumGroupId, forumInfo.ForumID, topicId, ti.TopicUrl, -1, -1, string.Empty, 1, forumInfo.SocialGroupId);
                    new Social().AddReplyToJournal(portalId, forumModuleId, forumId, topicId, ri.ReplyId, ri.Author.AuthorId, fullURL, ri.Content.Subject, string.Empty, ri.Content.Body, forumInfo.ActiveSocialSecurityOption, forumInfo.Security.Read, forumInfo.SocialGroupId);
                }
                else
                {
                    // Send Mod Notifications
                    var mods = Utilities.GetListOfModerators(portalId, forumId);
                    var notificationType = NotificationsController.Instance.GetNotificationType("AF-ForumModeration");
                    var notifySubject = Utilities.GetSharedResource("NotificationSubjectReply");
                    notifySubject = notifySubject.Replace("[DisplayName]", dnnUser.DisplayName);
                    notifySubject = notifySubject.Replace("[TopicSubject]", ri.Content.Subject);
                    var notifyBody = Utilities.GetSharedResource("NotificationBodyReply");
                    notifyBody = notifyBody.Replace("[Post]", ri.Content.Body);
                    var notificationKey = string.Format("{0}:{1}:{2}:{3}:{4}", yafContext.ModuleSettings.ForumTabId, forumModuleId, forumId, topicId, replyId);

                    var notification = new Notification
                    {
                        NotificationTypeID = notificationType.NotificationTypeId,
                        Subject = notifySubject,
                        Body = notifyBody,
                        IncludeDismissAction = false,
                        SenderUserID = dnnUser.UserID,
                        Context = notificationKey
                    };

                    NotificationsController.Instance.SendNotification(notification, portalId, null, mods);
                }

            }
            catch (Exception ex)
            {
                Services.Exceptions.Exceptions.LogException(ex);
            }


            var result = new XmlRpcStruct
            {
                {"result", true}, //"true" for success
                //{"result_text", "OK".ToBytes()}, 
                {"post_id", ri.ContentId.ToString()},
                {"post_content", HtmlToTapatalk(ri.Content.Body, false).ToBytes() },
                {"can_edit", canEdit || canModEdit },
                {"can_delete", canDelete || canModDelete },
                {"post_time", dt}/*,
                {"attachments", new {}}*/
           /* };

            if(!isApproved)
                result.Add("state", 1);

            return result;


        }

*/
        #endregion

        #region User API

        [XmlRpcMethod("login")]
        public XmlRpcStruct Login(params object[] parameters)
        {
            if (parameters.Length < 2)
            {
                throw new XmlRpcFaultException(100, "Invalid Method Signature");
            }

            var login = Encoding.UTF8.GetString((byte[])parameters[0]);
            var password = Encoding.UTF8.GetString((byte[])parameters[1]);

            return this.Login(login, password);
        }

        public XmlRpcStruct Login(string username, string password)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
            {
                throw new XmlRpcFaultException(100, "Invalid Context");
            }
            
            CombinedUserDataHelper forumUser = null;
            var result = YafContext.Current.Get<MembershipProvider>().ValidateUser(username, password);
            var resultText = string.Empty;

            if(result)
            {
                var userInfo = UserMembershipHelper.GetUser(username);

                if (userInfo == null)
                {
                    result = false;
                    resultText = "Unknown Login Error";
                }
                else
                {
                    var yafUserId = LegacyDb.user_get(YafContext.Current.PageBoardID, userInfo.ProviderUserKey);

                    // Set Login Cookie
                    var expiration = DateTime.Now.Add(FormsAuthentication.Timeout);

                    var ticket = new FormsAuthenticationTicket(1, username, DateTime.Now, expiration, false, yafUserId.ToString());
                    var authCookie = new HttpCookie(yafContext.AuthCookieName, FormsAuthentication.Encrypt(ticket))
                    {
                        Domain = FormsAuthentication.CookieDomain,
                        Path = FormsAuthentication.FormsCookiePath,
                    };


                    this.Context.Response.SetCookie(authCookie);

                    forumUser = new CombinedUserDataHelper(yafUserId); //YafUserProfile.GetProfile(userInfo.UserName);
                }
            }
            else
            {
                resultText = "Unknown Login Error";
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", result ? "true" : "false"); 


            var rpcstruct = new XmlRpcStruct
                                {
                                    {
                                        "result", result
                                    },
                                    {
                                        "result_text", resultText.ToBytes()
                                    },
                                    {
                                        "can_upload_avatar", false
                                    }
                                };

            if (!result)
            {
                return rpcstruct;
            }

            rpcstruct.Add("user_id", forumUser.UserID.ToString());
            rpcstruct.Add("username", forumUser.UserName.ToBytes());
            rpcstruct.Add("email", forumUser.Email.ToBytes());

            // TODO
            rpcstruct.Add("usergroup_id", new string[] { });

            rpcstruct.Add("post_count", forumUser.NumPosts);
            rpcstruct.Add("icon_url", GetAvatarUrl(forumUser.UserID));

            return rpcstruct;
        }

        [XmlRpcMethod("logout_user")]
        public void Logout()
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
            {
                throw new XmlRpcFaultException(100, "Invalid Context");
            }

            this.Context.Response.AddHeader("Mobiquo_is_login", "false");

            var authCookie = new HttpCookie(yafContext.AuthCookieName, string.Empty)
                            {
                                Expires = DateTime.Now.AddDays(-1)
                            };


            this.Context.Response.SetCookie(authCookie);
        }

        #endregion

        #region Subscribe API
        /*
        [XmlRpcMethod("subscribe_forum")]
        public XmlRpcStruct SubscribeForum(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature"); 

            var forumId = Convert.ToInt32(parameters[0]);

            return this.Subscribe(forumId, null, false);
        }

        [XmlRpcMethod("subscribe_topic")]
        public XmlRpcStruct SubscribeTopic(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature"); 

            var topicId = Convert.ToInt32(parameters[0]);

            return this.Subscribe(null, topicId, false);
        }

        [XmlRpcMethod("unsubscribe_forum")]
        public XmlRpcStruct UnsubscribeForum(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature"); 

            var forumId = Convert.ToInt32(parameters[0]);

            return this.Subscribe(forumId, null, true);
        }

        [XmlRpcMethod("unsubscribe_topic")]
        public XmlRpcStruct UnsubscribeTopic(params object[] parameters)
        {
            if (parameters.Length < 1)
                throw new XmlRpcFaultException(100, "Invalid Method Signature"); 

            var topicId = Convert.ToInt32(parameters[0]);

            return this.Subscribe(null, topicId, true);
        }

        private XmlRpcStruct Subscribe(int? forumId, int? topicId, bool unsubscribe)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            if (!forumId.HasValue && !topicId.HasValue)
                return new XmlRpcStruct{{"result", "0"},{"result_text", "Bad Request".ToBytes()}};


            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;

            // Look up the forum Id if needed
            if(!forumId.HasValue)
            {
                var ti = new TopicsController().Topics_Get(portalId, forumModuleId, topicId.Value);
                if(ti == null)
                    return new XmlRpcStruct { { "result", false }, { "result_text", "Topic Not Found".ToBytes() } };

                var post = new AFTForumController().GetForumPost(portalId, forumModuleId, ti.ContentId);
                if(post == null)
                    return new XmlRpcStruct { { "result", false }, { "result_text", "Topic Post Not Found".ToBytes() } };

                forumId = post.ForumId;
            }

            var subscriptionType = unsubscribe ? SubscriptionTypes.Disabled : SubscriptionTypes.Instant;

            var sub = new SubscriptionController().Subscription_Update(portalId, forumModuleId, forumId.Value, topicId.HasValue ? topicId.Value : -1, (int)subscriptionType, yafContext.UserId, yafContext.ForumUser.UserRoles);

            var result = (sub >= 0) ? "1" : "0";

            return new XmlRpcStruct
            {
                {"result", result},
                {"result_text", string.Empty.ToBytes()}, 
            };
        }
    
        [XmlRpcMethod("get_subscribed_forum")]
        public XmlRpcStruct GetSubscribedForums()
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var subscribedForums = fc.GetSubscribedForums(portalId, forumModuleId, userId, forumIds).ToList();

            return new XmlRpcStruct
                       {
                           {"total_forums_num", subscribedForums.Count},
                           {"forums", subscribedForums.Select(f => new ListForumStructure
                                {
                                    ForumId = f.ForumId.ToString(),
                                    ForumName = f.ForumName.ToBytes(),
                                    IsProtected = false,
                                    HasNewPosts =  f.LastPostDate > f.LastAccessDate
                                }).ToArray()}
                       };
        }

        [XmlRpcMethod("get_subscribed_topic")]
        public XmlRpcStruct GetSubscribedTopics(params object[] parameters)
        {
            var startIndex = parameters.Any() ? Convert.ToInt32(parameters[0]) : 0;
            var endIndex = parameters.Count() > 1 ? Convert.ToInt32(parameters[1]) : startIndex + 49;

            if (endIndex < startIndex)
                return null;

            if (endIndex > startIndex + 49)
                endIndex = startIndex + 49;

            return this.GetSubscribedTopics(startIndex, endIndex);
        }

        private XmlRpcStruct GetSubscribedTopics(int startIndex, int endIndex)
        {
            var yafContext = YAFTapatalkBoardContext.Create(this.Context);

            if (yafContext == null)
                { throw new XmlRpcFaultException(100, "Invalid Context"); }

            this.Context.Response.AddHeader("Mobiquo_is_login", yafContext.UserId > 0 ? "true" : "false");

            var portalId = yafContext.Module.PortalID;
            var forumModuleId = yafContext.ModuleSettings.ForumModuleId;
            var userId = yafContext.UserId;

            // Build a list of forums the user has access to
            var fc = new AFTForumController();
            var forumIds = fc.GetForumsForUser(yafContext.ForumUser.UserRoles, portalId, forumModuleId, "CanRead");

            var mainSettings = new SettingsInfo { MainSettings = new Entities.Modules.ModuleController().GetModuleSettings(forumModuleId) };

            var profilePath = string.Format("{0}://{1}{2}", this.Context.Request.Url.Scheme, this.Context.Request.Url.Host, VirtualPathUtility.ToAbsolute("~/profilepic.ashx"));

            var maxRows = endIndex + 1 - startIndex;

            var subscribedTopics = fc.GetSubscribedTopics(portalId, forumModuleId, userId, forumIds, startIndex, maxRows).ToList();



            return new XmlRpcStruct
                       {
                           {"total_topic_num", subscribedTopics.Count > 0 ? subscribedTopics[0].TopicCount : 0},
                           {"topics", subscribedTopics.Select(t => new ExtendedTopicStructure{ 
                                                   TopicId = t.TopicId.ToString(),
                                                   AuthorAvatarUrl = string.Format("{0}?userId={1}&w=64&h=64", profilePath, t.LastReplyAuthorId),
                                                   AuthorName = GetLastReplyAuthorName(mainSettings, t).ToBytes(),
                                                   AuthorId = t.LastReplyAuthorId.ToString(),
                                                   ForumId = t.ForumId.ToString(),
                                                   ForumName = t.ForumName.ToBytes(),
                                                   HasNewPosts =  (t.LastReplyId < 0 && t.TopicId > t.UserLastTopicRead) || t.LastReplyId > t.UserLastReplyRead,
                                                   IsLocked = t.IsLocked,
                                                   ReplyCount = t.ReplyCount,
                                                   Summary = GetSummary(null, t.LastReplyBody).ToBytes(),
                                                   ViewCount = t.ViewCount,
                                                   DateCreated = t.LastReplyDate,
                                                   Title = HttpUtility.HtmlDecode(t.Subject + string.Empty).ToBytes()
                                               }).ToArray()}
                       };   
        }
        */
        #endregion


        #region Private Helper Methods

        private static string GetSummary(string summary, string body)
        {
            var result = !string.IsNullOrWhiteSpace(summary) ? summary : body;

            result = "{0}{1}".FormatWith(result, string.Empty);

            result = HttpUtility.HtmlDecode(HtmlHelper.StripHtml(result));

            result = result.Length > 200 ? result.Substring(0, 200) : result;

            return result.Trim();
        }

        private static string TapatalkToHtml(string input)
        {
            input = input.Replace("<", "&lt;");
            input = input.Replace(">", "&gt;");

            input = input.Replace("\r\n", "\n");
            input = input.Trim(new [] {' ', '\n', '\r', '\t'}).Replace("\n", "<br />");

            input = Regex.Replace(input, @"\[quote\=\'([^\]]+?)\'\]", "<blockquote class='afQuote'><span class='afQuoteTitle'>$1</span><br />", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            input = Regex.Replace(input, @"\[quote\=\""([^\]]+?)\""\]", "<blockquote class='afQuote'><span class='afQuoteTitle'>$1</span><br />", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            input = Regex.Replace(input, @"\[quote\=([^\]]+?)\]",  "<blockquote class='afQuote'><span class='afQuoteTitle'>$1</span><br />", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            input = Regex.Replace(input, @"\[quote\]", "<blockquote class='afQuote'>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\[\/quote\]", "</blockquote>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            input = Regex.Replace(input, @"\[img\](.+?)\[\/img\]", "<img src='$1' />", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\[url=\'(.+?)\'\](.+?)\[\/url\]", "<a href='$1'>$2</a>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\[url=\""(.+?)\""\](.+?)\[\/url\]", "<a href='$1'>$2</a>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\[url=(.+?)\](.+?)\[\/url\]", "<a href='$1'>$2</a>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\[url\](.+?)\[\/url\]", "<a href='$1'>$1</a>", RegexOptions.IgnoreCase);
            input = Regex.Replace(input, @"\[(\/)?b\]", "<$1strong>", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            input = Regex.Replace(input, @"\[(\/)?i\]", "<$1i>", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return input;
        }

        /*private static string HtmlToTapatalk(string input, bool returnHtml)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = Regex.Replace(input, @"\s+", " ", RegexOptions.Multiline);

            var htmlBlock = new HtmlDocument();
            htmlBlock.LoadHtml(input);

            var tapatalkMarkup = new StringBuilder();

            ProcessNode(tapatalkMarkup, htmlBlock.DocumentNode, ProcessModes.Normal, returnHtml);

            return tapatalkMarkup.ToString().Trim(new[] { ' ', '\n', '\r', '\t' });
        }

        private static string HtmlToTapatalkQuote(string postedBy, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = Regex.Replace(input, @"\s+", " ", RegexOptions.Multiline);

            var htmlBlock = new HtmlDocument();
            htmlBlock.LoadHtml(input);

            var tapatalkMarkup = new StringBuilder();

            ProcessNode(tapatalkMarkup, htmlBlock.DocumentNode, ProcessModes.Quote, false);

            return string.Format("[quote={0}]\r\n{1}\r\n[/quote]\r\n", postedBy, tapatalkMarkup.ToString().Trim(new[] { ' ', '\n', '\r', '\t' }));
        }

        private static void ProcessNodes(StringBuilder output, IEnumerable<HtmlNode> nodes, ProcessModes mode, bool returnHtml)
        {
            foreach (var node in nodes)
                ProcessNode(output, node, mode, returnHtml);
        }

        private static void ProcessNode(StringBuilder output, HtmlNode node, ProcessModes mode, bool returnHtml)
        {
            var lineBreak = returnHtml ? "<br />" : "\r\n"; // (mode == ProcessModes.Quote) ? "\n" : "<br /> ";

            if (node == null || output == null || (mode == ProcessModes.TextOnly && node.Name != "#text"))
                return;

            switch (node.Name)
            {
                // No action needed for these node types
                case "#text":
                    var text = HttpUtility.HtmlDecode(node.InnerHtml);
                    if (mode != ProcessModes.Quote)
                        text = HttpContext.Current.Server.HtmlEncode(text);
                    output.Append(text);
                    return;

                case "tr":
                    ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                    output.Append(lineBreak);
                    return;

                case "script":
                    return;

                case "ol":
                case "ul":

                    if(mode != ProcessModes.Normal)
                        return;

                    output.Append(lineBreak);

                    var listItemNodes = node.SelectNodes("//li");

                    for(var i = 0;  i < listItemNodes.Count; i++)
                    {
                        var listItemNode = listItemNodes[i];
                        output.AppendFormat("{0} ", node.Name == "ol" ? (i + 1).ToString() : "*");
                        ProcessNodes(output, listItemNode.ChildNodes, mode, returnHtml);
                        output.Append(lineBreak);
                    }
                    
                    return;

                case "li":

                    if(mode == ProcessModes.Quote)
                        return; 

                    output.Append("* ");
                    ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                    output.Append(lineBreak);
                    return;

                case "p":
                    ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                    output.Append(lineBreak);
                    return;

                case "b":
                case "strong":

                    if(mode != ProcessModes.Quote)
                    {
                        output.Append("<b>");
                        ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                        output.Append("</b>");
                    }
                    else
                    {
                        output.Append("[b]");
                        ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                        output.Append("[/b]");
                    }

                    return;

                case "i":
                    if(mode != ProcessModes.Quote)
                    {
                        output.Append("<i>");
                        ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                        output.Append("</i>");
                    }
                    else
                    {
                        output.Append("[i]");
                        ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                        output.Append("[/i]");
                    }

                    return;

                case "blockquote":

                    if(mode != ProcessModes.Normal)
                        return;

                    output.Append("[quote]");
                    ProcessNodes(output, node.ChildNodes, mode, returnHtml);
                    output.Append("[/quote]" + lineBreak);
                    return;

                case "br":
                    output.Append(lineBreak);
                    return;


                case "img":

                    var src = node.Attributes["src"];
                    if (src == null || string.IsNullOrWhiteSpace(src.Value))
                        return;

                    var isEmoticon = src.Value.IndexOf("emoticon", 0, StringComparison.InvariantCultureIgnoreCase) >= 0;

                    var url = src.Value.Trim();
                    var request = HttpContext.Current.Request;

                    // Make a fully qualifed URL
                    if (!url.ToLower().StartsWith("http"))
                    {
                        var rootDirectory = url.StartsWith("/") ? string.Empty : "/";
                        url = string.Format("{0}://{1}{2}{3}", request.Url.Scheme, request.Url.Host, rootDirectory,  url);
                    }

                    if(mode == ProcessModes.Quote && isEmoticon)
                        return;

                    output.AppendFormat(isEmoticon ? "<img src='{0}' />" : "[img]{0}[/img]", url);

                    return;

                case "a":

                    var href = node.Attributes["href"];
                    if (href == null || string.IsNullOrWhiteSpace(href.Value))
                        return;

                    output.AppendFormat("[url={0}]", href.Value);
                    ProcessNodes(output, node.ChildNodes, ProcessModes.TextOnly, returnHtml); 
                    output.Append("[/url]");

                    return;

            }

            ProcessNodes(output, node.ChildNodes, mode, returnHtml);
        }*/
        /*
        private static string GetAuthorName(SettingsInfo settings, UserInfo user)
        {
            if (user == null || user.UserID <= 0)
                return "Guest";

            switch (settings.UserNameDisplay.ToUpperInvariant())
            {
                case "USERNAME":
                    return user.Username.Trim();
                case "FULLNAME":
                    return (user.FirstName.Trim() + " " + user.LastName.Trim());
                case "FIRSTNAME":
                    return user.FirstName.Trim();
                case "LASTNAME":
                    return user.LastName.Trim();
                default:
                    return user.DisplayName.Trim();
            }

        }

        private static string GetAuthorName(SettingsInfo settings, ForumTopic topic)
        {
            if (topic == null || topic.AuthorId <= 0)
                return "Guest";

            switch (settings.UserNameDisplay.ToUpperInvariant())
            {
                case "USERNAME":
                    return topic.AuthorUserName.Trim();
                case "FULLNAME":
                    return (topic.AuthorFirstName.Trim() + " " + topic.AuthorLastName.Trim());
                case "FIRSTNAME":
                    return topic.AuthorFirstName.Trim();
                case "LASTNAME":
                    return topic.AuthorLastName.Trim();
                default:
                    return topic.AuthorDisplayName.Trim();
            }

        }

        private static string GetLastReplyAuthorName(SettingsInfo settings, ForumTopic topic)
        {
            if (topic == null || topic.AuthorId <= 0)
                return "Guest";

            switch (settings.UserNameDisplay.ToUpperInvariant())
            {
                case "USERNAME":
                    return topic.LastReplyUserName.Trim();
                case "FULLNAME":
                    return (topic.LastReplyFirstName.Trim() + " " + topic.LastReplyLastName.Trim());
                case "FIRSTNAME":
                    return topic.LastReplyFirstName.Trim();
                case "LASTNAME":
                    return topic.LastReplyLastName.Trim();
                default:
                    return topic.LastReplyDisplayName.Trim();
            }

        }

        private static string GetAuthorName(SettingsInfo settings, ForumPost post)
        {
            if (post == null || post.AuthorId <= 0)
                return "Guest";

            switch (settings.UserNameDisplay.ToUpperInvariant())
            {
                case "USERNAME":
                    return post.UserName.Trim();
                case "FULLNAME":
                    return (post.FirstName.Trim() + " " + post.LastName.Trim());
                case "FIRSTNAME":
                    return post.FirstName.Trim();
                case "LASTNAME":
                    return post.LastName.Trim();
                default:
                    return post.DisplayName.Trim();
            }

        }

        private static string GetServerDateTime(SettingsInfo settings, DateTime displayDate)
        {
            //Dim newDate As Date 
            string dateString;
            try
            {
                dateString = displayDate.ToString(settings.DateFormatString + " " + settings.TimeFormatString);
                return dateString;
            }
            catch (Exception ex)
            {
                dateString = displayDate.ToString();
                return dateString;
            }
        }*/

        private static string GetAvatarUrl(int userId)
        {
            string avatarUrl = YafContext.Current.Get<IAvatars>().GetAvatarUrlForUser(userId);

            if (avatarUrl.IsNotSet())
            {
                avatarUrl = "{0}images/noavatar.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
            }

            return avatarUrl;
        }

       #endregion

    }

    

    public class YAFTapatalkBoardContext
    {
        private HttpContext _context;
        private int? _userId;
        //private User _forumUser;
        private YafBoardSettings _boardSettings;

        public bool? AuthCookieExpired { get; internal set; }

        public int BoardID { get; internal set; }

        public YafBoardSettings BoardSettings
        {
            get
            {
                return this._boardSettings ?? (this._boardSettings = new YafLoadBoardSettings(this.BoardID));
            }
        }

        public string AuthCookieName
        {
            get { return string.Format(".YAFNET-Tapatalk_{0}", this.BoardID); }
        }

        public int UserId
        {
            get
            {
                if (this._userId.HasValue)
                {
                    return this._userId.Value;
                }

                this._userId = 0;

                var authCookie = this._context.Request.Cookies[this.AuthCookieName];

                if (authCookie == null || authCookie.Value.IsNotSet())
                {
                    return this._userId.Value;
                }

                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket.Expired)
                {
                    this._context.Response.Cookies.Add(new HttpCookie(this.AuthCookieName, string.Empty) { Expires = DateTime.Now.AddDays(-1) });
                }
                else
                {
                    this._userId = int.Parse(ticket.UserData);
                }

                return this._userId.Value;
            }
        }

        /*public User ForumUser
        {
            get
            {
                if (_forumUser == null && Module != null)
                {
                    _forumUser = (UserId > 0) ? new UserController().GetUser(Module.PortalID, ModuleId, UserId) : new User();
                }

                return _forumUser;
            }
        }
        */

        public static YAFTapatalkBoardContext Create(HttpContext context)
        {
            var match = Regex.Match(context.Request.Path, @"\/aft(\d+)\/mobiquo", RegexOptions.IgnoreCase);

            int boardId;

            return match.Groups.Count < 2 || !int.TryParse(match.Groups[1].Value, out boardId)
                       ? null
                       : new YAFTapatalkBoardContext { _context = context, BoardID = boardId };
        }
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct ForumStructure
    {
        [XmlRpcMember("forum_id")]
        public string ForumId;

        [XmlRpcMember("forum_name")]
        public byte[] Name;

        [XmlRpcMember("description")]
        public byte[] Description;

        [XmlRpcMember("parent_id")]
        public string ParentId;

        [XmlRpcMember("logo_url")]
        public string LogoUrl;

        [XmlRpcMember("new_post")]
        public bool HasNewPosts;

        [XmlRpcMember("is_protected")]
        public bool IsProtected;

        [XmlRpcMember("is_subscribed")]
        public bool IsSubscribed;

        [XmlRpcMember("can_subscribe")]
        public bool CanSubscribe;

        [XmlRpcMember("url")]
        public string Url;

        [XmlRpcMember("sub_only")]
        public bool IsGroup;

        [XmlRpcMember("child")]
        public ForumStructure[] Children;

    }
} 