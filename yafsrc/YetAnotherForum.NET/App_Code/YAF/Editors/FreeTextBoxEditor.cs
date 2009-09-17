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
	public class FreeTextBoxEditor : RichClassEditor
	{
		public FreeTextBoxEditor()
			: base("FreeTextBoxControls.FreeTextBox,FreeTextBox")
		{
			InitEditorObject();
		}

		protected override void OnInit(EventArgs e)
		{
			if (_init)
			{
				Load += new EventHandler(Editor_Load);
				PropertyInfo pInfo = _typEditor.GetProperty("ID");
				pInfo.SetValue(_editor, "edit", null);
				pInfo = _typEditor.GetProperty("AutoGenerateToolbarsFromString");
				pInfo.SetValue(_editor, true, null);
				pInfo = _typEditor.GetProperty("ToolbarLayout");
				pInfo.SetValue(_editor, "FontFacesMenu,FontSizesMenu,FontForeColorsMenu;Bold,Italic,Underline|Cut,Copy,Paste,Delete,Undo,Redo|CreateLink,Unlink|JustifyLeft,JustifyRight,JustifyCenter,JustifyFull;BulletedList,NumberedList,Indent,Outdent", null);
				Controls.Add(_editor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender, EventArgs e)
		{
			if (_init && _editor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = _typEditor.GetProperty("SupportFolder");
				pInfo.SetValue(_editor, ResolveUrl("FreeTextBox/"), null);
				pInfo = _typEditor.GetProperty("Width");
				pInfo.SetValue(_editor, Unit.Percentage(100), null);
				pInfo = _typEditor.GetProperty("DesignModeCss");
				pInfo.SetValue(_editor, StyleSheet, null);
				//pInfo = typEditor.GetProperty("EnableHtmlMode");
				//pInfo.SetValue(objEditor,false,null);

				RegisterSmilieyScript();
			}
		}

		protected virtual void RegisterSmilieyScript()
		{
			YafContext.Current.PageElements.RegisterJsBlock( "InsertSmileyJs",
			                                                 "function insertsmiley(code){" + "FTB_InsertText('" + SafeID +
			                                                 "',code);" + "}\n" );
		}

		#region Properties
		public override string Description
		{
			get
			{
				return "Free Text Box v2 (HTML)";
			}
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 3;
			}
		}

		public override string Text
		{
			get
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Text");
					return Convert.ToString(pInfo.GetValue(_editor, null));
				}
				else return string.Empty;
			}
			set
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Text");
					pInfo.SetValue(_editor, value, null);
				}
			}
		}
		#endregion
	}

	public class FreeTextBoxEditorv3 : FreeTextBoxEditor
	{
		public FreeTextBoxEditorv3()
			: base()
		{
		}

		protected override void RegisterSmilieyScript()
		{
			YafContext.Current.PageElements.RegisterJsBlock( "InsertSmileyJs",
			                                                 @"function insertsmiley(code,img){" + "FTB_API['" + SafeID +
			                                                 "'].InsertHtml('<img src=\"' + img + '\" alt=\"\" />');" + "}\n" );
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 6;
			}
		}

		public override string Description
		{
			get
			{
				return "Free Text Box v3 (HTML)";
			}
		}
	}
}
