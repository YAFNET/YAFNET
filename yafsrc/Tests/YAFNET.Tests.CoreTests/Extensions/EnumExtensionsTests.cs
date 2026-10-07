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

namespace YAF.Tests.CoreTests.Extensions;

using YAF.Types.Attributes;

/// <summary>
/// Tests for EnumExtensions (YAF.Types) and EnumHelper (YAF.Core)
/// </summary>
[TestFixture]
public class EnumExtensionsTests
{
    public enum TestColor
    {
        Red = 1,

        [StringValue("Bright Green")]
        Green = 2,

        Blue = 4
    }

    [Test]
    [Description("GetAllItems returns every enum value")]
    public void GetAllItems_Test()
    {
        EnumExtensions.GetAllItems<TestColor>().Should().Equal(TestColor.Red, TestColor.Green, TestColor.Blue);
    }

    [Test]
    [Description("ToEnum converts ints and strings to enum values")]
    public void ToEnum_Test()
    {
        2.ToEnum<TestColor>().Should().Be(TestColor.Green);
        "Blue".ToEnum<TestColor>().Should().Be(TestColor.Blue);
        "blue".ToEnum<TestColor>(true).Should().Be(TestColor.Blue);
        "4".ToEnum<TestColor>().Should().Be(TestColor.Blue);
    }

    [Test]
    [Description("ToEnum rejects non-enum target types")]
    public void ToEnum_NonEnum_Throws_Test()
    {
        FluentActions.Invoking(() => 1.ToEnum<int>()).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(() => "1".ToEnum<int>()).Should().Throw<ArgumentNullException>();
        FluentActions.Invoking(() => "1".ToEnum<int>(true)).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("ToEnum throws for unknown names")]
    public void ToEnum_Unknown_Name_Throws_Test()
    {
        FluentActions.Invoking(() => "Purple".ToEnum<TestColor>()).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => "blue".ToEnum<TestColor>(false)).Should().Throw<ArgumentException>();
    }

    [Test]
    [Description("ToInt returns the underlying value")]
    public void ToInt_Test()
    {
        TestColor.Blue.ToInt().Should().Be(4);
    }

    [Test]
    [Description("EnumHelper.EnumToList returns typed values")]
    public void EnumHelper_EnumToList_Test()
    {
        EnumHelper.EnumToList<TestColor>().Should().Equal(TestColor.Red, TestColor.Green, TestColor.Blue);
        FluentActions.Invoking(EnumHelper.EnumToList<int>).Should().Throw<ArgumentException>();
    }

    [Test]
    [Description("EnumHelper.EnumToDictionary uses the StringValue attribute when present")]
    public void EnumHelper_EnumToDictionary_Test()
    {
        EnumHelper.EnumToDictionary<TestColor>().Should().BeEquivalentTo(
            new Dictionary<int, string> { [1] = "Red", [2] = "Bright Green", [4] = "Blue" });

        FluentActions.Invoking(EnumHelper.EnumToDictionary<int>).Should().Throw<ArgumentException>();
    }
}
