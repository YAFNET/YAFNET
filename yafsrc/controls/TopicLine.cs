using System;
using System.Data;
using System.Web.UI;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	[ParseChildren(false)]
	public class TopicLine : YAF.Classes.Base.BaseControl
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
				html.AppendFormat( @"<a href=""{0}"" class=""post_link"">{1}</a>", yaf_BuildLink.GetLink( ForumPages.posts, "t={0}&find=unread", m_row ["LinkTopicID"] ), General.BadWordReplace( Convert.ToString( m_row ["Subject"] ) ) );
			else
				html.AppendFormat( @"<a href=""{0}"" class=""post_link"">{1}</a>", yaf_BuildLink.GetLink( ForumPages.posts, "t={0}", m_row ["LinkTopicID"] ), General.BadWordReplace( Convert.ToString( m_row ["Subject"] ) ) );

			string tPager = CreatePostPager(Convert.ToInt32(m_row["Replies"])+1,PageContext.BoardSettings.PostsPerPage,Convert.ToInt32(m_row["LinkTopicID"]));
			if (tPager != String.Empty)
			{
				// more then one page to show
				html.AppendFormat( @"<br/><span class=""smallfont"">{0}</span>", String.Format( PageContext.Localization.GetText( "GOTO_POST_PAGER" ), tPager ) );
			}

			//html.AppendFormat("<br/><span class='smallfont'>{0}: {1}</span>",PageContext.Localization.GetText("TOPICS","CREATED"),PageContext.FormatDateShort(m_row["Posted"]));
			
			html.Append("</td>");
			// Topic Starter
			html.AppendFormat( @"<td><a href=""{0}"">{1}</a></td>", yaf_BuildLink.GetLink( ForumPages.profile, "u={0}", m_row ["UserID"] ), BBCode.EncodeHTML( m_row ["Starter"].ToString() ) );
			// Replies
			html.AppendFormat( @"<td align=""center"">{0}</td>", FormatReplies() );
			// Views
            html.AppendFormat(@"<td align=""center"">{0:N0}</td>", FormatViews());
			// Last Post
			html.AppendFormat( @"<td align=""center"" class=""smallfont"">{0}</td>", FormatLastPost() );
			writer.Write(html.ToString());

			base.RenderChildren(writer);

			writer.Write("</tr>");
		}

        private string FormatViews()
        {
            int nViews = Convert.ToInt32(m_row["Views"]);
            return ((m_row["TopicMovedID"].ToString().Length > 0)) ? "&nbsp;" : String.Format("{0:N0}", nViews);
        }

		protected string GetTopicImage(object o,ref string imgTitle) 
		{
			DataRowView	row			= (DataRowView)o;
			DateTime	lastPosted	= row["LastPosted"]!=DBNull.Value ? (DateTime)row["LastPosted"] : new DateTime(2000,1,1);
			bool		isLocked	= ((int)row["TopicFlags"] & (int)YAF.Classes.Data.TopicFlags.Locked) == (int)YAF.Classes.Data.TopicFlags.Locked;
			
			imgTitle = "???";

			try 
			{
				bool bIsLocked = isLocked || ((int)row["ForumFlags"] & (int)YAF.Classes.Data.ForumFlags.Locked)==(int)YAF.Classes.Data.ForumFlags.Locked;

				if(row["TopicMovedID"].ToString().Length>0)
				{
					imgTitle = PageContext.Localization.GetText( "MOVED" );
					return PageContext.Theme.GetItem( "ICONS", "TOPIC_MOVED" );
				}

				DateTime lastRead = Mession.GetTopicRead((int)row["TopicID"]);
				DateTime lastReadForum = Mession.GetForumRead( ( int ) row ["ForumID"] );
				if(lastReadForum>lastRead) lastRead = lastReadForum;

				if(lastPosted > lastRead) 
				{
					Mession.UnreadTopics++;

					if(row["PollID"]!=DBNull.Value) 
					{
						imgTitle = PageContext.Localization.GetText("POLL_NEW");
						return PageContext.Theme.GetItem("ICONS","TOPIC_POLL_NEW");
					}
					else if(row["Priority"].ToString() == "1")
					{
						imgTitle = PageContext.Localization.GetText("STICKY");
						return PageContext.Theme.GetItem("ICONS","TOPIC_STICKY");
					}
					else if(row["Priority"].ToString() == "2")
					{
						imgTitle = PageContext.Localization.GetText("ANNOUNCEMENT");
						return PageContext.Theme.GetItem("ICONS","TOPIC_ANNOUNCEMENT_NEW");
					}
					else if(bIsLocked)
					{
						imgTitle = PageContext.Localization.GetText("NEW_POSTS_LOCKED");
						return PageContext.Theme.GetItem("ICONS","TOPIC_NEW_LOCKED");
					}
					else
					{
						imgTitle = PageContext.Localization.GetText("NEW_POSTS");
						return PageContext.Theme.GetItem("ICONS","TOPIC_NEW");
					}
				}
				else 
				{
					if(row["PollID"]!=DBNull.Value)
					{
						imgTitle = PageContext.Localization.GetText("POLL");
						return PageContext.Theme.GetItem("ICONS","TOPIC_POLL");
					}
					else if(row["Priority"].ToString() == "1")
					{
						imgTitle = PageContext.Localization.GetText("STICKY");
						return PageContext.Theme.GetItem("ICONS","TOPIC_STICKY");
					}
					else if(row["Priority"].ToString() == "2")
					{
						imgTitle = PageContext.Localization.GetText("ANNOUNCEMENT");
						return PageContext.Theme.GetItem("ICONS","TOPIC_ANNOUNCEMENT");
					}
					else if(bIsLocked)
					{
						imgTitle = PageContext.Localization.GetText("NO_NEW_POSTS_LOCKED");
						return PageContext.Theme.GetItem("ICONS","TOPIC_LOCKED");
					}
					else
					{
						imgTitle = PageContext.Localization.GetText("NO_NEW_POSTS");
						return PageContext.Theme.GetItem("ICONS","TOPIC");
					}
				}
			}
			catch(Exception) 
			{
				return PageContext.Theme.GetItem("ICONS","TOPIC");
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
				strReturn = PageContext.Localization.GetText("MOVED");
			}
			else if (row["PollID"].ToString() != "")
			{
				strReturn = PageContext.Localization.GetText("POLL");
			}
			else switch(int.Parse(row["Priority"].ToString())) 
					 {
						 case 1: strReturn = PageContext.Localization.GetText("STICKY"); break;
						 case 2: strReturn = PageContext.Localization.GetText("ANNOUNCEMENT"); break;
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
			string strReturn = PageContext.Localization.GetText("no_posts");
			DataRowView row = m_row;
			
			if (row["LastMessageID"].ToString().Length>0) 
			{
				string strMiniPost = PageContext.Theme.GetItem("ICONS",(DateTime.Parse(row["LastPosted"].ToString()) > Mession.GetTopicRead((int)m_row["TopicID"])) ? "ICON_NEWEST" : "ICON_LATEST");

				string strBy =
					String.Format(PageContext.Localization.GetText("by"),String.Format("<a href=\"{0}\">{1}</a>&nbsp;<a title=\"{4}\" href=\"{3}\"><img border=0 src='{2}'></a>",
					yaf_BuildLink.GetLink( ForumPages.profile,"u={0}",row["LastUserID"]), 
					BBCode.EncodeHTML(row["LastUserName"].ToString()), 
					strMiniPost, 
					yaf_BuildLink.GetLink( ForumPages.posts,"m={0}#{0}",row["LastMessageID"]),
					PageContext.Localization.GetText("GO_LAST_POST")
					));

				strReturn =
					String.Format("{0}<br />{1}", 
					yaf_DateTime.FormatDateTimeTopic(Convert.ToDateTime(row["LastPosted"])),
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
					strReturn += MakeLink("1",yaf_BuildLink.GetLink( ForumPages.posts,"t={0}",TopicID));
					strReturn += " ... ";
					bool bFirst = true;

					// show links from the end
					for (int i=(PageCount-(NumToDisplay-1));i<PageCount;i++)
					{
						int iPost = i+1;

						if (bFirst) bFirst = false;
						else strReturn += ", ";

						strReturn += MakeLink(iPost.ToString(),yaf_BuildLink.GetLink( ForumPages.posts,"t={0}&p={1}",TopicID,iPost));
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
						
            strReturn += MakeLink(iPost.ToString(),yaf_BuildLink.GetLink( ForumPages.posts,"t={0}&p={1}",TopicID,iPost));
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
