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
using System.Web;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for restartapp.
	/// </summary>
	public partial class restartapp : YAF.Classes.Core.AdminPage
	{
		private long _lastVersion;
		private DateTime _lastVersionDate;

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Restart Application", "" );
			}

			DataBind();
		}

		protected void RestartApp_Click( object sender, EventArgs e )
		{
			if ( General.GetCurrentTrustLevel() == AspNetHostingPermissionLevel.High )
			{
				System.Web.HttpRuntime.UnloadAppDomain();
			}
			else
			{
				PageContext.LoadMessage.Add( "Must have High Trust to Unload Application. Restart Failed." );
			}
		}
	}
}

