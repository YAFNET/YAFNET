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
using System.Threading.Tasks;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The wysiwyg editor (SCEditor) of the host settings: a topic is posted with it.
/// </summary>
[TestFixture]
public class WysiwygEditorTests : ForumTestBase
{
    [Test]
    [Description("With the wysiwyg editor a member posts a topic and a reply without script errors")]
    public async Task Post_With_Wysiwyg_Editor_Test()
    {
        var subject = $"Wysiwyg {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();
        await this.SetWysiwygAsync(true);

        try
        {
            await this.LoginAsUserAsync(TestConfig.TestUserName);
            await this.GotoAsync("PostTopic?f=1");

            await this.Page.Locator(".sceditor-container").First.WaitForAsync();

            await this.Control("TopicSubjectTextBox").FillAsync(subject);

            // typed like a member (in the frame of the editor)
            var frame = this.Page.FrameLocator(".sceditor-container iframe").First;
            await frame.Locator("body").ClickAsync();
            await frame.Locator("body").PressSequentiallyAsync("The wysiwyg text.");
            await this.ClickAndWaitAsync(this.Control("PostReply"));

            (await this.Page.InnerTextAsync("body")).Should().Contain("The wysiwyg text.");

            this.BrowserErrors.Should().BeEmpty();
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetWysiwygAsync(false);
        }
    }

    private async Task SetWysiwygAsync(bool enabled)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("EnableWysiwygEditor").EvaluateAsync("(e, v) => e.checked = v", enabled);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }
}
