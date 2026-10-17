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

namespace YAF.Tests.Extensions;

using YAF.Types.Objects;
using YAF.Web.Extensions;

/// <summary>
/// The page links (breadcrumb) of pages with an unknown forum, category or topic in the address.
/// </summary>
[TestFixture]
public class PageLinkExtensionsTests
{
    [Test]
    [Description("A missing category, forum or topic (an unknown id in the address) adds no link, the page does not fail")]
    public void Missing_Items_Add_No_Link_Test()
    {
        var links = new List<PageLink>();

        links.AddCategory(null).AddForum(null).AddTopic(null).Should().BeSameAs(links);

        links.Should().BeEmpty();
    }
}
