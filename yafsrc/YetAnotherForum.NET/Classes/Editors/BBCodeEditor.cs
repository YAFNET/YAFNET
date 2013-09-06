/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
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
    using YAF.Core;
    using YAF.Core.BBCode;
    using YAF.Core.Services;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Utils;
    using YAF.Controls;
    using YAF.Types;

    #endregion

    /// <summary>
    /// The bb code editor.
    /// </summary>
    public class BBCodeEditor : TextEditor
    {
        #region Constants and Fields

        /// <summary>
        ///   The BB Code menu.
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
        ///   Gets Description.
        /// </summary>
        [NotNull]
        public override string Description
        {
            get
            {
                return "YAF Standard YafBBCode Editor";
            }
        }

        /// <summary>
        ///   Gets ModuleId.
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

        #endregion

        #region Methods

        /// <summary>
        /// The editor_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void Editor_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            base.Editor_PreRender(sender, e);

            // register custom YafBBCode javascript (if there is any)
            // this call is supposed to be after editor load since it may use
            // JS variables created in editor_load...
            this.Get<IBBCode>().RegisterCustomBBCodePageElements(this.Page, this.GetType(), this.SafeID);
            YafContext.Current.PageElements.RegisterJsResourceInclude("yafjs", "js/yaf.js");
        }

        /// <summary>
        /// The on init.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
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
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            writer.WriteLine(@"<table border=""0"" id=""bbcodeFeatures"">");
            writer.WriteLine("<tr><td>");

            this.RenderButton(
                writer, "bold", "FormatText('bold','')", this.GetText("COMMON", "TT_BOLD"), "yafEditor/bold.gif");
            this.RenderButton(
                writer, "italic", "FormatText('italic','')", this.GetText("COMMON", "TT_ITALIC"), "yafEditor/italic.gif");

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
                writer, "quote", "FormatText('quote','')", this.GetText("COMMON", "TT_QUOTE"), "yafEditor/quote.gif");

            // add drop down for optional "extra" codes...
            writer.WriteLine(
                @"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />"
                    .FormatWith(
                        this._popMenuBBCode.ControlOnClick,
                        this._popMenuBBCode.ControlOnMouseOver,
                        this.GetText("COMMON", "TT_CODE"),
                        this.ClientID + "_bbcode_popMenu",
                        this.GetText("COMMON", "TT_CODELANG"),
                        this.ResolveUrl("yafEditor/code.gif")));

            var highLightList = new List<HighLightList>
                {
                    new HighLightList { BrushAlias = "plain", BrushName = "Plain Text" },
                    new HighLightList { BrushAlias = "as3", BrushName = "ActionScript3" },
                    new HighLightList { BrushAlias = "bash", BrushName = "Bash(shell)" },
                    new HighLightList { BrushAlias = "coldfusion", BrushName = "ColdFusion" },
                    new HighLightList { BrushAlias = "csharp", BrushName = "C#" },
                    new HighLightList { BrushAlias = "cpp", BrushName = "C++" },
                    new HighLightList { BrushAlias = "css", BrushName = "CSS/Html" },
                    new HighLightList { BrushAlias = "delphi", BrushName = "Delphi" },
                    new HighLightList { BrushAlias = "diff", BrushName = "Diff" },
                    new HighLightList { BrushAlias = "erlang", BrushName = "Erlang" },
                    new HighLightList { BrushAlias = "groovy", BrushName = "Groovy" },
                    new HighLightList { BrushAlias = "jscript", BrushName = "JavaScript" },
                    new HighLightList { BrushAlias = "java", BrushName = "Java" },
                    new HighLightList { BrushAlias = "javafx", BrushName = "JavaFX" },
                    new HighLightList { BrushAlias = "perl", BrushName = "Perl" },
                    new HighLightList { BrushAlias = "php", BrushName = "PHP" },
                    new HighLightList { BrushAlias = "powershell", BrushName = "PowerShell" },
                    new HighLightList { BrushAlias = "python", BrushName = "Pyton" },
                    new HighLightList { BrushAlias = "ruby", BrushName = "Ruby" },
                    new HighLightList { BrushAlias = "scala", BrushName = "Scala" },
                    new HighLightList { BrushAlias = "sql", BrushName = "SQL" },
                    new HighLightList { BrushAlias = "vb", BrushName = "Visual Basic" },
                    new HighLightList { BrushAlias = "xml", BrushName = "XML" }
                };

            foreach (HighLightList item in highLightList)
            {
                this._popMenuBBCode.AddClientScriptItem(
                    item.BrushName, "setStyle('codelang','{0}')".FormatWith(item.BrushAlias));
            }

            this.RenderButton(
                writer, "img", "FormatText('img','')", this.GetText("COMMON", "TT_IMAGE"), "yafEditor/image.gif");

            if (this.Get<YafBoardSettings>().EnableAlbum && (this.PageContext.UsrAlbums > 0 && this.PageContext.NumAlbums > 0))
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
                        !string.IsNullOrEmpty(row["Caption"].ToString()) ? row["Caption"].ToString() : row["FileName"].ToString(),
                        "setStyle('AlbumImgId','{0}')".FormatWith(row["ImageID"]),
                        "{0}resource.ashx?image={1}".FormatWith(YafForumInfo.ForumClientFileRoot, row["ImageID"]));
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

            var customBbCode = this.Get<YafDbBroker>().GetCustomBBCode();

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
                    string name = row.Name;

                    if (row.Description.IsSet())
                    {
                        // use the description as the option "name"
                        name = this.Get<IBBCode>().LocalizeCustomBBCodeElement(row.Description.Trim());
                    }

                    string onclickJs = row.OnClickJS.IsSet()
                                           ? row.OnClickJS
                                           : "setStyle('{0}','')".FormatWith(row.Name.Trim());

                    this._popMenuBBCustom.AddClientScriptItem(name, onclickJs);
                }
            }

            // add spell check button.
            writer.WriteLine(
                @"<img src=""{2}"" id=""{0}_spell"" alt=""{1}"" title=""{1}"" onload=""Button_Load(this)"" />".FormatWith(
                        this.SafeID, this.GetText("COMMON", "SPELL"), this.ResolveUrl("yafEditor/spellcheck.gif")));

            writer.WriteLine("	</td></tr>");
            writer.WriteLine("	<tr><td>");

            // TODO: Convert to a control...
            writer.WriteLine(this.GetText("COMMON", "FONT_COLOR"));
            writer.WriteLine(
                "<select onchange=\"if(this.value!='') setStyle('color',this.value); this.value=''\">", this.SafeID);
            writer.WriteLine("<option value=\"\">Default</option>");

            string[] colors = {
                                  "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue",
                                  "Dark Blue", "Indigo", "Violet", "White", "Black"
                              };

            foreach (string color in colors)
            {
                string tValue = color.Replace(" ", string.Empty).ToLower();
                writer.WriteLine("<option style=\"color:{0}\" value=\"{0}\">{1}</option>".FormatWith(tValue, color));
            }

            writer.WriteLine("</select>");

            // TODO: Just convert to a drop down control...
            writer.WriteLine(this.GetText("COMMON", "FONT_SIZE"));
            writer.WriteLine(
                "<select onchange=\"if(this.value!='') setStyle('fontsize',this.value); this.value=''\">", this.SafeID);
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