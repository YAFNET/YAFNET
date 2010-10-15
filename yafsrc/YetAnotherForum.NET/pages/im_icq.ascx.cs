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
using System.Web.Security;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;


namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class im_icq : YAF.Classes.Core.ForumPage
	{
		public int UserID
		{
			get
			{
				return ( int )Security.StringToLongOrRedirect( Request.QueryString ["u"] );
			}
		}

		public im_icq()
			: base( "IM_ICQ" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( User == null )
				YafBuildLink.AccessDenied();

			if ( !IsPostBack )
			{
				Send.Text = GetText( "SEND" );
				From.Text = PageContext.User.UserName;
				Email.Text = PageContext.User.Email;

				// get user data...
				MembershipUser user = UserMembershipHelper.GetMembershipUserById( UserID );

				if ( user == null )
				{
					YafBuildLink.AccessDenied(/*No such user exists*/);
				}

                string displayName = UserMembershipHelper.GetDisplayNameFromID(UserID);

				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
                PageLinks.AddLink(!string.IsNullOrEmpty(displayName) ? displayName : user.UserName, YafBuildLink.GetLink(ForumPages.profile, "u={0}", UserID));
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				// get full user data...
				CombinedUserDataHelper userData = new CombinedUserDataHelper( user, UserID );

				ViewState ["to"] = userData.Profile.ICQ;
				Status.Src = "http://web.icq.com/whitepages/online?icq={0}&img=5".FormatWith(userData.Profile.ICQ);
			}
		}

		protected void Send_Click( object sender, EventArgs e )
		{
			string html = "http://wwp.icq.com/scripts/WWPMsg.dll?from={0}&fromemail={1}&subject={2}&to={3}&body={4}".FormatWith(this.Server.UrlEncode( this.From.Text ), this.Server.UrlEncode( this.Email.Text ), this.Server.UrlEncode( "From WebPager Panel" ), this.ViewState ["to"], this.Server.UrlEncode( this.Body.Text ));
			Response.Redirect( html );
		}
	}
}
