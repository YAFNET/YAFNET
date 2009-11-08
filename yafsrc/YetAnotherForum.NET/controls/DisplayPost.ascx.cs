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
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	///		Summary description for DisplayPost.
	/// </summary>
	public partial class DisplayPost : YAF.Classes.Core.BaseUserControl
	{
		#region Data Members

		// for parent page referencing
		private YAF.Classes.Core.ForumPage _parentPage;

		private DataRowView _row = null;
		private YafUserProfile _userProfile = null;
		private bool _isAlt = false;
		private bool _isThreaded = false;
		// flags
		private ForumFlags _forumFlags;
		private TopicFlags _topicFlags;
		private MessageFlags _messageFlags;

		#endregion

		#region Helper Properties

		protected int UserId
		{
			get
			{
				if ( this.DataRow != null )
				{
					return Convert.ToInt32( DataRow["UserID"] );
				}

				return 0;
			}
		}

		protected int MessageId
		{
			get
			{
				if ( this.DataRow != null )
				{
					return Convert.ToInt32( DataRow["MessageID"] );
				}

				return 0;
			}
		}

		#endregion

		protected void Page_Load( object sender, System.EventArgs e )
		{
			AjaxPro.Utility.RegisterTypeForAjax( typeof( YAF.Controls.ThankYou ) );


			string AddThankBoxHTML = "'<a class=\"yaflittlebutton\" href=\"javascript:addThanks(' + res.value.MessageID + ');\" onclick=\"this.blur();\" title=' + res.value.Title + '><span>' + res.value.Text + '</span></a>'";

			string RemoveThankBoxHTML = "'<a class=\"yaflittlebutton\" href=\"javascript:removeThanks(' + res.value.MessageID + ');\" onclick=\"this.blur();\" title=' + res.value.Title + '><span>' + res.value.Text + '</span></a>'";

			YafContext.Current.PageElements.RegisterJsBlockStartup( "addThanksJs", YAF.Utilities.JavaScriptBlocks.addThanksJs( RemoveThankBoxHTML ) );
			YafContext.Current.PageElements.RegisterJsBlockStartup( "removeThanksJs", YAF.Utilities.JavaScriptBlocks.removeThanksJs( AddThankBoxHTML ) );
			YafContext.Current.PageElements.RegisterJsBlockStartup( "asynchCallFailedJs", YAF.Utilities.JavaScriptBlocks.asynchCallFailedJs );

			PopMenu1.Visible = !IsGuest;
			if ( PopMenu1.Visible )
			{
				PopMenu1.ItemClick += new PopEventHandler( PopMenu1_ItemClick );
				PopMenu1.AddPostBackItem( "userprofile", PageContext.Localization.GetText( "POSTS", "USERPROFILE" ) );

				if ( PageContext.IsAdmin ) PopMenu1.AddPostBackItem( "edituser", "Edit User (Admin)" );
				if ( !PageContext.IsGuest )
				{
					if ( IsIgnored( UserId ) )
					{
						PopMenu1.AddPostBackItem( "toggleuserposts_show", PageContext.Localization.GetText( "POSTS", "TOGGLEUSERPOSTS_SHOW" ) );
					}
					else
					{
						PopMenu1.AddPostBackItem( "toggleuserposts_hide", PageContext.Localization.GetText( "POSTS", "TOGGLEUSERPOSTS_HIDE" ) );
					}
				}
				PopMenu1.Attach( UserProfileLink );
			}

			// setup jQuery, LightBox and YAF JS...
			YafContext.Current.PageElements.RegisterJQuery();
			YafContext.Current.PageElements.RegisterJsResourceInclude( "yafjs", "js/yaf.js" );
			YafContext.Current.PageElements.RegisterJsBlock( "toggleMessageJs", YAF.Utilities.JavaScriptBlocks.ToogleMessageJs );

			// lightbox only need if the browser is not IE6...
			if ( !UserAgentHelper.IsBrowserIE6() )
			{
				YafContext.Current.PageElements.RegisterJsResourceInclude( "lightboxjs", "js/jquery.lightbox.min.js" );
				YafContext.Current.PageElements.RegisterCssIncludeResource( "css/jquery.lightbox.css" );
				YafContext.Current.PageElements.RegisterJsBlock( "lightboxloadjs", YAF.Utilities.JavaScriptBlocks.LightBoxLoadJs );
			}

			NameCell.ColSpan = int.Parse( GetIndentSpan() );

			if ( PageContext.IsGuest )
			{
				btnTogglePost.Visible = false;
			}
			else if ( IsIgnored( UserId ) )
			{
				btnTogglePost.Visible = true;
				btnTogglePost.Attributes["onclick"] = string.Format( "toggleMessage('{0}'); return false;", panMessage.ClientID );
				panMessage.Style["display"] = "none";
			}
		}

		private void DisplayPost_PreRender( object sender, EventArgs e )
		{
			Attach.Visible = !PostDeleted && CanAttach && !IsLocked;

			Attach.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.attachments, "m={0}", MessageId );
			Edit.Visible = !PostDeleted && CanEditPost && !IsLocked;
			Edit.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.postmessage, "m={0}", MessageId );
			MovePost.Visible = PageContext.ForumModeratorAccess && !IsLocked;
			MovePost.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.movemessage, "m={0}", MessageId );
			Delete.Visible = !PostDeleted && CanDeletePost && !IsLocked;
			Delete.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.deletemessage, "m={0}&action=delete", MessageId );
			UnDelete.Visible = CanUnDeletePost && !IsLocked;
			UnDelete.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.deletemessage, "m={0}&action=undelete", MessageId );
			Quote.Visible = !PostDeleted && CanReply && !IsLocked;
			Quote.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.postmessage, "t={0}&f={1}&q={2}", PageContext.PageTopicID, PageContext.PageForumID, MessageId );

			Thank.Visible = CanThankPost && !PageContext.IsGuest;
			if ( DB.message_isThankedByUser( PageContext.PageUserID, DataRow["MessageID"] ) )
			{
				Thank.NavigateUrl = "javascript:removeThanks(" + DataRow["MessageID"] + ");";
				Thank.TextLocalizedTag = "BUTTON_THANKSDELETE";
				Thank.TitleLocalizedTag = "BUTTON_THANKSDELETE_TT";
			}
			else
			{
				Thank.NavigateUrl = "javascript:addThanks(" + DataRow["MessageID"] + ");";
				Thank.TextLocalizedTag = "BUTTON_THANKS";
				Thank.TitleLocalizedTag = "BUTTON_THANKS_TT";
			}

			int thanksNumber = DB.message_ThanksNumber( DataRow["MessageID"] );
			if ( thanksNumber != 0 )
			{
				Literal2.Text = ThankYou.GetThanks( Convert.ToInt32( DataRow["MessageID"] ) );
				if ( thanksNumber == 1 )
				{
					Literal1.Text = String.Format( PageContext.Localization.GetText( "THANKSINFOSINGLE" ), UserProfile.UserName );
				}
				else
				{
					Literal1.Text = String.Format( PageContext.Localization.GetText( "THANKSINFO" ), thanksNumber, UserProfile.UserName );
				}
			}

            // report posts
            ReportPostLinkButton.Visible = PageContext.BoardSettings.AllowReportAbuse && !IsGuest; // vzrus Addition 08/18/2007
            ReportPostLinkButton.Text = PageContext.Localization.GetText("REPORTPOST"); // Mek Addition 08/18/2007
            ReportPostLinkButton.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTPOST")));
			
            // report abuse posts

            ReportAbuseLinkButton.Visible = PageContext.BoardSettings.AllowReportAbuse && !IsGuest; // Mek Addition 08/18/2007
            ReportAbuseLinkButton.Text = PageContext.Localization.GetText("REPORTABUSE"); // Mek Addition 08/18/2007
            ReportAbuseLinkButton.Attributes.Add("onclick", String.Format("return confirm('{0}');", PageContext.Localization.GetText("CONFIRM_REPORTABUSE")));

			// report spam
			ReportSpamButton.Visible = PageContext.BoardSettings.AllowReportSpam && !IsGuest; // Mek Addition 08/18/2007
			ReportSpamButton.Text = PageContext.Localization.GetText( "REPORTSPAM" ); // Mek Addition 08/18/2007
			ReportSpamButton.Attributes.Add( "onclick", String.Format( "return confirm('{0}');", PageContext.Localization.GetText( "CONFIRM_REPORTSPAM" ) ) );

			// private messages
			Pm.Visible = !IsGuest && !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowPrivateMessages && !IsSponserMessage;
			Pm.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.pmessage, "u={0}", UserId );

			// emailing
			Email.Visible = !IsGuest && !PostDeleted && PageContext.User != null && PageContext.BoardSettings.AllowEmailSending && !IsSponserMessage;
			Email.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_email, "u={0}", UserId );

			// home page
			Home.Visible = !PostDeleted && !String.IsNullOrEmpty( UserProfile.Homepage );
			SetupThemeButtonWithLink( Home, UserProfile.Homepage );

			// blog page
			Blog.Visible = !PostDeleted && !String.IsNullOrEmpty( UserProfile.Blog );
			SetupThemeButtonWithLink( Blog, UserProfile.Blog );

			// MSN
			Msn.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.MSN );
			Msn.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_email, "u={0}", UserId );

			// Yahoo IM
			Yim.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.YIM );
			Yim.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_yim, "u={0}", UserId );

			// AOL IM
			Aim.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.AIM );
			Aim.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_aim, "u={0}", UserId );

			// ICQ
			Icq.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.ICQ );
			Icq.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_icq, "u={0}", UserId );

			// Skype
			Skype.Visible = !PostDeleted && PageContext.User != null && !String.IsNullOrEmpty( UserProfile.Skype );
			Skype.NavigateUrl = YafBuildLink.GetLinkNotEscaped( ForumPages.im_skype, "u={0}", UserId );

			if ( !PostDeleted )
			{
				AdminInformation.InnerHtml = @"<span class=""smallfont"">";
				if ( Convert.ToDateTime( DataRow["Edited"] ) > Convert.ToDateTime( DataRow["Posted"] ).AddSeconds( PageContext.BoardSettings.EditTimeOut ) )
				{
					// message has been edited
					// show, why the post was edited or deleted?
					string whoChanged = ( Convert.ToBoolean( DataRow["IsModeratorChanged"] ) ) ? PageContext.Localization.GetText( "EDITED_BY_MOD" ) : PageContext.Localization.GetText( "EDITED_BY_USER" );
					AdminInformation.InnerHtml += String.Format( @"| <span class=""editedinfo"">{0} {1}:</span> {2}", PageContext.Localization.GetText( "EDITED" ), whoChanged, YafServices.DateTime.FormatDateTimeShort( Convert.ToDateTime( DataRow["Edited"] ) ) );
					if ( Server.HtmlDecode( Convert.ToString( DataRow["EditReason"] ) ) != "" )
					{
						// reason was specified
						AdminInformation.InnerHtml += String.Format( " |<b> {0}:</b> {1}", PageContext.Localization.GetText( "EDIT_REASON" ), FormatMsg.RepairHtml( (string)DataRow["EditReason"], true ) );
					}
					else
					{
						//reason was not specified
						AdminInformation.InnerHtml += String.Format( " |<b> {0}:</b> {1}", PageContext.Localization.GetText( "EDIT_REASON" ), PageContext.Localization.GetText( "EDIT_REASON_NA" ) );
					}
				}
			}
			else
			{
				AdminInformation.InnerHtml = @"<span class=""smallfont"">";
				if ( Server.HtmlDecode( Convert.ToString( DataRow["DeleteReason"] ) ) != String.Empty )
				{
					// reason was specified
					AdminInformation.InnerHtml += String.Format( " |<b> {0}:</b> {1}", PageContext.Localization.GetText( "EDIT_REASON" ), FormatMsg.RepairHtml( (string)DataRow["DeleteReason"], true ) );
				}
				else
				{
					//reason was not specified
					AdminInformation.InnerHtml += String.Format( " |<b> {0}:</b> {1}", PageContext.Localization.GetText( "EDIT_REASON" ), PageContext.Localization.GetText( "EDIT_REASON_NA" ) );
				}
			}

			// display admin only info
			if ( PageContext.IsAdmin )
			{
				AdminInformation.InnerHtml += String.Format( " |<b> {0}:</b> {1}", PageContext.Localization.GetText( "IP" ), DataRow["IP"].ToString() );
			}
			AdminInformation.InnerHtml += "</span>";
		}

		override protected void OnInit( EventArgs e )
		{
            ReportPostLinkButton.Command += new CommandEventHandler(Report_Command);
            ReportAbuseLinkButton.Command += new CommandEventHandler(Report_Command);
			ReportSpamButton.Command += new CommandEventHandler( Report_Command );
			this.PreRender += new EventHandler( DisplayPost_PreRender );
			this.Init += new EventHandler( DisplayPost_Init );
			base.OnInit( e );
		}

		void DisplayPost_Init( object sender, EventArgs e )
		{
			// retrieves parent page (ForumPage)
			RetrieveParentPage();
		}

		protected void SetupThemeButtonWithLink( ThemeButton thisButton, string linkUrl )
		{
			if ( !String.IsNullOrEmpty( linkUrl ) )
			{
				string link = linkUrl.Replace( "\"", "" );
				if ( !link.ToLower().StartsWith( "http" ) )
				{
					link = "http://" + link;
				}
				thisButton.NavigateUrl = link;
				thisButton.Attributes.Add( "target", "_blank" );
				if ( PageContext.BoardSettings.UseNoFollowLinks ) thisButton.Attributes.Add( "rel", "nofollow" );
			}
			else
			{
				thisButton.NavigateUrl = "";
			}
		}

		/// <summary>
		/// Retrieve parent page (ForumPage) if it exists.
		/// </summary>
		protected void RetrieveParentPage()
		{
			// get parent page (ForumPage type), if applicable
			System.Web.UI.Control parent = this.Parent;
			// cycle until no there is no parent 
			while ( parent != null )
			{
				// is parent control of desired type?
				if ( parent is YAF.Classes.Core.ForumPage )
				{
					_parentPage = (YAF.Classes.Core.ForumPage)parent;
					break;
				}
				else
				{
					// go one step up in hierarchy
					parent = parent.Parent;

					// are we topmost?
					if ( parent == null )
					{
						_parentPage = null;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets parent forum page (null if parent is not ForumPage).
		/// </summary>
		public YAF.Classes.Core.ForumPage ParentPage
		{
			get { return _parentPage; }
		}


		public DataRowView DataRow
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;

				// get all flags for forum, topic and message
				if ( _row != null )
				{
					_forumFlags = new ForumFlags( _row["ForumFlags"] );
					_topicFlags = new TopicFlags( _row["TopicFlags"] );
					_messageFlags = new MessageFlags( _row["Flags"] );
				}
				else
				{
					_forumFlags = new ForumFlags( 0 );
					_topicFlags = new TopicFlags( 0 );
					_messageFlags = new MessageFlags( 0 );
				}
			}
		}

		public bool IsGuest
		{
			get
			{
				if ( DataRow != null )
				{
					return UserMembershipHelper.IsGuestUser( UserId );
				}
				else return true;
			}
		}

		/// <summary>
		/// IsLocked flag should only be used for "ghost" posts such as the
		/// Sponser post that isn't really there.
		/// </summary>
		public bool IsLocked
		{
			get
			{
				if ( _messageFlags != null )
				{
					return _messageFlags.IsLocked;
				}

				return false;
			}
		}

		public YafUserProfile UserProfile
		{
			get
			{
				if ( _userProfile == null )
				{
					// setup instance of the user profile...
					_userProfile = YafUserProfile.GetProfile( UserMembershipHelper.GetUserNameFromID( UserId ) );
				}

				return _userProfile;
			}
		}

		/*private MessageFlags PostMessageFlags
		{
			get
			{
				return new MessageFlags( Convert.ToInt32( DataRow ["Flags"] ) );
			}
		}*/

		protected bool IsSponserMessage
		{
			get
			{
				return ( DataRow["IP"].ToString() == "none" );
			}
		}

		protected bool CanThankPost
		{
			get
			{
				return ( (int)DataRow["UserID"] != PageContext.PageUserID );
			}
		}

		protected bool CanEditPost
		{
			get
			{
				// Ederon : 9/9/2007 - moderaotrs can edit locked posts
				// Ederon : 12/5/2007 - new flags implementation
				return ( ( !PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
					UserId == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
					PageContext.ForumEditAccess;
			}
		}

		private bool PostLocked
		{
			get
			{
				// post is explicitly locked
				if ( _messageFlags.IsLocked ) return true;

				// there is auto-lock period defined
				if ( !PageContext.IsAdmin && PageContext.BoardSettings.LockPosts > 0 )
				{
					DateTime edited = (DateTime)DataRow["Edited"];
					// check if post is locked according to this rule
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
				return _messageFlags.IsDeleted;
			}
		}

		protected bool CanAttach
		{
			get
			{
				// Ederon : 9/9/2007 - moderaotrs can attack to locked posts
				return ( ( !PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
					UserId == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
					PageContext.ForumUploadAccess;
			}
		}

		protected bool CanDeletePost
		{
			get
			{
				// Ederon : 9/9/2007 - moderaotrs can delete in locked posts
				return ( ( !PostLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked &&
					UserId == PageContext.PageUserID ) || PageContext.ForumModeratorAccess ) &&
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

		protected bool CanReply
		{
			get
			{
				// Ederon : 9/9/2007 - moderaotrs can reply in locked posts
				return ( ( !_messageFlags.IsLocked && !_forumFlags.IsLocked && !_topicFlags.IsLocked ) ||
					PageContext.ForumModeratorAccess ) && PageContext.ForumReplyAccess;
			}
		}

		public bool IsAlt
		{
			get { return this._isAlt; }
			set { this._isAlt = value; }
		}

		public bool IsThreaded
		{
			get
			{
				return _isThreaded;
			}
			set
			{
				_isThreaded = value;
			}
		}

		protected string GetIndentCell()
		{
			if ( !IsThreaded )
				return "";

			int iIndent = (int)DataRow["Indent"];
			if ( iIndent > 0 )
				return string.Format( @"<td rowspan=""3"" width=""1%""><img src=""{1}images/spacer.gif"" width=""{0}"" height=""2"" alt=""""/></td>", iIndent * 32, YafForumInfo.ForumRoot );
			else
				return "";
		}

		protected string GetIndentSpan()
		{
			if ( !IsThreaded || (int)DataRow["Indent"] == 0 )
				return "2";
			else
				return "1";
		}

		protected string GetPostClass()
		{
			if ( this.IsAlt )
				return "post_alt";
			else
				return "post";
		}

		// Prevents a high user box when displaying a deleted post.
		protected string GetUserBoxHeight()
		{
			if ( PostDeleted )
				return "0";
			return "100";
		}

		private List<int> _userIgnoreList = null;
		private bool IsIgnored( int ignoredUserId )
		{
			if ( _userIgnoreList == null )
			{
				_userIgnoreList = YafServices.DBBroker.UserIgnoredList( PageContext.PageUserID );
			}

			if ( _userIgnoreList.Count > 0 )
			{
				return _userIgnoreList.Contains( ignoredUserId );
			}

			return false;
		}

		private void ClearIgnoreCache()
		{
			// clear for the session
			string key = YafCache.GetBoardCacheKey( String.Format( Constants.Cache.UserIgnoreList, PageContext.PageUserID ) );
			Session.Remove( key );
		}

		private void AddIgnored( int ignoredUserId )
		{
			YAF.Classes.Data.DB.user_addignoreduser( PageContext.PageUserID, ignoredUserId );
			ClearIgnoreCache();
		}

		private void RemoveIgnored( int ignoredUserId )
		{
			YAF.Classes.Data.DB.user_removeignoreduser( PageContext.PageUserID, ignoredUserId );
			ClearIgnoreCache();
		}

		private void PopMenu1_ItemClick( object sender, PopEventArgs e )
		{
			switch ( e.Item )
			{
				case "userprofile":
					YafBuildLink.Redirect( ForumPages.profile, "u={0}", UserId );
					break;
				case "edituser":
					YafBuildLink.Redirect( ForumPages.admin_edituser, "u={0}", UserId );
					break;
				case "toggleuserposts_show":
					RemoveIgnored( UserId );
					Response.Redirect( Request.RawUrl );
					break;
				case "toggleuserposts_hide":
					AddIgnored( UserId );
					Response.Redirect( Request.RawUrl );
					break;
			}
		}

		// <Summary> Command Button - Report post as Abusive/Spam </Summary>
		protected void Report_Command( object sender, CommandEventArgs e )
		{
			int ReportFlag = 0;
			switch ( e.CommandName )
			{
				case "ReportAbuse":
					ReportFlag = 7;
					break;
				case "ReportSpam":
					ReportFlag = 8;
					break;
                case "ReportPost":
                    ReportFlag = 9;
                    break;
			}
			string reportMessage;
			switch ( ReportFlag )
			{
				case 7:
					reportMessage = PageContext.CurrentForumPage.GetText( "REPORTED" );
					break;
				case 8:
					reportMessage = PageContext.CurrentForumPage.GetText( "REPORTEDSPAM" );
					break;
				default:
                    // TODO: vzrus: required a window to enter custom report text with Report and Cancel buttons 
                    // Not sure how to implement it YAF-like ;)                  
					reportMessage = "Message reported!";
					break;
			}
            
			YAF.Classes.Data.DB.message_report( ReportFlag, e.CommandArgument.ToString(), PageContext.PageUserID, DateTime.Today, reportMessage );
			PageContext.AddLoadMessage( PageContext.Localization.GetText( "REPORTEDFEEDBACK" ) );
		}
	}
}
