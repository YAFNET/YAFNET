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
using System.Text.Json.Nodes;

using YAF.Types.Constants;
using YAF.Web.HtmlHelpers;

/// <summary>
/// The location of the active users: every page has a text of the page ACTIVELOCATION (a missing text was shown
/// as [ACTIVELOCATION.PAGE] and logged).
/// </summary>
[TestFixture]
public class ActiveLocationTextsTests
{
    [Test]
    [Description("Every page of the forum has a location text in english.json")]
    public void Every_Page_Has_A_Location_Text_Test()
    {
        var json = JsonNode.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "english.json")));
        var page = json!["Resources"]!["page"]!.AsArray().First(p => p!["@name"]!.GetValue<string>() == "ACTIVELOCATION");
        var tags = page!["Resource"]!.AsArray().Select(r => r!["@tag"]!.GetValue<string>().ToUpperInvariant()).ToHashSet();

        // the pages with their own texts (the board index, the albums, the profile and the forums), the host pages
        // are admin tasks
        var ownTexts = new[] { ForumPages.Index, ForumPages.Albums, ForumPages.Album, ForumPages.UserProfile, ForumPages.Topics };

        var missing = Enum.GetValues<ForumPages>().Except(ownTexts)
            .Where(p => !p.ToString().StartsWith("Host_", StringComparison.Ordinal))
            .Select(p => (Page: p, Tag: ActiveLocationHtmlHelper.LocationTag(p.ToString())))
            .Where(p => !tags.Contains(p.Tag)).Select(p => $"{p.Page}: {p.Tag}").ToList();

        missing.Should().BeEmpty();
        ActiveLocationHtmlHelper.LocationTag("Profile_EditProfile").Should().Be("EDIT_PROFILE");
        ActiveLocationHtmlHelper.LocationTag("Members").Should().Be("MEMBERS");
    }
}
