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

namespace YAF.Controls;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;

/// <summary>
/// The edit users settings.
/// </summary>
public partial class EditUsersSettings : BaseUserControl
{
    /// <summary>
    /// Gets or sets the current edit user.
    /// </summary>
    /// <value>The user.</value>
    public User User { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether UpdateEmailFlag.
    /// </summary>
    protected bool UpdateEmailFlag
    {
        get => this.ViewState["bUpdateEmail"] != null && this.ViewState["bUpdateEmail"].ToType<bool>();

        set => this.ViewState["bUpdateEmail"] = value;
    }

    /// <summary>
    /// Handles the Click event of the Cancel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(
            this.PageBoardContext.CurrentForumPage.IsAdminPage ? ForumPages.Admin_Users : ForumPages.MyAccount);
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
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.LoginInfo.Visible = true;

        // End Modifications for enhanced profile
        this.UserThemeRow.Visible = this.PageBoardContext.BoardSettings.AllowUserTheme;
        this.UserLanguageRow.Visible = this.PageBoardContext.BoardSettings.AllowUserLanguage;
        this.LoginInfo.Visible = this.PageBoardContext.BoardSettings.AllowEmailChange;

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
        if (this.UpdateEmailFlag)
        {
            var newEmail = this.Email.Text.Trim();

            if (!ValidationHelper.IsValidEmail(newEmail))
            {
                this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                return;
            }

            var userFromEmail = this.Get<IAspNetUsersHelper>().GetUserByEmail(this.Email.Text.Trim());

            if (userFromEmail != null)
            {
                this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_EMAIL"), MessageTypes.warning);
                return;
            }

            userFromEmail = this.Get<IAspNetUsersHelper>().GetUser(this.User.ProviderUserKey);

            try
            {
                this.Get<IAspNetUsersHelper>().UpdateEmail(userFromEmail, this.Email.Text.Trim());
            }
            catch (ApplicationException)
            {
                this.PageBoardContext.Notify(
                    this.GetText("PROFILE", "DUPLICATED_EMAIL"),
                    MessageTypes.warning);

                return;
            }
        }

        // vzrus: We should do it as we need to write null value to db, else it will be empty.
        // Localizer currently treats only nulls.
        string language = null;
        var culture = this.Culture.SelectedValue;
        var theme = this.Theme.SelectedValue;
        var pageSize = this.PageSize.SelectedValue.ToType<int>();

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
            StaticDataHelper.Cultures()
                .Where(row => culture == row.CultureTag).ForEach(
                    row => language = row.CultureFile);
        }

        // save remaining settings to the DB
        this.GetRepository<User>().Save(
            this.User.ID,
            this.TimeZones.SelectedValue,
            language,
            culture,
            theme,
            this.HideMe.Checked,
            this.Activity.Checked,
            pageSize);

        if (this.User.UserFlags.IsGuest)
        {
            this.GetRepository<Registry>().Save(
                "timezone",
                this.TimeZones.SelectedValue,
                this.PageBoardContext.PageBoardID);
        }

        // clear the cache for this user...)
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.User.ID));

        this.Get<IDataCache>().Clear();

        if (!this.PageBoardContext.CurrentForumPage.IsAdminPage)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.MyAccount);
        }
        else
        {
            this.BindData();
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.TimeZones.DataSource = StaticDataHelper.TimeZones();

        if (this.PageBoardContext.BoardSettings.AllowUserTheme)
        {
            this.Theme.DataSource = StaticDataHelper.Themes();
        }

        if (this.PageBoardContext.BoardSettings.AllowUserLanguage)
        {
            this.Culture.DataSource = StaticDataHelper.Cultures();
            this.Culture.DataValueField = "CultureTag";
            this.Culture.DataTextField = "CultureNativeName";
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";

        this.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.User.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
        }

        this.Email.Text = this.User.Email;

        var timeZoneItem = this.TimeZones.Items.FindByValue(this.User.TimeZoneInfo.Id);

        if (timeZoneItem != null)
        {
            timeZoneItem.Selected = true;
        }

        if (this.PageBoardContext.BoardSettings.AllowUserTheme && this.Theme.Items.Count > 0)
        {
            // Allows to use different per-forum themes,
            // While "Allow User Change Theme" option in the host settings is true
            var themeFile = this.PageBoardContext.BoardSettings.Theme;

            if (this.User.ThemeFile.IsSet())
            {
                themeFile = this.User.ThemeFile;
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

        this.HideMe.Checked = this.User.UserFlags.IsActiveExcluded
                              && (this.PageBoardContext.BoardSettings.AllowUserHideHimself || this.PageBoardContext.IsAdmin);

        this.Activity.Checked = this.User.Activity;

        if (!this.PageBoardContext.BoardSettings.AllowUserLanguage || this.Culture.Items.Count <= 0)
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
        var languageFile = this.PageBoardContext.BoardSettings.Language;
        var culture4Tag = this.PageBoardContext.BoardSettings.Culture;

        if (overrideByPageUserCulture)
        {
            if (this.PageBoardContext.PageUser.LanguageFile.IsSet())
            {
                languageFile = this.PageBoardContext.PageUser.LanguageFile;
            }

            if (this.PageBoardContext.PageUser.Culture.IsSet())
            {
                culture4Tag = this.PageBoardContext.PageUser.Culture;
            }
        }
        else
        {
            if (this.User.LanguageFile.IsSet())
            {
                languageFile = this.User.LanguageFile;
            }

            if (this.User.Culture.IsSet())
            {
                culture4Tag = this.User.Culture;
            }
        }

        // Get first default full culture from a language file tag.
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
        return langFileCulture.Substring(0, 2) == culture4Tag.Substring(0, 2) ? culture4Tag : langFileCulture;
    }
}