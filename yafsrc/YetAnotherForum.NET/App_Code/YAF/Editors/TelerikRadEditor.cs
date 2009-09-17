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
using System;
using System.Data;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Editors;

namespace YAF.Editors
{
	#region "Telerik RadEditor"
	public class RadEditor : RichClassEditor
	{
		// base("Namespace,AssemblyName")
		public RadEditor()
			: base("Telerik.Web.UI.RadEditor,Telerik.Web.UI")
		{
			InitEditorObject();
		}

		protected override void OnInit(EventArgs e)
		{
			if (_init)
			{
				Load += new EventHandler(Editor_Load);
				base.OnInit(e);
			}
		}

		protected virtual void Editor_Load(object sender, EventArgs e)
		{
			if (_init && _editor.Visible)
			{
				PropertyInfo pInfo = _typEditor.GetProperty("ID");
				pInfo.SetValue(_editor, "edit", null);
				pInfo = _typEditor.GetProperty("Skin");

				pInfo.SetValue(_editor, Config.RadEditorSkin, null);
				pInfo = _typEditor.GetProperty("Height");

				pInfo.SetValue(_editor, Unit.Pixel(400), null);
				pInfo = _typEditor.GetProperty("Width");

				pInfo.SetValue(_editor, Unit.Percentage(100), null);

				if (Config.UseRadEditorToolsFile)
				{
					pInfo = _typEditor.GetProperty("ToolsFile");
					pInfo.SetValue(_editor, Config.RadEditorToolsFile, null);
				}

				//Add Editor
				this.Controls.Add(_editor);

				//Register smiley JavaScript
				RegisterSmilieyScript();
			}
		}

		protected virtual void RegisterSmilieyScript()
		{
			YafContext.Current.PageElements.RegisterJsBlock( "InsertSmileyJs",
			                                                 @"function insertsmiley(code,img){" + "\n" + "var editor = $find('" +
			                                                 _editor.ClientID + "');" +
			                                                 "editor.pasteHtml('<img src=\"' + img + '\" alt=\"\" />');\n" +
			                                                 "}\n" );
		}

		#region Properties
		public override string Description
		{
			get
			{
				return "Telerik RAD Editor (HTML)";
			}
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 8;
			}
		}

		public override string Text
		{
			get
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Html");
					return Convert.ToString(pInfo.GetValue(_editor, null));
				}
				else return string.Empty;
			}
			set
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Html");
					pInfo.SetValue(_editor, value, null);
				}
			}
		}
		#endregion
	}
	#endregion
}