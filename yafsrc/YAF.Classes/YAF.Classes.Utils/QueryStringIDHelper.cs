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
using System.Web.Security;

namespace YAF.Classes.Utils
{
	public class QueryStringIDHelper
	{
		private Dictionary<string, long> _idDictionary = null;
		public Dictionary<string, long> Params
		{
			get
			{
				if ( _idDictionary == null )
				{
					_idDictionary = new Dictionary<string, long>();
				}

				return _idDictionary;
			}
		}

		public long this [string idName]
		{
			get
			{
				if ( Params.ContainsKey( idName ) )
				{
					return Params[idName];
				}

				return -1;
			}
		}

		public bool ContainsKey( string idName )
		{
			return Params.ContainsKey( idName );
		}

		/// <summary>
		/// False to ErrorOnInvalid
		/// </summary>
		/// <param name="idName"></param>
		public QueryStringIDHelper( string idName )
			: this( idName, false )
		{

		}

		/// <summary>
		/// False on ErrorOnInvalid
		/// </summary>
		/// <param name="idNames"></param>
		public QueryStringIDHelper( string [] idNames )
			: this( idNames, false )
		{

		}

		public QueryStringIDHelper( string idName, bool errorOnInvalid )
		{
			this.InitIDs( new string [] { idName }, new bool [] { errorOnInvalid } );
		}

		public QueryStringIDHelper( string [] idNames, bool errorOnInvalid )
		{
			bool [] failInvalid = new bool [idNames.Length];

			for ( int i = 0; i < failInvalid.Length; i++ )
			{
				failInvalid [i] = errorOnInvalid;
			}

			this.InitIDs( idNames, failInvalid );
		}

		public QueryStringIDHelper( string [] idNames, bool [] errorOnInvalid )
		{
			this.InitIDs( idNames, errorOnInvalid );
		}

		private void InitIDs( string [] idNames, bool [] errorOnInvalid )
		{
			if ( idNames.Length != errorOnInvalid.Length )
			{
				throw new Exception( "idNames and errorOnInvalid variables must be the same array length." );
			}

			for ( int i = 0; i < idNames.Length; i++ )
			{
				if ( !Params.ContainsKey( idNames [i] ) )
				{
					long idConverted = -1;

					if ( !String.IsNullOrEmpty( HttpContext.Current.Request.QueryString [idNames [i]] ) && long.TryParse( HttpContext.Current.Request.QueryString [idNames [i]], out idConverted ) )
					{
						Params.Add( idNames [i], idConverted );
					}
					else
					{
						// fail, see if it should be valid...
						if ( errorOnInvalid [i] )
						{
							// redirect to invalid id information...
							YafBuildLink.Redirect( ForumPages.info, "i=6" );
						}
					}
				}
			}
		}
	}
}
