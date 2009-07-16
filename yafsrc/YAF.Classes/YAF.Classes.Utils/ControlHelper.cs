/* Yet Another Forum.net
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
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;

namespace YAF.Classes.Utils
{
	static public class ControlHelper
	{
		static public System.Web.UI.Control FindControlRecursive(System.Web.UI.Control currentControl, string id)
		{
			System.Web.UI.Control foundControl = currentControl.FindControl(id);

			if (foundControl != null)
			{
				return foundControl;
			}
			else if (currentControl.Parent != null)
			{
				return FindControlRecursive(currentControl.Parent, id);
			}
			return null;
		}

		static public HtmlLink MakeCssIncludeControl(string href)
		{
			HtmlLink stylesheet = new HtmlLink();
			stylesheet.Href = href;
			stylesheet.Attributes.Add("rel", "stylesheet");
			stylesheet.Attributes.Add("type", "text/css");

			return stylesheet;
		}

		static public HtmlGenericControl MakeCssControl(string css)
		{
			HtmlGenericControl style = new HtmlGenericControl();
			style.TagName = "style";
			style.Attributes.Add("type", "text/css");
			style.InnerText = css;

			return style;
		}

		static public HtmlGenericControl MakeJsIncludeControl(string href)
		{
			HtmlGenericControl js = new HtmlGenericControl();
			js.TagName = "script";
			js.Attributes.Add( "type", "text/javascript" );
			js.Attributes.Add( "src", href );

			return js;
		}

		/* Ederon - 7/1/2007 start */
		static public void AddStyleAttributeSize(System.Web.UI.WebControls.WebControl control, string width, string height)
		{
			control.Attributes.Add("style", String.Format("width: {0}; height: {1};", width, height));
		}

		static public void AddStyleAttributeWidth(System.Web.UI.WebControls.WebControl control, string width)
		{
			control.Attributes.Add("style", String.Format("width: {0};", width));
		}

		static public void AddStyleAttributeHeight(System.Web.UI.WebControls.WebControl control, string height)
		{
			control.Attributes.Add("style", String.Format("height: {0};", height));
		}

		/* Ederon - 7/1/2007 end */
		static public void AddOnClickConfirmDialog(object control, string message)
		{
			AddOnClickConfirmDialog((System.Web.UI.WebControls.WebControl)control, message);
		}
		static public void AddOnClickConfirmDialog(System.Web.UI.WebControls.WebControl control, string message)
		{
			control.Attributes["onclick"] = String.Format("return confirm('{0}');", message);
		}

	}
}
