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

namespace YAF.Pages
{
	/// <summary>
	/// Summary description for editprofile.
	/// </summary>
	public partial class cp_profile : ForumPage
	{

		public cp_profile()
			: base( "CP_PROFILE" )
		{
		}

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if(User==null)
			{
				if(CanLogin)
					Forum.Redirect( ForumPages.login, "ReturnUrl={0}", Utils.GetSafeRawUrl() );
				else
					Forum.Redirect( ForumPages.forum );
			}

			if ( !IsPostBack )
			{
				BindData();

				PageLinks.AddLink( BoardSettings.Name, Forum.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( PageUserName, "" );
			}
		}

		private void BindData()
		{
			DataRow row;

			Groups.DataSource = YAF.Classes.Data.DB.usergroup_list( PageUserID );

			// Bind			
			DataBind();
			using ( DataTable dt = YAF.Classes.Data.DB.user_list( PageBoardID, PageUserID, true ) )
			{
				row = dt.Rows [0];
			}

			TitleUserName.Text = Server.HtmlEncode( ( string ) row ["Name"] );
			AccountEmail.Text = row ["Email"].ToString();
			Name.Text = Server.HtmlEncode( ( string ) row ["Name"] );
			Joined.Text = FormatDateTime( ( DateTime ) row ["Joined"] );
			NumPosts.Text = String.Format( "{0:N0}", row ["NumPosts"] );

			if ( BoardSettings.AvatarUpload && row ["HasAvatarImage"] != null && long.Parse( row ["HasAvatarImage"].ToString() ) > 0 )
			{
				AvatarImage.Src = String.Format( "{0}resource.ashx?u={1}", Data.ForumRoot, PageUserID );
			}
			else if ( row ["Avatar"].ToString().Length > 0 ) // Took out BoardSettings.AvatarRemote
			{
				AvatarImage.Src = String.Format( "{3}resource.ashx?url={0}&width={1}&height={2}",
					Server.UrlEncode( row ["Avatar"].ToString() ),
					BoardSettings.AvatarWidth,
					BoardSettings.AvatarHeight,
					Data.ForumRoot );
			}
			else
			{
				AvatarImage.Visible = false;
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
	}
}
