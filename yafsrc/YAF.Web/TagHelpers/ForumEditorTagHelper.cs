/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using YAF.Types.Attributes;
using YAF.Types.Extensions;
using static Lucene.Net.Util.Packed.PackedInt32s;

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
        var containerCardDiv = new TagBuilder("div") { Attributes = { ["class"] = "card" } };

        var containerCardHeader = new TagBuilder("div") { Attributes = { ["class"] = "card-header pb-0" } };

        var containerCardBody = new TagBuilder("div") { Attributes = { ["class"] = "card-body" } };

        this.RenderPreElement(output, containerCardDiv, containerCardHeader, containerCardBody);

        if (this.UsersCanUpload && this.EditorMode == EditorMode.Standard && BoardContext.Current.UploadAccess)
        {
            BoardContext.Current.InlineElements.InsertJsBlock(
                "autoUpload",
                JavaScriptBlocks.FileAutoUploadLoadJs(
                    this.Get<BoardSettings>().AllowedFileExtensions.Replace(",", "|"),
                    this.Get<BoardSettings>().MaxFileSize,
                    this.Get<IUrlHelper>().Action("Upload", "FileUpload"),
                    BoardContext.Current.BoardSettings.ImageAttachmentResizeWidth,
                    BoardContext.Current.BoardSettings.ImageAttachmentResizeHeight));
        }

        switch (this.EditorMode)
        {
            case EditorMode.Basic:
                output.Attributes.SetAttribute("class", "BBCodeEditor form-control");

                BoardContext.Current.InlineElements.InsertJsBlock(
                    nameof(JavaScriptBlocks.CreateYafEditorJs),
                    JavaScriptBlocks.CreateYafEditorJs(
                        this.AspFor.Name.Replace(".", "_"),
                        this.GetText("COMMON", "TT_URL_TITLE"),
                        this.GetText("COMMON", "TT_URL_DESC"),
                        this.GetText("COMMON", "TT_IMAGE_TITLE"),
                        this.GetText("COMMON", "TT_IMAGE_DESC"),
                        this.GetText("COMMON", "TT_DESCRIPTION")));
                break;
            case EditorMode.Standard:
                output.Attributes.SetAttribute("class", "BBCodeEditor form-control");

                BoardContext.Current.InlineElements.InsertJsBlock(
                    nameof(JavaScriptBlocks.CreateYafEditorJs),
                    JavaScriptBlocks.CreateYafEditorJs(
                        this.AspFor.Name.Replace(".", "_"),
                        this.GetText("COMMON", "TT_URL_TITLE"),
                        this.GetText("COMMON", "TT_URL_DESC"),
                        this.GetText("COMMON", "TT_IMAGE_TITLE"),
                        this.GetText("COMMON", "TT_IMAGE_DESC"),
                        this.GetText("COMMON", "TT_DESCRIPTION")));
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

        this.RenderPostElement(output, containerCardDiv, containerCardBody);

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

        content.AppendHtml(containerCardBody.RenderStartTag());

        output.PreElement.SetHtmlContent(content);
    }

    /// <summary>
    /// Renders the standard header.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderStandardHeader(IHtmlContentBuilder content)
    {
        this.RenderFirstToolbar(content);

        this.RenderSecondToolbar(content);
    }

    /// <summary>
    /// Renders the first toolbar.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderFirstToolbar(IHtmlContentBuilder content)
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
        //

        // Group 2
        var group2 = CreateBtnGroupTag();
        content.AppendHtml(group2.RenderStartTag());

        // Render Undo Button
        RenderButton(content, "setStyle('undo','')", this.GetText("COMMON", "TT_UNDO"), "rotate-left", "undo");

        // Render Redo Button
        RenderButton(content, "setStyle('redo','')", this.GetText("COMMON", "TT_REDO"), "rotate-right", "redo");

        content.AppendHtml(group2.RenderEndTag());
        //

        // Group 3
        var group3 = CreateBtnGroupTag();
        content.AppendHtml(group3.RenderStartTag());

        // Render Select All Button
        RenderButton(content, "setStyle('selectAll','')", this.GetText("COMMON", "TT_SELECT_ALL"), "object-group");

        // Render Remove Format Button
        RenderButton(content, "setStyle('removeFormat','')", this.GetText("COMMON", "TT_REMOVE"), "remove-format");

        content.AppendHtml(group3.RenderEndTag());
        //

        content.AppendHtml(toolbar.RenderEndTag());
    }

    /// <summary>
    /// Renders the second toolbar.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderSecondToolbar(IHtmlContentBuilder content)
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

        content.AppendHtml(group1.RenderEndTag());
        //

        // Group 11
        var group11 = CreateBtnGroupTag();
        content.AppendHtml(group11.RenderStartTag());

        // add drop down for optional "extra" codes...
        var toggleButton6 = new TagBuilder("button")
                                {
                                    Attributes =
                                        {
                                            ["type"] = "button",
                                            ["class"] = "btn btn-primary btn-sm dropdown-toggle",
                                            ["title"] = this.GetText("COMMON", "FONT_SIZE"),
                                            ["data-bs-toggle"] = "dropdown",
                                            ["aria-haspopup"] = "true",
                                            ["aria-expanded"] = "false"
                                        }
                                };
        content.AppendHtml(toggleButton6.RenderStartTag());

        var icon6 = new TagBuilder("i") { Attributes = { ["class"] = "fa fa-font fa-fw me-2" } };
        content.AppendHtml(icon6);

        content.Append(this.GetText("COMMON", "FONT_SIZE"));

        content.AppendHtml(toggleButton6.RenderEndTag());

        var dropDownMenu5 = new TagBuilder("div") { Attributes = { ["class"] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenu5.RenderStartTag());

        for (var index = 1; index < 9; index++)
        {
            var item = new TagBuilder("a")
                           {
                               Attributes =
                                   {
                                       ["class"] = "dropdown-item",
                                       ["href"] = "#",
                                       ["onclick"] = $"setStyle('fontsize', {index});"
                                   }
                           };

            content.AppendHtml(item.RenderStartTag());

            content.Append(index.Equals(5) ? "Default" : index.ToString());

            content.AppendHtml(item.RenderEndTag());
        }

        content.AppendHtml(dropDownMenu5.RenderEndTag());

        content.AppendHtml(group11.RenderEndTag());
        //

        // Group 10
        var group10 = CreateBtnGroupTag();
        content.AppendHtml(group10.RenderStartTag());

        // add drop down for optional "extra" codes...
        var toggleButton5 = new TagBuilder("button")
                                {
                                    Attributes =
                                        {
                                            ["type"] = "button",
                                            ["class"] = "btn btn-primary btn-sm dropdown-toggle",
                                            ["title"] = this.GetText("COMMON", "FONT_COLOR"),
                                            ["data-bs-toggle"] = "dropdown",
                                            ["aria-haspopup"] = "true",
                                            ["aria-expanded"] = "false"
                                        }
                                };
        content.AppendHtml(toggleButton5.RenderStartTag());

        var icon5 = new TagBuilder("i") { Attributes = { ["class"] = "fa fa-font fa-fw me-2" } };
        content.AppendHtml(icon5);
        content.Append(this.GetText("COMMON", "FONT_COLOR"));

        content.AppendHtml(toggleButton5.RenderEndTag());

        var dropDownMenu4 = new TagBuilder("div") { Attributes = { ["class"] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenu4.RenderStartTag());

        string[] colors =
            {
                "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue", "Dark Blue", "Indigo",
                "Violet", "White", "Black"
            };

        foreach (var color in colors)
        {
            var item = new TagBuilder("a")
                           {
                               Attributes =
                                   {
                                       ["class"] = "dropdown-item",
                                       ["href"] = "#",
                                       ["onclick"] =
                                           $"setStyle('color', '{color.Replace(" ", string.Empty).ToLower()}');",
                                       ["style"] = color == "White"
                                                       ? $"color:{color.Replace(" ", string.Empty).ToLower()};background-color:grey"
                                                       : $"color:{color.Replace(" ", string.Empty).ToLower()}"
                                   }
                           };

            content.AppendHtml(item.RenderStartTag());

            content.Append(color);

            content.AppendHtml(item.RenderEndTag());
        }

        content.AppendHtml(dropDownMenu4.RenderEndTag());

        content.AppendHtml(group10.RenderEndTag());

        // Group 2
        var group2 = CreateBtnGroupTag();
        content.AppendHtml(group2.RenderStartTag());

        RenderButton(content, "setStyle('highlight','')", this.GetText("COMMON", "TT_HIGHLIGHT"), "pen-square");

        content.AppendHtml(group2.RenderEndTag());
        //

        // Group 3
        var group3 = CreateBtnGroupTag();
        content.AppendHtml(group3.RenderStartTag());

        RenderButton(content, "setStyle('createlink','')", this.GetText("COMMON", "TT_CREATELINK"), "link");

        RenderButton(content, "setStyle('quote','')", this.GetText("COMMON", "TT_QUOTE"), "quote-left");

        // add drop down for optional "extra" codes...
        var toggleButton1 = new TagBuilder("button")
                                {
                                    Attributes =
                                        {
                                            ["type"] = "button",
                                            ["class"] = "btn btn-primary dropdown-toggle",
                                            ["title"] = this.GetText("COMMON", "TT_CODE"),
                                            ["data-bs-toggle"] = "dropdown",
                                            ["aria-haspopup"] = "true",
                                            ["aria-expanded"] = "false"
                                        }
                                };
        content.AppendHtml(toggleButton1.RenderStartTag());

        var icon1 = new TagBuilder("i") { Attributes = { ["class"] = "fa fa-code fa-fw" } };
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
                                    new() { Value = "python", Text = "Python" },
                                    new() { Value = "sql", Text = "SQL" },
                                    new() { Value = "markup", Text = "XML" },
                                    new() { Value = "vb", Text = "Visual Basic" }
                                };

        var dropDownMenu1 = new TagBuilder("div") { Attributes = { ["class"] = "dropdown-menu" } };
        content.AppendHtml(dropDownMenu1.RenderStartTag());

        highLightList.ForEach(
            item =>
                {
                    var tag = new TagBuilder("a")
                                  {
                                      Attributes =
                                          {
                                              ["class"] = "dropdown-item",
                                              ["href"] = "#",
                                              ["onclick"] = $"setStyle('codelang','{item.Value}')"
                                          }
                                  };

                    content.AppendHtml(tag.RenderStartTag());

                    content.Append(item.Text);

                    content.AppendHtml(tag.RenderEndTag());
                });

        content.AppendHtml(dropDownMenu1.RenderEndTag());

        content.AppendHtml(group3.RenderEndTag());
        //

        // Group 4
        var group4 = CreateBtnGroupTag();
        content.AppendHtml(group4.RenderStartTag());

        RenderButton(content, "setStyle('img','')", this.GetText("COMMON", "TT_IMAGE"), "image");

        if (this.Get<BoardSettings>().EnableAlbum && BoardContext.Current.NumAlbums > 0
                                                  && !BoardContext.Current.CurrentForumPage.IsAdminPage)
        {
            // add drop down for optional "extra" codes...
            var toggleButton2 = new TagBuilder("button")
                                    {
                                        Attributes =
                                            {
                                                ["type"] = "button",
                                                ["class"] = "btn btn-primary dropdown-toggle albums-toggle",
                                                ["title"] = this.GetText("COMMON", "ALBUMIMG_CODE"),
                                                ["data-bs-toggle"] = "dropdown",
                                                ["aria-haspopup"] = "true",
                                                ["aria-expanded"] = "false"
                                            }
                                    };
            content.AppendHtml(toggleButton2.RenderStartTag());

            var icon2 = new TagBuilder("i") { Attributes = { ["class"] = "fa fa-images fa-fw" } };
            content.AppendHtml(icon2);

            content.AppendHtml(toggleButton2.RenderEndTag());

            var dropDownMenu2 = new TagBuilder("div") { Attributes = { ["class"] = "dropdown-menu" } };
            content.AppendHtml(dropDownMenu2.RenderStartTag());

            this.RenderAlbumsDropDown(content);

            content.AppendHtml(dropDownMenu2.RenderEndTag());
            //
        }

        content.AppendHtml(group4.RenderEndTag());
        //

        if (this.UsersCanUpload && BoardContext.Current.UploadAccess)
        {
            // Group 5
            var group5 = CreateBtnGroupTag();
            content.AppendHtml(group5.RenderStartTag());

            // add drop down for optional "extra" codes...
            var toggleButton3 = new TagBuilder("button")
                                    {
                                        Attributes =
                                            {
                                                ["type"] = "button",
                                                ["class"] = "btn btn-primary dropdown-toggle attachments-toggle",
                                                ["title"] = this.GetText("COMMON", "ATTACH_BBCODE"),
                                                ["data-bs-toggle"] = "dropdown",
                                                ["aria-haspopup"] = "true",
                                                ["aria-expanded"] = "false"
                                            }
                                    };
            content.AppendHtml(toggleButton3.RenderStartTag());

            var icon3 = new TagBuilder("i") { Attributes = { ["class"] = "fa fa-paperclip fa-fw" } };
            content.AppendHtml(icon3);

            content.AppendHtml(toggleButton3.RenderEndTag());

            var dropDownMenu2 = new TagBuilder("div") { Attributes = { ["class"] = "dropdown-menu" } };
            content.AppendHtml(dropDownMenu2.RenderStartTag());

            this.RenderAttachmentsDropDown(content);

            content.AppendHtml(dropDownMenu2.RenderEndTag());
            //

            content.AppendHtml(group5.RenderEndTag());
        }

        // Group 6
        var group6 = CreateBtnGroupTag();
        content.AppendHtml(group6.RenderStartTag());

        RenderButton(content, "setStyle('unorderedlist','')", this.GetText("COMMON", "TT_LISTUNORDERED"), "list-ul");

        RenderButton(content, "setStyle('orderedlist','')", this.GetText("COMMON", "TT_LISTORDERED"), "list-ol");

        content.AppendHtml(group6.RenderEndTag());
        //

        // Group 7
        var group7 = CreateBtnGroupTag();
        content.AppendHtml(group7.RenderStartTag());

        RenderButton(content, "setStyle('justifyleft','')", this.GetText("COMMON", "TT_ALIGNLEFT"), "align-left");

        RenderButton(content, "setStyle('justifycenter','')", this.GetText("COMMON", "TT_ALIGNCENTER"), "align-center");

        RenderButton(content, "setStyle('justifyright','')", this.GetText("COMMON", "TT_ALIGNRIGHT"), "align-right");

        content.AppendHtml(group7.RenderEndTag());
        //

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
            customBbCode.Where(code => code.UseToolbar == false || code.UseToolbar.HasValue == false);

        content.AppendHtml(group8.RenderEndTag());
        //

        if (customBbCode.Any())
        {
            // Group 9
            var group9 = CreateBtnGroupTag();
            content.AppendHtml(group9.RenderStartTag());

            if (customBbCodesWithToolbar.Any())
            {
                customBbCodesWithToolbar.ForEach(
                    row =>
                        {
                            var onclickJs = row.OnClickJS.IsSet() ? row.OnClickJS : $"setStyle('{row.Name.Trim()}','')";

                            var item = new TagBuilder("button")
                                           {
                                               Attributes =
                                                   {
                                                       ["type"] = "button",
                                                       ["class"] = "btn btn-primary",
                                                       ["onclick"] = onclickJs,
                                                       ["title"] = this.Get<IBBCodeService>()
                                                           .LocalizeCustomBBCodeElement(row.Description.Trim())
                                                   }
                                           };

                            content.AppendHtml(item.RenderStartTag());

                            var icon = new TagBuilder("i")
                                           {
                                               Attributes = { ["class"] = $"fab fa-{row.Name.ToLower()} fa-fw" }
                                           };

                            content.AppendHtml(icon);

                            content.AppendHtml(item.RenderEndTag());
                        });
            }

            // add drop down for optional "extra" codes...
            var toggleButton4 = new TagBuilder("button")
                                    {
                                        Attributes =
                                            {
                                                ["type"] = "button",
                                                ["class"] = "btn btn-primary dropdown-toggle",
                                                ["title"] = this.GetText("COMMON", "CUSTOM_BBCODE"),
                                                ["data-bs-toggle"] = "dropdown",
                                                ["aria-haspopup"] = "true",
                                                ["aria-expanded"] = "false"
                                            }
                                    };
            content.AppendHtml(toggleButton4.RenderStartTag());

            var icon4 = new TagBuilder("i") { Attributes = { ["class"] = "fa fa-plug fa-fw" } };
            content.AppendHtml(icon4);

            content.AppendHtml(toggleButton4.RenderEndTag());

            var dropDownMenu3 = new TagBuilder("div") { Attributes = { ["class"] = "dropdown-menu fill-width" } };
            content.AppendHtml(dropDownMenu1.RenderStartTag());

            customBbCodesWithNoToolbar.Where(x => x.Name.ToLower() != "attach" && x.Name.ToLower() != "albumimg")
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

                            var item = new TagBuilder("a")
                                           {
                                               Attributes =
                                                   {
                                                       ["class"] = "dropdown-item",
                                                       ["href"] = "#",
                                                       ["onclick"] = onclickJs
                                                   }
                                           };

                            content.AppendHtml(item.RenderStartTag());

                            content.Append(name);

                            content.AppendHtml(item.RenderEndTag());
                        });

            content.AppendHtml(dropDownMenu3.RenderEndTag());
            //

            content.AppendHtml(group9.RenderEndTag());
            //
        }

        content.AppendHtml(toolbar.RenderEndTag());
    }

    /// <summary>
    /// Renders the basic header.
    /// </summary>
    /// <param name="content">The content.</param>
    private void RenderBasicHeader(IHtmlContentBuilder content)
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
    private void RenderFooter(IHtmlContentBuilder content)
    {
        var containerCardFooter =
            new TagBuilder("div") { Attributes = { ["class"] = "card-footer text-body-secondary text-end" } };

        content.AppendHtml(containerCardFooter.RenderStartTag());

        content.Append(this.GetText("COMMON", "CHARACTERS_LEFT"));

        var spanTag = new TagBuilder("span")
                          {
                              Attributes = { ["id"] = "editor-Counter", ["class"] = "badge bg-secondary ms-1" }
                          };

        content.AppendHtml(spanTag);

        content.AppendHtml(containerCardFooter.RenderEndTag());
    }


    private void RenderAlbumsDropDown(IHtmlContentBuilder content)
    {
        var menuTag =
            new TagBuilder("div") { Attributes = { ["class"] = "AlbumsListMenu dropdown-item" } };

        content.AppendHtml(menuTag.RenderStartTag());

        var listBoxTag =
            new TagBuilder("div") { Attributes = { ["class"] = "content", ["id"] = "AlbumsListBox" } };

        content.AppendHtml(listBoxTag.RenderStartTag());

        // Pager
        var pagerTag =
            new TagBuilder("div") { Attributes = {  ["id"] = "AlbumsListPager" } };
        content.AppendHtml(pagerTag);

        // Loading box
        var loaderTag =
            new TagBuilder("div") { Attributes = { ["id"] = "PostAlbumsLoader", ["class"] = "text-center" } };
        content.AppendHtml(loaderTag.RenderStartTag());

        var spanTag =
            new TagBuilder("span");
        content.AppendHtml(spanTag.RenderStartTag());
        content.Append(this.Get<ILocalization>().GetText("COMMON", "LOADING"));
        content.AppendHtml(spanTag.RenderEndTag());

        var divTag =
            new TagBuilder("div"){ Attributes = { ["class"] = "fa-3x" } };
        content.AppendHtml(divTag.RenderStartTag());
        var iconLoadTag =
            new TagBuilder("i") { Attributes = { ["class"] = "fas fa-spinner fa-pulse" } };
        content.AppendHtml(iconLoadTag);
        content.AppendHtml(divTag.RenderEndTag());

        content.AppendHtml(loaderTag.RenderEndTag());

        // PlaceHolder
        var placeHolderTag = new TagBuilder("div")
                                 {
                                     Attributes =
                                         {
                                             ["id"] = "PostAlbumsListPlaceholder",
                                             ["data-notext"] =
                                                 this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS"),
                                             ["style"] = "clear: both;"
                                         }
                                 };
        content.AppendHtml(placeHolderTag.RenderStartTag());

        // List 
        var listTag = new TagBuilder("ul") { Attributes = { ["class"] = "AttachmentList list-group" } };
        content.AppendHtml(listTag);

        content.AppendHtml(placeHolderTag.RenderEndTag());

        content.AppendHtml(listBoxTag.RenderEndTag());

        content.AppendHtml(menuTag.RenderEndTag());
    }

    private void RenderAttachmentsDropDown(IHtmlContentBuilder content)
    {
        var menuTag =
            new TagBuilder("div") { Attributes = { ["class"] = "AttachmentListMenu dropdown-item" } };

        content.AppendHtml(menuTag.RenderStartTag());

        var listBoxTag =
            new TagBuilder("div") { Attributes = { ["class"] = "content", ["id"] = "AttachmentsListBox" } };

        content.AppendHtml(listBoxTag.RenderStartTag());
        
        // Pager
        var pagerTag =
            new TagBuilder("div") { Attributes = { ["id"] = "AttachmentsListPager" } };
        content.AppendHtml(pagerTag);

        // Loading box
        var loaderTag =
            new TagBuilder("div") { Attributes = { ["id"] = "PostAttachmentLoader", ["class"] = "text-center" } };
        content.AppendHtml(loaderTag.RenderStartTag());

        var spanTag =
            new TagBuilder("span");
        content.AppendHtml(spanTag.RenderStartTag());
        content.Append(this.Get<ILocalization>().GetText("COMMON", "LOADING"));
        content.AppendHtml(spanTag.RenderEndTag());

        var divTag =
            new TagBuilder("div") { Attributes = { ["class"] = "fa-3x" } };
        content.AppendHtml(divTag.RenderStartTag());
        var iconLoadTag =
            new TagBuilder("i") { Attributes = { ["class"] = "fas fa-spinner fa-pulse" } };
        content.AppendHtml(iconLoadTag);
        content.AppendHtml(divTag.RenderEndTag());

        content.AppendHtml(loaderTag.RenderEndTag());

        var placeHolderTag = new TagBuilder("div")
                                 {
                                     Attributes =
                                         {
                                             ["id"] = "PostAttachmentListPlaceholder",
                                             ["data-notext"] =
                                                 this.Get<ILocalization>().GetText("ATTACHMENTS", "NO_ATTACHMENTS"),
                                             ["style"] = "clear: both;"
                                         }
                                 };
        content.AppendHtml(placeHolderTag.RenderStartTag());

        // List 
        var listTag = new TagBuilder("ul") { Attributes = { ["class"] = "AttachmentList list-group" } };
        content.AppendHtml(listTag);

        content.AppendHtml(placeHolderTag.RenderEndTag());

        // Upload Button
        var uploadDivTag = new TagBuilder("div")
                                 {
                                     Attributes =
                                         {
                                             ["class"] = "OpenUploadDialog"
                                         }
                                 };
        content.AppendHtml(uploadDivTag.RenderStartTag());

        var uploadButtonTag = new TagBuilder("button")
                                  {
                                      Attributes =
                                          {
                                              ["type"] = "button",
                                              ["class"] = "btn btn-primary btn-sm",
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
    /// The render button.
    /// </summary>
    /// <param name="content">The Content Builder.</param>
    /// <param name="command">The command.</param>
    /// <param name="title">The title.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="id">The identifier.</param>
    private static void RenderButton(
        [NotNull] IHtmlContentBuilder content,
        [NotNull] string command,
        [NotNull] string title,
        [NotNull] string icon,
        [CanBeNull] string id = null)
    {
        var iconTag = new TagBuilder("i") { Attributes = { ["class"] = $"fa fa-{icon} fa-fw" } };

        var button = new TagBuilder("button")
                         {
                             Attributes =
                                 {
                                     ["class"] = "btn btn-primary btn-sm",
                                     ["onclick"] = command,
                                     ["type"] = "button",
                                     ["title"] = title
                                 }
                         };

        if (id.IsSet())
        {
            button.Attributes.Add("id", id);
        }

        content.AppendHtml(button.RenderStartTag());

        content.AppendHtml(iconTag);

        content.AppendHtml(button.RenderEndTag());
    }

    private static TagBuilder CreateToolbarTag()
    {
        return new TagBuilder("div") { Attributes = { ["class"] = "btn-toolbar", ["role"] = "toolbar" } };
    }

    private static TagBuilder CreateBtnGroupTag()
    {
        return new TagBuilder("div")
                   {
                       Attributes = { ["class"] = "btn-group btn-group-sm me-2 mb-2", ["role"] = "group" }
                   };
    }
}