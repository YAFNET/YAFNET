/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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
namespace YAF.Editors
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Classes.Editors;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.BBCode;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The (old) YAF BBCode editor.
    /// </summary>
    public class BBCodeEditor : TextEditor
    {
        #region Constants and Fields

        /// <summary>
        ///   The Attachments list menu.
        /// </summary>
        private AttachmentsPopMenu popMenuAttachments;

        /// <summary>
        ///   The Album list menu.
        /// </summary>
        private AlbumListPopMenu popMenuAlbums;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the Description.
        /// </summary>
        [NotNull]
        public override string Description => "Standard BBCode Editor";

        /// <summary>
        ///   Gets the Module Id.
        /// </summary>
        public override string ModuleId => "1";

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

        #endregion

        #region Methods

        /// <summary>
        /// Handles the PreRender event of the Editor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void Editor_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            base.Editor_PreRender(sender, e);

            YafContext.Current.PageElements.RegisterJsInclude(
                "YafEditorJs",
#if DEBUG
                this.ResolveUrl("yafEditor/yafEditor.js"));
#else
                this.ResolveUrl("yafEditor/yafEditor.min.js"));
#endif

            YafContext.Current.PageElements.RegisterJsBlock("CreateYafEditorJs", @"var {0}=new yafEditor('{0}');
                  function setStyle(style,option) {{
                           {0}.FormatText(style,option);
                  }}
                  function insertAttachment(id,url) {{
                           {0}.FormatText('attach', id);
                  }}".FormatWith(this.SafeID));

            // register custom YafBBCode javascript (if there is any)
            // this call is supposed to be after editor load since it may use
            // JS variables created in editor_load...
            this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType(), this.SafeID);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);
            this._textCtl.Attributes.Add("class", "BBCodeEditor form-control");

            // add popmenu Albums to this mix...
            this.popMenuAlbums = new AlbumListPopMenu();
            this.Controls.Add(this.popMenuAlbums);
        }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            if (this.UserCanUpload)
            {
                // add popmenu Attachments to this mix...
                this.popMenuAttachments = new AttachmentsPopMenu();
                this.Controls.Add(this.popMenuAttachments);
            }

            writer.Write("<div class=\"btn-toolbar\" role=\"toolbar\">");
            writer.Write("<div class=\"btn-group mr-2\" role =\"group\">");

            RenderButton(writer, "setStyle('bold','')", this.GetText("COMMON", "TT_BOLD"), "bold");
            RenderButton(writer, "setStyle('italic','')", this.GetText("COMMON", "TT_ITALIC"), "italic");

            RenderButton(writer, "setStyle('underline','')", this.GetText("COMMON", "TT_UNDERLINE"), "underline");

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group mr-2\" role =\"group\">");

            RenderButton(
                writer,
                "setStyle('highlight','')",
                this.GetText("COMMON", "TT_HIGHLIGHT"),
                "pen-square");

            writer.Write("</div>");

            writer.Write("<div class=\"btn-group\" role =\"group\">");

            if (!this.Get<HttpRequestBase>().Browser.IsMobileDevice)
            {
                RenderButton(writer, "toggleEmojiPicker()", this.GetText("COMMON", "TT_QUOTE"), "smile", "emoji");
            }

            RenderButton(writer, "setStyle('quote','')", this.GetText("COMMON", "TT_QUOTE"), "quote-left");

            // add drop down for optional "extra" codes...
            writer.WriteLine(@"<button type=""button"" class=""btn btn-secondary btn-sm dropdown-toggle"" title=""{0}""
                       data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                  <i class=""fa fa-code fa-fw""></i></button>".FormatWith(this.GetText("COMMON", "TT_CODE")));

            var highLightList = new List<HighLightList>
                                    {
                                        new HighLightList
                                            {
                                                BrushAlias = "markup",
                                                BrushName = "Plain Text"
                                            },
                                        new HighLightList
                                            {
                                                BrushAlias = "bash",
                                                BrushName = "Bash(shell)"
                                            },
                                        new HighLightList { BrushAlias = "c", BrushName = "C" },
                                        new HighLightList { BrushAlias = "cpp", BrushName = "C++" },
                                        new HighLightList
                                            {
                                                BrushAlias = "csharp",
                                                BrushName = "C#"
                                            },
                                        new HighLightList { BrushAlias = "css", BrushName = "CSS" },
                                        new HighLightList { BrushAlias = "git", BrushName = "Git" },
                                        new HighLightList
                                            {
                                                BrushAlias = "markup",
                                                BrushName = "HTML"
                                            },
                                        new HighLightList
                                            {
                                                BrushAlias = "java",
                                                BrushName = "Java"
                                            },
                                        new HighLightList
                                            {
                                                BrushAlias = "javascript",
                                                BrushName = "JavaScript"
                                            },
                                        new HighLightList
                                            {
                                                BrushAlias = "python",
                                                BrushName = "Python"
                                            },
                                        new HighLightList { BrushAlias = "sql", BrushName = "SQL" },
                                        new HighLightList
                                            {
                                                BrushAlias = "markup",
                                                BrushName = "XML"
                                            },
                                        new HighLightList
                                            {
                                                BrushAlias = "vb",
                                                BrushName = "Visual Basic"
                                            }
                                    };

            writer.Write("<div class=\"dropdown-menu\">");

            foreach (var item in highLightList)
            {
                writer.WriteLine(
                    @"<a class=""dropdown-item"" href=""#"" onclick=""setStyle('codelang','{0}')"">{1}</a>",
                    item.BrushAlias,
                    item.BrushName);
            }

            writer.Write("</div>");
            writer.Write("</div>");

            writer.Write("<div class=\"btn-group\" role =\"group\">");

            RenderButton(writer, "setStyle('img','')", this.GetText("COMMON", "TT_IMAGE"), "image");

            if (this.Get<YafBoardSettings>().EnableAlbum
                && (this.PageContext.UsrAlbums > 0 && this.PageContext.NumAlbums > 0) && !this.PageContext.CurrentForumPage.IsAdminPage)
            {
                // add drop down for optional "extra" codes...
                writer.WriteLine(@"<button type=""button"" class=""btn btn-secondary btn-sm dropdown-toggle albums-toggle"" title=""{0}""
                       data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                  <i class=""fa fa-image fa-fw""></i></button>".FormatWith(this.GetText("COMMON", "ALBUMIMG_CODE")));

                writer.Write("<div class=\"dropdown-menu\">");

                this.popMenuAlbums.RenderControl(writer);

                writer.Write("</div>");
            }

            if (this.UserCanUpload)
            {
                writer.Write("</div>");
                writer.Write("<div class=\"btn-group\" role =\"group\">");

                // add drop down for optional "extra" codes...
                writer.WriteLine(@"<button type=""button"" class=""btn btn-secondary btn-sm dropdown-toggle attachments-toggle"" title=""{0}""
                       data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                  <i class=""fa fa-paperclip fa-fw""></i></button>".FormatWith(this.GetText("COMMON", "ATTACH_BBCODE")));

                writer.Write("<div class=\"dropdown-menu\">");

                this.popMenuAttachments.RenderControl(writer);

                writer.Write("</div>");
                writer.Write("</div>");
            }

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group mr-2\" role =\"group\">");

            RenderButton(writer, "setStyle('createlink','')", this.GetText("COMMON", "TT_CREATELINK"), "link");

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group  mr-2\" role =\"group\">");

            RenderButton(
                writer,
                "setStyle('unorderedlist','')",
                this.GetText("COMMON", "TT_LISTUNORDERED"),
                "list-ul");

            RenderButton(writer, "setStyle('orderedlist','')", this.GetText("COMMON", "TT_LISTORDERED"), "list-ol");

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group  mr-2\" role =\"group\">");

            RenderButton(writer, "setStyle('justifyleft','')", this.GetText("COMMON", "TT_ALIGNLEFT"), "align-left");

            RenderButton(
                writer,
                "setStyle('justifycenter','')",
                this.GetText("COMMON", "TT_ALIGNCENTER"),
                "align-center");

            RenderButton(
                writer,
                "setStyle('justifyright','')",
                this.GetText("COMMON", "TT_ALIGNRIGHT"),
                "align-right");

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group  mr-2\" role =\"group\">");

            RenderButton(writer, "setStyle('outdent','')", this.GetText("COMMON", "OUTDENT"), "outdent");

            RenderButton(writer, "setStyle('indent','')", this.GetText("COMMON", "INDENT"), "indent");

            var customBbCode = this.Get<YafDbBroker>().GetCustomBBCode().ToList();

            if (customBbCode.Any())
            {
                writer.Write("</div>");
                writer.Write("<div class=\"btn-group\" role =\"group\">");

                // add drop down for optional "extra" codes...
                writer.WriteLine(@"<div class=""btn-group"" role=""group""><button type=""button"" class=""btn btn-secondary btn-sm dropdown-toggle"" title=""{0}""
                       data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                  <i class=""fa fa-plug fa-fw""></i></button>".FormatWith(this.GetText("COMMON", "CUSTOM_BBCODE")));

                writer.Write("<div class=\"dropdown-menu fill-width\">");

                foreach (var row in customBbCode)
                {
                    var name = row.Name;

                    if (row.Description.IsSet())
                    {
                        // use the description as the option "name"
                        name = this.Get<IBBCode>().LocalizeCustomBBCodeElement(row.Description.Trim());
                    }

                    var onclickJs = row.OnClickJS.IsSet()
                                        ? row.OnClickJS
                                        : "setStyle('{0}','')".FormatWith(row.Name.Trim());

                    writer.WriteLine(@"<a class=""dropdown-item"" href=""#"" onclick=""{0}"">{1}</a>", onclickJs, name);
                }

                writer.Write("</div>");
                writer.Write("</div>");
            }

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group\" role =\"group\">");

            // add drop down for optional "extra" codes...
            writer.WriteLine(@"<button type=""button"" class=""btn btn-secondary btn-sm dropdown-toggle"" title=""{0}""
                       data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                  <i class=""fa fa-font fa-fw""></i> {0}</button>".FormatWith(this.GetText("COMMON", "FONT_COLOR")));

            writer.Write("<div class=\"dropdown-menu editorColorMenu\">");

            string[] colors =
                {
                    "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue",
                    "Dark Blue", "Indigo", "Violet", "White", "Black"
                };

            foreach (var color in colors)
            {
                writer.WriteLine(
                    @"<a class=""dropdown-item"" href=""#"" onclick=""setStyle('color', '{0}');"" style=""color:{0}"">{1}</a>",
                    color.Replace(" ", string.Empty).ToLower(),
                    color);
            }

            writer.Write("</div>");

            writer.Write("</div>");
            writer.Write("<div class=\"btn-group\" role =\"group\">");

            // add drop down for optional "extra" codes...
            writer.WriteLine(@"<button type=""button"" class=""btn btn-secondary btn-sm dropdown-toggle"" title=""{0}""
                       data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                  <i class=""fa fa-font fa-fw""></i> {0}</button>".FormatWith(this.GetText("COMMON", "FONT_SIZE")));

            writer.Write("<div class=\"dropdown-menu\">");

            for (var index = 1; index < 9; index++)
            {
                writer.WriteLine(
                    @"<a class=""dropdown-item"" href=""#"" onclick=""setStyle('fontsize', {0});"">{1}</a>",
                    index,
                    index.Equals(5) ? "Default" : index.ToString());
            }

            writer.Write("</div>");

            writer.Write("</div></div>");

            this._textCtl.RenderControl(writer);
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
            [NotNull] TextWriter writer,
            [NotNull] string command,
            [NotNull] string title,
            [NotNull] string icon,
            [CanBeNull] string id = null)
        {
            writer.WriteLine(@"<button type=""button"" class=""btn btn-secondary btn-sm"" onload=""Button_Load(this)"" onclick=""{2}"" title=""{1}""{3}>
                  <i class=""fa fa-{0} fa-fw""></i></button>",
                icon,
                title,
                command,
                id.IsSet() ? @" id=""{0}""".FormatWith(id) : string.Empty);
        }

        #endregion
    }
}