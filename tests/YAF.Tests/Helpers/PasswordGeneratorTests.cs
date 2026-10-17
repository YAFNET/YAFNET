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

/// <summary>
/// YAF.Core.Helpers PasswordGenerator Tests
/// </summary>
[TestFixture]
public class PasswordGeneratorTests
{
    private const string Special = @"!#$%&*@\";

    [TestCase(8)]
    [TestCase(16)]
    [TestCase(128)]
    [Description("Generated passwords have the requested length and satisfy the requested rules")]
    public void GeneratePassword_Length_And_Rules_Test(int length)
    {
        var password = PasswordGenerator.GeneratePassword(true, true, true, true, false, length);

        password.Should().HaveLength(length);
        PasswordGenerator.PasswordIsValid(true, true, true, true, false, password).Should().BeTrue();
        password.Should().NotContain(" ");
    }

    [TestCase(7)]
    [TestCase(129)]
    [TestCase(0)]
    [Description("Out of range lengths return an error message")]
    public void GeneratePassword_Invalid_Length_Test(int length)
    {
        PasswordGenerator.GeneratePassword(true, true, true, true, false, length)
            .Should().Be("Password length must be between 8 and 128.");
    }

    [Test]
    [Description("Every character of the selected set can appear, including the first and the last")]
    public void GeneratePassword_Uses_Whole_Character_Set_Test()
    {
        var seen = new HashSet<char>();

        for (var i = 0; i < 300; i++)
        {
            seen.UnionWith(PasswordGenerator.GeneratePassword(false, false, true, false, false, 32));
        }

        seen.Should().BeEquivalentTo("0123456789");
    }

    [Test]
    [Description("Only characters from the selected sets are used")]
    public void GeneratePassword_Only_Selected_Characters_Test()
    {
        for (var i = 0; i < 50; i++)
        {
            PasswordGenerator.GeneratePassword(true, false, false, false, false, 20).Should().MatchRegex("^[a-z]+$");
            PasswordGenerator.GeneratePassword(false, true, true, false, false, 20).Should().MatchRegex("^[A-Z0-9]+$");
            PasswordGenerator.GeneratePassword(false, false, false, true, false, 20).Should().Match(
                p => p.All(Special.Contains));
        }
    }

    [Test]
    [Description("Passwords never contain more than two identical characters in a row")]
    public void GeneratePassword_No_Triple_Characters_Test()
    {
        for (var i = 0; i < 200; i++)
        {
            var password = PasswordGenerator.GeneratePassword(false, false, true, false, false, 16);

            for (var c = 2; c < password.Length; c++)
            {
                (password[c] == password[c - 1] && password[c] == password[c - 2]).Should()
                    .BeFalse($"'{password}' has three identical characters at position {c - 2}");
            }
        }
    }

    [Test]
    [Description("Passwords with spaces can be generated")]
    [CancelAfter(5000)]
    public void GeneratePassword_With_Spaces_Test()
    {
        var password = PasswordGenerator.GeneratePassword(true, false, false, false, true, 16);

        password.Should().Contain(" ");
        PasswordGenerator.PasswordIsValid(true, false, false, false, true, password).Should().BeTrue();
    }

    [TestCase("abcdefgh", true, false, false, false, false, true)]
    [TestCase("ABCDEFGH", true, false, false, false, false, false)]
    [TestCase("abcDEF12", true, true, true, false, false, true)]
    [TestCase("abcDEFgh", true, true, true, false, false, false)]
    [TestCase("abcDEF1!", true, true, true, true, false, true)]
    [TestCase(@"abcDEF1\", true, true, true, true, false, true)]
    [TestCase("abcDEF12", true, true, true, true, false, false)]
    [TestCase("abc DEF1", true, false, false, false, true, true)]
    [TestCase("abcdefgh", false, false, false, false, true, false)]
    [TestCase("anything", false, false, false, false, false, true)]
    [Description("PasswordIsValid checks that every requested character class is present")]
    public void PasswordIsValid_Test(
        string password,
        bool lower,
        bool upper,
        bool numeric,
        bool special,
        bool spaces,
        bool expected)
    {
        PasswordGenerator.PasswordIsValid(lower, upper, numeric, special, spaces, password).Should().Be(expected);
    }
}
