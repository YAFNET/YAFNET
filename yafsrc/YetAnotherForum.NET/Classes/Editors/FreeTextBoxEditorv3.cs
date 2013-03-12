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
  using YAF.Core;

  /// <summary>
  /// The free text box editorv 3.
  /// </summary>
  public class FreeTextBoxEditorv3 : FreeTextBoxEditor
  {
    #region Properties

    /// <summary>
    ///   Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "Free Text Box v3 (HTML)";
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
        return "6";
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
        @"function insertsmiley(code,img){" + "FTB_API['" + this.SafeID +
        "'].InsertHtml('<img src=\"' + img + '\" alt=\"\" />');" + "}\n");
    }

    #endregion
  }
}