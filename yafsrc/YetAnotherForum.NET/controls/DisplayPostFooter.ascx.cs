/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The display post footer.
    /// </summary>
    public partial class DisplayPostFooter : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The current Post Data for this post.
        /// </summary>
        private PostDataHelperWrapper _postDataHelperWrapper;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the DataRow.
        /// </summary>
        [CanBeNull]
        public DataRow DataRow
        {
            get
            {
                return this._postDataHelperWrapper.DataRow;
            }

            set
            {
                this._postDataHelperWrapper = new PostDataHelperWrapper(value);
            }
        }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        public bool IsGuest
        {
            get
            {
                return this.PostData == null || UserMembershipHelper.IsGuestUser(this.PostData.UserId);
            }
        }

        /// <summary>
        ///   Gets access Post Data helper functions.
        /// </summary>
        public PostDataHelperWrapper PostData
        {
            get
            {
                return this._postDataHelperWrapper;
            }
        }

        /// <summary>
        ///   Gets the Provides access to the Toggle Post button.
        /// </summary>
        public ThemeButton TogglePost
        {
            get
            {
                return this.btnTogglePost;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.DisplayPostFooter_PreRender;
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
        }

        /// <summary>
        /// The setup theme button with link.
        /// </summary>
        /// <param name="thisButton">
        /// The this button.
        /// </param>
        /// <param name="linkUrl">
        /// The link url.
        /// </param>
        protected void SetupThemeButtonWithLink([NotNull] ThemeButton thisButton, [NotNull] string linkUrl)
        {
            if (linkUrl.IsSet())
            {
                string link = linkUrl.Replace("\"", string.Empty);
                if (!link.ToLower().StartsWith("http"))
                {
                    link = "http://{0}".FormatWith(link);
                }

                thisButton.NavigateUrl = link;
                thisButton.Attributes.Add("target", "_blank");
                if (this.Get<YafBoardSettings>().UseNoFollowLinks)
                {
                    thisButton.Attributes.Add("rel", "nofollow");
                }
            }
            else
            {
                thisButton.NavigateUrl = string.Empty;
            }
        }

        /// <summary>
        /// The display post footer_ pre render.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void DisplayPostFooter_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            // report posts
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ReportPostPermissions)
                && !this.PostData.PostDeleted)
            {
                if (this.PageContext.IsGuest || (!this.PageContext.IsGuest && this.PageContext.User != null))
                {
                    this.ReportPost.Visible = true;

                    this.ReportPost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                        ForumPages.reportpost, "m={0}", this.PostData.MessageId);
                }
            }

            string userName = this.Get<YafBoardSettings>().EnableDisplayName
                                  ? this.DataRow["DisplayName"].ToString()
                                  : this.DataRow["UserName"].ToString();

            userName = this.Get<HttpServerUtilityBase>().HtmlEncode(userName);

            // albums link
            if (this.PostData.UserId != this.PageContext.PageUserID && !this.PostData.PostDeleted
                && this.PageContext.User != null && this.Get<YafBoardSettings>().EnableAlbum)
            {
                var numAlbums =
                    this.Get<IDataCache>().GetOrSet<int?>(
                        Constants.Cache.AlbumCountUser.FormatWith(this.PostData.UserId),
                        () =>
                            {
                                DataTable usrAlbumsData = LegacyDb.user_getalbumsdata(
                                    this.PostData.UserId, YafContext.Current.PageBoardID);
                                return usrAlbumsData.GetFirstRowColumnAsValue<int?>("NumAlbums", null);
                            },
                        TimeSpan.FromMinutes(5));

                this.Albums.Visible = numAlbums.HasValue && numAlbums > 0;
                this.Albums.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.albums, "u={0}", this.PostData.UserId);
                this.Albums.ParamTitle0 = userName;
            }

            // private messages
            this.Pm.Visible = this.PostData.UserId != this.PageContext.PageUserID && !this.IsGuest
                              && !this.PostData.PostDeleted && this.PageContext.User != null
                              && this.Get<YafBoardSettings>().AllowPrivateMessages && !this.PostData.IsSponserMessage;
            this.Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", this.PostData.UserId);
            this.Pm.ParamTitle0 = userName;

            // emailing
            this.Email.Visible = this.PostData.UserId != this.PageContext.PageUserID && !this.IsGuest
                                 && !this.PostData.PostDeleted && this.PageContext.User != null
                                 && this.Get<YafBoardSettings>().AllowEmailSending && !this.PostData.IsSponserMessage;
            this.Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_email, "u={0}", this.PostData.UserId);
            this.Email.ParamTitle0 = userName;

            // home page
            this.Home.Visible = !this.PostData.PostDeleted && this.PostData.UserProfile.Homepage.IsSet();
            this.SetupThemeButtonWithLink(this.Home, this.PostData.UserProfile.Homepage);
            this.Home.ParamTitle0 = userName;

            // blog page
            this.Blog.Visible = !this.PostData.PostDeleted && this.PostData.UserProfile.Blog.IsSet();
            this.SetupThemeButtonWithLink(this.Blog, this.PostData.UserProfile.Blog);
            this.Blog.ParamTitle0 = userName;

            if (!this.PostData.PostDeleted && this.PageContext.User != null
                && (this.PostData.UserId != this.PageContext.PageUserID))
            {
                // MSN
                this.Msn.Visible = this.PostData.UserProfile.MSN.IsSet();
                this.Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_msn, "u={0}", this.PostData.UserId);
                this.Msn.ParamTitle0 = userName;

                // Yahoo IM
                this.Yim.Visible = this.PostData.UserProfile.YIM.IsSet();
                this.Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_yim, "u={0}", this.PostData.UserId);
                this.Yim.ParamTitle0 = userName;

                // AOL IM
                this.Aim.Visible = this.PostData.UserProfile.AIM.IsSet();
                this.Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_aim, "u={0}", this.PostData.UserId);
                this.Aim.ParamTitle0 = userName;

                // ICQ
                this.Icq.Visible = this.PostData.UserProfile.ICQ.IsSet();
                this.Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_icq, "u={0}", this.PostData.UserId);
                this.Icq.ParamTitle0 = userName;

                // XMPP
                this.Xmpp.Visible = this.PostData.UserProfile.XMPP.IsSet();
                this.Xmpp.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.im_xmpp, "u={0}", this.PostData.UserId);
                this.Xmpp.ParamTitle0 = userName;

                // Skype
                this.Skype.Visible = this.PostData.UserProfile.Skype.IsSet();
                this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                    ForumPages.im_skype, "u={0}", this.PostData.UserId);
                this.Skype.ParamTitle0 = userName;
            }

            var loadHoverCardJs = false;

            // Facebook
            if (this.PostData.UserProfile.Facebook.IsSet())
            {
                this.Facebook.Visible = this.PostData.UserProfile.Facebook.IsSet();

                if (this.PostData.UserProfile.Facebook.IsSet())
                {
                    this.Facebook.NavigateUrl =
                        ValidationHelper.IsNumeric(this.PostData.UserProfile.Facebook)
                                                ? "https://www.facebook.com/profile.php?id={0}".FormatWith(
                                                    this.PostData.UserProfile.Facebook)
                                                : this.PostData.UserProfile.Facebook;
                }

                this.Facebook.ParamTitle0 = userName;

                if (this.Get<YafBoardSettings>().EnableUserInfoHoverCards)
                {
                    this.Facebook.Attributes.Add("data-hovercard", this.PostData.UserProfile.Facebook);
                    this.Facebook.CssClass += " Facebook-HoverCard";

                    loadHoverCardJs = true;
                }
            }

            // Twitter
            if (this.PostData.UserProfile.Twitter.IsSet())
            {
                this.Twitter.Visible = this.PostData.UserProfile.Twitter.IsSet();
                this.Twitter.NavigateUrl = "http://twitter.com/{0}".FormatWith(this.PostData.UserProfile.Twitter);
                this.Twitter.ParamTitle0 = userName;

                if (this.Get<YafBoardSettings>().EnableUserInfoHoverCards)
                {
                    this.Twitter.Attributes.Add("data-hovercard", this.PostData.UserProfile.Twitter);
                    this.Twitter.CssClass += " Twitter-HoverCard";

                    loadHoverCardJs = true;
                }
            }

            // Google+
            if (this.PostData.UserProfile.Google.IsSet())
            {
                this.Google.Visible = this.PostData.UserProfile.Google.IsSet();
                this.Google.NavigateUrl = this.PostData.UserProfile.Google;
                this.Google.ParamTitle0 = userName;
            }

            if (!loadHoverCardJs || !this.Get<YafBoardSettings>().EnableUserInfoHoverCards)
            {
                return;
            }

            var hoverCardLoadJs = new StringBuilder();

            if (this.Facebook.Visible)
            {
                hoverCardLoadJs.Append(
                    JavaScriptBlocks.HoverCardLoadJs(
                        ".Facebook-HoverCard",
                        "Facebook",
                        this.GetText("DEFAULT", "LOADING_FB_HOVERCARD"),
                        this.GetText("DEFAULT", "ERROR_FB_HOVERCARD")));
            }

            if (this.Twitter.Visible)
            {
                hoverCardLoadJs.Append(
                    JavaScriptBlocks.HoverCardLoadJs(
                        ".Twitter-HoverCard",
                        "Twitter",
                        this.GetText("DEFAULT", "LOADING_TWIT_HOVERCARD"),
                        this.GetText("DEFAULT", "ERROR_TWIT_HOVERCARD")));
            }

            // Setup Hover Card JS
            YafContext.Current.PageElements.RegisterJsBlockStartup("hovercardjs", hoverCardLoadJs.ToString());
        }

        #endregion
    }
}