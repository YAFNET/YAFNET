using System;
using System.Data;

namespace yaf.controls
{
	/// <summary>
	/// Summary description for PageLinks.
	/// </summary>
	public class PageLinks : BaseControl
	{
		public void AddLink(string title,string url) 
		{
			DataTable dt = (DataTable)ViewState["data"];
			if(dt==null) 
			{
				dt = new DataTable();
				dt.Columns.Add("Title",typeof(string));
				dt.Columns.Add("URL",typeof(string));
				ViewState["data"] = dt;
			}
			DataRow dr = dt.NewRow();
			dr["Title"] = title;
			dr["URL"] = url;
			dt.Rows.Add(dr);
		}

		public void AddForumLinks(int forumID)
		{
			using(DataTable dtLinks=DB.forum_listpath(forumID))
				foreach(DataRow row in dtLinks.Rows)
					AddLink((string)row["Name"],Forum.GetLink(Pages.topics,"f={0}",row["ForumID"]));
		}

		private void Page_Load(object sender, System.EventArgs e) 
		{
		}

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			base.OnInit(e);
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			DataTable m_links = (DataTable)ViewState["data"];
			if(m_links==null || m_links.Rows.Count==0)
				return;

			writer.WriteLine("<p class='navlinks'>");

			bool bFirst = true;
			foreach(DataRow row in m_links.Rows)
			{
				if(!bFirst) 
				{
					writer.WriteLine("&#187;");
				} 
				else 
				{
					bFirst = false;
				}
				writer.WriteLine(String.Format("<a href='{0}'>{1}</a>",row["URL"],row["Title"]));
			}
			
			writer.WriteLine("</p>");
		}
	}
}
