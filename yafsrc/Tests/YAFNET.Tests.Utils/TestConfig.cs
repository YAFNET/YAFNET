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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Utils;

/// <summary>
/// Configuration Settings
/// </summary>
public class TestConfig
{
    /// <summary>
    /// Gets the test forum ID.
    /// </summary>
    public int TestForumId { get; set; } = 1;

    /// <summary>
    /// Gets the test topic ID.
    /// </summary>
    public int TestTopicId { get; set; } = 1;

    /// <summary>
    /// Gets the test message ID.
    /// </summary>
    public int TestMessageId { get; set; } = 1;

    /// <summary>
    /// Gets the name of the test application.
    /// </summary>
    /// <value>
    /// The name of the test application.
    /// </value>
    public string TestApplicationName { get; set; } = "YAFNETTEST";

    /// <summary>
    /// Gets the name of the admin user.
    /// </summary>
    public string AdminUserName { get; set; } = "Admin";

    /// <summary>
    /// Gets the admin password.
    /// </summary>
    public string AdminPassword { get; set; } = "AdminAdmin1234?!";

    /// <summary>
    /// Gets the name of the test user.
    /// </summary>
    /// <value>
    /// The name of the test user.
    /// </value>
    public string TestUserName { get; set; } = "TestUser";

    /// <summary>
    /// Gets the test user password.
    /// </summary>
    public string TestUserPassword { get; set; } = "TestUserTestUser1234?!";

    /// <summary>
    /// Gets the name of the test user 2.
    /// </summary>
    /// <value>
    /// The name of the test user 2.
    /// </value>
    public string TestUserName2 { get; set; } = "TestUser2";

    /// <summary>
    /// Gets the test user 2 password.
    /// </summary>
    public string TestUser2Password { get; set; } = "TestUser2TestUser21234?!";

    /// <summary>
    /// Gets Test Forum Url.
    /// </summary>
    public string TestForumUrl { get; set; } = $"https://localhost:{PlaywrightFixture.GetRandomUnusedPort()}/";

    /// <summary>
    /// Gets the test forum mail.
    /// </summary>
    public string TestForumMail { get; set; } = "forum@yafnettest.com";

    /// <summary>
    /// Gets or sets the test mail port.
    /// </summary>
    /// <value>The test mail port.</value>
    public int TestMailPort { get; set; } = 25;
}