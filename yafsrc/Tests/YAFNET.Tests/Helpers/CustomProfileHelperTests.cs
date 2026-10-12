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

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// <see cref="CustomProfileHelper.FindInvalidField"/>: the custom profile fields are checked against
/// the field definitions of the board, not against the required flags and types sent by the client.
/// </summary>
[TestFixture]
public class CustomProfileHelperTests
{
    private static readonly List<ProfileDefinition> Definitions =
    [
        new() { ID = 1, Name = "Text", DataType = nameof(DataType.Text), Required = true },
        new() { ID = 2, Name = "Age", DataType = nameof(DataType.Number), Required = false },
        new() { ID = 3, Name = "Newsletter", DataType = nameof(DataType.Check), Required = true }
    ];

    [Test]
    [Description("All values are valid")]
    public void Valid_Values_Test()
    {
        CustomProfileHelper.FindInvalidField(
            Definitions,
            [new ProfileDefinition { ID = 1, Value = "abc" }, new ProfileDefinition { ID = 2, Value = "-12.5" }],
            true,
            out var isNumberError).Should().BeNull();

        isNumberError.Should().BeFalse();
    }

    [Test]
    [Description("A required field is missing, also when the client leaves it out or sends it as not required")]
    public void Required_Field_Missing_Test()
    {
        CustomProfileHelper.FindInvalidField(Definitions, null, true, out _).ID.Should().Be(1);

        CustomProfileHelper.FindInvalidField(
            Definitions,
            [new ProfileDefinition { ID = 1, Value = "  ", Required = false, DataType = nameof(DataType.Check) }],
            true,
            out var isNumberError).ID.Should().Be(1);

        isNumberError.Should().BeFalse();
    }

    [Test]
    [Description("The required fields are not checked when the admin edits a user")]
    public void Required_Not_Checked_Test()
    {
        CustomProfileHelper.FindInvalidField(Definitions, [], false, out _).Should().BeNull();
    }

    [Test]
    [Description("The value of a number field must be a number, also when the client sends the field as text")]
    public void Number_Field_Test()
    {
        CustomProfileHelper.FindInvalidField(
            Definitions,
            [
                new ProfileDefinition { ID = 1, Value = "abc" },
                new ProfileDefinition { ID = 2, Value = "twelve", DataType = nameof(DataType.Text) }
            ],
            false,
            out var isNumberError).ID.Should().Be(2);

        isNumberError.Should().BeTrue();
    }
}
