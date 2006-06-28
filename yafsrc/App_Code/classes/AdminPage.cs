using System;

namespace yaf
{
	/// <summary>
	/// Summary description for AdminPage.
	/// </summary>
	public class AdminPage : pages.ForumPage
	{
		/// <summary>
		/// Creates the Administration page.
		/// </summary>
		public AdminPage() : base(null)
		{
			this.Load += new EventHandler(AdminPage_Load);
		}

		private void AdminPage_Load(object sender,EventArgs e)
		{
			if(!IsAdmin)
				Data.AccessDenied();

#if false
			if(!IsPostBack)
			{
				controls.PageLinks ctl = new controls.PageLinks();
				ctl.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				ctl.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				Controls.AddAt(0,ctl);
			}
#endif
		}
	}
}
