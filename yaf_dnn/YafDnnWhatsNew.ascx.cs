/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.DotNetNuke
{
    #region

    using System;
    using System.Collections;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Classes;

    using global::DotNetNuke.Common;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Framework;
    using global::DotNetNuke.Services.Exceptions;
    using global::DotNetNuke.Services.Localization;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.DotNetNuke.Controller;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Extensions;

    #endregion

    /// <summary>
    /// The yaf dnn whats new.
    /// </summary>
    public partial class YafDnnWhatsNew : PortalModuleBase
    {
        // Settings
        #region Constants and Fields

        /// <summary>
        ///   The cache size.
        /// </summary>
        private const int CacheSize = 500;

        /// <summary>
        ///   Use Relative Time Setting
        /// </summary>
        private bool useRelativeTime;

        /// <summary>
        ///   The yaf board id.
        /// </summary>
        private int boardId;

        /// <summary>
        ///   The max posts.
        /// </summary>
        private int maxPosts;

        /// <summary>
        ///   The yaf module id.
        /// </summary>
        private int yafModuleId;

        /// <summary>
        ///   The yaf tab id.
        /// </summary>
        private int yafTabId;

        private string headerTemplate;

        private string itemTemplate;

        private string footerTemplate;

        #endregion

        #region Methods
        
        /// <summary>
        /// The clean string for url.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// Returns the Cleaned String
        /// </returns>
        protected static string CleanStringForURL(string str)
        {
            var sb = new StringBuilder();

            // trim...
            str = HttpContext.Current.Server.HtmlDecode(str.Trim());

            // fix ampersand...
            str = str.Replace("&", "and");

            // normalize the Unicode
            str = str.Normalize(NormalizationForm.FormD);

            foreach (char currentChar in str)
            {
                if (char.IsWhiteSpace(currentChar) || currentChar == '.')
                {
                    sb.Append('-');
                }
                else if (char.GetUnicodeCategory(currentChar) != UnicodeCategory.NonSpacingMark &&
                         !char.IsPunctuation(currentChar) && !char.IsSymbol(currentChar) && currentChar < 128)
                {
                    sb.Append(currentChar);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The get cache name.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// Returns the Cache Name.
        /// </returns>
        protected string GetCacheName(string type, int id)
        {
            return @"urlRewritingDT-{0}-Range-{1}-to-{2}".FormatWith(type, this.HighRange(id), this.LowRange(id));
        }

        /// <summary>
        /// The get data row from cache.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// Returns cached Data Row
        /// </returns>
        protected DataRow GetDataRowFromCache(string type, int id)
        {
            // get the datatable and find the value
            var list = HttpContext.Current.Cache[this.GetCacheName(type, id)] as DataTable;

            if (list != null)
            {
                DataRow row = list.Rows.Find(id);

                // valid, return...
                if (row != null)
                {
                    return row;
                }

                // invalidate this cache section
                HttpContext.Current.Cache.Remove(this.GetCacheName(type, id));
            }

            return null;
        }

        /// <summary>
        /// The get forum name.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// Returns the Forum Name.
        /// </returns>
        protected string GetForumName(int id)
        {
            const string Type = "Forum";
            const string PrimaryKey = "ForumID";
            const string NameField = "Name";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = LegacyDb.forum_simplelist(this.LowRange(id), CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// The get topic name.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// Returns the Topic Name.
        /// </returns>
        protected string GetTopicName(int id)
        {
            const string Type = "Topic";
            const string PrimaryKey = "TopicID";
            const string NameField = "Topic";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = LegacyDb.topic_simplelist(this.LowRange(id), CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return CleanStringForURL(row[NameField].ToString());
        }

        /// <summary>
        /// The get topic name from message.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// Returns the Topic Name.
        /// </returns>
        protected string GetTopicNameFromMessage(int id)
        {
            const string Type = "Message";
            const string PrimaryKey = "MessageID";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = LegacyDb.message_simplelist(this.LowRange(id), CacheSize);

                // set it up in the cache
                row = this.SetupDataToCache(ref list, Type, id, PrimaryKey);

                if (row == null)
                {
                    return string.Empty;
                }
            }

            return this.GetTopicName(row["TopicID"].ToType<int>());
        }

        /// <summary>
        /// The high range.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The high range.
        /// </returns>
        protected int HighRange(int id)
        {
            return (int)(Math.Ceiling((double)(id / CacheSize)) * CacheSize);
        }

        /// <summary>
        /// The latest posts_ item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void LatestPostsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.Header:
                    {
                        Literal objLiteral = new Literal { Text = this.GetHeader() };
                        e.Item.Controls.Add(objLiteral);
                    }

                    break;
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    {
                        Literal objLiteral = new Literal { Text = this.ProcessItem(e) };
                        e.Item.Controls.Add(objLiteral); 
                    }

                    break;
                /*case ListItemType.Separator:
                    {
                        Literal objLiteral = new Literal { Text = this.ProcessSeparator() };
                        e.Item.Controls.Add(objLiteral);
                    }

                    break;*/
                case ListItemType.Footer:
                    {
                        Literal objLiteral = new Literal { Text = this.GetFooter() };
                        e.Item.Controls.Add(objLiteral);
                    }

                    break;
            }

            /*// populate the controls here...
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }*/            
        }

        /// <summary>
        /// Lows the range.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The low range.
        /// </returns>
        protected int LowRange(int id)
        {
            return (int)(Math.Floor((double)(id / CacheSize)) * CacheSize);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <param name="sender">
        /// The sender. 
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// -----------------------------------------------------------------------------
        protected void Page_Load(object sender, EventArgs e)
        {
            Type csType = typeof(Page);

            this.LoadSettings();

            if (this.useRelativeTime)
            {
                jQuery.RequestRegistration();

                ScriptManager.RegisterClientScriptInclude(
                    this,
                    csType,
                    "timeagojs",
                    this.ResolveUrl("~/DesktopModules/YetAnotherForumDotNet/resources/js/jquery.timeago.js"));

                var timeagoLoadJs =
                    @"Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(loadTimeAgo);
            function loadTimeAgo() {				      	
            " +
                    Localization.GetString("TIMEAGO_JS", this.LocalResourceFile) +
                    @"
              jQuery('abbr.timeago').timeago();	
			      }";

                ScriptManager.RegisterStartupScript(this, csType, "timeagoloadjs", timeagoLoadJs, true);
            }

            this.BindData();
        }

        /// <summary>
        /// The setup data to cache.
        /// </summary>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="primaryKey">
        /// The primary key.
        /// </param>
        /// <returns>
        /// Data Row
        /// </returns>
        protected DataRow SetupDataToCache(ref DataTable list, string type, int id, string primaryKey)
        {
            DataRow row = null;

            if (list != null)
            {
                list.Columns[primaryKey].Unique = true;
                list.PrimaryKey = new[] { list.Columns[primaryKey] };

                // store it for the future
                var randomValue = new Random();
                HttpContext.Current.Cache.Insert(
                    this.GetCacheName(type, id), 
                    list, 
                    null, 
                    DateTime.UtcNow.AddMinutes(randomValue.Next(5, 15)), 
                    Cache.NoSlidingExpiration, 
                    CacheItemPriority.Low, 
                    null);

                // find and return profile..
                row = list.Rows.Find(id);

                if (row == null)
                {
                    // invalidate this cache section
                    HttpContext.Current.Cache.Remove(this.GetCacheName(type, id));
                }
            }

            return row;
        }

        /// <summary>
        /// Get the Yaf Board id from ModuleId and the Module Settings
        /// </summary>
        /// <param name="iModuleId">
        /// The Module id of the Yaf Module Instance
        /// </param>
        /// <returns>
        /// The Board Id From the Settings
        /// </returns>
        private static int GetYafBoardId(int iModuleId)
        {
            var moduleController = new ModuleController();
            Hashtable moduleSettings = moduleController.GetModuleSettings(iModuleId);

            int iForumId;

            try
            {
                iForumId = int.Parse((string)moduleSettings["forumboardid"]);
            }
            catch (Exception)
            {
                iForumId = -1;
            }

            return iForumId;
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            try
            {
                var yafUserId = this.GetYafUserId();

                Data.ActiveAccessUser(this.boardId, yafUserId, HttpContext.Current.User.Identity.IsAuthenticated);

                var activeTopics = Data.TopicLatest(
                  this.boardId, this.maxPosts, yafUserId, false, true);

                this.LatestPosts.DataSource = activeTopics;
                this.LatestPosts.DataBind();

                if (activeTopics.Rows.Count <= 0)
                {
                    this.lInfo.Text = Localization.GetString("NoMessages.Text", this.LocalResourceFile);
                    this.lInfo.Style.Add("font-style", "italic");
                }
                else
                {
                    this.lInfo.Text = string.Empty;
                }
            }
            catch (Exception exc)
            {
                // Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Get Yaf User Id from the Current DNN User
        /// </summary>
        /// <returns>
        /// The Yaf User ID
        /// </returns>
        private int GetYafUserId()
        {
            // Check for user
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // get the user from the membership provider
                MembershipUser dnnUser = Membership.GetUser(UserInfo.Username, true);

                if (dnnUser != null)
                {
                    return LegacyDb.user_get(this.boardId, dnnUser.ProviderUserKey);
                }
            }

            return UserMembershipHelper.GuestUserId;
        }

        /// <summary>
        /// Load Module Settings
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                var objModuleController = new ModuleController();

                Hashtable moduleSettings = objModuleController.GetTabModuleSettings(this.TabModuleId);

                if (!string.IsNullOrEmpty((string)moduleSettings["YafPage"]))
                {
                    this.yafTabId = int.Parse((string)moduleSettings["YafPage"]);
                }
                else
                {
                    // If Module is not Configured show Message or Redirect to Settings Page if Current User has Edit Rights
                    if (this.IsEditable)
                    {
                        this.Response.Redirect(
                            this.ResolveUrl(
                                "~/tabid/{0}/ctl/Module/ModuleId/{1}/Default.aspx".FormatWith(
                                    this.PortalSettings.ActiveTab.TabID, this.ModuleId)));
                    }
                    else
                    {
                        this.lInfo.Text = Localization.GetString("NotConfigured.Text", this.LocalResourceFile);
                        this.lInfo.Style.Add("font-style", "italic");
                    }
                }

                if (!string.IsNullOrEmpty((string)moduleSettings["YafModuleId"]))
                {
                    this.yafModuleId = int.Parse((string)moduleSettings["YafModuleId"]);
                }
                else
                {
                    // If Module is not Configured show Message or Redirect to Settings Page if Current User has Edit Rights
                    if (this.IsEditable)
                    {
                        this.Response.Redirect(
                            this.ResolveUrl(
                                "~/tabid/{0}/ctl/Module/ModuleId/{1}/Default.aspx".FormatWith(
                                    this.PortalSettings.ActiveTab.TabID, this.ModuleId)));
                    }
                    else
                    {
                        this.lInfo.Text = Localization.GetString("NotConfigured.Text", this.LocalResourceFile);
                        this.lInfo.Style.Add("font-style", "italic");
                    }
                }

                // Get and Set Board Id
                this.boardId = GetYafBoardId(this.yafModuleId);

                if (this.boardId.Equals(-1))
                {
                    // If Module is not Configured show Message or Redirect to Settings Page if Current User has Edit Rights
                    if (this.IsEditable)
                    {
                        this.Response.Redirect(
                            this.ResolveUrl(
                                "~/tabid/{0}/ctl/Module/ModuleId/{1}/Default.aspx".FormatWith(
                                    this.PortalSettings.ActiveTab.TabID, this.ModuleId)));
                    }
                    else
                    {
                        this.lInfo.Text = Localization.GetString("NotConfigured.Text", this.LocalResourceFile);
                        this.lInfo.Style.Add("font-style", "italic");
                    }
                }

                try
                {
                    this.maxPosts = !string.IsNullOrEmpty((string)moduleSettings["YafMaxPosts"])
                                         ? int.Parse((string)moduleSettings["YafMaxPosts"])
                                         : 10;
                }
                catch (Exception)
                {
                    this.maxPosts = 10;
                }

                if (!string.IsNullOrEmpty((string)moduleSettings["YafUseRelativeTime"]))
                {
                    this.useRelativeTime = true;

                    bool.TryParse((string)moduleSettings["YafUseRelativeTime"], out this.useRelativeTime);
                }
                else
                {
                    this.useRelativeTime = true;
                }

                if (!string.IsNullOrEmpty((string)moduleSettings["YafWhatsNewHeader"]))
                {
                    this.headerTemplate = (string)moduleSettings["YafWhatsNewHeader"];
                }
                else
                {
                    this.headerTemplate = "<ul>";
                }

                if (!string.IsNullOrEmpty((string)moduleSettings["YafWhatsNewItemTemplate"]))
                {
                    this.itemTemplate = (string)moduleSettings["YafWhatsNewItemTemplate"];
                }
                else
                {
                    this.itemTemplate = "<li class=\"YafPosts\">[LASTPOSTICON]&nbsp;<strong>[TOPICLINK]</strong>&nbsp;([FORUMLINK])<br />[BYTEXT]&nbsp;[LASTUSERLINK]&nbsp;[LASTPOSTEDDATETIME]</li>";
                }

                if (!string.IsNullOrEmpty((string)moduleSettings["YafWhatsNewFooter"]))
                {
                    this.footerTemplate = (string)moduleSettings["YafWhatsNewFooter"];
                }
                else
                {
                    this.footerTemplate = "</ul>";
                }
            }
            catch (Exception exc)
            {
                // Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Gets the List header.
        /// </summary>
        /// <returns>Returns the html header.</returns>
        private string GetHeader()
        {
            return this.headerTemplate;
        }

        private string ProcessItem(RepeaterItemEventArgs e)
        {
            var currentRow = (DataRowView)e.Item.DataItem;

            string sMessageUrl =
                this.ResolveUrl(
                    "~/Default.aspx?tabid={1}&g=posts&m={0}#post{0}".FormatWith(
                        currentRow["LastMessageID"], this.yafTabId));

            // make message url...
            if (Classes.Config.EnableURLRewriting)
            {
                sMessageUrl =
                    Globals.ResolveUrl(
                        "~/tabid/{0}/g/posts/m/{1}/{2}.aspx#post{1}".FormatWith(
                            this.yafTabId,
                            currentRow["LastMessageID"],
                            this.GetTopicNameFromMessage(currentRow["LastMessageID"].ToType<int>())));
            }

            // Render [LASTPOSTICON]
            var lastPostedImage = new ThemeImage
                {
                    LocalizedTitlePage = "DEFAULT",
                    LocalizedTitleTag = "GO_LAST_POST",
                    LocalizedTitle = Localization.GetString("LastPost.Text", this.LocalResourceFile),
                    ThemeTag = "TOPIC_NEW",
                    Style = "width:16px;height:16px"
                };

            this.itemTemplate = this.itemTemplate.Replace("[LASTPOSTICON]", lastPostedImage.RenderToString());

            // Render [TOPICLINK]
            var textMessageLink = new HyperLink
                {
                    Text = YafContext.Current.Get<IBadWordReplace>().Replace(currentRow["Topic"].ToString()),
                    NavigateUrl = sMessageUrl
                };
            this.itemTemplate = this.itemTemplate.Replace("[TOPICLINK]", textMessageLink.RenderToString());

            // Render [FORUMLINK]
            var forumLink = new HyperLink
                {
                    Text = currentRow["Forum"].ToString(),
                    NavigateUrl =
                        Classes.Config.EnableURLRewriting
                            ? Globals.ResolveUrl(
                                "~/tabid/{0}/g/topics/f/{1}/{2}.aspx".FormatWith(
                                    this.yafTabId,
                                    currentRow["ForumID"],
                                    this.GetForumName(currentRow["ForumID"].ToType<int>())))
                            : this.ResolveUrl(
                                "~/Default.aspx?tabid={1}&g=topics&f={0}".FormatWith(currentRow["ForumID"], this.yafTabId))
                };

            this.itemTemplate = this.itemTemplate.Replace("[FORUMLINK]", forumLink.RenderToString());

            // Render [BYTEXT]
            this.itemTemplate = this.itemTemplate.Replace(
                "[BYTEXT]", YafContext.Current.Get<IHaveLocalization>().GetText("SEARCH", "BY"));

            // Render [LASTUSERLINK]
            // Just in case...
            if (currentRow["LastUserID"] != DBNull.Value)
            {
                var userName = YafContext.Current.Get<YafBoardSettings>().EnableDisplayName
                                   ? currentRow["LastUserDisplayName"].ToString()
                                   : currentRow["LastUserName"].ToString();

                var lastUserLink = new HyperLink
                    {
                        Text = userName,
                        ToolTip = userName,
                        NavigateUrl =
                            Classes.Config.EnableURLRewriting
                                ? Globals.ResolveUrl(
                                    "~/tabid/{0}/g/profile/u/{1}/{2}.aspx".FormatWith(
                                        this.yafTabId, currentRow["LastUserID"], userName))
                                : this.ResolveUrl(
                                    "~/Default.aspx?tabid={1}&g=profile&u={0}".FormatWith(
                                        currentRow["LastUserID"], this.yafTabId))
                    };


                this.itemTemplate = this.itemTemplate.Replace("[LASTUSERLINK]", lastUserLink.RenderToString());
            }

            // Render [LASTPOSTEDDATETIME]
            var displayDateTime = new DisplayDateTime { DateTime = currentRow["LastPosted"].ToType<DateTime>() };

            this.itemTemplate = this.itemTemplate.Replace("[LASTPOSTEDDATETIME]", displayDateTime.RenderToString());

            return this.itemTemplate;
        }

        /// <summary>
        /// Gets the List footer.
        /// </summary>
        /// <returns>Returns the html footer.</returns>
        private string GetFooter()
        {
            return this.footerTemplate;
        }

        #endregion
    }
}