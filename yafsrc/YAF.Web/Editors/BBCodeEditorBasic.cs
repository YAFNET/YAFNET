/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

using YAF.Types;
using YAF.Types.Extensions;
using YAF.Types.Interfaces;

/// <summary>
/// The YAF BBCode basic editor.
/// </summary>
public class BBCodeEditorBasic : TextEditor
{
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
    public override string ModuleId => "2";

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
    public override bool AllowsUploads => false;

    /// <summary>
    /// Handles the PreRender event of the Editor control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected override void Editor_PreRender(object sender, EventArgs e)
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
                this.GetText("COMMON", "TT_DESCRIPTION")));

        // register custom YafBBCode javascript (if there is any)
        // this call is supposed to be after editor load since it may use
        // JS variables created in editor_load...
        this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType(), this.SafeId);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.TextAreaControl.Attributes.Add("class", "BBCodeEditor form-control");
    }

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
    protected override void Render(HtmlTextWriter writer)
    {
        writer.Write("<div class=\"card\">");

        writer.Write("<div class=\"card-header pb-0\">");

        writer.Write("<div class=\"btn-toolbar\" role=\"toolbar\">");

        //
        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        // Render Undo Button
        RenderButton(writer, "setStyle('undo','')", this.GetText("COMMON", "TT_UNDO"), "rotate-left", "undo");

        // Render Redo Button
        RenderButton(writer, "setStyle('redo','')", this.GetText("COMMON", "TT_REDO"), "rotate-right", "redo");

        writer.Write("</div>");
        //

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('bold','')", this.GetText("COMMON", "TT_BOLD"), "bold");
        RenderButton(writer, "setStyle('italic','')", this.GetText("COMMON", "TT_ITALIC"), "italic");

        RenderButton(writer, "setStyle('underline','')", this.GetText("COMMON", "TT_UNDERLINE"), "underline");

        writer.Write("</div>");

        writer.Write("<div class=\"btn-group btn-group-sm me-2 mb-2\" role =\"group\">");

        RenderButton(writer, "setStyle('createlink','')", this.GetText("COMMON", "TT_CREATELINK"), "link");

        RenderButton(writer, "setStyle('quote','')", this.GetText("COMMON", "TT_QUOTE"), "quote-left");

        RenderButton(writer, "setStyle('img','')", this.GetText("COMMON", "TT_IMAGE"), "image");

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

    private void RenderFooter(HtmlTextWriter writer)
    {
        writer.Write("<div class=\"card-footer text-body-secondary text-end\">");

        writer.Write($"{this.GetText("COMMON", "CHARACTERS_LEFT")}<span id=\"editor-Counter\" class=\"badge bg-secondary ms-1\"></span>");

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
            <button type="button" class="btn btn-primary btn-sm" onclick="{2}" title="{1}"{3}>
                              <i class="fa fa-{0} fa-fw"></i></button>
            """,
            icon,
            title,
            command,
            id.IsSet() ? $@" id=""{id}""" : string.Empty);
    }
}