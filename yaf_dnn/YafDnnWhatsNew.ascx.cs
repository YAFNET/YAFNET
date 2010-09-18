// 
// DotNetNuke® - http://www.dotnetnuke.com 
// Copyright (c) 2002-2010 
// by DotNetNuke Corporation 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions: 
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software. 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE. 
// 

using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;
using YAF.Controls;

namespace YAF.DotNetNuke
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The ViewYAF_LatestPosts class displays the content 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 
    partial class YafDnnWhatsNew : PortalModuleBase
    {
        // Settings
        private int iYafTabId;
        private int iYafModuleId;
        private int iBoardId;
        private int iMaxPosts;

        #region "Event Handlers"

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Page_Load runs when the control is loaded 
        /// </summary> 
        /// ----------------------------------------------------------------------------- 
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadSettings();

            try
            {
                DataTable activeTopics = Controller.DataController.TopicLatest(iBoardId, iMaxPosts, GetYafUserId(), false, true);
               
                LatestPosts.DataSource = activeTopics;
                LatestPosts.DataBind();
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        /// <summary>
        /// Load Module Settings
        /// </summary>
        private void LoadSettings()
        {
            try
            {
                ModuleController objModuleController = new ModuleController();

                Hashtable moduleSettings = objModuleController.GetTabModuleSettings(TabModuleId);


                if (!string.IsNullOrEmpty((string)moduleSettings["YafPage"]))
                {
                    iYafTabId = int.Parse((string)moduleSettings["YafPage"]);
                }
                else
                {
                    // If Module is not Configured show Message or Redirect to Settings Page if Current User has Edit Rights
                    if (IsEditable)
                    {
                        Response.Redirect(
                            ResolveUrl(string.Format("~/tabid/{0}/ctl/Module/ModuleId/{1}/Default.aspx",
                                                     PortalSettings.ActiveTab.TabID, ModuleId)));
                    }
                    else
                    {
                        lInfo.Text = Localization.GetString("NotConfigured.Text", LocalResourceFile);
                        lInfo.Style.Add("font-style", "italic");
                    }
                }

                if (!string.IsNullOrEmpty((string)moduleSettings["YafModuleId"]))
                {
                    iYafModuleId = int.Parse((string)moduleSettings["YafModuleId"]);
                }
                else
                {
                    // If Module is not Configured show Message or Redirect to Settings Page if Current User has Edit Rights
                    if (IsEditable)
                    {
                        Response.Redirect(ResolveUrl(string.Format("~/tabid/{0}/ctl/Module/ModuleId/{1}/Default.aspx", PortalSettings.ActiveTab.TabID, ModuleId)));
                    }
                    else
                    {
                        lInfo.Text = Localization.GetString("NotConfigured.Text", LocalResourceFile);
                        lInfo.Style.Add("font-style", "italic");
                    }
                }

                // Get and Set Board Id
                iBoardId = GetYafBoardId(iYafModuleId);

                if  (iBoardId.Equals(-1))
                {
                    // If Module is not Configured show Message or Redirect to Settings Page if Current User has Edit Rights
                    if (IsEditable)
                    {
                        Response.Redirect(ResolveUrl(string.Format("~/tabid/{0}/ctl/Module/ModuleId/{1}/Default.aspx", PortalSettings.ActiveTab.TabID, ModuleId)));
                    }
                    else
                    {
                        lInfo.Text = Localization.GetString("NotConfigured.Text", LocalResourceFile);
                        lInfo.Style.Add("font-style", "italic");
                    }
                }
                

                try
                {
                    iMaxPosts = !string.IsNullOrEmpty((string)moduleSettings["YafMaxPosts"]) ? int.Parse((string)moduleSettings["YafMaxPosts"]) : 10;
                }
                catch (Exception)
                {
                    iMaxPosts = 10;
                }
            }
            catch (Exception exc)
            {
                //Module failed to load 
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        /// <summary>
        /// Get the Yaf Board id from ModuleId and the Module Settings
        /// </summary>
        /// <param name="iModuleId">The Module id of the Yaf Module Instance</param>
        /// <returns>The Board Id From the Settings</returns>
        private static int GetYafBoardId(int iModuleId)
        {
            ModuleController moduleController = new ModuleController();
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
        /// Get Yaf User Id from the Current DNN User
        /// </summary>
        /// <returns>The Yaf User ID</returns>
        private int GetYafUserId()
        {
            int iYafUserId = 1;

            // Check for user
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return iYafUserId;
            }

            //Get current Dnn user
            UserInfo dnnUserInfo = UserController.GetUserById(PortalSettings.PortalId, UserId);

            //get the user from the membership provider
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
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var currentRow = (DataRowView)e.Item.DataItem;

            // make message url...
            string sMessageUrl = ResolveUrl(string.Format("~/Default.aspx?tabid={1}&g=posts&m={0}#post{0}", currentRow["LastMessageID"], iYafTabId));

            // get the controls
            var textMessageLink = (HyperLink)e.Item.FindControl("TextMessageLink");
            var imageMessageLink = (HyperLink)e.Item.FindControl("ImageMessageLink");
            var lastPostedImage = (ThemeImage)e.Item.FindControl("LastPostedImage");
            var lastUserLink = (HyperLink)e.Item.FindControl("LastUserLink");
            var lastPostedDateLabel = (Label)e.Item.FindControl("LastPostedDateLabel");
            var forumLink = (HyperLink)e.Item.FindControl("ForumLink");

            // populate them...
            textMessageLink.Text = YafServices.BadWordReplace.Replace(currentRow["Topic"].ToString());
            textMessageLink.NavigateUrl = sMessageUrl;
            imageMessageLink.NavigateUrl = sMessageUrl;

            lastPostedImage.LocalizedTitle = Localization.GetString("LastPost.Text", LocalResourceFile);

            // Just in case...
            if (currentRow["LastUserID"] != DBNull.Value)
            {


                string sDisplayName =  UserMembershipHelper.GetUserNameFromID(int.Parse(currentRow["LastUserID"].ToString()));

                lastUserLink.Text = sDisplayName;
                lastUserLink.ToolTip = sDisplayName;

                lastUserLink.NavigateUrl = ResolveUrl(string.Format("~/Default.aspx?tabid={1}&g=profile&u={0}", currentRow["LastUserID"], iYafTabId));
            }

            if (currentRow["LastPosted"] != DBNull.Value)
            {
                lastPostedDateLabel.Text = YafServices.DateTime.FormatDateTimeTopic(currentRow["LastPosted"]);
                lastPostedImage.ThemeTag = (DateTime.Parse(currentRow["LastPosted"].ToString()) > Mession.GetTopicRead(Convert.ToInt32(currentRow["TopicID"])))
                                               ? "ICON_NEWEST"
                                               : "ICON_LATEST";
            }

            forumLink.Text = currentRow["Forum"].ToString();

            forumLink.NavigateUrl = ResolveUrl(string.Format("~/Default.aspx?tabid={1}&g=topics&f={0}", currentRow["ForumID"], iYafTabId));
        } 

        #endregion

    }

}