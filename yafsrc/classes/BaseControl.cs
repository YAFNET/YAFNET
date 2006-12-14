using System;
using System.Data;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseControl : System.Web.UI.Control
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
		}

		public YAF.Pages.ForumPage ForumPage
		{
			get 
			{
				System.Web.UI.Control ctl = Parent;
				System.Web.UI.Control thePage = this;
				while(ctl.GetType()!=typeof(YAF.Forum))
				{
					thePage = ctl;
					ctl = ctl.Parent;
				}

				return (YAF.Pages.ForumPage)thePage;
			}
		}
	}
}
