using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class ForumUsers : BaseControl
	{
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			DataTable dt = (DataTable)ViewState["data"];
			try 
			{
				if(dt==null) 
				{
					dt = DB.active_listforum(Page.PageForumID);
					ViewState["data"] = dt;
				}
	
				writer.WriteLine("<tr class=\"header2\">");
				writer.WriteLine(String.Format("<td colspan=\"6\">{0}</td>",Page.GetText("FORUMUSERS")));
				writer.WriteLine("</tr>");
				writer.WriteLine("<tr class=\"post\">");
				writer.WriteLine("<td colspan=\"6\">");

				bool bFirst = true;
				foreach(DataRow dr in dt.Rows) 
				{
					if(!bFirst) 
					{
						writer.WriteLine(",");
					} 
					else 
					{
						bFirst = false;
					}
					writer.Write(String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>",dr["UserID"],dr["UserName"]));
				}
				writer.WriteLine("</td>");
				writer.WriteLine("</tr>");
			}
			finally 
			{
				if(dt!=null) dt.Dispose();
			}
		}
	}
}
