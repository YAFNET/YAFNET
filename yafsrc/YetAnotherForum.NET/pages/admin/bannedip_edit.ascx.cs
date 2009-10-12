/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for bannedip_edit.
	/// </summary>
	public partial class bannedip_edit : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Banned IP Addresses", YafBuildLink.GetLink( ForumPages.admin_bannedip ) );

				BindData();
			}
		}

		private void BindData()
		{
			if ( Request.QueryString ["i"] != null )
			{
				DataRow row = YAF.Classes.Data.DB.bannedip_list( PageContext.PageBoardID, Request.QueryString ["i"] ).Rows [0];
				mask.Text = ( string ) row ["Mask"];
			}
		}

		protected void Save_Click( object sender, EventArgs e )
		{
			String [] ipParts = mask.Text.Trim().Split( '.' );

			// do some validation...
			string ipError = "";

			if ( ipParts.Length != 4 )
			{
				ipError += "Invalid IP address.";
			}

			// see if they are numbers...
			ulong number;

			foreach ( string ip in ipParts )
			{
				if ( !ulong.TryParse( ip, out number ) )
				{
					if ( ip.Trim() != "*" )
					{
						if ( ip.Trim().Length == 0 )
						{
							ipError += "\r\nOne of the IP section does not have a value. Valid values are 0-255 or \"*\" for a wildcard.";
						}
						else
						{
							ipError += String.Format( "\r\n\"{0}\" is not a valid IP section value.", ip );
						}
						break;
					}
				}
				else
				{
					// try parse succeeded... verify number amount...
					if ( number > 255 )
					{
						ipError += String.Format( "\r\n\"{0}\" is not a valid IP section value (must be less then 255).", ip );
					}
				}
			}

			// show error(s) if not valid...
			if ( !String.IsNullOrEmpty( ipError )  )
			{
				PageContext.AddLoadMessage( ipError );
				return;
			}

			YAF.Classes.Data.DB.bannedip_save( Request.QueryString ["i"], PageContext.PageBoardID, mask.Text.Trim() );

			// clear cache of banned IPs for this board
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.BannedIP ) );

			// go back to banned IP's administration page
			YafBuildLink.Redirect( ForumPages.admin_bannedip );
		}

		protected void Cancel_Click( object sender, EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_bannedip );
		}
	}
}
