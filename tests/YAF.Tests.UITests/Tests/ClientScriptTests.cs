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

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The scripts of the forum: the BBCode editor (buttons, auto closed tags, mentions), the upload dialog, the
/// context menu of the posts and the password strength of the registration.
/// </summary>
[TestFixture]
public class ClientScriptTests : ForumTestBase
{
    private static readonly byte[] Png = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

    [Test]
    [Description("Remove format only changes the selection, the same text before the selection is kept")]
    public async Task Remove_Format_Only_Changes_Selection_Test()
    {
        await this.OpenEditorAsync();

        await this.SetEditorAsync("[b]word[/b] and [b]word[/b]", 16, 27);
        await this.Page.EvaluateAsync("() => setStyle('removeFormat')");

        (await this.EditorValueAsync()).Should().Be("[b]word[/b] and word");
    }

    [Test]
    [Description("Outdent removes the indent tags of the selection, also when the selection starts with other text")]
    public async Task Outdent_Test()
    {
        await this.OpenEditorAsync();

        await this.SetEditorAsync("one [indent]two[/indent]", 0, 24);
        await this.Page.EvaluateAsync("() => setStyle('outdent')");

        (await this.EditorValueAsync()).Should().Be("one two");
    }

    [Test]
    [Description("A cancelled media dialog inserts nothing")]
    public async Task Media_Dialog_Cancel_Test()
    {
        await this.OpenEditorAsync();

        await this.SetEditorAsync("text", 4, 4);
        await this.Page.EvaluateAsync("() => setStyle('media')");

        await this.Page.Locator(".bootbox .bootbox-cancel").ClickAsync();
        await this.Page.Locator(".bootbox").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });

        (await this.EditorValueAsync()).Should().Be("text");
    }

    [Test]
    [Description("Paste inserts the text of the clipboard at the cursor")]
    public async Task Paste_At_Cursor_Test()
    {
        await this.OpenEditorAsync();

        await this.SetEditorAsync("one three", 4, 4);
        await this.Page.EvaluateAsync(
            "() => { navigator.clipboard.readText = () => Promise.resolve('two '); setStyle('paste'); }");

        await this.Page.WaitForFunctionAsync("() => document.querySelector(\"[id$='_YafTextEditor']\").value.length > 9");

        (await this.EditorValueAsync()).Should().Be("one two three");
    }

    [Test]
    [Description("A tag with a - in its name is closed with its own name")]
    public async Task Auto_Close_Tag_With_Dash_Test()
    {
        await this.OpenEditorAsync();

        var editor = this.Control("YafTextEditor");
        await editor.ClickAsync();
        await editor.PressSequentiallyAsync("[hide-thanks=1]");

        (await this.EditorValueAsync()).Should().Be("[hide-thanks=1][/hide-thanks]");
    }

    [Test]
    [Description("A selected mention replaces the typed name after the @, the same letters in other words are kept")]
    public async Task Mention_Replaces_Typed_Name_Test()
    {
        await this.OpenEditorAsync();

        var editor = this.Control("YafTextEditor");
        await editor.ClickAsync();

        // the list is loaded after 3 letters and shown with the next one
        await editor.PressSequentiallyAsync("watcher wrote @watc", new LocatorPressSequentiallyOptions { Delay = 300 });
        await this.Page.WaitForTimeoutAsync(1000);
        await editor.PressSequentiallyAsync("h", new LocatorPressSequentiallyOptions { Delay = 300 });

        var item = this.Page.Locator(".dropdown-mentions .mention-li-nt", new PageLocatorOptions { HasTextString = TestConfig.AdminUserName });
        await item.First.WaitForAsync();

        // the avatar of the user is clicked
        await item.First.Locator("img").ClickAsync();

        (await this.EditorValueAsync()).Should().Be($"watcher wrote @[userlink]{TestConfig.AdminUserName}[/userlink]");
    }

    [Test]
    [Description("The list of the upload dialog shows a new attachment after the upload, a refused upload shows the error")]
    public async Task Upload_Dialog_List_And_Error_Test()
    {
        var name = $"list{Guid.NewGuid():N}".Substring(0, 12) + ".png";

        // the admin uploads (members have no upload access by default)
        await this.LoginAsAdminAsync();
        await this.GotoAsync("PostTopic?f=1");

        await this.OpenUploadDialogAsync();
        await this.Page.Locator("#fileElem").SetInputFilesAsync(new FilePayload { Name = name, MimeType = "image/png", Buffer = Png });

        await this.Page.WaitForFunctionAsync(
            "() => document.querySelector(\"[id$='_YafTextEditor']\").value.toLowerCase().includes('[attach]')");
        await this.Page.Locator(".modal.show").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });

        // the dialog is opened again (without loading the page)
        await this.OpenUploadDialogAsync();
        await this.Page.Locator("#PostAttachmentListPlaceholder", new PageLocatorOptions { HasTextString = name })
            .WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

        // e.g. a file larger than the request limit of the server
        await this.Page.RouteAsync("**/FileUploader.ashx*", route => route.FulfillAsync(new RouteFulfillOptions { Status = 413, Body = "too large" }));

        try
        {
            await this.Page.Locator("#fileElem").SetInputFilesAsync(new FilePayload { Name = "large.png", MimeType = "image/png", Buffer = Png });

            await this.Page.Locator("[data-notify='container']", new PageLocatorOptions { HasTextString = "large.png: 413" })
                .WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
        }
        finally
        {
            await this.Page.UnrouteAsync("**/FileUploader.ashx*");
        }
    }

    [Test]
    [Description("The context menu of a post is opened at the clicked position, also when a part of the post is clicked")]
    public async Task Context_Menu_Position_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.OpenNewTopicAsync("A text before the [b]bold part of the post[/b].");

        // a part of the post (the bold text)
        var bold = this.Page.Locator(".message :is(b, strong)").First;
        var box = await bold.BoundingBoxAsync();

        await bold.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right, Position = new Position { X = 40, Y = 5 } });

        var menu = this.Page.Locator(".context-menu.show").First;
        await menu.WaitForAsync();

        var menuBox = await menu.BoundingBoxAsync();

        menuBox.X.Should().BeApproximately(box.X + 40, 3);
        menuBox.Y.Should().BeApproximately(box.Y + 5, 3);
    }

    [Test]
    [Description("On a phone the long press opens the context menu of a post at the pressed position")]
    public async Task Context_Menu_Long_Press_Position_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        var url = await this.OpenNewTopicAsync("A post for the long press.");

        var phone = await this.Context.NewPageAsync();

        try
        {
            await phone.SetViewportSizeAsync(400, 800);
            await phone.GotoAsync(url);

            var message = phone.Locator(".message").First;
            await message.ScrollIntoViewIfNeededAsync();
            var box = await message.BoundingBoxAsync();

            // the event of the long press script (the position in the page)
            await message.EvaluateAsync(
                @"(e, p) => e.dispatchEvent(new CustomEvent('long-press', {
                    bubbles: true, cancelable: true,
                    detail: { pageX: p[0] + window.scrollX, pageY: p[1] + window.scrollY }
                }))",
                new[] { (double)box.X + 30, (double)box.Y + 20 });

            var menu = phone.Locator(".context-menu.show").First;
            await menu.WaitForAsync();

            var menuBox = await menu.BoundingBoxAsync();

            menuBox.X.Should().BeApproximately(box.X + 30, 3);
            menuBox.Y.Should().BeApproximately(box.Y + 20, 3);
        }
        finally
        {
            await phone.CloseAsync();
        }
    }

    [Test]
    [Description("The strength of a weak password is shown as weak also after a strong password was entered")]
    public async Task Password_Strength_Weak_After_Strong_Test()
    {
        await this.LogoutAsync();
        await this.GotoAsync("Account/Register");

        var password = this.Control("Password");

        await password.FillAsync("Strong1234!x");
        await password.PressAsync("End");
        (await this.Page.Locator("#progress-password").GetAttributeAsync("class")).Should().Contain("bg-success");

        await password.FillAsync("weakweakweak");
        await password.PressAsync("End");

        (await this.Page.Locator("#progress-password").GetAttributeAsync("class")).Should()
            .Be("progress-bar bg-warning w-50");
    }

    private async Task OpenEditorAsync()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync("PostTopic?f=1");
    }

    private Task SetEditorAsync(string value, int selectionStart, int selectionEnd)
    {
        return this.Control("YafTextEditor").EvaluateAsync(
            "(e, a) => { e.value = a.value; e.focus(); e.setSelectionRange(a.start, a.end); }",
            new { value, start = selectionStart, end = selectionEnd });
    }

    private Task<string> EditorValueAsync()
    {
        return this.Control("YafTextEditor").InputValueAsync();
    }

    private async Task OpenUploadDialogAsync()
    {
        await this.Page.Locator("[data-bs-target='#UploadDialog']").First.ClickAsync();
        await this.Page.Locator("#UploadDialog").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    private async Task<string> OpenNewTopicAsync(string text)
    {
        var subject = $"Script topic {Guid.NewGuid():N}";

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.Page.Locator(".message").First.WaitForAsync();

        return this.Page.Url;
    }
}
