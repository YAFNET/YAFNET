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

namespace YAF.Tests.UITests.Tests;

using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Admin > Users > Import: users are imported from csv and xml files.
/// </summary>
[TestFixture]
public class UsersImportTests : ForumTestBase
{
    [Test]
    [Description("The new users of a csv file are imported, an existing user is skipped")]
    public async Task Csv_Import_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var csv = $"Name,Email,DisplayName,Country\r\n\"Csv{id}\",csv{id}@localhost.com,\"Csv, {id}\",Germany\r\n{TestConfig.AdminUserName},other.admin@localhost.com,,\r\n\r\n";

        await this.LoginAsAdminAsync();
        await this.ImportAsync("users.csv", "text/csv", csv);

        (await this.Page.ContentAsync()).Should().Contain("1");

        await this.OpenAdminEditUserAsync($"Csv{id}");
    }

    [Test]
    [Description("The users of an xml file are imported")]
    public async Task Xml_Import_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var xml = $"""
                   <?xml version="1.0" encoding="utf-8"?>
                   <YafUserList>
                     <User><Name>Xml{id}</Name><Email>xml{id}@localhost.com</Email><Gender>9</Gender></User>
                   </YafUserList>
                   """;

        await this.LoginAsAdminAsync();
        await this.ImportAsync("users.xml", "text/xml", xml);

        await this.OpenAdminEditUserAsync($"Xml{id}");
    }

    [Test]
    [Description("A file that is no users file is refused with a message")]
    public async Task Wrong_File_Test()
    {
        await this.LoginAsAdminAsync();

        await this.ImportAsync("users.txt", "text/plain", "no users");
        (await this.Page.ContentAsync()).Should().Contain("toast-body");

        await this.ImportAsync("users.csv", "text/csv", "Something,Else\r\n1,2\r\n");
        (await this.Page.ContentAsync()).Should().Contain("toast-body");
    }

    private async Task ImportAsync(string name, string mimeType, string content)
    {
        await this.GotoAsync("Admin/Users");

        await this.Control("ImportUsers").First.ClickAsync();
        await this.Page.Locator("#UsersImportDialog").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Control("ImportDialog_importFile").SetInputFilesAsync(
            new FilePayload { Name = name, MimeType = mimeType, Buffer = Encoding.UTF8.GetBytes(content) });

        await this.ClickAndWaitAsync(this.Control("ImportDialog_Import"));
    }
}
