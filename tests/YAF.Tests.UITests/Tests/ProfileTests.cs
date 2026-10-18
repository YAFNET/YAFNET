/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Tests.UITests.Tests;

using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The profile of a member: the custom profile fields of the admin, the checks of the input, the country and region.
/// </summary>
[TestFixture]
public class ProfileTests : ForumTestBase
{
    [Test]
    [Description("The custom profile fields of the admin are filled in by a member and shown on the profile page (the names are texts)")]
    public async Task Custom_Profile_Fields_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 6);
        var number = $"Shoe <b>size</b> {id}";
        var check = $"Likes <i>cats</i> {id}";
        var text = $"Motto {id}";
        var member = $"Prof{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.AddDefinitionAsync(number, "Number", required: true);
        await this.AddDefinitionAsync(check, "Check", required: false);
        await this.AddDefinitionAsync(text, "Text", required: false);

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");

            // the required field (also checked by the server, the browser checks it first)
            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true })
                .EvaluateAsync("e => e.removeAttribute('required')");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            (await this.Page.Locator("body").TextContentAsync()).Should().Contain($"{number} is required");

            // no number (the browser only accepts numbers)
            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true })
                .EvaluateAsync("e => e.type = 'text'");
            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).FillAsync("big");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            (await this.Page.Locator("body").TextContentAsync()).Should().Contain($"{number} must be a number");

            await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).FillAsync("42");
            await this.Page.GetByLabel(check, new PageGetByLabelOptions { Exact = true }).CheckAsync();
            await this.Page.GetByLabel(text, new PageGetByLabelOptions { Exact = true }).FillAsync("Hello <b>world</b>");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

            // the values are kept
            await this.GotoAsync("Profile/EditProfile");
            (await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).InputValueAsync()).Should().Be("42");
            (await this.Page.GetByLabel(check, new PageGetByLabelOptions { Exact = true }).IsCheckedAsync()).Should().BeTrue();

            // and shown on the profile page
            await this.OpenOwnProfileAsync();

            var profile = await this.Page.InnerTextAsync("body");
            profile.Should().Contain($"{number}:").And.Contain("42");
            profile.Should().Contain("Hello <b>world</b>", "the value is shown as text");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.DeleteDefinitionAsync(number);
            await this.DeleteDefinitionAsync(check);
            await this.DeleteDefinitionAsync(text);
        }
    }

    [Test]
    [Description("The details of the profile are shown on the profile page to another member: country, region, gender, the texts and the links")]
    public async Task Profile_Page_Details_Test()
    {
        var member = $"Det{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");

        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ProfileEditor_Country").SelectOptionAsync(
                "US",
                new LocatorSelectOptionOptions { Force = true }));
        await this.Control("ProfileEditor_Region").SelectOptionAsync("CA", new LocatorSelectOptionOptions { Force = true });
        await this.Control("ProfileEditor_Gender").SelectOptionAsync(
            new SelectOptionValue { Label = "Woman" },
            new LocatorSelectOptionOptions { Force = true });

        await this.Control("ProfileEditor_Realname").FillAsync("Real Name Of Member");
        await this.Control("ProfileEditor_Occupation").FillAsync("Tester Of Forums");
        await this.Control("ProfileEditor_Interests").FillAsync("Testing Things");
        await this.Control("ProfileEditor_City").FillAsync("Sacramento");
        await this.Control("ProfileEditor_Location").FillAsync("The Office");
        await this.Control("ProfileEditor_HomePage").FillAsync("https://www.example.com/home");
        await this.Control("ProfileEditor_Weblog").FillAsync("https://blog.example.com");
        await this.Control("ProfileEditor_Facebook").FillAsync("https://www.facebook.com/member");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        var href = await this.Page.Locator("a[href*='UserProfile']").First.GetAttributeAsync("href");
        var userId = Regex.Match(href, @"UserProfile/(\d+)").Groups[1].Value;

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync($"UserProfile/{userId}-{member}");

        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain("California").And.Contain("Woman").And.Contain("Real Name Of Member")
            .And.Contain("Tester Of Forums").And.Contain("Testing Things").And.Contain("Sacramento")
            .And.Contain("The Office");

        (await this.Page.Locator(".fi.fi-us").CountAsync()).Should().BeGreaterThan(0, "the flag of the country");
        (await this.Control("Home").GetAttributeAsync("href")).Should().Be("https://www.example.com/home");
        (await this.Control("Blog").GetAttributeAsync("href")).Should().Be("https://blog.example.com");
        (await this.Control("Blog").GetAttributeAsync("rel")).Should().Be("nofollow");
        (await this.Control("Facebook").GetAttributeAsync("href")).Should().Be("https://www.facebook.com/member");
    }

    [Test]
    [Description("With display names a too short and a taken display name are refused, a new one is shown; too long texts are refused")]
    public async Task Display_Name_And_Text_Length_Test()
    {
        var member = $"Dsp{DateTime.UtcNow:HHmmssfff}";
        var other = $"Dso{DateTime.UtcNow:HHmmssfff}";
        var displayName = $"Shown {Guid.NewGuid():N}".Substring(0, 16);

        await this.RegisterAsync(other, TestConfig.TestUserPassword);
        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.SetHostSettingsAsync("{ EnableDisplayName: true, AllowDisplayNameModification: true }");

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);

            // too short, taken, too long and with a semicolon
            foreach (var refused in new[] { "ab", other, new string('d', 51), "Semi;colon" })
            {
                await this.GotoAsync("Profile/EditProfile");
                await this.Control("ProfileEditor_DisplayName").EvaluateAsync("e => e.removeAttribute('maxlength')");
                await this.Control("ProfileEditor_DisplayName").FillAsync(refused);
                await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
                this.Page.Url.Should().Contain("EditProfile", refused);
            }

            // too long texts (the length of the fields is removed)
            foreach (var (field, length) in new[] { ("Interests", 4001), ("Occupation", 401) })
            {
                await this.GotoAsync("Profile/EditProfile");
                await this.Control($"ProfileEditor_{field}").EvaluateAsync("e => e.removeAttribute('maxlength')");
                await this.Control($"ProfileEditor_{field}").FillAsync(new string('x', length));
                await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
                this.Page.Url.Should().Contain("EditProfile", field);
            }

            await this.GotoAsync("Profile/EditProfile");
            await this.Control("ProfileEditor_DisplayName").FillAsync(displayName);
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

            await this.OpenOwnProfileAsync();
            (await this.Page.InnerTextAsync("body")).Should().Contain(displayName);
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync("{ EnableDisplayName: false, AllowDisplayNameModification: false }");
        }
    }

    [Test]
    [Description("A home page that is no address is refused, with a spam word it is logged as a bot (the profile is saved) or the member is deleted and banned")]
    public async Task Spam_Home_Page_Test()
    {
        var word = $"spamhome{Guid.NewGuid():N}".Substring(0, 16);
        var member = $"Sph{DateTime.UtcNow:HHmmssfff}";
        var bot = $"Spb{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(bot, TestConfig.TestUserPassword);
        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.GotoAsync("Admin/SpamWords");
        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Word" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);
        await this.Page.Locator("[id$='_EditDialog_Save']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
        await this.Control("EditDialog_spamword").FillAsync(word);
        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        // log the bots
        await this.SetHostSettingsAsync("{ BotHandlingOnRegister: '1' }");

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);

            // no address
            await this.GotoAsync("Profile/EditProfile");
            await this.Control("ProfileEditor_HomePage").FillAsync("https://not an address");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            this.Page.Url.Should().Contain("EditProfile");

            await this.GotoAsync("Profile/EditProfile");
            await this.Control("ProfileEditor_HomePage").FillAsync($"https://{word}.example.com");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

            await this.GotoAsync("Profile/EditProfile");
            (await this.Control("ProfileEditor_HomePage").InputValueAsync()).Should().Contain(word);

            // delete and ban the bots
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync("{ BotHandlingOnRegister: '2' }");

            await this.LoginAsync(bot, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");
            await this.Control("ProfileEditor_HomePage").FillAsync($"https://{word}.example.com");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            await this.AssertNoServerErrorAsync();

            await this.LoginAsync(bot, TestConfig.TestUserPassword);
            (await this.IsLoggedInAsAsync(bot)).Should().BeFalse("the bot is deleted");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync("{ BotHandlingOnRegister: '0' }");
        }
    }

    [Test]
    [Description("The registration with a display name (too short and taken ones are refused) and the custom fields of the registration, the admins get a notification")]
    public async Task Register_With_Display_Name_And_Fields_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var member = $"Rdn{id}";
        var other = $"Rdo{id}";
        var number = $"RegNumber{id}";
        var check = $"RegCheck{id}";
        var text = $"Reg<b>Text</b>{id}";
        var notify = $"notify{id}@localhost.com";

        await this.RegisterAsync(other, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.AddDefinitionAsync(number, "Number", required: false, onRegisterPage: true);
        await this.AddDefinitionAsync(check, "Check", required: false, onRegisterPage: true);
        await this.AddDefinitionAsync(text, "Text", required: false, onRegisterPage: true);
        await this.SetHostSettingsAsync("{ EnableDisplayName: true }");
        await this.SetNotificationListAsync(notify);

        try
        {
            // too short, taken, too long and with a semicolon
            foreach (var refused in new[] { "ab", other, new string('d', 51), "Semi;colon" })
            {
                await this.FillRegistrationAsync(member, refused);
                await this.ClickAndWaitAsync(this.Control("CreateUser"));
                (await this.Control("CreateUser").CountAsync()).Should().Be(1, $"the display name {refused} is refused");
            }

            // a too long user name
            await this.FillRegistrationAsync(new string('u', 51), $"Long {member}");
            await this.ClickAndWaitAsync(this.Control("CreateUser"));
            (await this.Control("CreateUser").CountAsync()).Should().Be(1, "the too long user name is refused");

            var started = DateTime.UtcNow;

            await this.FillRegistrationAsync(member, $"Shown {member}");
            await this.Page.Locator(".mb-3", new PageLocatorOptions { HasTextString = number }).Locator("[id$='_DefText']").FillAsync("42");
            await this.Page.Locator(".mb-3", new PageLocatorOptions { HasTextString = text }).Locator("[id$='_DefText']").FillAsync("A text");
            await this.Page.Locator("label", new PageLocatorOptions { HasTextString = check }).CheckAsync();
            await this.ClickAndWaitAsync(this.Control("CreateUser"));

            (await TestMail.WaitForAsync(notify, started)).Should().NotBeNull("the admins are notified");

            var mail = await TestMail.WaitForAsync(EmailOf(member), started);
            await this.GotoAsync(mail.GetLink("Approve"));

            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");
            (await this.Page.Locator("[id$='_DefText']").EvaluateAllAsync<string[]>("es => es.map(e => e.value)")).Should()
                .Contain("42").And.Contain("A text");

            // the login with the display name
            await this.LoginAsync($"Shown {member}", TestConfig.TestUserPassword);
            (await this.IsLoggedInAsAsync($"Shown {member}")).Should().BeTrue("the display name can be used to log in");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync("{ EnableDisplayName: false }");
            await this.SetNotificationListAsync(string.Empty);
            await this.DeleteDefinitionAsync(number);
            await this.DeleteDefinitionAsync(check);
            await this.DeleteDefinitionAsync(text);
        }
    }

    [Test]
    [Description("The addresses of the profile are checked, a home page without a scheme gets https")]
    public async Task Address_Checks_Test()
    {
        var member = $"Addr{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");

        await this.Control("ProfileEditor_Weblog").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_Weblog").FillAsync("not a blog");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("invalid address for your Blog");

        await this.Control("ProfileEditor_Weblog").FillAsync(string.Empty);
        await this.Control("ProfileEditor_Xmpp").FillAsync("no xmpp");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("invalid name for your XMPP account");

        await this.Control("ProfileEditor_Xmpp").FillAsync(string.Empty);
        await this.Control("ProfileEditor_Facebook").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_Facebook").FillAsync("not a profile");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("incorrect URL as Facebook Profile");

        await this.Control("ProfileEditor_Facebook").FillAsync(string.Empty);
        await this.Control("ProfileEditor_HomePage").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_HomePage").FillAsync("www.example.com");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");
        (await this.Control("ProfileEditor_HomePage").InputValueAsync()).Should().Be("https://www.example.com");
    }

    [Test]
    [Description("A javascript: address is no link of the profile: the Facebook profile refuses it, stored (e.g. imported) addresses are not shown")]
    public async Task Script_Links_Test()
    {
        var member = $"Link{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");

        await this.Control("ProfileEditor_Facebook").EvaluateAsync("e => e.type = 'text'");
        await this.Control("ProfileEditor_Facebook").FillAsync("javascript:alert(document.cookie)");
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
        (await this.Page.ContentAsync()).Should().Contain("incorrect URL as Facebook Profile");

        var href = await this.Page.Locator("a[href*='UserProfile']").First.GetAttributeAsync("href");
        var userId = Regex.Match(href, @"UserProfile/(\d+)").Groups[1].Value;

        ExecuteSql(
            "UPDATE [dbo].[yaf_AspNetUsers] SET [Profile_Facebook] = 'javascript:alert(1)', [Profile_Homepage] = 'javascript:alert(2)' WHERE [UserName] = @name",
            member);

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync($"UserProfile/{userId}-{member}");

        (await this.Page.Locator("a[href^='javascript:alert']").CountAsync()).Should().Be(0);
        (await this.Page.ContentAsync()).Should().NotContain("javascript:alert");
    }

    [Test]
    [Description("A country with regions shows the regions, the region of a newly selected country is saved, a country without regions has no region list")]
    public async Task Country_And_Region_Test()
    {
        var member = $"Ctry{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditProfile");

        (await this.Control("ProfileEditor_Region").CountAsync()).Should().Be(0, "no country is selected");

        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ProfileEditor_Country").SelectOptionAsync(
                "US",
                new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();

        await this.Control("ProfileEditor_Region").SelectOptionAsync("CA", new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");
        (await this.Control("ProfileEditor_Country").InputValueAsync()).Should().Be("US");
        (await this.Control("ProfileEditor_Region").InputValueAsync()).Should().Be("CA");

        // a country without regions
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("ProfileEditor_Country").SelectOptionAsync(
                "FR",
                new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();

        (await this.Control("ProfileEditor_Region").CountAsync()).Should().Be(0, "France has no regions");

        await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));

        await this.GotoAsync("Profile/EditProfile");
        (await this.Control("ProfileEditor_Country").InputValueAsync()).Should().Be("FR");
    }

    [Test]
    [Description("The custom profile fields without a value show the default values of the admin (profile and register page), the required fields are marked on the register page")]
    public async Task Default_Values_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 6);
        var number = $"Default number {id}";
        var check = $"Default check {id}";
        var text = $"Default text {id}";
        var member = $"Dflt{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        await this.AddDefinitionAsync(number, "Number", required: true, onRegisterPage: true, defaultValue: "7");
        await this.AddDefinitionAsync(check, "Check", required: false, onRegisterPage: true, defaultValue: "true");
        await this.AddDefinitionAsync(text, "Text", required: true, onRegisterPage: true, defaultValue: "Hello");

        try
        {
            // the register page (the default of the check field was ignored)
            await this.LogoutAsync();
            await this.GotoAsync("Account/Register");

            var registerNumber = this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true });
            (await registerNumber.InputValueAsync()).Should().Be("7");
            (await registerNumber.GetAttributeAsync("required")).Should().NotBeNull();
            (await this.Page.GetByLabel(check, new PageGetByLabelOptions { Exact = true }).IsCheckedAsync()).Should().BeTrue();
            (await this.Page.GetByLabel(text, new PageGetByLabelOptions { Exact = true }).InputValueAsync()).Should().Be("Hello");

            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");

            (await this.Page.GetByLabel(number, new PageGetByLabelOptions { Exact = true }).InputValueAsync()).Should().Be("7");
            (await this.Page.GetByLabel(check, new PageGetByLabelOptions { Exact = true }).IsCheckedAsync()).Should().BeTrue();
            (await this.Page.GetByLabel(text, new PageGetByLabelOptions { Exact = true }).InputValueAsync()).Should().Be("Hello");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.DeleteDefinitionAsync(number);
            await this.DeleteDefinitionAsync(check);
            await this.DeleteDefinitionAsync(text);
        }
    }

    [Test]
    [Description("A member with the persian culture enters the birthday in the persian calendar (with the setting), a profile without a birthday keeps it empty, a wrong date removes it")]
    public async Task Persian_Birthday_Test()
    {
        var member = $"Fa{DateTime.UtcNow:HHmmssfff}";
        var noBirthday = new DateTime(1903, 1, 1);

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        ExecuteSql("UPDATE [dbo].[yaf_User] SET [Culture] = 'fa-IR', [LanguageFile] = 'persian.json' WHERE [Name] = @name", member);

        await this.LoginAsAdminAsync();
        await this.SetHostSettingsAsync("{ UseFarsiCalender: true }");

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync("Profile/EditProfile");
            await this.AssertNoServerErrorAsync();

            (await this.Control("ProfileEditor_Birthday").GetAttributeAsync("type")).Should().NotBe("date", "the persian date is a text");

            // the profile without a birthday is saved (the empty date was saved as 2022)
            (await this.Control("ProfileEditor_Birthday").InputValueAsync()).Should().BeEmpty();
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            (Birthday(member) ?? noBirthday).Should().BeOnOrBefore(noBirthday);

            // 1 Farvardin 1370 is the 21 March 1991
            await this.GotoAsync("Profile/EditProfile");
            await this.Control("ProfileEditor_Birthday").FillAsync("1370/01/01");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            Birthday(member).Should().Be(new DateTime(1991, 3, 21));

            await this.GotoAsync("Profile/EditProfile");
            (await this.Control("ProfileEditor_Birthday").InputValueAsync()).Should().Contain("1370");

            // a wrong date removes the birthday (like a wrong date of the other calendars)
            await this.Control("ProfileEditor_Birthday").FillAsync("no date");
            await this.ClickAndWaitAsync(this.Control("ProfileEditor_UpdateProfile"));
            (Birthday(member) ?? noBirthday).Should().BeOnOrBefore(noBirthday);
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync("{ UseFarsiCalender: false }");
        }
    }

    private static DateTime? Birthday(string name)
    {
        using var connection = new SqlConnection(TestSite.ConnectionString);
        connection.Open();

        using var command = new SqlCommand(
            "SELECT [Profile_Birthday] FROM [dbo].[yaf_AspNetUsers] WHERE [UserName] = @name",
            connection);
        command.Parameters.AddWithValue("@name", name);

        return command.ExecuteScalar() as DateTime?;
    }

    private async Task AddDefinitionAsync(
        string name,
        string dataType,
        bool required,
        bool onRegisterPage = false,
        string defaultValue = null)
    {
        await this.GotoAsync("Admin/ProfileDefinitions");

        await this.ClickAndWaitAsync(
            this.Page.Locator("a", new PageLocatorOptions { HasTextString = "Add New Definition" })
                .Filter(new LocatorFilterOptions { Visible = true }).First);

        await this.Page.Locator("[id$='_EditDialog_Save']")
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Control("EditDialog_Name").FillAsync(name);
        await this.Control("EditDialog_DataTypes").SelectOptionAsync(dataType, new LocatorSelectOptionOptions { Force = true });
        await this.Control("EditDialog_Length").FillAsync("20");

        if (required)
        {
            await this.Control("EditDialog_Required").CheckAsync();
        }

        if (onRegisterPage)
        {
            await this.Control("EditDialog_ShowOnRegisterPage").CheckAsync();
        }

        if (defaultValue != null)
        {
            await this.Control("EditDialog_DefaultValue").FillAsync(defaultValue);
        }

        await this.ClickAndWaitAsync(this.Control("EditDialog_Save"));

        (await this.Page.InnerTextAsync("body")).Should().Contain(name);
    }

    private async Task DeleteDefinitionAsync(string name)
    {
        await this.GotoAsync("Admin/ProfileDefinitions");

        var row = this.Page.Locator(".list-group-item-menu", new PageLocatorOptions { HasTextString = name });

        if (await row.CountAsync() == 0)
        {
            return;
        }

        await row.Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
    }

    /// <summary>
    /// Sets values of the host settings (check boxes and lists, the fields are in hidden tabs).
    /// </summary>
    private async Task SetHostSettingsAsync(string values)
    {
        await this.GotoAsync("Admin/HostSettings");

        await this.Page.EvaluateAsync(
            $@"() => {{
                const values = {values};
                for (const [id, value] of Object.entries(values)) {{
                    const e = document.querySelector(`[id$='_${{id}}']`);
                    if (e.type === 'checkbox') {{ e.checked = value; }} else {{ e.value = value; }}
                }}
            }}");
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task FillRegistrationAsync(string userName, string displayName)
    {
        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");

        // the lengths are checked on the server
        await this.Page.EvaluateAsync("() => document.querySelectorAll('[maxlength]').forEach(e => e.removeAttribute('maxlength'))");
        await this.Control("UserName").FillAsync(userName);
        await this.Control("DisplayName").FillAsync(displayName);
        await this.Control("Email").FillAsync(EmailOf(userName));
        await this.Control("Password").FillAsync(TestConfig.TestUserPassword);
        await this.Control("ConfirmPassword").FillAsync(TestConfig.TestUserPassword);
    }

    /// <summary>
    /// Sets the addresses that are notified about new users (board settings).
    /// </summary>
    private async Task SetNotificationListAsync(string emails)
    {
        await this.GotoAsync("Admin/Settings");
        await this.Control("NotificationOnUserRegisterEmailList").EvaluateAsync("(e, v) => e.value = v", emails);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    private async Task OpenOwnProfileAsync()
    {
        var href = await this.Page.Locator("a[href*='UserProfile']").First.GetAttributeAsync("href");

        await this.GotoAsync(href.TrimStart('/'));
    }
}
