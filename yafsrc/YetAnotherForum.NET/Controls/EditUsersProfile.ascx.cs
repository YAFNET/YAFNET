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

using System.Text.RegularExpressions;

using FarsiLibrary.Utils;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models.Identity;
using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The edit users profile.
/// </summary>
public partial class EditUsersProfile : BaseUserControl
{
    private List<ProfileCustom> userProfileCustom;

    private IList<ProfileDefinition> profileDefinitions;

    /// <summary>
    /// The current culture information
    /// </summary>
    private CultureInfo currentCultureInfo;

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
    public Tuple<User, AspNetUsers, Rank, vaccess> User { get; set; }

    private IEnumerable<ProfileCustom> UserProfileCustom =>
        this.userProfileCustom ??= this.GetRepository<ProfileCustom>().Get(p => p.UserID == this.User.Item1.ID);

    private IList<ProfileDefinition> ProfileDefinitions =>
        this.profileDefinitions ??= this.GetRepository<ProfileDefinition>().GetByBoardId();

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

    protected void CustomProfile_OnItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
        {
            return;
        }

        var profileDef = (ProfileDefinition)e.Item.DataItem;

        var hidden = e.Item.FindControlAs<HiddenField>("DefID");
        var label = e.Item.FindControlAs<Label>("DefLabel");
        var textBox = e.Item.FindControlAs<TextBox>("DefText");
        var requiredMessage = e.Item.FindControlAs<LocalizedLabel>("RequiredMessage");

        var checkPlaceHolder = e.Item.FindControlAs<PlaceHolder>("CheckPlaceHolder");
        var check = e.Item.FindControlAs<CheckBox>("DefCheck");

        hidden.Value = profileDef.ID.ToString();

        label.Text = profileDef.Name;

        var userValue = this.UserProfileCustom.FirstOrDefault(p => p.ProfileDefinitionID == profileDef.ID);

        var type = profileDef.DataType.ToEnum<DataType>();

        if (profileDef.Required)
        { 
            requiredMessage.Param0 = profileDef.Name;
        }

        switch (type)
        {
            case DataType.Text:
                {
                    textBox.MaxLength = profileDef.Length;
                    textBox.CssClass = "form-control";
                    textBox.Visible = true;

                    if (profileDef.DefaultValue.IsSet())
                    {
                        textBox.Text = profileDef.DefaultValue;
                    }

                    if (profileDef.Required)
                    {
                        textBox.Attributes.Add("required", "required");
                    }

                    if (userValue != null)
                    {
                        textBox.Text = userValue.Value;
                    }

                    label.AssociatedControlID = textBox.ID;

                    break;
                }
            case DataType.Number:
                {
                    textBox.TextMode = TextBoxMode.Number;
                    textBox.MaxLength = profileDef.Length;
                    textBox.CssClass = "form-control";
                    textBox.Visible = true;

                    if (profileDef.DefaultValue.IsSet())
                    {
                        textBox.Text = profileDef.DefaultValue;
                    }

                    if (profileDef.Required)
                    {
                        textBox.Attributes.Add("required", "required");
                    }

                    if (userValue != null)
                    {
                        textBox.Text = userValue.Value;
                    }

                    label.AssociatedControlID = textBox.ID;

                    break;
                }
            case DataType.Check:
                {
                    checkPlaceHolder.Visible = true;

                    if (profileDef.DefaultValue.IsSet())
                    {
                        check.Checked = profileDef.DefaultValue.ToType<bool>();
                    }

                    if (userValue != null)
                    {
                        check.Checked = userValue.Value.ToType<bool>();
                    }

                    check.Text = profileDef.Name;
                    label.Visible = false;

                    break;
                }
        }
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            nameof(JavaScriptBlocks.FormValidatorJs),
            JavaScriptBlocks.FormValidatorJs(this.UpdateProfile.ClientID));

        this.Gender.DataSource = StaticDataHelper.Gender();
        this.Gender.DataValueField = "Value";
        this.Gender.DataTextField = "Name";

        // End Modifications for enhanced profile
        this.DisplayNamePlaceholder.Visible = this.PageBoardContext.BoardSettings.EnableDisplayName &&
                                              this.PageBoardContext.BoardSettings.AllowDisplayNameModification;

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
        var userName = this.User.Item1.DisplayOrUserName();

        if (this.HomePage.Text.IsSet())
        {
            // add http:// by default
            if (!Regex.IsMatch(this.HomePage.Text.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*"))
            {
                this.HomePage.Text = $@"https://{this.HomePage.Text.Trim()}";
            }

            if (!ValidationHelper.IsValidURL(this.HomePage.Text))
            {
                this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_HOME"), MessageTypes.warning);
                return;
            }

            if (this.User.Item1.NumPosts < this.PageBoardContext.BoardSettings.IgnoreSpamWordCheckPostCount)
            {
                // Check for spam
                if (this.Get<ISpamWordCheck>().CheckForSpamWord(this.HomePage.Text, out _))
                {
                    switch (this.PageBoardContext.BoardSettings.BotHandlingOnRegister)
                    {
                        // Log and Send Message to Admins
                        case 1:
                            this.Logger.Log(
                                null,
                                "Bot Detected",
                                $"Internal Spam Word Check detected a SPAM BOT: (user name : '{userName}', user id : '{this.User.Item1.ID}') after the user changed the profile Homepage url to: {this.HomePage.Text}",
                                EventLogTypes.SpamBotDetected);
                            break;
                        case 2:
                            {
                                this.Logger.Log(
                                    null,
                                    "Bot Detected",
                                    $"Internal Spam Word Check detected a SPAM BOT: (user name : '{userName}', user id : '{this.User.Item1.ID}') after the user changed the profile Homepage url to: {this.HomePage.Text}, user was deleted and the name, email and IP Address are banned.",
                                    EventLogTypes.SpamBotDetected);

                                // Kill user
                                if (!this.PageBoardContext.CurrentForumPage.IsAdminPage)
                                {
                                    this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                        this.User.Item1.ID,
                                        this.User.Item2,
                                        this.User.Item1.IP);
                                }

                                break;
                            }
                    }
                }
            }
        }

        if (this.Weblog.Text.IsSet() && !ValidationHelper.IsValidURL(this.Weblog.Text.Trim()))
        {
            this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_WEBLOG"), MessageTypes.warning);
            return;
        }

        if (this.Xmpp.Text.IsSet() && !ValidationHelper.IsValidXmpp(this.Xmpp.Text))
        {
            this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_XMPP"), MessageTypes.warning);
            return;
        }

        if (this.Facebook.Text.IsSet() && !ValidationHelper.IsValidURL(this.Facebook.Text))
        {
            this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_FACEBOOK"), MessageTypes.warning);
            return;
        }

        string displayName = null;

        if (this.PageBoardContext.BoardSettings.EnableDisplayName && this.PageBoardContext.BoardSettings.AllowDisplayNameModification)
        {
            // Check if name matches the required minimum length
            if (this.DisplayName.Text.Trim().Length < this.PageBoardContext.BoardSettings.DisplayNameMinLength)
            {
                this.PageBoardContext.Notify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.DisplayNameMinLength),
                    MessageTypes.warning);

                return;
            }

            // Check if name matches the required minimum length
            if (this.DisplayName.Text.Length > this.PageBoardContext.BoardSettings.UserNameMaxLength)
            {
                this.PageBoardContext.Notify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.UserNameMaxLength),
                    MessageTypes.warning);

                return;
            }

            if (this.DisplayName.Text.Trim() != this.User.Item1.DisplayName)
            {
                if (this.Get<IUserDisplayName>().FindUserByName(this.DisplayName.Text.Trim()) != null)
                {
                    this.PageBoardContext.Notify(
                        this.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"),
                        MessageTypes.warning);

                    return;
                }

                displayName = this.DisplayName.Text.Trim();
            }
        }

        if (this.Interests.Text.Trim().Length > 400)
        {
            this.PageBoardContext.Notify(
                this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "INTERESTS"), 400),
                MessageTypes.warning);

            return;
        }

        if (this.Occupation.Text.Trim().Length > 400)
        {
            this.PageBoardContext.Notify(
                this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "OCCUPATION"), 400),
                MessageTypes.warning);

            return;
        }

        this.UpdateUserProfile();

        // save display name
        this.GetRepository<User>().UpdateDisplayName(this.User.Item1, displayName);

        // Save Custom Profile
        if (this.CustomProfile.Visible)
        {
            this.GetRepository<ProfileCustom>().Delete(x => x.UserID == this.User.Item1.ID);

            this.CustomProfile.Items.Cast<RepeaterItem>().Where(x => x.ItemType is ListItemType.Item or ListItemType.AlternatingItem).ForEach(
                item =>
                    {
                        var id = item.FindControlAs<HiddenField>("DefID").Value.ToType<int>();
                        var profileDef = this.ProfileDefinitions.FirstOrDefault(x => x.ID == id);

                        var textBox = item.FindControlAs<TextBox>("DefText");
                        var check = item.FindControlAs<CheckBox>("DefCheck");

                        var type = profileDef.DataType.ToEnum<DataType>();

                        switch (type)
                        {
                            case DataType.Text:
                                {
                                    if (textBox.Text.IsSet())
                                    {
                                        this.GetRepository<ProfileCustom>().Insert(
                                            new ProfileCustom
                                                {
                                                    UserID = this.User.Item1.ID,
                                                    ProfileDefinitionID = profileDef.ID,
                                                    Value = textBox.Text
                                                });
                                    }

                                    break;
                                }
                            case DataType.Number:
                                {
                                    if (textBox.Text.IsSet())
                                    {
                                        this.GetRepository<ProfileCustom>().Insert(
                                            new ProfileCustom
                                                {
                                                    UserID = this.User.Item1.ID,
                                                    ProfileDefinitionID = profileDef.ID,
                                                    Value = textBox.Text
                                                });
                                    }

                                    break;
                                }
                            case DataType.Check:
                                {
                                    this.GetRepository<ProfileCustom>().Insert(
                                        new ProfileCustom
                                            {
                                                UserID = this.User.Item1.ID,
                                                ProfileDefinitionID = profileDef.ID,
                                                Value = check.Checked.ToString()
                                            });

                                    break;
                                }
                        }
                    });
        }

        // clear the cache for this user...)
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.User.Item1.ID));

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
        var userIpLocator = this.Get<IIpInfoService>().GetUserIpLocator(
            this.PageBoardContext.CurrentForumPage.IsAdminPage
                ? this.User.Item1.IP
                : this.Get<HttpRequestBase>().GetUserRealIPAddress());

        if (userIpLocator.CountryCode.IsSet() && !userIpLocator.CountryCode.Equals("-"))
        {
            var countryItem = this.Country.Items.FindByValue(userIpLocator.CountryCode);

            if (countryItem != null)
            {
                this.Country.ClearSelection();
                countryItem.Selected = true;
            }
        }

        if (userIpLocator.CityName.IsSet() && !userIpLocator.CityName.Equals("-"))
        {
            this.City.Text = userIpLocator.CityName;
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

        if (this.User.Item2.Profile_Country.IsSet())
        {
            this.LookForNewRegionsBind(this.User.Item2.Profile_Country);
        }

        this.DataBind();

        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
        {
            this.Birthday.Text =
                this.User.Item2.Profile_Birthday.HasValue &&
                this.User.Item2.Profile_Birthday.Value > DateTimeHelper.SqlDbMinTime()
                    ? PersianDateConverter.ToPersianDate(this.User.Item2.Profile_Birthday.Value).ToString("d")
                    : PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
        }
        else
        {
            this.Birthday.Text =
                this.User.Item2.Profile_Birthday.HasValue &&
                this.User.Item2.Profile_Birthday.Value > DateTimeHelper.SqlDbMinTime()
                    ? this.User.Item2.Profile_Birthday.Value.Date.ToString("yyyy-MM-dd")
                    : DateTimeHelper.SqlDbMinTime().Date.ToString("yyyy-MM-dd");

            this.Birthday.TextMode = TextBoxMode.Date;
        }

        this.Birthday.ToolTip = this.GetText("COMMON", "CAL_JQ_TT");

        this.DisplayName.Text = this.User.Item1.DisplayName;
        this.City.Text = this.User.Item2.Profile_City;
        this.Location.Text = this.User.Item2.Profile_Location;
        this.HomePage.Text = this.User.Item2.Profile_Homepage;
        this.Realname.Text = this.User.Item2.Profile_RealName;
        this.Occupation.Text = this.User.Item2.Profile_Occupation;
        this.Interests.Text = this.User.Item2.Profile_Interests;
        this.Weblog.Text = this.User.Item2.Profile_Blog;

        this.Facebook.Text = ValidationHelper.IsNumeric(this.User.Item2.Profile_Facebook)
                                 ? $"https://www.facebook.com/profile.php?id={this.User.Item2.Profile_Facebook}"
                                 : this.User.Item2.Profile_Facebook;

        this.Twitter.Text = this.User.Item2.Profile_Twitter;
        this.Xmpp.Text = this.User.Item2.Profile_XMPP;
        this.Skype.Text = this.User.Item2.Profile_Skype;
        this.Gender.SelectedIndex = this.User.Item2.Profile_Gender;

        if (this.User.Item2.Profile_Country.IsSet())
        {
            var countryItem = this.Country.Items.FindByValue(this.User.Item2.Profile_Country.Trim());
            if (countryItem != null)
            {
                countryItem.Selected = true;
            }
        }

        if (this.User.Item2.Profile_Region.IsSet())
        {
            var regionItem = this.Region.Items.FindByValue(this.User.Item2.Profile_Region.Trim());
            if (regionItem != null)
            {
                regionItem.Selected = true;
            }
        }

        if (!this.ProfileDefinitions.Any())
        {
            return;
        }

        this.CustomProfile.DataSource = this.ProfileDefinitions;
        this.CustomProfile.DataBind();

        if (!Config.IsDotNetNuke)
        {
            this.CustomProfile.Visible = true;
        }
    }

    /// <summary>
    /// Update user Profile Info.
    /// </summary>
    private void UpdateUserProfile()
    {
        var userProfile = new ProfileInfo
                              {
                                  Country = this.Country.SelectedItem != null ? this.Country.SelectedItem.Value.Trim() : string.Empty,
                                  Region =
                                      this.Region.SelectedItem != null && this.Country.SelectedItem != null &&
                                      this.Country.SelectedItem.Value.Trim().IsSet()
                                          ? this.Region.SelectedItem.Value.Trim()
                                          : string.Empty,
                                  City = this.City.Text.Trim(),
                                  Location = this.Location.Text.Trim(),
                                  Homepage = this.HomePage.Text.Trim(),
                                  Facebook = this.Facebook.Text.Trim(),
                                  Twitter = this.Twitter.Text.Trim(),
                                  XMPP = this.Xmpp.Text.Trim(),
                                  Skype = this.Skype.Text.Trim(),
                                  RealName = this.Realname.Text.Trim(),
                                  Occupation = this.Occupation.Text.Trim(),
                                  Interests = this.Interests.Text.Trim(),
                                  Gender = this.Gender.SelectedIndex,
                                  Blog = this.Weblog.Text.Trim()
                              };

        DateTime userBirthdate;

        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
        {
            try
            {
                var persianDate = new PersianDate(this.Birthday.Text.PersianNumberToEnglish());

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

        this.User.Item2.Profile_Birthday = userProfile.Birthday;
        this.User.Item2.Profile_Blog = userProfile.Blog;
        this.User.Item2.Profile_Gender = userProfile.Gender;
        this.User.Item2.Profile_GoogleId = userProfile.GoogleId;
        this.User.Item2.Profile_Homepage = userProfile.Homepage;
        this.User.Item2.Profile_Facebook = userProfile.Facebook;
        this.User.Item2.Profile_FacebookId = userProfile.FacebookId;
        this.User.Item2.Profile_Twitter = userProfile.Twitter;
        this.User.Item2.Profile_TwitterId = userProfile.TwitterId;
        this.User.Item2.Profile_Interests = userProfile.Interests;
        this.User.Item2.Profile_Location = userProfile.Location;
        this.User.Item2.Profile_Country = userProfile.Country;
        this.User.Item2.Profile_Region = userProfile.Region;
        this.User.Item2.Profile_City = userProfile.City;
        this.User.Item2.Profile_Occupation = userProfile.Occupation;
        this.User.Item2.Profile_RealName = userProfile.RealName;
        this.User.Item2.Profile_Skype = userProfile.Skype;
        this.User.Item2.Profile_XMPP = userProfile.XMPP;

        this.Get<IAspNetUsersHelper>().Update(this.User.Item2);
    }

    /// <summary>
    /// Looks for new regions bind.
    /// </summary>
    /// <param name="country">The country.</param>
    private void LookForNewRegionsBind(string country)
    {
        var dt = StaticDataHelper.Region(country);

        // The first row is empty
        if (dt.Any())
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
            if (this.User.Item1.LanguageFile.IsSet())
            {
                languageFile = this.User.Item1.LanguageFile;
            }

            if (this.User.Item1.Culture.IsSet())
            {
                culture4Tag = this.User.Item1.Culture;
            }
        }

        // Get first default full culture from a language file tag.
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
        return langFileCulture.Substring(0, 2) == culture4Tag.Substring(0, 2) ? culture4Tag : langFileCulture;
    }
}