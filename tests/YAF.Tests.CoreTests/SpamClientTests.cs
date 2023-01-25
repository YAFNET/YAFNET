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

namespace YAF.Tests.CoreTests;

using System;

using YAF.Core.Services.CheckForSpam;

/// <summary>
/// The spam client tester.
/// </summary>
[TestFixture]
public class SpamClientTests
{
    /// <summary>
    /// A Test to Check for Bot via StopForumSpam.com API
    /// </summary>
    [Test]
    [Description("A Test to Check for Bot via StopForumSpam.com API")]
    [Ignore("Ignore from automatic testing")]
    public void Check_For_Bot_Test_via_StopForumSpam()
    {
        Assert.IsTrue(
            new StopForumSpam().IsBot("84.16.230.111", "uuruznfdxw@gmail.com", "someone", out var responseText),
            "This should be a Bot{0}",
            responseText);
    }

    /// <summary>
    /// A Test to Check for Bot via BotScout.com API
    /// </summary>
    [Test]
    [Description("A Test to Check for Bot via BotScout.com API")]
    [Ignore("Ignore from automatic testing")]
    public void Check_For_Bot_Test_via_BotScout()
    {
        Assert.IsTrue(
            new BotScout().IsBot("84.16.230.111", "krasnhello@mail.ru", "someone", out var responseText),
            "This should be a Bot{0}",
            responseText);
    }

    /// <summary>
    /// A Test to Check for Bot via BotScout.com or StopForumSpam.com API
    /// </summary>
    [Test]
    [Description("A Test to Check for Bot via BotScout.com API or StopForumSpam.com API")]
    [Ignore("Ignore from automatic testing")]
    public void Check_For_Bot_Test()
    {
        var botScoutCheck = new BotScout().IsBot("84.16.230.111", "krasnhello@mail.ru", "someone", out _);
        var stopForumSpamCheck = new StopForumSpam().IsBot(
            "84.16.230.111",
            "krasnhello@mail.ru",
            "someone",
            out _);

        Assert.IsTrue(botScoutCheck | stopForumSpamCheck, "This should be a Bot");
    }

    /// <summary>
    /// The report_ user_ as_ bot_ test.
    /// </summary>
    [Test]
    [Ignore("Ignore until api key is set")]
    public void Report_User_As_Bot_Test()
    {
        var parameters =
            "username=someone&ip_addr=84.16.230.111&email=krasnhello@mail.ru&api_key=XXXXXXXXXXX";

        var result = new HttpClient().PostRequest(
            new Uri("https://www.stopforumspam.com/add.php"),
            null,
            5000,
            parameters);

        Assert.IsTrue(result.Equals("success"), result);
    }
}