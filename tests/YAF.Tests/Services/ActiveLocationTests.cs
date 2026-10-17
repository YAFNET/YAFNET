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

using YAF.Tests.TestInfrastructure;
using YAF.Web.Controls;

/// <summary>
/// The location of a user in the active users list (<see cref="ActiveLocation"/>): the location is the user of the
/// address (u) of the albums, album and profile pages.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ActiveLocationTests
{
    private TestBoardContext board;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.alice = this.board.Database.AddUser("Alice");

        this.board.Localization
            .Add("ACTIVELOCATION", "ALBUM", "Views Album")
            .Add("ACTIVELOCATION", "ALBUM_OFUSER", "of user")
            .Add("ACTIVELOCATION", "ALBUM_OWN", "View/Edit your Album")
            .Add("ACTIVELOCATION", "ALBUMS", "View Albums")
            .Add("ACTIVELOCATION", "ALBUMS_OFUSER", "View User Albums")
            .Add("ACTIVELOCATION", "ALBUMS_OWN", "Views/Edit the Albums")
            .Add("ACTIVELOCATION", "PROFILE", "Views profile of a user")
            .Add("ACTIVELOCATION", "PROFILE_OFUSER", "Views profile of user")
            .Add("ACTIVELOCATION", "PROFILE_OWN", "Views his/her own profile")
            .Add("COMMON", "VIEW_USRPROFILE", "View Profile");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase("UserProfile", "Views profile of a user")]
    [TestCase("Albums", "View Albums")]
    [TestCase("Album", "Views Album")]
    [Description("The location of a user that does not exist (e.g. deleted while a member looked at the profile) shows the general text, the list does not fail")]
    public void Unknown_User_Test(string page, string expected)
    {
        Render(page, "999999", viewer: 5000).Should().Contain(expected).And.NotContain("999999");
    }

    [TestCase("UserProfile", "Views profile of user")]
    [TestCase("Albums", "View User Albums")]
    [TestCase("Album", "of user")]
    [Description("The location of another user shows the name of that user")]
    public void Other_User_Test(string page, string expected)
    {
        Render(page, this.alice.ToString(), viewer: 5000).Should().Contain(expected).And.Contain("Alice");
    }

    [TestCase("UserProfile", "Views his/her own profile")]
    [TestCase("Albums", "Views/Edit the Albums")]
    [TestCase("Album", "View/Edit your Album")]
    [Description("The own location is shown without the name")]
    public void Own_Location_Test(string page, string expected)
    {
        Render(page, this.alice.ToString(), viewer: this.alice).Should().Contain(expected).And.NotContain("Alice");
    }

    [Test]
    [Description("The album location is not an album taken from the user id (the location has no album)")]
    public void Album_Of_Unknown_Album_Test()
    {
        Render("Album", "12", viewer: 5000).Should().Be("Views Album");
    }

    private static string Render(string page, string location, int viewer)
    {
        return new ActiveLocation { ForumPage = page, Location = location, UserID = viewer }.RenderToString().Trim();
    }
}
