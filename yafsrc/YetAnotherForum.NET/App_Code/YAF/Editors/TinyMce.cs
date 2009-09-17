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
	public abstract class TinyMceEditor : TextEditor
	{
		protected override void OnInit(EventArgs e)
		{
			Load += new EventHandler(Editor_Load);
			base.OnInit(e);

			_textCtl.Attributes.CssStyle.Add( "width","100%" );
			_textCtl.Attributes.CssStyle.Add( "height", "350px" );
		}

		protected override void Editor_Load(object sender, EventArgs e)
		{
			ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "tinymce", ResolveUrl("tiny_mce/tiny_mce.js"));
			RegisterTinyMceCustomJS();
			RegisterSmilieyScript();
		}

		protected abstract void RegisterTinyMceCustomJS();

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

		new protected string SafeID
		{
			get { return _textCtl.ClientID.Replace("$", "_"); }
		}
	}

	public class TinyMceHtmlEditor : TinyMceEditor
	{
		protected override void RegisterTinyMceCustomJS()
		{
			YafContext.Current.PageElements.RegisterJsInclude( "tinymceinit", ResolveUrl( "tiny_mce/tiny_mce_init.js" ) );
		}

		protected override void RegisterSmilieyScript()
		{
			YafContext.Current.PageElements.RegisterJsBlock( "InsertSmileyJs",
			                                                 "function insertsmiley(code,img) {\n" +
			                                                 "	tinyMCE.execCommand('mceInsertContent',false,'<img src=\"' + img + '\" alt=\"\" />');\n" +
			                                                 "}\n" );
		}

		public override string Description
		{
			get
			{
				return "TinyMCE (HTML)";
			}
		}

		public override bool UsesHTML
		{
			get { return true; }
		}

		public override bool UsesBBCode
		{
			get { return false; }
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 7;
			}
		}
	}

	public class TinyMceBBCodeEditor : TinyMceEditor
	{
		protected override void RegisterTinyMceCustomJS()
		{
			ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), "tinymceinit",
																								 ResolveUrl("tiny_mce/tiny_mce_initbbcode.js"));
		}

		protected override void RegisterSmilieyScript()
		{
			ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), "insertsmiley",
				"function insertsmiley(code,img) {\n" +
				"	tinyMCE.execCommand('mceInsertContent',false,'[img]' + img + '[/img]');\n" +
				"}\n", true);
		}

		public override string Description
		{
			get
			{
				return "TinyMCE (BBCode)";
			}
		}

		public override bool UsesHTML
		{
			get { return false; }
		}

		public override bool UsesBBCode
		{
			get { return true; }
		}

		public override int ModuleId
		{
			get
			{
				return Description.GetHashCode();
			}
		}
	}
}
