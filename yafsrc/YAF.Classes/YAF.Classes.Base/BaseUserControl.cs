using System;
using System.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class BaseUserControl : System.Web.UI.UserControl
	{
		public YafContext PageContext
		{
			get
			{
				return YafContext.Current;
			}
		}
	}
}
