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

namespace YAF.Tests.CoreTests;

using System.Net.Http;
using System.Threading.Tasks;

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
    public async Task Check_For_Bot_Test_via_StopForumSpam()
    {
        var check = await new StopForumSpam().IsBotAsync("84.16.230.111", "uuruznfdxw@gmail.com", "someone");

        Assert.IsTrue(check.IsBot, "This should be a Bot{0}", check.ResponseText);
    }

    /// <summary>
    /// A Test to Check for Bot via BotScout.com API
    /// </summary>
    [Test]
    [Description("A Test to Check for Bot via BotScout.com API")]
    public async Task Check_For_Bot_Test_via_BotScout()
    {
        var check = await new BotScout().IsBotAsync("84.16.230.111", "krasnhello@mail.ru", "someone");

        Assert.IsTrue(check.IsBot, "This should be a Bot{0}", check.ResponseText);
    }

    /// <summary>
    /// A Test to Check for Bot via BotScout.com or StopForumSpam.com API
    /// </summary>
    [Test]
    [Description("A Test to Check for Bot via BotScout.com API or StopForumSpam.com API")]
    public async Task Check_For_Bot_Test()
    {
        var botScoutCheck = await new BotScout().IsBotAsync("84.16.230.111", "krasnhello@mail.ru", "someone");
        var stopForumSpamCheck = await new StopForumSpam().IsBotAsync("84.16.230.111", "krasnhello@mail.ru", "someone");

        Assert.IsTrue(botScoutCheck.IsBot | stopForumSpamCheck.IsBot, "This should be a Bot");
    }

    /// <summary>
    /// The report_ user_ as_ bot_ test.
    /// </summary>
    [Test]
    [Ignore("Ignore until api key is set")]
    public async Task Report_User_As_Bot_Test()
    {
        var parameters =
            "username=someone&ip_addr=84.16.230.111&email=krasnhello@mail.ru&api_key=XXXXXXXXX";

        var client = new HttpClient(new HttpClientHandler());
        var response = await client.GetAsync($"https://www.stopforumspam.com/add.php?{parameters}");

        var result = await response.Content.ReadAsStringAsync();

        Assert.IsTrue(result.Contains("success"), result);
    }
}