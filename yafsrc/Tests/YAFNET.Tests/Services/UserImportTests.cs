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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Services;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Autofac;

using Microsoft.AspNetCore.Identity;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Services.Import;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The admin user import (<see cref="DataImporter.ImportingUsersAsync"/>) from the xml export and from csv files.
/// The identity layer is substituted, forum users are created in the in-memory database.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserImportTests
{
    private TestBoardContext board;

    private IAspNetUsersHelper users;

    private IAspNetRolesHelper roles;

    private List<AspNetUsers> created;

    private IDataImporter importer;

    [SetUp]
    public void SetUp()
    {
        this.users = Substitute.For<IAspNetUsersHelper>();
        this.roles = Substitute.For<IAspNetRolesHelper>();
        this.created = [];

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<DataImporter>().As<IDataImporter>();
                b.RegisterInstance(this.users).As<IAspNetUsersHelper>();
                b.RegisterInstance(this.roles).As<IAspNetRolesHelper>();
            },
            useDatabase: true);

        // the identity store: names and e-mails of created users are taken
        this.users.GetUserByNameAsync(Arg.Any<string>())
            .Returns(c => this.created.Find(u => u.UserName == c.Arg<string>()));
        this.users.GetUserByEmailAsync(Arg.Any<string>())
            .Returns(c => this.created.Find(u => u.Email == c.Arg<string>()));
        this.users.CreateUserAsync(Arg.Any<AspNetUsers>(), Arg.Any<string>()).Returns(
            c =>
            {
                this.created.Add(c.Arg<AspNetUsers>());
                return IdentityResult.Success;
            });

        this.roles.CreateForumUserAsync(Arg.Any<AspNetUsers>(), Arg.Any<int>()).Returns(
            c => (int?)this.board.Database.AddUser(
                c.Arg<AspNetUsers>().UserName,
                u =>
                {
                    // like CreateForumUserAsync in production
                    u.Email = c.Arg<AspNetUsers>().Email;
                    u.TimeZone = TimeZoneInfo.Local.Id;
                }));

        this.board.BoardSettings.Registry["ApplicationId"] = Guid.NewGuid().ToString();

        this.importer = this.board.Resolve<IDataImporter>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The xml file of the admin user export can be imported, the users get a password mail")]
    public async Task Import_Admin_Export_Xml_Test()
    {
        var export = new List<User>
        {
            new() { Name = "alice", Email = "alice@example.com", LanguageFile = "german.json", Culture = "de-DE", TimeZone = "Tokyo Standard Time" },
            new() { Name = "bob", Email = "bob@example.com" }
        };

        var count = await this.importer.ImportingUsersAsync(Xml(export), true);

        count.Should().Be(2);
        this.created.Select(u => u.UserName).Should().Equal("alice", "bob");
        this.created.Should().OnlyContain(u => u.IsApproved);

        var alice = this.ForumUser("alice");
        alice.LanguageFile.Should().Be("german.json");
        alice.Culture.Should().Be("de-DE");
        alice.TimeZone.Should().Be("Tokyo Standard Time", "the time zone of the export is kept");
        this.ForumUser("bob").TimeZone.Should().Be(TimeZoneInfo.Local.Id, "without a time zone the default stays");

        await this.board.Notifications.Received(2).SendRegistrationNotificationToUserAsync(
            Arg.Any<AspNetUsers>(),
            Arg.Is<string>(p => p.Length == 16),
            "NOTIFICATION_ON_REGISTER");
        await this.roles.Received(2).SetupUserRolesAsync(TestDatabase.BoardId, Arg.Any<AspNetUsers>());
    }

    [Test]
    [Description("Users whose name or e-mail already exists are skipped")]
    public async Task Import_Skips_Existing_Users_Test()
    {
        this.created.Add(new AspNetUsers { UserName = "alice", Email = "alice@example.com" });
        this.created.Add(new AspNetUsers { UserName = "carol", Email = "taken@example.com" });

        var export = new List<User>
        {
            new() { Name = "alice", Email = "other@example.com" },
            new() { Name = "bob", Email = "taken@example.com" },
            new() { Name = "dave", Email = "dave@example.com" },
            new() { Name = "dave", Email = "dave2@example.com" }
        };

        (await this.importer.ImportingUsersAsync(Xml(export), true)).Should().Be(1);
        this.created.Select(u => u.UserName).Should().Equal("alice", "carol", "dave");
    }

    [Test]
    [Description("An xml file without users is rejected")]
    public async Task Import_Wrong_Xml_Test()
    {
        await FluentActions.Awaiting(() => this.importer.ImportingUsersAsync(Text("<Other><Row /></Other>"), true))
            .Should().ThrowAsync<FormatException>();
    }

    [Test]
    [Description("csv files: profile fields are imported, quoted values may contain commas")]
    public async Task Import_Csv_Profile_Test()
    {
        const string Csv = """
                           Name,Email,RealName,Gender,Birthday,City,Homepage,Timezone,ThemeFile
                           alice,alice@example.com,"Smith, Alice",2,1990-05-17,Berlin,https://alice.example,W. Europe Standard Time,dark.css
                           bob,bob@example.com,,,,,,,
                           """;

        var count = await this.importer.ImportingUsersAsync(Text(Csv), false);

        count.Should().Be(2);

        var alice = this.created.Single(u => u.UserName == "alice");
        alice.Profile_RealName.Should().Be("Smith, Alice");
        alice.Profile_Gender.Should().Be(2);
        alice.Profile_Birthday.Should().Be(new DateTime(1990, 5, 17));
        alice.Profile_City.Should().Be("Berlin");
        alice.Profile_Homepage.Should().Be("https://alice.example");

        var forumAlice = this.ForumUser("alice");
        forumAlice.TimeZone.Should().Be("W. Europe Standard Time");
        forumAlice.ThemeFile.Should().Be("dark.css");

        var bob = this.created.Single(u => u.UserName == "bob");
        bob.Profile_RealName.Should().BeNull();
        bob.Profile_Birthday.Should().Be(DateTimeHelper.SqlDbMinTime(), "the profile default means no birthday");
    }

    [Test]
    [Description("The profile values of a csv file have to fit into the columns (other databases than Sqlite refuse longer values), a country is only taken as its code")]
    public async Task Import_Csv_Too_Long_Values_Test()
    {
        var csv = $"Name,Email,RealName,Interests,Occupation,Country,Region{Environment.NewLine}"
                  + $"alice,alice@example.com,{new string('r', 300)},{new string('i', 5000)},{new string('o', 500)},Germany,Bavaria";

        var count = await this.importer.ImportingUsersAsync(Text(csv), false);

        count.Should().Be(1);

        var alice = this.created.Single();
        alice.Profile_RealName.Length.Should().BeLessThanOrEqualTo(255);
        alice.Profile_Interests.Length.Should().BeLessThanOrEqualTo(4000);
        alice.Profile_Occupation.Length.Should().BeLessThanOrEqualTo(400);
        alice.Profile_Country.Should().BeNull("the country column has 2 characters, only a known country code is taken");
        alice.Profile_Region.Should().BeNull();
    }

    [Test]
    [Description("csv files with an empty line at the end or rows with missing columns can be imported")]
    public async Task Import_Csv_Empty_Line_And_Short_Rows_Test()
    {
        const string Csv = "Name,Email,RealName,City\r\nalice,alice@example.com\r\nbob,bob@example.com,Bob,Rome\r\n\r\n";

        var count = await this.importer.ImportingUsersAsync(Text(Csv), false);

        count.Should().Be(2);
        this.created.Single(u => u.UserName == "bob").Profile_City.Should().Be("Rome");
        this.created.Single(u => u.UserName == "alice").Profile_City.Should().BeNull();
    }

    [Test]
    [Description("Rows without user name or e-mail are skipped")]
    public async Task Import_Csv_Missing_Name_Or_Email_Test()
    {
        var count = await this.importer.ImportingUsersAsync(Text("Name,Email\r\n,nobody@example.com\r\nnomail,\r\nalice,alice@example.com"), false);

        count.Should().Be(1);
        this.created.Single().UserName.Should().Be("alice");
        this.created.Single().LoweredUserName.Should().Be("alice");
    }

    [Test]
    [Description("A csv file without the required columns is rejected")]
    public async Task Import_Csv_Without_Email_Column_Test()
    {
        await FluentActions.Awaiting(() => this.importer.ImportingUsersAsync(Text("Name,City\r\nalice,Rome"), false))
            .Should().ThrowAsync<FormatException>();

        this.created.Should().BeEmpty();
    }

    [Test]
    [Description("When the forum user can not be created the user is not counted and gets no mail")]
    public async Task Import_Forum_User_Fails_Test()
    {
        this.roles.CreateForumUserAsync(Arg.Any<AspNetUsers>(), Arg.Any<int>()).Returns((int?)null);

        var count = await this.importer.ImportingUsersAsync(Text("Name,Email\r\nalice,alice@example.com"), false);

        count.Should().Be(0);
        await this.board.Notifications.DidNotReceiveWithAnyArgs()
            .SendRegistrationNotificationToUserAsync(null, null, null);
    }

    [Test]
    [Description("A user refused by the identity (e.g. a name with characters that are not allowed) gets no forum user and no mail")]
    public async Task Import_Identity_Refuses_User_Test()
    {
        this.users.CreateUserAsync(Arg.Is<AspNetUsers>(u => u.UserName == "John Doe"), Arg.Any<string>())
            .Returns(IdentityResult.Failed(new IdentityError { Code = "InvalidUserName" }));

        var count = await this.importer.ImportingUsersAsync(
                        Text("Name,Email\r\nJohn Doe,john@example.com\r\nalice,alice@example.com"),
                        false);

        count.Should().Be(1);
        this.ForumUser("John Doe").Should().BeNull();
        this.ForumUser("alice").Should().NotBeNull();

        await this.roles.DidNotReceive().CreateForumUserAsync(Arg.Is<AspNetUsers>(u => u.UserName == "John Doe"), Arg.Any<int>());
        await this.board.Notifications.Received(1).SendRegistrationNotificationToUserAsync(
            Arg.Any<AspNetUsers>(),
            Arg.Any<string>(),
            "NOTIFICATION_ON_REGISTER");
    }

    private User ForumUser(string name)
    {
        return this.board.Current.GetRepository<User>().GetSingle(u => u.Name == name);
    }

    private static MemoryStream Xml(List<User> users)
    {
        var stream = new MemoryStream();
        new XmlSerializer(typeof(List<User>)).Serialize(stream, users);
        stream.Position = 0;

        return stream;
    }

    private static MemoryStream Text(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}
