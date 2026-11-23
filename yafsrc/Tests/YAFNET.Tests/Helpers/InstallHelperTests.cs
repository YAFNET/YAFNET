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
/// YAF.Core.Helpers InstallHelper Tests
/// </summary>
[TestFixture]
public class InstallHelperTests
{
    [Test]
    [Description("The values of the new board are accepted when all are set and the passwords are the same")]
    public void CheckCreateForum_Accepts_Complete_Values_Test()
    {
        InstallHelper.CheckCreateForum("Board", "board@example.test", "https://example.test/", "admin", "admin@example.test", "Secret1!", "Secret1!")
            .Should().BeNull();
    }

    [Test]
    [Description("A different confirmation of the password is refused (it was never compared, a typo locked the only admin out)")]
    public void CheckCreateForum_Refuses_Different_Passwords_Test()
    {
        InstallHelper.CheckCreateForum("Board", "board@example.test", "https://example.test/", "admin", "admin@example.test", "Secret1!", "Secret1?")
            .Should().Be(InstallHelper.PasswordsDifferent);
    }

    [Test]
    [Description("The passwords are compared case sensitive")]
    public void CheckCreateForum_Compares_Passwords_Case_Sensitive_Test()
    {
        InstallHelper.CheckCreateForum("Board", "board@example.test", "https://example.test/", "admin", "admin@example.test", "Secret1!", "secret1!")
            .Should().Be(InstallHelper.PasswordsDifferent);
    }

    [TestCase(null, "board@example.test", "https://example.test/", "admin", "admin@example.test", "Secret1!", "Secret1!")]
    [TestCase("Board", " ", "https://example.test/", "admin", "admin@example.test", "Secret1!", "Secret1!")]
    [TestCase("Board", "board@example.test", "", "admin", "admin@example.test", "Secret1!", "Secret1!")]
    [TestCase("Board", "board@example.test", "https://example.test/", null, "admin@example.test", "Secret1!", "Secret1!")]
    [TestCase("Board", "board@example.test", "https://example.test/", "admin", null, "Secret1!", "Secret1!")]
    [TestCase("Board", "board@example.test", "https://example.test/", "admin", "admin@example.test", null, null)]
    [TestCase("Board", "board@example.test", "https://example.test/", "admin", "admin@example.test", "Secret1!", null)]
    [Description("A missing value is refused (the user name and email address were used without a check and failed with an error)")]
    public void CheckCreateForum_Refuses_Missing_Values_Test(
        string forumName,
        string forumEmail,
        string forumUrl,
        string userName,
        string adminEmail,
        string password,
        string confirm)
    {
        InstallHelper.CheckCreateForum(forumName, forumEmail, forumUrl, userName, adminEmail, password, confirm)
            .Should().Be(InstallHelper.MissingValue);
    }
}
