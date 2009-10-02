/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using YAF.Classes.Data;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for postmessage.
	/// </summary>
	public partial class deletemessage : YAF.Classes.Core.ForumPage
	{
		protected int _ownerUserId;
		protected DataRow _messageRow;
		protected bool _isModeratorChanged;
		protected ForumFlags _forumFlags = null;
		protected TopicFlags _topicFlags = null;

		public deletemessage()
			: base( "DELETEMESSAGE" )
		{

		}

		private void LinkedPosts_ItemDataBound( object sender, RepeaterItemEventArgs e )
		{
			if ( e.Item.ItemType == ListItemType.Header )
			{
				CheckBox deleteAllPosts = ( CheckBox )e.Item.FindControl( "DeleteAllPosts" );
				deleteAllPosts.Checked = deleteAllPosts.Enabled = PageContext.ForumModeratorAccess || PageContext.IsAdmin;
				ViewState ["delAll"] = deleteAllPosts.Checked;
			}
		}


		protected void Page_Load( object sender, System.EventArgs e )
		{
			_messageRow = null;

			if ( Request.QueryString ["m"] != null )
			{

				_messageRow =
					DBHelper.GetFirstRowOrInvalid( DB.message_list( Security.StringToLongOrRedirect( Request.QueryString["m"] ) ) );

				if ( !PageContext.ForumModeratorAccess && PageContext.PageUserID != ( int )_messageRow ["UserID"] )
				{
					YafBuildLink.AccessDenied();
				}
			}

			_forumFlags = new ForumFlags( _messageRow ["ForumFlags"] );
			_topicFlags = new TopicFlags( _messageRow ["TopicFlags"] );
			_ownerUserId = ( int )_messageRow ["UserID"];
			_isModeratorChanged = ( PageContext.PageUserID != _ownerUserId );

			if ( PageContext.PageForumID == 0 )
				YafBuildLink.AccessDenied();
			if ( Request ["t"] == null && !PageContext.ForumPostAccess )
				YafBuildLink.AccessDenied();
			if ( Request ["t"] != null && !PageContext.ForumReplyAccess )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				// setup page links
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( PageContext.PageCategoryName, YafBuildLink.GetLink( ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				PageLinks.AddForumLinks( PageContext.PageForumID );

				EraseMessage.Checked = false;
				ViewState ["delAll"] = false;
				EraseRow.Visible = false;
				DeleteReasonRow.Visible = false;
				LinkedPosts.Visible = false;
				ReasonEditor.Attributes.Add( "style", "width:100%" );
				Cancel.Text = GetText( "Cancel" );

				if ( Request.QueryString ["m"] != null )
				{
					// delete message...
					PreviewRow.Visible = true;

					DataTable tempdb = DB.message_getRepliesList( Request.QueryString ["m"] );

					if ( tempdb.Rows.Count != 0 && (PageContext.ForumModeratorAccess || PageContext.IsAdmin) )
					{
						LinkedPosts.Visible = true;
						LinkedPosts.DataSource = tempdb;
						LinkedPosts.DataBind();
					}

					if ( Request.QueryString ["action"].ToLower() == "delete" )
					{
						Title.Text = GetText( "EDIT" ); //GetText("EDIT");
						Delete.Text = GetText( "DELETE" ); // "GetText("Save");

						if ( PageContext.IsAdmin )
						{
							EraseRow.Visible = true;
						}
					}
					else
					{
						Title.Text = GetText( "EDIT" );
						Delete.Text = GetText( "UNDELETE" ); // "GetText("Save");
					}

					Subject.Text = Convert.ToString( _messageRow ["Topic"] );
					DeleteReasonRow.Visible = true;
					ReasonEditor.Text = Convert.ToString( _messageRow ["DeleteReason"] );

					// populate the message preview with the message datarow...
					MessagePreview.Message = _messageRow["message"].ToString();
					MessagePreview.MessageFlags.BitValue = Convert.ToInt32( _messageRow ["Flags"] );
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			// get the forum editor based on the settings
			//Message = yaf.editor.EditorHelper.CreateEditorFromType(PageContext.BoardSettings.ForumEditor);
			//	EditorLine.Controls.Add(Message);
			this.LinkedPosts.ItemDataBound += new System.Web.UI.WebControls.RepeaterItemEventHandler( this.LinkedPosts_ItemDataBound );

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

		}
		#endregion

		private bool PostLocked
		{
			get
			{
				if ( !PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0 )
				{
					DateTime edited = ( DateTime )_messageRow ["Edited"];
					if ( edited.AddDays( PageContext.BoardSettings.LockPosts ) < DateTime.Now )
						return true;
				}
				return false;
			}
		}

		private bool PostDeleted
		{
			get
			{

				int deleted = ( int )_messageRow ["Flags"] & 8;
				if ( deleted == 8 )
					return true;
				return false;
			}
		}

		public bool CanDeletePost
		{
			get
			{
				// Ederon : 9/9/2007 - moderators can delete in locked topics
				return ( ( !PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
						( int )_messageRow ["UserID"] == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
						PageContext.ForumDeleteAccess;
			}
		}

		public bool CanUnDeletePost
		{
			get
			{
				return PostDeleted && CanDeletePost;
			}
		}

		protected void ToogleDeleteStatus_Click( object sender, EventArgs e )
		{
			if ( !CanDeletePost )
				return;

			//Create objects for easy access
			object tmpMessageID = _messageRow ["MessageID"];
			object tmpForumID = _messageRow ["ForumID"];
			object tmpTopicID = _messageRow ["TopicID"];

			// Toogle delete message -- if the message is currently deleted it will be undeleted.
			// If it's not deleted it will be marked deleted.
			// If it is the last message of the topic, the topic is also deleted
			DB.message_delete( tmpMessageID, _isModeratorChanged, ReasonEditor.Text, PostDeleted ? 0 : 1, ( bool )ViewState ["delAll"], EraseMessage.Checked );

			// retrieve topic information.
			DataRow topic = DB.topic_info( tmpTopicID );

			//If topic has been deleted, redirect to topic list for active forum, else show remaining posts for topic
			if ( topic == null )
			{
				YafBuildLink.Redirect( ForumPages.topics, "f={0}", tmpForumID );
			}
			else
			{
				YafBuildLink.Redirect( ForumPages.posts, "t={0}", tmpTopicID );
			}
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			if ( Request.QueryString ["t"] != null || Request.QueryString ["m"] != null )
			{
				// reply to existing topic or editing of existing topic
				YafBuildLink.Redirect( ForumPages.posts, "t={0}", PageContext.PageTopicID );
			}
			else
			{
				// new topic -- cancel back to forum
				YafBuildLink.Redirect( ForumPages.topics, "f={0}", PageContext.PageForumID );
			}
		}

		protected string GetActionText()
		{
			if ( Request.QueryString ["action"].ToLower() == "delete" )
			{
				return GetText( "DELETE" );
			}
			else
			{
				return GetText( "UNDELETE" );
			}
		}

		protected string GetReasonText()
		{
			if ( Request.QueryString ["action"].ToLower() == "delete" )
			{
				return GetText( "DELETE_REASON" );
			}
			else
			{
				return GetText( "UNDELETE_REASON" );
			}
		}

		public void DeleteAllPosts_CheckedChanged1( object sender, EventArgs e )
		{
			ViewState ["delAll"] = ( ( CheckBox )sender ).Checked;
		}
	}
}
