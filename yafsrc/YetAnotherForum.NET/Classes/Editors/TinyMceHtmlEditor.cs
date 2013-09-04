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
    using YAF.Classes;
    using YAF.Core;
  using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
  /// The TinyMCE html editor.
  /// </summary>
  public class TinyMceHtmlEditor : TinyMceEditor
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
        return "TinyMCE (HTML)";
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
        return "7";
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
    /// The register Smiley script.
    /// </summary>
    protected override void RegisterSmilieyScript()
    {
      YafContext.Current.PageElements.RegisterJsBlock(
          "InsertSmileyJs",
          "function insertsmiley(code,img) {\n" +
          "	tinyMCE.execCommand('mceInsertContent',false,'<img src=\"' + img + '\" alt=\"\" />');\n" + "}\n");
    }

    /// <summary>
    /// The register tiny mce custom js.
    /// </summary>
    protected override void RegisterTinyMceCustomJS()
    {
        YafContext.Current.PageElements.RegisterJsBlock(
        "editorlang",
        @"var editorLanguage = ""{0}"";".FormatWith(YafContext.Current.CultureUser.IsSet() ? YafContext.Current.CultureUser.Substring(0, 2) : this.Get<YafBoardSettings>().Culture.Substring(0, 2)));

      YafContext.Current.PageElements.RegisterJsInclude("tinymceinit", this.ResolveUrl("tiny_mce/tiny_mce_init.js"));
    }

    #endregion
  }
}