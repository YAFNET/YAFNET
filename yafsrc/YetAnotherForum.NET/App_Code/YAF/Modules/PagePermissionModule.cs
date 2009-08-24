/* YetAnotherForum.NET
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
using System.Web;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Module that handles page permission feature
	/// </summary>
	[YafModule( "Page Permission Module", "Tiny Gecko", 1 )]
	public class PagePermissionModule : SimpleBaseModule
	{
		public PagePermissionModule()
		{

		}

		override public void InitAfterPage()
		{
			CurrentForumPage.Load += new EventHandler(CurrentPage_Load);
		}

		void CurrentPage_Load(object sender, EventArgs e)
		{
			// check access permissions for specific pages...
			switch (ForumPageType)
			{
				case ForumPages.activeusers:
					YafServices.Permissions.HandleRequest(PageContext.BoardSettings.ActiveUsersViewPermissions);
					break;
				case ForumPages.members:
					YafServices.Permissions.HandleRequest(PageContext.BoardSettings.MembersListViewPermissions);
					break;
				case ForumPages.profile:
					YafServices.Permissions.HandleRequest(PageContext.BoardSettings.ProfileViewPermissions);
					break;
				case ForumPages.search:
					YafServices.Permissions.HandleRequest(PageContext.BoardSettings.SearchPermissions);
					break;
				default:
					break;
			}
		}
	}
}