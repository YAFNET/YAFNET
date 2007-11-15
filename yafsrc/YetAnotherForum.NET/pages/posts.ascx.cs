/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for posts.
	/// </summary>
	public partial class posts : YAF.Classes.Base.ForumPage
	{
		protected YAF.Editor.ForumEditor QuickReplyEditor;

		private DataRow _forum, _topic;
		private DataTable _dtPoll;
		private bool _dataBound = false;
		private bool _ignoreQueryString = false;

		public posts()
			: base( "POSTS" )
		{
		}

		private void posts_PreRender( object sender, EventArgs e )
		{
			bool isWatched = HandleWatchTopic();

			MyTestMenu.AddItem( "watch", isWatched ? GetText( "unwatchtopic" ) : GetText( "watchtopic" ) );
			if (PageContext.BoardSettings.AllowEmailTopic) MyTestMenu.AddItem("email", GetText("emailtopic"));
			MyTestMenu.AddItem( "print", GetText( "printtopic" ) );
			if ( PageContext.BoardSettings.ShowRSSLink ) MyTestMenu.AddItem( "rssfeed", GetText( "rsstopic" ) );
			ViewMenu.AddItem( "normal", GetText( "NORMAL" ) );
			ViewMenu.AddItem( "threaded", GetText( "THREADED" ) );
			MyTestMenu.Attach( MyTest );
			ViewMenu.Attach( View );

			if ( !_dataBound )
				BindData();
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			QuickReplyEditor.BaseDir = YafForumInfo.ForumRoot + "editors";
			QuickReplyEditor.StyleSheet = YafBuildLink.ThemeFile( "theme.css" );

			_topic = YAF.Classes.Data.DB.topic_info( PageContext.PageTopicID );

			// in case topic is deleted or not existant
			if (_topic == null)
				YafBuildLink.Redirect(ForumPages.info, "i=6");	// invalid argument message

			using ( DataTable dt = YAF.Classes.Data.DB.forum_list( PageContext.PageBoardID, PageContext.PageForumID ) )
				_forum = dt.Rows [0];

			if ( !PageContext.ForumReadAccess )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					PageLinks.AddLink( PageContext.PageCategoryName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				}

				QuickReply.Text = GetText( "POSTMESSAGE", "SAVE" );
				DataPanel1.TitleText = GetText( "QUICKREPLY" );
				DataPanel1.ExpandText = GetText( "QUICKREPLY_SHOW" );
				DataPanel1.CollapseText = GetText( "QUICKREPLY_HIDE" );

				PageLinks.AddForumLinks( PageContext.PageForumID );
				PageLinks.AddLink( General.BadWordReplace( Server.HtmlDecode( PageContext.PageTopicName ) ), "" );

				TopicTitle.Text = General.BadWordReplace( ( string ) _topic ["Topic"] );

				ViewOptions.Visible = PageContext.BoardSettings.AllowThreaded;
				ForumJumpHolder.Visible = PageContext.BoardSettings.ShowForumJump && PageContext.Settings.LockedForum == 0;

				RssTopic.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.rsstopic, "pg={0}&amp;t={1}", Request.QueryString ["g"], PageContext.PageTopicID );
				RssTopic.Visible = PageContext.BoardSettings.ShowRSSLink;

				if ( PageContext.BoardSettings.EnableCaptchaForPost )
				{
					Session ["CaptchaImageText"] = General.GetCaptchaString();
					imgCaptcha.ImageUrl = String.Format( "{0}resource.ashx?c=1", YafForumInfo.ForumRoot );
					CaptchaDiv.Visible = true;
				}

				if ( !PageContext.ForumPostAccess )
				{
					NewTopic1.Visible = false;
					NewTopic2.Visible = false;
				}

				// Ederon : 9/9/2007 - moderators can relpy in locked topics
				if (!PageContext.ForumReplyAccess || 
					(General.BinaryAnd(_topic["Flags"], TopicFlags.Locked) && !PageContext.ForumModeratorAccess))
				{
					PostReplyLink1.Visible = PostReplyLink2.Visible = false;
					QuickReplyPlaceHolder.Visible = false;
				}

				if ( PageContext.ForumModeratorAccess )
				{
					MoveTopic1.Visible = true;
					MoveTopic2.Visible = true;
				}
				else
				{
					MoveTopic1.Visible = false;
					MoveTopic2.Visible = false;
				}
                /*
				PostReplyLink1.Text = GetThemeContents( "BUTTONS", "POSTREPLY" );
				PostReplyLink1.ToolTip = GetText( "POSTS", "TIP_REPLY_TOPIC" );
				PostReplyLink2.Text = PostReplyLink1.Text;
				PostReplyLink2.ToolTip = PostReplyLink1.ToolTip;
				NewTopic1.Text = GetThemeContents( "BUTTONS", "NEWTOPIC" );
				NewTopic1.ToolTip = GetText( "POSTS", "TIP_NEW_TOPIC" );
				NewTopic2.Text = NewTopic1.Text;
				NewTopic2.ToolTip = NewTopic1.ToolTip;
				DeleteTopic1.Text = GetThemeContents( "BUTTONS", "DELETETOPIC" );
				DeleteTopic1.ToolTip = GetText( "POSTS", "TIP_DELETE_TOPIC" );
				DeleteTopic2.Text = DeleteTopic1.Text;
				DeleteTopic2.ToolTip = DeleteTopic1.ToolTip;
				LockTopic1.Text = GetThemeContents( "BUTTONS", "LOCKTOPIC" );
				LockTopic1.ToolTip = GetText( "POSTS", "TIP_LOCK_TOPIC" );
				LockTopic2.Text = LockTopic1.Text;
				LockTopic2.ToolTip = LockTopic1.ToolTip;
				UnlockTopic1.Text = GetThemeContents( "BUTTONS", "UNLOCKTOPIC" );
				UnlockTopic1.ToolTip = GetText( "POSTS", "TIP_UNLOCK_TOPIC" );
				UnlockTopic2.Text = UnlockTopic1.Text;
				UnlockTopic2.ToolTip = UnlockTopic1.ToolTip;
				MoveTopic1.Text = GetThemeContents( "BUTTONS", "MOVETOPIC" );
				MoveTopic1.ToolTip = GetText( "POSTS", "TIP_MOVE_TOPIC" );
				MoveTopic2.Text = MoveTopic1.Text;
				MoveTopic2.ToolTip = MoveTopic1.ToolTip;
                */

				if ( !PageContext.ForumModeratorAccess )
				{
					LockTopic1.Visible = false;
					UnlockTopic1.Visible = false;
					DeleteTopic1.Visible = false;
					LockTopic2.Visible = false;
					UnlockTopic2.Visible = false;
					DeleteTopic2.Visible = false;
				}
				else
				{
					LockTopic1.Visible = !General.BinaryAnd(_topic["Flags"], TopicFlags.Locked);
					UnlockTopic1.Visible = !LockTopic1.Visible;
					LockTopic2.Visible = LockTopic1.Visible;
					UnlockTopic2.Visible = !LockTopic2.Visible;
				}

				// handle custom BBCode javascript or CSS...
				BBCode.RegisterCustomBBCodePageElements( Page, this.GetType() );
			}
			// Mark topic read
			Mession.SetTopicRead( PageContext.PageTopicID, DateTime.Now );

			BindData();
		}

		protected void DeleteMessage_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton ) sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", GetText( "confirm_deletemessage" ) );
		}

		protected void DeleteTopic_Load( object sender, System.EventArgs e )
		{
			( ( LinkButton ) sender ).Attributes ["onclick"] = String.Format( "return confirm('{0}')", GetText( "confirm_deletetopic" ) );
		}

		private void QuickReply_Click( object sender, EventArgs e )
		{
			if (!PageContext.ForumReplyAccess ||
				(General.BinaryAnd(_topic["Flags"], TopicFlags.Locked) && !PageContext.ForumModeratorAccess))
				YafBuildLink.AccessDenied();

			if ( QuickReplyEditor.Text.Length <= 0 )
			{
				PageContext.AddLoadMessage( GetText( "EMPTY_MESSAGE") );
				return;
			}

			if ( PageContext.BoardSettings.EnableCaptchaForPost && Session ["CaptchaImageText"].ToString() != tbCaptcha.Text.Trim() )
			{
				PageContext.AddLoadMessage( GetText( "BAD_CAPTCHA" ) );
				return;
			}

			if ( !( PageContext.IsAdmin || PageContext.IsModerator ) && PageContext.BoardSettings.PostFloodDelay > 0 )
			{
				if ( Mession.LastPost > DateTime.Now.AddSeconds( -PageContext.BoardSettings.PostFloodDelay ) )
				{
					PageContext.AddLoadMessage( String.Format( GetText( "wait" ), ( Mession.LastPost - DateTime.Now.AddSeconds( -PageContext.BoardSettings.PostFloodDelay ) ).Seconds ) );
					return;
				}
			}
			Mession.LastPost = DateTime.Now;

			// post message...

			long TopicID;
			long nMessageID = 0;
			object replyTo = -1;
			string msg = QuickReplyEditor.Text;
			TopicID = PageContext.PageTopicID;

			MessageFlags tFlags = new MessageFlags();

			tFlags.IsHTML = QuickReplyEditor.UsesHTML;
			tFlags.IsBBCode = QuickReplyEditor.UsesBBCode;

			if ( !YAF.Classes.Data.DB.message_save( TopicID, PageContext.PageUserID, msg, null, Request.UserHostAddress, null, replyTo, tFlags.BitValue, ref nMessageID ) )
				TopicID = 0;

			bool bApproved = false;

			using ( DataTable dt = YAF.Classes.Data.DB.message_list( nMessageID ) )
				foreach ( DataRow row in dt.Rows )
					bApproved = ( ( int ) row ["Flags"] & 16 ) == 16;

			if ( bApproved )
			{
				// Ederon : 7/26/2007
				// send new post notification to users watching this topic/forum
				General.CreateWatchEmail(nMessageID);
				// redirect to newly posted message
				YAF.Classes.Utils.YafBuildLink.Redirect(YAF.Classes.Utils.ForumPages.posts, "m={0}&#{0}", nMessageID);
			}
			else
			{
				string url = YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", PageContext.PageForumID );
				if ( YAF.Classes.Config.IsRainbow )
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.info, "i=1" );
				else
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.info, "i=1&url={0}", Server.UrlEncode( url ) );
			}
		}

		private void Pager_PageChange( object sender, EventArgs e )
		{
			_ignoreQueryString = true;
			SmartScroller1.Reset();
			BindData();
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			// Quick Reply Modification Begin
			QuickReplyEditor = new YAF.Editor.BasicBBCodeEditor();
			QuickReplyLine.Controls.Add( QuickReplyEditor );
			QuickReply.Click += new EventHandler( QuickReply_Click );
			Pager.PageChange += new EventHandler( Pager_PageChange );

			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit( e );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Poll.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler( this.Poll_ItemCommand );
			this.PreRender += new EventHandler( posts_PreRender );
			this.MyTestMenu.ItemClick += new YAF.Controls.PopEventHandler( MyTestMenu_ItemClick );
			this.ViewMenu.ItemClick += new YAF.Controls.PopEventHandler( ViewMenu_ItemClick );
		}
		#endregion

		private void BindData()
		{
			_dataBound = true;

			Pager.PageSize = PageContext.BoardSettings.PostsPerPage;

			if ( _topic == null )
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.topics, "f={0}", PageContext.PageForumID );

			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;
			pds.PageSize = Pager.PageSize;

            using ( DataTable dt0 = YAF.Classes.Data.DB.post_list(PageContext.PageTopicID, IsPostBack ? 0 : 1, PageContext.BoardSettings.ShowDeletedMessages))
			{
				bool ShowAds = true;
				if ( User != null )
					ShowAds = PageContext.BoardSettings.ShowAdsToSignedInUsers;

				if ( PageContext.BoardSettings.AdPost != null && PageContext.BoardSettings.AdPost.Length > 0 && ShowAds )
				{
					// create row for "sponsered" message
					dt0.ImportRow( dt0.Rows [0] );
					DataRow sponserRow = dt0.Rows [dt0.Rows.Count - 1];
					populateSponserRow( ref sponserRow );					
				}

				DataView dt = dt0.DefaultView;

				if ( IsThreaded )
					dt.Sort = "Position";
				else
					dt.Sort = "Posted";

				Pager.Count = dt.Count;
				pds.DataSource = dt;
				int nFindMessage = 0;
				try
				{
					if ( _ignoreQueryString )
					{
					}
					else if ( Request.QueryString ["p"] != null )
					{
						// show specific page (p is 1 based)
						int tPage = Convert.ToInt32( Request.QueryString ["p"] );
						if ( pds.PageCount >= tPage )
						{
							pds.CurrentPageIndex = tPage - 1;
							Pager.CurrentPageIndex = pds.CurrentPageIndex;
						}
					}
					else if ( Request.QueryString ["m"] != null )
					{
						// Show this message
						nFindMessage = int.Parse( Request.QueryString ["m"] );
					}
					else if ( Request.QueryString ["find"] != null && Request.QueryString ["find"].ToLower() == "unread" )
					{
						// Find next unread
						using ( DataTable dtUnread = YAF.Classes.Data.DB.message_findunread( PageContext.PageTopicID, Mession.LastVisit ) )
						{
							foreach ( DataRow row in dtUnread.Rows )
							{
								nFindMessage = ( int ) row ["MessageID"];
								break;
							}
						}
					}
				}
				catch ( Exception x )
				{
					YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				}

				if ( nFindMessage > 0 )
				{
					CurrentMessage = nFindMessage;
					// Find correct page for message
					for ( int foundRow = 0; foundRow < dt.Count; foundRow++ )
					{
						if ( ( int ) dt [foundRow] ["MessageID"] == nFindMessage )
						{
							pds.CurrentPageIndex = foundRow / pds.PageSize;
							Pager.CurrentPageIndex = pds.CurrentPageIndex;
							break;
						}
					}
				}
				else
				{
					foreach ( DataRow row in dt0.Rows )
					{
						CurrentMessage = ( int ) row ["MessageID"];
						break;
					}
				}
			}

			pds.CurrentPageIndex = Pager.CurrentPageIndex;

			if ( pds.CurrentPageIndex >= pds.PageCount ) pds.CurrentPageIndex = pds.PageCount - 1;

			MessageList.DataSource = pds;

			if ( _topic ["PollID"] != DBNull.Value )
			{
				Poll.Visible = true;
				_dtPoll = YAF.Classes.Data.DB.poll_stats( _topic ["PollID"] );
				Poll.DataSource = _dtPoll;
			}

			DataBind();
		}

		private void populateSponserRow( ref DataRow sponserRow )
		{
			sponserRow ["UserName"] = GetText( "AD_USERNAME" );
			sponserRow ["UserID"] = 1;
			sponserRow ["Message"] = PageContext.BoardSettings.AdPost;
			sponserRow ["HasAvatarImage"] = false;
			sponserRow ["HasAttachments"] = false;
			sponserRow ["Signature"] = GetText( "AD_SIGNATURE" );
			sponserRow ["IP"] = "none";
			sponserRow ["Edited"] = sponserRow ["Posted"];

			MessageFlags sponserMessageFlags = new MessageFlags( 0 );

			// message is not editable and doesn't need formatting...
			sponserMessageFlags.NotFormatted = true;
			sponserMessageFlags.IsLocked = true;

			sponserRow ["Flags"] = sponserMessageFlags.BitValue;
		}

		private bool HandleWatchTopic()
		{
			if ( PageContext.IsGuest ) return false;
			// check if this forum is being watched by this user
			using ( DataTable dt = YAF.Classes.Data.DB.watchtopic_check( PageContext.PageUserID, PageContext.PageTopicID ) )
			{
				if ( dt.Rows.Count > 0 )
				{
					// subscribed to this forum
					TrackTopic.Text = GetText( "UNWATCHTOPIC" );
					foreach ( DataRow row in dt.Rows )
					{
						WatchTopicID.InnerText = row ["WatchTopicID"].ToString();
						return true;
					}
				}
				else
				{
					// not subscribed
					WatchTopicID.InnerText = "";
					TrackTopic.Text = GetText( "WATCHTOPIC" );
				}
			}
			return false;
		}

		protected void DeleteTopic_Click( object sender, System.EventArgs e )
		{
			if ( !PageContext.ForumModeratorAccess )
				YafBuildLink.AccessDenied(/*"You don't have access to delete topics."*/);

			// Take away 150 points once!
			YAF.Classes.Data.DB.user_removepointsByTopicID( PageContext.PageTopicID, 150 );
			YAF.Classes.Data.DB.topic_delete( PageContext.PageTopicID );
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.topics, "f={0}", PageContext.PageForumID );
		}

		protected void LockTopic_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Data.DB.topic_lock( PageContext.PageTopicID, true );
			BindData();
			PageContext.AddLoadMessage( GetText( "INFO_TOPIC_LOCKED" ) );
			LockTopic1.Visible = !LockTopic1.Visible;
			UnlockTopic1.Visible = !UnlockTopic1.Visible;
			LockTopic2.Visible = LockTopic1.Visible;
			UnlockTopic2.Visible = UnlockTopic1.Visible;
			/*PostReplyLink1.Visible = false;
			PostReplyLink2.Visible = false;*/
		}

		protected void UnlockTopic_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Data.DB.topic_lock( PageContext.PageTopicID, false );
			BindData();
			PageContext.AddLoadMessage( GetText( "INFO_TOPIC_UNLOCKED" ) );
			LockTopic1.Visible = !LockTopic1.Visible;
			UnlockTopic1.Visible = !UnlockTopic1.Visible;
			LockTopic2.Visible = LockTopic1.Visible;
			UnlockTopic2.Visible = UnlockTopic1.Visible;
			PostReplyLink1.Visible = PageContext.ForumReplyAccess;
			PostReplyLink2.Visible = PageContext.ForumReplyAccess;
		}

		/// <summary>
		/// Property to verify if the current user can vote in this poll.
		/// </summary>
		protected bool CanVote
		{
			get
			{
				// rule out users without voting rights
				if ( !PageContext.ForumVoteAccess ) return false;

				if ( IsPollClosed() ) return false;

				// check for voting cookie
				string cookieName = String.Format( "poll#{0}", _topic ["PollID"] );
				if ( Request.Cookies [cookieName] != null ) return false;

				object UserID = null;
				object RemoteIP = null;

				if ( PageContext.BoardSettings.PollVoteTiedToIP ) RemoteIP = General.IPStrToLong( Request.UserHostAddress ).ToString();

				if ( !PageContext.IsGuest )
				{
					UserID = PageContext.PageUserID;
				}

				// check for a record of a vote
				using ( DataTable dt = YAF.Classes.Data.DB.pollvote_check( _topic ["PollID"], UserID, RemoteIP ) )
				{
					if ( dt.Rows.Count == 0 )
					{
						// user hasn't voted yet...
						return true;
					}
				}

				return false;
			}
		}

		private void Poll_ItemCommand( object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e )
		{
			if ( e.CommandName == "vote" && PageContext.ForumVoteAccess )
			{
				if ( !this.CanVote )
				{
					PageContext.AddLoadMessage( GetText( "WARN_ALREADY_VOTED" ) );
					return;
				}

				if (General.BinaryAnd(_topic["Flags"], TopicFlags.Locked))
				{
					PageContext.AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
					return;
				}

				if ( IsPollClosed() )
				{
					PageContext.AddLoadMessage( GetText( "WARN_POLL_CLOSED" ) );
					return;
				}

				object UserID = null;
				object RemoteIP = null;

				if ( PageContext.BoardSettings.PollVoteTiedToIP ) RemoteIP = General.IPStrToLong( Request.ServerVariables ["REMOTE_ADDR"] ).ToString();

				if ( !PageContext.IsGuest )
				{
					UserID = PageContext.PageUserID;
				}

				YAF.Classes.Data.DB.choice_vote( e.CommandArgument, UserID, RemoteIP );

				string cookieName = String.Format( "poll#{0}", e.CommandArgument );
				HttpCookie c = new HttpCookie( cookieName, e.CommandArgument.ToString() );
				c.Expires = DateTime.Now.AddYears( 1 );
				Response.Cookies.Add( c );

				PageContext.AddLoadMessage( GetText( "INFO_VOTED" ) );
				BindData();
			}
		}

		protected bool IsPollClosed()
		{
			bool bIsClosed = false;

			if ( _dtPoll.Rows [0] ["Closes"] != DBNull.Value )
			{
				DateTime tCloses = Convert.ToDateTime( _dtPoll.Rows [0] ["Closes"] );
				if ( tCloses < DateTime.Now )
				{
					bIsClosed = true;
				}
			}
			return bIsClosed;
		}

		protected string GetPollIsClosed()
		{
			string strPollClosed = "";
			if ( IsPollClosed() ) strPollClosed = GetText( "POLL_CLOSED" );
			return strPollClosed;
		}

		protected string GetPollQuestion()
		{
			return HtmlEncode( General.BadWordReplace( _dtPoll.Rows [0] ["Question"].ToString() ) );
		}

		protected void PostReplyLink_Click( object sender, System.EventArgs e )
		{
			// Ederon : 9/9/2007 - moderator can reply in locked posts
			if (!PageContext.ForumModeratorAccess)
			{
				if (General.BinaryAnd(_topic["Flags"], TopicFlags.Locked))
				{
					PageContext.AddLoadMessage(GetText("WARN_TOPIC_LOCKED"));
					return;
				}

				if (General.BinaryAnd(_forum["Flags"], ForumFlags.Locked))
				{
					PageContext.AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
					return;
				}
			}

			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.postmessage, "t={0}&f={1}", PageContext.PageTopicID, PageContext.PageForumID );
		}

		protected void NewTopic_Click( object sender, System.EventArgs e )
		{
			if (General.BinaryAnd(_forum["Flags"], ForumFlags.Locked))
			{
				PageContext.AddLoadMessage(GetText("WARN_FORUM_LOCKED"));
				return;
			}
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.postmessage, "f={0}", PageContext.PageForumID );
		}

		protected void TrackTopic_Click( object sender, System.EventArgs e )
		{
			if ( PageContext.IsGuest )
			{
				PageContext.AddLoadMessage( GetText( "WARN_WATCHLOGIN" ) );
				return;
			}

			if ( WatchTopicID.InnerText == "" )
			{
				YAF.Classes.Data.DB.watchtopic_add( PageContext.PageUserID, PageContext.PageTopicID );
				PageContext.AddLoadMessage( GetText( "INFO_WATCH_TOPIC" ) );
			}
			else
			{
				int tmpID = Convert.ToInt32( WatchTopicID.InnerText );
				YAF.Classes.Data.DB.watchtopic_delete( tmpID );
				PageContext.AddLoadMessage( GetText( "INFO_UNWATCH_TOPIC" ) );
			}

			HandleWatchTopic();

			BindData();
		}

		protected void MoveTopic_Click( object sender, System.EventArgs e )
		{
			if ( !PageContext.ForumModeratorAccess )
				YafBuildLink.AccessDenied(/*"You are not a forum moderator."*/);

			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.movetopic, "t={0}", PageContext.PageTopicID );
		}

		protected void PrevTopic_Click( object sender, System.EventArgs e )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.topic_findprev( PageContext.PageTopicID ) )
			{
				if ( dt.Rows.Count == 0 )
				{
					PageContext.AddLoadMessage( GetText( "INFO_NOMORETOPICS" ) );
					return;
				}
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.posts, "t={0}", dt.Rows [0] ["TopicID"] );
			}
		}

		protected void NextTopic_Click( object sender, System.EventArgs e )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.topic_findnext( PageContext.PageTopicID ) )
			{
				if ( dt.Rows.Count == 0 )
				{
					PageContext.AddLoadMessage( GetText( "INFO_NOMORETOPICS" ) );
					return;
				}
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.posts, "t={0}", dt.Rows [0] ["TopicID"] );
			}
		}

		protected void EmailTopic_Click( object sender, System.EventArgs e )
		{
			if ( User == null )
			{
				PageContext.AddLoadMessage( GetText( "WARN_EMAILLOGIN" ) );
				return;
			}
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.emailtopic, "t={0}", PageContext.PageTopicID );
		}

		protected void PrintTopic_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.printtopic, "t={0}", PageContext.PageTopicID );
		}

		protected int VoteWidth( object o )
		{
			DataRowView row = ( DataRowView ) o;
			return ( int ) row ["Stats"] * 80 / 100;
		}

		public bool IsThreaded
		{
			get
			{
				if ( Request.QueryString ["threaded"] != null )
					Session ["IsThreaded"] = bool.Parse( Request.QueryString ["threaded"] );
				else if ( Session ["IsThreaded"] == null )
					Session ["IsThreaded"] = false;

				return ( bool ) Session ["IsThreaded"];
			}
			set
			{
				Session ["IsThreaded"] = value;
			}
		}

		protected int CurrentMessage
		{
			get
			{
				if ( ViewState ["CurrentMessage"] != null )
					return ( int ) ViewState ["CurrentMessage"];
				else
					return 0;
			}
			set
			{
				ViewState ["CurrentMessage"] = value;
			}
		}

		protected bool IsCurrentMessage( object o )
		{
			DataRowView row = ( DataRowView ) o;

			return !IsThreaded || CurrentMessage == ( int ) row ["MessageID"];
		}

		protected string GetThreadedRow( object o )
		{
			DataRowView row = ( DataRowView ) o;
			if ( !IsThreaded || CurrentMessage == ( int ) row ["MessageID"] )
				return "";

			System.Text.StringBuilder html = new System.Text.StringBuilder( 1000 );

			// Threaded
			string brief = row ["Message"].ToString();

			RegexOptions options = RegexOptions.IgnoreCase /*| RegexOptions.Singleline | RegexOptions.Multiline*/;
			options |= RegexOptions.Singleline;
			while ( Regex.IsMatch( brief, @"\[quote=(.*?)\](.*)\[/quote\]", options ) )
				brief = Regex.Replace( brief, @"\[quote=(.*?)\](.*)\[/quote\]", "", options );
			while ( Regex.IsMatch( brief, @"\[quote\](.*)\[/quote\]", options ) )
				brief = Regex.Replace( brief, @"\[quote\](.*)\[/quote\]", "", options );

			while ( Regex.IsMatch( brief, @"<.*?>", options ) )
				brief = Regex.Replace( brief, @"<.*?>", "", options );
			while ( Regex.IsMatch( brief, @"\[.*?\]", options ) )
				brief = Regex.Replace( brief, @"\[.*?\]", "", options );

			brief = General.BadWordReplace( brief );

			if ( brief.Length > 42 )
				brief = brief.Substring( 0, 40 ) + "...";
			brief = FormatMsg.AddSmiles( brief );

			html.AppendFormat( "<tr class='post'><td colspan='3' nowrap>" );
			html.AppendFormat( GetIndentImage( row ["Indent"] ) );
			html.AppendFormat( "\n<a href='{0}'>{2} ({1}", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "m={0}#{0}", row ["MessageID"] ), row ["UserName"], brief );
			html.AppendFormat( " - {0})</a>", YafDateTime.FormatDateTimeShort( row ["Posted"] ) );

			return html.ToString();
		}
		protected string GetIndentImage( object o )
		{
			if ( !IsThreaded )
				return "";

			int iIndent = ( int ) o;
			if ( iIndent > 0 )
				return string.Format( "<img src='{1}images/spacer.gif' width='{0}' height='2'/>", iIndent * 32, YafForumInfo.ForumRoot );
			else
				return "";
		}

		private void MyTestMenu_ItemClick( object sender, YAF.Controls.PopEventArgs e )
		{
			switch ( e.Item )
			{
				case "print":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.printtopic, "t={0}", PageContext.PageTopicID );
					break;
				case "watch":
					TrackTopic_Click( sender, e );
					break;
				case "email":
					EmailTopic_Click( sender, e );
					break;
				case "rssfeed":
					YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.rsstopic, "pg={0}&t={1}", Request.QueryString ["g"], PageContext.PageTopicID );
					break;
				default:
					throw new ApplicationException( e.Item );
			}
		}

		private void ViewMenu_ItemClick( object sender, YAF.Controls.PopEventArgs e )
		{
			switch ( e.Item )
			{
				case "normal":
					IsThreaded = false;
					BindData();
					break;
				case "threaded":
					IsThreaded = true;
					BindData();
					break;
				default:
					throw new ApplicationException( e.Item );
			}
		}
	}
}
