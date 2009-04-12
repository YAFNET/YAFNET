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
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class im_icq : YAF.Classes.Base.ForumPage
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
				MembershipUser user = UserMembershipHelper.GetMembershipUser( UserID );

				if ( user == null )
				{
					YafBuildLink.AccessDenied(/*No such user exists*/);
				}

				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( user.UserName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", UserID ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				// get full user data...
				YafCombinedUserData userData = new YafCombinedUserData( user, UserID );

				ViewState ["to"] = userData.Profile.ICQ;
				Status.Src = string.Format( "http://web.icq.com/whitepages/online?icq={0}&img=5", userData.Profile.ICQ );
			}
		}

		protected void Send_Click( object sender, EventArgs e )
		{
			string html = string.Format( "http://wwp.icq.com/scripts/WWPMsg.dll?from={0}&fromemail={1}&subject={2}&to={3}&body={4}",
				Server.UrlEncode( From.Text ),
				Server.UrlEncode( Email.Text ),
				Server.UrlEncode( "From WebPager Panel" ),
				ViewState ["to"],
				Server.UrlEncode( Body.Text )
				);
			Response.Redirect( html );
		}
	}
}
