using System;
using System.Text;

namespace yaf
{
	/// <summary>
	/// Summary description for ForumHeader.
	/// </summary>
	public class ForumHeader : System.Web.UI.UserControl
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("<img src=\"images/yaflogo.jpg\" width=\"400\" height=\"50\" /><br />");

			writer.Write(sb.ToString());
			base.Render (writer);
		}

	}
}
