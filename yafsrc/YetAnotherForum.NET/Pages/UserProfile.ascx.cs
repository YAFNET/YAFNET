/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Pages;

using System.Text;

using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Web.Controls;

/// <summary>
/// The User Profile Page.
/// </summary>
public partial class UserProfile : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "UserProfile" /> class.
    /// </summary>
    public UserProfile()
        : base("PROFILE", ForumPages.UserProfile)
    {
    }

    /// <summary>
    ///   Gets or sets UserId.
    /// </summary>
    public int UserId =>
        this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

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
        // admin or moderator, set edit control to moderator mode...
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.IsForumModerator)
        {
            this.SignatureEditControl.InModeratorMode = true;
        }

        if (!this.IsPostBack)
        {
            this.userGroupsRow.Visible = this.PageBoardContext.BoardSettings.ShowGroupsProfile || this.PageBoardContext.IsAdmin;
        }

        if (this.UserId == 0)
        {
            // No such user exists
            this.Get<LinkBuilder>().AccessDenied();
        }

        this.BindData();
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
            var link = linkUrl.Replace("\"", string.Empty);
            if (!link.ToLower().StartsWith("http"))
            {
                link = $"http://{link}";
            }

            thisButton.NavigateUrl = link;
            thisButton.Attributes.Add("target", "_blank");
            if (this.PageBoardContext.BoardSettings.UseNoFollowLinks)
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
    /// Add user as Buddy
    /// </summary>lnkBuddy
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
    protected void lnk_AddBuddy([NotNull] object sender, [NotNull] CommandEventArgs e)
    {
        if (e.CommandArgument.ToString() == "addbuddy")
        {
            this.PageBoardContext.Notify(
                this.Get<IFriends>().AddRequest(this.UserId)
                    ? this.GetTextFormatted(
                        "NOTIFICATION_BUDDYAPPROVED_MUTUAL",
                        this.Get<IUserDisplayName>().GetNameById(this.UserId))
                    : this.GetText("NOTIFICATION_BUDDYREQUEST"),
                MessageTypes.success);
        }
        else
        {
            this.PageBoardContext.Notify(
                this.GetTextFormatted("REMOVEBUDDY_NOTIFICATION", this.Get<IFriends>().Remove(this.UserId)),
                MessageTypes.success);
        }

        this.BindData();
    }

    /// <summary>
    /// add page links.
    /// </summary>
    /// <param name="userDisplayName">
    /// The user display name.
    /// </param>
    private void AddPageLinks([NotNull] string userDisplayName)
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("MEMBERS"),
            this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.MembersListViewPermissions)
                ? this.Get<LinkBuilder>().GetLink(ForumPages.Members)
                : null);
        this.PageBoardContext.PageLinks.AddLink(userDisplayName, string.Empty);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        var user = this.Get<IAspNetUsersHelper>().GetBoardUser(this.UserId);

        if (user == null || user.Item1.ID == 0)
        {
            // No such user exists or this is an nntp user ("0")
            this.Get<LinkBuilder>().AccessDenied();
        }

        // populate user information controls...
        // Is BuddyList feature enabled?
        if (this.PageBoardContext.BoardSettings.EnableBuddyList)
        {
            this.SetupBuddyList(this.UserId, user);
        }
        else
        {
            // BuddyList feature is disabled. don't show any link.
            this.lnkBuddy.Visible = false;
            this.BuddyCard.Visible = false;
        }

        var userNameOrDisplayName = this.HtmlEncode(user.Item1.DisplayOrUserName());

        this.SetupUserProfileInfo(user);

        this.AddPageLinks(userNameOrDisplayName);

        this.SetupUserStatistics(user);

        this.SetupUserLinks(user, userNameOrDisplayName);

        this.SetupAvatar(user.Item1);

        var groups = this.GetRepository<UserGroup>().List(user.Item1.ID);

        this.Groups.DataSource = groups;

        this.ModerateTab.Visible = this.PageBoardContext.IsAdmin || this.PageBoardContext.IsForumModerator;

        this.SignatureEditControl.Visible = this.ModerateTab.Visible;

        this.AdminUserButton.Visible = this.PageBoardContext.IsAdmin;

        var userPosts = this.GetRepository<Message>().GetAllUserMessagesWithAccess(
            this.PageBoardContext.PageBoardID,
            this.UserId,
            this.PageBoardContext.PageUserID,
            10);

        this.LastPosts.DataSource = userPosts;

        if (userPosts.NullOrEmpty())
        {
            this.LastPostsHolder.Visible = false;
        }

        this.SearchUser.NavigateUrl = this.Get<LinkBuilder>().GetLink(
            ForumPages.Search,
            new { postedby = userNameOrDisplayName });

        this.DataBind();
    }

    /// <summary>
    /// The setup avatar.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    private void SetupAvatar(User user)
    {
        this.Avatar.ImageUrl = this.Get<IAvatars>().GetAvatarUrlForUser(user);
        this.Avatar.Attributes.CssStyle.Add("max-width", this.PageBoardContext.BoardSettings.AvatarWidth.ToString());
        this.Avatar.Attributes.CssStyle.Add("max-height", this.PageBoardContext.BoardSettings.AvatarHeight.ToString());
    }

    /// <summary>
    /// The setup buddy list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    private void SetupBuddyList(int userID, [NotNull] Tuple<User, AspNetUsers, Rank, VAccess> user)
    {
        if (userID == this.PageBoardContext.PageUserID)
        {
            this.lnkBuddy.Visible = false;
        }
        else if (this.Get<IFriends>().IsBuddy(user.Item1.ID, true) && !this.PageBoardContext.IsGuest)
        {
            this.lnkBuddy.Visible = true;
            this.lnkBuddy.Icon = "user-minus";
            this.lnkBuddy.TextLocalizedTag = "REMOVEBUDDY";
            this.lnkBuddy.TextLocalizedPage = "PAGE";
            this.lnkBuddy.Type = ButtonStyle.Warning;
            this.lnkBuddy.CommandArgument = "removebuddy";
            this.lnkBuddy.ReturnConfirmTag = "NOTIFICATION_REMOVE";
        }
        else if (this.Get<IFriends>().IsBuddy(user.Item1.ID, false))
        {
            this.lnkBuddy.Visible = false;
        }
        else
        {
            if (!this.PageBoardContext.IsGuest && !user.Item1.Block.BlockFriendRequests)
            {
                this.lnkBuddy.Visible = true;
                this.lnkBuddy.Icon = "user-plus";
                this.lnkBuddy.TextLocalizedTag = "ADDBUDDY";
                this.lnkBuddy.TextLocalizedPage = "PAGE";
                this.lnkBuddy.Type = ButtonStyle.Success;
                this.lnkBuddy.CommandArgument = "addbuddy";
            }
            else
            {
                this.lnkBuddy.Visible = false;
            }
        }

        this.Friends.DataSource = this.Get<IFriends>().GetForUser(userID)
            .Where(x => x.Approved && x.FromUserID == userID).ToList();
        this.Friends.DataBind();

        this.BuddyCard.Visible = this.Friends.Items.Count > 0;
    }

    /// <summary>
    /// The setup user links.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    /// <param name="userName">
    /// Name of the user.
    /// </param>
    private void SetupUserLinks([NotNull] Tuple<User, AspNetUsers, Rank, VAccess> user, string userName)
    {
        // homepage link
        this.HomePlaceHolder.Visible = user.Item2.Profile_Homepage.IsSet();
        this.Home.NavigateUrl = user.Item2.Profile_Homepage;

        // blog link
        this.Blog.Visible = user.Item2.Profile_Blog.IsSet();
        this.SetupThemeButtonWithLink(this.Blog, user.Item2.Profile_Blog);
        this.Blog.ParamTitle0 = userName;

        this.Facebook.Visible = user.Item2.Profile_Facebook.IsSet();

        if (user.Item2.Profile_Facebook.IsSet())
        {
            this.Facebook.NavigateUrl = ValidationHelper.IsNumeric(user.Item2.Profile_Facebook)
                                            ? $"https://www.facebook.com/profile.php?id={user.Item2.Profile_Facebook}"
                                            : user.Item2.Profile_Facebook;
        }

        this.Facebook.ParamTitle0 = userName;

        this.Twitter.Visible = user.Item2.Profile_Twitter.IsSet();
        this.Twitter.NavigateUrl = $"http://twitter.com/{this.HtmlEncode(user.Item2.Profile_Twitter)}";
        this.Twitter.ParamTitle0 = userName;

        this.XMPP.Visible = user.Item2.Profile_XMPP.IsSet();
        this.XMPP.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Jabber, new { u = user.Item1.ID });
        this.XMPP.ParamTitle0 = userName;

        this.Skype.Visible = user.Item2.Profile_Skype.IsSet();
        this.Skype.NavigateUrl = $"skype:{user.Item2.Profile_Skype}?call";
        this.Skype.ParamTitle0 = userName;

        if (!this.Skype.Visible && !this.Blog.Visible && !this.XMPP.Visible && !this.Facebook.Visible &&
            !this.Twitter.Visible)
        {
            this.SocialMediaHolder.Visible = false;
        }

        var customProfile = this.DataCache.GetOrSet(
            string.Format(Constants.Cache.UserCustomProfileData, this.UserId),
            () => this.GetRepository<ProfileCustom>().ListByUser(this.UserId));

        this.CustomProfile.DataSource = customProfile;
        this.CustomProfile.DataBind();

        if (user.Item1.ID == this.PageBoardContext.PageUserID)
        {
            return;
        }

        var isFriend = this.Get<IFriends>().IsBuddy(user.Item1.ID, true);

        this.PM.Visible = !user.Item1.UserFlags.IsGuest && this.PageBoardContext.BoardSettings.AllowPrivateMessages;

        if (this.PM.Visible)
        {
            if (user.Item1.Block.BlockPMs)
            {
                this.PM.Visible = false;
            }

            if (this.PageBoardContext.IsAdmin || isFriend)
            {
                this.PM.Visible = true;
            }
        }

        this.PM.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.PostPrivateMessage, new { u = user.Item1.ID });
        this.PM.ParamTitle0 = userName;

        // email link
        this.Email.Visible = !user.Item1.UserFlags.IsGuest && this.PageBoardContext.BoardSettings.AllowEmailSending;

        if (this.Email.Visible)
        {
            if (user.Item1.Block.BlockEmails && !this.PageBoardContext.IsAdmin)
            {
                this.Email.Visible = false;
            }

            if (this.PageBoardContext.IsAdmin || isFriend)
            {
                this.Email.Visible = true;
            }
        }

        this.Email.NavigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Email, new { u = user.Item1.ID });
        if (this.PageBoardContext.IsAdmin)
        {
            this.Email.TitleNonLocalized = user.Item1.Email;
        }

        this.Email.ParamTitle0 = userName;
    }

    /// <summary>
    /// The setup user profile info.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    private void SetupUserProfileInfo([NotNull] Tuple<User, AspNetUsers, Rank, VAccess> user)
    {
        this.UserLabel1.UserID = user.Item1.ID;
        this.UserLabel1.ReplaceName = user.Item1.DisplayOrUserName();
        this.UserLabel1.Style = user.Item1.UserStyle;

        this.Joined.Text = this.Get<IDateTimeService>().FormatDateLong(user.Item1.Joined);

        // vzrus: Show last visit only to admins if user is hidden
        if (!this.PageBoardContext.IsAdmin && user.Item1.UserFlags.IsActiveExcluded)
        {
            this.LastVisit.Text = this.GetText("COMMON", "HIDDEN");
            this.LastVisit.Visible = true;
        }
        else
        {
            this.LastVisitDateTime.DateTime = user.Item1.LastVisit;
            this.LastVisitDateTime.Visible = true;
        }

        if (user.Item3.Name.IsSet())
        {
            this.RankTR.Visible = true;
            this.Rank.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item3.Name));
        }

        if (user.Item2.Profile_Location.IsSet())
        {
            this.LocationTR.Visible = true;
            this.Location.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item2.Profile_Location));
        }

        if (user.Item2.Profile_Country.IsSet() && !user.Item2.Profile_Country.Equals("N/A"))
        {
            this.CountryTR.Visible = true;
            this.CountryLabel.Text =
                $"<span class=\"fi fi-{user.Item2.Profile_Country.Trim().ToLower()}\"></span>&nbsp;{this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.GetText("COUNTRY", user.Item2.Profile_Country.Trim())))}";
        }

        if (user.Item2.Profile_Region.IsSet())
        {
            this.RegionTR.Visible = true;

            try
            {
                var tag =
                    $"RGN_{(user.Item2.Profile_Country.Trim().IsSet() ? user.Item2.Profile_Country.Trim() : this.Get<ILocalization>().Culture.Name.Remove(0, 3).ToUpperInvariant())}_{user.Item2.Profile_Region}";
                this.RegionLabel.Text =
                    this.HtmlEncode(this.Get<IBadWordReplace>().Replace(this.GetText("REGION", tag)));
            }
            catch (Exception)
            {
                this.RegionTR.Visible = false;
            }
        }

        if (user.Item2.Profile_City.IsSet())
        {
            this.CityTR.Visible = true;
            this.CityLabel.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item2.Profile_City));
        }

        if (user.Item2.Profile_Location.IsSet())
        {
            this.LocationTR.Visible = true;
            this.Location.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item2.Profile_Location));
        }

        if (user.Item2.Profile_RealName.IsSet())
        {
            this.RealNameTR.Visible = true;
            this.RealName.Text = this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item2.Profile_RealName));
        }

        if (user.Item2.Profile_Interests.IsSet())
        {
            this.InterestsTR.Visible = true;
            this.Interests.Text =
                this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item2.Profile_Interests));
        }

        if (user.Item2.Profile_Gender > 0)
        {
            this.GenderTR.Visible = true;

            var genders = EnumExtensions.GetAllItems<Gender>().ToList();

            this.Gender.Text = this.GetText("GENDER", genders[user.Item2.Profile_Gender].ToString());
        }

        if (user.Item2.Profile_Occupation.IsSet())
        {
            this.OccupationTR.Visible = true;
            this.Occupation.Text =
                this.HtmlEncode(this.Get<IBadWordReplace>().Replace(user.Item2.Profile_Occupation));
        }

        this.ThanksFrom.Text = this.GetRepository<Thanks>().ThanksFromUser(user.Item1.ID).ToString();

        var thanks = this.GetRepository<Thanks>().ThanksToUser(user.Item1.ID);
        this.ThanksToTimes.Text = thanks.ThanksReceived;
        this.ThanksToPosts.Text = thanks.Posts.ToString();

        this.ReputationReceived.Text =
            this.Get<IReputation>().GenerateReputationBar(user.Item1.Points, user.Item1.ID);

        if (user.Item2.Profile_Birthday >= DateTimeHelper.SqlDbMinTime())
        {
            this.BirthdayTR.Visible = true;
            this.Birthday.Text = this.Get<IDateTimeService>().FormatDateLong(
                user.Item2.Profile_Birthday.Value.AddMinutes(-DateTimeHelper.GetTimeZoneOffset(user.Item1.TimeZoneInfo)));
        }
        else
        {
            this.BirthdayTR.Visible = false;
        }

        // Show User Medals
        if (this.PageBoardContext.BoardSettings.ShowMedals)
        {
            this.ShowUserMedals();
        }
    }

    /// <summary>
    /// Show the user medals.
    /// </summary>
    private void ShowUserMedals()
    {
        var key = string.Format(Constants.Cache.UserMedals, this.UserId);

        // get the medals cached...
        var userMedals = this.DataCache.GetOrSet(
            key,
            () => this.GetRepository<Medal>().ListUserMedals(this.UserId),
            TimeSpan.FromMinutes(10));

        if (!userMedals.Any())
        {
            this.MedalsRow.Visible = false;

            return;
        }

        var ribbonBar = new StringBuilder();
        var medals = new StringBuilder();

        userMedals.ForEach(
            medal =>
                {
                    var flags = new MedalFlags(medal.Flags);

                    // skip hidden medals
                    if (flags.AllowHiding && medal.Hide)
                    {
                        return;
                    }

                    var title = $"{medal.Name}{(flags.ShowMessage ? $": {medal.Message}" : string.Empty)}";

                    ribbonBar.AppendFormat(
                        "<li class=\"list-inline-item\"><img src=\"{0}{3}/{1}\" alt=\"{2}\" title=\"{2}\" data-bs-toggle=\"tooltip\"></li>",
                        BoardInfo.ForumClientFileRoot,
                        medal.MedalURL,
                        title,
                        this.Get<BoardFolders>().Medals);
                });

        this.MedalsPlaceHolder.Text = $"<ul class=\"list-inline\">{ribbonBar}{medals}</ul>";
        this.MedalsRow.Visible = true;
    }

    /// <summary>
    /// The setup user statistics.
    /// </summary>
    /// <param name="user">
    /// The user.
    /// </param>
    private void SetupUserStatistics([NotNull] Tuple<User, AspNetUsers, Rank, VAccess> user)
    {
        var allPosts = 0.0;

        var postsInBoard = this.GetRepository<Message>()
            .Count(m => (m.Flags & 16) == 16 && (m.Flags & 8) != 8);

        if (postsInBoard > 0)
        {
            allPosts = 100.0 * user.Item1.NumPosts / postsInBoard;
        }

        var numberDays = DateTimeHelper.DateDiffDay(user.Item1.Joined, DateTime.UtcNow) + 1;

        this.Stats.Text =
            $"{user.Item1.NumPosts:N0} [{this.GetTextFormatted("NUMALL", allPosts)} / {this.GetTextFormatted("NUMDAY", (double)user.Item1.NumPosts / numberDays)}]";
    }
}