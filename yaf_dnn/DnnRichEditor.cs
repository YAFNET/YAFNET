/* YetAnotherForum.NET
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
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using global::DotNetNuke.Modules.HTMLEditorProvider;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;

    #endregion

  /// <summary>
  /// Adds Support for the DNN Editors
  ///   Code provided by Balbes
  ///   http://forum.yetanotherforum.net/yaf_postst8907_DotNetNuke-HTMLEditorProvider-integration-UPDATED-to-YAF-1-9-4.aspx
  /// </summary>
  public class DnnRichEditor : RichClassEditor
  {
    #region Constants and Fields

    /// <summary>
    /// The _editor loaded.
    /// </summary>
    private readonly bool _editorLoaded;

    /// <summary>
    /// The _editor.
    /// </summary>
    private HtmlEditorProvider _editor;

    /// <summary>
    /// The _style sheet.
    /// </summary>
    private string _styleSheet;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DnnRichEditor"/> class.
    /// </summary>
    public DnnRichEditor()
    {
      this._styleSheet = string.Empty;
      this._editorLoaded = this.InitDnnEditor();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether Active.
    /// </summary>
    public override bool Active
    {
      get
      {
        return this._editorLoaded;
      }
    }

    /// <summary>
    /// Gets Description.
    /// </summary>
    public override string Description
    {
      get
      {
        return "DotNetNuke Text Editor (HTML)";
      }
    }

    /// <summary>
    /// Gets ModuleId.
    /// </summary>
    public override string ModuleId
    {
      get
      {
        // backward compatibility...
        return "9";
      }
    }

    /// <summary>
    /// Gets or sets StyleSheet.
    /// </summary>
    public override string StyleSheet
    {
      get
      {
        return this._styleSheet;
      }

      set
      {
        this._styleSheet = value;
      }
    }

    /// <summary>
    /// Gets or sets Text.
    /// </summary>
    public override string Text
    {
      get
      {
        return !this._editorLoaded ? string.Empty : this._editor.Text;
      }

      set
      {
        if (!this._editorLoaded)
        {
          return;
        }

        this._editor.Text = value;
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
    /// Gets a value indicating whether UsesHTML.
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
    /// Handles the Load event of the Editor control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    protected virtual void Editor_Load(object sender, EventArgs e)
    {
      if (!this._editorLoaded || !this._editor.Visible)
      {
        return;
      }

      this._editor.Height = Unit.Pixel(400);
      this._editor.Width = Unit.Percentage(100); // Change Me

      this.Controls.Add(this._editor.HtmlEditorControl);

      this.RegisterSmilieyScript();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    protected override void OnInit(EventArgs e)
    {
      if (!this._editorLoaded)
      {
        return;
      }

      this._editor.ControlID = "yafDnnRichEditor";
      this._editor.Initialize();
      this.Load += this.Editor_Load;
      base.OnInit(e);
    }

    /// <summary>
    /// The register smiliey script.
    /// </summary>
    protected virtual void RegisterSmilieyScript()
    {
      Type editorType = this._editor.GetType();
      Control editor = this.FindControl(this._editor.ControlID);
      if (editor == null)
      {
        return;
      }

      switch (editorType.ToString())
      {
        case "Telerik.DNN.Providers.RadEditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley", 
            "<script type='text/javascript'>function insertsmiley(code,img){{\nvar editor = $find('{0}');editor.pasteHtml('<img src=\"' + img + '\" alt=\"\" />');\n}}\n</script>".FormatWith(editor.ClientID));
          break;
        case "DotNetNuke.HtmlEditor.FckHtmlEditorProvider.FckHtmlEditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley", 
            "<script language=\"javascript\" type=\"text/javascript\">\nfunction insertsmiley(code,img) {{\nvar oEditor = FCKeditorAPI.GetInstance('{0}');\nif ( oEditor.EditMode == FCK_EDITMODE_WYSIWYG ) {{\noEditor.InsertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }}\nelse alert( 'You must be on WYSIWYG mode!' );\n}}\n</script>\n".FormatWith(editor.ClientID.Replace("$", "_")));
          break;
        case "WatchersNET.CKEditor.CKHtmlEditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley", 
            "<script language=\"javascript\" type=\"text/javascript\">\nfunction insertsmiley(code,img) {{\nvar ckEditor = CKEDITOR.instances.{0};\nif ( ckEditor.mode == 'wysiwyg' ) {{\nckEditor.insertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }}\nelse alert( 'You must be on WYSIWYG mode!' );\n}}\n</script>\n".FormatWith(editor.ClientID));
          break;
        case "DotNetNuke.HtmlEditor.TelerikEditorProvider.EditorProvider":
          this.Page.ClientScript.RegisterClientScriptBlock(
            this.Page.GetType(), 
            "insertsmiley", 
            "<script type='text/javascript'>function insertsmiley(code,img){{\nvar editor = $find('{0}');editor.pasteHtml('<img src=\"' + img + '\" alt=\"\" />');\n}}\n</script>".FormatWith(editor.ClientID));
          break;
      }
    }

    /// <summary>
    /// Init the DNN editor.
    /// </summary>
    /// <returns>
    /// The init DNN editor.
    /// </returns>
    private bool InitDnnEditor()
    {
      if (!Config.IsDotNetNuke)
      {
        return false;
      }

      try
      {
        this._editor = HtmlEditorProvider.Instance();
        return true;
      }
      catch (Exception ex)
      {
        LegacyDb.eventlog_create(null, this.GetType().ToString(), ex, EventLogTypes.Error);
      }

      return false;
    }

    #endregion
  }
}