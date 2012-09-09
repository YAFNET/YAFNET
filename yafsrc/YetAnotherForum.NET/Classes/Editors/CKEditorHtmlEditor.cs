/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    /// <summary>
    /// The tiny mce html editor.
    /// </summary>
    public class CKEditorHtmlEditor : CKEditor
    {
        #region Properties

        /// <summary>
        ///   Gets Description.
        /// </summary>
        [NotNull]
        public override string Description
        {
            get
            {
                return "CKEditor (HTML)";
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
                return this.Description.GetHashCode().ToString();
            }
        }

        /// <summary>
        ///   Gets a value indicating whether UsesBBCode.
        /// </summary>
        public override bool UsesBBCode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether UsesHTML.
        /// </summary>
        public override bool UsesHTML
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The register smiliey script.
        /// </summary>
        protected override void RegisterSmilieyScript()
        {
            YafContext.Current.PageElements.RegisterJsBlock(
                "InsertSmileyJs",
                "function insertsmiley(code,img) {{\n var ckEditor = CKEDITOR.instances.{0};\nif ( ckEditor.mode == 'wysiwyg' ) {{\nckEditor.insertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }}\nelse alert( 'You must be on WYSIWYG mode!' );\n}}\n".FormatWith(this._textCtl.ClientID));
        }

        /// <summary>
        /// The register ckeditor custom js.
        /// </summary>
        protected override void RegisterCKEditorCustomJS()
        {
            YafContext.Current.PageElements.RegisterJsBlock(
        "teditorlang",
        @"var editorLanguage = ""{0}"";".FormatWith(YafContext.Current.CultureUser.IsSet() ? YafContext.Current.CultureUser.Substring(0, 2) : this.Get<YafBoardSettings>().Culture.Substring(0, 2)));

            YafContext.Current.PageElements.RegisterJsInclude("ckeditorinit", this.ResolveUrl("ckeditor/ckeditor_init.js"));
        }

        #endregion
    }
}