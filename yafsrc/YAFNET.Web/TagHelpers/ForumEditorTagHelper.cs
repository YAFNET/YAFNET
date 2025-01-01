/* Yet Another Forum.NET
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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.TagHelpers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using ServiceStack.OrmLite;

using YAF.Types.Extensions;

/// <summary>
/// The Bootstrap alert tag helper.
/// </summary>
[HtmlTargetElement("textarea", Attributes = "editor-mode")]
public class ForumEditorTagHelper : TagHelper, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    /// Gets or sets the editor mode.
    /// </summary>
    /// <value>The editor mode.</value>
    [HtmlAttributeName("editor-mode")]
    public EditorMode EditorMode { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [users can upload].
    /// </summary>
    /// <value><c>true</c> if [users can upload]; otherwise, <c>false</c>.</value>
    public bool UsersCanUpload { get; set; }

    /// <summary>
    /// Gets or sets the maximum characters.
    /// </summary>
    /// <value>The maximum characters.</value>
    public int MaxCharacters { get; set; }

    /// <summary>
    /// Gets or sets the ASP for.
    /// </summary>
    /// <value>The ASP for.</value>
    public ModelExpression AspFor { get; set; }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="output">
    /// The output.
    /// </param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var containerCardDiv = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "card" } };

        var containerCardHeader = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "card-header pb-0" } };

        var containerCardBody = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "card-body" } };

        if (BoardContext.Current.BoardSettings.EnableWysiwygEditor && this.EditorMode is EditorMode.Standard)
        {
            this.EditorMode = EditorMode.SCEditor;
        }

        if (this.EditorMode is not EditorMode.SCEditor)
        {
            this.RenderPreElement(output, containerCardDiv, containerCardHeader, containerCardBody);
        }

        if (this.UsersCanUpload && this.EditorMode == EditorMode.Standard && BoardContext.Current.UploadAccess)
        {
            BoardContext.Current.InlineElements.InsertJsBlock(
                nameof(JavaScriptBlocks.FileAutoUploadLoadJs),
                JavaScriptBlocks.FileAutoUploadLoadJs(this.Get<IUrlHelper>().Action("Upload", "FileUpload"),
                    this.EditorMode is EditorMode.SCEditor ? "sceditor-container" : "BBCodeEditor"));
        }

        switch (this.EditorMode)
        {
            case EditorMode.SCEditor:
                var language = BoardContext.Current.PageUser.Culture.IsSet()
                    ? BoardContext.Current.PageUser.Culture[..2]
                    : this.Get<BoardSettings>().Culture[..2];

                if (ValidationHelper.IsNumeric(language))
                {
                    language = this.Get<BoardSettings>().Culture;
                }

                var albums = string.Empty;
                var attachments = string.Empty;

                if (this.Get<BoardSettings>().EnableAlbum && BoardContext.Current.NumAlbums > 0)
                {
                    albums = ",albums";
                }

                if (BoardContext.Current.UploadAccess)
                {
                    attachments = "|attachments";
                }

                var toolbar =
                $"bold,italic,underline,strike|font,size,color|mark|email,link,unlink,quote,code,|image{albums}{attachments}|bulletlist,orderedlist,|left,center,right|cut,copy,pastetext,removeformat|undo,redo|youtube,vimeo,instagram,facebook,media|extensions|source|reply";

                var dragDropJs = string.Empty;

                if (this.UsersCanUpload && BoardContext.Current.UploadAccess)
                {
                    dragDropJs = JavaScriptBlocks.SCEditorDragDropJs(this.Get<IUrlHelper>().Action("Upload", "FileUpload"));
                }


                BoardContext.Current.InlineElements.InsertJsBlock(
                    nameof(JavaScriptBlocks.CreateSCEditorJs),
                    JavaScriptBlocks.CreateSCEditorJs(
                        this.AspFor.Name.Replace(".", "_"),
                        this.MaxCharacters,
                        language,
                        toolbar,
                        $"'{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")}', '{this.Get<BoardInfo>().GetUrlToCss("forum.min.css")}'",
                        this.Get<IUrlHelper>().Action(
                            "GetList",
                            "CustomBBCodes"), dragDropJs));
                break;
            case EditorMode.Basic:
                output.Attributes.SetAttribute(HtmlAttribute.Class, "BBCodeEditor form-control");

                BoardContext.Current.InlineElements.InsertJsBlock(
                    nameof(JavaScriptBlocks.CreateEditorJs),
                    JavaScriptBlocks.CreateEditorJs(
                        this.AspFor.Name.Replace(".", "_"),
                        this.GetText("COMMON", "TT_URL_TITLE"),
                        this.GetText("COMMON", "TT_URL_DESC"),
                        this.GetText("COMMON", "TT_IMAGE_TITLE"),
                        this.GetText("COMMON", "TT_IMAGE_DESC"),
                        this.GetText("COMMON", "TT_DESCRIPTION"),
                        this.GetText("COMMON", "TT_MEDIA")));
                break;
            case EditorMode.Standard:
                output.Attributes.SetAttribute(HtmlAttribute.Class, "BBCodeEditor form-control");

                BoardContext.Current.InlineElements.InsertJsBlock(
                    nameof(JavaScriptBlocks.CreateEditorJs),
                    JavaScriptBlocks.CreateEditorJs(
                        this.AspFor.Name.Replace(".", "_"),
                        this.GetText("COMMON", "TT_URL_TITLE"),
                        this.GetText("COMMON", "TT_URL_DESC"),
                        this.GetText("COMMON", "TT_IMAGE_TITLE"),
                        this.GetText("COMMON", "TT_IMAGE_DESC"),
                        this.GetText("COMMON", "TT_DESCRIPTION"),
                        this.GetText("COMMON", "TT_MEDIA")));
                break;
            case EditorMode.Sql:
                {
                    var serverName = OrmLiteConfig.DialectProvider.SQLServerName();

                    var mime = serverName switch
                        {
                            "Microsoft SQL Server" => "text/x-mssql",
                            "MySQL" => "text/x-mysql",
                            "PostgreSQL" => "text/x-pgsql",
                            _ => "text/x-sql"
                        };

                    BoardContext.Current.InlineElements.InsertJsBlock(
                        nameof(JavaScriptBlocks.CodeMirrorSqlLoadJs),
                        JavaScriptBlocks.CodeMirrorSqlLoadJs(this.AspFor.Name.Replace(".", "_"), mime));
                    break;
                }
        }

        if (this.EditorMode != EditorMode.SCEditor)
        {
            this.RenderPostElement(output, containerCardDiv, containerCardBody);
        }

        // register custom BBCode javascript (if there is any)
        // this call is supposed to be after editor load since it may use
        // JS variables created in editor_load...
        this.Get<IBBCodeService>().RegisterCustomBBCodeInlineElements(this.AspFor.Name);
    }

    /// <summary>
    /// Renders the pre element.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="containerCardDiv">The container card div.</param>
    /// <param name="containerCardHeader">The container card header.</param>
    /// <param name="containerCardBody">The container card body.</param>
    private void RenderPreElement(
        TagHelperOutput output,
        TagBuilder containerCardDiv,
        TagBuilder containerCardHeader,
        TagBuilder containerCardBody)
    {
        var content = new HtmlContentBuilder();

        content.AppendHtml(containerCardDiv.RenderStartTag());

        if (this.EditorMode is not EditorMode.Sql)
        {
            content.AppendHtml(containerCardHeader.RenderStartTag());

            switch (this.EditorMode)
            {
                case EditorMode.Basic:
                    this.RenderBasicHeader(content);
                    break;
                case EditorMode.Standard:
                    this.RenderStandardHeader(content);
                    break;
            }

            content.AppendHtml(containerCardHeader.RenderEndTag());
        }

        content.AppendHtml(containerCardBody.RenderStartTag());

        output.PreElement.SetHtmlContent(content);
    }

    /// <summary>
    /// Renders the standard header.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderStandardHeader(HtmlContentBuilder content)
    {
        this.RenderFirstToolbar(content);

        this.RenderSecondToolbar(content);
    }

    /// <summary>
    /// Renders the first toolbar.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderFirstToolbar(HtmlContentBuilder content)
    {
        // Render First Toolbar
        var toolbar = CreateToolbarTag();

        content.AppendHtml(toolbar.RenderStartTag());

        // Group 1
        var group1 = CreateBtnGroupTag();
        content.AppendHtml(group1.RenderStartTag());

        // Render Cut Button
        RenderButton(content, "setStyle('cut','')", this.GetText("COMMON", "TT_CUT"), "scissors");

        // Render Copy Button
        RenderButton(content, "setStyle('copy','')", this.GetText("COMMON", "TT_COPY"), "copy");

        // Render Paste Button
        RenderButton(content, "setStyle('paste','')", this.GetText("COMMON", "TT_PASTE"), "paste");

        content.AppendHtml(group1.RenderEndTag());

        // Group 2
        var group2 = CreateBtnGroupTag();
        content.AppendHtml(group2.RenderStartTag());

        // Render Undo Button
        RenderButton(content, "setStyle('undo','')", this.GetText("COMMON", "TT_UNDO"), "rotate-left", "undo");

        // Render Redo Button
        RenderButton(content, "setStyle('redo','')", this.GetText("COMMON", "TT_REDO"), "rotate-right", "redo");

        content.AppendHtml(group2.RenderEndTag());

        // Group 3
        var group3 = CreateBtnGroupTag();
        content.AppendHtml(group3.RenderStartTag());

        // Render Select All Button
        RenderButton(content, "setStyle('selectAll','')", this.GetText("COMMON", "TT_SELECT_ALL"), "object-group");

        // Render Remove Format Button
        RenderButton(content, "setStyle('removeFormat','')", this.GetText("COMMON", "TT_REMOVE"), "remove-format");

        content.AppendHtml(group3.RenderEndTag());

        content.AppendHtml(toolbar.RenderEndTag());
    }

    /// <summary>
    /// Renders the second toolbar.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderSecondToolbar(HtmlContentBuilder content)
    {
        // Render First Toolbar
        var toolbar = CreateToolbarTag();

        content.AppendHtml(toolbar.RenderStartTag());

        // Group 1
        var group1 = CreateBtnGroupTag();
        content.AppendHtml(group1.RenderStartTag());

        RenderButton(content, "setStyle('bold','')", this.GetText("COMMON", "TT_BOLD"), "bold");
        RenderButton(content, "setStyle('italic','')", this.GetText("COMMON", "TT_ITALIC"), "italic");
        RenderButton(content, "setStyle('underline','')", this.GetText("COMMON", "TT_UNDERLINE"), "underline");
        RenderButton(content, "setStyle('strikethrough','')", this.GetText("COMMON", "TT_STRIKE_THROUGH"), "strikethrough");

        content.AppendHtml(group1.RenderEndTag());

        this.RenderFontNameButton(content);

        this.RenderFontSizeButton(content);

        this.RenderFontColorButton(content);

        // Group 2
        var group2 = CreateBtnGroupTag();
        content.AppendHtml(group2.RenderStartTag());

        RenderButton(content, "setStyle('highlight','')", this.GetText("COMMON", "TT_HIGHLIGHT"), "highlighter");

        content.AppendHtml(group2.RenderEndTag());

        // Group 3
        var group3 = CreateBtnGroupTag();
        content.AppendHtml(group3.RenderStartTag());

        RenderButton(content, "setStyle('email','')", this.GetText("COMMON", "TT_EMAIL"), "envelope");

        RenderButton(content, "setStyle('createlink','')", this.GetText("COMMON", "TT_CREATELINK"), "link");

        RenderButton(content, "setStyle('quote','')", this.GetText("COMMON", "TT_QUOTE"), "quote-left");

        // add drop down for optional "extra" codes...
        var toggleButton1 = new TagBuilder(HtmlTag.Button)
                                {
                                    Attributes =
                                        {
                                            [HtmlAttribute.Type] = HtmlTag.Button,
                                            [HtmlAttribute.Class] = "btn btn-primary dropdown-toggle",
                                            [HtmlAttribute.Title] = this.GetText("COMMON", "TT_CODE"),
                                            ["data-bs-toggle"] = "dropdown",
                                            ["aria-haspopup"] = "true",
                                            [HtmlAttribute.AriaExpanded] = "false"
                                        }
                                };
        content.AppendHtml(toggleButton1.RenderStartTag());

        var icon1 = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = "fa fa-code fa-fw" } };
        content.AppendHtml(icon1);

        content.AppendHtml(toggleButton1.RenderEndTag());

        var highLightList = new List<SelectListItem>
                                {
                                    new() { Value = "markup", Text = "Plain Text" },
                                    new() { Value = "bash", Text = "Bash(shell)" },
                                    new() { Value = "c", Text = "C" },
                                    new() { Value = "cpp", Text = "C++" },
                                    new() { Value = "csharp", Text = "C#" },
                                    new() { Value = "css", Text = "CSS" },
                                    new() { Value = "scss", Text = "SCSS" },
                                    new() { Value = "git", Text = "Git" },
                                    new() { Value = "markup", Text = "HTML" },
                                    new() { Value = "java", Text = "Java" },
                                    new() { Value = "javascript", Text = "JavaScript" },
                                    new() { Value = "json", Text = "JSON" },
                                    new() { Value = "python", Text = "Python" },
                                    new() { Value = "sql", Text = "SQL" },
                                    new() { Value = "markup", Text = "XML" },
                                    new() { Value = "vb", Text = "Visual Basic" }
                                };

        var dropDownMenu1 = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenu1.RenderStartTag());

        highLightList.ForEach(
            item =>
                {
                    var tag = new TagBuilder(HtmlTag.A)
                                  {
                                      Attributes =
                                          {
                                              [HtmlAttribute.Class] = "dropdown-item",
                                              [HtmlAttribute.Href] = "#",
                                              ["onclick"] = $"setStyle('codelang','{item.Value}')"
                                          }
                                  };

                    content.AppendHtml(tag.RenderStartTag());

                    content.Append(item.Text);

                    content.AppendHtml(tag.RenderEndTag());
                });

        content.AppendHtml(dropDownMenu1.RenderEndTag());

        content.AppendHtml(group3.RenderEndTag());

        // Group 4
        var group4 = CreateBtnGroupTag();
        content.AppendHtml(group4.RenderStartTag());

        RenderButton(content, "setStyle('img','')", this.GetText("COMMON", "TT_IMAGE"), "image");

        if (this.Get<BoardSettings>().EnableAlbum && BoardContext.Current.NumAlbums > 0
                                                  && !BoardContext.Current.CurrentForumPage.IsAdminPage)
        {
            // add drop down for optional "extra" codes...
            var toggleButton2 = new TagBuilder(HtmlTag.Button)
                                    {
                                        Attributes =
                                            {
                                                [HtmlAttribute.Type] = HtmlTag.Button,
                                                [HtmlAttribute.Class] = "btn btn-primary dropdown-toggle albums-toggle",
                                                [HtmlAttribute.Title] = this.GetText("COMMON", "ALBUMIMG_CODE"),
                                                ["data-bs-toggle"] = "dropdown",
                                                ["aria-haspopup"] = "true",
                                                [HtmlAttribute.AriaExpanded] = "false"
                                            }
                                    };
            content.AppendHtml(toggleButton2.RenderStartTag());

            var icon2 = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = "fa fa-images fa-fw" } };
            content.AppendHtml(icon2);

            content.AppendHtml(toggleButton2.RenderEndTag());

            var dropDownMenu2 = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu" } };
            content.AppendHtml(dropDownMenu2.RenderStartTag());

            this.RenderAlbumsDropDown(content);

            content.AppendHtml(dropDownMenu2.RenderEndTag());
        }

        content.AppendHtml(group4.RenderEndTag());

        if (this.UsersCanUpload && BoardContext.Current.UploadAccess)
        {
            // Group 5
            var group5 = CreateBtnGroupTag();
            content.AppendHtml(group5.RenderStartTag());

            // add drop down for optional "extra" codes...
            var toggleButton3 = new TagBuilder(HtmlTag.Button)
                                    {
                                        Attributes =
                                            {
                                                [HtmlAttribute.Type] = HtmlTag.Button,
                                                [HtmlAttribute.Class] = "btn btn-primary dropdown-toggle attachments-toggle",
                                                [HtmlAttribute.Title] = this.GetText("COMMON", "ATTACH_BBCODE"),
                                                ["data-bs-toggle"] = "dropdown",
                                                ["aria-haspopup"] = "true",
                                                [HtmlAttribute.AriaExpanded] = "false"
                                            }
                                    };
            content.AppendHtml(toggleButton3.RenderStartTag());

            var icon3 = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = "fa fa-paperclip fa-fw" } };
            content.AppendHtml(icon3);

            content.AppendHtml(toggleButton3.RenderEndTag());

            var dropDownMenu2 = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu" } };
            content.AppendHtml(dropDownMenu2.RenderStartTag());

            this.RenderAttachmentsDropDown(content);

            content.AppendHtml(dropDownMenu2.RenderEndTag());

            content.AppendHtml(group5.RenderEndTag());
        }

        // Group 6
        var group6 = CreateBtnGroupTag();
        content.AppendHtml(group6.RenderStartTag());

        RenderButton(content, "setStyle('unorderedlist','')", this.GetText("COMMON", "TT_LISTUNORDERED"), "list-ul");

        RenderButton(content, "setStyle('orderedlist','')", this.GetText("COMMON", "TT_LISTORDERED"), "list-ol");

        content.AppendHtml(group6.RenderEndTag());

        // Group 7
        var group7 = CreateBtnGroupTag();
        content.AppendHtml(group7.RenderStartTag());

        RenderButton(content, "setStyle('justifyleft','')", this.GetText("COMMON", "TT_ALIGNLEFT"), "align-left");

        RenderButton(content, "setStyle('justifycenter','')", this.GetText("COMMON", "TT_ALIGNCENTER"), "align-center");

        RenderButton(content, "setStyle('justifyright','')", this.GetText("COMMON", "TT_ALIGNRIGHT"), "align-right");

        content.AppendHtml(group7.RenderEndTag());

        // Group 8
        var group8 = CreateBtnGroupTag();
        content.AppendHtml(group8.RenderStartTag());

        RenderButton(content, "setStyle('outdent','')", this.GetText("COMMON", "OUTDENT"), "outdent");

        RenderButton(content, "setStyle('indent','')", this.GetText("COMMON", "INDENT"), "indent");

        var customBbCode = this.Get<IDataCache>().GetOrSet(
            Constants.Cache.CustomBBCode,
            () => this.GetRepository<BBCode>().GetByBoardId());

        var customBbCodesWithToolbar = customBbCode.Where(code => code.UseToolbar == true).ToList();
        var customBbCodesWithNoToolbar =
            customBbCode.Where(code => code.UseToolbar is false or null);

        content.AppendHtml(group8.RenderEndTag());

        if (customBbCode.Any())
        {
            // Group 9
            var group9 = CreateBtnGroupTag();
            content.AppendHtml(group9.RenderStartTag());

            RenderButton(content, "setStyle('media','')", this.GetText("COMMON", "MEDIA"), "photo-film");

            if (customBbCodesWithToolbar.Count != 0)
            {
                customBbCodesWithToolbar.ForEach(
                    row =>
                    {
                        var onclickJs = row.OnClickJS.IsSet() ? row.OnClickJS : $"setStyle('{row.Name.Trim()}','')";

                        var item = new TagBuilder(HtmlTag.Button) {
                            Attributes = {
                                [HtmlAttribute.Type] = HtmlTag.Button,
                                [HtmlAttribute.Class] = "btn btn-primary",
                                ["onclick"] = onclickJs,
                                [HtmlAttribute.Title] = this.Get<IBBCodeService>()
                                    .LocalizeCustomBBCodeElement(row.Description.Trim())
                            }
                        };

                        content.AppendHtml(item.RenderStartTag());

                        var icon = new TagBuilder(HtmlTag.I) {
                            Attributes = { [HtmlAttribute.Class] = $"fab fa-{row.Name.ToLower()} fa-fw" }
                        };

                        content.AppendHtml(icon);

                        content.AppendHtml(item.RenderEndTag());
                    });
            }

            // add drop down for optional "extra" codes...
            var toggleButton4 = new TagBuilder(HtmlTag.Button) {
                Attributes = {
                    [HtmlAttribute.Type] = HtmlTag.Button,
                    [HtmlAttribute.Class] = "btn btn-primary dropdown-toggle",
                    [HtmlAttribute.Title] = this.GetText("COMMON", "CUSTOM_BBCODE"),
                    ["data-bs-toggle"] = "dropdown",
                    ["aria-haspopup"] = "true",
                    [HtmlAttribute.AriaExpanded] = "false"
                }
            };
            content.AppendHtml(toggleButton4.RenderStartTag());

            var icon4 = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = "fa fa-plug fa-fw" } };
            content.AppendHtml(icon4);

            content.AppendHtml(toggleButton4.RenderEndTag());

            var dropDownMenu3 = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu fill-width" } };
            content.AppendHtml(dropDownMenu1.RenderStartTag());

            customBbCodesWithNoToolbar.Where(
                    x => !x.Name.Equals("attach", StringComparison.CurrentCultureIgnoreCase) &&
                         !x.Name.Equals("albumimg", StringComparison.CurrentCultureIgnoreCase) &&
                         !x.Name.Equals("media", StringComparison.CurrentCultureIgnoreCase))
                .ForEach(
                    row =>
                    {
                        var name = row.Name;

                        if (row.Description.IsSet())
                        {
                            // use the description as the option "name"
                            name = this.Get<IBBCodeService>().LocalizeCustomBBCodeElement(row.Description.Trim());
                        }

                        var onclickJs = row.OnClickJS.IsSet() ? row.OnClickJS : $"setStyle('{row.Name.Trim()}','')";

                        var item = new TagBuilder(HtmlTag.A) {
                            Attributes = {
                                [HtmlAttribute.Class] = "dropdown-item",
                                [HtmlAttribute.Href] = "#",
                                ["onclick"] = onclickJs
                            }
                        };

                        content.AppendHtml(item.RenderStartTag());

                        content.Append(name);

                        content.AppendHtml(item.RenderEndTag());
                    });

            content.AppendHtml(dropDownMenu3.RenderEndTag());

            content.AppendHtml(group9.RenderEndTag());
        }

        content.AppendHtml(toolbar.RenderEndTag());
    }

    /// <summary>
    /// Renders the basic header.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderBasicHeader(HtmlContentBuilder content)
    {
        var toolbar = CreateToolbarTag();

        content.AppendHtml(toolbar.RenderStartTag());

        // Group 1
        var group1 = CreateBtnGroupTag();
        content.AppendHtml(group1.RenderStartTag());

        // Render Undo Button
        RenderButton(content, "setStyle('undo','')", this.GetText("COMMON", "TT_UNDO"), "rotate-left", "undo");

        // Render Redo Button
        RenderButton(content, "setStyle('redo','')", this.GetText("COMMON", "TT_REDO"), "rotate-right", "redo");

        content.AppendHtml(group1.RenderEndTag());

        // Group 2
        var group2 = CreateBtnGroupTag();
        content.AppendHtml(group2.RenderStartTag());

        RenderButton(content, "setStyle('bold','')", this.GetText("COMMON", "TT_BOLD"), "bold");
        RenderButton(content, "setStyle('italic','')", this.GetText("COMMON", "TT_ITALIC"), "italic");
        RenderButton(content, "setStyle('underline','')", this.GetText("COMMON", "TT_UNDERLINE"), "underline");

        content.AppendHtml(group2.RenderEndTag());

        // Group 3
        var group3 = CreateBtnGroupTag();
        content.AppendHtml(group3.RenderStartTag());

        RenderButton(content, "setStyle('createlink','')", this.GetText("COMMON", "TT_CREATELINK"), "link");

        RenderButton(content, "setStyle('quote','')", this.GetText("COMMON", "TT_QUOTE"), "quote-left");

        RenderButton(content, "setStyle('img','')", this.GetText("COMMON", "TT_IMAGE"), "image");

        content.AppendHtml(group3.RenderEndTag());

        content.AppendHtml(toolbar.RenderEndTag());
    }

    /// <summary>
    /// Renders the post element.
    /// </summary>
    /// <param name="output">The output.</param>
    /// <param name="containerCardDiv">The container card div.</param>
    /// <param name="containerCardBody">The container card body.</param>
    private void RenderPostElement(TagHelperOutput output, TagBuilder containerCardDiv, TagBuilder containerCardBody)
    {
        var content = new HtmlContentBuilder();

        content.AppendHtml(containerCardBody.RenderEndTag());

        this.RenderFooter(content);

        content.AppendHtml(containerCardDiv.RenderEndTag());

        output.PostElement.SetHtmlContent(content);
    }

    /// <summary>
    /// Renders the footer.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderFooter(HtmlContentBuilder content)
    {
        var containerCardFooter =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "card-footer text-body-secondary text-end" } };

        content.AppendHtml(containerCardFooter.RenderStartTag());

        content.Append(this.GetText("COMMON", "CHARACTERS_LEFT"));

        var spanTag = new TagBuilder(HtmlTag.Span) {
            Attributes = {
                [HtmlAttribute.Id] = "editor-Counter",
                [HtmlAttribute.Class] = "badge bg-secondary ms-1"
            }
        };

        content.AppendHtml(spanTag);

        content.AppendHtml(containerCardFooter.RenderEndTag());
    }

    /// <summary>
    /// Renders the albums drop down.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderAlbumsDropDown(HtmlContentBuilder content)
    {
        var menuTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "AlbumsListMenu dropdown-item" } };

        content.AppendHtml(menuTag.RenderStartTag());

        var listBoxTag =
            new TagBuilder(HtmlTag.Div) {
                Attributes = {
                    [HtmlAttribute.Class] = "content",
                    [HtmlAttribute.Id] = "AlbumsListBox"
                }
            };

        content.AppendHtml(listBoxTag.RenderStartTag());

        // Pager
        var pagerTag =
            new TagBuilder(HtmlTag.Div) { Attributes = {  [HtmlAttribute.Id] = "AlbumsListPager" } };
        content.AppendHtml(pagerTag);

        // Loading box
        var loaderTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Id] = "PostAlbumsLoader", [HtmlAttribute.Class] = "text-center" } };
        content.AppendHtml(loaderTag.RenderStartTag());

        var spanTag =
            new TagBuilder(HtmlTag.Span);
        content.AppendHtml(spanTag.RenderStartTag());
        content.Append(this.Get<ILocalization>().GetText("COMMON", "LOADING"));
        content.AppendHtml(spanTag.RenderEndTag());

        var divTag =
            new TagBuilder(HtmlTag.Div){ Attributes = { [HtmlAttribute.Class] = "fa-3x" } };
        content.AppendHtml(divTag.RenderStartTag());
        var iconLoadTag =
            new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = "fas fa-spinner fa-pulse" } };
        content.AppendHtml(iconLoadTag);
        content.AppendHtml(divTag.RenderEndTag());

        content.AppendHtml(loaderTag.RenderEndTag());

        // PlaceHolder
        var placeHolderTag = new TagBuilder(HtmlTag.Div)
                                 {
                                     Attributes =
                                         {
                                             [HtmlAttribute.Id] = "PostAlbumsListPlaceholder",
                                             ["data-notext"] =
                                                 this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS"),
                                             [HtmlAttribute.Style] = "clear: both;"
                                         }
                                 };
        content.AppendHtml(placeHolderTag.RenderStartTag());

        // List
        var listTag = new TagBuilder(HtmlTag.Ul) { Attributes = { [HtmlAttribute.Class] = "AttachmentList list-group" } };
        content.AppendHtml(listTag);

        content.AppendHtml(placeHolderTag.RenderEndTag());

        content.AppendHtml(listBoxTag.RenderEndTag());

        content.AppendHtml(menuTag.RenderEndTag());
    }

    /// <summary>
    /// Renders the attachments drop down.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderAttachmentsDropDown(HtmlContentBuilder content)
    {
        var menuTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "AttachmentListMenu dropdown-item" } };

        content.AppendHtml(menuTag.RenderStartTag());

        var listBoxTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "content", [HtmlAttribute.Id] = "AttachmentsListBox" } };

        content.AppendHtml(listBoxTag.RenderStartTag());

        // Pager
        var pagerTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Id] = "AttachmentsListPager" } };
        content.AppendHtml(pagerTag);

        // Loading box
        var loaderTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Id] = "PostAttachmentLoader", [HtmlAttribute.Class] = "text-center" } };
        content.AppendHtml(loaderTag.RenderStartTag());

        var spanTag =
            new TagBuilder(HtmlTag.Span);
        content.AppendHtml(spanTag.RenderStartTag());
        content.Append(this.Get<ILocalization>().GetText("COMMON", "LOADING"));
        content.AppendHtml(spanTag.RenderEndTag());

        var divTag =
            new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "fa-3x" } };
        content.AppendHtml(divTag.RenderStartTag());
        var iconLoadTag =
            new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = "fas fa-spinner fa-pulse" } };
        content.AppendHtml(iconLoadTag);
        content.AppendHtml(divTag.RenderEndTag());

        content.AppendHtml(loaderTag.RenderEndTag());

        var placeHolderTag = new TagBuilder(HtmlTag.Div)
                                 {
                                     Attributes =
                                         {
                                             [HtmlAttribute.Id] = "PostAttachmentListPlaceholder",
                                             ["data-notext"] =
                                                 this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS"),
                                             [HtmlAttribute.Style] = "clear: both;"
                                         }
                                 };
        content.AppendHtml(placeHolderTag.RenderStartTag());

        // List
        var listTag = new TagBuilder(HtmlTag.Ul) { Attributes = { [HtmlAttribute.Class] = "AttachmentList list-group" } };
        content.AppendHtml(listTag);

        content.AppendHtml(placeHolderTag.RenderEndTag());

        // Upload Button
        var uploadDivTag = new TagBuilder(HtmlTag.Div)
                                 {
                                     Attributes =
                                         {
                                             [HtmlAttribute.Class] = "OpenUploadDialog mt-1 d-grid gap-2"
                                         }
                                 };
        content.AppendHtml(uploadDivTag.RenderStartTag());

        var uploadButtonTag = new TagBuilder(HtmlTag.Button)
                                  {
                                      Attributes =
                                          {
                                              [HtmlAttribute.Type] = HtmlTag.Button,
                                              [HtmlAttribute.Class] = "btn btn-primary btn-sm",
                                              ["data-bs-toggle"] = "modal",
                                              ["data-bs-target"] = "#UploadDialog"
                                          }
                                  };
        content.AppendHtml(uploadButtonTag.RenderStartTag());
        content.Append(this.GetText("ATTACHMENTS", "UPLOAD_NEW"));
        content.AppendHtml(uploadButtonTag.RenderEndTag());

        content.AppendHtml(uploadDivTag.RenderEndTag());

        content.AppendHtml(listBoxTag.RenderEndTag());

        content.AppendHtml(menuTag.RenderEndTag());
    }

    /// <summary>
    /// Renders the font name button.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderFontNameButton(HtmlContentBuilder content)
    {
        var group = CreateBtnGroupTag();
        content.AppendHtml(group.RenderStartTag());

        var toggleButtonFontNames = new TagBuilder(HtmlTag.Button)
        {
            Attributes =
            {
                [HtmlAttribute.Type] = HtmlTag.Button,
                [HtmlAttribute.Class] = "btn btn-primary btn-sm dropdown-toggle",
                [HtmlAttribute.Title] = this.GetText("COMMON", "FONT_NAME"),
                ["data-bs-toggle"] = "dropdown",
                ["aria-haspopup"] = "true",
                [HtmlAttribute.AriaExpanded] = "false"
            }
        };

        content.AppendHtml(toggleButtonFontNames.RenderStartTag());

        RenderStackButton(content, "font", "font");

        var dropDownMenuFontName = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenuFontName.RenderStartTag());

        string[] fontNames = [
            "Arial",
            "Arial Black",
            "Comic Sans MS",
            "Courier New",
            "Georgia,Impact",
            "Sans-serif",
            "Serif",
            "Times New Roman,Trebuchet MS",
            "Verdana"
        ];

        foreach (var name in fontNames)
        {
            var item = new TagBuilder(HtmlTag.A)
            {
                Attributes = {
                    [HtmlAttribute.Class] = "dropdown-item",
                    [HtmlAttribute.Href] = "#",
                    ["onclick"] =
                        $"setStyle('font', '{name}');",
                    [HtmlAttribute.Style] = $"font-family:{name}"
                }
            };

            content.AppendHtml(item.RenderStartTag());

            content.Append(name);

            content.AppendHtml(item.RenderEndTag());
        }

        content.AppendHtml(dropDownMenuFontName.RenderEndTag());

        content.AppendHtml(group.RenderEndTag());
    }

    /// <summary>
    /// Renders the font size button.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderFontSizeButton(HtmlContentBuilder content)
    {
        var group = CreateBtnGroupTag();
        content.AppendHtml(group.RenderStartTag());

        var toggleButton6 = new TagBuilder(HtmlTag.Button)
        {
            Attributes =
            {
                [HtmlAttribute.Type] = HtmlTag.Button,
                [HtmlAttribute.Class] = "btn btn-primary btn-sm dropdown-toggle",
                [HtmlAttribute.Title] = this.GetText("COMMON", "FONT_SIZE"),
                ["data-bs-toggle"] = "dropdown",
                ["aria-haspopup"] = "true",
                [HtmlAttribute.AriaExpanded] = "false"
            }
        };
        content.AppendHtml(toggleButton6.RenderStartTag());

        RenderStackButton(content, "font", "up-down");

        content.AppendHtml(toggleButton6.RenderEndTag());

        var dropDownMenu5 = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenu5.RenderStartTag());

        for (var index = 1; index < 9; index++)
        {
            var item = new TagBuilder(HtmlTag.A)
            {
                Attributes =
                {
                    [HtmlAttribute.Class] = "dropdown-item",
                    [HtmlAttribute.Href] = "#",
                    ["onclick"] = $"setStyle('fontsize', {index});"
                }
            };

            content.AppendHtml(item.RenderStartTag());

            content.Append(index.Equals(5) ? "Default" : index.ToString());

            content.AppendHtml(item.RenderEndTag());
        }

        content.AppendHtml(dropDownMenu5.RenderEndTag());

        content.AppendHtml(group.RenderEndTag());
    }

    /// <summary>
    /// Renders the font color button.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderFontColorButton(HtmlContentBuilder content)
    {
        var group = CreateBtnGroupTag();
        content.AppendHtml(group.RenderStartTag());

        var toggleButton5 = new TagBuilder(HtmlTag.Button)
        {
            Attributes =
                                        {
                                            [HtmlAttribute.Type] = HtmlTag.Button,
                                            [HtmlAttribute.Class] = "btn btn-primary btn-sm dropdown-toggle",
                                            [HtmlAttribute.Title] = this.GetText("COMMON", "FONT_COLOR"),
                                            ["data-bs-toggle"] = "dropdown",
                                            ["aria-haspopup"] = "true",
                                            [HtmlAttribute.AriaExpanded] = "false"
                                        }
        };
        content.AppendHtml(toggleButton5.RenderStartTag());

        RenderStackButton(content, "font", "palette sce-color");

        content.AppendHtml(toggleButton5.RenderEndTag());

        var dropDownMenu4 = new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenu4.RenderStartTag());

        string[] colors = [
            "Dark Red",
            "Red",
            "Orange",
            "Brown",
            "Yellow",
            "Green",
            "Olive",
            "Cyan",
            "Blue",
            "Dark Blue",
            "Indigo",
            "Violet",
            "White",
            "Black"
        ];

        foreach (var color in colors)
        {
            var item = new TagBuilder(HtmlTag.A)
            {
                Attributes =
                                   {
                                       [HtmlAttribute.Class] = "dropdown-item",
                                       [HtmlAttribute.Href] = "#",
                                       ["onclick"] =
                                           $"setStyle('color', '{color.Replace(" ", string.Empty).ToLower()}');",
                                       [HtmlAttribute.Style] = color == "White"
                                                       ? $"color:{color.Replace(" ", string.Empty).ToLower()};background-color:grey"
                                                       : $"color:{color.Replace(" ", string.Empty).ToLower()}"
                                   }
            };

            content.AppendHtml(item.RenderStartTag());

            content.Append(color);

            content.AppendHtml(item.RenderEndTag());
        }

        content.AppendHtml(dropDownMenu4.RenderEndTag());

        content.AppendHtml(group.RenderEndTag());
    }

    /// <summary>
    /// Renders the stack button.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="stackIcon">The stack icon.</param>
    private static void RenderStackButton(HtmlContentBuilder content, string icon, string stackIcon)
    {
        var stackTag = new TagBuilder(HtmlTag.Span) { Attributes = { [HtmlAttribute.Class] = "fa-stack fa-stack-editor-button me-3 pe-2" } };

        content.AppendHtml(stackTag.RenderStartTag());

        var iconTag = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = $"fas fa-{icon} fa-stack-1x fa-stack-1x-editor-button" } };
        content.AppendHtml(iconTag);

        var stackIconTag = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = $"fa fa-xs fa-{stackIcon} fa-badge-editor-button" } };
        content.AppendHtml(stackIconTag);

        content.AppendHtml(stackTag.RenderEndTag());
    }

    /// <summary>
    /// The render button.
    /// </summary>
    /// <param name="content">The Content Builder.</param>
    /// <param name="command">The command.</param>
    /// <param name="title">The title.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="id">The identifier.</param>
    private static void RenderButton(
        HtmlContentBuilder content,
        string command,
        string title,
        string icon,
        string id = null)
    {
        var iconTag = new TagBuilder(HtmlTag.I) { Attributes = { [HtmlAttribute.Class] = $"fa fa-{icon} fa-fw" } };

        var button = new TagBuilder(HtmlTag.Button)
                         {
                             Attributes =
                                 {
                                     [HtmlAttribute.Class] = "btn btn-primary btn-sm",
                                     ["onclick"] = command,
                                     [HtmlAttribute.Type] = HtmlTag.Button,
                                     [HtmlAttribute.Title] = title
                                 }
                         };

        if (id.IsSet())
        {
            button.Attributes.Add(HtmlAttribute.Id, id);
        }

        content.AppendHtml(button.RenderStartTag());

        content.AppendHtml(iconTag);

        content.AppendHtml(button.RenderEndTag());
    }

    /// <summary>
    /// Creates the toolbar tag.
    /// </summary>
    /// <returns>TagBuilder.</returns>
    private static TagBuilder CreateToolbarTag()
    {
        return new TagBuilder(HtmlTag.Div) { Attributes = { [HtmlAttribute.Class] = "btn-toolbar", [HtmlAttribute.Role] = "toolbar" } };
    }

    /// <summary>
    /// Creates the BTN group tag.
    /// </summary>
    /// <returns>TagBuilder.</returns>
    private static TagBuilder CreateBtnGroupTag()
    {
        return new TagBuilder(HtmlTag.Div)
                   {
                       Attributes = { [HtmlAttribute.Class] = "btn-group btn-group-sm me-2 mb-2", [HtmlAttribute.Role] = "group" }
                   };
    }
}