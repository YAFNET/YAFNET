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
		public yaf_Context PageContext
		{
			get
			{
				return yaf_Context.Current;
			}
		}
	}
}
