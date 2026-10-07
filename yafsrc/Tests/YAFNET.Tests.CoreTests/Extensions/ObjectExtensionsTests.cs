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

using System.ComponentModel;

/// <summary>
/// YAF.Types.Extensions ObjectExtensions Tests
/// </summary>
[TestFixture]
public class ObjectExtensionsTests
{
    [Test]
    [Description("IsNullOrEmptyField detects null, DBNull and blank values")]
    public void IsNullOrEmptyField_Test()
    {
        ((object)null).IsNullOrEmptyField().Should().BeTrue();
        DBNull.Value.IsNullOrEmptyField().Should().BeTrue();
        "".IsNullOrEmptyField().Should().BeTrue();
        "  ".IsNullOrEmptyField().Should().BeTrue();
        "x".IsNullOrEmptyField().Should().BeFalse();
        0.IsNullOrEmptyField().Should().BeFalse();
    }

    [Test]
    [Description("GetAttribute returns the attribute or null")]
    public void GetAttribute_Test()
    {
        typeof(DecoratedClass).GetAttribute<DescriptionAttribute>().Description.Should().Be("decorated");
        typeof(ObjectExtensionsTests).GetAttribute<ObsoleteAttribute>().Should().BeNull();
    }

    [Test]
    [Description("HasInterface checks interface implementation")]
    public void HasInterface_Test()
    {
        new List<int>().HasInterface<IEnumerable<int>>().Should().BeTrue();
        "abc".HasInterface<IDisposable>().Should().BeFalse();
    }

    [Test]
    [Description("IsIn checks membership in the params list")]
    public void IsIn_Test()
    {
        3.IsIn(1, 2, 3).Should().BeTrue();
        4.IsIn(1, 2, 3).Should().BeFalse();
        "b".IsIn("a", "b").Should().BeTrue();
    }

    [Test]
    [Description("ToType converts between primitive types")]
    public void ToType_Converts_Primitives_Test()
    {
        "42".ToType<int>().Should().Be(42);
        42.ToType<string>().Should().Be("42");
        42L.ToType<int>().Should().Be(42);
        "true".ToType<bool>().Should().BeTrue();
        1.ToType<bool>().Should().BeTrue();
        "42".ToType<int?>().Should().Be(42);
    }

    [Test]
    [Description("ToType returns default for null, DBNull and blank strings")]
    public void ToType_Returns_Default_Test()
    {
        ((object)null).ToType<int>().Should().Be(0);
        DBNull.Value.ToType<int>().Should().Be(0);
        "".ToType<int>().Should().Be(0);
        "  ".ToType<int?>().Should().BeNull();
        ((object)null).ToType<string>().Should().BeNull();
    }

    [Test]
    [Description("ToType casts non-convertible reference types directly")]
    public void ToType_Casts_Classes_Test()
    {
        var list = new List<int> { 1 };

        list.ToType<IEnumerable<int>>().Should().BeSameAs(list);
    }

    [Test]
    [Description("ToType throws on invalid conversions")]
    public void ToType_Invalid_Throws_Test()
    {
        FluentActions.Invoking(() => "abc".ToType<int>()).Should().Throw<FormatException>();
    }

    [Test]
    [Description("ToTypeOrDefault swallows conversion errors and returns the fallback")]
    public void ToTypeOrDefault_Test()
    {
        "abc".ToTypeOrDefault(7).Should().Be(7);
        long.MaxValue.ToTypeOrDefault(7).Should().Be(7);
        new object().ToTypeOrDefault(7).Should().Be(7);
        "12".ToTypeOrDefault(7).Should().Be(12);
    }

    [Description("decorated")]
    private sealed class DecoratedClass;
}
