/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Linq;
    using System.Text;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.EventsArgs;

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

        /// <summary>
        ///   Instance of the style transformation class
        /// </summary>
        private IStyleTransform styleTransform;

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

        /// <summary>
        ///   Gets Refines style string from other skins info
        /// </summary>
        private IStyleTransform TransformStyle =>
            this.styleTransform ?? (this.styleTransform = this.Get<IStyleTransform>());

        /// <summary>
        /// The role rank style table.
        /// </summary>
        private DataTable roleRankStyleTable => this.Get<IDataCache>().GetOrSet(
            Constants.Cache.GroupRankStyles,
            () => this.GetRepository<Group>().RankStyleAsDataTable(YafContext.Current.PageBoardID),
            TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ForumStatisticsCacheTimeout));

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
            rawString.Split(',').ForEach(
                chunk =>
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
                                this.GetTextFormatted("ONDATE", this.Get<IDateTime>().FormatDateShort(thanksDate)));
                        }
                    });

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

            this.GetRepository<User>().AddPoints(this.PostData.UserId, this.PageContext.PageUserID, 1);

            this.DataRow["ReputationVoteDate"] = DateTime.UtcNow;

            // Reload UserBox
            this.PageContext.CurrentForumPage.PageCache[Constants.Cache.UserBoxes] = null;

            this.DataRow["Points"] = this.DataRow["Points"].ToType<int>() + 1;

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "REP_VOTE_UP_MSG",
                    this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"]
                            .ToString())),
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

            this.GetRepository<User>().RemovePoints(this.PostData.UserId, this.PageContext.PageUserID, 1);

            this.DataRow["ReputationVoteDate"] = DateTime.UtcNow;

            // Reload UserBox
            this.PageContext.CurrentForumPage.PageCache[Constants.Cache.UserBoxes] = null;

            this.DataRow["Points"] = this.DataRow["Points"].ToType<int>() - 1;

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "REP_VOTE_DOWN_MSG",
                    this.Get<HttpServerUtilityBase>().HtmlEncode(
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"]
                            .ToString())),
                MessageTypes.success);
        }

        /// <summary>
        /// Get the User Groups
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetUserRoles()
        {
            var filler = string.Empty;

            if (!this.Get<YafBoardSettings>().ShowGroups)
            {
                return filler;
            }

            const string StyledNick = @"<span class=""YafGroup_{0}"" style=""{1}"">{0}</span>";

            var groupsText = new StringBuilder(500);

            var first = true;
            var hasRole = false;
            string roleStyle = null;

            var userName = this.DataRow["IsGuest"].ToType<bool>()
                               ? UserMembershipHelper.GuestUserName
                               : this.DataRow["UserName"].ToString();

            RoleMembershipHelper.GetRolesForUser(userName).ForEach(
                role =>
                    {
                        var role1 = role;

                        foreach (var dataRow in this.roleRankStyleTable.Rows.Cast<DataRow>().Where(
                            row => row["LegendID"].ToType<int>() == 1 && row["Style"] != null
                                                                        && row["Name"].ToString() == role1))
                        {
                            roleStyle = this.TransformStyle.DecodeStyleByString(dataRow["Style"].ToString(), true);
                            break;
                        }

                        if (first)
                        {
                            groupsText.AppendLine(
                                this.Get<YafBoardSettings>().UseStyledNicks
                                    ? string.Format(StyledNick, role, roleStyle)
                                    : role);

                            first = false;
                        }
                        else
                        {
                            if (this.Get<YafBoardSettings>().UseStyledNicks)
                            {
                                groupsText.AppendLine(string.Format(@", " + StyledNick, role, roleStyle));
                            }
                            else
                            {
                                groupsText.AppendFormat(", {0}", role);
                            }
                        }

                        roleStyle = null;
                        hasRole = true;
                    });

            // vzrus: Only a guest normally has no role
            if (!hasRole)
            {
                var dt = this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.GuestGroupsCache,
                    () => this.GetRepository<Group>().MemberAsDataTable(
                        this.PageContext.PageBoardID,
                        this.DataRow["UserID"]),
                    TimeSpan.FromMinutes(60));

                foreach (var guestRole in dt.Rows.Cast<DataRow>().Where(role => role["Member"].ToType<int>() > 0)
                    .Select(role => role["Name"].ToString()))
                {
                    foreach (var dataRow in this.roleRankStyleTable.Rows.Cast<DataRow>().Where(
                        row => row["LegendID"].ToType<int>() == 1 && row["Style"] != null
                                                                    && row["Name"].ToString() == guestRole))
                    {
                        roleStyle = this.TransformStyle.DecodeStyleByString(dataRow["Style"].ToString(), true);
                        break;
                    }

                    groupsText.AppendLine(
                        this.Get<YafBoardSettings>().UseStyledNicks
                            ? string.Format(StyledNick, guestRole, roleStyle)
                            : guestRole);
                    break;
                }
            }

            filler = $"<strong>{this.GetText("GROUPS")}:</strong> {groupsText}";

            // Remove the space before the first comma when multiple groups exist.
            filler = filler.Replace("\r\n,", ",");

            return filler;
        }

        /// <summary>
        /// Get the User Rank
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string GetUserRank()
        {
            var rankStyle = (from DataRow dataRow in this.roleRankStyleTable.Rows
                             where dataRow["LegendID"].ToType<int>() == 2 && dataRow["Style"] != null
                                                                          && dataRow["Name"].ToString()
                                                                          == this.DataRow["RankName"].ToString()
                             select this.TransformStyle.DecodeStyleByString(dataRow["Style"].ToString(), true))
                .FirstOrDefault();

            return
                $"<strong>{this.GetText("RANK")}:</strong> {(this.Get<YafBoardSettings>().UseStyledNicks ? $@"<span class=""YafRank_{this.DataRow["RankName"]}"" style=""{rankStyle}"">{this.DataRow["RankName"]}</span>" : this.DataRow["RankName"].ToString())}";
        }

        /// <summary>
        /// Shows the IP information.
        /// </summary>
        private void ShowIpInfo()
        {
            // Display admin/moderator only info
            if (!this.PageContext.IsAdmin && (!this.Get<YafBoardSettings>().AllowModeratorsViewIPs
                                              || !this.PageContext.ForumModeratorAccess))
            {
                return;
            }

            // We should show IP
            this.IPInfo.Visible = true;
            this.IPHolder.Visible = true;
            var ip = IPHelper.GetIp4Address(this.PostData.DataRow["IP"].ToString());
            this.IPLink1.HRef = string.Format(this.Get<YafBoardSettings>().IPInfoPageURL, ip);
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
                    this.PopMenu1.AddPostBackItem(
                        "toggleuserposts_show",
                        this.GetText("POSTS", "TOGGLEUSERPOSTS_SHOW"),
                        "fa fa-eye");
                }
                else
                {
                    this.PopMenu1.AddPostBackItem(
                        "toggleuserposts_hide",
                        this.GetText("POSTS", "TOGGLEUSERPOSTS_HIDE"),
                        "fa fa-eye-slash");
                }
            }

            var userId = this.PostData.UserId;

            if (this.Get<YafBoardSettings>().EnableBuddyList && this.PageContext.PageUserID != userId)
            {
                // Should we add the "Add Buddy" item?
                if (!this.Get<IBuddy>().IsBuddy(userId, false) && !this.PageContext.IsGuest
                                                               && !this.GetRepository<User>()
                                                                   .GetSingle(u => u.ID == userId).Block
                                                                   .BlockFriendRequests)
                {
                    this.PopMenu1.AddPostBackItem("addbuddy", this.GetText("BUDDY", "ADDBUDDY"), "fa fa-user-plus");
                }
                else if (this.Get<IBuddy>().IsBuddy(userId, true) && !this.PageContext.IsGuest)
                {
                    // Are the users approved buddies? Add the "Remove buddy" item.
                    this.PopMenu1.AddClientScriptItemWithPostback(
                        this.GetText("BUDDY", "REMOVEBUDDY"),
                        "removebuddy",
                        $"if (confirm('{this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE")}')) {{postbackcode}}",
                        "fa fa-user-times");
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
                this.ShowHideIgnoredUserPost.Visible = false;
                this.MessageRow.CssClass = "collapse show";
            }
            else if (this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                this.MessageRow.CssClass = "collapse";
                this.ShowHideIgnoredUserPost.Visible = true;
            }
            else if (!this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                this.MessageRow.CssClass = "collapse show";
            }

            this.Edit.Visible = !this.PostData.PostDeleted && this.PostData.CanEditPost && !this.PostData.IsLocked;
            this.Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.postmessage,
                "m={0}",
                this.PostData.MessageId);
            this.MovePost.Visible = this.PageContext.ForumModeratorAccess && !this.PostData.IsLocked;
            this.MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.movemessage,
                "m={0}",
                this.PostData.MessageId);
            this.Delete.Visible = !this.PostData.PostDeleted && this.PostData.CanDeletePost && !this.PostData.IsLocked;
            this.Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.deletemessage,
                "m={0}&action=delete",
                this.PostData.MessageId);
            this.UnDelete.Visible = this.PostData.CanUnDeletePost && !this.PostData.IsLocked;
            this.UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.deletemessage,
                "m={0}&action=undelete",
                this.PostData.MessageId);

            this.Quote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;
            this.MultiQuote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;

            this.Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped(
                ForumPages.postmessage,
                "t={0}&f={1}&q={2}",
                this.PageContext.PageTopicID,
                this.PageContext.PageForumID,
                this.PostData.MessageId);

            if (this.MultiQuote.Visible)
            {
                this.MultiQuote.Attributes.Add(
                    "onclick",
                    $"handleMultiQuoteButton(this, '{this.PostData.MessageId}', '{this.PostData.TopicId}')");

                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "MultiQuoteButtonJs",
                    JavaScriptBlocks.MultiQuoteButtonJs);
                YafContext.Current.PageElements.RegisterJsBlockStartup(
                    "MultiQuoteCallbackSuccessJS",
                    JavaScriptBlocks.MultiQuoteCallbackSuccessJs);

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

            YafContext.Current.PageElements.RegisterJsBlockStartup(
                "asynchCallFailedJs",
                "function CallFailed(res){console.log(res);  }");

            this.FormatThanksRow();

            this.ShowIpInfo();

            this.panMessage.CssClass = "col";

            var userId = this.PostData.UserId;

            var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(userId);
            var displayName = this.Get<YafBoardSettings>().EnableDisplayName
                                  ? UserMembershipHelper.GetDisplayNameFromID(userId)
                                  : UserMembershipHelper.GetUserNameFromID(userId);

            if (avatarUrl.IsSet())
            {
                this.Avatar.Visible = true;
                this.Avatar.AlternateText = displayName;
                this.Avatar.ToolTip = displayName;
                this.Avatar.ImageUrl = avatarUrl;
            }
            else
            {
                this.Avatar.Visible = false;
            }
        }

        /// <summary>
        /// Add Reputation Controls to the User PopMenu
        /// </summary>
        private void AddReputationControls()
        {
            if (this.PageContext.PageUserID != this.PostData.UserId
                && this.Get<YafBoardSettings>().EnableUserReputation && !this.IsGuest && !this.PageContext.IsGuest)
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
                if (this.DataRow["Points"].ToType<int>() > 0 && this.Get<YafBoardSettings>().ReputationAllowNegative)
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
                "'<a class=\"btn btn-link\" href=\"javascript:addThanks(' + response.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + response.Title + '><span><i class=\"fas fa-heart text-danger fa-fw\"></i>&nbsp;' + response.Text + '</span></a>'";

            const string RemoveThankBoxHTML =
                "'<a class=\"btn btn-link\" href=\"javascript:removeThanks(' + response.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + response.Title + '><span><i class=\"far fa-heart fa-fw\"></i>&nbsp;' + response.Text + '</span></a>'";

            var thanksJs = JavaScriptBlocks.AddThanksJs(RemoveThankBoxHTML) + Environment.NewLine
                                                                            + JavaScriptBlocks.RemoveThanksJs(
                                                                                AddThankBoxHTML);

            YafContext.Current.PageElements.RegisterJsBlockStartup("ThanksJs", thanksJs);

            this.Thank.Visible = this.PostData.CanThankPost && !this.PageContext.IsGuest
                                                            && this.Get<YafBoardSettings>().EnableThanksMod;

            if (Convert.ToBoolean(this.DataRow["IsThankedByUser"]))
            {
                this.Thank.NavigateUrl = $"javascript:removeThanks({this.DataRow["MessageID"]});";
                this.Thank.TextLocalizedTag = "BUTTON_THANKSDELETE";
                this.Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
                this.Thank.Icon = "heart";
                this.Thank.IconCssClass = "far";
            }
            else
            {
                this.Thank.NavigateUrl = $"javascript:addThanks({this.DataRow["MessageID"]});";
                this.Thank.TextLocalizedTag = "BUTTON_THANKS";
                this.Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
                this.Thank.Icon = "heart";
                this.Thank.IconCssClass = "fas";
                this.Thank.IconColor = "text-danger";
            }

            var thanksNumber = this.DataRow["MessageThanksNumber"].ToType<int>();

            if (thanksNumber == 0)
            {
                return;
            }

            var username = this.HtmlEncode(
                this.Get<YafBoardSettings>().EnableDisplayName
                    ? UserMembershipHelper.GetDisplayNameFromID(this.PostData.UserId)
                    : UserMembershipHelper.GetUserNameFromID(this.PostData.UserId));

            var thanksLabelText = thanksNumber == 1
                                      ? string.Format(this.Get<ILocalization>().GetText("THANKSINFOSINGLE"), username)
                                      : string.Format(
                                          this.Get<ILocalization>().GetText("THANKSINFO"),
                                          thanksNumber,
                                          username);

            this.ThanksDataLiteral.Text =
                $"<i class=\"fa fa-heart\" style=\"color:#e74c3c\"></i>&nbsp;{thanksLabelText}";

            this.ThanksDataLiteral.Visible = true;

            this.thanksDataExtendedLiteral.Text = this.FormatThanksInfo(this.DataRow["ThanksInfo"].ToString());
            this.thanksDataExtendedLiteral.Visible = true;
        }

        /// <summary>
        /// The pop menu 1_ item click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PopEventArgs"/> instance containing the event data.</param>
        private void PopMenu1_ItemClick([NotNull] object sender, [NotNull] PopEventArgs e)
        {
            switch (e.Item)
            {
                case "userprofile":
                    YafBuildLink.Redirect(
                        ForumPages.profile,
                        "u={0}&name={1}",
                        this.PostData.UserId,
                        this.DataRow[this.Get<YafBoardSettings>().EnableDisplayName ? "DisplayName" : "UserName"]);
                    break;
                case "lastposts":
                    YafBuildLink.Redirect(
                        ForumPages.search,
                        "postedby={0}",
                        this.Get<YafBoardSettings>().EnableDisplayName
                            ? this.DataRow["DisplayName"]
                            : this.DataRow["UserName"]);
                    break;
                case "addbuddy":
                    var strBuddyRequest = this.Get<IBuddy>().AddRequest(this.PostData.UserId);

                    if (Convert.ToBoolean(strBuddyRequest[1].ToType<int>()))
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted("NOTIFICATION_BUDDYAPPROVED_MUTUAL", strBuddyRequest[0]),
                            MessageTypes.success);

                        this.PopMenu1.RemovePostBackItem("addbuddy");

                        this.PopMenu1.AddClientScriptItemWithPostback(
                            this.GetText("BUDDY", "REMOVEBUDDY"),
                            "removebuddy",
                            $"if (confirm('{this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE")}')) {{postbackcode}}",
                            "fa fa-user-times");
                    }
                    else
                    {
                        this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_BUDDYREQUEST"));
                    }

                    break;
                case "removebuddy":
                    {
                        this.Get<IBuddy>().Remove(this.PostData.UserId);

                        this.PopMenu1.RemovePostBackItem("removebuddy");

                        this.PopMenu1.AddPostBackItem("addbuddy", this.GetText("BUDDY", "ADDBUDDY"), "fa fa-user-plus");

                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted(
                                "REMOVEBUDDY_NOTIFICATION",
                                this.Get<IBuddy>().Remove(this.PostData.UserId)),
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