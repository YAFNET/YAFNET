using System;
using System.Data;

namespace yaf
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseUserControl : System.Web.UI.UserControl
	{
		public yaf.pages.BasePage MyPage
		{
			get 
			{
				System.Web.UI.Control ctl = Parent;
				System.Web.UI.Control thePage = this;
				while(ctl.GetType()!=typeof(yaf.Forum))
				{
					thePage = ctl;
					ctl = ctl.Parent;
				}

				return (yaf.pages.BasePage)thePage;
			}
		}
	}
}
