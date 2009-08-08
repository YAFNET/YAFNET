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
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for bannedip.
	/// </summary>
	public partial class bannedip : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Banned IP Addresses", "" );

				BindData();
			}
		}

		private void BindData()
		{
			list.DataSource = YAF.Classes.Data.DB.bannedip_list( PageContext.PageBoardID, null );
			DataBind();
		}

		protected void list_ItemCommand( object sender, RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "add" )
				YafBuildLink.Redirect( ForumPages.admin_bannedip_edit );
			else if ( e.CommandName == "edit" )
				YafBuildLink.Redirect( ForumPages.admin_bannedip_edit, "i={0}", e.CommandArgument );
			else if ( e.CommandName == "delete" )
			{
				YAF.Classes.Data.DB.bannedip_delete( e.CommandArgument );

				// clear cache of banned IPs for this board
				YafCache.Current.Remove( YafCache.GetBoardCacheKey( Constants.Cache.BannedIP ) );

				BindData();
				PageContext.AddLoadMessage( "Removed IP address ban." );
			}
		}
	}
}
