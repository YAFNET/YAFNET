using System;
using System.Data;

namespace yaf
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseControl : System.Web.UI.Control
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
		}

		public new BasePage Page 
		{
			get 
			{
				return (BasePage)base.Page;
			}
		}
	}
}
