﻿/* Yet Another Forum.NET
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
  /// The fck editor v 2.
  /// </summary>
  public class FCKEditorV2 : RichClassEditor
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "FCKEditorV2" /> class.
    /// </summary>
    public FCKEditorV2()
      : base("FredCK.FCKeditorV2.FCKeditor,FredCK.FCKeditorV2")
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
        return "FCK Editor v2 (HTML)";
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
        return "2";
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
          PropertyInfo pInfo = this._typEditor.GetProperty("Value");
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
          PropertyInfo pInfo = this._typEditor.GetProperty("Value");
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
        pInfo = this._typEditor.GetProperty("BasePath");
        pInfo.SetValue(this._editor, this.ResolveUrl("FCKEditorV2/"), null);

        pInfo = this._typEditor.GetProperty("Height");
        pInfo.SetValue(this._editor, Unit.Pixel(300), null);
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
      YafContext.Current.PageElements.RegisterJsInclude("FckEditorJs", this.ResolveUrl("FCKEditorV2/FCKEditor.js"));

      this.RegisterSmilieyScript();
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected virtual void RegisterSmilieyScript()
    {
      // insert smiliey code -- can't get this working with FireFox!
      YafContext.Current.PageElements.RegisterJsBlock(
        "InsertSmileyJs", 
        "function insertsmiley(code,img) {\n" + "var oEditor = FCKeditorAPI.GetInstance('" + this.SafeID + "');\n" +
        "if ( oEditor.EditMode == FCK_EDITMODE_WYSIWYG ) {\n" +
        "oEditor.InsertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }\n" +
        "else alert( 'You must be on WYSIWYG mode!' );\n" + "}\n");
    }

    #endregion
  }
}