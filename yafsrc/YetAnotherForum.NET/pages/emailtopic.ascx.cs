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
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for emailtopic.
	/// </summary>
	public partial class emailtopic : YAF.Classes.Base.ForumPage
	{

		public emailtopic()
			: base( "EMAILTOPIC" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( Request.QueryString ["t"] == null || !PageContext.ForumReadAccess || !PageContext.BoardSettings.AllowEmailTopic )
				yaf_BuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
					PageLinks.AddLink( PageContext.PageCategoryName, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				}
				PageLinks.AddForumLinks( PageContext.PageForumID );
				PageLinks.AddLink( PageContext.PageTopicName, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", PageContext.PageTopicID ) );

				SendEmail.Text = GetText( "send" );

				Subject.Text = PageContext.PageTopicName;
				string msg = General.ReadTemplate( "emailtopic.txt" );
				msg = msg.Replace( "{link}", String.Format( "{0}{1}", YAF.Classes.Utils.yaf_ForumInfo.ServerURL, YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.posts, "t={0}", PageContext.PageTopicID ) ) );
				msg = msg.Replace( "{user}", PageContext.PageUserName );
				Message.Text = msg;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
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

		protected void SendEmail_Click( object sender, System.EventArgs e )
		{
			if ( EmailAddress.Text.Length == 0 )
			{
				PageContext.AddLoadMessage( GetText( "need_email" ) );
				return;
			}

			try
			{
				string senderemail;
				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
					senderemail = ( string ) dt.Rows [0] ["Email"];

				//  Build a MailMessage
				General.SendMail( senderemail, EmailAddress.Text, Subject.Text, Message.Text );
				YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.posts, "t={0}", PageContext.PageTopicID );
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				PageContext.AddLoadMessage( String.Format( GetText( "failed" ), x.Message ) );
			}
		}
	}
}
