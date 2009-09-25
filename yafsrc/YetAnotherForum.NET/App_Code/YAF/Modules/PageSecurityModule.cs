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
	/// Module that handles individual page security features -- needs to be expanded.
	/// </summary>
	[YafModule( "Page Security Module", "Tiny Gecko", 1 )]
	public class PageSecurityModule : SimpleBaseModule
	{
		public PageSecurityModule()
		{
			
		}

		public override void InitBeforePage()
		{
			PageContext.PagePreLoad += new EventHandler<EventArgs>( CurrentForumPage_PreLoad );
		}

		void CurrentForumPage_PreLoad( object sender, EventArgs e )
		{
			// no security features for login/logout pages
			if (ForumPageType == ForumPages.login || ForumPageType == ForumPages.approve || ForumPageType == ForumPages.logout)
				return;

			// check if it's a "registered user only page" and check permissions.
			if (CurrentForumPage.IsRegisteredPage && CurrentForumPage.User == null)
			{
				CurrentForumPage.RedirectNoAccess();
			}

			// not totally necessary... but provides another layer of protection...
			if (CurrentForumPage.IsAdminPage && !PageContext.IsAdmin)
			{
				YafBuildLink.AccessDenied();
				return;
			}

			// handle security features...
			switch (ForumPageType)
			{
				case ForumPages.recoverpassword:
					if ( PageContext.BoardSettings.DisableRegistrations )
						YafBuildLink.AccessDenied();
					break;
				default:
					if (PageContext.IsPrivate && CurrentForumPage.User == null)
					{
						// register users only...
						CurrentForumPage.RedirectNoAccess();
					}
					break;
			}
		}
	}
}