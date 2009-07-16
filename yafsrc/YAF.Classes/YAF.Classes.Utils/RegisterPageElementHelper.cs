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
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace YAF.Classes.Utils
{
	static public class RegisterPageElementHelper
	{
		static public bool PageElementExists(string name)
		{
			if ( YafContext.Current != null )
			{
				return YafContext.Current.RegisteredElements.Contains( name.ToLower() );
			}

			return false;
		}

		static public void AddPageElement( string name )
		{
			if (YafContext.Current != null)
			{
				YafContext.Current.RegisteredElements.Add( name.ToLower() );
			}
		}

		/// <summary>
		/// Adds the given CSS to the page header within a <![CDATA[<style>]]> tag
		/// </summary>
		/// <param name="element">Control to add element</param>
		/// <param name="name">Name of this element so it is not added more then once (not case sensitive)</param>
		/// <param name="cssContents">The contents of the text/css style block</param>
		static public void RegisterCssBlock(Control element, string name, string cssContents)
		{
			if (!PageElementExists( name ))
			{
				// Add to the end of the controls collection
				element.Controls.AddAt(element.Controls.Count, ControlHelper.MakeCssControl(cssContents));
				AddPageElement( name );
			}
		}

		/// <summary>
		/// Adds the given CSS to the page header within a <![CDATA[<style>]]> tag
		/// </summary>
		/// <param name="element">Control to add element</param>
		/// <param name="cssUrl">Url of the CSS file to add</param>
		static public void RegisterCssInclude(Control element, string cssUrl)
		{
			if (!PageElementExists(cssUrl))
			{
				element.Controls.Add( ControlHelper.MakeCssIncludeControl( cssUrl.ToLower() ) );
				AddPageElement(cssUrl);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		static public void RegisterJQuery( Control element )
		{
			if (!PageElementExists("jquery"))
			{
				// load jQuery from google...
				const string jQueryLoad = "<script type=\"text/javascript\" src=\"\"></script>";
				element.Controls.Add(
					ControlHelper.MakeJsIncludeControl( "http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" ) );
				AddPageElement( "jquery" );
			}
		}
	}
}
