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

namespace YAF.Tests.Helpers;

using System.Text.RegularExpressions;

using Autofac;

using NSubstitute;

using YAF.Core.Helpers;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The rules for user names and display names (<see cref="UserHelper.IsAllowedName"/>),
/// used by the registration and the profile pages.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserHelperTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        var usersHelper = Substitute.For<IAspNetUsersHelper>();
        usersHelper.GuestUser(Arg.Any<int>()).Returns(new User { Name = "Guest" });

        var badWords = Substitute.For<IBadWordReplace>();
        badWords.ReplaceItems.Returns(new List<BadWordReplaceItem> { new("***", "Idiot", RegexOptions.IgnoreCase) });

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(usersHelper).As<IAspNetUsersHelper>();
                b.RegisterInstance(badWords).As<IBadWordReplace>();
            });
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase("Alice")]
    [TestCase("Alice Guest")]
    [TestCase("Guests")]
    [Description("Normal names are allowed")]
    public void Allowed_Name_Test(string name)
    {
        (UserHelper.IsAllowedName(name, 1)).Should().BeTrue();
    }

    [TestCase("Guest")]
    [TestCase(" guest ")]
    [TestCase("Ali;ce")]
    [TestCase("idiot")]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    [Description("The guest name, semicolons, bad words and empty names are not allowed")]
    public void Not_Allowed_Name_Test(string name)
    {
        (UserHelper.IsAllowedName(name, 1)).Should().BeFalse();
    }
}
