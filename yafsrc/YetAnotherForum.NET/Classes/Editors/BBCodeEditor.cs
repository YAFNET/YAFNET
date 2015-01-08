/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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
    using System.Linq;
    using System.Web.UI;
    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Classes.Editors;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.BBCode;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
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
        private AttachmentsPopMenu _popMenuAttachments;

        /// <summary>
        ///   The Album list menu.
        /// </summary>
        private AlbumListPopMenu _popMenuAlbums;

        /// <summary>
        ///   The BB Code menu.
        /// </summary>
        private PopMenu _popMenuBBCode;

        /// <summary>
        ///   The  BB Custom menu.
        /// </summary>
        private PopMenu _popMenuBBCustom;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the Description.
        /// </summary>
        [NotNull]
        public override string Description
        {
            get
            {
                return "Standard BBCode Editor";
            }
        }

        /// <summary>
        ///   Gets the Module Id.
        /// </summary>
        public override string ModuleId
        {
            get
            {
                // backward compatibility...
                return "1";
            }
        }

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public override bool UsesBBCode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [allows uploads].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allows uploads]; otherwise, <c>false</c>.
        /// </value>
        public override bool AllowsUploads
        {
            get
            {
                return true;
            }
        }

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

            YafContext.Current.PageElements.RegisterJsBlock(
                "CreateYafEditorJs",
                "var {0}=new yafEditor('{0}');\nfunction setStyle(style,option) {{\n{0}.FormatText(style,option);\n}}\nfunction insertAttachment(id,url) {{\n{0}.FormatText('attach', id);\n}}\n"
                    .FormatWith(this.SafeID));

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
            this._textCtl.Attributes.Add("class", "BBCodeEditor");

            // add popmenu BB Custom to this mix...
            this._popMenuBBCustom = new PopMenu();
            this.Controls.Add(this._popMenuBBCustom);

            // add popmenu BB Code to this mix...
            this._popMenuBBCode = new PopMenu();
            this.Controls.Add(this._popMenuBBCode);

            // add popmenu Albums to this mix...
            this._popMenuAlbums = new AlbumListPopMenu();
            this.Controls.Add(this._popMenuAlbums);
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
                this._popMenuAttachments = new AttachmentsPopMenu();
                this.Controls.Add(this._popMenuAttachments);
            }

            writer.WriteLine(@"<table border=""0"" id=""bbcodeFeatures"">");
            writer.WriteLine("<tr><td>");

            this.RenderButton(
                writer,
                "bold",
                "FormatText('bold','')",
                this.GetText("COMMON", "TT_BOLD"),
                "yafEditor/bold.gif");
            this.RenderButton(
                writer,
                "italic",
                "FormatText('italic','')",
                this.GetText("COMMON", "TT_ITALIC"),
                "yafEditor/italic.gif");

            this.RenderButton(
                writer,
                "underline",
                "FormatText('underline','')",
                this.GetText("COMMON", "TT_UNDERLINE"),
                "yafEditor/underline.gif");

            writer.WriteLine("&nbsp;");

            this.RenderButton(
                writer,
                "highlight",
                "FormatText('highlight','')",
                this.GetText("COMMON", "TT_HIGHLIGHT"),
                "yafEditor/highlight.gif");

            this.RenderButton(
                writer,
                "quote",
                "FormatText('quote','')",
                this.GetText("COMMON", "TT_QUOTE"),
                "yafEditor/quote.gif");

            // add drop down for optional "extra" codes...
            writer.WriteLine(
                @"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />"
                    .FormatWith(
                        this._popMenuBBCode.ControlOnClick,
                        this._popMenuBBCode.ControlOnMouseOver,
                        this.GetText("COMMON", "TT_CODE"),
                        "{0}_bbcode_popMenu".FormatWith(this.ClientID),
                        this.GetText("COMMON", "TT_CODELANG"),
                        this.ResolveUrl("yafEditor/code.gif")));

            var highLightList = new List<HighLightList>
                                    {
                                        new HighLightList
                                            {
                                                BrushAlias = "markup",
                                                BrushName = "Plain Text"
                                            },
                                        new HighLightList
                                            {
                                                BrushAlias = "markup",
                                                BrushName = "HTML"
                                            },
                                        new HighLightList { BrushAlias = "css", BrushName = "CSS" },
                                        new HighLightList
                                            {
                                                BrushAlias = "javascript",
                                                BrushName = "JavaScript"
                                            },
                                        new HighLightList { BrushAlias = "c", BrushName = "C" },
                                        new HighLightList { BrushAlias = "cpp", BrushName = "C++" },
                                        new HighLightList { BrushAlias = "csharp", BrushName = "C#" },
                                        new HighLightList { BrushAlias = "git", BrushName = "Git" },
                                        new HighLightList { BrushAlias = "sql", BrushName = "SQL" },
                                        new HighLightList
                                            {
                                                BrushAlias = "markup",
                                                BrushName = "XML"
                                            },
                                    };

            foreach (HighLightList item in highLightList)
            {
                this._popMenuBBCode.AddClientScriptItem(
                    item.BrushName,
                    "setStyle('codelang','{0}')".FormatWith(item.BrushAlias));
            }

            this.RenderButton(
                writer,
                "img",
                "FormatText('img','')",
                this.GetText("COMMON", "TT_IMAGE"),
                "yafEditor/image.gif");

            if (this.Get<YafBoardSettings>().EnableAlbum
                && (this.PageContext.UsrAlbums > 0 && this.PageContext.NumAlbums > 0))
            {
                var albumImageList = LegacyDb.album_images_by_user(this.PageContext.PageUserID);

                writer.WriteLine(
                    @"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />"
                        .FormatWith(
                            this._popMenuAlbums.ControlOnClick,
                            this._popMenuAlbums.ControlOnMouseOver,
                            this.GetText("COMMON", "ALBUMIMG_BBCODE"),
                            "{0}_albums_popMenu".FormatWith(this.ClientID),
                            this.GetText("COMMON", "ALBUMIMG_BBCODE"),
                            this.ResolveUrl("yafEditor/albums.gif")));

                foreach (DataRow row in albumImageList.Rows)
                {
                    this._popMenuAlbums.AddClientScriptItem(
                        row["Caption"].ToString().IsSet()
                            ? row["Caption"].ToString()
                            : row["FileName"].ToString(),
                        "setStyle('AlbumImgId','{0}')".FormatWith(row["ImageID"]),
                        "{0}resource.ashx?image={1}".FormatWith(YafForumInfo.ForumClientFileRoot, row["ImageID"]));
                }
            }

            if (this.UserCanUpload)
            {
                var attachments = this.GetRepository<Attachment>()
                    .ListTyped(userID: this.PageContext.PageUserID, pageSize: 10000);

                writer.WriteLine(
                    @"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />"
                        .FormatWith(
                            this._popMenuAttachments.ControlOnClick,
                            this._popMenuAttachments.ControlOnMouseOver,
                            this.GetText("COMMON", "ATTACH_BBCODE"),
                            "{0}_attachments_popMenu".FormatWith(this.ClientID),
                            this.GetText("COMMON", "ATTACH_BBCODE"),
                            this.ResolveUrl("yafEditor/attach.png")));

                foreach (var attachment in attachments)
                {
                    var url = attachment.FileName.IsImageName()
                                  ? "{0}resource.ashx?i={1}&editor=true".FormatWith(
                                      YafForumInfo.ForumClientFileRoot,
                                      attachment.ID)
                                  : "{0}Images/document.png".FormatWith(YafForumInfo.ForumClientFileRoot);

                    this._popMenuAttachments.AddClientScriptItem(
                        attachment.FileName,
                        "insertAttachment('{0}', '{1}')".FormatWith(attachment.ID, url),
                        attachment.FileName.IsImageName()
                            ? "{0}resource.ashx?i={1}&editor=true".FormatWith(YafForumInfo.ForumClientFileRoot, attachment.ID)
                            : "{0}Images/document.png".FormatWith(YafForumInfo.ForumClientFileRoot));
                }
            }

            this.RenderButton(
                writer,
                "createlink",
                "FormatText('createlink','')",
                this.GetText("COMMON", "TT_CREATELINK"),
                "yafEditor/link.gif");

            writer.WriteLine("&nbsp;");

            this.RenderButton(
                writer,
                "unorderedlist",
                "FormatText('unorderedlist','')",
                this.GetText("COMMON", "TT_LISTUNORDERED"),
                "yafEditor/unorderedlist.gif");
            this.RenderButton(
                writer,
                "orderedlist",
                "FormatText('orderedlist','')",
                this.GetText("COMMON", "TT_LISTORDERED"),
                "yafEditor/orderedlist.gif");

            writer.WriteLine("&nbsp;");

            this.RenderButton(
                writer,
                "justifyleft",
                "FormatText('justifyleft','')",
                this.GetText("COMMON", "TT_ALIGNLEFT"),
                "yafEditor/justifyleft.gif");
            this.RenderButton(
                writer,
                "justifycenter",
                "FormatText('justifycenter','')",
                this.GetText("COMMON", "TT_ALIGNCENTER"),
                "yafEditor/justifycenter.gif");
            this.RenderButton(
                writer,
                "justifyright",
                "FormatText('justifyright','')",
                this.GetText("COMMON", "TT_ALIGNRIGHT"),
                "yafEditor/justifyright.gif");

            writer.WriteLine("&nbsp;");

            this.RenderButton(
                writer,
                "outdent",
                "FormatText('outdent','')",
                this.GetText("COMMON", "OUTDENT"),
                "yafEditor/outdent.gif");

            this.RenderButton(
                writer,
                "indent",
                "FormatText('indent','')",
                this.GetText("COMMON", "INDENT"),
                "yafEditor/indent.gif");

            var customBbCode = this.Get<YafDbBroker>().GetCustomBBCode().ToList();

            if (customBbCode.Any())
            {
                writer.WriteLine("&nbsp;");

                // add drop down for optional "extra" codes...
                writer.WriteLine(
                    @"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />"
                        .FormatWith(
                            this._popMenuBBCustom.ControlOnClick,
                            this._popMenuBBCustom.ControlOnMouseOver,
                            this.GetText("COMMON", "CUSTOM_BBCODE"),
                            this.ClientID + "_bbcustom_popMenu",
                            this.GetText("COMMON", "TT_CUSTOMBBCODE"),
                            this.ResolveUrl("yafEditor/bbcode.gif")));

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

                    this._popMenuBBCustom.AddClientScriptItem(name, onclickJs);
                }
            }

            writer.WriteLine("	</td></tr>");
            writer.WriteLine("	<tr><td>");

            // TODO: Convert to a control...
            writer.WriteLine(this.GetText("COMMON", "FONT_COLOR"));
            writer.WriteLine(
                "<select onchange=\"if(this.value!='') setStyle('color',this.value); this.value=''\">");
            writer.WriteLine("<option value=\"\">Default</option>");

            string[] colors =
                {
                    "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue",
                    "Dark Blue", "Indigo", "Violet", "White", "Black"
                };

            foreach (var color in colors)
            {
                var tValue = color.Replace(" ", string.Empty).ToLower();
                writer.WriteLine("<option style=\"color:{0}\" value=\"{0}\">{1}</option>".FormatWith(tValue, color));
            }

            writer.WriteLine("</select>");

            // TODO: Just convert to a drop down control...
            writer.WriteLine(this.GetText("COMMON", "FONT_SIZE"));
            writer.WriteLine(
                "<select onchange=\"if(this.value!='') setStyle('fontsize',this.value); this.value=''\">");
            writer.WriteLine("<option value=\"1\">1</option>");
            writer.WriteLine("<option value=\"2\">2</option>");
            writer.WriteLine("<option value=\"3\">3</option>");
            writer.WriteLine("<option value=\"4\">4</option>");
            writer.WriteLine("<option selected=\"selected\" value=\"5\">Default</option>");
            writer.WriteLine("<option value=\"6\">6</option>");
            writer.WriteLine("<option value=\"7\">7</option>");
            writer.WriteLine("<option value=\"8\">8</option>");
            writer.WriteLine("<option value=\"9\">9</option>");
            writer.WriteLine("</select>");

            writer.WriteLine("</td></tr>");
            writer.WriteLine("</table>");

            this._textCtl.RenderControl(writer);

            this._popMenuBBCustom.RenderControl(writer);
            this._popMenuBBCode.RenderControl(writer);
            this._popMenuAlbums.RenderControl(writer);

            if (this.UserCanUpload)
            {
                this._popMenuAttachments.RenderControl(writer);
            }
        }

        /// <summary>
        /// The render button.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="cmd">
        /// The cmd.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        private void RenderButton(
            [NotNull] HtmlTextWriter writer,
            [NotNull] string id,
            [NotNull] string cmd,
            [NotNull] string title,
            [NotNull] string image)
        {
            // writer.WriteLine("		<td><img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"{1}.{3}\"></td><td>&nbsp;</td>",ResolveUrl(image),SafeID,title,cmd,id);
            writer.WriteLine(
                @"<img id=""{1}_{4}"" onload=""Button_Load(this)"" src=""{0}"" width=""21"" height=""20"" alt=""{2}"" title=""{2}"" onclick=""setStyle('{4}','')"" />",
                this.ResolveUrl(image),
                this.SafeID,
                title,
                cmd,
                id);
        }

        #endregion
    }
}