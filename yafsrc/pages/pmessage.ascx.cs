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
using System.Collections.Specialized;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for pmessage.
	/// </summary>
	public partial class pmessage : ForumPage
	{
		protected YAF.Editor.ForumEditor Editor;

		public pmessage()
			: base( "PMESSAGE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			Editor.BaseDir = Data.ForumRoot + "editors";
			Editor.StyleSheet = this.ThemeFile( "theme.css" );

			if(User==null)
			{
				if(CanLogin)
					Forum.Redirect( ForumPages.login, "ReturnUrl={0}", Utils.GetSafeRawUrl() );
				else
					Forum.Redirect( ForumPages.forum );
			}

			if ( !IsPostBack )
			{

				BindData();
				PageLinks.AddLink( BoardSettings.Name, Forum.GetLink( ForumPages.forum ) );
				Save.Text = GetText( "Save" );
				Cancel.Text = GetText( "Cancel" );
				FindUsers.Text = GetText( "FINDUSERS" );
				AllUsers.Text = GetText( "ALLUSERS" );

				if ( IsAdmin )
				{
					AllUsers.Visible = true;
				}
				else
				{
					AllUsers.Visible = false;
				}

				int ToUserID = 0;

				if ( Request.QueryString ["p"] != null )
				{
					using ( DataTable dt = YAF.Classes.Data.DB.userpmessage_list( Request.QueryString ["p"] ) )
					{
						DataRow row = dt.Rows [0];
						Subject.Text = ( string ) row ["Subject"];

						if ( Subject.Text.Length < 4 || Subject.Text.Substring( 0, 4 ) != "Re: " )
							Subject.Text = "Re: " + Subject.Text;

						ToUserID = ( int ) row ["FromUserID"];
					}
				}


				if ( Request.QueryString ["p"] != null )
				{
					using ( DataTable dt = YAF.Classes.Data.DB.userpmessage_list( Request.QueryString ["p"] ) )
					{
						// default is quote
						bool bQuote = true;

						if ( Request.QueryString ["q"] != null && Request.QueryString ["q"] == "0" )
							bQuote = false;

						DataRow row = dt.Rows [0];

						if ( ( int ) row ["ToUserID"] != PageUserID && ( int ) row ["FromUserID"] != PageUserID )
							Data.AccessDenied();

						Subject.Text = ( string ) row ["Subject"];

						if ( Subject.Text.Length < 4 || Subject.Text.Substring( 0, 4 ) != "Re: " )
							Subject.Text = "Re: " + Subject.Text;

						ToUserID = ( int ) row ["FromUserID"];

						if ( bQuote )
						{
							string body = row ["Body"].ToString();
							bool isHtml = body.IndexOf( '<' ) >= 0;

							if ( BoardSettings.RemoveNestedQuotes )
							{
								RegexOptions m_options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
								Regex quote = new Regex( @"\[quote(\=.*)?\](.*?)\[/quote\]", m_options );
								// remove quotes from old messages
								body = quote.Replace( body, "" );
							}
							body = String.Format( "[QUOTE={0}]{1}[/QUOTE]", row ["FromUser"], body );
							Editor.Text = body;
						}
					}
				}

				if ( Request.QueryString ["u"] != null )
					ToUserID = int.Parse( Request.QueryString ["u"].ToString() );

				if ( ToUserID != 0 )
				{
					using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageBoardID, ToUserID, true ) )
					{
						To.Text = ( string ) dt.Rows [0] ["Name"];
						To.Enabled = false;
					}
				}
			}
		}

		private void BindData()
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			Editor = YAF.Editor.EditorHelper.CreateEditorFromType( BoardSettings.ForumEditor );
			EditorLine.Controls.Add( Editor );

			this.Save.Click += new System.EventHandler( this.Save_Click );
			this.Cancel.Click += new System.EventHandler( this.Cancel_Click );
			this.FindUsers.Click += new System.EventHandler( this.FindUsers_Click );
			this.AllUsers.Click += new System.EventHandler( this.AllUsers_Click );
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

		private void Save_Click( object sender, System.EventArgs e )
		{
			if ( To.Text.Length <= 0 )
			{
				AddLoadMessage( GetText( "need_to" ) );
				return;
			}
			if ( ToList.Visible )
				To.Text = ToList.SelectedItem.Text;


			if ( ToList.SelectedItem != null && ToList.SelectedItem.Value == "0" )
			{
				string body = Editor.Text;
				MessageFlags tFlags = new MessageFlags();
				tFlags.IsHTML = Editor.UsesHTML;
				tFlags.IsBBCode = Editor.UsesBBCode;
				YAF.Classes.Data.DB.pmessage_save( PageUserID, 0, Subject.Text, body, tFlags.BitValue );
				Forum.Redirect( ForumPages.cp_profile );
			}
			else
			{
				using ( DataTable dt = YAF.Classes.Data.DB.user_find( PageBoardID, false, To.Text, null ) )
				{
					if ( dt.Rows.Count != 1 )
					{
						AddLoadMessage( GetText( "NO_SUCH_USER" ) );
						return;
					}
					else if ( ( int ) dt.Rows [0] ["IsGuest"] > 0 )
					{
						AddLoadMessage( GetText( "NOT_GUEST" ) );
						return;
					}

					if ( Subject.Text.Length <= 0 )
					{
						AddLoadMessage( GetText( "need_subject" ) );
						return;
					}
					if ( Editor.Text.Length <= 0 )
					{
						AddLoadMessage( GetText( "need_message" ) );
						return;
					}

					string body = Editor.Text;

					MessageFlags tFlags = new MessageFlags();
					tFlags.IsHTML = Editor.UsesHTML;
					tFlags.IsBBCode = Editor.UsesBBCode;

					YAF.Classes.Data.DB.pmessage_save( PageUserID, dt.Rows [0] ["UserID"], Subject.Text, body, tFlags.BitValue );

					if ( BoardSettings.AllowPMEmailNotification )
						SendPMNotification( Convert.ToInt32(dt.Rows [0] ["UserID"]), Subject.Text );

					Forum.Redirect( ForumPages.cp_profile );
				}
			}
		}

		private void SendPMNotification(int toUserID, string subject)
		{
			try
			{
				bool pmNotificationAllowed;
				string toEMail;

				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageBoardID, toUserID, true ) )
				{
					pmNotificationAllowed = ( bool ) dt.Rows [0] ["PMNotification"];
					toEMail = ( string ) dt.Rows [0] ["EMail"];
				}

				if ( pmNotificationAllowed )
				{
					int userPMessageID;
					//string senderEmail;

					// get the PM ID
					using ( DataTable dt = YAF.Classes.Data.DB.pmessage_list( toUserID, PageBoardID, null ) )
						userPMessageID = ( int ) dt.Rows [0] ["UserPMessageID"];

					// get the sender e-mail -- DISABLED: too much information...
					//using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageBoardID, PageUserID, true ) )
					//	senderEmail = ( string ) dt.Rows [0] ["Email"];

					// send this user a PM notification e-mail
					StringDictionary emailParameters = new StringDictionary();

					emailParameters ["{fromuser}"] = PageUserName;
					emailParameters ["{link}"] = String.Format( "{1}{0}\r\n\r\n", Forum.GetLink( ForumPages.cp_message, "pm={0}", userPMessageID ), ServerURL );
					emailParameters ["{forumname}"] = BoardSettings.Name;
					emailParameters ["{subject}"] = subject;

					string message = Utils.CreateEmailFromTemplate( "pmnotification.txt", ref emailParameters );

					string emailSubject = string.Format(GetText("COMMON","PM_NOTIFICATION_SUBJECT" ),PageUserName,BoardSettings.Name,subject);

					//  Build a MailMessage
					Utils.SendMail( this, BoardSettings.ForumEmail, toEMail, emailSubject, message );
				}
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageUserID, this, x );
				AddLoadMessage( String.Format( GetText( "failed" ), x.Message ) );
			}
		}

		private void Cancel_Click( object sender, System.EventArgs e )
		{
			Forum.Redirect( ForumPages.cp_profile );
		}

		private void FindUsers_Click( object sender, System.EventArgs e )
		{
			if ( To.Text.Length < 2 ) return;

			using ( DataTable dt = YAF.Classes.Data.DB.user_find( PageBoardID, true, To.Text, null ) )
			{
				if ( dt.Rows.Count > 0 )
				{
					ToList.DataSource = dt;
					ToList.DataValueField = "UserID";
					ToList.DataTextField = "Name";
					ToList.SelectedIndex = 0;
					ToList.Visible = true;
					To.Visible = false;
					FindUsers.Visible = false;
				}
				DataBind();
			}
		}
		private void AllUsers_Click( object sender, System.EventArgs e )
		{
			ListItem li = new ListItem( "All Users", "0" );
			ToList.Items.Add( li );
			ToList.Visible = true;
			To.Text = "All Users";
			To.Visible = false;
			FindUsers.Visible = false;
			AllUsers.Visible = false;
		}
	}
}
