/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Controls;

using System.Text;

using YAF.Core.Context.Start;
using YAF.Web.Controls;
using YAF.Types.Models;

using ButtonStyle = YAF.Types.Constants.ButtonStyle;

/// <summary>
/// DisplayPost Class.
/// </summary>
public partial class DisplayPost : BaseUserControl
{
    /// <summary>
    ///   The current Post Data for this post.
    /// </summary>
    private PostDataHelperWrapper postDataHelperWrapper;

    /// <summary>
    ///   Gets or sets Current Page Index.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the data source.
    /// </summary>
    [CanBeNull]
    public PagedMessage DataSource { get; set; }

    /// <summary>
    ///   Gets a value indicating whether IsGuest.
    /// </summary>
    public bool IsGuest => this.PostData == null || this.PostData.IsGuest;

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
            if (this.postDataHelperWrapper == null && this.DataSource != null)
            {
                this.postDataHelperWrapper = new PostDataHelperWrapper(this.DataSource);
            }

            return this.postDataHelperWrapper;
        }
    }

    /// <summary>
    /// The on pre render.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnPreRender(EventArgs e)
    {
        if (this.PageBoardContext.IsGuest)
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

        this.Edit.Visible = this.Edit2.Visible =
                                !this.PostData.PostDeleted && this.PostData.CanEditPost && !this.PostData.IsLocked;
        this.Edit.NavigateUrl = this.Edit2.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                    ForumPages.EditMessage,
                                    new { m = this.PostData.MessageId });
        this.MovePost.Visible =
            this.Move.Visible = this.PageBoardContext.ForumModeratorAccess && !this.PostData.IsLocked;
        this.MovePost.NavigateUrl = this.Move.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                        ForumPages.MoveMessage,
                                        new { m = this.PostData.MessageId });
        this.Delete.Visible = this.Delete2.Visible =
                                  !this.PostData.PostDeleted && this.PostData.CanDeletePost
                                                             && !this.PostData.IsLocked;

        this.Delete.NavigateUrl = this.Delete2.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                      ForumPages.DeleteMessage,
                                      new { m = this.PostData.MessageId, action = "delete" });
        this.UnDelete.Visible = this.UnDelete2.Visible = this.PostData.CanUnDeletePost && !this.PostData.IsLocked;
        this.UnDelete.NavigateUrl = this.UnDelete2.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                        ForumPages.DeleteMessage,
                                        new { m = this.PostData.MessageId, action = "undelete" });

        this.Quote.Visible = this.Quote2.Visible = this.Reply.Visible = this.ReplyFooter.Visible =
                                                                            this.QuickReplyLink.Visible = this.Separator1.Visible =
                                                                                !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;

        if (this.Edit2.Visible || this.Move.Visible || this.Delete2.Visible || this.UnDelete2.Visible)
        {
            this.Separator2.Visible = true;
        }

        if (this.PageBoardContext.BoardSettings.UseCustomContextMenu)
        {
            this.MessagePanel.CssClass = "message";

            if (!this.PostData.PostDeleted && this.PostData.CanReply
                                           && !this.PostData.IsLocked)
            {
                this.ContextMenu.Attributes.Add(
                    "data-url",
                    this.Get<LinkBuilder>().GetLink(
                        ForumPages.PostMessage,
                        new { t = this.PageBoardContext.PageTopicID, f = this.PageBoardContext.PageForumID }));

                this.ContextMenu.Attributes.Add(
                    "data-quote",
                    this.GetText("COMMON", "SELECTED_QUOTE"));
            }

            this.ContextMenu.Attributes.Add(
                "data-copy",
                this.GetText("COMMON", "SELECTED_COPY"));

            this.ContextMenu.Attributes.Add(
                "data-search",
                this.GetText("COMMON", "SELECTED_SEARCH"));
        }
        else
        {
            this.ContextMenu.Visible = false;
        }

        this.Quote.IconMobileOnly = true;

        this.ReplyFooter.Text = this.GetText("REPLY");
        this.ReplyFooter.IconMobileOnly = true;

        this.MultiQuote.Visible = !this.PostData.PostDeleted && this.PostData.CanReply && !this.PostData.IsLocked;

        this.Quote.NavigateUrl = this.Quote2.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                     ForumPages.PostMessage,
                                     new { t = this.PageBoardContext.PageTopicID, f = this.PageBoardContext.PageForumID, q = this.PostData.MessageId });

        this.Reply.NavigateUrl = this.ReplyFooter.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                     ForumPages.PostMessage,
                                     new { t = this.PageBoardContext.PageTopicID, f = this.PageBoardContext.PageForumID });

        if (this.MultiQuote.Visible)
        {
            this.MultiQuote.Attributes.Add(
                "onclick",
                $"handleMultiQuoteButton(this, '{this.PostData.MessageId}', '{this.PostData.TopicId}')");

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "MultiQuoteButtonJs",
                JavaScriptBlocks.MultiQuoteButtonJs);
            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                "MultiQuoteCallbackSuccessJS",
                JavaScriptBlocks.MultiQuoteCallbackSuccessJs);

            var icon = new Icon { IconName = "quote-left", IconNameBadge = "plus" };

            this.MultiQuote.Text = $"{icon.RenderToString()}<span class=\"ms-1 d-none d-lg-inline-block\">{this.GetText("BUTTON_MULTI_QUOTE")}</span>";

            this.MultiQuote.ToolTip = this.GetText("BUTTON_MULTI_QUOTE_TT");
        }

        if (this.PageBoardContext.BoardSettings.EnableUserReputation)
        {
            this.AddReputationControls();
        }

        if (this.Edit.Visible || this.Delete.Visible || this.MovePost.Visible)
        {
            this.ManageDropPlaceHolder.Visible = true;
        }
        else
        {
            this.ManageDropPlaceHolder.Visible = false;
        }

        this.FormatThanksRow();

        this.ShowIpInfo();

        this.panMessage.CssClass = "col";

        var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(
            this.PostData.UserId,
            this.DataSource.Avatar,
            this.DataSource.HasAvatarImage);

        var displayName = this.PageBoardContext.BoardSettings.EnableDisplayName
                              ? this.DataSource.DisplayName
                              : this.DataSource.UserName;

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

        // report post
        if (this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ReportPostPermissions)
            && !this.PostData.PostDeleted)
        {
            if (!this.PageBoardContext.IsGuest && this.PageBoardContext.MembershipUser != null)
            {
                this.ReportPost.Visible = this.ReportPost2.Visible = true;

                this.ReportPost.NavigateUrl = this.ReportPost2.NavigateUrl = this.Get<LinkBuilder>().GetLink(
                                                  ForumPages.ReportPost,
                                                  new { m = this.PostData.MessageId });
            }
        }

        // mark post as answer
        if (!this.PostData.PostDeleted && !this.PageBoardContext.IsGuest && this.PageBoardContext.MembershipUser != null
            && this.PageBoardContext.PageUserID.Equals(this.DataSource.TopicOwnerID)
            && !this.PostData.UserId.Equals(this.PageBoardContext.PageUserID))
        {
            this.MarkAsAnswer.Visible = true;

            if (this.PostData.PostIsAnswer)
            {
                this.MarkAsAnswer.TextLocalizedTag = "MARK_ANSWER_REMOVE";
                this.MarkAsAnswer.TitleLocalizedTag = "MARK_ANSWER_REMOVE_TITLE";
                this.MarkAsAnswer.Icon = "minus-square";
                this.MarkAsAnswer.IconColor = "text-danger";
            }
            else
            {
                this.MarkAsAnswer.TextLocalizedTag = "MARK_ANSWER";
                this.MarkAsAnswer.TitleLocalizedTag = "MARK_ANSWER_TITLE";
                this.MarkAsAnswer.Icon = "check-square";
                this.MarkAsAnswer.IconColor = "text-success";
            }
        }

        if (this.ReportPost.Visible == false && this.MarkAsAnswer.Visible == false
                                             && this.ManageDropPlaceHolder.Visible == false)
        {
            this.ToolsHolder.Visible = false;
        }

        if (this.ThanksDataLiteral.Visible == false
            && this.Thank.Visible == false
            && this.Quote.Visible == false && this.MultiQuote.Visible == false)
        {
            this.Footer.Visible = false;
        }

        if (!this.PostData.IsGuest)
        {
            this.UserCustomProfile.Visible = true;

            var customProfile = this.Get<IDataCache>().GetOrSet(
                    string.Format(Constants.Cache.UserCustomProfileData, this.PostData.UserId),
                    () => this.GetRepository<ProfileCustom>().ListByUser(this.PostData.UserId))
                .Where(x => x.Item2.ShowInUserInfo && x.Item1.Value.IsSet());

            this.UserCustomProfile.DataSource = customProfile;
            this.UserCustomProfile.DataBind();
        }

        base.OnPreRender(e);
    }

    /// <summary>
    /// Marks as answer click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void MarkAsAnswerClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        var messageFlags = new MessageFlags(this.PostData.DataRow.Flags) { IsAnswer = true };

        if (this.PostData.PostIsAnswer)
        {
            // Remove Current Message
            messageFlags.IsAnswer = false;

            this.GetRepository<Message>().UpdateFlags(this.PostData.MessageId, messageFlags.BitValue);

            this.GetRepository<Topic>().RemoveAnswerMessage(this.PageBoardContext.PageTopicID);
        }
        else
        {
            // Check for duplicates
            var answerMessageId = this.GetRepository<Topic>().GetAnswerMessage(this.PageBoardContext.PageTopicID);

            if (answerMessageId != null)
            {
                var message = this.GetRepository<Message>().GetById(answerMessageId.Value);

                var oldMessageFlags = new MessageFlags(message.Flags) { IsAnswer = false };

                this.GetRepository<Message>().UpdateFlags(message.ID, oldMessageFlags.BitValue);
            }

            messageFlags.IsAnswer = true;

            this.GetRepository<Topic>().SetAnswerMessage(this.PageBoardContext.PageTopicID, this.PostData.MessageId);

            this.GetRepository<Message>().UpdateFlags(this.PostData.MessageId, messageFlags.BitValue);
        }

        this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
            new {m = this.PostData.MessageId, name = this.PageBoardContext.PageTopicID });
    }

    /// <summary>
    /// Formats the thanks information.
    /// </summary>
    /// <returns>
    /// The format thanks info.
    /// </returns>
    [NotNull]
    protected string FormatThanksInfo()
    {
        var sb = new StringBuilder();

        sb.AppendFormat("<ol id=\"popover-list-{0}\">", this.PostData.MessageId);
        sb.Append("</ol>");

        return sb.ToString();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit([NotNull] EventArgs e)
    {
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
        this.UserProfileLink.UserID = this.PostData.UserId;
        this.UserProfileLink.ReplaceName = this.PageBoardContext.BoardSettings.EnableDisplayName
                                               ? this.DataSource.DisplayName
                                               : this.DataSource.UserName;
        this.UserProfileLink.PostfixText = this.DataSource.IP == "NNTP"
                                               ? this.GetText("EXTERNALUSER")
                                               : string.Empty;
        this.UserProfileLink.Style = this.DataSource.Style;

        this.UserProfileLink.Suspended = this.DataSource.Suspended;

        if (this.IsGuest)
        {
            return;
        }

        // Set up popup menu if it's not a guest.
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
        if (!this.Get<IReputation>().CheckIfAllowReputationVoting(this.DataSource.ReputationVoteDate))
        {
            return;
        }

        this.AddReputation.Visible = false;
        this.RemoveReputation.Visible = false;

        this.GetRepository<User>().AddPoints(this.PostData.UserId, this.PageBoardContext.PageUserID, 1);

        this.DataSource.ReputationVoteDate = DateTime.UtcNow;

        this.DataSource.Points = this.DataSource.Points + 1;

        this.PageBoardContext.Notify(
            this.GetTextFormatted(
                "REP_VOTE_UP_MSG",
                this.Get<HttpServerUtilityBase>().HtmlEncode(
                    this.PageBoardContext.BoardSettings.EnableDisplayName
                        ? this.DataSource.DisplayName
                        : this.DataSource.UserName)),
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
        if (!this.Get<IReputation>().CheckIfAllowReputationVoting(this.DataSource.ReputationVoteDate))
        {
            return;
        }

        this.AddReputation.Visible = false;
        this.RemoveReputation.Visible = false;

        this.GetRepository<User>().RemovePoints(this.PostData.UserId, this.PageBoardContext.PageUserID, 1);

        this.DataSource.ReputationVoteDate = DateTime.UtcNow;

        this.DataSource.Points = this.DataSource.Points - 1;

        this.PageBoardContext.Notify(
            this.GetTextFormatted(
                "REP_VOTE_DOWN_MSG",
                this.Get<HttpServerUtilityBase>().HtmlEncode(
                    this.PageBoardContext.BoardSettings.EnableDisplayName
                        ? this.DataSource.DisplayName
                        : this.DataSource.UserName)),
            MessageTypes.success);
    }

    /// <summary>
    /// Shows the IP information.
    /// </summary>
    private void ShowIpInfo()
    {
        // Display admin/moderator only info
        if (!this.PageBoardContext.IsAdmin && (!this.PageBoardContext.BoardSettings.AllowModeratorsViewIPs
                                               || !this.PageBoardContext.ForumModeratorAccess))
        {
            return;
        }

        // We should show IP
        this.IPInfo.Visible = true;
        this.IPHolder.Visible = true;
        var ip = IPHelper.GetIpAddressAsString(this.PostData.DataRow.IP);
        this.IPLink1.HRef = string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, ip);
        this.IPLink1.Title = this.GetText("COMMON", "TT_IPDETAILS");
        this.IPLink1.InnerText = this.HtmlEncode(ip);
    }

    /// <summary>
    /// Setup the popup menu.
    /// </summary>
    private void SetupPopupMenu()
    {
        this.UserDropHolder.Controls.Add(
            new ThemeButton
                {
                    Type = ButtonStyle.None,
                    Icon = "th-list",
                    TextLocalizedPage = "PAGE",
                    TextLocalizedTag = "SEARCHUSER",
                    CssClass = "dropdown-item",
                    NavigateUrl = this.Get<LinkBuilder>().GetLink(
                        ForumPages.Search,
                        new
                            {
                                postedby = this.PageBoardContext.BoardSettings.EnableDisplayName
                                               ? this.DataSource.DisplayName
                                               : this.DataSource.UserName
                            })
                });
        this.UserDropHolder2.Controls.Add(
            new ThemeButton
                {
                    Type = ButtonStyle.None,
                    Icon = "th-list",
                    TextLocalizedPage = "PAGE",
                    TextLocalizedTag = "SEARCHUSER",
                    CssClass = "dropdown-item",
                    NavigateUrl = this.Get<LinkBuilder>().GetLink(
                        ForumPages.Search,
                        new
                            {
                                postedby = this.PageBoardContext.BoardSettings.EnableDisplayName
                                               ? this.DataSource.DisplayName
                                               : this.DataSource.UserName
                            })
                });

        if (this.PageBoardContext.IsAdmin)
        {
            this.UserDropHolder.Controls.Add(
                new ThemeButton
                    {
                        Type = ButtonStyle.None,
                        Icon = "cogs",
                        TextLocalizedPage = "POSTS",
                        TextLocalizedTag = "EDITUSER",
                        CssClass = "dropdown-item",
                        NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Admin_EditUser, new { u = this.PostData.UserId })
                    });
            this.UserDropHolder2.Controls.Add(
                new ThemeButton
                    {
                        Type = ButtonStyle.None,
                        Icon = "cogs",
                        TextLocalizedPage = "POSTS",
                        TextLocalizedTag = "EDITUSER",
                        CssClass = "dropdown-item",
                        NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Admin_EditUser, new { u = this.PostData.UserId })
                    });
        }

        if (this.PageBoardContext.PageUserID != this.PostData.UserId)
        {
            if (this.Get<IUserIgnored>().IsIgnored(this.PostData.UserId))
            {
                var showButton = new ThemeButton
                                     {
                                         Type = ButtonStyle.None,
                                         Icon = "eye",
                                         TextLocalizedPage = "POSTS",
                                         TextLocalizedTag = "TOGGLEUSERPOSTS_SHOW",
                                         CssClass = "dropdown-item"
                                     };

                showButton.Click += (_, _) =>
                    {
                        this.Get<IUserIgnored>().RemoveIgnored(this.PostData.UserId);
                        this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().RawUrl);
                    };

                this.UserDropHolder.Controls.Add(showButton);
            }
            else
            {
                var hideButton = new ThemeButton
                                     {
                                         Type = ButtonStyle.None,
                                         Icon = "eye-slash",
                                         TextLocalizedPage = "POSTS",
                                         TextLocalizedTag = "TOGGLEUSERPOSTS_HIDE",
                                         CssClass = "dropdown-item"
                                     };

                hideButton.Click += (_, _) =>
                    {
                        this.Get<IUserIgnored>().AddIgnored(this.PostData.UserId);
                        this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().RawUrl);
                    };

                this.UserDropHolder.Controls.Add(hideButton);
            }
        }

        if (this.PageBoardContext.BoardSettings.EnableBuddyList && this.PageBoardContext.PageUserID != this.PostData.UserId)
        {
            var userBlockFlags = new UserBlockFlags(this.DataSource.BlockFlags);

            // Should we add the "Add Buddy" item?
            if (!this.Get<IFriends>().IsBuddy(this.PostData.UserId, false) && !this.PageBoardContext.IsGuest
                                                                           && !userBlockFlags
                                                                               .BlockFriendRequests)
            {
                var addFriendButton = new ThemeButton
                                          {
                                              Type = ButtonStyle.None,
                                              Icon = "user-plus",
                                              TextLocalizedPage = "BUDDY",
                                              TextLocalizedTag = "ADDBUDDY",
                                              CssClass = "dropdown-item"
                                          };

                var userName = this.PageBoardContext.BoardSettings.EnableDisplayName
                                   ? this.DataSource.DisplayName
                                   : this.DataSource.UserName;

                addFriendButton.Click += (_, _) =>
                    {
                        if (this.Get<IFriends>().AddRequest(this.PostData.UserId))
                        {
                            this.PageBoardContext.Notify(
                                this.GetTextFormatted("NOTIFICATION_BUDDYAPPROVED_MUTUAL", userName),
                                MessageTypes.success);

                            this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().RawUrl);
                        }
                        else
                        {
                            this.PageBoardContext.Notify(
                                this.GetText("NOTIFICATION_BUDDYREQUEST"),
                                MessageTypes.success);
                        }
                    };

                this.UserDropHolder.Controls.Add(addFriendButton);
            }
            else if (this.Get<IFriends>().IsBuddy(this.PostData.UserId, true) && !this.PageBoardContext.IsGuest)
            {
                // Are the users approved buddies? Add the "Remove buddy" item.
                var removeFriendButton = new ThemeButton
                                             {
                                                 Type = ButtonStyle.None,
                                                 Icon = "user-times",
                                                 TextLocalizedPage = "BUDDY",
                                                 TextLocalizedTag = "REMOVEBUDDY",
                                                 CssClass = "dropdown-item",
                                                 ReturnConfirmTag = "NOTIFICATION_REMOVE"
                                             };

                removeFriendButton.Click += (_, _) =>
                    {
                        this.Get<IFriends>().Remove(this.PostData.UserId);

                        this.Get<HttpResponseBase>().Redirect(this.Get<HttpRequestBase>().RawUrl);

                        this.PageBoardContext.Notify(
                            this.GetTextFormatted(
                                "REMOVEBUDDY_NOTIFICATION",
                                this.Get<IFriends>().Remove(this.PostData.UserId)),
                            MessageTypes.success);
                    };

                this.UserDropHolder.Controls.Add(removeFriendButton);
            }
        }

        this.UserDropHolder.Controls.Add(new Panel { CssClass = "dropdown-divider" });
        this.UserDropHolder2.Controls.Add(new Panel { CssClass = "dropdown-divider" });
    }

    /// <summary>
    /// Add Reputation Controls to the User PopMenu
    /// </summary>
    private void AddReputationControls()
    {
        if (this.PageBoardContext.PageUserID != this.PostData.UserId && this.PageBoardContext.BoardSettings.EnableUserReputation
                                                                     && !this.IsGuest && !this.PageBoardContext.IsGuest)
        {
            if (!this.Get<IReputation>().CheckIfAllowReputationVoting(this.DataSource.ReputationVoteDate))
            {
                return;
            }

            // Check if the User matches minimal requirements for voting up
            if (this.PageBoardContext.PageUser.Points >= this.PageBoardContext.BoardSettings.ReputationMinUpVoting)
            {
                this.AddReputation.Visible = true;
            }

            // Check if the User matches minimal requirements for voting down
            if (this.PageBoardContext.PageUser.Points < this.PageBoardContext.BoardSettings.ReputationMinDownVoting)
            {
                return;
            }

            // Check if the Value is 0 or Bellow
            if (this.DataSource.Points > 0 && this.PageBoardContext.BoardSettings.ReputationAllowNegative)
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
        if (this.PostData.PostDeleted || this.PostData.IsLocked)
        {
            return;
        }

        // Register Javascript
        var addThankBoxHTML =
            "'<a class=\"btn btn-link\" href=\"javascript:addThanks(' + response.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + response.Title + '><i class=\"fas fa-heart text-danger fa-fw\"></i><span class=\"ms-1 d-none d-lg-inline-block\">' + response.Text + '</span></a>'";

        var removeThankBoxHTML =
            "'<a class=\"btn btn-link\" href=\"javascript:removeThanks(' + response.MessageID + ');\" onclick=\"jQuery(this).blur();\" title=' + response.Title + '><i class=\"far fa-heart fa-fw\"></i><span class=\"ms-1 d-none d-lg-inline-block\">' + response.Text + '</span></a>'";

        var thanksJs =
            $"{JavaScriptBlocks.AddThanksJs(removeThankBoxHTML)}{Environment.NewLine}{JavaScriptBlocks.RemoveThanksJs(addThankBoxHTML)}";

        this.PageBoardContext.PageElements.RegisterJsBlockStartup("ThanksJs", thanksJs);

        this.Thank.Visible = this.PostData.CanThankPost && !this.PageBoardContext.IsGuest;
        this.Thank.IconMobileOnly = true;

        if (this.DataSource.IsThankedByUser)
        {
            this.Thank.NavigateUrl = $"javascript:removeThanks({this.DataSource.MessageID});";

            this.Thank.Text = this.GetText("BUTTON_THANKSDELETE");
            this.Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
            this.Thank.Icon = "heart";
            this.Thank.IconCssClass = "far";
        }
        else
        {
            this.Thank.NavigateUrl = $"javascript:addThanks({this.DataSource.MessageID});";

            this.Thank.Text = this.GetText("BUTTON_THANKS");
            this.Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
            this.Thank.Icon = "heart";
            this.Thank.IconCssClass = "fas";
            this.Thank.IconColor = "text-danger";
        }

        var thanksNumber = this.DataSource.ThanksNumber;

        if (thanksNumber == 0)
        {
            return;
        }

        var username = this.HtmlEncode(
            this.PageBoardContext.BoardSettings.EnableDisplayName ? this.DataSource.DisplayName : this.DataSource.UserName);

        var thanksLabelText = thanksNumber == 1
                                  ? this.Get<ILocalization>().GetTextFormatted("THANKSINFOSINGLE", username)
                                  : this.Get<ILocalization>().GetTextFormatted(
                                      "THANKSINFO",
                                      thanksNumber,
                                      username);

        this.ThanksDataLiteral.Text = $@"<a class=""btn btn-link thanks-popover""
                           data-bs-toggle=""popover""
                           data-bs-trigger=""click hover""
                           data-bs-html=""true""
                           data-messageid=""{this.PostData.MessageId}""
                           data-url=""{BoardInfo.ForumClientFileRoot}{WebApiConfig.UrlPrefix}""
                           title=""{thanksLabelText}""
                           data-bs-content=""{this.FormatThanksInfo().ToJsString()}"">
                           <i class=""fa fa-heart me-1"" style=""color:#e74c3c""></i>+{thanksNumber}
                  </a>";

        this.ThanksDataLiteral.Visible = true;
    }
}