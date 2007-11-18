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

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for editgroup.
	/// </summary>
	public partial class editgroup : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Groups", "" );

				BindData();
				if ( Request.QueryString ["i"] != null )
				{
					NewGroupRow.Visible = false;
					using ( DataTable dt = YAF.Classes.Data.DB.group_list( PageContext.PageBoardID, Request.QueryString ["i"] ) )
					{
						DataRow row = dt.Rows [0];
						Name.Text = ( string ) row ["Name"];
						IsGuestX.Checked = General.BinaryAnd( row ["Flags"], GroupFlags.IsGuest );
						IsAdminX.Checked = General.BinaryAnd( row ["Flags"], GroupFlags.IsAdmin );
						IsStartX.Checked = General.BinaryAnd( row ["Flags"], GroupFlags.IsStart );
						IsModeratorX.Checked = General.BinaryAnd( row ["Flags"], GroupFlags.IsModerator );

						// only if this IsGuest can they edit this flag
						if ( IsGuestX.Checked ) IsGuestTR.Visible = true;
					}
				}
			}
		}

		private void BindData()
		{
			using ( DataTable dt = new DataTable( "Files" ) )
			{
				dt.Columns.Add( "FileID", typeof( long ) );
				dt.Columns.Add( "FileName", typeof( string ) );
				DataRow dr = dt.NewRow();
				dr ["FileID"] = 0;
				dr ["FileName"] = "Select Rank Image";
				dt.Rows.Add( dr );

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( Request.MapPath( String.Format( "{0}images/ranks", YafForumInfo.ForumRoot ) ) );
				System.IO.FileInfo [] files = dir.GetFiles( "*.*" );
				long nFileID = 1;
				foreach ( System.IO.FileInfo file in files )
				{
					string sExt = file.Extension.ToLower();
					if ( sExt != ".png" && sExt != ".gif" && sExt != ".jpg" )
						continue;

					dr = dt.NewRow();
					dr ["FileID"] = nFileID++;
					dr ["FileName"] = file.Name;
					dt.Rows.Add( dr );
				}
			}

			if ( Request.QueryString ["i"] != null )
				AccessList.DataSource = YAF.Classes.Data.DB.forumaccess_group( Request.QueryString ["i"] );

			DataBind();
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_groups );
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			// Group
			long GroupID = 0;
			if ( Request.QueryString ["i"] != null ) GroupID = long.Parse( Request.QueryString ["i"] );

			string roleName = Name.Text.Trim();
			string oldRoleName = string.Empty;

			if ( GroupID != 0 )
			{
				// get the current group name in the DB
				using ( DataTable dt = YAF.Classes.Data.DB.group_list( YafContext.Current.PageBoardID, GroupID ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						oldRoleName = row ["Name"].ToString();
					}
				}
			}

			GroupID = YAF.Classes.Data.DB.group_save( GroupID, PageContext.PageBoardID, roleName, IsAdminX.Checked, IsGuestX.Checked, IsStartX.Checked, IsModeratorX.Checked, AccessMaskID.SelectedValue );

			if ( !System.Web.Security.Roles.RoleExists( roleName ) )
			{
				if ( oldRoleName != string.Empty || IsGuestX.Checked )
				{
					// delete and re-create (if not guest role)
					System.Web.Security.Roles.DeleteRole( oldRoleName, false );
				}

				if ( !IsGuestX.Checked )
				{
					// simply create it
					System.Web.Security.Roles.CreateRole( roleName );
				}
			}

			// Access
			if ( Request.QueryString ["i"] != null )
			{
				for ( int i = 0; i < AccessList.Items.Count; i++ )
				{
					RepeaterItem item = AccessList.Items [i];
					int ForumID = int.Parse( ( ( Label ) item.FindControl( "ForumID" ) ).Text );
					YAF.Classes.Data.DB.forumaccess_save( ForumID, GroupID, ( ( DropDownList ) item.FindControl( "AccessmaskID" ) ).SelectedValue );
				}
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_groups );
			}

			// remove caching in case something got updated...
			YafCache.Current.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ForumModerators ) );
			
			// Done
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editgroup, "i={0}", GroupID );
		}

		protected void BindData_AccessMaskID( object sender, System.EventArgs e )
		{
			( ( DropDownList ) sender ).DataSource = YAF.Classes.Data.DB.accessmask_list( PageContext.PageBoardID, null );
			( ( DropDownList ) sender ).DataValueField = "AccessMaskID";
			( ( DropDownList ) sender ).DataTextField = "Name";
		}

		protected void SetDropDownIndex( object sender, System.EventArgs e )
		{
			DropDownList list = ( DropDownList ) sender;
			list.Items.FindByValue( list.Attributes ["value"] ).Selected = true;
		}
	}
}
