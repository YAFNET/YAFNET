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

using YAF.Types.Interfaces;
using YAF.Types.Objects;

/// <summary>
/// Tests for IPagerExtensions (YAF.Types)
/// </summary>
[TestFixture]
public class PagerExtensionsTests
{
    [Test]
    [Description("GetPaged returns the rows of the page and sets the count")]
    public void GetPaged_Test()
    {
        var pager = new Paging { CurrentPageIndex = 1, PageSize = 3 };

        Enumerable.Range(1, 10).ToList().GetPaged(pager).Should().Equal(4, 5, 6);

        pager.Count.Should().Be(10);
        pager.PageCount().Should().Be(4);
    }

    [Test]
    [Description("A page index from the url that is negative or huge does not wrap around to the first page")]
    public void SkipIndex_Out_Of_Range_Test()
    {
        new Paging { CurrentPageIndex = -1, PageSize = 10 }.SkipIndex().Should().Be(0);

        var huge = new Paging { CurrentPageIndex = int.MaxValue - 1, PageSize = 10 };

        huge.SkipIndex().Should().Be(int.MaxValue);
        Enumerable.Range(1, 10).ToList().GetPaged(huge).Should().BeEmpty();
    }
}
