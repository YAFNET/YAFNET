using System;
using System.Data;
using System.Web.UI;

namespace yaf.controls
{
	[ParseChildren(false)]
	public class TopicLine : BaseControl
	{
		private DataRowView	m_row = null;

		public object DataRow
		{
			set 
			{
				m_row = (DataRowView)value;
			}
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{	
			System.Text.StringBuilder html = new System.Text.StringBuilder(2000);

			html.Append("<tr class='post'>");
			// Icon
			string imgTitle = "", img = GetTopicImage(m_row,ref imgTitle);
			html.AppendFormat("<td><img title='{1}' src='{0}'></td>",img,imgTitle);
			// Topic
			html.AppendFormat("<td>{0}",GetPriorityMessage(m_row));
			html.AppendFormat("<a href='posts.aspx?t={0}'>{1}</a>",m_row["LinkTopicID"],m_row["Subject"]);
			html.AppendFormat("<br/><span class='smallfont'>{0}: {1}</span>",Page.GetText("TOPICS","CREATED"),Page.FormatDateShort(m_row["Posted"]));
			html.Append("</td>");
			// Topic Starter
			html.AppendFormat("<td><a href='profile.aspx?u={0}'>{1}</a></td>",m_row["UserID"],m_row["Starter"]);
			// Replies
			html.AppendFormat("<td align=center>{0}</td>",FormatReplies());
			// Views
			html.AppendFormat("<td align=center>{0:N0}</td>",m_row["Views"]);
			// Last Post
			html.AppendFormat("<td align=center class=smallfont>{0}</td>",FormatLastPost());
			writer.Write(html.ToString());

			base.RenderChildren(writer);

			writer.Write("</tr>");
		}

		protected string GetTopicImage(object o,ref string imgTitle) 
		{
			DataRowView	row			= (DataRowView)o;
			DateTime	lastPosted	= (DateTime)row["LastPosted"];
			bool		isLocked	= (bool)row["IsLocked"];
			
			imgTitle = "???";

			try 
			{
				bool bIsLocked = isLocked || (bool)m_row["ForumLocked"];

				if(row["TopicMovedID"].ToString().Length>0)
					return Page.GetThemeContents("ICONS","TOPIC_MOVED");

				DateTime lastRead = Page.GetTopicRead((int)row["TopicID"]);
				DateTime lastReadForum = Page.GetForumRead((int)row["ForumID"]);
				if(lastReadForum>lastRead) lastRead = lastReadForum;

				if(lastPosted > lastRead) 
				{
					Page.Session["unreadtopics"] = 1 + (int)Page.Session["unreadtopics"];

					if(row["PollID"]!=DBNull.Value) 
					{
						imgTitle = Page.GetText("POLL_NEW");
						return Page.GetThemeContents("ICONS","TOPIC_POLL_NEW");
					}
					else if(row["Priority"].ToString() == "1")
					{
						imgTitle = Page.GetText("STICKY");
						return Page.GetThemeContents("ICONS","TOPIC_STICKY");
					}
					else if(row["Priority"].ToString() == "2")
					{
						imgTitle = Page.GetText("ANNOUNCEMENT");
						return Page.GetThemeContents("ICONS","TOPIC_ANNOUNCEMENT_NEW");
					}
					else if(bIsLocked)
					{
						imgTitle = Page.GetText("NEW_POSTS_LOCKED");
						return Page.GetThemeContents("ICONS","TOPIC_NEW_LOCKED");
					}
					else
					{
						imgTitle = Page.GetText("NEW_POSTS");
						return Page.GetThemeContents("ICONS","TOPIC_NEW");
					}
				}
				else 
				{
					if(row["PollID"]!=DBNull.Value)
					{
						imgTitle = Page.GetText("POLL");
						return Page.GetThemeContents("ICONS","TOPIC_POLL");
					}
					else if(row["Priority"].ToString() == "1")
					{
						imgTitle = Page.GetText("STICKY");
						return Page.GetThemeContents("ICONS","TOPIC_STICKY");
					}
					else if(row["Priority"].ToString() == "2")
					{
						imgTitle = Page.GetText("ANNOUNCEMENT");
						return Page.GetThemeContents("ICONS","TOPIC_ANNOUNCEMENT");
					}
					else if(bIsLocked)
					{
						imgTitle = Page.GetText("NO_NEW_POSTS_LOCKED");
						return Page.GetThemeContents("ICONS","TOPIC_LOCKED");
					}
					else
					{
						imgTitle = Page.GetText("NO_NEW_POSTS");
						return Page.GetThemeContents("ICONS","TOPIC");
					}
				}
			}
			catch(Exception) 
			{
				return Page.GetThemeContents("ICONS","TOPIC");
			}
		}
		protected string GetPriorityMessage(DataRowView row) 
		{
			if(row["TopicMovedID"].ToString().Length>0)
				return "[ Moved ] ";

			if(row["PollID"].ToString()!="")
				return "[ Poll ] ";

			switch(int.Parse(row["Priority"].ToString())) 
			{
				case 1:
					return "[ Sticky ] ";
				case 2:
					return "[ Announcement ] ";
				default:
					return "";
			}
		}
		protected string FormatReplies() 
		{
			int nReplies = (int)m_row["Replies"];
			if(nReplies<0)
				return "&nbsp;";
			else
				return String.Format("{0:N0}",nReplies);
		}
		protected string FormatLastPost() 
		{
			DataRowView row = m_row;
			if(row["LastMessageID"].ToString().Length>0) 
			{
				string minipost;
				if(DateTime.Parse(row["LastPosted"].ToString()) > (DateTime)Page.Session["lastvisit"])
					minipost = Page.GetThemeContents("ICONS","ICON_NEWEST");
				else
					minipost = Page.GetThemeContents("ICONS","ICON_LATEST");
				
				string by = String.Format(Page.GetText("by"),String.Format("<a href=\"profile.aspx?u={0}\">{1}</a>&nbsp;<a title=\"{4}\" href=\"posts.aspx?m={3}#{3}\"><img border=0 src='{2}'></a>",
					row["LastUserID"], 
					row["LastUserName"], 
					minipost, 
					row["LastMessageID"],
					Page.GetText("GO_LAST_POST")
					));
				return String.Format("{0}<br />{1}", 
					Page.FormatDateTime((DateTime)row["LastPosted"]),
					by
					);
			} 
			else
				return Page.GetText("no_posts");
		}
	}
}
