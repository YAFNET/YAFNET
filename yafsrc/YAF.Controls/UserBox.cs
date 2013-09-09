/* Yet Another Forum.NET
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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Classes.Pattern;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The user box.
    /// </summary>
    public class UserBox : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The current data row.
        /// </summary>
        private DataRow _row;

        /// <summary>
        ///   Instance of the style transformation class
        /// </summary>
        private IStyleTransform _styleTransforum;

        /// <summary>
        ///   The _user profile.
        /// </summary>
        private YafUserProfile _userProfile;

        /// <summary>
        ///   The _message flags.
        /// </summary>
        private MessageFlags messageFlags;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DataRow.
        /// </summary>
        public DataRow DataRow
        {
            get
            {
                return this._row;
            }

            set
            {
                this._row = value;
                this.messageFlags = (this._row != null) ? new MessageFlags(this._row["Flags"]) : new MessageFlags(0);
            }
        }

        /// <summary>
        ///   Gets or sets PageCache.
        /// </summary>
        public Hashtable PageCache { get; set; }

        /// <summary>
        ///   Gets or sets CachedUserBox.
        /// </summary>
        [CanBeNull]
        protected string CachedUserBox
        {
            get
            {
                if (this.PageCache != null && this.DataRow != null)
                {
                    // get cache for user boxes
                    object cache = this.PageCache[Constants.Cache.UserBoxes];

                    var hashtable = cache as Hashtable;
                    // is it hashtable?
                    if (hashtable != null)
                    {
                        // get only record for user who made message being
                        cache = hashtable[this.UserId];

                        // return from cache if there is something there
                        if (cache != null && cache.ToString() != string.Empty)
                        {
                            return cache.ToString();
                        }
                    }
                }

                return null;
            }

            set
            {
                if (this.PageCache == null || this.DataRow == null)
                {
                    return;
                }

                // get cache for user boxes
                var cache = this.PageCache[Constants.Cache.UserBoxes] as Hashtable;

                // is it hashtable?
                if (cache != null)
                {
                    // save userbox for user of this id to cache
                    cache[this.UserId] = value;
                }
                else
                {
                    // create new hashtable for userbox caching
                    cache = new Hashtable();

                    // save userbox of this user
                    ((Hashtable)cache)[this.UserId] = value;

                    // save cache
                    this.PageCache[Constants.Cache.UserBoxes] = cache;
                }
            }
        }

        /// <summary>
        /// Gets UserBoxRegex.
        /// </summary>
        protected ConcurrentDictionary<string, Regex> UserBoxRegex
        {
            get
            {
                return this.Get<IObjectStore>().GetOrSet("UserBoxRegexDictionary", () => new ConcurrentDictionary<string, Regex>());
            }
        }

        /// <summary>
        ///   Gets UserId.
        /// </summary>
        protected int UserId
        {
            get
            {
                return this.DataRow != null ? this.DataRow["UserID"].ToType<int>() : 0;
            }
        }

        /// <summary>
        ///   Gets UserProfile.
        /// </summary>
        protected YafUserProfile UserProfile
        {
            get
            {
                return this._userProfile ??
                       (this._userProfile =
                        YafUserProfile.GetProfile(UserMembershipHelper.GetUserNameFromID(this.UserId)));
            }
        }

        /// <summary>
        ///   Gets a value indicating whether PostDeleted.
        /// </summary>
        private bool PostDeleted
        {
            get
            {
                return this.messageFlags.IsDeleted;
            }
        }

        /// <summary>
        ///   Gets Refines style string from other skins info
        /// </summary>
        private IStyleTransform TransformStyle
        {
            get
            {
                return this._styleTransforum ?? (this._styleTransforum = this.Get<IStyleTransform>());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The create user box.
        /// </summary>
        /// <returns>
        /// User box control string to display on a page.
        /// </returns>
        [NotNull]
        protected string CreateUserBox()
        {
            string userBox = this.Get<YafBoardSettings>().UserBox;

            // Get styles table for user 
            // this should be called once for groups and for rank for each user/post.
            DataTable roleRankStyleTable = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.GroupRankStyles,
                () => LegacyDb.group_rank_style(YafContext.Current.PageBoardID),
                TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ForumStatisticsCacheTimeout));

            // Avatar
            userBox = this.MatchUserBoxAvatar(userBox);

            // User Medals     
            userBox = this.MatchUserBoxMedals(userBox);

            // Rank Image
            userBox = this.MatchUserBoxRankImages(userBox);

            // Rank     
            userBox = this.MatchUserBoxRank(userBox, roleRankStyleTable);

            // Groups
            userBox = this.MatchUserBoxGroups(userBox, roleRankStyleTable);

            // ThanksFrom
            userBox = this.MatchUserBoxThanksFrom(userBox);

            // ThanksTo
            userBox = this.MatchUserBoxThanksTo(userBox);

            if (!this.PostDeleted)
            {
                // Ederon : 02/24/2007
                // Joined Date
                userBox = this.MatchUserBoxJoinedDate(userBox);

                // Posts
                userBox = this.MatchUserBoxPostCount(userBox);

                // Reputation
                userBox = this.MatchUserBoxReputation(userBox);

                // Gender
                userBox = this.MatchUserBoxGender(userBox);

                // CountryImage
                userBox = this.MatchUserBoxCountryImages(userBox);

                // Location
                userBox = this.MatchUserBoxLocation(userBox);
            }
            else
            {
                userBox = this.MatchUserBoxClearAll(userBox);
            }

            // vzrus: to remove empty dividers  
            return this.RemoveEmptyDividers(userBox);
        }

        /// <summary>
        /// The get regex.
        /// </summary>
        /// <param name="search">
        /// The search.
        /// </param>
        /// <returns>
        /// Returns the regex.
        /// </returns>
        protected Regex GetRegex([NotNull] string search)
        {
            Regex thisRegex;

            if (!this.UserBoxRegex.TryGetValue(search, out thisRegex))
            {
                thisRegex = new Regex(search, RegexOptions.Compiled);
                this.UserBoxRegex.AddOrUpdate(search, (k) => thisRegex, (k, v) => thisRegex);
            }

            return thisRegex;
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            output.WriteLine(@"<div class=""yafUserBox"" id=""{0}"">".FormatWith(this.ClientID));

            string userBox = this.CachedUserBox;

            if (string.IsNullOrEmpty(userBox))
            {
                userBox = this.CreateUserBox();

                // cache...
                this.CachedUserBox = userBox;
            }

            // output the user box info...
            output.WriteLine(userBox);

            output.WriteLine("</div>");
        }

        /// <summary>
        /// The match user box avatar.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Avatar UserBox Content.
        /// </returns>
        [NotNull]
        private string MatchUserBoxAvatar([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Avatar);

            if (!this.PostDeleted)
            {
                string avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(this.UserId);

                if (avatarUrl.IsSet())
                {
                    filler =
                        this.Get<YafBoardSettings>().UserBoxAvatar.FormatWith(
                            @"<a href=""{1}"" title=""{2}""><img class=""avatarimage"" src=""{0}"" alt=""{2}"" title=""{2}""  /></a>"
                                .FormatWith(
                                    avatarUrl,
                                    YafBuildLink.GetLinkNotEscaped(ForumPages.profile, "u={0}", this.UserId),
                                    Page.HtmlEncode(
                                        this.Get<YafBoardSettings>().EnableDisplayName
                                            ? UserMembershipHelper.GetDisplayNameFromID(this.UserId)
                                            : UserMembershipHelper.GetUserNameFromID(this.UserId))));
                }
            }

            // replaces template placeholder with actual avatar
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box clear all.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Cleared User Box
        /// </returns>
        [NotNull]
        private string MatchUserBoxClearAll([NotNull] string userBox)
        {
            string filler = string.Empty;

            var rx = this.GetRegex(Constants.UserBox.JoinDate);
            userBox = rx.Replace(userBox, filler);
            rx = this.GetRegex(Constants.UserBox.Posts);
            userBox = rx.Replace(userBox, filler);
            rx = this.GetRegex(Constants.UserBox.Reputation);
            userBox = rx.Replace(userBox, filler);
            /* rx = this.GetRegex(Constants.UserBox.CountryImage);
             userBox = rx.Replace(userBox, filler); */
            rx = this.GetRegex(Constants.UserBox.Location);
            userBox = rx.Replace(userBox, filler);
            rx = this.GetRegex(Constants.UserBox.ThanksFrom);
            userBox = rx.Replace(userBox, filler);
            rx = this.GetRegex(Constants.UserBox.ThanksTo);
            userBox = rx.Replace(userBox, filler);

            // vzrus: to remove empty dividers  
            return this.RemoveEmptyDividers(userBox);
        }

        /// <summary>
        /// The match user box gender.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Gender UserBox Content.
        /// </returns>
        [NotNull]
        private string MatchUserBoxGender([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Gender);
            int userGender = this.UserProfile.Gender;
            string imagePath = string.Empty;
            string imageAlt = string.Empty;

            if (this.Get<YafBoardSettings>().AllowGenderInUserBox)
            {
                if (userGender > 0)
                {
                    switch (userGender)
                    {
                        case 1:
                            imagePath = this.PageContext.Get<ITheme>().GetItem("ICONS", "GENDER_MALE", null);
                            imageAlt = this.GetText("USERGENDER_MAS");
                            break;
                        case 2:
                            imagePath = this.PageContext.Get<ITheme>().GetItem("ICONS", "GENDER_FEMALE", null);
                            imageAlt = this.GetText("USERGENDER_FEM");
                            break;
                    }

                    filler =
                        this.Get<YafBoardSettings>().UserBoxGender.FormatWith(
                            @"<a><img src=""{0}"" alt=""{1}"" title=""{1}"" /></a>".FormatWith(imagePath, imageAlt));
                }
            }

            // replaces template placeholder with actual image
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box groups.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <param name="roleStyleTable">
        /// The role Style Table.
        /// </param>
        /// <returns>
        /// Returns the Groups Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxGroups([NotNull] string userBox, [NotNull] DataTable roleStyleTable)
        {
            const string StyledNick = @"<span class=""YafGroup_{0}"" style=""{1}"">{0}</span>";

            string filler = string.Empty;

            Regex rx = this.GetRegex(Constants.UserBox.Groups);

            if (this.Get<YafBoardSettings>().ShowGroups)
            {
                var groupsText = new StringBuilder(500);

                bool bFirst = true;
                bool hasRole = false;
                string roleStyle = null;

                var userName = this.DataRow["IsGuest"].ToType<bool>()
                                   ? UserMembershipHelper.GuestUserName
                                   : this.DataRow["UserName"].ToString();

                foreach (string role in RoleMembershipHelper.GetRolesForUser(userName))
                {
                    string role1 = role;

                    foreach (DataRow drow in
                        roleStyleTable.Rows.Cast<DataRow>().Where(
                            drow =>
                            drow["LegendID"].ToType<int>() == 1 && drow["Style"] != null &&
                            drow["Name"].ToString() == role1))
                    {
                        roleStyle = this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true);
                        break;
                    }

                    if (bFirst)
                    {
                        groupsText.AppendLine(
                            this.Get<YafBoardSettings>().UseStyledNicks ? StyledNick.FormatWith(role, roleStyle) : role);

                        bFirst = false;
                    }
                    else
                    {
                        if (this.Get<YafBoardSettings>().UseStyledNicks)
                        {
                            groupsText.AppendLine((@", " + StyledNick).FormatWith(role, roleStyle));
                        }
                        else
                        {
                            groupsText.AppendFormat(", {0}", role);
                        }
                    }

                    roleStyle = null;
                    hasRole = true;
                }

                // vzrus: Only a guest normally has no role
                if (!hasRole)
                {
                    DataTable dt = this.Get<IDataCache>().GetOrSet(
                        Constants.Cache.GuestGroupsCache,
                        () => LegacyDb.group_member(PageContext.PageBoardID, this.DataRow["UserID"]),
                        TimeSpan.FromMinutes(60));

                    foreach (string guestRole in
                        dt.Rows.Cast<DataRow>().Where(role => role["Member"].ToType<int>() > 0).Select(
                            role => role["Name"].ToString()))
                    {
                        foreach (DataRow drow in
                            roleStyleTable.Rows.Cast<DataRow>().Where(
                                drow =>
                                drow["LegendID"].ToType<int>() == 1 && drow["Style"] != null &&
                                drow["Name"].ToString() == guestRole))
                        {
                            roleStyle = this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true);
                            break;
                        }

                        groupsText.AppendLine(
                            this.Get<YafBoardSettings>().UseStyledNicks
                                ? StyledNick.FormatWith(guestRole, roleStyle)
                                : guestRole);
                        break;
                    }
                }

                filler = this.Get<YafBoardSettings>().UserBoxGroups.FormatWith(this.GetText("groups"), groupsText);

                // mddubs : 02/21/2009
                // Remove the space before the first comma when multiple groups exist.
                filler = filler.Replace("\r\n,", ",");
            }

            // replaces template placeholder with actual groups
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box joined date.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Joined Date Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxJoinedDate([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.JoinDate);

            if (this.Get<YafBoardSettings>().DisplayJoinDate)
            {
                filler = this.Get<YafBoardSettings>().UserBoxJoinDate.FormatWith(
                    this.GetText("JOINED"), this.Get<IDateTime>().FormatDateShort((DateTime)this.DataRow["Joined"]));
            }

            // replaces template placeholder with actual join date
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box location.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Location Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxLocation([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Location);

            if (this.UserProfile.Location.IsSet())
            {
                filler = this.Get<YafBoardSettings>().UserBoxLocation.FormatWith(
                    this.GetText("LOCATION"), this.Get<IFormatMessage>().RepairHtml(this.UserProfile.Location, false));
            }

            // replaces template placeholder with actual location
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box medals.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Medals Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxMedals([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Medals);

            if (this.Get<YafBoardSettings>().ShowMedals)
            {
                DataTable dt = this.Get<YafDbBroker>().UserMedals(this.UserId);

                // vzrus: If user doesn't have we shouldn't render this waisting resources
                if (dt.Rows.Count <= 0)
                {
                    return rx.Replace(userBox, filler);
                }

                var ribbonBar = new StringBuilder(500);
                var medals = new StringBuilder(500);

                DataRow r;
                MedalFlags f;

                int i = 0;
                int inRow = 0;

                // do ribbon bar first
                while (dt.Rows.Count > i)
                {
                    r = dt.Rows[i];
                    f = new MedalFlags(r["Flags"]);

                    // do only ribbon bar items first
                    if (!r["OnlyRibbon"].ToType<bool>())
                    {
                        break;
                    }

                    // skip hidden medals
                    if (!f.AllowHiding || !r["Hide"].ToType<bool>())
                    {
                        if (inRow == 3)
                        {
                            // add break - only three ribbons in a row
                            ribbonBar.Append("<br />");
                            inRow = 0;
                        }

                        var title = "{0}{1}".FormatWith(
                            r["Name"], f.ShowMessage ? ": {0}".FormatWith(r["Message"]) : string.Empty);

                        ribbonBar.AppendFormat(
                            "<img src=\"{0}{5}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}\" title=\"{4}\" />",
                            YafForumInfo.ForumClientFileRoot,
                            r["SmallRibbonURL"],
                            r["SmallRibbonWidth"],
                            r["SmallRibbonHeight"],
                            title,
                            YafBoardFolders.Current.Medals);

                        inRow++;
                    }

                    // move to next row
                    i++;
                }

                // follow with the rest
                while (dt.Rows.Count > i)
                {
                    r = dt.Rows[i];
                    f = new MedalFlags(r["Flags"]);

                    // skip hidden medals
                    if (!f.AllowHiding || !r["Hide"].ToType<bool>())
                    {
                        medals.AppendFormat(
                            "<img src=\"{0}{6}/{1}\" width=\"{2}\" height=\"{3}\" alt=\"{4}{5}\" title=\"{4}{5}\" />",
                            YafForumInfo.ForumClientFileRoot,
                            r["SmallMedalURL"],
                            r["SmallMedalWidth"],
                            r["SmallMedalHeight"],
                            r["Name"],
                            f.ShowMessage ? ": {0}".FormatWith(r["Message"]) : string.Empty,
                            YafBoardFolders.Current.Medals);
                    }

                    // move to next row
                    i++;
                }

                filler = this.Get<YafBoardSettings>().UserBoxMedals.FormatWith(
                    this.GetText("MEDALS"), ribbonBar, medals);
            }

            // replaces template placeholder with actual medals
            userBox = rx.Replace(userBox, filler);

            return userBox;
        }

        /// <summary>
        /// The match user box Reputation.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Reputation UserBox Content.
        /// </returns>
        [NotNull]
        private string MatchUserBoxReputation([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Reputation);

            if (this.Get<YafBoardSettings>().DisplayPoints && !this.DataRow["IsGuest"].ToType<bool>())
            {
                filler = this.Get<YafBoardSettings>().UserBoxReputation.FormatWith(
                    this.GetText("REPUTATION"),
                    YafReputation.GenerateReputationBar(this.DataRow["Points"].ToType<int>(), this.UserId));
            }

            // replaces template placeholder with actual points
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box post count.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Post Count Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxPostCount([NotNull] string userBox)
        {
            var rx = this.GetRegex(Constants.UserBox.Posts);

            // vzrus: should not display posts count string if the user post only in a forum with no post count?
            // if ((int)this.DataRow["Posts"] > 0)
            // {
            string filler = this.Get<YafBoardSettings>().UserBoxPosts.FormatWith(
                this.GetText("posts"), this.DataRow["Posts"]);

            // }

            // replaces template placeholder with actual post count
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box rank.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <param name="roleStyleTable">
        /// The role Style Table.
        /// </param>
        /// <returns>
        /// Returns the Rank Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxRank([NotNull] string userBox, [NotNull] DataTable roleStyleTable)
        {
            string rankStyle = (from DataRow drow in roleStyleTable.Rows
                                where
                                    drow["LegendID"].ToType<int>() == 2 && drow["Style"] != null &&
                                    drow["Name"].ToString() == this.DataRow["RankName"].ToString()
                                select this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true)).FirstOrDefault();

            var rx = this.GetRegex(Constants.UserBox.Rank);

            string filler = this.Get<YafBoardSettings>().UserBoxRank.FormatWith(
                this.GetText("rank"),
                this.Get<YafBoardSettings>().UseStyledNicks ? @"<span class=""YafRank_{0}"" style=""{1}"">{0}</span>".FormatWith(this.DataRow["RankName"], rankStyle) : this.DataRow["RankName"]);

            // replaces template placeholder with actual rank
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box rank images.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Rank Images Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxRankImages([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.RankImage);

            if (!this.DataRow["RankImage"].IsNullOrEmptyDBField())
            {
                filler =
                    this.Get<YafBoardSettings>().UserBoxRankImage.FormatWith(
                        @"<img class=""rankimage"" src=""{0}{1}/{2}"" alt="""" />".FormatWith(
                            YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Ranks, this.DataRow["RankImage"]));
            }

            // replaces template placeholder with actual rank image
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box country images.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Country Image Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxCountryImages([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.CountryImage);

            if (this.Get<YafBoardSettings>().ShowCountryInfoInUserBox && this.UserProfile.Country.IsSet() && !this.UserProfile.Country.Equals("N/A"))
            {
                string imagePath = this.PageContext.Get<ITheme>().GetItem(
                    "FLAGS",
                    "{0}_MEDIUM".FormatWith(this.UserProfile.Country.ToUpperInvariant()),
                    YafForumInfo.GetURLToResource("images/flags/{0}.png".FormatWith(this.UserProfile.Country.Trim())));

                string imageAlt = this.GetText("COUNTRY", this.UserProfile.Country.ToUpperInvariant());

                filler =
                    this.Get<YafBoardSettings>().UserBoxCountryImage.FormatWith(
                        @"<a><img src=""{0}"" alt=""{1}"" title=""{1}"" /></a>".FormatWith(imagePath, imageAlt));
            }

            // replaces template placeholder with actual rank image
            userBox = rx.Replace(userBox, filler);
            return userBox;
        }

        /// <summary>
        /// The match user box thanks from.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the Thanks From Userbox string.
        /// </returns>
        [NotNull]
        private string MatchUserBoxThanksFrom([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.ThanksFrom);

            // vzrus: should not display if no thanks?
            if (this.Get<YafBoardSettings>().EnableThanksMod && (int)this.DataRow["ThanksFromUserNumber"] > 0)
            {
                filler =
                    this.Get<YafBoardSettings>().UserBoxThanksFrom.FormatWith(
                        (this.UserProfile.Gender == 1
                             ? this.GetText("thanksfrom_musc")
                             : (this.UserProfile.Gender == 2
                                    ? this.GetText("thanksfrom_fem")
                                    : this.GetText("thanksfrom"))).FormatWith(this.DataRow["ThanksFromUserNumber"]));
            }

            // replaces template placeholder with actual thanks from
            userBox = rx.Replace(userBox, filler);

            return userBox;
        }

        /// <summary>
        /// The match user box thanks to.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// String with Thanks string added to UserBox .
        /// </returns>
        [NotNull]
        private string MatchUserBoxThanksTo([NotNull] string userBox)
        {
            string filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.ThanksTo);

            // vzrus: should not display if no thanks?
            if (this.Get<YafBoardSettings>().EnableThanksMod)
            {
                if ((int)this.DataRow["ThanksToUserNumber"] > 0 && (int)this.DataRow["ThanksToUserPostsNumber"] > 0)
                {
                    filler =
                        this.Get<YafBoardSettings>().UserBoxThanksTo.FormatWith(
                            this.GetText("thanksto").FormatWith(
                                this.DataRow["ThanksToUserNumber"], this.DataRow["ThanksToUserPostsNumber"]));
                }
            }

            // replaces template placeholder with actual thanks from
            userBox = rx.Replace(userBox, filler);

            return userBox;
        }

        /// <summary>
        /// The remove empty dividers.
        /// </summary>
        /// <param name="userBox">
        /// The user box.
        /// </param>
        /// <returns>
        /// Returns the emptyd string
        /// </returns>
        [NotNull]
        private string RemoveEmptyDividers([NotNull] string userBox)
        {
            userBox = userBox.Replace("\"\"section\"\"", "\"section\"").Replace(@"""section""", "\"section\"");
            if (userBox.IndexOf("<div class=\"section\"></div>", StringComparison.Ordinal) > 0)
            {
                userBox =
                    userBox.Replace(
                        userBox.IndexOf("<div class=\"section\"></div><br />", StringComparison.Ordinal) > 0
                            ? "<div class=\"section\"></div><br />"
                            : "<div class=\"section\"></div>",
                        string.Empty);
            }

            return userBox;
        }

        #endregion
    }
}