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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Profile;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FarsiLibrary.Utils;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Attributes;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

using DataType = System.ComponentModel.DataAnnotations.DataType;

/// <summary>
/// The edit profile page
/// </summary>
public class EditProfileModel : ProfilePage
{
    /// <summary>
    /// The current culture information
    /// </summary>
    private CultureInfo currentCultureInfo;

    /// <summary>
    ///   Initializes a new instance of the <see cref = "EditProfileModel" /> class.
    /// </summary>
    public EditProfileModel()
        : base("EDIT_PROFILE", ForumPages.Profile_EditProfile)
    {
    }

    /// <summary>
    /// Gets or sets the genders.
    /// </summary>
    [BindProperty]
    public List<SelectListItem> Genders { get; set; }

    /// <summary>
    /// Gets or sets the countries.
    /// </summary>
    public List<SelectListItem> Countries { get; set; }

    /// <summary>
    /// Gets or sets the regions.
    /// </summary>
    public List<SelectListItem> Regions { get; set; }

    /// <summary>
    /// Gets or sets the user profile custom.
    /// </summary>
    [BindProperty]
    public IEnumerable<ProfileCustom> UserProfileCustom { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets the current Culture information.
    /// </summary>
    /// <value>
    /// The current Culture information.
    /// </value>
    [NotNull]
    private CultureInfo CurrentCultureInfo {
        get {
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
    private Tuple<User, AspNetUsers, Rank, VAccess> CurrentUser =>
        this.Get<IAspNetUsersHelper>().GetBoardUser(this.PageBoardContext.PageUserID);

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("EDIT_PROFILE", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Page Load
    /// </summary>
    public IActionResult OnGet()
    {
        this.Input = new InputModel();

        this.UserProfileCustom = this.GetRepository<ProfileCustom>()
            .Get(p => p.UserID == this.PageBoardContext.PageUserID);

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Update Country
    /// </summary>
    public void OnPost()
    {
        this.UserProfileCustom = this.GetRepository<ProfileCustom>()
            .Get(p => p.UserID == this.PageBoardContext.PageUserID);

        this.LoadCountriesAndRegions(this.Input.Country);
    }

    /// <summary>
    /// Save the Profile
    /// </summary>
    public IActionResult OnPostUpdateProfile()
    {
        var userName = this.CurrentUser.Item1.DisplayOrUserName();

        if (this.Input.HomePage.IsSet())
        {
            // add http:// by default
            if (!Regex.IsMatch(this.Input.HomePage.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*", RegexOptions.NonBacktracking))
            {
                this.Input.HomePage = $"https://{this.Input.HomePage.Trim()}";
            }

            if (!ValidationHelper.IsValidUrl(this.Input.HomePage))
            {
                return this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_HOME"), MessageTypes.warning);
            }

            if (this.CurrentUser.Item1.NumPosts < this.PageBoardContext.BoardSettings.IgnoreSpamWordCheckPostCount)
            {
                // Check for spam
                if (this.Get<ISpamWordCheck>().CheckForSpamWord(this.Input.HomePage, out _))
                {
                    switch (this.PageBoardContext.BoardSettings.BotHandlingOnRegister)
                    {
                        // Log and Send Message to Admins
                        case 1:
                            this.Get<ILogger<EditProfileModel>>().Log(
                                null,
                                "Bot Detected",
                                $"Internal Spam Word Check detected a SPAM BOT: (user name : '{userName}', user id : '{this.PageBoardContext.PageUserID}') after the user changed the profile Homepage url to: {this.Input.HomePage}",
                                EventLogTypes.SpamBotDetected);
                            break;
                        case 2:
                        {
                            this.Get<ILogger<EditProfileModel>>().Log(
                                null,
                                "Bot Detected",
                                $"Internal Spam Word Check detected a SPAM BOT: (user name : '{userName}', user id : '{this.PageBoardContext.PageUserID}') after the user changed the profile Homepage url to: {this.Input.HomePage}, user was deleted and the name, email and IP Address are banned.",
                                EventLogTypes.SpamBotDetected);

                            // Kill user
                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                this.PageBoardContext.PageUser,
                                this.CurrentUser.Item2,
                                this.CurrentUser.Item1.IP);

                            break;
                        }
                    }
                }
            }
        }

        if (this.Input.Blog.IsSet() && !ValidationHelper.IsValidUrl(this.Input.Blog.Trim()))
        {
            return this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_WEBLOG"), MessageTypes.warning);
        }

        if (this.Input.Xmpp.IsSet() && !ValidationHelper.IsValidXmpp(this.Input.Xmpp))
        {
            return this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_XMPP"), MessageTypes.warning);
        }

        if (this.Input.Facebook.IsSet() && !ValidationHelper.IsValidUrl(this.Input.Facebook))
        {
            return this.PageBoardContext.Notify(this.GetText("PROFILE", "BAD_FACEBOOK"), MessageTypes.warning);
        }

        string displayName = null;

        if (this.PageBoardContext.BoardSettings.EnableDisplayName && this.PageBoardContext.BoardSettings.AllowDisplayNameModification)
        {
            // Check if name matches the required minimum length
            if (this.Input.DisplayName.Trim().Length < this.PageBoardContext.BoardSettings.DisplayNameMinLength)
            {
                return this.PageBoardContext.Notify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.DisplayNameMinLength),
                    MessageTypes.warning);            }

            // Check if name matches the required minimum length
            if (this.Input.DisplayName.Length > this.PageBoardContext.BoardSettings.UserNameMaxLength)
            {
                return this.PageBoardContext.Notify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.UserNameMaxLength),
                    MessageTypes.warning);
            }

            if (this.Input.DisplayName.Trim() != this.CurrentUser.Item1.DisplayName)
            {
                if (this.Get<IUserDisplayName>().FindUserByName(this.Input.DisplayName.Trim()) != null)
                {
                    return this.PageBoardContext.Notify(
                        this.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"),
                        MessageTypes.warning);
                }

                displayName = this.Input.DisplayName.Trim();
            }
        }

        if (this.Input.Interests.IsSet() && this.Input.Interests.Trim().Length > 400)
        {
            return this.PageBoardContext.Notify(
                this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "INTERESTS"), 400),
                MessageTypes.warning);
        }

        if (this.Input.Occupation.IsSet() && this.Input.Occupation.Trim().Length > 400)
        {
            return this.PageBoardContext.Notify(
                this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "OCCUPATION"), 400),
                MessageTypes.warning);
        }

        this.UpdateUserProfile();

        // save display name
        this.GetRepository<User>().UpdateDisplayName(this.CurrentUser.Item1, displayName);

        this.SaveCustomProfile();

        // clear the cache for this user...)
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        this.Get<IDataCache>().Clear();

        return this.Get<LinkBuilder>().Redirect(ForumPages.MyAccount);
    }

    /// <summary>
    /// Get Location via IP Address
    /// </summary>
    public async Task OnPostGetLocationAsync()
    {
        var userIpLocator = await this.Get<IIpInfoService>().GetUserIpLocatorAsync(this.Request.GetUserRealIPAddress());

        this.LoadCountriesAndRegions(userIpLocator.CountryCode);

        if (userIpLocator.CityName.IsSet() && !userIpLocator.CityName.Equals("-"))
        {
            this.Input.City = userIpLocator.CityName;
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
        {
            this.Input.Birthday =
                this.CurrentUser.Item2.Profile_Birthday.HasValue && this.CurrentUser.Item2.Profile_Birthday.Value >
                DateTimeHelper.SqlDbMinTime()
                    ? PersianDateConverter.ToPersianDate(this.CurrentUser.Item2.Profile_Birthday.Value)
                        .ToString("d")
                    : PersianDateConverter.ToPersianDate(PersianDate.MinValue).ToString("d");
        }
        else
        {
            this.Input.Birthday =
                this.CurrentUser.Item2.Profile_Birthday.HasValue && this.CurrentUser.Item2.Profile_Birthday.Value >
                DateTimeHelper.SqlDbMinTime()
                    ? this.CurrentUser.Item2.Profile_Birthday.Value.Date.ToString("yyyy-MM-dd")
                    : DateTimeHelper.SqlDbMinTime().Date.ToString("yyyy-MM-dd");
        }

        this.Input.DisplayName = this.CurrentUser.Item1.DisplayName;
        this.Input.City = this.CurrentUser.Item2.Profile_City;
        this.Input.Location = this.CurrentUser.Item2.Profile_Location;
        this.Input.HomePage = this.CurrentUser.Item2.Profile_Homepage;
        this.Input.RealName = this.CurrentUser.Item2.Profile_RealName;
        this.Input.Occupation = this.CurrentUser.Item2.Profile_Occupation;
        this.Input.Interests = this.CurrentUser.Item2.Profile_Interests;
        this.Input.Blog = this.CurrentUser.Item2.Profile_Blog;

        this.Input.Facebook = ValidationHelper.IsNumeric(this.CurrentUser.Item2.Profile_Facebook)
                                  ? $"https://www.facebook.com/profile.php?id={this.CurrentUser.Item2.Profile_Facebook}"
                                  : this.CurrentUser.Item2.Profile_Facebook;

        this.Input.Twitter = this.CurrentUser.Item2.Profile_Twitter;
        this.Input.Xmpp = this.CurrentUser.Item2.Profile_XMPP;
        this.Input.Skype = this.CurrentUser.Item2.Profile_Skype;

        this.LoadCountriesAndRegions(this.CurrentUser.Item2.Profile_Country);

        this.LoadGenders(this.CurrentUser.Item2.Profile_Gender);

        this.LoadCustomProfile();
    }

    /// <summary>
    /// Load the custom profile.
    /// </summary>
    private void LoadCustomProfile()
    {
        this.Input.CustomProfile = this.GetRepository<ProfileDefinition>().GetByBoardId().ToList();
            
        if (this.Input.CustomProfile == null || !this.Input.CustomProfile.Any())
        {
            return;
        }

        foreach (var profileDef in this.Input.CustomProfile)
        {
            var userValue = this.UserProfileCustom.FirstOrDefault(p => p.ProfileDefinitionID == profileDef.ID);

            if (userValue != null)
            {
                profileDef.Value = userValue.Value;
            }
            else
            {
                if (profileDef.DefaultValue.IsSet())
                {
                    profileDef.Value = profileDef.DefaultValue;
                }
            }
        }
    }

    /// <summary>
    /// The load genders.
    /// </summary>
    /// <param name="genderSelectedItem">
    /// The gender selected item.
    /// </param>
    private void LoadGenders([CanBeNull] int genderSelectedItem)
    {
        var genders = StaticDataHelper.Gender().ToList();

        try
        {
            genders[genderSelectedItem].Selected = true;
        }
        catch (Exception)
        {
            genders[0].Selected = true;
        }

        this.Genders = genders;
    }

    /// <summary>
    /// The load countries and regions.
    /// </summary>
    /// <param name="countryCode">
    /// The country code.
    /// </param>
    private void LoadCountriesAndRegions([CanBeNull] string countryCode)
    {
        var countries = StaticDataHelper.Countries().ToList();

        if (countryCode.IsSet())
        {
            countries.ForEach(
                country =>
                {
                    if (country.Value == countryCode)
                    {
                        country.Selected = true;
                    }
                });

            var regions = LookForNewRegionsBind(countryCode);

            if (this.CurrentUser.Item2.Profile_Region.IsSet())
            {
                regions.ForEach(
                    region =>
                    {
                        if (region.Value == this.CurrentUser.Item2.Profile_Region.Trim())
                        {
                            region.Selected = true;
                        }
                    });
            }

            this.Regions = regions;
        }
        else
        {
            this.Regions = new List<SelectListItem>();
        }

        this.Countries = countries;
    }

    /// <summary>
    /// Looks for new regions bind.
    /// </summary>
    /// <param name="country">
    /// The country.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    private static List<SelectListItem> LookForNewRegionsBind(string country)
    {
        return StaticDataHelper.Regions(country).ToList();
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
            if (this.CurrentUser.Item1.LanguageFile.IsSet())
            {
                languageFile = this.CurrentUser.Item1.LanguageFile;
            }

            if (this.CurrentUser.Item1.Culture.IsSet())
            {
                culture4Tag = this.CurrentUser.Item1.Culture;
            }
        }

        // Get first default full culture from a language file tag.
        var langFileCulture = StaticDataHelper.CultureDefaultFromFile(languageFile);
        return langFileCulture[..2] == culture4Tag[..2] ? culture4Tag : langFileCulture;
    }

    /// <summary>
    /// Update user Profile Info.
    /// </summary>
    private void UpdateUserProfile()
    {
        var userProfile = new ProfileInfo {
                                              Country = this.Input.Country,
                                              Region = this.Input.Region.IsSet() && this.Input.Region.IsSet()
                                                           ? this.Input.Region
                                                           : string.Empty,
                                              City = this.Input.City.IsSet() ? this.Input.City.Trim() : null,
                                              Location =
                                                  this.Input.Location.IsSet() ? this.Input.Location.Trim() : null,
                                              Homepage =
                                                  this.Input.HomePage.IsSet() ? this.Input.HomePage.Trim() : null,
                                              Facebook =
                                                  this.Input.Facebook.IsSet() ? this.Input.Facebook.Trim() : null,
                                              Twitter = this.Input.Twitter.IsSet() ? this.Input.Twitter.Trim() : null,
                                              XMPP = this.Input.Xmpp.IsSet() ? this.Input.Xmpp.Trim() : null,
                                              Skype = this.Input.Skype.IsSet() ? this.Input.Skype.Trim() : null,
                                              RealName =
                                                  this.Input.RealName.IsSet() ? this.Input.RealName.Trim() : null,
                                              Occupation =
                                                  this.Input.Occupation.IsSet() ? this.Input.Occupation.Trim() : null,
                                              Interests = this.Input.Interests.IsSet()
                                                              ? this.Input.Interests.Trim()
                                                              : null,
                                              Gender = this.Genders.FindIndex(g => g.Value == this.Input.Gender),
                                              Blog = this.Input.Blog.IsSet() ? this.Input.Blog.Trim() : null
                                          };

        DateTime userBirthdate;

        if (this.PageBoardContext.BoardSettings.UseFarsiCalender && this.CurrentCultureInfo.IsFarsiCulture())
        {
            try
            {
                var persianDate = new PersianDate(this.Input.Birthday.PersianNumberToEnglish());

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
            DateTime.TryParse(this.Input.Birthday, this.CurrentCultureInfo, DateTimeStyles.None, out userBirthdate);

            if (userBirthdate >= DateTimeHelper.SqlDbMinTime().Date)
            {
                // Attention! This is stored in profile in the user timezone date
                userProfile.Birthday = userBirthdate.Date;
            }
        }

        var user = this.CurrentUser.Item2;

        user.Profile_Birthday = userProfile.Birthday;
        user.Profile_Blog = userProfile.Blog;
        user.Profile_Gender = userProfile.Gender;
        user.Profile_GoogleId = userProfile.GoogleId;
        user.Profile_Homepage = userProfile.Homepage;
        user.Profile_Facebook = userProfile.Facebook;
        user.Profile_FacebookId = userProfile.FacebookId;
        user.Profile_Twitter = userProfile.Twitter;
        user.Profile_TwitterId = userProfile.TwitterId;
        user.Profile_Interests = userProfile.Interests;
        user.Profile_Location = userProfile.Location;
        user.Profile_Country = userProfile.Country;
        user.Profile_Region = userProfile.Region;
        user.Profile_City = userProfile.City;
        user.Profile_Occupation = userProfile.Occupation;
        user.Profile_RealName = userProfile.RealName;
        user.Profile_Skype = userProfile.Skype;
        user.Profile_XMPP = userProfile.XMPP;

        this.Get<IAspNetUsersHelper>().Update(user);
    }

    /// <summary>
    /// Saves the custom profile.
    /// </summary>
    private void SaveCustomProfile()
    {
        if (this.Input.CustomProfile.NullOrEmpty())
        {
            return;
        }

        this.GetRepository<ProfileCustom>().Delete(x => x.UserID == this.PageBoardContext.PageUserID);

        this.Input.CustomProfile.ForEach(
            item =>
            {
                var type = item.DataType.ToEnum<Types.Constants.DataType>();

                switch (type)
                {
                    case Types.Constants.DataType.Text:
                    {
                        if (item.Value.IsSet())
                        {
                            this.GetRepository<ProfileCustom>().Insert(
                                new ProfileCustom
                                {
                                    UserID = this.PageBoardContext.PageUserID,
                                    ProfileDefinitionID = item.ID,
                                    Value = item.Value
                                });
                        }

                        break;
                    }

                    case Types.Constants.DataType.Number:
                    {
                        if (item.Value.IsSet())
                        {
                            this.GetRepository<ProfileCustom>().Insert(
                                new ProfileCustom
                                {
                                    UserID = this.PageBoardContext.PageUserID,
                                    ProfileDefinitionID = item.ID,
                                    Value = item.Value
                                });
                        }

                        break;
                    }

                    case Types.Constants.DataType.Check:
                    {
                        this.GetRepository<ProfileCustom>().Insert(
                            new ProfileCustom
                            {
                                UserID = this.PageBoardContext.PageUserID,
                                ProfileDefinitionID = item.ID,
                                Value = item.Value
                            });

                        break;
                    }
                }
            });
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public string DisplayName { get; set; }

        public string RealName { get; set; }

        [DataType(DataType.Date)]
        public string Birthday { get; set; }

        [BindProperty, MaxLength(400)]
        public string Occupation { get; set; }

        [BindProperty, MaxLength(400)]
        public string Interests { get; set; }

        public string Gender { get; set; }

        public string Location { get; set; }

        public string Country { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        [DataType(DataType.Url)]
        public string HomePage { get; set; }

        [DataType(DataType.Url)]
        public string Blog { get; set; }

        public string Twitter { get; set; }

        public string Xmpp { get; set; }

        public string Skype { get; set; }

        public string Facebook { get; set; }

        public List<ProfileDefinition> CustomProfile { get; set; }
    }
}