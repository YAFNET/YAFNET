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
			bool bTopic = MyPage.PageTopicID>0;
			try 
			{
				if(dt==null) 
				{
					if(bTopic)
						dt = DB.active_listtopic(MyPage.PageTopicID);
					else
						dt = DB.active_listforum(MyPage.PageForumID);
					ViewState["data"] = dt;
				}
	
				if(bTopic) 
				{
					writer.WriteLine("<tr class=\"header2\">");
					writer.WriteLine(String.Format("<td colspan=\"2\">{0}</td>",MyPage.GetText("TOPICBROWSERS")));
					writer.WriteLine("</tr>");
					writer.WriteLine("<tr class=\"post\">");
					writer.WriteLine("<td colspan=\"2\">");
				} 
				else 
				{
					writer.WriteLine("<tr class=\"header2\">");
					writer.WriteLine(String.Format("<td colspan=\"6\">{0}</td>",MyPage.GetText("FORUMUSERS")));
					writer.WriteLine("</tr>");
					writer.WriteLine("<tr class=\"post\">");
					writer.WriteLine("<td colspan=\"6\">");
				}

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
