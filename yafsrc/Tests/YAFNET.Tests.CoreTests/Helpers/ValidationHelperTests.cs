﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Tests.CoreTests.Helpers;

/// <summary>
/// YAF.Utils.Helpers ValidationHelper Tests
/// </summary>
[TestFixture]
public class ValidationHelperTests
{
    /// <summary>
    /// Determines whether [is valid email test].
    /// </summary>
    [Test]
    [Description("Determines whether [is valid email test]")]
    public void IsValidEmail_Test()
    {
        const string testEmail = "yaf@co.geauga.oh.us";

        ValidationHelper.IsValidEmail(testEmail).Should().BeTrue("Email address is not valid");
    }

    /// <summary>
    /// Test the IPHelper if the ip address matches the pattern.
    /// </summary>
    [Test]
    [Description("Test the IPHelper if the ip address matches the pattern.")]
    public void IpHelperTest()
    {
        IPHelper.IsBanned( "43.*.*.*", "43.134.127.238").Should().BeTrue("true");
    }
}