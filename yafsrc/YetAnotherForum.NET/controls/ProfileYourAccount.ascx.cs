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
using YAF.Classes.Core;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	public partial class ProfileYourAccount : YAF.Classes.Core.BaseUserControl
	{
		protected void Page_Load( object sender, EventArgs e )
		{
			if ( !IsPostBack )
			{
				BindData();
			}
		}

		private void BindData()
		{
			Groups.DataSource = YAF.Classes.Data.DB.usergroup_list( PageContext.PageUserID );

			// Bind			
			DataBind();

			

			//TitleUserName.Text = HtmlEncode( userData.Membership.UserName );
			AccountEmail.Text = PageContext.CurrentUserData.Membership.Email;
			Name.Text = HtmlEncode( PageContext.CurrentUserData.Membership.UserName );
			Joined.Text = YafServices.DateTime.FormatDateTime( PageContext.CurrentUserData.Joined );
			NumPosts.Text = String.Format( "{0:N0}", PageContext.CurrentUserData.NumPosts );

			string avatarImg = YafServices.Avatar.GetAvatarUrlForCurrentUser();
      
			if ( !String.IsNullOrEmpty(avatarImg))
			{
				AvatarImage.ImageUrl = avatarImg;
			}
			else
			{
				AvatarImage.Visible = false;
			}
		}
	}
}
