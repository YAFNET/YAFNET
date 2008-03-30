/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for pmessage.
	/// </summary>
	public partial class pmessage : Classes.Base.ForumPage
	{

		#region Data Members

		// message body editor
		protected Editor.ForumEditor _editor;

		#endregion


		#region Construcotrs & Overridden Methods

		/// <summary>
		/// Default constructor.
		/// </summary>
		public pmessage() : base( "PMESSAGE" ) { }


		/// <summary>
		/// Creates page links for this page.
		/// </summary>
		protected override void CreatePageLinks()
		{
			// forum index
			PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
			// users control panel
			PageLinks.AddLink( PageContext.PageUserName, YafBuildLink.GetLink( ForumPages.cp_profile ) );
			// private messages
			PageLinks.AddLink( PageContext.Localization.GetText( ForumPages.cp_pm.ToString(), "TITLE" ), YafBuildLink.GetLink( ForumPages.cp_pm ) );
			// post new message
			PageLinks.AddLink( GetText( "TITLE" ) );
		}

		#endregion


		#region Event Handlers

		/// <summary>
		/// Page initialization handler.
		/// </summary>
		protected void Page_Init( object sender, EventArgs e )
		{
			// create editor based on administrator's settings
			_editor = YAF.Editor.EditorHelper.CreateEditorFromType( PageContext.BoardSettings.ForumEditor );
			// add editor to the page
			EditorLine.Controls.Add( _editor );
		}


		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load( object sender, EventArgs e )
		{
			// if user isn't authenticated, redirect him to login page
			if ( User == null || PageContext.IsGuest )
				YafBuildLink.Redirect( ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );

			// set attributes of editor
			_editor.BaseDir = YafForumInfo.ForumRoot + "editors";
			_editor.StyleSheet = YafBuildLink.ThemeFile( "theme.css" );

			// this needs to be done just once, not during postbacks
			if ( !IsPostBack )
			{
				// create page links
				CreatePageLinks();

				// localize button labels
				Save.Text = GetText( "SAVE" );
				Preview.Text = GetText( "PREVIEW" );
				Cancel.Text = GetText( "CANCEL" );
				FindUsers.Text = GetText( "FINDUSERS" );
				AllUsers.Text = GetText( "ALLUSERS" );
				Clear.Text = GetText( "CLEAR" );

				// only administrators can send messages to all users
				AllUsers.Visible = PageContext.IsAdmin;

				if ( !String.IsNullOrEmpty( Request.QueryString ["p"] ) )
				{
					// PM is a reply or quoted reply (isQuoting)
					// to the given message id "p"
					bool isQuoting = Request.QueryString ["q"] == "1";

					// get quoted message
					DataTable dt = DB.pmessage_list( Security.StringToLongOrRedirect( Request.QueryString ["p"] ) );

					// there is such a message
					if ( dt.Rows.Count > 0 )
					{
						// message info is in first row
						DataRow row = dt.Rows [0];
						// get message sender/recipient
						int toUserId = ( int )row ["ToUserID"];
						int fromUserId = ( int )row ["FromUserID"];

						// verify access to this PM
						if ( toUserId != PageContext.PageUserID && fromUserId != PageContext.PageUserID ) YafBuildLink.AccessDenied();

						// handle subject
						string subject = ( string )row ["Subject"];
						if ( !subject.StartsWith( "Re: " ) )
							subject = "Re: " + subject;
						Subject.Text = subject;

						// set "To" user and disable changing...
						To.Text = row ["FromUser"].ToString();
						To.Enabled = false;
						FindUsers.Enabled = false;
						AllUsers.Enabled = false;

						if ( isQuoting )
						{
							// PM is a quoted reply
							string body = row ["Body"].ToString();
							bool isHtml = body.IndexOf( '<' ) >= 0;

							if ( PageContext.BoardSettings.RemoveNestedQuotes )
							{
								// nested quotes need to be removed
								body = FormatMsg.RemoveNestedQuotes( body );
							}
							// quote original message
							body = String.Format( "[QUOTE={0}]{1}[/QUOTE]", row ["FromUser"], body );
							// we don't want any whitespaces at the beginning of message
							_editor.Text = body.TrimStart();
						}
					}
				}
				else if ( !String.IsNullOrEmpty( Request.QueryString ["u"] ) )
				{
					// PM is being sent to a predefined user
					int toUserId;

					if ( Int32.TryParse( Request.QueryString ["u"], out toUserId ) )
					{
						// get user's name
						using ( DataTable dt = DB.user_list( PageContext.PageBoardID, toUserId, true ) )
						{
							To.Text = dt.Rows [0] ["Name"] as string;
							To.Enabled = false;
							// hide find user/all users buttons
							FindUsers.Enabled = false;
							AllUsers.Enabled = false;
						}
					}
				}
				else
				{
					// Blank PM

					// multi-receiver info is relevant only when sending blank PM
					if ( PageContext.BoardSettings.PrivateMessageMaxRecipients > 1 )
					{
						// format localized string
						MultiReceiverInfo.Text = String.Format(
							"<br />{0}<br />{1}",
							String.Format(
								PageContext.Localization.GetText( "MAX_RECIPIENT_INFO" ),
								PageContext.BoardSettings.PrivateMessageMaxRecipients
								),
							PageContext.Localization.GetText( "MULTI_RECEIVER_INFO" )
							);
						// display info
						MultiReceiverInfo.Visible = true;
					}
				}
			}
		}


		/// <summary>
		/// Handles save button click event. 
		/// </summary>
		protected void Save_Click( object sender, EventArgs e )
		{
			// recipient was set in dropdown
			if ( ToList.Visible ) To.Text = ToList.SelectedItem.Text;
			if ( To.Text.Length <= 0 )
			{
				// recipient is required field
				PageContext.AddLoadMessage( GetText( "need_to" ) );
				return;
			}

			if ( ToList.SelectedItem != null && ToList.SelectedItem.Value == "0" )
			{
				// administrator is sending PMs tp all users

				string body = _editor.Text;
				MessageFlags messageFlags = new MessageFlags();

				messageFlags.IsHtml = _editor.UsesHTML;
				messageFlags.IsBBCode = _editor.UsesBBCode;

				DB.pmessage_save( PageContext.PageUserID, 0, Subject.Text, body, messageFlags.BitValue );

				// redirect to outbox (sent items), not control panel
				YafBuildLink.Redirect( ForumPages.cp_pm, "v={0}", "out" );
			}
			else
			{
				// subject is required
				if ( Subject.Text.Trim().Length <= 0 )
				{
					PageContext.AddLoadMessage( GetText( "need_subject" ) );
					return;
				}
				// message is required
				if ( _editor.Text.Trim().Length <= 0 )
				{
					PageContext.AddLoadMessage( GetText( "need_message" ) );
					return;
				}

				// remove all abundant whitespaces and separators
				To.Text.Trim();
				Regex rx = new Regex( @";(\s|;)*;" );
				To.Text = rx.Replace( To.Text, ";" );
				if ( To.Text.StartsWith( ";" ) ) To.Text = To.Text.Substring( 1 );
				if ( To.Text.EndsWith( ";" ) ) To.Text = To.Text.Substring( 0, To.Text.Length - 1 );
				rx = new Regex( @"\s*;\s*" );
				To.Text = rx.Replace( To.Text, ";" );

				// list of recipients
				List<string> recipients = new List<string>( To.Text.Split( ';' ) );
				// list of recipient's ids
				int [] recipientID = new int [recipients.Count];

				if ( recipients.Count > PageContext.BoardSettings.PrivateMessageMaxRecipients && !PageContext.IsAdmin )
				{
					// to many recipients
					PageContext.AddLoadMessage( String.Format( GetText( "TOO_MANY_RECIPIENTS" ), PageContext.BoardSettings.PrivateMessageMaxRecipients ) );
					return;
				}

				// test sending user's PM count
				if ( PageContext.BoardSettings.MaxPrivateMessagesPerUser != 0 &&
					( DB.user_pmcount( PageContext.PageUserID ) + recipients.Count ) > PageContext.BoardSettings.MaxPrivateMessagesPerUser &&
					!PageContext.IsAdmin )
				{
					// user has full PM box
					PageContext.AddLoadMessage( String.Format( GetText( "OWN_PMBOX_FULL" ), PageContext.BoardSettings.MaxPrivateMessagesPerUser ) );
					return;
				}

				// get recipients' IDs
				for ( int i = 0; i < recipients.Count; i++ )
				{
					using ( DataTable dt = DB.user_find( PageContext.PageBoardID, false, recipients [i], null ) )
					{
						if ( dt.Rows.Count != 1 )
						{
							PageContext.AddLoadMessage( String.Format( GetText( "NO_SUCH_USER" ), recipients [i] ) );
							return;
						}
						else if ( ( int )dt.Rows [0] ["IsGuest"] > 0 )
						{
							PageContext.AddLoadMessage( GetText( "NOT_GUEST" ) );
							return;
						}

						// get recipient's ID from the database
						recipientID [i] = Convert.ToInt32( dt.Rows [0] ["UserID"] );

						// test receiving user's PM count
						if ( PageContext.BoardSettings.MaxPrivateMessagesPerUser != 0 &&
							DB.user_pmcount( recipientID [i] ) >= PageContext.BoardSettings.MaxPrivateMessagesPerUser &&
							!PageContext.IsAdmin )
						{
							// recipient has full PM box
							PageContext.AddLoadMessage( String.Format( GetText( "RECIPIENTS_PMBOX_FULL" ), recipients [i] ) );
							return;
						}
					}
				}

				// send PM to all recipients
				for ( int i = 0; i < recipients.Count; i++ )
				{
					string body = _editor.Text;

					MessageFlags messageFlags = new MessageFlags();

					messageFlags.IsHtml = _editor.UsesHTML;
					messageFlags.IsBBCode = _editor.UsesBBCode;

					DB.pmessage_save( PageContext.PageUserID, recipientID [i], Subject.Text, body, messageFlags.BitValue );

					if ( PageContext.BoardSettings.AllowPMEmailNotification )
						SendPMNotification( recipientID [i], Subject.Text );
				}

				// redirect to outbox (sent items), not control panel
				YafBuildLink.Redirect( ForumPages.cp_pm, "v={0}", "out" );
			}
		}


		/// <summary>
		/// Handles preview button click event.
		/// </summary>
		protected void Preview_Click( object sender, EventArgs e )
		{
			// make preview row visible
			PreviewRow.Visible = true;

			// set message flags
			MessageFlags tFlags = new MessageFlags();
			tFlags.IsHtml = _editor.UsesHTML;
			tFlags.IsBBCode = _editor.UsesBBCode;

			// format message body
			string body = FormatMsg.FormatMessage( _editor.Text, tFlags );

			// attack sender's signature
			using ( DataTable dt = DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
			{
				if ( !dt.Rows [0].IsNull( "Signature" ) )
					body += "<br/><hr noshade/>" + FormatMsg.FormatMessage( dt.Rows [0] ["Signature"].ToString(), new MessageFlags() );
			}

			// display preview HTML code in preview cell
			PreviewCell.InnerHtml = body;
		}


		/// <summary>
		/// Handles cancel button click event.
		/// </summary>
		protected void Cancel_Click( object sender, EventArgs e )
		{
			// redirect user back to his PM inbox
			YafBuildLink.Redirect( ForumPages.cp_pm );
		}


		/// <summary>
		/// Handles find users button click event.
		/// </summary>
		protected void FindUsers_Click( object sender, EventArgs e )
		{
			if ( To.Text.Length < 2 )
			{
				// need at least 2 latters of user's name
				PageContext.AddLoadMessage( GetText( "NEED_MORE_LETTERS" ) );
				return;
			}

			// try to find users by user name
			using ( DataTable dt = DB.user_find( PageContext.PageBoardID, true, To.Text, null ) )
			{
				if ( dt.Rows.Count > 0 )
				{
					// we found a user(s)
					ToList.DataSource = dt;
					ToList.DataValueField = "UserID";
					ToList.DataTextField = "Name";
					ToList.DataBind();
					//ToList.SelectedIndex = 0;
					// hide To text box and show To drop down
					ToList.Visible = true;
					To.Visible = false;
					// find is no more needed
					FindUsers.Visible = false;
					// we need clear button displayed now
					Clear.Visible = true;
				}

				// re-bind data to the controls
				DataBind();
			}
		}


		/// <summary>
		/// Handles all users button click event.
		/// </summary>
		protected void AllUsers_Click( object sender, EventArgs e )
		{
			// create one entry to show in dropdown
			ListItem li = new ListItem( "All Users", "0" );

			// bind the list to dropdown
			ToList.Items.Add( li );
			ToList.Visible = true;
			To.Text = "All Users";
			// hide To text box
			To.Visible = false;
			// hide find users/all users buttons
			FindUsers.Visible = false;
			AllUsers.Visible = false;
			// we need clear button now
			Clear.Visible = true;
		}


		/// <summary>
		/// Handles clear button click event.
		/// </summary>
		protected void Clear_Click( object sender, EventArgs e )
		{
			// clear drop down
			ToList.Items.Clear();
			// hide it and show empty To text box
			ToList.Visible = false;
			To.Text = "";
			To.Visible = true;
			// show find users and all users (if user is admin)
			FindUsers.Visible = true;
			AllUsers.Visible = PageContext.IsAdmin;
			// clear button is not necessary now
			Clear.Visible = false;
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Sends notification about new PM in user's inbox.
		/// </summary>
		/// <param name="toUserID">User supposed to receive notification about new PM.</param>
		/// <param name="subject">Subject of PM user is notified about.</param>
		private void SendPMNotification( int toUserID, string subject )
		{
			/// TODO : add email to email queue?

			try
			{
				// user's PM notification setting
				bool pmNotificationAllowed;
				// user's email
				string toEMail;

				// read user's info from DB
				using ( DataTable dt = DB.user_list( PageContext.PageBoardID, toUserID, true ) )
				{
					pmNotificationAllowed = ( bool )dt.Rows [0] ["PMNotification"];
					toEMail = ( string )dt.Rows [0] ["EMail"];
				}

				if ( pmNotificationAllowed )
				{
					// user has PM notification set on

					int userPMessageID;
					//string senderEmail;

					// get the PM ID
					// Ederon : 11/21/2007 - PageBoardID as parameter of DB.pmessage_list?
					// using (DataTable dt = DB.pmessage_list(toUserID, PageContext.PageBoardID, null))
					using ( DataTable dt = DB.pmessage_list( toUserID, null, null ) )
						userPMessageID = ( int )dt.Rows [0] ["UserPMessageID"];

					// get the sender e-mail -- DISABLED: too much information...
					//using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
					//	senderEmail = ( string ) dt.Rows [0] ["Email"];

					// send this user a PM notification e-mail
					YafTemplateEmail pmNotification = new YafTemplateEmail( "PMNOTIFICATION" );

					// fill the template with relevant info
					pmNotification.TemplateParams ["{fromuser}"] = PageContext.PageUserName;
					pmNotification.TemplateParams ["{link}"] = String.Format( "{1}{0}\r\n\r\n", YafBuildLink.GetLinkNotEscaped( ForumPages.cp_message, "pm={0}", userPMessageID ), YafForumInfo.ServerURL );
					pmNotification.TemplateParams ["{forumname}"] = PageContext.BoardSettings.Name;
					pmNotification.TemplateParams ["{subject}"] = subject;

					// create notification email subject
					string emailSubject = string.Format( GetText( "COMMON", "PM_NOTIFICATION_SUBJECT" ), PageContext.PageUserName, PageContext.BoardSettings.Name, subject );

					// send email
					pmNotification.SendEmail( new System.Net.Mail.MailAddress( toEMail ), subject, true );
				}
			}
			catch ( Exception x )
			{
				// report exception to the forum's event log
				DB.eventlog_create( PageContext.PageUserID, this, x );
				// tell user about failure
				PageContext.AddLoadMessage( String.Format( GetText( "failed" ), x.Message ) );
			}
		}

		#endregion
	}
}