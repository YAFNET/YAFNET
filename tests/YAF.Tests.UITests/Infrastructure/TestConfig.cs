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

namespace YAF.Tests.UITests.Infrastructure;

using System;

/// <summary>
/// The settings of the UI tests. The values can be changed with environment variables
/// (e.g. YAF_UITEST_PORT=8092, YAF_UITEST_HEADED=1 to watch the browser, YAF_UITEST_KEEP=1 to keep the
/// site copy and the database after the run).
/// </summary>
public static class TestConfig
{
    /// <summary>
    /// Gets the port of IIS Express.
    /// </summary>
    public static int Port => GetInt("YAF_UITEST_PORT", 8091);

    /// <summary>
    /// Gets the url of the forum.
    /// </summary>
    public static string BaseUrl => $"http://localhost:{Port}/";

    /// <summary>
    /// Gets the LocalDB instance.
    /// </summary>
    public static string LocalDbInstance =>
        Environment.GetEnvironmentVariable("YAF_UITEST_LOCALDB") ?? @"(localdb)\MSSQLLocalDB";

    /// <summary>
    /// Gets the name of the test database (created for every run).
    /// </summary>
    public static string DatabaseName => Environment.GetEnvironmentVariable("YAF_UITEST_DATABASE") ?? "YafUiTests";

    /// <summary>
    /// Gets a value indicating whether the browser is shown.
    /// </summary>
    public static bool Headed => Environment.GetEnvironmentVariable("YAF_UITEST_HEADED") == "1";

    /// <summary>
    /// Gets a value indicating whether the site copy and the database are kept after the run.
    /// </summary>
    public static bool Keep => Environment.GetEnvironmentVariable("YAF_UITEST_KEEP") == "1";

    /// <summary>
    /// Gets the name of the test board.
    /// </summary>
    public static string BoardName => "Testboard created by UI Test";

    /// <summary>
    /// Gets the e-mail address of the forum.
    /// </summary>
    public static string ForumEmail => "test@localhost.com";

    /// <summary>
    /// Gets the name of the admin (created by the install wizard).
    /// </summary>
    public static string AdminUserName => "Watcher";

    /// <summary>
    /// Gets the e-mail address of the admin.
    /// </summary>
    public static string AdminEmail => "watcher@localhost.com";

    /// <summary>
    /// Gets the password of the admin.
    /// </summary>
    public static string AdminPassword => "Watcher1234!";

    /// <summary>
    /// Gets the name of the test user.
    /// </summary>
    public static string TestUserName => "TestUser1";

    /// <summary>
    /// Gets the password of the test users.
    /// </summary>
    public static string TestUserPassword => "TestUser1234!";

    /// <summary>
    /// Gets the name of the second test user.
    /// </summary>
    public static string TestUserName2 => "TestUser2";

    private static int GetInt(string name, int defaultValue)
    {
        return int.TryParse(Environment.GetEnvironmentVariable(name), out var value) ? value : defaultValue;
    }
}
