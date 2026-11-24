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

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Helpers;

/// <summary>
/// YAF.Core.Helpers HtmlContentHelper Tests
/// </summary>
[TestFixture]
public class HtmlContentHelperTests
{
    /// <summary>
    /// The text of a tag (a user name in a user link) is encoded, the html of the tag is kept.
    /// </summary>
    [Test]
    [Description("The text of a tag is encoded, the html of the tag is kept")]
    public void Text_Of_A_Tag_Is_Encoded_Test()
    {
        var link = new TagBuilder("a");

        link.MergeAttribute("href", "/user/2/name?a=1&b=2");
        link.InnerHtml.AppendHtml("<i class=\"fa\"></i>");
        link.InnerHtml.Append("Name <img src=x onerror=alert(1)> [[1]]");

        link.RenderToString().Should().Be(
            "<a href=\"/user/2/name?a=1&amp;b=2\"><i class=\"fa\"></i>Name &lt;img src=x onerror=alert(1)&gt; [[1]]</a>");
    }
}
