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
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Text.RegularExpressions;
    using System.Web.Security;

    using FarsiLibrary.Utils;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
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
        /// Gets or sets a value indicating whether UpdateEmailFlag.
        /// </summary>
        protected bool UpdateEmailFlag
        {
            get
            {
                return this.ViewState["bUpdateEmail"] != null && this.ViewState["bUpdateEmail"].ToType<bool>();
            }

            set
            {
                this.ViewState["bUpdateEmail"] = value;
            }
        }

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
        private CombinedUserDataHelper UserData
        {
            get
            {
                return this.userData ?? (this.userData = new CombinedUserDataHelper(this.currentUserId));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(this.PageContext.CurrentForumPage.IsAdminPage ? ForumPages.admin_users : ForumPages.cp_profile);
        }

        /// <summary>
        /// Handles the TextChanged event of the Email control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Email_TextChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UpdateEmailFlag = true;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            YafContext.Current.PageElements.RegisterJsBlockStartup(
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
            this.Page.Form.DefaultButton = this.UpdateProfile.UniqueID;

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

            this.LoginInfo.Visible = true;

            // Begin Modifications for enhanced profile
            this.Gender.Items.Add(this.GetText("PROFILE", "gender0"));
            this.Gender.Items.Add(this.GetText("PROFILE", "gender1"));
            this.Gender.Items.Add(this.GetText("PROFILE", "gender2"));

            // End Modifications for enhanced profile
            this.ForumSettingsRows.Visible = this.Get<YafBoardSettings>().AllowUserTheme
                                             || this.Get<YafBoardSettings>().AllowUserLanguage;

            this.UserThemeRow.Visible = this.Get<YafBoardSettings>().AllowUserTheme;
            this.TrTextEditors.Visible = this.Get<YafBoardSettings>().AllowUsersTextEditor;
            this.UserLanguageRow.Visible = this.Get<YafBoardSettings>().AllowUserLanguage;
            this.MetaWeblogAPI.Visible = this.Get<YafBoardSettings>().AllowPostToBlog;
            this.LoginInfo.Visible = this.Get<YafBoardSettings>().AllowEmailChange;
            this.DisplayNamePlaceholder.Visible = this.Get<YafBoardSettings>().EnableDisplayName
                                                  && this.Get<YafBoardSettings>().AllowDisplayNameModification;

            // override Place Holders for DNN, dnn users should only see the forum settings but not the profile page
            if (Config.IsDotNetNuke)
            {
                this.ProfilePlaceHolder.Visible = false;

                this.IMServicesPlaceHolder.Visible = false;

                this.LoginInfo.Visible = false;
            }

            this.BindData();
        }

        /// <summary>
        /// Saves the Updated Profile
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void UpdateProfile_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var userName = UserMembershipHelper.GetUserNameFromID(this.currentUserId);

            if (this.HomePage.Text.IsSet())
            {
                // add http:// by default
                if (!Regex.IsMatch(this.HomePage.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
                {
                    this.HomePage.Text = "http://{0}".FormatWith(this.HomePage.Text.Trim());
                }

                if (!ValidationHelper.IsValidURL(this.HomePage.Text))
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_HOME"), MessageTypes.warning);
                    return;
                }

                if (this.UserData.NumPosts < this.Get<YafBoardSettings>().IgnoreSpamWordCheckPostCount)
                {
                    string result;

                    // Check for spam
                    if (this.Get<ISpamWordCheck>().CheckForSpamWord(this.HomePage.Text, out result))
                    {
                        // Log and Send Message to Admins
                        if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(1))
                        {
                            this.Logger.Log(
                                null,
                                "Bot Detected",
                                "Internal Spam Word Check detected a SPAM BOT: (user name : '{0}', user id : '{1}') after the user changed the profile Homepage url to: {2}"
                                    .FormatWith(userName, this.currentUserId, this.HomePage.Text),
                                EventLogTypes.SpamBotDetected);
                        }
                        else if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(2))
                        {
                            this.Logger.Log(
                                null,
                                "Bot Detected",
                                "Internal Spam Word Check detected a SPAM BOT: (user name : '{0}', user id : '{1}') after the user changed the profile Homepage url to: {2}, user was deleted and the name, email and IP Address are banned."
                                    .FormatWith(userName, this.currentUserId, this.HomePage.Text),
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

            if (this.MSN.Text.IsSet() && !ValidationHelper.IsValidEmail(this.MSN.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_MSN"), MessageTypes.warning);
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

            if (this.Google.Text.IsSet() && !ValidationHelper.IsValidURL(this.Google.Text))
            {
                this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_GOOGLE"), MessageTypes.warning);
                return;
            }

            string displayName = null;

            if (this.Get<YafBoardSettings>().EnableDisplayName
                && this.Get<YafBoardSettings>().AllowDisplayNameModification)
            {
                // Check if name matches the required minimum length
                if (this.DisplayName.Text.Trim().Length < this.Get<YafBoardSettings>().DisplayNameMinLength)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("USERNAME_TOOLONG", this.Get<YafBoardSettings>().DisplayNameMinLength),
                        MessageTypes.warning);

                    return;
                }

                // Check if name matches the required minimum length
                if (this.DisplayName.Text.Length > this.Get<YafBoardSettings>().UserNameMaxLength)
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("USERNAME_TOOLONG", this.Get<YafBoardSettings>().UserNameMaxLength),
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

            if (this.UpdateEmailFlag)
            {
                var newEmail = this.Email.Text.Trim();

                if (!ValidationHelper.IsValidEmail(newEmail))
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                    return;
                }

                var userNameFromEmail = this.Get<MembershipProvider>().GetUserNameByEmail(this.Email.Text.Trim());

                if (userNameFromEmail.IsSet() && userNameFromEmail != userName)
                {
                    this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                    return;
                }

                if (this.Get<YafBoardSettings>().EmailVerification)
                {
                    this.SendEmailVerification(newEmail);
                }
                else
                {
                    // just update the e-mail...
                    try
                    {
                        UserMembershipHelper.UpdateEmail(this.currentUserId, this.Email.Text.Trim());
                    }
                    catch (ApplicationException)
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetText("PROFILE", "DUPLICATED_EMAIL"),
                            MessageTypes.warning);

                        return;
                    }
                }
            }

            if (this.Interests.Text.Trim().Length > 400)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("FIELD_TOOLONG", this.GetText("CP_EDITPROFILE", "INTERESTS"), 400),
                    MessageTypes.warning);

                return;
            }

            if (this.Occupation.Text.Trim().Length > 400)
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("FIELD_TOOLONG", this.GetText("CP_EDITPROFILE", "OCCUPATION"), 400),
                    MessageTypes.warning);

                return;
            }

            this.UpdateUserProfile(userName);

            // vzrus: We should do it as we need to write null value to db, else it will be empty.
            // Localizer currently treats only nulls.
            object language = null;
            object culture = this.Culture.SelectedValue;
            object theme = this.Theme.SelectedValue;
            object editor = this.ForumEditor.SelectedValue;

            if (this.Theme.SelectedValue.IsNotSet())
            {
                theme = null;
            }

            if (this.ForumEditor.SelectedValue.IsNotSet())
            {
                editor = null;
            }

            if (this.Culture.SelectedValue.IsNotSet())
            {
                culture = null;
            }
            else
            {
                foreach (var row in
                    StaticDataHelper.Cultures()
                        .Rows.Cast<DataRow>()
                        .Where(row => culture.ToString() == row["CultureTag"].ToString()))
                {
                    language = row["CultureFile"].ToString();
                }
            }

            // save remaining settings to the DB
            LegacyDb.user_save(
                this.currentUserId,
                this.PageContext.PageBoardID,
                null,
                displayName,
                null,
                this.TimeZones.SelectedValue,
                language,
                culture,
                theme,
                editor,
                this.UseMobileTheme.Checked,
                null,
                null,
                null,
                false,
                this.HideMe.Checked,
                null);

            // vzrus: If it's a guest edited by an admin registry value should be changed
            var dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.currentUserId, true, null, null, false);

            if (dt.HasRows() && dt.Rows[0]["IsGuest"].ToType<bool>())
            {
                LegacyDb.registry_save("timezone", this.TimeZones.SelectedValue, this.PageContext.PageBoardID);
            }

            // clear the cache for this user...)
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.currentUserId));

            this.Get<IDataCache>().Clear();

            if (!this.PageContext.CurrentForumPage.IsAdminPage)
            {
                YafBuildLink.Redirect(ForumPages.cp_profile);
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
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.TimeZones.DataSource = StaticDataHelper.TimeZones();

            if (this.Get<YafBoardSettings>().AllowUserTheme)
            {
                this.Theme.DataSource = StaticDataHelper.Themes();
                this.Theme.DataTextField = "Theme";
                this.Theme.DataValueField = "FileName";
            }

            if (this.Get<YafBoardSettings>().AllowUserLanguage)
            {
                this.Culture.DataSource = StaticDataHelper.Cultures();
                this.Culture.DataValueField = "CultureTag";
                this.Culture.DataTextField = "CultureNativeName";
            }

            this.Country.DataSource = StaticDataHelper.Country();
            this.Country.ImageLocation = YafForumInfo.GetURLToContent("images/flags/{0}.png");
            this.Country.DataValueField = "Value";
            this.Country.DataTextField = "Name";

            if (this.UserData.Profile.Country.IsSet())
            {
                this.LookForNewRegionsBind(this.UserData.Profile.Country);
            }

            if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
            {
                this.ForumEditor.DataSource = ForumEditorHelper.GetFilteredEditorList();
                this.ForumEditor.DataValueField = "Value";
                this.ForumEditor.DataTextField = "Name";
            }

            this.DataBind();

            if (this.Get<YafBoardSettings>().UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
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
            this.Email.Text = this.UserData.Email;
            this.Realname.Text = this.UserData.Profile.RealName;
            this.Occupation.Text = this.UserData.Profile.Occupation;
            this.Interests.Text = this.UserData.Profile.Interests;
            this.Weblog.Text = this.UserData.Profile.Blog;
            this.WeblogUrl.Text = this.UserData.Profile.BlogServiceUrl;
            this.WeblogID.Text = this.UserData.Profile.BlogServicePassword;
            this.WeblogUsername.Text = this.UserData.Profile.BlogServiceUsername;
            this.MSN.Text = this.UserData.Profile.MSN;
            this.YIM.Text = this.UserData.Profile.YIM;
            this.AIM.Text = this.UserData.Profile.AIM;
            this.ICQ.Text = this.UserData.Profile.ICQ;

            this.Facebook.Text = ValidationHelper.IsNumeric(this.UserData.Profile.Facebook)
                                     ? "https://www.facebook.com/profile.php?id={0}".FormatWith(
                                         this.UserData.Profile.Facebook)
                                     : this.UserData.Profile.Facebook;

            this.Twitter.Text = this.UserData.Profile.Twitter;
            this.Google.Text = this.UserData.Profile.Google;
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

            var timeZoneItem = this.TimeZones.Items.FindByValue(this.UserData.TimeZoneInfo.Id);

            if (timeZoneItem != null)
            {
                timeZoneItem.Selected = true;
            }

            this.HideMe.Checked = this.UserData.IsActiveExcluded
                                  && (this.Get<YafBoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin);

            if (this.Get<YafBoardSettings>().MobileTheme.IsSet())
            {
                this.UseMobileThemeRow.Visible = true;
                this.UseMobileTheme.Checked = this.UserData.UseMobileTheme;
            }

            if (this.Get<YafBoardSettings>().AllowUserTheme && this.Theme.Items.Count > 0)
            {
                // Allows to use different per-forum themes,
                // While "Allow User Change Theme" option in hostsettings is true
                var themeFile = this.Get<YafBoardSettings>().Theme;

                if (this.UserData.ThemeFile.IsSet())
                {
                    themeFile = this.UserData.ThemeFile;
                }

                var themeItem = this.Theme.Items.FindByValue(themeFile);
                if (themeItem != null)
                {
                    themeItem.Selected = true;
                }
            }

            if (this.Get<YafBoardSettings>().AllowUsersTextEditor && this.ForumEditor.Items.Count > 0)
            {
                // Text editor
                var textEditor = this.UserData.TextEditor.IsSet()
                                        ? this.UserData.TextEditor
                                        : this.Get<YafBoardSettings>().ForumEditor;

                var editorItem = this.ForumEditor.Items.FindByValue(textEditor);
                if (editorItem != null)
                {
                    editorItem.Selected = true;
                }
                else
                {
                    editorItem = this.ForumEditor.Items.FindByValue("1");
                    editorItem.Selected = true;
                }
            }

            if (!this.Get<YafBoardSettings>().AllowUserLanguage || this.Culture.Items.Count <= 0)
            {
                return;
            }

            // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
            var foundCultItem = this.Culture.Items.FindByValue(this.GetCulture(true));

            if (foundCultItem != null)
            {
                foundCultItem.Selected = true;
            }

            if (!this.Page.IsPostBack)
            {
                this.Realname.Focus();
            }
        }

        /// <summary>
        /// The send email verification.
        /// </summary>
        /// <param name="newEmail">
        /// The new email.
        /// </param>
        private void SendEmailVerification([NotNull] string newEmail)
        {
            var hashinput = "{0}{1}{2}".FormatWith(DateTime.UtcNow, this.Email.Text, Security.CreatePassword(20));
            var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

            // Create Email
            var changeEmail = new YafTemplateEmail("CHANGEEMAIL")
                                  {
                                      TemplateParams =
                                          {
                                              ["{user}"] =
                                              this.PageContext
                                                  .PageUserName,
                                              ["{link}"] =
                                              "{0}\r\n\r\n".FormatWith(
                                                  YafBuildLink
                                                      .GetLinkNotEscaped(
                                                          ForumPages
                                                              .approve,
                                                          true,
                                                          "k={0}",
                                                          hash)),
                                              ["{newemail}"] =
                                              this.Email.Text,
                                              ["{key}"] = hash,
                                              ["{forumname}"] =
                                              this.Get<YafBoardSettings>()
                                                  .Name,
                                              ["{forumlink}"] =
                                              YafForumInfo.ForumURL
                                          }
                                  };


            // save a change email reference to the db
            this.GetRepository<CheckEmail>().Save(this.currentUserId, hash, newEmail);

            // send a change email message...
            changeEmail.SendEmail(new MailAddress(newEmail), this.GetText("COMMON", "CHANGEEMAIL_SUBJECT"), true);

            // show a confirmation
            this.PageContext.AddLoadMessage(this.GetText("PROFILE", "mail_sent").FormatWith(this.Email.Text));
        }

        /// <summary>
        /// The update user profile.
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        private void UpdateUserProfile([NotNull] string userName)
        {
            var userProfile = YafUserProfile.GetProfile(userName);

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
            userProfile.MSN = this.MSN.Text.Trim();
            userProfile.YIM = this.YIM.Text.Trim();
            userProfile.AIM = this.AIM.Text.Trim();
            userProfile.ICQ = this.ICQ.Text.Trim();
            userProfile.Facebook = this.Facebook.Text.Trim();
            userProfile.Twitter = this.Twitter.Text.Trim();
            userProfile.Google = this.Google.Text.Trim();
            userProfile.XMPP = this.Xmpp.Text.Trim();
            userProfile.Skype = this.Skype.Text.Trim();
            userProfile.RealName = this.Realname.Text.Trim();
            userProfile.Occupation = this.Occupation.Text.Trim();
            userProfile.Interests = this.Interests.Text.Trim();
            userProfile.Gender = this.Gender.SelectedIndex;
            userProfile.Blog = this.Weblog.Text.Trim();

            DateTime userBirthdate;

            if (this.Get<YafBoardSettings>().UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
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

            userProfile.BlogServiceUrl = this.WeblogUrl.Text.Trim();
            userProfile.BlogServiceUsername = this.WeblogUsername.Text.Trim();
            userProfile.BlogServicePassword = this.WeblogID.Text.Trim();

            try
            {
                // Sync to User Profile Mirror table while it's dirty
                var settingsPropertyValueCollection = userProfile.PropertyValues;

                LegacyDb.SetPropertyValues(
                    this.PageContext.PageBoardID,
                    UserMembershipHelper.ApplicationName(),
                    this.currentUserId,
                    settingsPropertyValueCollection);
            }
            catch (Exception ex)
            {
                this.Logger.Log(
                    "Error while syncinng the User Profile",
                    EventLogTypes.Error,
                    this.PageContext.PageUserName,
                    "Edit User Profile page",
                    ex);
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
            var languageFile = this.Get<YafBoardSettings>().Language;
            var culture4Tag = this.Get<YafBoardSettings>().Culture;

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