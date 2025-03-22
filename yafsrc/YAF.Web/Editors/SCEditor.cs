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

namespace YAF.Web.Editors;

using System;
using System.Web.UI;

using YAF.Configuration;
using YAF.Core.Utilities;
using YAF.Types.Interfaces;

/// <summary>
/// The SCEditor.
/// </summary>
public class SCEditor : TextEditor
{
    /// <summary>
    ///   Gets the Description.
    /// </summary>
    public override string Description => "SCEditor BBCode Editor";

    /// <summary>
    ///   Gets SafeID.
    /// </summary>
    protected string SafeId => this.TextAreaControl.ClientID.Replace("$", "_");

    /// <summary>
    ///   Gets the Module Id.
    /// </summary>
    public override string ModuleId => "SCEditor";

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

        this.PageBoardContext.PageElements.AddScriptReference("SCEditor", "sceditor/sceditor.comb.min.js");

        this.PageBoardContext.PageElements.RegisterCssIncludeContent("sceditor.min.css");

        var language = BoardContext.Current.PageUser.Culture.IsSet()
            ? BoardContext.Current.PageUser.Culture.Substring(0, 2)
            : this.PageBoardContext.BoardSettings.Culture.Substring(0, 2);

        if (ValidationHelper.IsNumeric(language))
        {
            language = this.PageBoardContext.BoardSettings.Culture;
        }

        if (language != "en")
        {
            this.PageBoardContext.PageElements.AddScriptReference("SCEditorLanguage", $"sceditor/languages/{language}.js");
        }

        var albums = string.Empty;
        var attachments = string.Empty;

        if (this.PageBoardContext.BoardSettings.EnableAlbum && this.PageBoardContext.NumAlbums > 0)
        {
            albums = ",albums";
        }

        if (this.PageBoardContext.UploadAccess)
        {
            attachments = "|attachments";
        }

        var toolbar =
            $"bold,italic,underline,strike|font,size,color|mark|email,link,unlink,quote,code|image{albums}{attachments}|note|bulletlist,orderedlist|left,center,right|cut,copy,pastetext,removeformat|undo,redo|youtube,vimeo,instagram,facebook,media|extensions|source|reply";

        var dragDropJs = string.Empty;

        if (this.UserCanUpload && this.AllowsUploads)
        {
            dragDropJs = JavaScriptBlocks.SCEditorDragDropJs($"{BoardInfo.ForumClientFileRoot}FileUploader.ashx");
        }

        this.PageBoardContext.PageElements.RegisterJsBlock(
            nameof(JavaScriptBlocks.CreateSCEditorJs),
            JavaScriptBlocks.CreateSCEditorJs(
                this.SafeId,
                this.MaxCharacters,
                language,
                toolbar,
                BoardInfo.ForumClientFileRoot,
                $"'{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")}', '{BoardInfo.GetURLToContent("forum.min.css")}'",
                $"{BoardInfo.ForumClientFileRoot}resource.ashx?bbcodelist=json",
                dragDropJs
            ));

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
        this.TextAreaControl.RenderControl(writer);
    }
}