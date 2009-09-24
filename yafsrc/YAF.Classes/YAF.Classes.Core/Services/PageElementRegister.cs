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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	public class PageElementRegister
	{
		private readonly List<string> _registeredElements = new List<string>();
		/// <summary>
		/// Elements (using in the head or header) that are registered on the page.
		/// Used mostly by RegisterPageElementHelper.
		/// </summary>
		public List<string> RegisteredElements
		{
			get
			{
				return _registeredElements;
			}
		}

		public bool PageElementExists(string name)
		{
			return _registeredElements.Contains(name.ToLower());
		}

		public void AddPageElement( string name )
		{
			_registeredElements.Add( name.ToLower() );
		}

		/// <summary>
		/// Adds the given CSS to the page header within a <![CDATA[<style>]]> tag
		/// </summary>
		/// <param name="element">Control to add element</param>
		/// <param name="name">Name of this element so it is not added more then once (not case sensitive)</param>
		/// <param name="cssContents">The contents of the text/css style block</param>
		public void RegisterCssBlock(Control element, string name, string cssContents)
		{
			if (!PageElementExists( name ))
			{
				// Add to the end of the controls collection
				element.Controls.Add(ControlHelper.MakeCssControl(cssContents));
				AddPageElement( name );
			}
		}

		/// <summary>
		/// Registers a Javascript block using the script manager. Adds script tags.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="script"></param>
		public void RegisterJsBlock(string name, string script )
		{
			RegisterJsBlock( YafContext.Current.CurrentForumPage, name, script );
		}

		/// <summary>
		/// Registers a Javascript block using the script manager. Adds script tags.
		/// </summary>
		/// <param name="thisControl"></param>
		/// <param name="name"></param>
		/// <param name="script"></param>
		public void RegisterJsBlock( Control thisControl, string name, string script )
		{
			if ( !PageElementExists( name ) )
			{
				ScriptManager.RegisterClientScriptBlock( thisControl, thisControl.GetType(), name,
				                                         JsAndCssHelper.CompressJavaScript( script ), true );
			}
		}

		/// <summary>
		/// Registers a Javascript block using the script manager. Adds script tags.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="script"></param>
		public void RegisterJsBlockStartup( string name, string script )
		{
			RegisterJsBlockStartup( YafContext.Current.CurrentForumPage, name, script );
		}

		/// <summary>
		/// Registers a Javascript block using the script manager. Adds script tags.
		/// </summary>
		/// <param name="thisControl"></param>
		/// <param name="name"></param>
		/// <param name="script"></param>
		public void RegisterJsBlockStartup( Control thisControl, string name, string script )
		{
			if ( !PageElementExists( name ) )
			{
				ScriptManager.RegisterStartupScript( thisControl, thisControl.GetType(), name,
				                                     JsAndCssHelper.CompressJavaScript( script ), true );
			}
		}

		/// <summary>
		/// Registers a Javascript include using the script manager.
		/// </summary>
		/// <param name="thisControl"></param>
		/// <param name="name"></param>
		/// <param name="url"></param>
		public void RegisterJsInclude( Control thisControl, string name, string url )
		{
			if ( !PageElementExists( name ) )
			{
				ScriptManager.RegisterClientScriptInclude( thisControl, thisControl.GetType(), name, url );
				AddPageElement( name );
			}
		}

		/// <summary>
		/// Registers a Javascript include using the script manager.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="url"></param>
		public void RegisterJsInclude( string name, string url )
		{
			RegisterJsInclude( YafContext.Current.CurrentForumPage, name, url );
		}

		/// <summary>
		/// Registers a Javascript resource include using the script manager.
		/// </summary>
		/// <param name="thisControl"></param>
		/// <param name="name"></param>
		/// <param name="relativeResourceUrl"></param>
		public void RegisterJsResourceInclude( Control thisControl, string name, string relativeResourceUrl )
		{
			if ( !PageElementExists( name ) )
			{
				ScriptManager.RegisterClientScriptInclude( thisControl, thisControl.GetType(), name,
				                                           YafForumInfo.GetURLToResource( relativeResourceUrl ) );
				AddPageElement( name );
			}
		}

		/// <summary>
		/// Registers a Javascript resource include using the script manager.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="relativeResourceUrl"></param>
		public void RegisterJsResourceInclude( string name, string relativeResourceUrl )
		{
			RegisterJsResourceInclude( YafContext.Current.CurrentForumPage, name, relativeResourceUrl );
		}

		public void RegisterCssBlock(string name, string cssContents)
		{
			RegisterCssBlock( YafContext.Current.CurrentForumPage.TopPageControl, name, JsAndCssHelper.CompressCss( cssContents ) );
		}

		/// <summary>
		/// Add the given CSS to the page header within a style tag
		/// </summary>
		/// <param name="cssUrl">Url of the CSS file to add</param>
		public void RegisterCssInclude(string cssUrl)
		{
			RegisterCssInclude(YafContext.Current.CurrentForumPage.TopPageControl, cssUrl);
		}

		/// <summary>
		/// Add the given CSS to the page header within a style tag
		/// </summary>
		/// <param name="cssUrlResource">Url of the CSS Resource file to add</param>
		public void RegisterCssIncludeResource( string cssUrlResource )
		{
			RegisterCssInclude( YafContext.Current.CurrentForumPage.TopPageControl,
			                    YafForumInfo.GetURLToResource( cssUrlResource ) );
		}

		/// <summary>
		/// Adds the given CSS to the page header within a <![CDATA[<style>]]> tag
		/// </summary>
		/// <param name="element">Control to add element</param>
		/// <param name="cssUrl">Url of the CSS file to add</param>
		public void RegisterCssInclude(Control element, string cssUrl)
		{
			if (!PageElementExists(cssUrl))
			{
				element.Controls.Add( ControlHelper.MakeCssIncludeControl( cssUrl.ToLower() ) );
				AddPageElement(cssUrl);
			}
		}

		public void RegisterJQuery()
		{
			RegisterJQuery( YafContext.Current.CurrentForumPage.TopPageControl );
		}

		public void RegisterJQueryUI()
		{
			RegisterJQueryUI( YafContext.Current.CurrentForumPage.TopPageControl );
		}

		/// <summary>
		/// 
		/// </summary>
		public void RegisterJQuery( Control element )
		{
			if (!PageElementExists("jquery"))
			{
				bool registerJQuery = true;

				string key = "JQuery-Javascripts";
				// check to see if DotNetAge is around and has registered jQuery for us...
				if ( HttpContext.Current.Items[key] != null )
				{
					var collection = (HttpContext.Current.Items[key] as System.Collections.Specialized.StringCollection);

					if ( collection != null && collection.Contains( "jquery" ) )
					{
						registerJQuery = false;
					}
				}
				else if ( Config.IsDotNetNuke )
				{
					// latest version of DNN (v5) should register jQuery for us...
					registerJQuery = false;
				}

				// load jQuery from google...
				//const string jQueryLoad = "<script type=\"text/javascript\" src=\"\"></script>";
				if ( registerJQuery )
				{
					element.Controls.Add(
						ControlHelper.MakeJsIncludeControl( "http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js" ) );
				}

				AddPageElement( "jquery" );
			}
		}

		public void RegisterJQueryUI( Control element )
		{
			if ( !PageElementExists( "jqueryui" ) )
			{
				// requires jQuery first...
				if ( !PageElementExists( "jquery" ) ) RegisterJQuery( element );

				// load jQuery UI from google...
				element.Controls.Add(
					ControlHelper.MakeJsIncludeControl( "http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js" ) );
				AddPageElement( "jqueryui" );
			}
		}
	}
}
