/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using System;
using System.Data;
using System.Web.UI;
using YAF.Classes.Utils;
using YAF.Classes.UI;
using YAF.Classes.Data;

namespace YAF.Controls
{
	[ParseChildren( false )]
	public class TopicLine : BaseControl
	{
		private DataRowView _row = null;
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
				_row = ( DataRowView ) value;
			}
		}

		protected void WriteBeginTD(System.Web.UI.HtmlTextWriter writer, string classId)
		{
			writer.WriteBeginTag("td");
			if ( !String.IsNullOrEmpty( classId ))
			{
				writer.WriteAttribute( "class", classId );
			}
			writer.Write(HtmlTextWriter.TagRightChar);		
		}

		protected void WriteBeginTD( System.Web.UI.HtmlTextWriter writer )
		{
			this.WriteBeginTD( writer, null );
		}

		protected void WriteEndTD( System.Web.UI.HtmlTextWriter writer )
		{			
			writer.WriteEndTag( "td" );
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder( 2000 );

			writer.WriteBeginTag( "tr" );
			writer.WriteAttribute("class", (IsAlt ? "topicRow_Alt post_alt" : "topicRow post"));
			writer.Write( HtmlTextWriter.TagRightChar );

			// Icon
			string imgTitle = "";
			string imgSrc = GetTopicImage( _row, ref imgTitle );

			WriteBeginTD( writer );
			RenderImgTag( writer, imgSrc, imgTitle, imgTitle );
			WriteEndTD( writer );

			// Topic
			WriteBeginTD( writer, "topicMain" );

			string priorityMessage = GetPriorityMessage( _row );
			if ( !String.IsNullOrEmpty( priorityMessage ) )
			{
				writer.WriteBeginTag( "span" );
				writer.WriteAttribute( "class", "post_priority" );
				writer.Write( HtmlTextWriter.TagRightChar );
				writer.Write( priorityMessage );
				writer.WriteEndTag( "span" );
			}

			string linkParams = "t={0}";
			if ( FindUnread ) linkParams += "&find=unread";

			string firstMessage = null;

			if (_row ["FirstMessage"] != DBNull.Value)
			{
				firstMessage = 	_row ["FirstMessage"].ToString();
			}

            RenderAnchorBegin( writer, YafBuildLink.GetLink( ForumPages.posts, linkParams, _row ["LinkTopicID"] ), "post_link", General.Truncate( General.BadWordReplace( firstMessage), 255 ) );

            writer.WriteLine( General.BadWordReplace( Convert.ToString( _row ["Subject"] ) ) );
			writer.WriteEndTag( "a" );

			int actualPostCount = Convert.ToInt32( _row ["Replies"] ) + 1;

			if ( PageContext.BoardSettings.ShowDeletedMessages )
			{
				// add deleted posts not included in replies...
				actualPostCount += Convert.ToInt32( _row ["NumPostsDeleted"] );
			}

			string tPager = CreatePostPager( actualPostCount, PageContext.BoardSettings.PostsPerPage, Convert.ToInt32( _row ["LinkTopicID"] ) );

			if ( tPager != String.Empty )
			{
				writer.WriteLine();
				writer.WriteBreak();
				writer.WriteBeginTag( "span" );
				writer.WriteAttribute( "class", "topicPager smallfont" );
				writer.Write( HtmlTextWriter.TagRightChar );
				// more then one page to show
				writer.Write( String.Format( PageContext.Localization.GetText( "GOTO_POST_PAGER" ), tPager ) );
				writer.WriteEndTag( "span" );
				writer.WriteLine();
			}

			WriteEndTD( writer );

			// Topic Starter
			UserLink topicStarterLink = new UserLink();
			topicStarterLink.ID = GetUniqueID( "topicStarterLink" );
			topicStarterLink.UserID = Convert.ToInt32( _row ["UserID"] );
			topicStarterLink.UserName = _row ["Starter"].ToString();

			// render the user link control
			WriteBeginTD(writer, "topicStarter");
			topicStarterLink.RenderControl( writer );
			WriteEndTD( writer );

			// Replies
			writer.WriteBeginTag( "td" );
			writer.WriteAttribute("class", "topicReplies");
			writer.WriteAttribute( "style", "text-align: center" );
			writer.Write( HtmlTextWriter.TagRightChar );
			writer.Write( FormatReplies() );
			writer.WriteEndTag( "td" );

			// Views
			writer.WriteBeginTag( "td" );
			writer.WriteAttribute("class", "topicViews");
			writer.WriteAttribute( "style", "text-align: center" );
			writer.Write( HtmlTextWriter.TagRightChar );
			writer.Write( FormatViews() );
			writer.WriteEndTag( "td" );
		
			// Last Post
			writer.WriteBeginTag( "td" );
			writer.WriteAttribute("class", "topicLastPost");
			writer.WriteAttribute( "style", "text-align: center" );
			writer.WriteAttribute( "class", "smallfont" );
			writer.Write( HtmlTextWriter.TagRightChar );
			RenderLastPost( writer );
			writer.WriteEndTag( "td" );

			base.RenderChildren( writer );

			writer.WriteEndTag( "tr" );
			writer.WriteLine();
		}

		private string FormatViews()
		{
			int nViews = Convert.ToInt32( _row ["Views"] );
			return ( ( _row ["TopicMovedID"].ToString().Length > 0 ) ) ? "&nbsp;" : String.Format( "{0:N0}", nViews );
		}

		protected string GetTopicImage( object o, ref string imgTitle )
		{
			DataRowView row = ( DataRowView ) o;
			DateTime lastPosted = row ["LastPosted"] != DBNull.Value ? ( DateTime ) row ["LastPosted"] : new DateTime( 2000, 1, 1 );
			TopicFlags topicFlags = new TopicFlags(row["TopicFlags"]);
			ForumFlags forumFlags = new ForumFlags(row["ForumFlags"]);
			// Obsolette : Ederon
			//bool isLocked = General.BinaryAnd(row["TopicFlags"], TopicFlags.Locked);

			imgTitle = "???";

			try
			{
				// Obsolette : Ederon
				//bool bIsLocked = isLocked || General.BinaryAnd( row ["ForumFlags"], ForumFlags.Locked );

				if ( row ["TopicMovedID"].ToString().Length > 0 )
				{
					imgTitle = PageContext.Localization.GetText( "MOVED" );
					return PageContext.Theme.GetItem( "ICONS", "TOPIC_MOVED" );
				}

				DateTime lastRead = Mession.GetTopicRead( ( int ) row ["TopicID"] );
				DateTime lastReadForum = Mession.GetForumRead( ( int ) row ["ForumID"] );
				if ( lastReadForum > lastRead ) lastRead = lastReadForum;

				if ( lastPosted > lastRead )
				{
					Mession.UnreadTopics++;

					if ( row ["PollID"] != DBNull.Value )
					{
						imgTitle = PageContext.Localization.GetText( "POLL_NEW" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_POLL_NEW" );
					}
					else if ( row ["Priority"].ToString() == "1" )
					{
						imgTitle = PageContext.Localization.GetText( "STICKY" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_STICKY" );
					}
					else if ( row ["Priority"].ToString() == "2" )
					{
						imgTitle = PageContext.Localization.GetText( "ANNOUNCEMENT" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_ANNOUNCEMENT_NEW" );
					}
					else if ( topicFlags.IsLocked || forumFlags.IsLocked )
					{
						imgTitle = PageContext.Localization.GetText( "NEW_POSTS_LOCKED" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_NEW_LOCKED" );
					}
					else
					{
						imgTitle = PageContext.Localization.GetText( "NEW_POSTS" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_NEW" );
					}
				}
				else
				{
					if ( row ["PollID"] != DBNull.Value )
					{
						imgTitle = PageContext.Localization.GetText( "POLL" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_POLL" );
					}
					else if ( row ["Priority"].ToString() == "1" )
					{
						imgTitle = PageContext.Localization.GetText( "STICKY" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_STICKY" );
					}
					else if ( row ["Priority"].ToString() == "2" )
					{
						imgTitle = PageContext.Localization.GetText( "ANNOUNCEMENT" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_ANNOUNCEMENT" );
					}
					else if (topicFlags.IsLocked || forumFlags.IsLocked)
					{
						imgTitle = PageContext.Localization.GetText( "NO_NEW_POSTS_LOCKED" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC_LOCKED" );
					}
					else
					{
						imgTitle = PageContext.Localization.GetText( "NO_NEW_POSTS" );
						return PageContext.Theme.GetItem( "ICONS", "TOPIC" );
					}
				}
			}
			catch ( Exception )
			{
				return PageContext.Theme.GetItem( "ICONS", "TOPIC" );
			}
		}
		/// <summary>
		/// Creates the status message text for a topic. (i.e. Moved, Poll, Sticky, etc.)
		/// </summary>
		/// <param name="row">Current Topic Data Row</param>
		/// <returns>Topic status text</returns>
		protected string GetPriorityMessage( DataRowView row )
		{
			string strReturn = "";

			if ( row ["TopicMovedID"].ToString().Length > 0 )
			{
				strReturn = PageContext.Localization.GetText( "MOVED" );
			}
			else if ( row ["PollID"].ToString() != "" )
			{
				strReturn = PageContext.Localization.GetText( "POLL" );
			}
			else switch ( int.Parse( row ["Priority"].ToString() ) )
				{
					case 1: strReturn = PageContext.Localization.GetText( "STICKY" ); break;
					case 2: strReturn = PageContext.Localization.GetText( "ANNOUNCEMENT" ); break;
				}

			if ( strReturn.Length > 0 ) strReturn = String.Format( "[ {0} ] ", strReturn );

			return strReturn;
		}
		/// <summary>
		/// Formats replies number for Topic Line
		/// </summary>
		/// <returns>"&nbsp;" if no replies or the number of replies.</returns>
		protected string FormatReplies()
		{
			string repStr = "&nbsp;";

			int nReplies = Convert.ToInt32( _row ["Replies"] );
			int numDeleted = Convert.ToInt32( _row ["NumPostsDeleted"] );

			if ( nReplies >= 0 )
			{
				if ( PageContext.BoardSettings.ShowDeletedMessages && numDeleted > 0 )
				{
					repStr = String.Format( "{0:N0}", nReplies + numDeleted );
				}
				else
				{
					repStr = String.Format( "{0:N0}", nReplies );
				}
			}

			return repStr;
		}

		/// <summary>
		/// Formats the Last Post for the Topic Line
		/// </summary>		
		protected string RenderLastPost( System.Web.UI.HtmlTextWriter writer  )
		{
			string strReturn = PageContext.Localization.GetText( "no_posts" );
			DataRowView row = _row;

			if ( row ["LastMessageID"].ToString().Length > 0 )
			{
				string strMiniPost = PageContext.Theme.GetItem( "ICONS", ( DateTime.Parse( row ["LastPosted"].ToString() ) > Mession.GetTopicRead( ( int ) _row ["TopicID"] ) ) ? "ICON_NEWEST" : "ICON_LATEST" );

				writer.Write( YafDateTime.FormatDateTimeTopic( Convert.ToDateTime( row ["LastPosted"] ) ) );
				writer.WriteBreak();
				writer.Write( String.Format( PageContext.Localization.GetText( "by" ), string.Empty ) );

				UserLink byLink = new UserLink();
				byLink.UserID = Convert.ToInt32( row ["LastUserID"] );
				byLink.UserName = row ["LastUserName"].ToString();
				
				byLink.RenderControl( writer );

				writer.Write( "&nbsp;" );

				writer.WriteBeginTag( "a" );
				writer.WriteAttribute( "href", YafBuildLink.GetLink( ForumPages.posts, "m={0}#post{0}", row ["LastMessageID"] ) );
				writer.WriteAttribute( "title", PageContext.Localization.GetText( "GO_LAST_POST" ) );
				writer.Write( HtmlTextWriter.TagRightChar );
				
				RenderImgTag( writer, strMiniPost, PageContext.Localization.GetText( "GO_LAST_POST" ), PageContext.Localization.GetText( "GO_LAST_POST" ));

				writer.WriteEndTag( "a" );
			}

			return strReturn;
		}

		/// <summary>
		/// Create pager for post.
		/// </summary>
		/// 
		protected string CreatePostPager( int count, int pageSize,int topicID )
		{
			string strReturn = "";

			int NumToDisplay = 4;
			int PageCount = ( int ) Math.Ceiling( ( double ) count / pageSize );

			if ( PageCount > 1 )
			{
				if ( PageCount > NumToDisplay )
				{
					strReturn += MakeLink( "1", YafBuildLink.GetLink( ForumPages.posts, "t={0}", topicID ) );
					strReturn += " ... ";
					bool bFirst = true;

					// show links from the end
					for ( int i = ( PageCount - ( NumToDisplay - 1 ) ); i < PageCount; i++ )
					{
						int iPost = i + 1;

						if ( bFirst ) bFirst = false;
						else strReturn += ", ";

						strReturn += MakeLink( iPost.ToString(), YafBuildLink.GetLink( ForumPages.posts, "t={0}&p={1}", topicID, iPost ) );
					}
				}
				else
				{
					bool bFirst = true;
					for ( int i = 0; i < PageCount; i++ )
					{
						int iPost = i + 1;

						if ( bFirst ) bFirst = false;
						else strReturn += ", ";

						strReturn += MakeLink( iPost.ToString(), YafBuildLink.GetLink( ForumPages.posts, "t={0}&p={1}", topicID, iPost ) );
					}
				}
			}
			return strReturn;
		}

		private string MakeLink( string text, string link )
		{
			return String.Format( "<a href=\"{0}\">{1}</a>", link, text );
		}

		public bool FindUnread
		{
			set
			{
				ViewState ["FindUnread"] = value;
			}
			get
			{
				return ( ViewState ["FindUnread"] != null ) ? Convert.ToBoolean( ViewState ["FindUnread"] ) : false;
			}
		}
	}
}
