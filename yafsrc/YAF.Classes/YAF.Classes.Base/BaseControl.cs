using System;
using System.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseControl : System.Web.UI.Control
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
		}

		public yaf_Context PageContext
		{
			get 
			{
				return yaf_Context.Current;
			}
		}
	}
}
