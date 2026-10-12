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

namespace YAF.Tests.CoreTests.Utilities;

using Acornima;

using YAF.Core.Helpers;
using YAF.Core.Utilities;
using YAF.Tests.CoreTests.TestInfrastructure;

/// <summary>
/// The generated JavaScript blocks are valid scripts, also with texts that contain quotes, line breaks or markup
/// (translations like "l'URL", board names, notification messages).
/// </summary>
[TestFixture]
[NonParallelizable]
public class JavaScriptBlocksTests
{
    /// <summary>
    /// A text that breaks a JavaScript string literal if it is not encoded.
    /// </summary>
    private const string Evil = "l'URL \"LinkBack\" \\ </script><script>alert(1)</script>\r\nnext line";

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
        this.board.BoardSettings.Name = "The \"Best\" Board's";
        this.board.Localization.Add("ATTACHMENTS", "NO_ATTACHMENTS", "No 'attachments'");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    /// <summary>
    /// All blocks with text parameters, called with the evil text.
    /// </summary>
    private static IEnumerable<TestCaseData> TextBlocks()
    {
        yield return Case("CreateSubscriptionsJs", () => JavaScriptBlocks.CreateSubscriptionsJs(Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil));
        yield return Case("GetBoardTagsJs", () => JavaScriptBlocks.GetBoardTagsJs("Tags", "TagsValue", Evil));
        yield return Case("CollapseToggleJs", () => JavaScriptBlocks.CollapseToggleJs(Evil, Evil));
        yield return Case("CreateEditorJs", () => JavaScriptBlocks.CreateEditorJs("editor", Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil));
        yield return Case("SelectTopicsLoadJs", () => JavaScriptBlocks.SelectTopicsLoadJs("TopicSelect", "TopicId", "ForumSelect", Evil));
        yield return Case("SelectForumsLoadJs", () => JavaScriptBlocks.SelectForumsLoadJs("ForumSelect", Evil, true, false, "ForumId"));
        yield return Case("PasswordStrengthCheckerJs", () => JavaScriptBlocks.PasswordStrengthCheckerJs("Password", "Confirm", 8, Evil, Evil, Evil, Evil, Evil));
        yield return Case("DoQuickSearchJs", () => JavaScriptBlocks.DoQuickSearchJs(Evil));
        yield return Case("TopicLinkPopoverJs", () => JavaScriptBlocks.TopicLinkPopoverJs(Evil, ".topic-link-popover", "focus hover"));
        yield return Case("ForumModsPopoverJs", () => JavaScriptBlocks.ForumModsPopoverJs(Evil));
        yield return Case("BootBoxConfirmJs", () => JavaScriptBlocks.BootBoxConfirmJs(Evil, Evil, Evil, Evil, Evil));
        yield return Case("NotifyJs", () => JavaScriptBlocks.NotifyJs("warning", Evil));
        yield return Case("BootBoxPromptJs", () => JavaScriptBlocks.BootBoxPromptJs(Evil, Evil, Evil, Evil, Evil));
        yield return Case("SelectUsersLoadJs", () => JavaScriptBlocks.SelectUsersLoadJs("UserSelect", "UserID", Evil));
        yield return Case("LogOutJs", () => JavaScriptBlocks.LogOutJs(Evil, Evil, Evil, Evil, Evil));
        yield return Case("ToggleDiffSelectionJs", () => JavaScriptBlocks.ToggleDiffSelectionJs(Evil));
    }

    /// <summary>
    /// The blocks without (or with only developer defined) parameters.
    /// </summary>
    private static IEnumerable<TestCaseData> OtherBlocks()
    {
        yield return Case("MultiQuoteCallbackSuccessJs", () => JavaScriptBlocks.MultiQuoteCallbackSuccessJs);
        yield return Case("MultiQuoteButtonJs", () => JavaScriptBlocks.MultiQuoteButtonJs);
        yield return Case("BootstrapTabsLoadJs", () => JavaScriptBlocks.BootstrapTabsLoadJs("ProfileTabs", "LastTab"));
        yield return Case("EditUserTabsLoadJs", () => JavaScriptBlocks.EditUserTabsLoadJs("Admin", 5, "var x = 1;"));
        yield return Case("LoadGotoAnchor", () => JavaScriptBlocks.LoadGotoAnchor("post5"));
        yield return Case("AddThanksJs", () => JavaScriptBlocks.AddThanksJs("'<span>' + response.text + '</span>'"));
        yield return Case("RemoveThanksJs", () => JavaScriptBlocks.RemoveThanksJs("'<span>' + response.text + '</span>'"));
        yield return Case("CreateSCEditorJs", () => JavaScriptBlocks.CreateSCEditorJs("editor", 5000, "de", "bold,italic", "'/css/editor.css'", "/extensions", string.Empty));
        yield return Case("CreateSCEditorJs with drag and drop", () => JavaScriptBlocks.CreateSCEditorJs("editor", 5000, "de", "bold,italic", "'/css/editor.css'", "/extensions", JavaScriptBlocks.SCEditorDragDropJs("/api/FileUpload/Upload")));
        yield return Case("FileAutoUploadLoadJs", () => JavaScriptBlocks.FileAutoUploadLoadJs("/api/FileUpload/Upload", ".EditorDiv"));
        yield return Case("FileUploadLoadJs", () => JavaScriptBlocks.FileUploadLoadJs("/api/FileUpload/Upload"));
        yield return Case("LoadTopicsByForumJs", JavaScriptBlocks.LoadTopicsByForumJs);
        yield return Case("SelectForumsLoadJs all forums", () => JavaScriptBlocks.SelectForumsLoadJs("ForumSelect", "Forum", false, true, null, JavaScriptBlocks.LoadTopicsByForumJs()));
        yield return Case("ForumIconLegendPopoverJs", () => JavaScriptBlocks.ForumIconLegendPopoverJs("<span class='legend'></span>", "forum-icon-legend-popvover"));
        yield return Case("HoverCardJs", JavaScriptBlocks.HoverCardJs);
        // the onkeydown handler of an input
        yield return Case("ClickOnEnterJs", () => $"function onKeyDown(event) {{ {JavaScriptBlocks.ClickOnEnterJs("SearchButton")} }}");
        yield return Case("ModalNotifyJs", JavaScriptBlocks.ModalNotifyJs);
        yield return Case("DialogFunctionsJs", () => JavaScriptBlocks.DialogFunctionsJs(JavaScriptBlocks.ChoicesLoadJs));
        yield return Case("SetGroupMaskJs", () => JavaScriptBlocks.SetGroupMaskJs);
        yield return Case("CookieConsentJs", () => JavaScriptBlocks.CookieConsentJs);
        yield return Case("ChoicesLoadJs", () => JavaScriptBlocks.ChoicesLoadJs);
        yield return Case("FormValidatorJs", () => JavaScriptBlocks.FormValidatorJs);
        yield return Case("ToolTipJs", () => JavaScriptBlocks.ToolTipJs);
    }

    [TestCaseSource(nameof(TextBlocks))]
    [Description("Blocks with texts are valid JavaScript and the text can not end the script element")]
    public void Text_Block_Is_Valid_Script_Test(Func<string> block)
    {
        var script = block();

        AssertValidScript(script);

        script.Should().NotContain("</script>", "a text must not end the script element");
        script.Should().NotContain("l'URL", "quotes in texts are encoded");
    }

    [TestCaseSource(nameof(OtherBlocks))]
    [Description("The other blocks are valid JavaScript")]
    public void Block_Is_Valid_Script_Test(Func<string> block)
    {
        AssertValidScript(block());
    }

    [Test]
    [Description("The encoded text is shown unchanged in the browser")]
    public void Text_Is_Kept_Test()
    {
        var script = JavaScriptBlocks.NotifyJs("info", "Hello \"you\"");

        script.Should().Contain("message: \"Hello \\\"you\\\"\"")
            .And.Contain("title: \"The \\u0026quot;Best\\u0026quot; Board\\u0026#39;s\"")
            .And.Contain("var type = \"info\";");
    }

    [Test]
    [Description("The notify script shows the title as html: the name of the board is html encoded")]
    public void Notify_Title_Is_Html_Encoded_Test()
    {
        this.board.BoardSettings.Name = "Board <img src=x onerror=alert(1)>";

        var script = JavaScriptBlocks.NotifyJs("info", "Hello");

        script.Should().NotContain("<img").And.NotContain("\\u003Cimg", "the script decodes the escaped characters");
        script.Should().Contain("title: \"Board \\u0026lt;img");
    }

    [Test]
    [Description("The minified blocks stay valid JavaScript")]
    public void Minified_Blocks_Are_Valid_Test()
    {
        AssertValidScript(JsAndCssHelper.CompressJavaScript(JavaScriptBlocks.NotifyJs("warning", Evil)));
        AssertValidScript(JsAndCssHelper.CompressJavaScript(JavaScriptBlocks.LogOutJs(Evil, Evil, Evil, Evil, "LogoutForm")));
        AssertValidScript(JsAndCssHelper.CompressJavaScript(JavaScriptBlocks.SetGroupMaskJs));
    }

    [Test]
    [Description("The logout dialog posts the logout form (with the antiforgery token), it does not open the logout link")]
    public void LogOut_Submits_Form_Test()
    {
        var script = JavaScriptBlocks.LogOutJs("Logout", "Really?", "Yes", "No", "LogoutForm");

        script.Should().Contain("document.getElementById('LogoutForm').submit();")
            .And.NotContain("document.location.href");
    }

    private static TestCaseData Case(string name, Func<string> block)
    {
        return new TestCaseData(block).SetArgDisplayNames(name);
    }

    private static void AssertValidScript(string script)
    {
        FluentActions.Invoking(() => new Parser().ParseScript(script)).Should().NotThrow(script);
    }
}
