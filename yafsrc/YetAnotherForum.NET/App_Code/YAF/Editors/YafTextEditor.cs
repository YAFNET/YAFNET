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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using YAF.Editors;

namespace YAF.Editors
{
	public class TextEditor : BaseForumEditor
	{
		protected HtmlTextArea _textCtl;

		protected override void OnInit(EventArgs e)
		{
			Load += new EventHandler(Editor_Load);

			_textCtl = new HtmlTextArea();
			_textCtl.ID = "YafTextEditor";
			_textCtl.Rows = 15;
			_textCtl.Cols = 100;
			_textCtl.Attributes.Add( "class", "YafTextEditor" );
			Controls.Add(_textCtl);

			base.OnInit(e);
		}

		protected virtual void Editor_Load(object sender, EventArgs e)
		{
			// Ederon : 9/6/2007
			/*if (this.Visible || this.)
			{*/
			ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "yafeditorjs", ResolveUrl("yafEditor/yafEditor.js"));

			ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "createyafeditor",
				"var " + SafeID + "=new yafEditor('" + SafeID + "');\n" +
				"function setStyle(style,option) {\n" +
				"	" + SafeID + ".FormatText(style,option);\n" +
				"}\n", true);

			RegisterSmilieyScript();
			/*}*/
		}

		protected virtual void RegisterSmilieyScript()
		{
			ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "insertsmiley",
				"function insertsmiley(code) {\n" +
				"	" + SafeID + ".InsertSmiley(code);\n" +
				"}\n", true);
		}

		public override bool Active
		{
			get
			{
				return true;
			}
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 0;
			}
		}

		public override string Description
		{
			get
			{
				return "Plain Text Editor";
			}
		}

		public override string Text
		{
			get
			{
				return _textCtl.InnerText;
			}
			set
			{
				_textCtl.InnerText = value;
			}
		}

		protected string SafeID
		{
			get { return _textCtl.ClientID.Replace("$", "_"); }
		}

		public override bool UsesHTML
		{
			get { return false; }
		}
		public override bool UsesBBCode
		{
			get { return false; }
		}
	}
}
