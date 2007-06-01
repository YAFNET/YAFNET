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
using YAF.Classes.Utils;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for pmessage.
	/// </summary>
	public partial class pmessage : YAF.Classes.Base.ForumPage
	{
		protected YAF.Editor.ForumEditor Editor;

		public pmessage()
			: base( "PMESSAGE" )
		{
		}

        protected void Page_Init(object sender, System.EventArgs e)
        {
            Editor = YAF.Editor.EditorHelper.CreateEditorFromType(PageContext.BoardSettings.ForumEditor);
            EditorLine.Controls.Add(Editor);
        }

		protected void Page_Load( object sender, System.EventArgs e )
		{
			Editor.BaseDir = yaf_ForumInfo.ForumRoot + "editors";
			Editor.StyleSheet = yaf_BuildLink.ThemeFile( "theme.css" );
            
			if (User==null)
			{
                //CanLogin obsolete... removed.
				//if(CanLogin)
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );
				//else
				//	YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.forum );
			}

			if (!IsPostBack)
			{

				BindData();
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				Save.Text = GetText( "Save" );
                Preview.Text = GetText("Preview");
				Cancel.Text = GetText( "Cancel" );
				FindUsers.Text = GetText( "FINDUSERS" );
				AllUsers.Text = GetText( "ALLUSERS" );
                Clear.Text = GetText("CLEAR");

				if ( PageContext.IsAdmin )
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
                    DataTable dt = YAF.Classes.Data.DB.pmessage_list(Request.QueryString["p"]);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        Subject.Text = (string)row["Subject"];

                        if (Subject.Text.Length < 4 || Subject.Text.Substring(0, 4) != "Re: ")
                            Subject.Text = "Re: " + Subject.Text;

                        ToUserID = (int)row["FromUserID"];
                    }
				}


				if ( Request.QueryString ["p"] != null )
				{
					DataTable dt = YAF.Classes.Data.DB.pmessage_list(Request.QueryString["p"]);
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];

                        // default is quote
                        bool bQuote = true;

                        if (Request.QueryString["q"] != null && Request.QueryString["q"] == "0")
                            bQuote = false;

                        if ((int)row["ToUserID"] != PageContext.PageUserID && (int)row["FromUserID"] != PageContext.PageUserID)
                            yaf_BuildLink.AccessDenied();

                        Subject.Text = (string)row["Subject"];

                        if (Subject.Text.Length < 4 || Subject.Text.Substring(0, 4) != "Re: ")
                            Subject.Text = "Re: " + Subject.Text;

                        ToUserID = (int)row["FromUserID"];

                        if (bQuote)
                        {
                            string body = row["Body"].ToString();
                            bool isHtml = body.IndexOf('<') >= 0;

                            if (PageContext.BoardSettings.RemoveNestedQuotes)
                            {
                                RegexOptions m_options = RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline;
                                Regex quote = new Regex(@"\[quote(\=.*)?\](.*?)\[/quote\]", m_options);
                                // remove quotes from old messages
                                body = quote.Replace(body, "");
                            }
                            body = String.Format("[QUOTE={0}]{1}[/QUOTE]", row["FromUser"], body);
                            Editor.Text = body;
                        }
                    }
				
				}

				if ( Request.QueryString ["u"] != null )
					ToUserID = int.Parse( Request.QueryString ["u"].ToString() );

				if ( ToUserID != 0 )
				{
					using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, ToUserID, true ) )
					{
						To.Text = ( string ) dt.Rows [0] ["Name"];
						To.Enabled = false;
                        FindUsers.Enabled = false; 
                        AllUsers.Enabled = false;
					}
				}
			}
		}

		private void BindData()
		{
		}

        protected void Save_Click(object sender, System.EventArgs e)
		{
			if ( To.Text.Length <= 0 )
			{
				PageContext.AddLoadMessage( GetText( "need_to" ) );
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
				YAF.Classes.Data.DB.pmessage_save( PageContext.PageUserID, 0, Subject.Text, body, tFlags.BitValue );
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
			}
			else
			{
				using ( DataTable dt = YAF.Classes.Data.DB.user_find( PageContext.PageBoardID, false, To.Text, null ) )
				{
					if ( dt.Rows.Count != 1 )
					{
						PageContext.AddLoadMessage( GetText( "NO_SUCH_USER" ) );
						return;
					}
					else if ( ( int ) dt.Rows [0] ["IsGuest"] > 0 )
					{
						PageContext.AddLoadMessage( GetText( "NOT_GUEST" ) );
						return;
					}

					if ( Subject.Text.Length <= 0 )
					{
						PageContext.AddLoadMessage( GetText( "need_subject" ) );
						return;
					}
					if ( Editor.Text.Length <= 0 )
					{
						PageContext.AddLoadMessage( GetText( "need_message" ) );
						return;
					}

					string body = Editor.Text;

					MessageFlags tFlags = new MessageFlags();
					tFlags.IsHTML = Editor.UsesHTML;
					tFlags.IsBBCode = Editor.UsesBBCode;

					YAF.Classes.Data.DB.pmessage_save( PageContext.PageUserID, dt.Rows [0] ["UserID"], Subject.Text, body, tFlags.BitValue );

					if ( PageContext.BoardSettings.AllowPMEmailNotification )
						SendPMNotification( Convert.ToInt32(dt.Rows [0] ["UserID"]), Subject.Text );

					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
				}
			}
		}

		private void SendPMNotification(int toUserID, string subject)
		{
			try
			{
				bool pmNotificationAllowed;
				string toEMail;

				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, toUserID, true ) )
				{
					pmNotificationAllowed = ( bool ) dt.Rows [0] ["PMNotification"];
					toEMail = ( string ) dt.Rows [0] ["EMail"];
				}

				if ( pmNotificationAllowed )
				{
					int userPMessageID;
					//string senderEmail;

					// get the PM ID
					using ( DataTable dt = YAF.Classes.Data.DB.pmessage_list( toUserID, PageContext.PageBoardID, null ) )
						userPMessageID = ( int ) dt.Rows [0] ["UserPMessageID"];

					// get the sender e-mail -- DISABLED: too much information...
					//using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
					//	senderEmail = ( string ) dt.Rows [0] ["Email"];

					// send this user a PM notification e-mail
					StringDictionary emailParameters = new StringDictionary();

					emailParameters ["{fromuser}"] = PageContext.PageUserName;
					emailParameters ["{link}"] = String.Format( "{1}{0}\r\n\r\n", YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.cp_message, "pm={0}", userPMessageID ), YAF.Classes.Utils.yaf_ForumInfo.ServerURL );
					emailParameters ["{forumname}"] = PageContext.BoardSettings.Name;
					emailParameters ["{subject}"] = subject;

					string message = General.CreateEmailFromTemplate( "pmnotification.txt", ref emailParameters );

					string emailSubject = string.Format(GetText("COMMON","PM_NOTIFICATION_SUBJECT" ),PageContext.PageUserName,PageContext.BoardSettings.Name,subject);

					//  Build a MailMessage
					General.SendMail( PageContext.BoardSettings.ForumEmail, toEMail, emailSubject, message );
				}
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				PageContext.AddLoadMessage( String.Format( GetText( "failed" ), x.Message ) );
			}
		}

        protected void Preview_Click(object sender, System.EventArgs e)
        {
            PreviewRow.Visible = true;

            MessageFlags tFlags = new MessageFlags();
            tFlags.IsHTML = Editor.UsesHTML;
            tFlags.IsBBCode = Editor.UsesBBCode;

            string body = FormatMsg.FormatMessage(Editor.Text, tFlags);

            using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, PageContext.PageUserID, true))
            {
                if (!dt.Rows[0].IsNull("Signature"))
                    body += "<br/><hr noshade/>" + FormatMsg.FormatMessage(dt.Rows[0]["Signature"].ToString(), new MessageFlags());
            }

            PreviewCell.InnerHtml = body;
        }

		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.cp_profile );
		}

        protected void FindUsers_Click(object sender, System.EventArgs e)
		{
            if (To.Text.Length < 2)
            {
                PageContext.AddLoadMessage(GetText("NEED_MORE_LETTERS"));
                return;
            }

			using ( DataTable dt = YAF.Classes.Data.DB.user_find( PageContext.PageBoardID, true, To.Text, null ) )
			{
				if ( dt.Rows.Count > 0 )
				{
					ToList.DataSource = dt;
					ToList.DataValueField = "UserID";
					ToList.DataTextField = "Name";
                    ToList.DataBind();
					//ToList.SelectedIndex = 0;
					ToList.Visible = true;
					To.Visible = false;
					FindUsers.Visible = false;
                    Clear.Visible = true;
				}
				DataBind();
			}
		}
		protected void AllUsers_Click(object sender, EventArgs e)
		{
			ListItem li = new ListItem( "All Users", "0" );
			ToList.Items.Add( li );
			ToList.Visible = true;
			To.Text = "All Users";
			To.Visible = false;
			FindUsers.Visible = false;
			AllUsers.Visible = false;
            Clear.Visible = true;
		}
        protected void Clear_Click(object sender, EventArgs e)
        {
            ToList.Items.Clear();
            ToList.Visible = false;
            To.Text = "";
            To.Visible = true;
            FindUsers.Visible = true;
            AllUsers.Visible = true;
            Clear.Visible = false;
        }
	}
}
