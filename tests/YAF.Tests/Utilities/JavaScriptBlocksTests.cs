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

namespace YAF.Tests.Utilities;

using Acornima;

using YAF.Core.Helpers;
using YAF.Core.Utilities;
using YAF.Tests.TestInfrastructure;

/// <summary>
/// The generated JavaScript blocks are valid scripts, also with texts that contain quotes, line breaks or markup
/// (translations like "l'URL", board names, user names).
/// </summary>
[TestFixture]
[NonParallelizable]
public class JavaScriptBlocksTests
{
    /// <summary>
    /// The backslash of the escaped characters.
    /// </summary>
    private const char Backslash = (char)92;

    /// <summary>
    /// A text that breaks a JavaScript string literal if it is not encoded.
    /// </summary>
    private static readonly string Evil =
        $"l'URL \"LinkBack\" {Backslash} </script><script>alert(1)</script>{(char)13}{(char)10}next line";

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
        yield return Case("CollapseToggleJs", () => JavaScriptBlocks.CollapseToggleJs(Evil, Evil));
        yield return Case("CreateEditorJs", () => JavaScriptBlocks.CreateEditorJs("editor", Evil, Evil, Evil, Evil, Evil, Evil, Evil, Evil));
        yield return Case("SelectTopicsLoadJs", () => JavaScriptBlocks.SelectTopicsLoadJs("TopicSelect", "TopicId", "ForumSelect", Evil));
        yield return Case("SelectForumsLoadJs", () => JavaScriptBlocks.SelectForumsLoadJs("ForumSelect", Evil, true, false, "ForumId"));
        yield return Case("PasswordStrengthCheckerJs", () => JavaScriptBlocks.PasswordStrengthCheckerJs("Password", "Confirm", 8, Evil, Evil, Evil, Evil, Evil));
        yield return Case("TopicLinkPopoverJs", () => JavaScriptBlocks.TopicLinkPopoverJs(Evil, ".topic-link-popover", "focus hover"));
        yield return Case("ForumModsPopoverJs", () => JavaScriptBlocks.ForumModsPopoverJs(Evil));
        yield return Case("BootBoxConfirmJs", () => JavaScriptBlocks.BootBoxConfirmJs(Evil, Evil, Evil, Evil, Evil));
        yield return Case("BootBoxPromptJs", () => JavaScriptBlocks.BootBoxPromptJs(Evil, Evil, Evil, Evil, Evil));
        yield return Case("SelectUsersLoadJs", () => JavaScriptBlocks.SelectUsersLoadJs("UserSelect", "UserID", Evil));
        yield return Case("LogOutJs", () => JavaScriptBlocks.LogOutJs(Evil, Evil, Evil, Evil, Evil));
        yield return Case("ToggleDiffSelectionJs", () => JavaScriptBlocks.ToggleDiffSelectionJs(Evil));
    }

    /// <summary>
    /// The blocks with the name of the board as title of the notifications.
    /// </summary>
    private static IEnumerable<TestCaseData> NotifyBlocks()
    {
        yield return Case("SCEditorDragDropJs", () => $"var x = {{ a: 1 {JavaScriptBlocks.SCEditorDragDropJs("/api/FileUpload/Upload")} }};");
        yield return Case("FileAutoUploadLoadJs", () => JavaScriptBlocks.FileAutoUploadLoadJs("/api/FileUpload/Upload", "EditorDiv"));
        yield return Case("FileUploadLoadJs", () => JavaScriptBlocks.FileUploadLoadJs("/api/FileUpload/Upload"));
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

    [TestCaseSource(nameof(NotifyBlocks))]
    [Description("The name of the board is encoded in the title of the notifications (shown as html)")]
    public void Notify_Title_Is_Encoded_Test(Func<string> block)
    {
        this.board.BoardSettings.Name = "Board <img src=x onerror=alert(1)> \"x\"";

        var script = block();

        AssertValidScript(script);
        script.Should().NotContain("<img").And.NotContain($"{Backslash}u003Cimg", "the script decodes the escaped characters");
        script.Should().Contain($"Board {Backslash}u0026lt;img");
    }

    [Test]
    [Description("The encoded text is shown unchanged in the browser")]
    public void Text_Is_Kept_Test()
    {
        var script = JavaScriptBlocks.ToggleDiffSelectionJs("Hello \"you\"");

        script.Should().Contain($"bootbox.alert(\"Hello {Backslash}\"you{Backslash}\"\");");
    }

    [Test]
    [Description("The minified blocks stay valid JavaScript")]
    public void Minified_Blocks_Are_Valid_Test()
    {
        AssertValidScript(JsAndCssHelper.CompressJavaScript(JavaScriptBlocks.BootBoxConfirmJs(Evil, Evil, Evil, Evil, Evil)));
        AssertValidScript(JsAndCssHelper.CompressJavaScript(JavaScriptBlocks.LogOutJs(Evil, Evil, Evil, Evil, "/logout")));
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
