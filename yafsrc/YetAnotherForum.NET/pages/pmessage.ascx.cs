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
using System.Data;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;
using YAF.Editors;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for pmessage.
	/// </summary>
	public partial class pmessage : ForumPage
	{

		#region Data Members

		// message body editor
		protected BaseForumEditor _editor;

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
			PageLinks.AddLink( YafContext.Current.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
			// users control panel
			PageLinks.AddLink( YafContext.Current.PageUserName, YafBuildLink.GetLink( ForumPages.cp_profile ) );
			// private messages
			PageLinks.AddLink( YafContext.Current.Localization.GetText( ForumPages.cp_pm.ToString(), "TITLE" ), YafBuildLink.GetLink( ForumPages.cp_pm ) );
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
			_editor = YafContext.Current.EditorModuleManager.GetEditorInstance( YafContext.Current.BoardSettings.ForumEditor );
			// add editor to the page
			EditorLine.Controls.Add( _editor );
		}


		/// <summary>
		/// Handles page load event.
		/// </summary>
		protected void Page_Load( object sender, EventArgs e )
		{
			// if user isn't authenticated, redirect him to login page
			if ( User == null || YafContext.Current.IsGuest )
				RedirectNoAccess();

			// set attributes of editor
			_editor.BaseDir = YafForumInfo.ForumRoot + "editors";
			_editor.StyleSheet = YafContext.Current.Theme.BuildThemePath( "theme.css" );

			// this needs to be done just once, not during postbacks
			if ( !IsPostBack )
			{
				// create page links
				CreatePageLinks();

				// localize button labels
				FindUsers.Text = GetText( "FINDUSERS" );
				AllUsers.Text = GetText( "ALLUSERS" );
				Clear.Text = GetText( "CLEAR" );

				// only administrators can send messages to all users
				AllUsers.Visible = YafContext.Current.IsAdmin;

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
						if ( toUserId != YafContext.Current.PageUserID && fromUserId != YafContext.Current.PageUserID ) YafBuildLink.AccessDenied();

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

						if ( isQuoting ) // PM is a quoted reply
						{
							string body = row ["Body"].ToString();

							if ( YafContext.Current.BoardSettings.RemoveNestedQuotes )
								body = FormatMsg.RemoveNestedQuotes(body);

							// Ensure quoted replies have bad words removed from them
							body = YafServices.BadWordReplace.Replace(body);

							// Quote the original message
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
						using ( DataTable dt = DB.user_list( YafContext.Current.PageBoardID, toUserId, true ) )
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
					if ( YafContext.Current.BoardSettings.PrivateMessageMaxRecipients > 1 )
					{
						// format localized string
						MultiReceiverInfo.Text = String.Format(
							"<br />{0}<br />{1}",
							String.Format(
								YafContext.Current.Localization.GetText( "MAX_RECIPIENT_INFO" ),
								YafContext.Current.BoardSettings.PrivateMessageMaxRecipients
								),
							YafContext.Current.Localization.GetText( "MULTI_RECEIVER_INFO" )
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
				YafContext.Current.AddLoadMessage( GetText( "need_to" ) );
				return;
			}

			// subject is required
			if ( Subject.Text.Trim().Length <= 0 )
			{
				YafContext.Current.AddLoadMessage( GetText( "need_subject" ) );
				return;
			}
			// message is required
			if ( _editor.Text.Trim().Length <= 0 )
			{
				YafContext.Current.AddLoadMessage( GetText( "need_message" ) );
				return;
			}
            
            if (ToList.SelectedItem != null && ToList.SelectedItem.Value == "0" )
			{
             
				// administrator is sending PMs tp all users           

				string body = _editor.Text;
				MessageFlags messageFlags = new MessageFlags();

				messageFlags.IsHtml = _editor.UsesHTML;
				messageFlags.IsBBCode = _editor.UsesBBCode;

                DB.pmessage_save(YafContext.Current.PageUserID, 0, Subject.Text, body, messageFlags.BitValue);

				// redirect to outbox (sent items), not control panel
				YafBuildLink.Redirect( ForumPages.cp_pm, "v={0}", "out" );
			}
			else
			{
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
				int [,] recipientID = new int [recipients.Count,2];

				if ( recipients.Count > YafContext.Current.BoardSettings.PrivateMessageMaxRecipients && !YafContext.Current.IsAdmin && YafContext.Current.BoardSettings.PrivateMessageMaxRecipients != 0 )
				{
					// to many recipients
					YafContext.Current.AddLoadMessage( GetTextFormatted( "TOO_MANY_RECIPIENTS", YafContext.Current.BoardSettings.PrivateMessageMaxRecipients ) );
					return;
				}
                
                                  
				// test sending user's PM count
                // get user's name
                
                DataRow drPMInfo = DB.user_pmcount(YafContext.Current.PageUserID).Rows[0];
				if ( ( Convert.ToInt32( drPMInfo[ "NumberTotal" ] ) > Convert.ToInt32( drPMInfo[ "NumberAllowed" ] ) + recipients.Count  ) &&
					!YafContext.Current.IsAdmin )
				{
					// user has full PM box
                    YafContext.Current.AddLoadMessage(GetTextFormatted("OWN_PMBOX_FULL", drPMInfo["NumberAllowed"]));
					return;
				}

				// get recipients' IDs
				for ( int i = 0; i < recipients.Count; i++ )
				{
					using ( DataTable dt = DB.user_find( YafContext.Current.PageBoardID, false, recipients [i], null ) )
					{
						if ( dt.Rows.Count != 1 )
						{
							YafContext.Current.AddLoadMessage( GetTextFormatted( "NO_SUCH_USER", recipients [i] ) );
							return;
						}
                        else if (SqlDataLayerConverter.VerifyInt32(dt.Rows [0] ["IsGuest"]) > 0 )						
						{
							YafContext.Current.AddLoadMessage( GetText( "NOT_GUEST" ) );
							return;
						}

						// get recipient's ID from the database
						recipientID [ i, 0 ] = Convert.ToInt32( dt.Rows [ i ] [ "UserID" ] );
                        recipientID [ i, 1 ] = Convert.ToInt32( dt.Rows [ i ][ "IsAdmin" ] );
						// test receiving user's PM count
                        
						if ( ( Convert.ToInt32( DB.user_pmcount( recipientID [ i ,0] ).Rows[ 0 ] ["NumberTotal"] ) >= Convert.ToInt32( DB.user_pmcount( recipientID [ i , 0] ).Rows[ 0 ] [ "NumberAllowed" ] ) ) &&
                            !YafContext.Current.IsAdmin && recipientID[i, 1]  == 0 )
						{                            
							// recipient has full PM box
							YafContext.Current.AddLoadMessage( GetTextFormatted( "RECIPIENTS_PMBOX_FULL", recipients [ i ] ) );
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

					DB.pmessage_save( YafContext.Current.PageUserID, recipientID [i,0], Subject.Text, body, messageFlags.BitValue );

					if ( YafContext.Current.BoardSettings.AllowPMEmailNotification )
						CreateMail.PmNotification( recipientID[i,0], Subject.Text );
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

			PreviewMessagePost.MessageFlags.IsHtml = _editor.UsesHTML;
			PreviewMessagePost.MessageFlags.IsBBCode = _editor.UsesBBCode;
			PreviewMessagePost.Message = _editor.Text;

			// set message flags
			MessageFlags tFlags = new MessageFlags();
			tFlags.IsHtml = _editor.UsesHTML;
			tFlags.IsBBCode = _editor.UsesBBCode;

			if ( YafContext.Current.BoardSettings.AllowSignatures )
			{
				using ( DataTable userDT = DB.user_list( YafContext.Current.PageBoardID, YafContext.Current.PageUserID, true ) )
				{
					if ( !userDT.Rows [0].IsNull( "Signature" ) )
					{
						PreviewMessagePost.Signature = userDT.Rows [0] ["Signature"].ToString();
					}
				}
			}
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
				YafContext.Current.AddLoadMessage( GetText( "NEED_MORE_LETTERS" ) );
				return;
			}

			// try to find users by user name
			using ( DataTable dt = DB.user_find( YafContext.Current.PageBoardID, true, To.Text, null ) )
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
			AllUsers.Visible = YafContext.Current.IsAdmin;
			// clear button is not necessary now
			Clear.Visible = false;
		}

		#endregion
	}
}
