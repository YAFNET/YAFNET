/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System;
  using System.Reflection;
  using System.Web.UI.WebControls;
  using YAF.Classes.Core;

  /// <summary>
  /// The free text box editor.
  /// </summary>
  public class FreeTextBoxEditor : RichClassEditor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FreeTextBoxEditor"/> class.
    /// </summary>
    public FreeTextBoxEditor()
      : base("FreeTextBoxControls.FreeTextBox,FreeTextBox")
    {
      InitEditorObject();
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      if (this._init)
      {
        Load += new EventHandler(Editor_Load);
        PropertyInfo pInfo = this._typEditor.GetProperty("ID");
        pInfo.SetValue(this._editor, "edit", null);
        pInfo = this._typEditor.GetProperty("AutoGenerateToolbarsFromString");
        pInfo.SetValue(this._editor, true, null);
        pInfo = this._typEditor.GetProperty("ToolbarLayout");
        pInfo.SetValue(
          this._editor, 
          "FontFacesMenu,FontSizesMenu,FontForeColorsMenu;Bold,Italic,Underline|Cut,Copy,Paste,Delete,Undo,Redo|CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent", 
          null);
        Controls.Add(this._editor);
      }

      base.OnInit(e);
    }

    /// <summary>
    /// The editor_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected virtual void Editor_Load(object sender, EventArgs e)
    {
      if (this._init && this._editor.Visible)
      {
        PropertyInfo pInfo;
        pInfo = this._typEditor.GetProperty("SupportFolder");
        pInfo.SetValue(this._editor, ResolveUrl("FreeTextBox/"), null);
        pInfo = this._typEditor.GetProperty("Width");
        pInfo.SetValue(this._editor, Unit.Percentage(100), null);
        pInfo = this._typEditor.GetProperty("DesignModeCss");
        pInfo.SetValue(this._editor, StyleSheet, null);

        // pInfo = typEditor.GetProperty("EnableHtmlMode");
        // pInfo.SetValue(objEditor,false,null);
        RegisterSmilieyScript();
      }
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected virtual void RegisterSmilieyScript()
    {
      YafContext.Current.PageElements.RegisterJsBlock("InsertSmileyJs", "function insertsmiley(code){" + "FTB_InsertText('" + SafeID + "',code);" + "}\n");
    }

    #region Properties

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "Free Text Box v2 (HTML)";
      }
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public override int ModuleId
    {
      get
      {
        // backward compatibility...
        return 3;
      }
    }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
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
  }

  /// <summary>
  /// The free text box editorv 3.
  /// </summary>
  public class FreeTextBoxEditorv3 : FreeTextBoxEditor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FreeTextBoxEditorv3"/> class.
    /// </summary>
    public FreeTextBoxEditorv3()
      : base()
    {
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public override int ModuleId
    {
      get
      {
        // backward compatibility...
        return 6;
      }
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "Free Text Box v3 (HTML)";
      }
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected override void RegisterSmilieyScript()
    {
      YafContext.Current.PageElements.RegisterJsBlock(
        "InsertSmileyJs", @"function insertsmiley(code,img){" + "FTB_API['" + SafeID + "'].InsertHtml('<img src=\"' + img + '\" alt=\"\" />');" + "}\n");
    }
  }
}