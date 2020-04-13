/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web;

    using FarsiLibrary.Utils;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users profile.
    /// </summary>
    public partial class EditUsersProfile : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        /// The current user id.
        /// </summary>
        private int currentUserId;

        /// <summary>
        /// The _user data.
        /// </summary>
        private CombinedUserDataHelper userData;

        /// <summary>
        /// The current culture information
        /// </summary>
        private CultureInfo currentCultureInfo;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current Culture information.
        /// </summary>
        /// <value>
        /// The current Culture information.
        /// </value>
        [NotNull]
        private CultureInfo CurrentCultureInfo
        {
            get
            {
                if (this.currentCultureInfo != null)
                {
                    return this.currentCultureInfo;
                }

                this.currentCultureInfo = CultureInfo.CreateSpecificCulture(this.GetCulture(true));

                return this.currentCultureInfo;
            }
        }

        /// <summary>
        /// Gets the User Data.
        /// </summary>
        [NotNull]
        private CombinedUserDataHelper UserData => this.userData ?? (this.userData = new CombinedUserDataHelper(this.currentUserId));

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(this.PageContext.CurrentForumPage.IsAdminPage ? ForumPages.Admin_Users : ForumPages.Account);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            BoardContext.Current.PageElements.RegisterJsBlockStartup(
                "DatePickerJs",
                JavaScriptBlocks.DatePickerLoadJs(
                    this.GetText("COMMON", "CAL_JQ_CULTURE_DFORMAT"),
                    this.GetText("COMMON", "CAL_JQ_CULTURE")));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            //this.Page.Form.DefaultButton = this.UpdateProfile.UniqueID;

            this.PageContext.QueryIDs = new QueryStringIDHelper("u");

            if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
            {
                this.currentUserId = this.PageContext.QueryIDs["u"].ToType<int>();
            }
            else
            {
                this.currentUserId = this.PageContext.PageUserID;
            }

            if (this.IsPostBack)
            {
                return;
            }

            // Begin Modifications for enhanced profile
            this.Gender.Items.Add(this.GetText("PROFILE", "gender0"));
            this.Gender.Items.Add(this.GetText("PROFILE", "gender1"));
            this.Gender.Items.Add(this.GetText("PROFILE", "gender2"));

            // End Modifications for enhanced profile
            this.DisplayNamePlaceholder.Visible = this.Get<BoardSettings>().EnableDisplayName
                                                  && this.Get<BoardSettings>().AllowDisplayNameModification;

            // override Place Holders for DNN, dnn users should only see the forum settings but not the profile page
            if (Config.IsDotNetNuke)
            {
                this.ProfilePlaceHolder.Visible = false;

                this.IMServicesPlaceHolder.Visible = false;
            }

            this.BindData();
        }

        /// <summary>
        /// Saves the Updated Profile
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void UpdateProfileClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var userName = UserMembershipHelper.GetUserNameFromID(this.currentUserId);

            if (this.HomePage.Text.IsSet())
            {
                // add http:// by default
                if (!Regex.IsMatch(this.HomePage.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
                {
                    this.HomePage.Text = $"http://{this.HomePage.Text.Trim()}";
                }

                if (!ValidationHelper.IsValidURL(this.HomePage.Text))
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_HOME"), MessageTypes.warning);
                    return;
                }

                if (this.UserData.NumPosts < this.Get<BoardSettings>().IgnoreSpamWordCheckPostCount)
                {
                    // Check for spam
                    if (this.Get<ISpamWordCheck>().CheckForSpamWord(this.HomePage.Text, out _))
                    {
                        // Log and Send Message to Admins
                        if (this.Get<BoardSettings>().BotHandlingOnRegister.Equals(1))
                        {
                            this.Logger.Log(
                                null,
                                "Bot Detected",
                                $"Internal Spam Word Check detected a SPAM BOT: (user name : '{userName}', user id : '{this.currentUserId}') after the user changed the profile Homepage url to: {this.HomePage.Text}",
                                EventLogTypes.SpamBotDetected);
                        }
                        else if (this.Get<BoardSettings>().BotHandlingOnRegister.Equals(2))
                        {
                            this.Logger.Log(
                                null,
                                "Bot Detected",
                                $"Internal Spam Word Check detected a SPAM BOT: (user name : '{userName}', user id : '{this.currentUserId}') after the user changed the profile Homepage url to: {this.HomePage.Text}, user was deleted and the name, email and IP Address are banned.",
                                EventLogTypes.SpamBotDetected);

                            // Kill user
                            if (!this.PageContext.CurrentForumPage.IsAdminPage)
                            {
                                var user = UserMembershipHelper.GetMembershipUserById(this.currentUserId);
                                var userId = this.currentUserId;

                                var userIp = new CombinedUserDataHelper(user, userId).LastIP;

                                UserMembershipHelper.DeleteAndBanUser(this.currentUserId, user, userIp);
                            }
                        }
                    }
                }
            }

            if (this.Weblog.Text.IsSet() && !ValidationHelper.IsValidURL(this.Weblog.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_WEBLOG"), MessageTypes.warning);
                return;
            }

            if (this.Xmpp.Text.IsSet() && !ValidationHelper.IsValidXmpp(this.Xmpp.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_XMPP"), MessageTypes.warning);
                return;
            }

            if (this.ICQ.Text.IsSet()
                && !(ValidationHelper.IsValidEmail(this.ICQ.Text) || ValidationHelper.IsNumeric(this.ICQ.Text)))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_ICQ"), MessageTypes.warning);
                return;
            }

            if (this.Facebook.Text.IsSet() && !ValidationHelper.IsValidURL(this.Facebook.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_FACEBOOK"), MessageTypes.warning);
                return;
            }

            string displayName = null;

            if (this.Get<BoardSettings>().EnableDisplayName
                && this.Get<BoardSettings>().AllowDisplayNameModification)
            {
                // Check if name matches the required minimum length
                if (this.DisplayName.Text.Trim().Length < this.Get<BoardSettings>().DisplayNameMinLength)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("USERNAME_TOOLONG", this.Get<BoardSettings>().DisplayNameMinLength),
                        MessageTypes.warning);

                    return;
                }

                // Check if name matches the required minimum length
                if (this.DisplayName.Text.Length > this.Get<BoardSettings>().UserNameMaxLength)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("USERNAME_TOOLONG", this.Get<BoardSettings>().UserNameMaxLength),
                        MessageTypes.warning);

                    return;
                }

                if (this.DisplayName.Text.Trim() != this.UserData.DisplayName)
                {
                    if (this.Get<IUserDisplayName>().GetId(this.DisplayName.Text.Trim()).HasValue)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"),
                            MessageTypes.warning);

                        return;
                    }

                    displayName = this.DisplayName.Text.Trim();
                }
            }

            if (this.Interests.Text.Trim().Length > 400)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "INTERESTS"), 400),
                    MessageTypes.warning);

                return;
            }

            if (this.Occupation.Text.Trim().Length > 400)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "OCCUPATION"), 400),
                    MessageTypes.warning);

                return;
            }

            this.UpdateUserProfile(userName);

            // save remaining settings to the DB
            this.GetRepository<User>().Save(
                this.currentUserId,
                this.PageContext.PageBoardID,
                null,
                displayName,
                null,
                this.UserData.TimeZoneInfo.Id,
                this.UserData.LanguageFile,
                this.UserData.CultureUser,
                this.UserData.ThemeFile,
                null,
                null,
                null,
                false,
                this.UserData.IsActiveExcluded,
                null);

            // clear the cache for this user...)
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));

            this.Get<IDataCache>().Clear();

            if (!this.PageContext.CurrentForumPage.IsAdminPage)
            {
                BuildLink.Redirect(ForumPages.Account);
            }
            else
            {
                this.userData = null;
                this.BindData();
            }
        }

        /// <summary>
        /// Check if the Selected Country has any Regions
        /// and if yes load them.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void LookForNewRegions(object sender, EventArgs e)
        {
            if (this.Country.SelectedValue != null)
            {
                if (this.Country.SelectedValue.IsSet())
                {
                    this.LookForNewRegionsBind(this.Country.SelectedValue);
                    this.Region.DataBind();
                }
                else
                {
                    this.Region.DataSource = null;
                    this.RegionTr.Visible = false;
                }
            }
            else
            {
                this.Region.DataSource = null;
                this.Region.DataBind();
            }
        }

        /// <summary>
        /// Set the Location Info (Country and City) based on the users last IP
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void GetLocationOnClick(object sender, EventArgs e)
        {
            var userIpLocator = BoardContext.Current.Get<IIpInfoService>().GetUserIpLocator(
                this.PageContext.CurrentForumPage.IsAdminPage
                    ? this.UserData.LastIP
                    : BoardContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress());

            if (userIpLocator["CountryCode"] != null && userIpLocator["CountryCode"].IsSet() && !userIpLocator["CountryCode"].Equals("-"))
            {
                var countryItem = this.Country.Items.FindByValue(userIpLocator["CountryCode"]);

                if (countryItem != null)
                {
                    this.Country.ClearSelection();
                    countryItem.Selected = true;
                }
            }

            if (userIpLocator["CityName"] != null && userIpLocator["CityName"].IsSet() && !userIpLocator["CityName"].Equals("-"))
            {
                this.City.Text = userIpLocator["CityName"];
            }
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.Country.DataSource = StaticDataHelper.Country();
            this.Country.DataValueField = "Value";
            this.Country.DataTextField = "Name";

            if (this.UserData.Profile.Country.IsSet())
            {
                this.LookForNewRegionsBind(this.UserData.Profile.Country);
            }

            this.DataBind();

            if (this.Get<BoardSettings>().UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
            {
                this.Birthday.Text = this.UserData.Profile.Birthday > DateTimeHelper.SqlDbMinTime()
                                     || this.UserData.Profile.Birthday.IsNullOrEmptyDBField()
                                         ? PersianDateConverter.ToPersianDate(this.UserData.Profile.Birthday)
                                               .ToString("d")
                                         : PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
            }
            else
            {
                this.Birthday.Text = this.UserData.Profile.Birthday > DateTimeHelper.SqlDbMinTime()
                                     || this.UserData.Profile.Birthday.IsNullOrEmptyDBField()
                                         ? this.UserData.Profile.Birthday.Date.ToString(
                                             this.CurrentCultureInfo.DateTimeFormat.ShortDatePattern,
                                             CultureInfo.InvariantCulture)
                                         : DateTimeHelper.SqlDbMinTime()
                                               .Date.ToString(
                                                   this.CurrentCultureInfo.DateTimeFormat.ShortDatePattern,
                                                   CultureInfo.InvariantCulture);
            }

            this.Birthday.ToolTip = this.GetText("COMMON", "CAL_JQ_TT");

            this.DisplayName.Text = this.UserData.DisplayName;
            this.City.Text = this.UserData.Profile.City;
            this.Location.Text = this.UserData.Profile.Location;
            this.HomePage.Text = this.UserData.Profile.Homepage;
            this.Realname.Text = this.UserData.Profile.RealName;
            this.Occupation.Text = this.UserData.Profile.Occupation;
            this.Interests.Text = this.UserData.Profile.Interests;
            this.Weblog.Text = this.UserData.Profile.Blog;
            this.ICQ.Text = this.UserData.Profile.ICQ;

            this.Facebook.Text = ValidationHelper.IsNumeric(this.UserData.Profile.Facebook)
                                     ? $"https://www.facebook.com/profile.php?id={this.UserData.Profile.Facebook}"
                                     : this.UserData.Profile.Facebook;

            this.Twitter.Text = this.UserData.Profile.Twitter;
            this.Xmpp.Text = this.UserData.Profile.XMPP;
            this.Skype.Text = this.UserData.Profile.Skype;
            this.Gender.SelectedIndex = this.UserData.Profile.Gender;

            if (this.UserData.Profile.Country.IsSet())
            {
                var countryItem = this.Country.Items.FindByValue(this.UserData.Profile.Country.Trim());
                if (countryItem != null)
                {
                    countryItem.Selected = true;
                }
            }

            if (this.UserData.Profile.Region.IsSet())
            {
                var regionItem = this.Region.Items.FindByValue(this.UserData.Profile.Region.Trim());
                if (regionItem != null)
                {
                    regionItem.Selected = true;
                }
            }
        }

        /// <summary>
        /// The update user profile.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        private void UpdateUserProfile([NotNull] string userName)
        {
            var userProfile = Utils.UserProfile.GetProfile(userName);

            userProfile.Country = this.Country.SelectedItem != null
                                      ? this.Country.SelectedItem.Value.Trim()
                                      : string.Empty;
            userProfile.Region = this.Region.SelectedItem != null && this.Country.SelectedItem != null
                                 && this.Country.SelectedItem.Value.Trim().IsSet()
                                     ? this.Region.SelectedItem.Value.Trim()
                                     : string.Empty;
            userProfile.City = this.City.Text.Trim();
            userProfile.Location = this.Location.Text.Trim();
            userProfile.Homepage = this.HomePage.Text.Trim();
            userProfile.ICQ = this.ICQ.Text.Trim();
            userProfile.Facebook = this.Facebook.Text.Trim();
            userProfile.Twitter = this.Twitter.Text.Trim();
            userProfile.XMPP = this.Xmpp.Text.Trim();
            userProfile.Skype = this.Skype.Text.Trim();
            userProfile.RealName = this.Realname.Text.Trim();
            userProfile.Occupation = this.Occupation.Text.Trim();
            userProfile.Interests = this.Interests.Text.Trim();
            userProfile.Gender = this.Gender.SelectedIndex;
            userProfile.Blog = this.Weblog.Text.Trim();

            DateTime userBirthdate;

            if (this.Get<BoardSettings>().UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
            {
                try
                {
                    var persianDate = new PersianDate(this.Birthday.Text);

                    userBirthdate = PersianDateConverter.ToGregorianDateTime(persianDate);
                }
                catch (Exception)
                {
                    userBirthdate = DateTimeHelper.SqlDbMinTime().Date;
                }

                if (userBirthdate >= DateTimeHelper.SqlDbMinTime().Date)
                {
                    userProfile.Birthday = userBirthdate.Date;
                }
            }
            else
            {
                DateTime.TryParse(this.Birthday.Text, this.CurrentCultureInfo, DateTimeStyles.None, out userBirthdate);

                if (userBirthdate >= DateTimeHelper.SqlDbMinTime().Date)
                {
                    // Attention! This is stored in profile in the user timezone date
                    userProfile.Birthday = userBirthdate.Date;
                }
            }

            userProfile.Save();
        }

        /// <summary>
        /// Looks for new regions bind.
        /// </summary>
        /// <param name="country">The country.</param>
        private void LookForNewRegionsBind(string country)
        {
            var dt = StaticDataHelper.Region(country);

            // The first row is empty
            if (dt.Rows.Count > 1)
            {
                this.Region.DataSource = dt;
                this.Region.DataValueField = "Value";
                this.Region.DataTextField = "Name";
                this.RegionTr.Visible = true;
            }
            else
            {
                this.Region.DataSource = null;
                this.Region.DataBind();
                this.RegionTr.Visible = false;
                this.RegionTr.DataBind();
            }
        }

        #endregion

        /// <summary>
        /// Gets the culture.
        /// </summary>
        /// <param name="overrideByPageUserCulture">if set to <c>true</c> [override by page user culture].</param>
        /// <returns>
        /// The get culture.
        /// </returns>
        private string GetCulture(bool overrideByPageUserCulture)
        {
            // Language and culture
            var languageFile = this.Get<BoardSettings>().Language;
            var culture4Tag = this.Get<BoardSettings>().Culture;

            if (overrideByPageUserCulture)
            {
                if (this.PageContext.CurrentUserData.LanguageFile.IsSet())
                {
                    languageFile = this.PageContext.CurrentUserData.LanguageFile;
                }

                if (this.PageContext.CurrentUserData.CultureUser.IsSet())
                {
                    culture4Tag = this.PageContext.CurrentUserData.CultureUser;
                }
            }
            else
            {
                if (this.UserData.LanguageFile.IsSet())
                {
                    languageFile = this.UserData.LanguageFile;
                }

                if (this.UserData.CultureUser.IsSet())
                {
                    culture4Tag = this.UserData.CultureUser;
                }
            }

            // Get first default full culture from a language file tag.
            var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
            return langFileCulture.Substring(0, 2) == culture4Tag.Substring(0, 2) ? culture4Tag : langFileCulture;
        }
    }
}