#region Usings

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Modules.HTMLEditorProvider;
using YAF.Classes;
using YAF.Classes.Data;

#endregion
/* YetAnotherForum.NET
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
    /// <summary>
    /// Adds Support for the DNN Editors
    /// Code provided by Balbes
    /// http://forum.yetanotherforum.net/yaf_postst8907_DotNetNuke-HTMLEditorProvider-integration-UPDATED-to-YAF-1-9-4.aspx
    /// </summary>
    public class DnnRichEditor : BaseForumEditor
    {
        private readonly bool _editorLoaded;
        private HtmlEditorProvider _editor;
        private string _styleSheet;

        public DnnRichEditor()
        {
            _styleSheet = string.Empty;
            _editorLoaded = InitDnnEditor();
        }

        public override string Text
        {
            get { return !_editorLoaded ? string.Empty : _editor.Text; }
            set
            {
                if (!_editorLoaded)
                    return;

                _editor.Text = value;
            }
        }

        public override string StyleSheet
        {
            get { return _styleSheet; }
            set { _styleSheet = value; }
        }

        public override bool UsesHTML
        {
            get { return true; }
        }

        public override bool UsesBBCode
        {
            get { return false; }
        }

        public override bool Active
        {
            get { return _editorLoaded; }
        }

        public override string Description
        {
            get { return "Dotnetnuke Text Editor (Html)"; }
        }

        public override int ModuleId
        {
            get
            {
                // backward compatibility...
                return 9;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            if (!_editorLoaded)
                return;

            _editor.ControlID = "yafDnnRichEditor";
            _editor.Initialize();
            Load += Editor_Load;
            base.OnInit(e);
        }

        protected virtual void Editor_Load(object sender, EventArgs e)
        {
            if (!_editorLoaded || !_editor.Visible)
                return;

            _editor.Height = Unit.Pixel(400);
            _editor.Width = Unit.Percentage(100);

            //Controls.Add(_editor.HtmlEditorControl);

            this.AddEditorControl(_editor.HtmlEditorControl);
            
            RegisterSmilieyScript();
        }

        protected virtual void RegisterSmilieyScript()
        {
            Type editorType = _editor.GetType();
            Control editor = FindControl(_editor.ControlID);
            if (editor == null)
                return;

            switch (editorType.ToString())
            {
                case "Telerik.DNN.Providers.RadEditorProvider":
                    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(),
                                                                "insertsmiley",
                                                                string.Format(
                                                                    "<script type='text/javascript'>function insertsmiley(code,img){{\nvar editor = $find('{0}');editor.pasteHtml('<img src=\"' + img + '\" alt=\"\" />');\n}}\n</script>",
                                                                    editor.ClientID));
                    break;
                case "DotNetNuke.HtmlEditor.FckHtmlEditorProvider.FckHtmlEditorProvider":
                    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(),
                                                                "insertsmiley",
                                                                string.Format(
                                                                    "<script language=\"javascript\" type=\"text/javascript\">\nfunction insertsmiley(code,img) {{\nvar oEditor = FCKeditorAPI.GetInstance('{0}');\nif ( oEditor.EditMode == FCK_EDITMODE_WYSIWYG ) {{\noEditor.InsertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }}\nelse alert( 'You must be on WYSIWYG mode!' );\n}}\n</script>\n",
                                                                    editor.ClientID.Replace("$", "_")));
                    break;
                case "WatchersNET.CKEditor.CKHtmlEditorProvider":
                    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(),
                                                                "insertsmiley",
                                                                string.Format(
                                                                    "<script language=\"javascript\" type=\"text/javascript\">\nfunction insertsmiley(code,img) {{\nvar ckEditor = CKEDITOR.instances.{0};\nif ( ckEditor.mode == 'wysiwyg' ) {{\nckEditor.insertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }}\nelse alert( 'You must be on WYSIWYG mode!' );\n}}\n</script>\n",
                                                                    editor.ClientID));
                    break;
                case "DotNetNuke.HtmlEditor.TelerikEditorProvider.EditorProvider":
                    Page.ClientScript.RegisterClientScriptBlock(Page.GetType(),
                                                                "insertsmiley",
                                                                string.Format(
                                                                    "<script type='text/javascript'>function insertsmiley(code,img){{\nvar editor = $find('{0}');editor.pasteHtml('<img src=\"' + img + '\" alt=\"\" />');\n}}\n</script>",
                                                                    editor.ClientID));
                    break;
            }
        }

        private bool InitDnnEditor()
        {
            if (!Config.IsDotNetNuke)
                return false;
            try
            {
                _editor = HtmlEditorProvider.Instance();
                return true;
            }
            catch (Exception ex)
            {
                DB.eventlog_create(null, GetType().ToString(), ex, EventLogTypes.Error);
            }
            return false;
        }
    }
}