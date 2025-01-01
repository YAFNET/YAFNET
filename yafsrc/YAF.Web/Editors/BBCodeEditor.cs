﻿/* Yet Another Forum.NET
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

namespace YAF.Web.Editors;

using System;
using System.IO;
using System.Web.UI;

using YAF.Configuration;
using YAF.Core.Utilities;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;
using YAF.Web.Controls;

/// <summary>
/// The YAF BBCode editor.
/// </summary>
public class BBCodeEditor : TextEditor
{
    /// <summary>
    ///   The Attachments list menu.
    /// </summary>
    private AttachmentsPopMenu popMenuAttachments;

    /// <summary>
    ///   The Album list menu.
    /// </summary>
    private AlbumListPopMenu popMenuAlbums;

    /// <summary>
    ///   Gets the Description.
    /// </summary>
    public override string Description => "Standard BBCode Editor";

    /// <summary>
    ///   Gets SafeID.
    /// </summary>
    protected string SafeId => this.TextAreaControl.ClientID.Replace("$", "_");

    /// <summary>
    ///   Gets the Module Id.
    /// </summary>
    public override string ModuleId => "YAFEditor";

    /// <summary>
    ///   Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode => true;

    /// <summary>
    /// Gets a value indicating whether [allows uploads].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [allows uploads]; otherwise, <c>false</c>.
    /// </value>
    public override bool AllowsUploads => true;

    /// <summary>
    /// Handles the PreRender event of the Editor control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    override protected void Editor_PreRender(object sender, EventArgs e)
    {
        base.Editor_PreRender(sender, e);

        this.PageBoardContext.PageElements.AddScriptReference("YafEditor", "editor/editor.min.js");

        this.PageBoardContext.PageElements.RegisterJsBlock(
            nameof(JavaScriptBlocks.CreateEditorJs),
            JavaScriptBlocks.CreateEditorJs(
                this.SafeId,
                this.GetText("COMMON", "TT_URL_TITLE"),
                this.GetText("COMMON", "TT_URL_DESC"),
                this.GetText("COMMON", "TT_IMAGE_TITLE"),
                this.GetText("COMMON", "TT_IMAGE_DESC"),
                this.GetText("COMMON", "TT_DESCRIPTION"),
                this.GetText("COMMON", "TT_MEDIA")));

        if (this.UserCanUpload && this.AllowsUploads)
        {
            BoardContext.Current.PageElements.RegisterJsBlock(
                nameof(JavaScriptBlocks.FileAutoUploadLoadJs),
                JavaScriptBlocks.FileAutoUploadLoadJs($"{BoardInfo.ForumClientFileRoot}FileUploader.ashx",
                    "BBCodeEditor"));
        }

        // register custom YafBBCode javascript (if there is any)
        // this call is supposed to be after editor load since it may use
        // JS variables created in editor_load...
        this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType(), this.SafeId);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.TextAreaControl.Attributes.Add("class", "BBCodeEditor form-control");
    }

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
    override protected void Render(HtmlTextWriter writer)
    {
        if (this.PageBoardContext.UploadAccess)
        {
            // add PopMenu Attachments to this mix...
            this.popMenuAttachments = new AttachmentsPopMenu();
            this.Controls.Add(this.popMenuAttachments);
        }

        if (this.PageBoardContext.BoardSettings.EnableAlbum && this.PageBoardContext.NumAlbums > 0)
        {
            // add PopMenu Albums to this mix...
            this.popMenuAlbums = new AlbumListPopMenu();
            this.Controls.Add(this.popMenuAlbums);
        }

        writer.Write("<div class=\"card\">");

        writer.Write("<div class=\"card-header pb-0\">");

        // First toolbar row
        this.RenderFirstToolbar(writer);

        // Second toolbar row
        writer.Write("<div class=\"btn-toolbar\" role=\"toolbar\">");
        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('bold','')", this.GetText("COMMON", "TT_BOLD"), "bold");
        RenderButton(writer, "setStyle('italic','')", this.GetText("COMMON", "TT_ITALIC"), "italic");

        RenderButton(writer, "setStyle('underline','')", this.GetText("COMMON", "TT_UNDERLINE"), "underline");

        writer.Write("</div>");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        // add drop down for optional "extra" codes...
        writer.WriteLine(
            """
            <button type="button" class="btn btn-primary dropdown-toggle" title="{0}"
                                   data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                              <i class="fa fa-font fa-fw"></i> {0}</button>
            """,
            this.GetText("COMMON", "FONT_COLOR"));

        writer.Write("<div class=\"dropdown-menu editorColorMenu\">");

        string[] colors = [
            "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue", "Dark Blue",
            "Indigo", "Violet", "White", "Black"
        ];

        colors.ForEach(
            color =>
            {
                writer.WriteLine(
                    color == "White"
                        ? """<a class="dropdown-item" href="#" onclick="setStyle('color', '{0}');" style="color:{0};background:grey">{1}</a>"""
                        : """<a class="dropdown-item" href="#" onclick="setStyle('color', '{0}');" style="color:{0}">{1}</a>""",
                    color.Replace(" ", string.Empty).ToLower(),
                    color);
            });

        writer.Write("</div>");

        writer.Write("</div>");
        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        // add drop down for optional "extra" codes...
        writer.WriteLine(
            """
            <button type="button" class="btn btn-primary dropdown-toggle" title="{0}"
                                   data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                              <i class="fa fa-font fa-fw"></i> {0}</button>
            """,
            this.GetText("COMMON", "FONT_SIZE"));

        writer.Write("<div class=\"dropdown-menu\">");

        for (var index = 1; index < 9; index++)
        {
            writer.WriteLine(
                """<a class="dropdown-item" href="#" onclick="setStyle('fontsize', {0});">{1}</a>""",
                index,
                index.Equals(5) ? "Default" : index.ToString());
        }

        writer.Write("</div></div>");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('highlight','')", this.GetText("COMMON", "TT_HIGHLIGHT"), "pen-square");

        writer.Write("</div>");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('createlink','')", this.GetText("COMMON", "TT_CREATELINK"), "link");

        RenderButton(writer, "setStyle('quote','')", this.GetText("COMMON", "TT_QUOTE"), "quote-left");

        // add drop down for optional "extra" codes...
        writer.WriteLine(
            """
            <button type="button" class="btn btn-primary dropdown-toggle" title="{0}"
                                   data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                              <i class="fa fa-code fa-fw"></i></button>
            """,
            this.GetText("COMMON", "TT_CODE"));

        var highLightList = new List<Types.Objects.ListItem> {
                                                                 new() { Value = "markup", Name = "Plain Text" },
                                                                 new() { Value = "bash", Name = "Bash(shell)" },
                                                                 new() { Value = "c", Name = "C" },
                                                                 new() { Value = "cpp", Name = "C++" },
                                                                 new() { Value = "csharp", Name = "C#" },
                                                                 new() { Value = "css", Name = "CSS" },
                                                                 new() { Value = "scss", Name = "SCSS" },
                                                                 new() { Value = "git", Name = "Git" },
                                                                 new() { Value = "markup", Name = "HTML" },
                                                                 new() { Value = "java", Name = "Java" },
                                                                 new() { Value = "json", Name = "JSON" },
                                                                 new() { Value = "javascript", Name = "JavaScript" },
                                                                 new() { Value = "python", Name = "Python" },
                                                                 new() { Value = "sql", Name = "SQL" },
                                                                 new() { Value = "markup", Name = "XML" },
                                                                 new() { Value = "vb", Name = "Visual Basic" }
                                                             };

        writer.Write("<div class=\"dropdown-menu\">");

        highLightList.ForEach(
            item => writer.WriteLine(
                """<a class="dropdown-item" href="#" onclick="setStyle('codelang','{0}')">{1}</a>""",
                item.Value,
                item.Name));

        writer.Write("</div>");

        writer.Write("</div>");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('img','')", this.GetText("COMMON", "TT_IMAGE"), "image");

         if (this.Get<BoardSettings>().EnableAlbum && this.PageBoardContext.NumAlbums > 0
                                                      && !this.PageBoardContext.CurrentForumPage.IsAdminPage)
         {
             // add drop down for optional "extra" codes...
             writer.WriteLine(
                 """
                 <button type="button" class="btn btn-primary dropdown-toggle albums-toggle" title="{0}"
                                     data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i class="fa fa-images fa-fw"></i></button>
                 """,
                 this.GetText("COMMON", "ALBUMIMG_CODE"));

             writer.Write("<div class=\"dropdown-menu\">");

             this.popMenuAlbums.RenderControl(writer);

             writer.Write("</div>");

             writer.Write("</div>");
         }

        if (this.UserCanUpload && this.PageBoardContext.UploadAccess)
        {
            writer.Write("</div>");
            writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

            // add drop down for optional "extra" codes...
            writer.WriteLine(
                """
                <button type="button" class="btn btn-primary dropdown-toggle attachments-toggle" title="{0}"
                                       data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                  <i class="fa fa-paperclip fa-fw"></i></button>
                """,
                this.GetText("COMMON", "ATTACH_BBCODE"));

            writer.Write("<div class=\"dropdown-menu\">");

            this.popMenuAttachments.RenderControl(writer);

            writer.Write("</div>");
            writer.Write("</div>");
        }

        writer.Write("</div>");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('unorderedlist','')", this.GetText("COMMON", "TT_LISTUNORDERED"), "list-ul");

        RenderButton(writer, "setStyle('orderedlist','')", this.GetText("COMMON", "TT_LISTORDERED"), "list-ol");

        writer.Write("</div>");
        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('justifyleft','')", this.GetText("COMMON", "TT_ALIGNLEFT"), "align-left");

        RenderButton(
            writer,
            "setStyle('justifycenter','')",
            this.GetText("COMMON", "TT_ALIGNCENTER"),
            "align-center");

        RenderButton(writer, "setStyle('justifyright','')", this.GetText("COMMON", "TT_ALIGNRIGHT"), "align-right");

        writer.Write("</div>");
        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('outdent','')", this.GetText("COMMON", "OUTDENT"), "outdent");

        RenderButton(writer, "setStyle('indent','')", this.GetText("COMMON", "INDENT"), "indent");

        var customBbCode = this.Get<IDataCache>().GetOrSet(
            Constants.Cache.CustomBBCode,
            () => this.GetRepository<Types.Models.BBCode>().GetByBoardId());

        var customBbCodesWithToolbar = customBbCode.Where(code => code.UseToolbar == true).ToList();
        var customBbCodesWithNoToolbar =
            customBbCode.Where(code => code.UseToolbar == false || !code.UseToolbar.HasValue);

        if (customBbCode.Any())
        {
            writer.Write("</div>");
            writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

            RenderButton(writer, "setStyle('media','')", this.GetText("COMMON", "MEDIA"), "photo-film");

            if (customBbCodesWithToolbar.Any())
            {
                customBbCodesWithToolbar.ForEach(
                    row =>
                        {
                            var name = row.Name;

                            var onclickJs = row.OnClickJS.IsSet()
                                                ? row.OnClickJS
                                                : $"setStyle('{row.Name.Trim()}','')";

                            writer.WriteLine(
                                """
                                <button type="button" class="btn btn-primary" onclick="{2}" title="{1}"{3}>
                                              <i class="fab fa-{0} fa-fw"></i></button>
                                """,
                                row.Name.ToLower(),
                                this.Get<IBBCode>().LocalizeCustomBBCodeElement(row.Description.Trim()),
                                onclickJs,
                                name);
                        });
            }

            // add drop down for optional "extra" codes...
            writer.WriteLine(
                """
                <div class="btn-group btn-group-sm" role="group"><button type="button" class="btn btn-primary dropdown-toggle" title="{0}"
                                   data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                              <i class="fa fa-plug fa-fw"></i></button>
                """,
                this.GetText("COMMON", "CUSTOM_BBCODE"));

            writer.Write("<div class=\"dropdown-menu fill-width\">");

            customBbCodesWithNoToolbar.Where(
                    x => x.Name.ToLower() != "attach" && x.Name.ToLower() != "albumimg" && x.Name.ToLower() != "media")
                .ForEach(
                    row =>
                    {
                        var name = row.Name;

                        if (row.Description.IsSet())
                        {
                            // use the description as the option "name"
                            name = this.Get<IBBCode>().LocalizeCustomBBCodeElement(row.Description.Trim());
                        }

                        var onclickJs = row.OnClickJS.IsSet() ? row.OnClickJS : $"setStyle('{row.Name.Trim()}','')";

                        writer.WriteLine(
                            """<a class="dropdown-item" href="#" onclick="{0}">{1}</a>""",
                            onclickJs,
                            name);
                    });

            writer.Write("</div>");
            writer.Write("</div>");
        }

        writer.Write("</div>");

        writer.Write("</div>");

        // end card-header
        writer.Write("</div>");

        writer.Write("<div class=\"card-body\">");

        this.TextAreaControl.RenderControl(writer);

        writer.Write("</div>");

        // Render Footer
        this.RenderFooter(writer);

        writer.Write("</div>");
    }

    private void RenderFooter(TextWriter writer)
    {
        writer.Write("<div class=\"card-footer text-body-secondary text-end\">");

        writer.Write($"{this.GetText("COMMON", "CHARACTERS_LEFT")}<span id=\"editor-Counter\" class=\"badge bg-secondary ms-1\"></span>");

        writer.Write("</div>");
    }

    private void RenderFirstToolbar(TextWriter writer)
    {
        writer.Write("<div class=\"btn-toolbar\" role=\"toolbar\">");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        // Render Cut Button
        RenderButton(writer, "setStyle('cut','')", this.GetText("COMMON", "TT_CUT"), "scissors");

        // Render Copy Button
        RenderButton(writer, "setStyle('copy','')", this.GetText("COMMON", "TT_COPY"), "copy");

        // Render Paste Button
        RenderButton(writer, "setStyle('paste','')", this.GetText("COMMON", "TT_PASTE"), "paste");

        writer.Write("</div>");


        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        // Render Undo Button
        RenderButton(writer, "setStyle('undo','')", this.GetText("COMMON", "TT_UNDO"), "rotate-left", "undo");

        // Render Redo Button
        RenderButton(writer, "setStyle('redo','')", this.GetText("COMMON", "TT_REDO"), "rotate-right", "redo");

        writer.Write("</div>");


        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        // Render Select All Button
        RenderButton(writer, "setStyle('selectAll','')", this.GetText("COMMON", "TT_SELECT_ALL"), "object-group");

        // Render Remove Format Button
        RenderButton(writer, "setStyle('removeFormat','')", this.GetText("COMMON", "TT_REMOVE"), "remove-format");

        writer.Write("</div>");

        writer.Write("</div>");
    }

    /// <summary>
    /// The render button.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="command">The command.</param>
    /// <param name="title">The title.</param>
    /// <param name="icon">The icon.</param>
    /// <param name="id">The identifier.</param>
    private static void RenderButton(
        TextWriter writer,
        string command,
        string title,
        string icon,
        string id = null)
    {
        writer.WriteLine(
            """
            <button type="button" class="btn btn-primary" onclick="{2}" title="{1}"{3}>
                              <i class="fa fa-{0} fa-fw"></i></button>
            """,
            icon,
            title,
            command,
            id.IsSet() ? $"""
                           id="{id}"
                          """ : string.Empty);
    }
}