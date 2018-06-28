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
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
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
        private DataRow row;

        /// <summary>
        ///   Instance of the style transformation class
        /// </summary>
        private IStyleTransform styleTransforum;

        /// <summary>
        ///   The _user profile.
        /// </summary>
        private YafUserProfile userProfile;

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
                return this.row;
            }

            set
            {
                this.row = value;
                this.messageFlags = this.row != null ? new MessageFlags(this.row["Flags"]) : new MessageFlags(0);
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
                if (this.PageCache == null || this.DataRow == null)
                {
                    return null;
                }

                // get cache for user boxes
                var cache = this.PageCache[Constants.Cache.UserBoxes];

                var hashtable = cache as Hashtable;

                // is it hashtable?
                if (hashtable == null)
                {
                    return null;
                }

                // get only record for user who made message being
                cache = hashtable[this.UserId];

                // return from cache if there is something there
                if (cache != null && cache.ToString() != string.Empty)
                {
                    return cache.ToString();
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
                    cache = new Hashtable { [this.UserId] = value };

                    // save userbox of this user

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
                return this.DataRow?["UserID"].ToType<int>() ?? 0;
            }
        }

        /// <summary>
        ///   Gets UserProfile.
        /// </summary>
        protected YafUserProfile UserProfile
        {
            get
            {
                return this.userProfile ??
                       (this.userProfile =
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
                return this.styleTransforum ?? (this.styleTransforum = this.Get<IStyleTransform>());
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
            var userBox = this.Get<YafBoardSettings>().UserBox;

            // Get styles table for user 
            // this should be called once for groups and for rank for each user/post.
            var roleRankStyleTable = this.Get<IDataCache>().GetOrSet(
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

                // Topic Author Badge
                // userBox = this.MatchUserBoxAuthorBadge(userBox);
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
            Regex regex;

            if (!this.UserBoxRegex.TryGetValue(search, out regex))
            {
                regex = new Regex(search, RegexOptions.Compiled);
                this.UserBoxRegex.AddOrUpdate(search, (k) => regex, (k, v) => regex);
            }

            return regex;
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

            var userBox = this.CachedUserBox;

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
            var filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Avatar);

            if (!this.PostDeleted)
            {
                var avatarUrl = this.Get<IAvatars>().GetAvatarUrlForUser(this.UserId);
                var displayName = this.Get<YafBoardSettings>().EnableDisplayName
                                      ? UserMembershipHelper.GetDisplayNameFromID(this.UserId)
                                      : UserMembershipHelper.GetUserNameFromID(this.UserId);

                if (avatarUrl.IsSet())
                {
                    filler = this.Get<YafBoardSettings>().UserBoxAvatar.FormatWith(
                        @"<a href=""{1}"" title=""{2}""><img class=""rounded img-fluid"" src=""{0}"" alt=""{2}"" title=""{2}""  /></a>"
                            .FormatWith(
                                avatarUrl,
                                YafBuildLink.GetLinkNotEscaped(
                                    ForumPages.profile,
                                    "u={0}&name={1}",
                                    this.UserId,
                                    displayName),
                                this.Page.HtmlEncode(displayName)));
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
            var filler = string.Empty;

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
            var filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Gender);
            var userGender = this.UserProfile.Gender;
            var imagePath = string.Empty;
            var imageAlt = string.Empty;

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

            var filler = string.Empty;

            var rx = this.GetRegex(Constants.UserBox.Groups);

            if (this.Get<YafBoardSettings>().ShowGroups)
            {
                var groupsText = new StringBuilder(500);

                var first = true;
                var hasRole = false;
                string roleStyle = null;

                var userName = this.DataRow["IsGuest"].ToType<bool>()
                                   ? UserMembershipHelper.GuestUserName
                                   : this.DataRow["UserName"].ToString();

                foreach (var role in RoleMembershipHelper.GetRolesForUser(userName))
                {
                    var role1 = role;

                    foreach (var drow in
                        roleStyleTable.Rows.Cast<DataRow>().Where(
                            drow =>
                            drow["LegendID"].ToType<int>() == 1 && drow["Style"] != null &&
                            drow["Name"].ToString() == role1))
                    {
                        roleStyle = this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true);
                        break;
                    }

                    if (first)
                    {
                        groupsText.AppendLine(
                            this.Get<YafBoardSettings>().UseStyledNicks ? StyledNick.FormatWith(role, roleStyle) : role);

                        first = false;
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
                    var dt = this.Get<IDataCache>().GetOrSet(
                        Constants.Cache.GuestGroupsCache,
                        () => LegacyDb.group_member(this.PageContext.PageBoardID, this.DataRow["UserID"]),
                        TimeSpan.FromMinutes(60));

                    foreach (var guestRole in
                        dt.Rows.Cast<DataRow>().Where(role => role["Member"].ToType<int>() > 0).Select(
                            role => role["Name"].ToString()))
                    {
                        foreach (var drow in
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
            var filler = string.Empty;
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
            var filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Location);

            if (this.UserProfile.Location.IsSet())
            {
                filler = this.Get<YafBoardSettings>().UserBoxLocation.FormatWith(
                    this.GetText("LOCATION"), this.Get<IFormatMessage>().RepairHtml(this.HtmlEncode(this.UserProfile.Location), false));
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
            var filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.Medals);

            if (this.Get<YafBoardSettings>().ShowMedals)
            {
                var dt = this.Get<YafDbBroker>().UserMedals(this.UserId);

                // vzrus: If user doesn't have we shouldn't render this waisting resources
                if (!dt.HasRows())
                {
                    return rx.Replace(userBox, filler);
                }

                var ribbonBar = new StringBuilder(500);
                var medals = new StringBuilder(500);

                DataRow r;
                MedalFlags f;

                var i = 0;
                var inRow = 0;

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
            var filler = string.Empty;
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
            var filler = this.Get<YafBoardSettings>().UserBoxPosts.FormatWith(
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
            var rankStyle = (from DataRow drow in roleStyleTable.Rows
                                where
                                    drow["LegendID"].ToType<int>() == 2 && drow["Style"] != null &&
                                    drow["Name"].ToString() == this.DataRow["RankName"].ToString()
                                select this.TransformStyle.DecodeStyleByString(drow["Style"].ToString(), true)).FirstOrDefault();

            var rx = this.GetRegex(Constants.UserBox.Rank);

            var filler = this.Get<YafBoardSettings>().UserBoxRank.FormatWith(
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
            var filler = string.Empty;
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
            var filler = string.Empty;
            var rx = this.GetRegex(Constants.UserBox.CountryImage);

            if (this.Get<YafBoardSettings>().ShowCountryInfoInUserBox && this.UserProfile.Country.IsSet() && !this.UserProfile.Country.Equals("N/A"))
            {
                var imagePath = this.PageContext.Get<ITheme>().GetItem(
                    "FLAGS",
                    "{0}_MEDIUM".FormatWith(this.UserProfile.Country.ToUpperInvariant()),
                    YafForumInfo.GetURLToContent("images/flags/{0}.png".FormatWith(this.UserProfile.Country.Trim())));

                var imageAlt = this.GetText("COUNTRY", this.UserProfile.Country.ToUpperInvariant());

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
            var filler = string.Empty;
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
            var filler = string.Empty;
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
            userBox = userBox.Replace("<li class=\"list-group-item\"></li>", string.Empty);

            return userBox;
        }

        #endregion
    }
}