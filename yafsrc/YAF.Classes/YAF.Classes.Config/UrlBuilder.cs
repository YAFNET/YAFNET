/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web;

namespace YAF.Classes
{
	/// <summary>
	/// Implements URL Builder.
	/// </summary>
	public class UrlBuilder : IUrlBuilder
	{
		/// <summary>
		/// Builds URL for calling page with parameter URL as page's escaped parameter.
		/// </summary>
		/// <param name="url">URL to put into parameter.</param>
		/// <returns>URL to calling page with URL argument as page's parameter with escaped characters to make it valid parameter.</returns>
		public string BuildUrl(string url)
		{
			// escape & to &amp;
			url = url.Replace( "&", "&amp;" );

			// return URL to current script with URL from parameter as script's parameter
			return String.Format( "{0}?{1}", HttpContext.Current.Request.ServerVariables ["SCRIPT_NAME"], url );
		}
	}
}
