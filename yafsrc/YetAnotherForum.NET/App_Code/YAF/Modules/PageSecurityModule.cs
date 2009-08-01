using System;
using System.Collections.Generic;
using System.Web;
using YAF.Classes.Base;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Module that handles individual page security features -- needs to be expanded.
	/// </summary>
	public class PageSecurityModule : SimpleBaseModule
	{
		public PageSecurityModule()
		{
			
		}

		override public void InitAfterPage()
		{
			CurrentForumPage.Load += new EventHandler(CurrentPage_Load);
		}

		void CurrentPage_Load(object sender, EventArgs e)
		{
			// no security features for login/logout pages
			if (ForumPageType == ForumPages.login || ForumPageType == ForumPages.logout)
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
				case ForumPages.moderate_index:
					

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