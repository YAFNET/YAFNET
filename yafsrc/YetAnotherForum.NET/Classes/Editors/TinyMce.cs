/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System.Web.UI;
  using YAF.Classes.Core;

  /// <summary>
  /// The tiny mce editor.
  /// </summary>
  public abstract class TinyMceEditor : TextEditor
  {
    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public override string Text
    {
      get
      {
        return this._textCtl.InnerText;
      }

      set
      {
        this._textCtl.InnerText = value;
      }
    }

    /// <summary>
    /// Gets SafeID.
    /// </summary>
    protected new string SafeID
    {
      get
      {
        return this._textCtl.ClientID.Replace("$", "_");
      }
    }

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      // TODO: Adding another Event when already added to the base, and overrided??? Don't think the below is needed
      // PreRender += new EventHandler(Editor_PreRender); 
      base.OnInit(e);

      this._textCtl.Attributes.CssStyle.Add("width", "100%");
      this._textCtl.Attributes.CssStyle.Add("height", "350px");
    }

    /// <summary>
    /// The editor_ PreRender.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void Editor_PreRender(object sender, EventArgs e)
    {
        ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "tinymce", ResolveUrl("tiny_mce/tiny_mce.js"));
        RegisterTinyMceCustomJS();
        RegisterSmilieyScript();
    }

    /// <summary>
    /// The register tiny mce custom js.
    /// </summary>
    protected abstract void RegisterTinyMceCustomJS();
  }

  /// <summary>
  /// The tiny mce html editor.
  /// </summary>
  public class TinyMceHtmlEditor : TinyMceEditor
  {
    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "TinyMCE (HTML)";
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    public override bool UsesHTML
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode
    {
      get
      {
        return false;
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
        return 7;
      }
    }

    /// <summary>
    /// The register tiny mce custom js.
    /// </summary>
    protected override void RegisterTinyMceCustomJS()
    {
      YafContext.Current.PageElements.RegisterJsInclude("tinymceinit", ResolveUrl("tiny_mce/tiny_mce_init.js"));
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected override void RegisterSmilieyScript()
    {
      YafContext.Current.PageElements.RegisterJsBlock(
        "InsertSmileyJs", 
        "function insertsmiley(code,img) {\n" + "	tinyMCE.execCommand('mceInsertContent',false,'<img src=\"' + img + '\" alt=\"\" />');\n" + "}\n");
    }
  }

  /// <summary>
  /// The tiny mce bb code editor.
  /// </summary>
  public class TinyMceBBCodeEditor : TinyMceEditor
  {
    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "TinyMCE (BBCode)";
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesHTML.
    /// </summary>
    public override bool UsesHTML
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether UsesBBCode.
    /// </summary>
    public override bool UsesBBCode
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public override int ModuleId
    {
      get
      {
        return Description.GetHashCode();
      }
    }

    /// <summary>
    /// The register tiny mce custom js.
    /// </summary>
    protected override void RegisterTinyMceCustomJS()
    {
      ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "tinymceinit", ResolveUrl("tiny_mce/tiny_mce_initbbcode.js"));
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected override void RegisterSmilieyScript()
    {
      ScriptManager.RegisterClientScriptBlock(
        Page, Page.GetType(), "insertsmiley", "function insertsmiley(code,img) {\n" + "	tinyMCE.execCommand('mceInsertContent',false,code);\n" + "}\n", true);
    }
  }
}