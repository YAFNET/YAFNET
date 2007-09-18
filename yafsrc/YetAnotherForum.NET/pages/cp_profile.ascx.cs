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
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for editprofile.
	/// </summary>
	public partial class cp_profile : YAF.Classes.Base.ForumPage
	{

		public cp_profile()
			: base( "CP_PROFILE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if(User==null)
			{
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );
			}

			if ( !IsPostBack )
			{
				BindData();

				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( HtmlEncode( PageContext.PageUserName ), "" );
			}
		}

		private void BindData()
		{
			Groups.DataSource = YAF.Classes.Data.DB.usergroup_list( PageContext.PageUserID );

			// Bind			
			DataBind();

			YafCombinedUserData userData = new YafCombinedUserData( PageContext.PageUserID );

			TitleUserName.Text = HtmlEncode( userData.Membership.UserName );
			AccountEmail.Text = userData.Membership.Email;
			Name.Text = HtmlEncode( userData.Membership.UserName );
			Joined.Text = YafDateTime.FormatDateTime( userData.Joined );
			NumPosts.Text = String.Format( "{0:N0}", userData.NumPosts );

			if ( PageContext.BoardSettings.AvatarUpload && userData.HasAvatarImage )
			{
				AvatarImage.Src = String.Format( "{0}resource.ashx?u={1}", YafForumInfo.ForumRoot, PageContext.PageUserID );
			}
			else if ( userData.Avatar != null && userData.Avatar.Length > 0 ) // Took out PageContext.BoardSettings.AvatarRemote
			{
				AvatarImage.Src = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
					Server.UrlEncode( userData.Avatar ),
					PageContext.BoardSettings.AvatarWidth,
					PageContext.BoardSettings.AvatarHeight,
					YafForumInfo.ForumRoot );
			}
			else
			{
				AvatarImage.Visible = false;
			}
		}
	}
}
