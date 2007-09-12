using System;
using System.Data;
using System.Web.UI;
using YAF.Classes.Utils;
using YAF.Classes.UI;
using YAF.Classes.Data;

namespace YAF.Controls
{
	[ParseChildren(false)]
	public class TopicLine : BaseControl
	{
		private DataRowView	_row = null;
		private bool _isAlt;
		
		public bool IsAlt
		{
			get { return this._isAlt; }
			set { this._isAlt = value; }
		}		

		public object DataRow
		{
			set 
			{
				_row = (DataRowView)value;
			}
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{	
			System.Text.StringBuilder html = new System.Text.StringBuilder(2000);

			html.AppendFormat("<tr class=\"{0}\">",(IsAlt ? "post_alt" : "post"));

			// Icon
			string imgTitle = "", img = GetTopicImage(_row,ref imgTitle);
			html.AppendFormat( @"<td><img title=""{1}"" src=""{0}""></td>", img, imgTitle );
			// Topic
			html.AppendFormat( @"<td><span class=""post_priority"">{0}</span>", GetPriorityMessage( _row ) );
			if(FindUnread)
				html.AppendFormat( @"<a href=""{0}"" class=""post_link"">{1}</a>", YafBuildLink.GetLink( ForumPages.posts, "t={0}&find=unread", _row ["LinkTopicID"] ), General.BadWordReplace( Convert.ToString( _row ["Subject"] ) ) );
			else
				html.AppendFormat( @"<a href=""{0}"" class=""post_link"">{1}</a>", YafBuildLink.GetLink( ForumPages.posts, "t={0}", _row ["LinkTopicID"] ), General.BadWordReplace( Convert.ToString( _row ["Subject"] ) ) );

			string tPager = CreatePostPager(Convert.ToInt32(_row["Replies"])+1,PageContext.BoardSettings.PostsPerPage,Convert.ToInt32(_row["LinkTopicID"]));
			if (tPager != String.Empty)
			{
				// more then one page to show
				html.AppendFormat( @"<br/><span class=""smallfont"">{0}</span>", String.Format( PageContext.Localization.GetText( "GOTO_POST_PAGER" ), tPager ) );
			}

			//html.AppendFormat("<br/><span class='smallfont'>{0}: {1}</span>",PageContext.Localization.GetText("TOPICS","CREATED"),PageContext.FormatDateShort(m_row["Posted"]));
			
			html.Append("</td>");
			// Topic Starter
			html.AppendFormat( @"<td><a href=""{0}"">{1}</a></td>", YafBuildLink.GetLink( ForumPages.profile, "u={0}", _row ["UserID"] ), BBCode.EncodeHTML( _row ["Starter"].ToString() ) );
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
            int nViews = Convert.ToInt32(_row["Views"]);
            return ((_row["TopicMovedID"].ToString().Length > 0)) ? "&nbsp;" : String.Format("{0:N0}", nViews);
        }

		protected string GetTopicImage(object o,ref string imgTitle) 
		{
			DataRowView	row			= (DataRowView)o;
			DateTime	lastPosted	= row["LastPosted"]!=DBNull.Value ? (DateTime)row["LastPosted"] : new DateTime(2000,1,1);
			bool		isLocked	= General.BinaryAnd(row["TopicFlags"], TopicFlags.Locked);
			
			imgTitle = "???";

			try 
			{
				bool bIsLocked = isLocked || General.BinaryAnd(row["ForumFlags"], ForumFlags.Locked);

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
			int nReplies = Convert.ToInt32(_row["Replies"]);
			return (nReplies < 0) ? "&nbsp;" : String.Format("{0:N0}",nReplies);
		}

		/// <summary>
		/// Formats the Last Post for the Topic Line
		/// </summary>
		/// <returns>Formatted Last Post Text</returns>
		protected string FormatLastPost() 
		{
			string strReturn = PageContext.Localization.GetText("no_posts");
			DataRowView row = _row;
			
			if (row["LastMessageID"].ToString().Length>0) 
			{
				string strMiniPost = PageContext.Theme.GetItem("ICONS",(DateTime.Parse(row["LastPosted"].ToString()) > Mession.GetTopicRead((int)_row["TopicID"])) ? "ICON_NEWEST" : "ICON_LATEST");

				string strBy =
					String.Format(PageContext.Localization.GetText("by"),String.Format("<a href=\"{0}\">{1}</a>&nbsp;<a title=\"{4}\" href=\"{3}\"><img border=0 src='{2}'></a>",
					YafBuildLink.GetLink( ForumPages.profile,"u={0}",row["LastUserID"]), 
					BBCode.EncodeHTML(row["LastUserName"].ToString()), 
					strMiniPost, 
					YafBuildLink.GetLink( ForumPages.posts,"m={0}#{0}",row["LastMessageID"]),
					PageContext.Localization.GetText("GO_LAST_POST")
					));

				strReturn =
					String.Format("{0}<br />{1}", 
					YafDateTime.FormatDateTimeTopic(Convert.ToDateTime(row["LastPosted"])),
					strBy);
			} 

			return strReturn;			
		}

		/// <summary>
		/// Create pager for post.
		/// </summary>
		/// 
		protected string CreatePostPager(int count,int pageSize,int topicID)
		{
			string strReturn = "";

			int NumToDisplay = 4;
			int PageCount = (int)Math.Ceiling((double)count/pageSize);

			if (PageCount > 1)
			{
				if (PageCount > NumToDisplay)
				{
					strReturn += MakeLink("1",YafBuildLink.GetLink( ForumPages.posts,"t={0}",topicID));
					strReturn += " ... ";
					bool bFirst = true;

					// show links from the end
					for (int i=(PageCount-(NumToDisplay-1));i<PageCount;i++)
					{
						int iPost = i+1;

						if (bFirst) bFirst = false;
						else strReturn += ", ";

						strReturn += MakeLink(iPost.ToString(),YafBuildLink.GetLink( ForumPages.posts,"t={0}&p={1}",topicID,iPost));
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
						
            strReturn += MakeLink(iPost.ToString(),YafBuildLink.GetLink( ForumPages.posts,"t={0}&p={1}",topicID,iPost));
					}
				}
			}
			return strReturn;
		}

		private string MakeLink(string text,string link)
		{
			return String.Format("<a href=\"{0}\">{1}</a>",link,text); 
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
