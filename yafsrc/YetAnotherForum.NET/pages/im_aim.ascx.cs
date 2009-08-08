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

using System.Web.Security;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for active.
	/// </summary>
	public partial class im_aim : YAF.Classes.Core.ForumPage
	{
		public int UserID
		{
			get
			{
				return ( int )Security.StringToLongOrRedirect( Request.QueryString ["u"] );
			}
		}

		public im_aim()
			: base( "IM_AIM" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( User == null )
			{
				YafBuildLink.AccessDenied();
			}

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

				// get full user data...
				CombinedUserDataHelper userData = new CombinedUserDataHelper( user, UserID );

				Msg.NavigateUrl = string.Format( "aim:goim?screenname={0}&message=Hi.+Are+you+there?", userData.Profile.AIM );
				Buddy.NavigateUrl = string.Format( "aim:addbuddy?screenname={0}", userData.Profile.AIM );
			}
		}
	}
}
