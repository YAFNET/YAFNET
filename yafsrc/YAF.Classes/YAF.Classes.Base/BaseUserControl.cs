/* Yet Another Forum.NET
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
using System.Data;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using YAF.Classes.Utils;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseUserControl : System.Web.UI.UserControl
	{
		public YafContext PageContext
		{
			get
			{
				return YafContext.Current;
			}
		}				

		public string HtmlEncode( object data )
		{
			return HttpContext.Current.Server.HtmlEncode( data.ToString() );
		}

		#region ViewStateID Helper Code
		private Dictionary<string, long> _viewStateIdDictionary = null;
		protected Dictionary<string, long> ViewStateIDDictionary
		{
			get
			{
				if ( _viewStateIdDictionary == null )
				{
					if ( ViewState ["ViewStateIDDictionary"] != null )
					{
						_viewStateIdDictionary = ViewState ["ViewStateIDDictionary"] as Dictionary<string, long>;
					}
				}

				if ( _viewStateIdDictionary == null )
				{
					_viewStateIdDictionary = new Dictionary<string, long>();
				}

				return _viewStateIdDictionary;
			}
		}

		protected void InitViewStateIDs( string [] idNames )
		{
			InitViewStateIDs( idNames, false );
		}

		protected void InitViewStateIDs( string [] idNames, bool failOnInvalid )
		{
			bool [] failInvalid = new bool [idNames.Length];

			for( int i=0;i<failInvalid.Length;i++)
			{
				failInvalid[i] = failOnInvalid;
			}

			InitViewStateIDs( idNames, failInvalid );
		}

		protected void InitViewStateIDs( string [] idNames, bool [] failOnInvalid )
		{
			if ( idNames.Length != failOnInvalid.Length)
			{
				throw new Exception("idNames and failOnValid variables must be the same array length.");
			}

			for( int i=0;i<idNames.Length;i++)
			{
				if ( !ViewStateIDDictionary.ContainsKey( idNames[i] ) )
				{
					long idConverted = -1;

					if ( !String.IsNullOrEmpty( Request.QueryString [idNames [i]] ) && long.TryParse( Request.QueryString [idNames [i]], out idConverted ) )
					{
						ViewStateIDDictionary.Add( idNames [i], idConverted );
					}
					else
					{
						// fail, see if it should be valid...
						if ( failOnInvalid [i] )
						{
							// redirect to invalid id information...
							YafBuildLink.Redirect( ForumPages.info, "i=6" );
						}
					}
				}
			}		
		}
		#endregion
	}
}
