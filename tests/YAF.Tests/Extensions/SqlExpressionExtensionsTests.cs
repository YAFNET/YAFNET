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

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Types.Models;

/// <summary>
/// Tests for SqlExpressionExtensions
/// </summary>
[TestFixture]
public class SqlExpressionExtensionsTests
{
    [Test]
    [Description("Page sets the offset and the rows of the page")]
    public void Page_Test()
    {
        var expression = SqliteDialect.Provider.SqlExpression<Topic>().Page(3, 20);

        expression.Offset.Should().Be(40);
        expression.Rows.Should().Be(20);
    }

    [Test]
    [Description("A page of 0 or less (from the url) is the first page")]
    public void Page_Not_Positive_Test()
    {
        SqliteDialect.Provider.SqlExpression<Topic>().Page(0, 20).Offset.Should().Be(0);
        SqliteDialect.Provider.SqlExpression<Topic>().Page(-5, 20).Offset.Should().Be(0);
    }

    [Test]
    [Description("A huge page number (from the url) does not overflow to a negative offset")]
    public void Page_Huge_Page_Number_Test()
    {
        var expression = SqliteDialect.Provider.SqlExpression<Topic>().Page(int.MaxValue, 100);

        expression.Offset.Should().Be(int.MaxValue);
        expression.Rows.Should().Be(100);
    }

    [Test]
    [Description("The page size must be greater than 0")]
    public void Page_Size_Not_Positive_Test()
    {
        var action = () => SqliteDialect.Provider.SqlExpression<Topic>().Page(1, 0);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
