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
using System.Web.Security;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class im_email : YAF.Classes.Core.ForumPage
	{

		public int UserID
		{
			get
			{
				return ( int )Security.StringToLongOrRedirect( Request.QueryString ["u"] );
			}
		}

		public im_email()
			: base( "IM_EMAIL" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( User == null || !PageContext.BoardSettings.AllowEmailSending )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				// get user data...
				MembershipUser user = UserMembershipHelper.GetMembershipUser( UserID );

				if ( user == null )
				{
					YafBuildLink.AccessDenied(/*No such user exists*/);
				}

				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( user.UserName, YafBuildLink.GetLink( ForumPages.profile, "u={0}", UserID ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				Send.Text = GetText( "SEND" );
			}
		}

		protected void Send_Click( object sender, EventArgs e )
		{
			try
			{
				// get "to" user...
				MembershipUser toUser = UserMembershipHelper.GetMembershipUser( UserID );

				// send it...
				YafServices.SendMail.Send( new System.Net.Mail.MailAddress( PageContext.User.Email, PageContext.User.UserName ), new System.Net.Mail.MailAddress( toUser.Email.Trim(), toUser.UserName.Trim() ), Subject.Text.Trim(), Body.Text.Trim() );

				// redirect to profile page...
				YafBuildLink.Redirect( ForumPages.profile, "u={0}", UserID );
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
				if ( PageContext.IsAdmin )
				{
					PageContext.AddLoadMessage( x.Message );
				}
				else
				{
					PageContext.AddLoadMessage( GetText( "ERROR" ) );
				}
			}
		}
	}
}
