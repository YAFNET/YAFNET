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
	/// Summary description for postmessage.
	/// </summary>
	public partial class postmessage : YAF.Classes.Base.ForumPage
	{
		protected YAF.Editor.ForumEditor Message;
		protected System.Web.UI.WebControls.Label NoEditSubject;
        protected int OwnerUserId;

		public postmessage()
			: base( "POSTMESSAGE" )
		{

		}

		override protected void OnInit( System.EventArgs e )
		{
			// get the forum editor based on the settings
			Message = YAF.Editor.EditorHelper.CreateEditorFromType( PageContext.BoardSettings.ForumEditor );
			EditorLine.Controls.Add( Message );

			base.OnInit( e );
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			DataRow currentRow = null;

			if ( Request.QueryString ["q"] != null )
			{
				using ( DataTable dt = YAF.Classes.Data.DB.message_list( Request.QueryString ["q"] ) )
					currentRow = dt.Rows [0];

				if ( Convert.ToInt32( currentRow ["TopicID"] ) != PageContext.PageTopicID )
					yaf_BuildLink.AccessDenied();

				if ( !CanQuotePostCheck( currentRow ) )
					yaf_BuildLink.AccessDenied();
			}
			else if ( Request.QueryString ["m"] != null )
			{
				using ( DataTable dt = YAF.Classes.Data.DB.message_list( Request.QueryString ["m"] ) )
					currentRow = dt.Rows [0];
                OwnerUserId = Convert.ToInt32(currentRow["UserId"]);
				if ( !CanEditPostCheck( currentRow ) )
					yaf_BuildLink.AccessDenied();
			}

			if ( PageContext.PageForumID == 0 )
				yaf_BuildLink.AccessDenied();
			if ( Request ["t"] != null && !PageContext.ForumPostAccess )
				yaf_BuildLink.AccessDenied();
			if ( Request ["t"] != null && !PageContext.ForumReplyAccess )
				yaf_BuildLink.AccessDenied();

			//Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
			Message.StyleSheet = yaf_BuildLink.ThemeFile( "theme.css" );
			Message.BaseDir = yaf_ForumInfo.ForumRoot + "editors";

			Title.Text = GetText( "NEWTOPIC" );
			PollExpire.Attributes.Add( "style", "width:50px" );

			if ( !IsPostBack )
			{
				Priority.Items.Add( new ListItem( GetText( "normal" ), "0" ) );
				Priority.Items.Add( new ListItem( GetText( "sticky" ), "1" ) );
				Priority.Items.Add( new ListItem( GetText( "announcement" ), "2" ) );
				Priority.SelectedIndex = 0;

                EditReasonRow.Visible = false;
				Preview.Text = GetText( "preview" );
				PostReply.Text = GetText( "Save" );
				Cancel.Text = GetText( "Cancel" );
				CreatePoll.Text = GetText( "createpoll" );

				PriorityRow.Visible = PageContext.ForumPriorityAccess;
				CreatePollRow.Visible = Request.QueryString ["t"] == null && PageContext.ForumPollAccess;

				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					PageLinks.AddLink( PageContext.PageCategoryName, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				}
				PageLinks.AddForumLinks( PageContext.PageForumID );

				if ( Request.QueryString ["t"] != null )
				{
					// new post...
					DataRow topic = YAF.Classes.Data.DB.topic_info( Request.QueryString ["t"] );
					if ( ( ( int ) topic ["Flags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) == ( int ) YAF.Classes.Data.TopicFlags.Locked )
						Response.Redirect( Request.UrlReferrer.ToString() );
					SubjectRow.Visible = false;
					Title.Text = GetText( "reply" );

					if ( YAF.Classes.Config.IsDotNetNuke || YAF.Classes.Config.IsRainbow || YAF.Classes.Config.IsPortal )
					{
						// can't use the last post iframe
						LastPosts.Visible = true;
						LastPosts.DataSource = YAF.Classes.Data.DB.post_list_reverse10( Request.QueryString ["t"] );
						LastPosts.DataBind();
					}
					else
					{
						LastPostsIFrame.Visible = true;
						LastPostsIFrame.Attributes.Add( "src", "framehelper.aspx?g=lastposts&t=" + Request.QueryString ["t"] );
					}
				}

				if ( Request.QueryString ["q"] != null )
				{
					// reply to post...
					bool isHtml = currentRow ["Message"].ToString().IndexOf( '<' ) >= 0;

					string tmpMessage = currentRow ["Message"].ToString();

					if ( PageContext.BoardSettings.RemoveNestedQuotes )
					{
						RegexOptions m_options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
						Regex quote = new Regex( @"\[quote(\=.*)?\](.*?)\[/quote\]", m_options );
						// remove quotes from old messages
						tmpMessage = quote.Replace( tmpMessage, "" );
					}

						Message.Text = String.Format( "[quote={0}]{1}[/quote]", currentRow ["username"], tmpMessage );
				}
				else if ( Request.QueryString ["m"] != null )
				{
					// edit message...
					string body = currentRow ["message"].ToString();
					Message.Text = body;
					Title.Text = GetText( "EDIT" );

					Subject.Text = Server.HtmlDecode( Convert.ToString( currentRow ["Topic"] ) );

					if ( ( Convert.ToInt32( currentRow ["TopicOwnerID"] ) == Convert.ToInt32( currentRow ["UserID"] ) ) || PageContext.ForumModeratorAccess )
					{
						// allow editing of the topic subject
						Subject.Enabled = true;
					}
					else
					{
						// disable the subject
						Subject.Enabled = false;
					}

					CreatePollRow.Visible = false;
					Priority.SelectedItem.Selected = false;
					Priority.Items.FindByValue( currentRow ["Priority"].ToString() ).Selected = true;
                    EditReasonRow.Visible = true;
                    ReasonEditor.Text = Server.HtmlDecode(Convert.ToString(currentRow["EditReason"]));
				}

				From.Text = PageContext.PageUserName;
				if(User!=null)
					FromRow.Visible = false;
			}
		}

		private bool CanEditPostCheck( DataRow message )
		{
			bool postLocked = false;

			if ( !PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0 )
			{
				DateTime edited = ( DateTime ) message ["Edited"];

				if ( edited.AddDays( PageContext.BoardSettings.LockPosts ) < DateTime.Now )
					postLocked = true;
			}

			DataRow forumInfo, topicInfo;

			// get topic and forum information
			topicInfo = YAF.Classes.Data.DB.topic_info( PageContext.PageTopicID );
			using ( DataTable dt = YAF.Classes.Data.DB.forum_list( PageContext.PageBoardID, PageContext.PageForumID ) )
				forumInfo = dt.Rows [0];

			return !postLocked && ( ( int ) forumInfo ["Flags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) topicInfo ["Flags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && ( ( int ) message ["UserID"] == PageContext.PageUserID || PageContext.ForumModeratorAccess ) && PageContext.ForumEditAccess;
		}

		private bool CanQuotePostCheck( DataRow message )
		{
			DataRow forumInfo, topicInfo;

			// get topic and forum information
			topicInfo = YAF.Classes.Data.DB.topic_info( PageContext.PageTopicID );
			using ( DataTable dt = YAF.Classes.Data.DB.forum_list( PageContext.PageBoardID, PageContext.PageForumID ) )
				forumInfo = dt.Rows [0];

			return ( ( int ) forumInfo ["Flags"] & ( int ) YAF.Classes.Data.ForumFlags.Locked ) != ( int ) YAF.Classes.Data.ForumFlags.Locked && ( ( int ) topicInfo ["Flags"] & ( int ) YAF.Classes.Data.TopicFlags.Locked ) != ( int ) YAF.Classes.Data.TopicFlags.Locked && PageContext.ForumReplyAccess;
		}

		protected void PostReply_Click( object sender, System.EventArgs e )
		{
			if ( SubjectRow.Visible && Subject.Text.Length <= 0 )
			{
				PageContext.AddLoadMessage( GetText( "need_subject" ) );
				return;
			}

			if ( PollRow1.Visible )
			{
				if ( Question.Text.Trim().Length == 0 )
				{
					PageContext.AddLoadMessage( GetText( "NEED_QUESTION" ) );
					return;
				}

				string p1 = PollChoice1.Text.Trim();
				string p2 = PollChoice2.Text.Trim();
				if ( p1.Length == 0 || p2.Length == 0 )
				{
					PageContext.AddLoadMessage( GetText( "NEED_CHOICES" ) );
					return;
				}
			}


			// see if there is a post delay
			if ( !( PageContext.IsAdmin || PageContext.IsModerator ) && PageContext.BoardSettings.PostFloodDelay > 0 )
			{
				// see if they've past that delay point
				if ( Mession.LastPost > DateTime.Now.AddSeconds( -PageContext.BoardSettings.PostFloodDelay ) && Request.QueryString ["m"] == null )
				{
					PageContext.AddLoadMessage( String.Format( GetText( "wait" ), ( Mession.LastPost - DateTime.Now.AddSeconds( -PageContext.BoardSettings.PostFloodDelay ) ).Seconds ) );
					return;
				}
			}

			long TopicID;
			long nMessageID = 0;
			object replyTo = null;

			if ( Request.QueryString ["q"] != null )
				replyTo = int.Parse( Request.QueryString ["q"] );
			else
				// Let save procedure find first post
				replyTo = -1;

			string msg = Message.Text;

			Mession.LastPost = DateTime.Now;

			if ( Request.QueryString ["t"] != null )
			{
				if ( !PageContext.ForumReplyAccess )
					yaf_BuildLink.AccessDenied();

				TopicID = long.Parse( Request.QueryString ["t"] );
				// make message flags
				MessageFlags tFlags = new MessageFlags();

				tFlags.IsHTML = Message.UsesHTML;
				tFlags.IsBBCode = Message.UsesBBCode;

				if ( !YAF.Classes.Data.DB.message_save( TopicID, PageContext.PageUserID, msg, User!=null ? null : From.Text, Request.UserHostAddress, null, replyTo, tFlags.BitValue, ref nMessageID ) )
					TopicID = 0;
			}
			else if ( Request.QueryString ["m"] != null )
			{
				if ( !PageContext.ForumEditAccess )
					yaf_BuildLink.AccessDenied();

				string SubjectSave = "";
				if ( Subject.Enabled ) SubjectSave = Server.HtmlEncode( Subject.Text );

				// make message flags
				MessageFlags tFlags = new MessageFlags();

				tFlags.IsHTML = Message.UsesHTML;
				tFlags.IsBBCode = Message.UsesBBCode;

                bool isModeratorChanged = (PageContext.PageUserID != OwnerUserId);
                YAF.Classes.Data.DB.message_update(Request.QueryString["m"], Priority.SelectedValue, msg, SubjectSave, tFlags.BitValue, ReasonEditor.Text, isModeratorChanged);
				TopicID = PageContext.PageTopicID;
				nMessageID = long.Parse( Request.QueryString ["m"] );
			}
			else
			{
				if ( !PageContext.ForumPostAccess )
					yaf_BuildLink.AccessDenied();

				object PollID = null;

				if ( PollRow1.Visible )
				{

					int daysPollExpire = 0;
					object datePollExpire = null;

					try
					{
						daysPollExpire = Convert.ToInt32( PollExpire.Text.Trim() );
					}
					catch
					{

					}

					if ( daysPollExpire > 0 )
					{
						datePollExpire = DateTime.Now.AddDays( daysPollExpire );
					}

					PollID = YAF.Classes.Data.DB.poll_save( Question.Text,
						PollChoice1.Text,
						PollChoice2.Text,
						PollChoice3.Text,
						PollChoice4.Text,
						PollChoice5.Text,
						PollChoice6.Text,
						PollChoice7.Text,
						PollChoice8.Text,
						PollChoice9.Text,
						datePollExpire );
				}

				// make message flags
				MessageFlags tFlags = new MessageFlags();

				tFlags.IsHTML = Message.UsesHTML;
				tFlags.IsBBCode = Message.UsesBBCode;

				string subject = Server.HtmlEncode( Subject.Text );
				TopicID = YAF.Classes.Data.DB.topic_save( PageContext.PageForumID, subject, msg, PageContext.PageUserID, Priority.SelectedValue, PollID, User!=null ? null : From.Text, Request.UserHostAddress, null, tFlags.BitValue, ref nMessageID );
			}

			// Check if message is approved
			bool bApproved = false;
			using ( DataTable dt = YAF.Classes.Data.DB.message_list( nMessageID ) )
				foreach ( DataRow row in dt.Rows )
					bApproved = ( ( int ) row ["Flags"] & 16 ) == 16;

			// Create notification emails
			if ( bApproved )
			{
				General.CreateWatchEmail( nMessageID );
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.posts, "m={0}&#{0}", nMessageID );
			}
			else
			{
				// Tell user that his message will have to be approved by a moderator
				//PageContext.AddLoadMessage("Since you posted to a moderated forum, a forum moderator must approve your post before it will become visible.");
				string url = YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", PageContext.PageForumID );
				if ( YAF.Classes.Config.IsRainbow )
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.info, "i=1" );
				else
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.info, "i=1&url={0}", Server.UrlEncode( url ) );
			}
		}

		protected void CreatePoll_Click( object sender, System.EventArgs e )
		{
			CreatePollRow.Visible = false;
			PollRow1.Visible = true;
			PollRow2.Visible = true;
			PollRow3.Visible = true;
			PollRow4.Visible = true;
			PollRow5.Visible = true;
			PollRow6.Visible = true;
			PollRow7.Visible = true;
			PollRow8.Visible = true;
			PollRow9.Visible = true;
			PollRow10.Visible = true;
			PollRowExpire.Visible = true;
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			if ( Request.QueryString ["t"] != null || Request.QueryString ["m"] != null )
			{
				// reply to existing topic or editing of existing topic
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.posts, "t={0}", PageContext.PageTopicID );
			}
			else
			{
				// new topic -- cancel back to forum
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.topics, "f={0}", PageContext.PageForumID );
			}
		}

		protected void Preview_Click( object sender, System.EventArgs e )
		{
			PreviewRow.Visible = true;

			MessageFlags tFlags = new MessageFlags();
			tFlags.IsHTML = Message.UsesHTML;
			tFlags.IsBBCode = Message.UsesBBCode;

			string body = FormatMsg.FormatMessage( Message.Text, tFlags );

			using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
			{
				if ( !dt.Rows [0].IsNull( "Signature" ) )
					body += "<br/><hr noshade/>" + FormatMsg.FormatMessage( dt.Rows [0] ["Signature"].ToString(), new MessageFlags() );
			}

			PreviewCell.InnerHtml = body;
		}

		protected string FormatBody( object o )
		{
			DataRowView row = ( DataRowView ) o;
			string html = FormatMsg.FormatMessage( row ["Message"].ToString(), new MessageFlags( Convert.ToInt32( row ["Flags"] ) ) );

			string messageSignature = row ["Signature"].ToString();

			if ( messageSignature != string.Empty )
			{
				MessageFlags flags = new MessageFlags();
				flags.IsHTML = false;
				messageSignature = FormatMsg.FormatMessage( messageSignature, flags );
				html += "<br/><hr noshade/>" + messageSignature;
			}

			return html;
		}
	}
}
