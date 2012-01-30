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
  #region Using

  using System;
  using System.Reflection;
  using System.Web.UI.WebControls;

  using YAF.Core;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The free text box editor.
  /// </summary>
  public class FreeTextBoxEditor : RichClassEditor
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "FreeTextBoxEditor" /> class.
    /// </summary>
    public FreeTextBoxEditor()
      : base("FreeTextBoxControls.FreeTextBox,FreeTextBox")
    {
      this.InitEditorObject();
    }

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
        return "Free Text Box v2 (HTML)";
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
        return "3";
      }
    }

    /// <summary>
    ///   Gets or sets Text.
    /// </summary>
    [NotNull]
    public override string Text
    {
      get
      {
        if (this._init)
        {
          PropertyInfo pInfo = this._typEditor.GetProperty("Text");
          return Convert.ToString(pInfo.GetValue(this._editor, null));
        }
        else
        {
          return string.Empty;
        }
      }

      set
      {
        if (this._init)
        {
          PropertyInfo pInfo = this._typEditor.GetProperty("Text");
          pInfo.SetValue(this._editor, value, null);
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The editor_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected virtual void Editor_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this._init && this._editor.Visible)
      {
        PropertyInfo pInfo;
        pInfo = this._typEditor.GetProperty("SupportFolder");
        pInfo.SetValue(this._editor, this.ResolveUrl("FreeTextBox/"), null);
        pInfo = this._typEditor.GetProperty("Width");
        pInfo.SetValue(this._editor, Unit.Percentage(100), null);
        pInfo = this._typEditor.GetProperty("DesignModeCss");
        pInfo.SetValue(this._editor, this.StyleSheet, null);

        // pInfo = typEditor.GetProperty("EnableHtmlMode");
        // pInfo.SetValue(objEditor,false,null);
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      if (this._init)
      {
        this.Load += this.Editor_Load;
        PropertyInfo pInfo = this._typEditor.GetProperty("ID");
        pInfo.SetValue(this._editor, "edit", null);
        pInfo = this._typEditor.GetProperty("AutoGenerateToolbarsFromString");
        pInfo.SetValue(this._editor, true, null);
        pInfo = this._typEditor.GetProperty("ToolbarLayout");
        pInfo.SetValue(
          this._editor, 
          "FontFacesMenu,FontSizesMenu,FontForeColorsMenu;Bold,Italic,Underline|Cut,Copy,Paste,Delete,Undo,Redo|CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent", 
          null);

        this.AddEditorControl(this._editor);
      }

      base.OnInit(e);
    }

    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
      this.RegisterSmilieyScript();
      base.OnPreRender(e);
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected virtual void RegisterSmilieyScript()
    {
      YafContext.Current.PageElements.RegisterJsBlock(
        "InsertSmileyJs", "function insertsmiley(code){" + "FTB_InsertText('" + this.SafeID + "',code);" + "}\n");
    }

    #endregion
  }
}