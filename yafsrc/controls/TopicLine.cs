using System;
using System.Data;
using System.Web.UI;

namespace yaf.controls
{
	[ParseChildren(false)]
	public class TopicLine : BaseControl
	{
		private DataRowView	m_row = null;
		private bool m_isAlt;
		
		public bool IsAlt
		{
			get { return this.m_isAlt; }
			set { this.m_isAlt = value; }
		}		

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

			html.AppendFormat("<tr class=\"{0}\">",(IsAlt ? "post_alt" : "post"));

			// Icon
			string imgTitle = "", img = GetTopicImage(m_row,ref imgTitle);
			html.AppendFormat( @"<td><img title=""{1}"" src=""{0}""></td>", img, imgTitle );
			// Topic
			html.AppendFormat( @"<td><span class=""post_priority"">{0}</span>", GetPriorityMessage( m_row ) );
			if(FindUnread)
				html.AppendFormat( @"<a href=""{0}"" class=""post_link"">{1}</a>", Forum.GetLink( Pages.posts, "t={0}&find=unread", m_row ["LinkTopicID"] ), Utils.BadWordReplace( Convert.ToString( m_row ["Subject"] ) ) );
			else
				html.AppendFormat( @"<a href=""{0}"" class=""post_link"">{1}</a>", Forum.GetLink( Pages.posts, "t={0}", m_row ["LinkTopicID"] ), Utils.BadWordReplace( Convert.ToString( m_row ["Subject"] ) ) );

			string tPager = CreatePostPager(Convert.ToInt32(m_row["Replies"])+1,ForumPage.BoardSettings.PostsPerPage,Convert.ToInt32(m_row["LinkTopicID"]));
			if (tPager != String.Empty)
			{
				// more then one page to show
				html.AppendFormat( @"<br/><span class=""smallfont"">{0}</span>", String.Format( ForumPage.GetText( "GOTO_POST_PAGER" ), tPager ) );
			}

			//html.AppendFormat("<br/><span class='smallfont'>{0}: {1}</span>",ForumPage.GetText("TOPICS","CREATED"),ForumPage.FormatDateShort(m_row["Posted"]));
			
			html.Append("</td>");
			// Topic Starter
			html.AppendFormat( @"<td><a href=""{0}"">{1}</a></td>", Forum.GetLink( Pages.profile, "u={0}", m_row ["UserID"] ), BBCode.EncodeHTML( m_row ["Starter"].ToString() ) );
			// Replies
			html.AppendFormat( @"<td align=""center"">{0}</td>", FormatReplies() );
			// Views
			html.AppendFormat( @"<td align=""center"">{0:N0}</td>", m_row ["Views"] );
			// Last Post
			html.AppendFormat( @"<td align=""center"" class=""smallfont"">{0}</td>", FormatLastPost() );
			writer.Write(html.ToString());

			base.RenderChildren(writer);

			writer.Write("</tr>");
		}

		protected string GetTopicImage(object o,ref string imgTitle) 
		{
			DataRowView	row			= (DataRowView)o;
			DateTime	lastPosted	= row["LastPosted"]!=DBNull.Value ? (DateTime)row["LastPosted"] : new DateTime(2000,1,1);
			bool		isLocked	= ((int)row["TopicFlags"] & (int)TopicFlags.Locked) == (int)TopicFlags.Locked;
			
			imgTitle = "???";

			try 
			{
				bool bIsLocked = isLocked || ((int)row["ForumFlags"] & (int)ForumFlags.Locked)==(int)ForumFlags.Locked;

				if(row["TopicMovedID"].ToString().Length>0)
				{
					imgTitle = ForumPage.GetText("MOVED");
					return ForumPage.GetThemeContents("ICONS","TOPIC_MOVED");
				}

				DateTime lastRead = ForumPage.GetTopicRead((int)row["TopicID"]);
				DateTime lastReadForum = ForumPage.GetForumRead((int)row["ForumID"]);
				if(lastReadForum>lastRead) lastRead = lastReadForum;

				if(lastPosted > lastRead) 
				{
					Mession.UnreadTopics++;

					if(row["PollID"]!=DBNull.Value) 
					{
						imgTitle = ForumPage.GetText("POLL_NEW");
						return ForumPage.GetThemeContents("ICONS","TOPIC_POLL_NEW");
					}
					else if(row["Priority"].ToString() == "1")
					{
						imgTitle = ForumPage.GetText("STICKY");
						return ForumPage.GetThemeContents("ICONS","TOPIC_STICKY");
					}
					else if(row["Priority"].ToString() == "2")
					{
						imgTitle = ForumPage.GetText("ANNOUNCEMENT");
						return ForumPage.GetThemeContents("ICONS","TOPIC_ANNOUNCEMENT_NEW");
					}
					else if(bIsLocked)
					{
						imgTitle = ForumPage.GetText("NEW_POSTS_LOCKED");
						return ForumPage.GetThemeContents("ICONS","TOPIC_NEW_LOCKED");
					}
					else
					{
						imgTitle = ForumPage.GetText("NEW_POSTS");
						return ForumPage.GetThemeContents("ICONS","TOPIC_NEW");
					}
				}
				else 
				{
					if(row["PollID"]!=DBNull.Value)
					{
						imgTitle = ForumPage.GetText("POLL");
						return ForumPage.GetThemeContents("ICONS","TOPIC_POLL");
					}
					else if(row["Priority"].ToString() == "1")
					{
						imgTitle = ForumPage.GetText("STICKY");
						return ForumPage.GetThemeContents("ICONS","TOPIC_STICKY");
					}
					else if(row["Priority"].ToString() == "2")
					{
						imgTitle = ForumPage.GetText("ANNOUNCEMENT");
						return ForumPage.GetThemeContents("ICONS","TOPIC_ANNOUNCEMENT");
					}
					else if(bIsLocked)
					{
						imgTitle = ForumPage.GetText("NO_NEW_POSTS_LOCKED");
						return ForumPage.GetThemeContents("ICONS","TOPIC_LOCKED");
					}
					else
					{
						imgTitle = ForumPage.GetText("NO_NEW_POSTS");
						return ForumPage.GetThemeContents("ICONS","TOPIC");
					}
				}
			}
			catch(Exception) 
			{
				return ForumPage.GetThemeContents("ICONS","TOPIC");
			}
		}
		/// <summary>
		/// Creates the status message text for a topic. (i.e. Moved, Poll, Sticky, etc.)
		/// </summary>
		/// <param name="row">Current Topic Data Row</param>
		/// <returns>Topic status text</returns>
		protected string GetPriorityMessage(DataRowView row) 
		{
			string strReturn = "";

			if (row["TopicMovedID"].ToString().Length > 0)
			{
				strReturn = ForumPage.GetText("MOVED");
			}
			else if (row["PollID"].ToString() != "")
			{
				strReturn = ForumPage.GetText("POLL");
			}
			else switch(int.Parse(row["Priority"].ToString())) 
					 {
						 case 1: strReturn = ForumPage.GetText("STICKY"); break;
						 case 2: strReturn = ForumPage.GetText("ANNOUNCEMENT"); break;
					 }

			if (strReturn.Length > 0) strReturn = String.Format("[ {0} ] ",strReturn);

			return strReturn;
		}
		/// <summary>
		/// Formats replies number for Topic Line
		/// </summary>
		/// <returns>"&nbsp;" if no replies or the number of replies.</returns>
		protected string FormatReplies() 
		{
			int nReplies = Convert.ToInt32(m_row["Replies"]);
			return (nReplies < 0) ? "&nbsp;" : String.Format("{0:N0}",nReplies);
		}

		/// <summary>
		/// Formats the Last Post for the Topic Line
		/// </summary>
		/// <returns>Formatted Last Post Text</returns>
		protected string FormatLastPost() 
		{
			string strReturn = ForumPage.GetText("no_posts");
			DataRowView row = m_row;
			
			if (row["LastMessageID"].ToString().Length>0) 
			{
				string strMiniPost = ForumPage.GetThemeContents("ICONS",(DateTime.Parse(row["LastPosted"].ToString()) > ForumPage.GetTopicRead((int)m_row["TopicID"])) ? "ICON_NEWEST" : "ICON_LATEST");

				string strBy =
					String.Format(ForumPage.GetText("by"),String.Format("<a href=\"{0}\">{1}</a>&nbsp;<a title=\"{4}\" href=\"{3}\"><img border=0 src='{2}'></a>",
					Forum.GetLink(Pages.profile,"u={0}",row["LastUserID"]), 
					BBCode.EncodeHTML(row["LastUserName"].ToString()), 
					strMiniPost, 
					Forum.GetLink(Pages.posts,"m={0}#{0}",row["LastMessageID"]),
					ForumPage.GetText("GO_LAST_POST")
					));

				strReturn =
					String.Format("{0}<br />{1}", 
					ForumPage.FormatDateTimeTopic(Convert.ToDateTime(row["LastPosted"])),
					strBy);
			} 

			return strReturn;			
		}

		/// <summary>
		/// Create pager for post.
		/// </summary>
		/// 
		protected string CreatePostPager(int Count,int PageSize,int TopicID)
		{
			string strReturn = "";

			int NumToDisplay = 4;
			int PageCount = (int)Math.Ceiling((double)Count/PageSize);

			if (PageCount > 1)
			{
				if (PageCount > NumToDisplay)
				{
					strReturn += MakeLink("1",Forum.GetLink(Pages.posts,"t={0}",TopicID));
					strReturn += " ... ";
					bool bFirst = true;

					// show links from the end
					for (int i=(PageCount-(NumToDisplay-1));i<PageCount;i++)
					{
						int iPost = i+1;

						if (bFirst) bFirst = false;
						else strReturn += ", ";

						strReturn += MakeLink(iPost.ToString(),Forum.GetLink(Pages.posts,"t={0}&p={1}",TopicID,iPost));
					}
				}
				else
				{
					bool bFirst = true;
					for (int i=0;i<PageCount;i++)
					{
						int iPost = i+1;

						if (bFirst) bFirst = false;
						else strReturn += ", ";
						
            strReturn += MakeLink(iPost.ToString(),Forum.GetLink(Pages.posts,"t={0}&p={1}",TopicID,iPost));
					}
				}
			}
			return strReturn;
		}

		private string MakeLink(string Text,string Link)
		{
			return String.Format("<a href=\"{0}\">{1}</a>",Link,Text); 
		}

		public bool FindUnread
		{
			set
			{
				ViewState["FindUnread"] = value;
			}
			get
			{
				return (ViewState["FindUnread"] != null) ? Convert.ToBoolean(ViewState["FindUnread"]) : false;
			}
		}
	}
}
