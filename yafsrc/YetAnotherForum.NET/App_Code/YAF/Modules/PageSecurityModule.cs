using System;
using System.Collections.Generic;
using System.Web;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	/// <summary>
	/// Module that handles individual page security features -- needs to be expanded.
	/// </summary>
	public class PageSecurityModule : HelperBaseModule
	{
		public PageSecurityModule()
		{
			
		}

		public override void InitModule()
		{
			// hook forumpage handlers....
			ForumPage.Load += new EventHandler(CurrentPage_Load);
		}

		void CurrentPage_Load(object sender, EventArgs e)
		{
			// no security features for login/logout pages
			if (ForumPageType == ForumPages.login || ForumPageType == ForumPages.logout)
				return;

			// check if it's a "registered user only page" and check permissions.
			if (ForumPage.IsRegisteredPage && ForumPage.User == null)
			{
				ForumPage.RedirectNoAccess();
			}

			// not totally necessary... but provides another layer of protection...
			if (ForumPage.IsAdminPage && !PageContext.IsAdmin)
			{
				YafBuildLink.AccessDenied();
				return;
			}

			// handle security features...
			switch (ForumPageType)
			{
				case ForumPages.moderate_index:
					

				default:
					if (PageContext.IsPrivate && ForumPage.User == null)
					{
						// register users only...
						ForumPage.RedirectNoAccess();
					}
					break;
			}
		}
	}
}