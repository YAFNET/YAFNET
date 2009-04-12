/* Yet Another Forum.NET
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
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages // YAF.Pages
{
	public partial class im_skype : YAF.Classes.Base.ForumPage
	{
		public int UserID
		{
			get
			{
				return ( int )Security.StringToLongOrRedirect( Request.QueryString ["u"] );
			}
		}

		public im_skype()
			: base( "IM_SKYPE" )
		{
		}

		protected void Page_Load( object sender, EventArgs e )
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

				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( user.UserName, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", UserID ) );
				PageLinks.AddLink( GetText( "TITLE" ), "" );

				// get full user data...
				YafCombinedUserData userData = new YafCombinedUserData( user, UserID );

				Msg.NavigateUrl = string.Format( "skype:{0}?call", userData.Profile.Skype );
				Msg.Attributes.Add( "onclick", "return skypeCheck();" );
				Img.Src = string.Format( "http://mystatus.skype.com/bigclassic/{0}", userData.Profile.Skype );
			}
		}
	}
}