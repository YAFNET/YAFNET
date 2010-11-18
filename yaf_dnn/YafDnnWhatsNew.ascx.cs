/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
    using System.Text;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using global::DotNetNuke.Common;
    using global::DotNetNuke.Entities.Modules;
    using global::DotNetNuke.Entities.Users;
    using global::DotNetNuke.Framework;
    using global::DotNetNuke.Services.Exceptions;
    using global::DotNetNuke.Services.Localization;

    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.Utils;
    using YAF.Controls;
    using YAF.Utilities;

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
        ///   The i board id.
        /// </summary>
        private int iBoardId;

        /// <summary>
        ///   The i max posts.
        /// </summary>
        private int iMaxPosts;

        /// <summary>
        ///   The yaf module id.
        /// </summary>
        private int iYafModuleId;

        /// <summary>
        ///   The yaf tab id.
        /// </summary>
        private int iYafTabId;

        #endregion

        #region Methods

        /// <summary>
        /// The clean string for url.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The clean string for url.
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
        /// The get cache name.
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
        /// The get forum name.
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
                DataTable list = DB.forum_simplelist(this.LowRange(id), CacheSize);

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
        /// The get topic name.
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
                DataTable list = DB.topic_simplelist(this.LowRange(id), CacheSize);

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
        /// The get topic name from message.
        /// </returns>
        protected string GetTopicNameFromMessage(int id)
        {
            const string Type = "Message";
            const string PrimaryKey = "MessageID";

            DataRow row = this.GetDataRowFromCache(Type, id);

            if (row == null)
            {
                // get the section desired...
                DataTable list = DB.message_simplelist(this.LowRange(id), CacheSize);

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
            // populate the controls here...
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var currentRow = (DataRowView)e.Item.DataItem;

            string sMessageUrl =
                this.ResolveUrl(
                    "~/Default.aspx?tabid={1}&g=posts&m={0}#post{0}".FormatWith(
                        currentRow["LastMessageID"], this.iYafTabId));

            // make message url...
            if (Config.EnableURLRewriting)
            {
                sMessageUrl =
                    Globals.ResolveUrl(
                        "~/tabid/{0}/g/posts/m/{1}/{2}.aspx#post{1}".FormatWith(
                            this.iYafTabId, 
                            currentRow["LastMessageID"], 
                            this.GetTopicNameFromMessage(currentRow["LastMessageID"].ToType<int>())));
            }

            // get the controls
            var textMessageLink = (HyperLink)e.Item.FindControl("TextMessageLink");
            var imageMessageLink = (HyperLink)e.Item.FindControl("ImageMessageLink");
            var lastPostedImage = (ThemeImage)e.Item.FindControl("LastPostedImage");
            var lastUserLink = (HyperLink)e.Item.FindControl("LastUserLink");
            var forumLink = (HyperLink)e.Item.FindControl("ForumLink");

            // populate them...
            textMessageLink.Text = YafContext.Current.Get<YafBadWordReplace>().Replace(currentRow["Topic"].ToString());
            textMessageLink.NavigateUrl = sMessageUrl;
            imageMessageLink.NavigateUrl = sMessageUrl;

            lastPostedImage.LocalizedTitle = Localization.GetString("LastPost.Text", this.LocalResourceFile);

            // Just in case...
            if (currentRow["LastUserID"] != DBNull.Value)
            {
                var sDisplayName = UserMembershipHelper.GetDisplayNameFromID((int)currentRow["LastUserID"]);

                lastUserLink.Text = sDisplayName;
                lastUserLink.ToolTip = sDisplayName;

                if (Config.EnableURLRewriting)
                {
                    lastUserLink.NavigateUrl =
                        Globals.ResolveUrl(
                            "~/tabid/{0}/g/profile/u/{1}/{2}.aspx".FormatWith(
                                this.iYafTabId, currentRow["LastUserID"], sDisplayName));
                }
                else
                {
                    lastUserLink.NavigateUrl =
                        this.ResolveUrl(
                            "~/Default.aspx?tabid={1}&g=profile&u={0}".FormatWith(
                                currentRow["LastUserID"], this.iYafTabId));
                }
            }

            if (currentRow["LastPosted"] != DBNull.Value)
            {
                lastPostedImage.ThemeTag = (DateTime.Parse(currentRow["LastPosted"].ToString()) >
                                            YafContext.Current.Get<YafSession>().GetTopicRead(
                                                currentRow["TopicID"].ToType<int>()))
                                               ? "ICON_NEWEST"
                                               : "ICON_LATEST";
            }

            forumLink.Text = currentRow["Forum"].ToString();

            if (Config.EnableURLRewriting)
            {
                forumLink.NavigateUrl =
                    Globals.ResolveUrl(
                        "~/tabid/{0}/g/topics/f/{1}/{2}.aspx".FormatWith(
                            this.iYafTabId, 
                            currentRow["ForumID"], 
                            this.GetForumName(currentRow["ForumID"].ToType<int>())));
            }
            else
            {
                forumLink.NavigateUrl =
                    this.ResolveUrl(
                        "~/Default.aspx?tabid={1}&g=topics&f={0}".FormatWith(currentRow["ForumID"], this.iYafTabId));
            }
        }

        /// <summary>
        /// The low range.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
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

                ScriptManager.RegisterStartupScript(this, csType, "timeagoloadjs", JavaScriptBlocks.TimeagoLoadJs, true);
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
                DataTable activeTopics = Controller.DataController.TopicLatest(
                    this.iBoardId, this.iMaxPosts, this.GetYafUserId(), false, true);

                this.LatestPosts.DataSource = activeTopics;
                this.LatestPosts.DataBind();
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
            int iYafUserId = 1;

            // Check for user
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return iYafUserId;
            }

            // Get current Dnn user
            UserInfo dnnUserInfo = UserController.GetUserById(this.PortalSettings.PortalId, this.UserId);

            // get the user from the membership provider
            MembershipUser dnnUser = Membership.GetUser(dnnUserInfo.Username, true);

            if (dnnUser == null)
            {
                return iYafUserId;
            }

            try
            {
                iYafUserId = DB.user_get(1, dnnUser.ProviderUserKey);
            }
            catch (Exception)
            {
                return iYafUserId;
            }

            return iYafUserId;
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
                    this.iYafTabId = int.Parse((string)moduleSettings["YafPage"]);
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
                    this.iYafModuleId = int.Parse((string)moduleSettings["YafModuleId"]);
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
                this.iBoardId = GetYafBoardId(this.iYafModuleId);

                if (this.iBoardId.Equals(-1))
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
                    this.iMaxPosts = !string.IsNullOrEmpty((string)moduleSettings["YafMaxPosts"])
                                         ? int.Parse((string)moduleSettings["YafMaxPosts"])
                                         : 10;
                }
                catch (Exception)
                {
                    this.iMaxPosts = 10;
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
            }
            catch (Exception exc)
            {
                // Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion
    }
}