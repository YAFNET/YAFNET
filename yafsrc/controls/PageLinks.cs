using System;
using System.Data;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for PageLinks.
	/// </summary>
	public class PageLinks : BaseControl
	{
		public void AddLink(string title,string url) 
		{
			DataTable dt = (DataTable)ViewState["data"];
			if ( dt == null ) 
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

		/// <summary>
		/// Clear all Links
		/// </summary>
		public void Clear()
		{
			DataTable dt = (DataTable)ViewState["data"];
			if ( dt != null )
			{
				ViewState ["data"] = null;
			}
		}

		public void AddForumLinks(int forumID)
		{
			this.AddForumLinks(forumID,false);
		}

		public void AddForumLinks(int forumID, bool noForumLink)
		{
			using(DataTable dtLinks=YAF.Classes.Data.DB.forum_listpath(forumID))
			{
				foreach(DataRow row in dtLinks.Rows)				
				{
					if (noForumLink && Convert.ToInt32(row["ForumID"]) == forumID)
						AddLink(row["Name"].ToString(),"");
					else
						AddLink(row["Name"].ToString(),Forum.GetLink( ForumPages.topics,"f={0}",row["ForumID"]));
				}
			}
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
			
			if (m_links == null || m_links.Rows.Count == 0) return;

			writer.WriteLine("<p class=\"navlinks\">");

			bool bFirst = true;
			foreach(DataRow row in m_links.Rows)
			{
				if(!bFirst) 
					writer.WriteLine("&#187;"); 
				else 
					bFirst = false;

				if (row["URL"].ToString().Equals(""))
					writer.WriteLine("<span id=\"current\">" + row["Title"].ToString() + "</span>");
				else
					writer.WriteLine(String.Format("<a href=\"{0}\">{1}</a>",row["URL"],row["Title"]));
			}
			
			writer.WriteLine("</p>");
		}
	}
}
