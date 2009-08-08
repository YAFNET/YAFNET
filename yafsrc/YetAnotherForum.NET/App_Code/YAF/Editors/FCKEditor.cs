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
	public class FCKEditorV2 : RichClassEditor
	{
		public FCKEditorV2()
			: base("FredCK.FCKeditorV2.FCKeditor,FredCK.FCKeditorV2")
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
				Controls.Add(_editor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender, EventArgs e)
		{
			if (_init && _editor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = _typEditor.GetProperty("BasePath");
				pInfo.SetValue(_editor, ResolveUrl("FCKEditorV2/"), null);

				pInfo = _typEditor.GetProperty("Height");
				pInfo.SetValue(_editor, Unit.Pixel(300), null);

				Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "fckeditorjs", string.Format(@"<script language=""javascript"" type=""text/javascript"" src=""{0}""></script>", ResolveUrl("FCKEditorV2/FCKEditor.js")));

				RegisterSmilieyScript();
			}
		}

		protected virtual void RegisterSmilieyScript()
		{
			// insert smiliey code -- can't get this working with FireFox!
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "insertsmiley",
				@"<script language=""javascript"" type=""text/javascript"">" + "\n" +
				"function insertsmiley(code,img) {\n" +
				"var oEditor = FCKeditorAPI.GetInstance('" + SafeID + "');\n" +
				"if ( oEditor.EditMode == FCK_EDITMODE_WYSIWYG ) {\n" +
				"oEditor.InsertHtml( '<img src=\"' + img + '\" alt=\"\" />' ); }\n" +
				"else alert( 'You must be on WYSIWYG mode!' );\n" +
				"}\n" +
				"</script>\n");
		}

		#region Properties

		public override string Description
		{
			get
			{
				return "FCK Editor v2 (HTML)";
			}
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 2;
			}
		}

		public override string Text
		{
			get
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Value");
					return Convert.ToString(pInfo.GetValue(_editor, null));
				}
				else return string.Empty;
			}
			set
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Value");
					pInfo.SetValue(_editor, value, null);
				}
			}
		}
		#endregion
	}

	public class FCKEditorV1 : RichClassEditor
	{
		public FCKEditorV1()
			: base("FredCK.FCKeditor,FredCK.FCKeditor")
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
				Controls.Add(_editor);
			}
			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender, EventArgs e)
		{
			if (_init && _editor.Visible)
			{
				PropertyInfo pInfo;
				pInfo = _typEditor.GetProperty("BasePath");
				pInfo.SetValue(_editor, ResolveUrl("FCKEditorV1/"), null);

				Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "fckeditorjs", string.Format(@"<script language=""javascript"" type=""text/javascript"" src=""{0}""></script>", ResolveUrl("FCKEditorV1/FCKEditor.js")));
			}
		}

		#region Properties
		public override string Description
		{
			get
			{
				return "FCK Editor v1.6 (HTML)";
			}
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 4;
			}
		}

		public override string Text
		{
			get
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Value");
					return Convert.ToString(pInfo.GetValue(_editor, null));
				}
				else return string.Empty;
			}
			set
			{
				if (_init)
				{
					PropertyInfo pInfo = _typEditor.GetProperty("Value");
					pInfo.SetValue(_editor, value, null);
				}
			}
		}
		#endregion
	}
}
