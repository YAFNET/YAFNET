/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for emailtopic.
	/// </summary>
	public partial class emailtopic : YAF.Classes.Core.ForumPage
	{

		public emailtopic()
			: base( "EMAILTOPIC" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( Request.QueryString ["t"] == null || !PageContext.ForumReadAccess || !PageContext.BoardSettings.AllowEmailTopic )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				if ( PageContext.Settings.LockedForum == 0 )
				{
					PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
					PageLinks.AddLink( PageContext.PageCategoryName, YafBuildLink.GetLink( ForumPages.forum, "c={0}", PageContext.PageCategoryID ) );
				}

				PageLinks.AddForumLinks( PageContext.PageForumID );
				PageLinks.AddLink( PageContext.PageTopicName, YafBuildLink.GetLink( ForumPages.posts, "t={0}", PageContext.PageTopicID ) );

				SendEmail.Text = GetText( "send" );

				Subject.Text = PageContext.PageTopicName;

				YafTemplateEmail emailTopic = new YafTemplateEmail();

				emailTopic.TemplateParams ["{link}"] = YafBuildLink.GetLinkNotEscaped( ForumPages.posts, true, "t={0}", PageContext.PageTopicID );
				emailTopic.TemplateParams ["{user}"] = PageContext.PageUserName;

				Message.Text = emailTopic.ProcessTemplate( "EMAILTOPIC" );
			}
		}

		protected void SendEmail_Click( object sender, System.EventArgs e )
		{
			if ( EmailAddress.Text.Length == 0 )
			{
				PageContext.AddLoadMessage( GetText( "need_email" ) );
				return;
			}

			try
			{
				string senderEmail = null;

				using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageContext.PageBoardID, PageContext.PageUserID, true ) )
				{
					senderEmail = ( string )dt.Rows [0] ["Email"];
				}

				// send the email...
				YafServices.SendMail.Send( senderEmail, EmailAddress.Text.Trim(), Subject.Text.Trim(), Message.Text.Trim() );

				YafBuildLink.Redirect( ForumPages.posts, "t={0}", PageContext.PageTopicID );
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				PageContext.AddLoadMessage( GetTextFormatted( "failed", x.Message ) );
			}
		}
	}
}
