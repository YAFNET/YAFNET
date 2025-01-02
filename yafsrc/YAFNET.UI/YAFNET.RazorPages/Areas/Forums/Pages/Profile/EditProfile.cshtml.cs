/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using Microsoft.AspNetCore.Identity;

namespace YAF.Pages.Profile;

using System.Collections.Generic;
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
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

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
    /// Gets a value indicating whether this instance is farsi culture.
    /// </summary>
    /// <value><c>true</c> if this instance is farsi culture; otherwise, <c>false</c>.</value>
    public bool IsFarsiCulture => this.CurrentCultureInfo.IsFarsiCulture();

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditProfileInputModel Input { get; set; }

    /// <summary>
    /// Gets the current Culture information.
    /// </summary>
    /// <value>
    /// The current Culture information.
    /// </value>
    private CultureInfo CurrentCultureInfo {
        get {
            if (this.currentCultureInfo != null)
            {
                return this.currentCultureInfo;
            }

            this.currentCultureInfo = CultureInfoHelper.GetCultureByUser(
                this.PageBoardContext.BoardSettings,
                this.PageBoardContext.PageUser);

            return this.currentCultureInfo;
        }
    }

    /// <summary>
    /// Gets the User Data.
    /// </summary>
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
        this.Input = new EditProfileInputModel();

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
    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var userName = this.CurrentUser.Item1.DisplayOrUserName();

        if (this.Input.HomePage.IsSet())
        {
            // add http:// by default
            if (!Regex.IsMatch(this.Input.HomePage.Trim(), @"^(http|https|ftp|ftps|git|svn|news)\://.*",
                    RegexOptions.NonBacktracking))
            {
                this.Input.HomePage = $"https://{this.Input.HomePage.Trim()}";
            }

            if (!ValidationHelper.IsValidUrl(this.Input.HomePage))
            {
                return this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_HOME"), MessageTypes.warning,
                    ForumPages.Profile_EditProfile);
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
            return this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_WEBLOG"), MessageTypes.warning,
                ForumPages.Profile_EditProfile);
        }

        if (this.Input.Xmpp.IsSet() && !ValidationHelper.IsValidXmpp(this.Input.Xmpp))
        {
            return this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_XMPP"), MessageTypes.warning,
                ForumPages.Profile_EditProfile);
        }

        if (this.Input.Facebook.IsSet() && !ValidationHelper.IsValidUrl(this.Input.Facebook))
        {
            return this.PageBoardContext.SessionNotify(this.GetText("PROFILE", "BAD_FACEBOOK"), MessageTypes.warning,
                ForumPages.Profile_EditProfile);
        }

        string displayName = null;

        if (this.PageBoardContext.BoardSettings.EnableDisplayName &&
            this.PageBoardContext.BoardSettings.AllowDisplayNameModification)
        {
            // Check if name matches the required minimum length
            if (this.Input.DisplayName.Trim().Length < this.PageBoardContext.BoardSettings.DisplayNameMinLength)
            {
                return this.PageBoardContext.SessionNotify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.DisplayNameMinLength),
                    MessageTypes.warning, ForumPages.Profile_EditProfile);
            }

            // Check if name matches the required minimum length
            if (this.Input.DisplayName.Length > this.PageBoardContext.BoardSettings.UserNameMaxLength)
            {
                return this.PageBoardContext.SessionNotify(
                    this.GetTextFormatted("USERNAME_TOOLONG", this.PageBoardContext.BoardSettings.UserNameMaxLength),
                    MessageTypes.warning, ForumPages.Profile_EditProfile);
            }

            if (this.Input.DisplayName.Trim() != this.CurrentUser.Item1.DisplayName)
            {
                if (this.Get<IUserDisplayName>().FindUserByName(this.Input.DisplayName.Trim()) != null)
                {
                    return this.PageBoardContext.SessionNotify(
                        this.GetText("REGISTER", "ALREADY_REGISTERED_DISPLAYNAME"),
                        MessageTypes.warning, ForumPages.Profile_EditProfile);
                }

                displayName = this.Input.DisplayName.Trim();
            }
        }

        if (this.Input.Interests.IsSet() && this.Input.Interests.Trim().Length > 4000)
        {
            return this.PageBoardContext.SessionNotify(
                this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "INTERESTS"), 4000),
                MessageTypes.warning, ForumPages.Profile_EditProfile);
        }

        if (this.Input.Occupation.IsSet() && this.Input.Occupation.Trim().Length > 400)
        {
            return this.PageBoardContext.SessionNotify(
                this.GetTextFormatted("FIELD_TOOLONG", this.GetText("EDIT_PROFILE", "OCCUPATION"), 400),
                MessageTypes.warning, ForumPages.Profile_EditProfile);
        }

        await this.UpdateUserProfileAsync();

        // save display name
        this.GetRepository<User>().UpdateDisplayName(this.CurrentUser.Item1, displayName);

        this.SaveCustomProfile();

        // clear the cache for this user...
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
        this.Input.Birthday =
            this.CurrentUser.Item2.Profile_Birthday.HasValue && this.CurrentUser.Item2.Profile_Birthday.Value >
            DateTimeHelper.SqlDbMinTime()
                ? this.CurrentUser.Item2.Profile_Birthday.Value.Date.ToString("yyyy-MM-dd")
                : DateTimeHelper.SqlDbMinTime().Date.ToString("yyyy-MM-dd");

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
        this.Input.CustomProfile = [.. this.GetRepository<ProfileDefinition>().GetByBoardId()];

        if (this.Input.CustomProfile is null || this.Input.CustomProfile.Count == 0)
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
    private void LoadGenders(int genderSelectedItem)
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
    private void LoadCountriesAndRegions(string countryCode)
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
            this.Regions = [];
        }

        this.Countries = countries;
    }

    /// <summary>
    /// Looks for new regions bind.
    /// </summary>
    /// <param name="country">
    /// The country.
    /// </param>
    private static List<SelectListItem> LookForNewRegionsBind(string country)
    {
        return [.. StaticDataHelper.Regions(country)];
    }

    /// <summary>
    /// Update user Profile Info.
    /// </summary>
    private Task<IdentityResult> UpdateUserProfileAsync()
    {
        var userProfile = new ProfileInfo {
                                              Country = this.Input.Country,
                                              Region = this.Input.Region.IsSet()
                                                           ? this.Input.Region.Trim()
                                                           : string.Empty,
                                              City = this.Input.City.IsSet() ? this.Input.City.Trim() : null,
                                              Location =
                                                  this.Input.Location.IsSet() ? this.Input.Location.Trim() : null,
                                              Homepage =
                                                  this.Input.HomePage.IsSet() ? this.Input.HomePage.Trim() : null,
                                              Facebook =
                                                  this.Input.Facebook.IsSet() ? this.Input.Facebook.Trim() : null,
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
        user.Profile_Interests = userProfile.Interests;
        user.Profile_Location = userProfile.Location;
        user.Profile_Country = userProfile.Country;
        user.Profile_Region = userProfile.Region;
        user.Profile_City = userProfile.City;
        user.Profile_Occupation = userProfile.Occupation;
        user.Profile_RealName = userProfile.RealName;
        user.Profile_Skype = userProfile.Skype;
        user.Profile_XMPP = userProfile.XMPP;

        return this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);
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
                var type = item.DataType.ToEnum<DataType>();

                switch (type)
                {
                    case DataType.Number:
                    case DataType.Text:
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

                    case DataType.Check:
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
}