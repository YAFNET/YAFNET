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
using System.Collections.Specialized;
using System.Text;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Helps parse URLs
	/// </summary>
	public class SimpleURLParameterParser
	{
		private string _urlParameters = "";
		private string _urlAnchor = "";
		private NameValueCollection _nameValues = new NameValueCollection();

		public SimpleURLParameterParser( string urlParameters )
		{
			_urlParameters = urlParameters;
			ParseURLParameters();
		}

		private void ParseURLParameters()
		{
			string urlTemp = _urlParameters;
			int index;

			// get the url end anchor (#blah) if there is one...
			_urlAnchor = "";
			index = urlTemp.LastIndexOf( '#' );

			if ( index > 0 )
			{
				// there's an anchor
				_urlAnchor = urlTemp.Substring( index + 1 );
				// remove the anchor from the url...
				urlTemp = urlTemp.Remove( index );
			}

			_nameValues.Clear();
			string [] arrayPairs = urlTemp.Split( new char [] { '&' } );

			foreach ( string tValue in arrayPairs )
			{
				if ( tValue.Trim().Length > 0 )
				{
					// parse...
					string [] nvalue = tValue.Trim().Split( new char [] { '=' } );
					if ( nvalue.Length == 1 )
						_nameValues.Add( nvalue [0], string.Empty );
					else if ( nvalue.Length > 1 )
						_nameValues.Add( nvalue [0], nvalue [1] );
				}
			}
		}

		public string CreateQueryString( string [] excludeValues )
		{
			string queryString = "";
			bool bFirst = true;

			for ( int i = 0; i < _nameValues.Count; i++ )
			{
				string key = _nameValues.Keys [i].ToLower();
				string value = _nameValues [i];
				if ( !KeyInsideArray( excludeValues, key ) )
				{
					if ( bFirst ) bFirst = false;
					else queryString += "&";
					queryString += key + "=" + value;
				}
			}

			return queryString;
		}

		private bool KeyInsideArray( string [] array, string key )
		{
			foreach ( string tmp in array )
			{
				if ( tmp.Equals( key ) ) return true;
			}
			return false;
		}

		public string Anchor
		{
			get
			{
				return _urlAnchor;
			}
		}

		public bool HasAnchor
		{
			get
			{
				return ( _urlAnchor != "" );
			}
		}

		public NameValueCollection Parameters
		{
			get
			{
				return _nameValues;
			}
		}

		public int Count
		{
			get
			{
				return _nameValues.Count;
			}
		}

		public string this [string name]
		{
			get
			{
				return _nameValues [name];
			}
		}

		public string this [int index]
		{
			get
			{
				return _nameValues [index];
			}
		}
	}
}
