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

using YAF.Core.Services;

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

    [HtmlAttributeName("editor-mode")]
    public EditorMode EditorMode { get; set; }

    public bool UsersCanUpload { get; set; }

    public int MaxCharacters { get; set; }

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
        var container = new TagBuilder("div") { Attributes = { ["class"] = "EditorDiv" } };

        output.PreElement.SetHtmlContent(container.RenderStartTag());

        var toolbar = this.EditorMode == EditorMode.Basic
                          ? this.Get<BoardSettings>().EditorToolbarBasic
                          : this.Get<BoardSettings>().EditorToolbarFull;

        if (!(this.Get<BoardSettings>().EnableAlbum && BoardContext.Current.NumAlbums > 0))
        {
            // remove albums
            toolbar = toolbar.Replace(", \"albumsbrowser\"", string.Empty);
        }

        if (!BoardContext.Current.UploadAccess && !this.UsersCanUpload)
        {
            // remove attachments
            toolbar = toolbar.Replace(", \"attachments\"", string.Empty);
        }

        if (this.UsersCanUpload && this.EditorMode == EditorMode.Standard)
        {
            BoardContext.Current.InlineElements.InsertJsBlock(
                "autoUpload",
                JavaScriptBlocks.FileAutoUploadLoadJs(
                    this.Get<BoardSettings>().AllowedFileExtensions.Replace(",", "|"),
                    this.Get<BoardSettings>().MaxFileSize,
                    this.Get<IUrlHelper>().Action("Upload", "FileUpload"),
                    BoardContext.Current.BoardSettings.ImageAttachmentResizeWidth,
                    BoardContext.Current.BoardSettings.ImageAttachmentResizeHeight,
                    this.AspFor.Name));
        }

        var language = BoardContext.Current.PageUser.Culture.IsSet()
                           ? BoardContext.Current.PageUser.Culture[..2]
                           : this.Get<BoardSettings>().Culture[..2];

        if (ValidationHelper.IsNumeric(language))
        {
            language = this.Get<BoardSettings>().Culture;
        }

        if (this.EditorMode == EditorMode.Basic)
        {
            BoardContext.Current.InlineElements.InsertJsBlock(
                nameof(JavaScriptBlocks.CKEditorBasicLoadJs),
                JavaScriptBlocks.CKEditorBasicLoadJs(
                    this.AspFor.Name.Replace(".", "_"),
                    language,
                    this.MaxCharacters,
                    this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css"),
                    this.Get<BoardInfo>().GetUrlToCss("forum.min.css"),
                    toolbar));
        }
        else if (this.EditorMode == EditorMode.Standard)
        {
            BoardContext.Current.InlineElements.InsertJsBlock(
                nameof(JavaScriptBlocks.CKEditorLoadJs),
                JavaScriptBlocks.CKEditorLoadJs(
                    this.AspFor.Name.Replace(".", "_"),
                    language,
                    this.MaxCharacters,
                    this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css"),
                    this.Get<BoardInfo>().GetUrlToCss("forum.min.css"),
                    toolbar,
                    BoardContext.Current.UploadAccess));
        }
        else if (this.EditorMode == EditorMode.Sql)
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
                nameof(JavaScriptBlocks.CKEditorSqlLoadJs),
                JavaScriptBlocks.CKEditorSqlLoadJs(
                    this.AspFor.Name.Replace(".", "_"),
                    language,
                    this.MaxCharacters,
                    this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css"),
                    this.Get<BoardInfo>().GetUrlToCss("forum.min.css"),
                    mime));
        }

        output.PostElement.SetHtmlContent(container.RenderEndTag());

        // register custom BBCode javascript (if there is any)
        // this call is supposed to be after editor load since it may use
        // JS variables created in editor_load...
        this.Get<IBBCodeService>().RegisterCustomBBCodeInlineElements(this.AspFor.Name);
    }
}