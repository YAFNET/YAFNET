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
    using System.Data;
    using System.Linq;
    using System.Web.Security;

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
    /// The edit users settings.
    /// </summary>
    public partial class EditUsersSettings : BaseUserControl
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether UpdateEmailFlag.
        /// </summary>
        protected bool UpdateEmailFlag
        {
            get => this.ViewState["bUpdateEmail"] != null && this.ViewState["bUpdateEmail"].ToType<bool>();

            set => this.ViewState["bUpdateEmail"] = value;
        }

        /// <summary>
        /// Gets the User Data.
        /// </summary>
        [NotNull]
        private CombinedUserDataHelper UserData =>
            this.userData ?? (this.userData = new CombinedUserDataHelper(this.currentUserId));

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(
                this.PageContext.CurrentForumPage.IsAdminPage ? ForumPages.Admin_Users : ForumPages.Account);
        }

        /// <summary>
        /// Handles the TextChanged event of the Email control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void EmailTextChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UpdateEmailFlag = true;
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

            if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin
                                                              && this.PageContext.QueryIDs.ContainsKey("u"))
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

            // End Modifications for enhanced profile
            this.ForumSettingsRows.Visible = this.Get<BoardSettings>().AllowUserTheme
                                             || this.Get<BoardSettings>().AllowUserLanguage;

            this.UserThemeRow.Visible = this.Get<BoardSettings>().AllowUserTheme;
            this.UserLanguageRow.Visible = this.Get<BoardSettings>().AllowUserLanguage;
            this.LoginInfo.Visible = this.Get<BoardSettings>().AllowEmailChange;

            // override Place Holders for DNN, dnn users should only see the forum settings but not the profile page
            if (Config.IsDotNetNuke)
            {
                this.LoginInfo.Visible = false;
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

                if (this.Get<BoardSettings>().EmailVerification)
                {
                    this.Get<ISendNotification>().SendEmailChangeVerification(newEmail, this.currentUserId, userName);
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

            // vzrus: We should do it as we need to write null value to db, else it will be empty.
            // Localizer currently treats only nulls.
            object language = null;
            object culture = this.Culture.SelectedValue;
            object theme = this.Theme.SelectedValue;

            if (this.Theme.SelectedValue.IsNotSet())
            {
                theme = null;
            }

            if (this.Culture.SelectedValue.IsNotSet())
            {
                culture = null;
            }
            else
            {
                StaticDataHelper.Cultures().Rows.Cast<DataRow>()
                    .Where(row => culture.ToString() == row["CultureTag"].ToString()).ForEach(
                        row => { language = row["CultureFile"].ToString(); });
            }

            // save remaining settings to the DB
            this.GetRepository<User>().Save(
                this.currentUserId,
                this.PageContext.PageBoardID,
                null,
                this.UserData.DisplayName,
                null,
                this.TimeZones.SelectedValue,
                language,
                culture,
                theme,
                null,
                null,
                null,
                false,
                this.HideMe.Checked,
                null);

            this.GetRepository<User>().UpdateOnly(
                () => new User { Activity = this.Activity.Checked },
                u => u.ID == this.currentUserId);

            // vzrus: If it's a guest edited by an admin registry value should be changed
            var dt = this.GetRepository<User>().ListAsDataTable(
                this.PageContext.PageBoardID,
                this.currentUserId,
                true,
                null,
                null,
                false);

            if (dt.HasRows() && dt.Rows[0]["IsGuest"].ToType<bool>())
            {
                this.GetRepository<Registry>().Save(
                    "timezone",
                    this.TimeZones.SelectedValue,
                    this.PageContext.PageBoardID);
            }

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
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.TimeZones.DataSource = StaticDataHelper.TimeZones();

            if (this.Get<BoardSettings>().AllowUserTheme)
            {
                this.Theme.DataSource = StaticDataHelper.Themes();
            }

            if (this.Get<BoardSettings>().AllowUserLanguage)
            {
                this.Culture.DataSource = StaticDataHelper.Cultures();
                this.Culture.DataValueField = "CultureTag";
                this.Culture.DataTextField = "CultureNativeName";
            }

            this.DataBind();

            this.Email.Text = this.UserData.Email;

            var timeZoneItem = this.TimeZones.Items.FindByValue(this.UserData.TimeZoneInfo.Id);

            if (timeZoneItem != null)
            {
                timeZoneItem.Selected = true;
            }

            if (this.Get<BoardSettings>().AllowUserTheme && this.Theme.Items.Count > 0)
            {
                // Allows to use different per-forum themes,
                // While "Allow User Change Theme" option in the host settings is true
                var themeFile = this.Get<BoardSettings>().Theme;

                if (this.UserData.ThemeFile.IsSet())
                {
                    themeFile = this.UserData.ThemeFile;
                }

                var themeItem = this.Theme.Items.FindByValue(themeFile);

                if (themeItem != null)
                {
                    themeItem.Selected = true;
                }
                else
                {
                    themeItem = this.Theme.Items.FindByValue("yaf");

                    if (themeItem != null)
                    {
                        themeItem.Selected = true;
                    }
                }
            }

            this.HideMe.Checked = this.UserData.IsActiveExcluded
                                  && (this.Get<BoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin);

            this.Activity.Checked = this.UserData.Activity;

            if (!this.Get<BoardSettings>().AllowUserLanguage || this.Culture.Items.Count <= 0)
            {
                return;
            }

            // If 2-letter language code is the same we return Culture, else we return a default full culture from language file
            var foundCultItem = this.Culture.Items.FindByValue(this.GetCulture(true));

            if (foundCultItem != null)
            {
                foundCultItem.Selected = true;
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