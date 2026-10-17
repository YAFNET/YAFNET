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

using System.Threading.Tasks;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The installed forum (the install wizard runs in the assembly setup).
/// </summary>
[TestFixture]
public class InstallTests : ForumTestBase
{
    [Test]
    [Description("After the install wizard the board index shows the new board")]
    public async Task Board_Is_Installed_Test()
    {
        await this.GotoAsync(string.Empty);

        (await this.Page.TitleAsync()).Should().Contain(TestConfig.BoardName);
    }

    [Test]
    [Description("The install wizard can not be opened again on the installed forum")]
    public async Task Install_Wizard_Is_Closed_After_Install_Test()
    {
        await this.GotoAsync("install/default.aspx");

        this.Page.Url.Should().NotContain("install");
    }
}
