/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Core.Services.Auth;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// DisplayPost Class.
    /// </summary>
    public partial class DisplayPost : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The current Post Data for this post.
        /// </summary>
        private PostDataHelperWrapper postDataHelperWrapper;

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets Current Page Index.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        ///   Gets or sets the DataRow.
        /// </summary>
        [CanBeNull]
        public DataRow DataRow { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether IsAlt.
        /// </summary>
        public bool IsAlt { get; set; }

        /// <summary>
        ///   Gets a value indicating whether IsGuest.
        /// </summary>
        public bool IsGuest => this.PostData == null || UserMembershipHelper.IsGuestUser(this.PostData.UserId);

        /// <summary>
        ///   Gets or sets a value indicating whether IsThreaded.
        /// </summary>
        public bool IsThreaded { get; set; }

        /// <summary>
        ///   Gets or sets Post Count.
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        ///   Gets Post Data helper functions.
        /// </summary>
        public PostDataHelperWrapper PostData
        {
            get
            {
                if (this.postDataHelperWrapper == null && this.DataRow != null)
                {
                    this.postDataHelperWrapper = new PostDataHelperWrapper(this.DataRow);
                }

                return this.postDataHelperWrapper;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the thanks information.
        /// </summary>
        /// <param name="rawString">The raw string.</param>
        /// <returns>
        /// The format thanks info.
        /// </returns>
        [NotNull]
        protected string FormatThanksInfo([NotNull] string rawString)
        {
            var sb = new StringBuilder();

            var showDate = this.Get<YafBoardSettings>().ShowThanksDate;

            // Extract all user IDs, user name's and (If enabled thanks dates) related to this message.
            foreach (var chunk in rawString.Split(','))
            {
                var subChunks = chunk.Split('|');

                var userId = int.Parse(subChunks[0]);
                var thanksDate = DateTime.Parse(subChunks[1]);

                if (sb.Length > 0)
                {
                    sb.Append(",&nbsp;");
                }

                // Get the username related to this User ID
                var displayName = this.Get<IUserDisplayName>().GetName(userId);

                sb.AppendFormat(
                    @"<a id=""{0}"" href=""{1}""><u>{2}</u></a>",
                    userId,
                    YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", userId, displayName),
                    this.Get<HttpServerUtilityBase>().HtmlEncode(displayName));

                // If showing thanks date is enabled, add it to the formatted string.
                if (showDate)
                {
                    sb.AppendFormat(
                        @" {0}",
                        this.GetText("DEFAULT", "ONDATE").FormatWith(this.Get<IDateTime>().FormatDateShort(thanksDate)));
                }
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// The get post class.
        /// </summary>
        /// <returns>
        /// Returns the post class.
        /// </returns>
        [NotNull]
        protected string GetPostClass()
        {
            return this.IsAlt ? "post_alt" : "post";
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.PreRender += this.DisplayPost_PreRender;
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
            if (this.IsGuest)
            {
                return;
            }

            // Set up popup menu if it's not a guest.
            this.PopMenu1.Visible = true;
            this.SetupPopupMenu();
        }

        /// <summary>
        /// Adds the user reputation.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void AddUserReputation(object sender, EventArgs e)
        {
            if (!YafReputation.CheckIfAllowReputationVoting(this.DataRow["ReputationVoteDate"]))
            {
                return;
            }

            this.AddReputation.Visible = false;
            this.RemoveReputation.Visible = false;

            LegacyDb.user_addpoints(this.PostData.UserId, this.PageContext.PageUserID, 1);

            this.DataRow["ReputationVoteDate"] = DateTime.UtcNow;

            // Reload UserBox
            this.PageContext.CurrentForumPage.PageCache[Constants.Cache.UserBoxes] = null;

            this.DataRow["Points"] = this.DataRow["Points"].ToType<int>() + 1;
            this.UserBox1.PageCache = null;

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "REP_VOTE_UP_MSG",
                    this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"].ToString())),
                MessageTypes.success);
        }

        /// <summary>
        /// Removes the user reputation.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void RemoveUserReputation(object sender, EventArgs e)
        {
            if (!YafReputation.CheckIfAllowReputationVoting(this.DataRow["ReputationVoteDate"]))
            {
                return;
            }

            this.AddReputation.Visible = false;
            this.RemoveReputation.Visible = false;

            LegacyDb.user_removepoints(this.PostData.UserId, this.PageContext.PageUserID, 1);

            this.DataRow["ReputationVoteDate"] = DateTime.UtcNow;

            // Reload UserBox
            this.PageContext.CurrentForumPage.PageCache[Constants.Cache.UserBoxes] = null;

            this.DataRow["Points"] = this.DataRow["Points"].ToType<int>() - 1;
            this.UserBox1.PageCache = null;

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                   "REP_VOTE_DOWN_MSG",
                   this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"].ToString())),
                MessageTypes.success);
        }

        /// <summary>
        /// Re-tweets Message thru the Twitter API
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void Retweet_Click(object sender, EventArgs e)
        {
            var twitterName = this.Get<YafBoardSettings>().TwitterUserName.IsSet()
                                  ? "@{0} ".FormatWith(this.Get<YafBoardSettings>().TwitterUserName)
                                  : string.Empty;

            // process message... clean html, strip html, remove bbcode, etc...
            var twitterMsg =
                BBCodeHelper.StripBBCode(
                    HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString((string)this.DataRow["Message"]))).RemoveMultipleWhitespace();

            var topicUrl = YafBuildLink.GetLinkNotEscaped(ForumPages.posts, true, "m={0}#post{0}", this.DataRow["MessageID"]);

            // Send Retweet Directlly thru the Twitter API if User is Twitter User
            if (Config.TwitterConsumerKey.IsSet() && Config.TwitterConsumerSecret.IsSet() &&
                this.Get<IYafSession>().TwitterToken.IsSet() && this.Get<IYafSession>().TwitterTokenSecret.IsSet() &&
                this.Get<IYafSession>().TwitterTokenSecret.IsSet() && this.PageContext.IsTwitterUser)
            {
                var auth = new OAuthTwitter
                {
                    ConsumerKey = Config.TwitterConsumerKey,
                    ConsumerSecret = Config.TwitterConsumerSecret,
                    Token = this.Get<IYafSession>().TwitterToken,
                    TokenSecret = this.Get<IYafSession>().TwitterTokenSecret
                };

                var tweets = new TweetAPI(auth);

                tweets.UpdateStatus(
                    TweetAPI.ResponseFormat.json,
                    this.Server.UrlEncode("RT {1}: {0} {2}".FormatWith(twitterMsg.Truncate(100), twitterName, topicUrl)),
                    string.Empty);
            }
            else
            {
                this.Get<HttpResponseBase>().Redirect(
                    "http://twitter.com/share?url={0}&text={1}".FormatWith(
                        this.Server.UrlEncode(topicUrl),
                        this.Server.UrlEncode(
                            "RT {1}: {0}".FormatWith(twitterMsg.Truncate(100), twitterName))));
            }
        }

        /// <summary>
        /// Shows the IP information.
        /// </summary>
        private void ShowIpInfo()
        {
            // Display admin/moderator only info
            if (!this.PageContext.IsAdmin
                && (!this.Get<YafBoardSettings>().AllowModeratorsViewIPs || !this.PageContext.ForumModeratorAccess))
            {
                return;
            }

            // We should show IP
            this.IPSpan1.Visible = true;
            var ip = IPHelper.GetIp4Address(this.PostData.DataRow["IP"].ToString());
            this.IPLink1.HRef = this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(ip);
            this.IPLink1.Title = this.GetText("COMMON", "TT_IPDETAILS");
            this.IPLink1.InnerText = this.HtmlEncode(ip);
        }

        /// <summary>
        /// Setup the popup menu.
        /// </summary>
        private void SetupPopupMenu()
        {
            this.PopMenu1.ItemClick += this.PopMenu1_ItemClick;
            this.PopMenu1.AddPostBackItem("userprofile", this.GetText("POSTS", "USERPROFILE"), "fa fa-user");

            this.PopMenu1.AddPostBackItem("lastposts", this.GetText("PROFILE", "SEARCHUSER"), "fa fa-th-list");

            if (this.Get<YafBoardSettings>().EnableThanksMod)
            {
                this.PopMenu1.AddPostBackItem("viewthanks", this.GetText("VIEWTHANKS", "TITLE"), "fa fa-heart");
            }

            if (this.PageContext.IsAdmin)
            {
                this.PopMenu1.AddPostBackItem("edituser", this.GetText("POSTS", "EDITUSER"), "fa fa-cogs");
            }

            if (!this.PageContext.IsGuest)
            {
                if (this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
                {
                    this.PopMenu1.AddPostBackItem("toggleuserposts_show", this.GetText("POSTS", "TOGGLEUSERPOSTS_SHOW"), "fa fa-eye");
                }
                else
                {
                    this.PopMenu1.AddPostBackItem("toggleuserposts_hide", this.GetText("POSTS", "TOGGLEUSERPOSTS_HIDE"), "fa fa-eye-slash");
                }
            }

            var userId = this.DataRow["UserID"].ToType<int>();

            if (this.Get<YafBoardSettings>().EnableBuddyList &&
                this.PageContext.PageUserID != userId)
            {
                // Should we add the "Add Buddy" item?
                if (!this.Get<IBuddy>().IsBuddy(userId, false) && !this.PageContext.IsGuest)
                {
                    this.PopMenu1.AddPostBackItem("addbuddy", this.GetText("BUDDY", "ADDBUDDY"), "fa fa-plus");
                }
                else if (this.Get<IBuddy>().IsBuddy(userId, true) && !this.PageContext.IsGuest)
                {
                    // Are the users approved buddies? Add the "Remove buddy" item.
                    this.PopMenu1.AddClientScriptItemWithPostback(
                        this.GetText("BUDDY", "REMOVEBUDDY"),
                        "removebuddy",
                        "if (confirm('{0}')) {1}".FormatWith(
                            this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE"), "{postbackcode}"));
                }
            }

            this.PopMenu1.Attach(this.UserProfileLink);
        }

        /// <summary>
        /// The display post_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DisplayPost_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.IsGuest)
            {
                this.PostFooter.TogglePost.Visible = false;
            }
            else if (this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                this.panMessage.Attributes["style"] = "display:none";
                this.PostFooter.TogglePost.Visible = true;
                this.PostFooter.TogglePost.Attributes["onclick"] =
                    "toggleMessage('{0}'); return false;".FormatWith(this.panMessage.ClientID);
            }
            else if (!this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                this.panMessage.Attributes["style"] = "display:block";
                this.panMessage.Visible = true;
            }

            this.Retweet.Visible = this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowRetweetMessageTo)
                                   && !this.PostData.PostDeleted && !this.PostData.IsLocked;

            this.Edit.Visible = !this.PostData.PostDeleted && this.PostData.CanEditPost && !this.PostData.IsLocked;
            this.Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.postmessage, "m={0}", this.PostData.MessageId);
            this.MovePost.Visible = this.PageContext.ForumModeratorAccess && !this.PostData.IsLocked;
            this.MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.movemessage, "m={0}", this.PostData.MessageId);
            this.Delete.Visible = !this.PostData.PostDeleted && this.PostData.CanDeletePost && !this.PostData.IsLocked;
            this.Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.deletemessage, "m={0}&action=delete", this.PostData.MessageId);
            this.UnDelete.Visible = this.PostData.CanUnDeletePost && !this.PostData.IsLocked;
            this.UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.deletemessage, "m={0}&action=undelete", this.PostData.MessageId);

            this.Quote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;
            this.MultiQuote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;

            this.Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.postmessage,
                "t={0}&f={1}&q={2}",
                this.PageContext.PageTopicID,
                this.PageContext.PageForumID,
                this.PostData.MessageId);

            // setup jQuery and YAF JS...
            YafContext.Current.PageElements.RegisterJsBlock("toggleMessageJs", JavaScriptBlocks.ToggleMessageJs);

            if (this.MultiQuote.Visible)
            {
                this.MultiQuote.Attributes.Add(
                    "onclick",
                    "handleMultiQuoteButton(this, '{0}', '{1}')".FormatWith(
                        this.PostData.MessageId,
                        this.PostData.TopicId));

                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "MultiQuoteButtonJs", JavaScriptBlocks.MultiQuoteButtonJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                  "MultiQuoteCallbackSuccessJS", JavaScriptBlocks.MultiQuoteCallbackSuccessJS);

                this.MultiQuote.Text = this.GetText("BUTTON_MULTI_QUOTE");
                this.MultiQuote.ToolTip = this.GetText("BUTTON_MULTI_QUOTE_TT");
            }

            if (this.Get<YafBoardSettings>().EnableUserReputation)
            {
                this.AddReputationControls();
            }

            if (this.Edit.Visible || this.Delete.Visible || this.MovePost.Visible)
            {
                this.Manage.Visible = true;
            }
            else
            {
                this.Manage.Visible = false;
            }

            YafContext.Current.PageElements.RegisterJsBlockStartup("asynchCallFailedJs", "function CallFailed(res){ alert('Error Occurred'); }");

            this.FormatThanksRow();

            this.ShowIpInfo();
        }

        /// <summary>
        /// Add Reputation Controls to the User PopMenu
        /// </summary>
        private void AddReputationControls()
        {
            if (this.PageContext.PageUserID != this.DataRow["UserID"].ToType<int>() &&
                this.Get<YafBoardSettings>().EnableUserReputation && !this.IsGuest && !this.PageContext.IsGuest)
            {
                if (!YafReputation.CheckIfAllowReputationVoting(this.DataRow["ReputationVoteDate"]))
                {
                    return;
                }

                // Check if the User matches minimal requirements for voting up
                if (this.PageContext.Reputation >= this.Get<YafBoardSettings>().ReputationMinUpVoting)
                {
                    this.AddReputation.Visible = true;
                }

                // Check if the User matches minimal requirements for voting down
                if (this.PageContext.Reputation < this.Get<YafBoardSettings>().ReputationMinDownVoting)
                {
                    return;
                }

                // Check if the Value is 0 or Bellow
                if (this.DataRow["Points"].ToType<int>() > 0 &&
                    this.Get<YafBoardSettings>().ReputationAllowNegative)
                {
                    this.RemoveReputation.Visible = true;
                }
            }
            else
            {
                this.AddReputation.Visible = false;
                this.RemoveReputation.Visible = false;
            }
        }

        /// <summary>
        /// Do thanks row formatting.
        /// </summary>
        private void FormatThanksRow()
        {
            if (!this.Get<YafBoardSettings>().EnableThanksMod)
            {
                return;
            }

            if (this.PostData.PostDeleted || this.PostData.IsLocked)
            {
                return;
            }

            // Register Javascript
            const string AddThankBoxHTML =
                "'<a class=\"yaflittlebutton\" href=\"javascript:addThanks(' + res.d.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + res.d.Title + '><span>' + res.d.Text + '</span></a>'";

            const string RemoveThankBoxHTML =
                "'<a class=\"yaflittlebutton\" href=\"javascript:removeThanks(' + res.d.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + res.d.Title + '><span>' + res.d.Text + '</span></a>'";

            var thanksJs = JavaScriptBlocks.AddThanksJs(RemoveThankBoxHTML) + Environment.NewLine + JavaScriptBlocks.RemoveThanksJs(AddThankBoxHTML);

            YafContext.Current.PageElements.RegisterJsBlockStartup("ThanksJs", thanksJs);

            this.Thank.Visible = this.PostData.CanThankPost && !this.PageContext.IsGuest &&
                                 this.Get<YafBoardSettings>().EnableThanksMod;

            if (Convert.ToBoolean(this.DataRow["IsThankedByUser"]))
            {
                this.Thank.NavigateUrl = "javascript:removeThanks({0});".FormatWith(this.DataRow["MessageID"]);
                this.Thank.TextLocalizedTag = "BUTTON_THANKSDELETE";
                this.Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
            }
            else
            {
                this.Thank.NavigateUrl = "javascript:addThanks({0});".FormatWith(this.DataRow["MessageID"]);
                this.Thank.TextLocalizedTag = "BUTTON_THANKS";
                this.Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
            }

            var thanksNumber = this.DataRow["MessageThanksNumber"].ToType<int>();

            if (thanksNumber == 0)
            {
                return;
            }

            var username =
                this.HtmlEncode(
                    this.Get<YafBoardSettings>().EnableDisplayName
                        ? UserMembershipHelper.GetDisplayNameFromID(this.PostData.UserId)
                        : UserMembershipHelper.GetUserNameFromID(this.PostData.UserId));

            var thanksLabelText = thanksNumber == 1
                                  ? this.Get<ILocalization>().GetText("THANKSINFOSINGLE").FormatWith(username)
                                  : this.Get<ILocalization>().GetText("THANKSINFO").FormatWith(thanksNumber, username);

            this.ThanksDataLiteral.Text =
                "<i class=\"fa fa-heart\" style=\"color:#e74c3c\"></i>&nbsp;{0}".FormatWith(thanksLabelText);

            this.ThanksDataLiteral.Visible = true;

            this.thanksDataExtendedLiteral.Text = this.FormatThanksInfo(this.DataRow["ThanksInfo"].ToString());
            this.thanksDataExtendedLiteral.Visible = true;
        }

        /// <summary>
        /// The pop menu 1_ item click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="YAF.Controls.PopEventArgs"/> instance containing the event data.</param>
        private void PopMenu1_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
        {
            switch (e.Item)
            {
                case "userprofile":
                    YafBuildLink.Redirect(
                        ForumPages.profile,
                        "u={0}&name={1}",
                        this.PostData.UserId,
                        this.Get<YafBoardSettings>().EnableDisplayName
                            ? this.DataRow["DisplayName"]
                            : this.DataRow["UserName"]);
                    break;
                case "lastposts":
                    YafBuildLink.Redirect(
                        ForumPages.search,
                        "postedby={0}",
                        this.Get<YafBoardSettings>().EnableDisplayName ? this.DataRow["DisplayName"] : this.DataRow["UserName"]);
                    break;
                case "addbuddy":
                    this.PopMenu1.RemovePostBackItem("addbuddy");
                    var strBuddyRequest = this.Get<IBuddy>().AddRequest(this.PostData.UserId);
                    if (Convert.ToBoolean(strBuddyRequest[1]))
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("NOTIFICATION_BUDDYAPPROVED_MUTUAL", strBuddyRequest[0]), MessageTypes.success);

                        this.PopMenu1.AddClientScriptItemWithPostback(
                            this.GetText("BUDDY", "REMOVEBUDDY"),
                            "removebuddy",
                            "if (confirm('{0}')) {1}".FormatWith(
                                this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE"), "{postbackcode}"));
                    }
                    else
                    {
                        this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_BUDDYREQUEST"));
                    }

                    break;
                case "removebuddy":
                    {
                        this.PopMenu1.RemovePostBackItem("removebuddy");
                        this.PopMenu1.AddPostBackItem("addbuddy", this.GetText("BUDDY", "ADDBUDDY"));
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted(
                                "REMOVEBUDDY_NOTIFICATION", this.Get<IBuddy>().Remove(this.PostData.UserId)),
                            MessageTypes.success);
                        break;
                    }

                case "edituser":
                    YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", this.PostData.UserId);
                    break;
                case "toggleuserposts_show":
                    this.Get<IUserIgnored>().RemoveIgnored(this.PostData.UserId);
                    this.Response.Redirect(this.Request.RawUrl);
                    break;
                case "toggleuserposts_hide":
                    this.Get<IUserIgnored>().AddIgnored(this.PostData.UserId);
                    this.Response.Redirect(this.Request.RawUrl);
                    break;
                case "viewthanks":
                    YafBuildLink.Redirect(ForumPages.viewthanks, "u={0}", this.PostData.UserId);
                    break;
            }
        }

        #endregion
    }
}