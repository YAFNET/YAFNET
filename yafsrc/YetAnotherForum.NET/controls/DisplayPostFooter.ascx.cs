/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
    using YAF.Types.Interfaces;
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
        /// The create message details.
        /// </summary>
        protected void CreateMessageDetails()
        {
            var sb = new StringBuilder();

            // display admin only info
            if (this.PageContext.IsAdmin ||
                (this.Get<YafBoardSettings>().AllowModeratorsViewIPs && this.PageContext.IsModerator))
            {
                // We should show IP
                this.IPSpan1.Visible = true;
                string ip = IPHelper.GetIp4Address(this.DataRow["IP"].ToString());
                this.IPLink1.HRef = this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(ip);
                this.IPLink1.Title = this.GetText("COMMON", "TT_IPDETAILS");
                this.IPLink1.InnerText = this.HtmlEncode(ip);

                sb.Append(' ');
            }
          
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.DisplayPostFooter_PreRender;
            base.OnInit(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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
                    link = "http://" + link;
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
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DisplayPostFooter_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            // report posts
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ReportPostPermissions) &&
                !this.PostData.PostDeleted)
            {
                if (this.PageContext.IsGuest || (!this.PageContext.IsGuest && this.PageContext.User != null))
                {
                    this.ReportPost.Visible = true;

                    this.ReportPost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.reportpost, "m={0}", this.PostData.MessageId);
                }
            }

            string userName = this.DataRow["DisplayName"].ToString();

            // albums link
            if (this.PostData.UserId != this.PageContext.PageUserID &&
                                  !this.PostData.PostDeleted && this.PageContext.User != null &&
                                  this.Get<YafBoardSettings>().EnableAlbum)
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
                this.Albums.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.albums, "u={0}", this.PostData.UserId);
                this.Albums.ParamTitle0 = userName;
            }

            // private messages
            this.Pm.Visible = this.PostData.UserId != this.PageContext.PageUserID && !this.IsGuest &&
                              !this.PostData.PostDeleted && this.PageContext.User != null &&
                              this.Get<YafBoardSettings>().AllowPrivateMessages && !this.PostData.IsSponserMessage;
            this.Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.pmessage, "u={0}", this.PostData.UserId);
            this.Pm.ParamTitle0 = userName;

            // emailing
            this.Email.Visible = this.PostData.UserId != this.PageContext.PageUserID && !this.IsGuest &&
                                 !this.PostData.PostDeleted && this.PageContext.User != null &&
                                 this.Get<YafBoardSettings>().AllowEmailSending && !this.PostData.IsSponserMessage;
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

            if (!this.PostData.PostDeleted && this.PageContext.User != null &&
                (this.PostData.UserId != this.PageContext.PageUserID))
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
                this.Xmpp.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_xmpp, "u={0}", this.PostData.UserId);
                this.Xmpp.ParamTitle0 = userName;

                // Skype
                this.Skype.Visible = this.PostData.UserProfile.Skype.IsSet();
                this.Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.im_skype, "u={0}", this.PostData.UserId);
                this.Skype.ParamTitle0 = userName;
            }

            // Facebook
            this.Facebook.Visible = this.PostData.UserProfile.Facebook.IsSet();

            if (this.PostData.UserProfile.Facebook.IsSet())
            {
                this.Facebook.NavigateUrl = "https://www.facebook.com/profile.php?id={0}".FormatWith(this.PostData.UserProfile.Facebook);
            }

            this.Facebook.ParamTitle0 = userName;

            // Twitter
            this.Twitter.Visible = this.PostData.UserProfile.Twitter.IsSet();
            this.Twitter.NavigateUrl = "http://twitter.com/{0}".FormatWith(this.PostData.UserProfile.Twitter);
            this.Twitter.ParamTitle0 = userName;

            this.CreateMessageDetails();
        }

        #endregion
    }
}