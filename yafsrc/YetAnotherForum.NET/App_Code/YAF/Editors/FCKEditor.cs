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
  /// The fck editor v 2.
  /// </summary>
  public class FCKEditorV2 : RichClassEditor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FCKEditorV2"/> class.
    /// </summary>
    public FCKEditorV2()
      : base("FredCK.FCKeditorV2.FCKeditor,FredCK.FCKeditorV2")
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
        pInfo = this._typEditor.GetProperty("BasePath");
        pInfo.SetValue(this._editor, ResolveUrl("FCKEditorV2/"), null);

        pInfo = this._typEditor.GetProperty("Height");
        pInfo.SetValue(this._editor, Unit.Pixel(300), null);

        YafContext.Current.PageElements.RegisterJsInclude("FckEditorJs", ResolveUrl("FCKEditorV2/FCKEditor.js"));

        RegisterSmilieyScript();
      }
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected virtual void RegisterSmilieyScript()
    {
      // insert smiliey code -- can't get this working with FireFox!
      YafContext.Current.PageElements.RegisterJsBlock(
        "InsertSmileyJs", 
        "function insertsmiley(code,img) {\n" + "var oEditor = FCKeditorAPI.GetInstance('" + SafeID + "');\n" +
        "if ( oEditor.EditMode == FCK_EDITMODE_WYSIWYG ) {\n" + "oEditor.InsertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }\n" +
        "else alert( 'You must be on WYSIWYG mode!' );\n" + "}\n");
    }

    #region Properties

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "FCK Editor v2 (HTML)";
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
        return 2;
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
  }

  /// <summary>
  /// The fck editor v 1.
  /// </summary>
  public class FCKEditorV1 : RichClassEditor
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="FCKEditorV1"/> class.
    /// </summary>
    public FCKEditorV1()
      : base("FredCK.FCKeditor,FredCK.FCKeditor")
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
        pInfo = this._typEditor.GetProperty("BasePath");
        pInfo.SetValue(this._editor, ResolveUrl("FCKEditorV1/"), null);

        YafContext.Current.PageElements.RegisterJsInclude("FckEditorJs", ResolveUrl("FCKEditorV1/FCKEditor.js"));
      }
    }

    #region Properties

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "FCK Editor v1.6 (HTML)";
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
        return 4;
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
  }
}