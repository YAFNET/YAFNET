using System;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Summary description for AdminPage.
	/// </summary>
	public class AdminPage : ForumPage
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
			if ( !PageContext.IsAdmin )
				yaf_BuildLink.AccessDenied();

#if false
			if(!IsPostBack)
			{
				controls.PageLinks ctl = new controls.PageLinks();
				ctl.AddLink(BoardSettings.Name,YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				ctl.AddLink("Administration",YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				Controls.AddAt(0,ctl);
			}
#endif
		}
	}
}
