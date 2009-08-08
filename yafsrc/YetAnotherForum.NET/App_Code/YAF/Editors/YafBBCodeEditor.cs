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
using YAF.Classes.Utils;

namespace YAF.Editors
{
	public class BBCodeEditor : TextEditor
	{
		private YAF.Controls.PopMenu _popMenu = null;

		private void RenderButton(HtmlTextWriter writer, string id, string cmd, string title, string image)
		{
			//writer.WriteLine("		<td><img id='{1}_{4}' onload='Button_Load(this)' src='{0}' width='21' height='20' alt='{2}' title='{2}' onclick=\"{1}.{3}\"></td><td>&nbsp;</td>",ResolveUrl(image),SafeID,title,cmd,id);
			writer.WriteLine(@"<img id=""{1}_{4}"" onload=""Button_Load(this)"" src=""{0}"" width=""21"" height=""20"" alt=""{2}"" title=""{2}"" onclick=""setStyle('{4}','')"" />", ResolveUrl(image), SafeID, title, cmd, id);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			writer.WriteLine("<table border='0' cellpadding='0' cellspacing='2' width='100%'>");
			writer.WriteLine("<tr><td valign='top'>");
			writer.WriteLine("	<table border='0' cellpadding='1' cellspacing='2' id='bbcodeFeatures'>");
			writer.WriteLine("	<tr><td valign='middle'>");

			RenderButton(writer, "bold", "FormatText('bold','')", YafContext.Current.Localization.GetText("COMMON", "TT_BOLD"), "yafEditor/bold.gif");
			RenderButton(writer, "italic", "FormatText('italic','')", YafContext.Current.Localization.GetText("COMMON", "TT_ITALIC"), "yafEditor/italic.gif");
			RenderButton(writer, "underline", "FormatText('underline','')", YafContext.Current.Localization.GetText("COMMON", "TT_UNDERLINE"), "yafEditor/underline.gif");

			writer.WriteLine("&nbsp;");

			RenderButton(writer, "quote", "FormatText('quote','')", YafContext.Current.Localization.GetText("COMMON", "TT_QUOTE"), "yafEditor/quote.gif");
			RenderButton(writer, "code", "FormatText('code','')", YafContext.Current.Localization.GetText("COMMON", "TT_CODE"), "yafEditor/code.gif");
			RenderButton(writer, "img", "FormatText('img','')", YafContext.Current.Localization.GetText("COMMON", "TT_IMAGE"), "yafEditor/image.gif");
			RenderButton(writer, "createlink", "FormatText('createlink','')", YafContext.Current.Localization.GetText("COMMON", "TT_CREATELINK"), "yafEditor/link.gif");

			writer.WriteLine("&nbsp;");

			RenderButton(writer, "justifyleft", "FormatText('justifyleft','')", YafContext.Current.Localization.GetText("COMMON", "TT_ALIGNLEFT"), "yafEditor/justifyleft.gif");
			RenderButton(writer, "justifycenter", "FormatText('justifycenter','')", YafContext.Current.Localization.GetText("COMMON", "TT_ALIGNCENTER"), "yafEditor/justifycenter.gif");
			RenderButton(writer, "justifyright", "FormatText('justifyright','')", YafContext.Current.Localization.GetText("COMMON", "TT_ALIGNRIGHT"), "yafEditor/justifyright.gif");

			DataTable bbCodeTable = YAF.Classes.UI.YafBBCode.GetCustomBBCode();

			if (bbCodeTable.Rows.Count > 0)
			{
				writer.WriteLine("&nbsp;");

				// add drop down for optional "extra" codes...
				writer.WriteLine(String.Format(@"<img src=""{5}"" id=""{3}"" alt=""{4}"" title=""{4}"" onclick=""{0}"" onload=""Button_Load(this)"" onmouseover=""{1}"" />", _popMenu.ControlOnClick, _popMenu.ControlOnMouseOver, YafContext.Current.Localization.GetText("COMMON", "CUSTOM_BBCODE"), this.ClientID + "_bbcode_popMenu", YafContext.Current.Localization.GetText("COMMON", "TT_CUSTOMBBCODE"), ResolveUrl("yafEditor/bbcode.gif")));

				foreach (DataRow row in bbCodeTable.Rows)
				{
					string name = row["Name"].ToString();

					if (row["Description"] != DBNull.Value && !String.IsNullOrEmpty(row["Description"].ToString()))
					{
						// use the description as the option "name"
						name = YAF.Classes.UI.YafBBCode.LocalizeCustomBBCodeElement(row["Description"].ToString());
					}

					string onclickJS = string.Empty;

					if (row["OnClickJS"] != DBNull.Value && !String.IsNullOrEmpty(row["OnClickJS"].ToString()))
					{
						onclickJS = row["OnClickJS"].ToString();
					}
					else
					{
						// assume the bbcode is just the name... 
						onclickJS = string.Format("setStyle('{0}','')", row["Name"].ToString().Trim());
					}

					_popMenu.AddClientScriptItem(name, onclickJS);
				}
			}

			writer.WriteLine("	</td></tr>");
			writer.WriteLine("	<tr><td valign='middle'>");

			writer.WriteLine(YafContext.Current.Localization.GetText("COMMON", "FONT_COLOR"));
			writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('color',this.value); this.value=''\">", SafeID);
			writer.WriteLine("<option value=\"\">Default</option>");

			string[] Colors = { "Dark Red", "Red", "Orange", "Brown", "Yellow", "Green", "Olive", "Cyan", "Blue", "Dark Blue", "Indigo", "Violet", "White", "Black" };
			foreach (string color in Colors)
			{
				string tValue = color.Replace(" ", "").ToLower();
				writer.WriteLine(string.Format("<option style=\"color:{0}\" value=\"{0}\">{1}</option>", tValue, color));
			}

			writer.WriteLine("</select>");

			writer.WriteLine(YafContext.Current.Localization.GetText("COMMON", "FONT_SIZE"));
			writer.WriteLine("<select onchange=\"if(this.value!='') setStyle('fontsize',this.value); this.value=''\">", SafeID);
			writer.WriteLine("<option value=\"1\">1</option>");
			writer.WriteLine("<option value=\"2\">2</option>");
			writer.WriteLine("<option value=\"3\">3</option>");
			writer.WriteLine("<option value=\"4\">4</option>");
			writer.WriteLine("<option selected=\"selected\" value=\"5\">Default</option>");
			writer.WriteLine("<option value=\"6\">6</option>");
			writer.WriteLine("<option value=\"7\">7</option>");
			writer.WriteLine("<option value=\"8\">8</option>");
			writer.WriteLine("<option value=\"9\">9</option>");
			writer.WriteLine("</select>");

			writer.WriteLine("	</td></tr>");
			writer.WriteLine("	</table>");
			writer.WriteLine(@"</td></tr><tr><td height=""99%"">");

			_textCtl.RenderControl(writer);

			writer.WriteLine("</td></tr></table>");

			_popMenu.RenderControl(writer);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_textCtl.Attributes.Add("class", "BBCodeEditor");
			// add popmenu to this mix...
			_popMenu = new YAF.Controls.PopMenu();
			this.Controls.Add(_popMenu);
		}

		protected override void Editor_Load(object sender, EventArgs e)
		{
			base.Editor_Load(sender, e);
			// register custom YafBBCode javascript (if there is any)
			// this call is supposed to be after editor load since it may use
			// JS variables created in editor_load...
			YAF.Classes.UI.YafBBCode.RegisterCustomBBCodePageElements(Page, this.GetType(), SafeID);
			ScriptManager.RegisterClientScriptInclude(this, typeof(BBCodeEditor), "yafjs", YafForumInfo.GetURLToResource("js/yaf.js"));
		}

		#region Properties

		public override bool UsesBBCode
		{
			get { return true; }
		}

		public override string Description
		{
			get
			{
				return "YAF Standard YafBBCode Editor";
			}
		}

		public override int ModuleId
		{
			get
			{
				// backward compatibility...
				return 1;
			}
		}

		#endregion
	}
}
