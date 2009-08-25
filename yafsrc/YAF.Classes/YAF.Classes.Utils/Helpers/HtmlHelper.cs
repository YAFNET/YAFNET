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
using System.Text.RegularExpressions;
using System.Web;

namespace YAF.Classes.Utils
{
	static public class HtmlHelper
	{
		static public string StripHtml( string text )
		{
			return Regex.Replace( text, @"<(.|\n)*?>", string.Empty );
		}

		/// <summary>
		/// Validates an html tag against the allowedTags. Also check that
		/// it doesn't have any "extra" features such as javascript in it.
		/// </summary>
		/// <param name="tag"></param>
		/// <param name="allowedTags"></param>
		/// <returns></returns>
		static public bool IsValidTag( string tag, string[] allowedTags )
		{
			if ( tag.IndexOf( "javascript" ) >= 0 ) return false;
			if ( tag.IndexOf( "vbscript" ) >= 0 ) return false;
			if ( tag.IndexOf( "onclick" ) >= 0 ) return false;

			char[] endchars = new char[] { ' ', '>', '/', '\t' };

			int pos = tag.IndexOfAny( endchars, 1 );
			if ( pos > 0 ) tag = tag.Substring( 0, pos );
			if ( tag[0] == '/' ) tag = tag.Substring( 1 );

			// check if it's a valid tag
			foreach ( string aTag in allowedTags )
			{
				if ( tag == aTag ) return true;
			}

			return false;
		}
	}
}
