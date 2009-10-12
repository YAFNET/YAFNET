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
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for editcategory.
	/// </summary>
	public partial class editcategory : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Forums", YafBuildLink.GetLink( ForumPages.admin_forums ) );
				PageLinks.AddLink( "Category" );

				// Populate Category Table
				CreateImagesDataTable();

				CategoryImages.Attributes ["onchange"] = String.Format(
					"getElementById('{1}').src='{0}images/categories/' + this.value",
					YafForumInfo.ForumRoot,
					Preview.ClientID
					);

				Name.Style.Add( "width", "100%" );

				BindData();
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

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_forums );
		}

		private void BindData()
		{
			Preview.Src = String.Format( "{0}images/spacer.gif", YafForumInfo.ForumRoot );

			if ( Request.QueryString ["c"] != null )
			{
				using ( DataTable dt = YAF.Classes.Data.DB.category_list( PageContext.PageBoardID, Request.QueryString ["c"] ) )
				{
					DataRow row = dt.Rows [0];
					Name.Text = ( string ) row ["Name"];
					SortOrder.Text = row ["SortOrder"].ToString();
					CategoryNameTitle.Text = Name.Text;

					ListItem item = CategoryImages.Items.FindByText( row ["CategoryImage"].ToString() );
					if ( item != null )
					{
						item.Selected = true;
						Preview.Src = String.Format( "{0}images/categories/{1}", YafForumInfo.ForumRoot, row ["CategoryImage"] ); //path corrected
					}
				}
			}
		}

		protected void CreateImagesDataTable()
		{
			using ( DataTable dt = new DataTable( "Files" ) )
			{
				dt.Columns.Add( "FileID", typeof( long ) );
				dt.Columns.Add( "FileName", typeof( string ) );
				dt.Columns.Add( "Description", typeof( string ) );
				DataRow dr = dt.NewRow();
				dr ["FileID"] = 0;
				dr ["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
				dr ["Description"] = "None";
				dt.Rows.Add( dr );

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( Request.MapPath( String.Format( "{0}images/categories", YafForumInfo.ForumFileRoot ) ) );
				if ( dir.Exists )
				{
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
						dr ["Description"] = file.Name;
						dt.Rows.Add( dr );
					}
				}

				CategoryImages.DataSource = dt;
				CategoryImages.DataValueField = "FileName";
				CategoryImages.DataTextField = "Description";
				CategoryImages.DataBind();
			}
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			int CategoryID = 0;
			if ( Request.QueryString ["c"] != null ) CategoryID = int.Parse( Request.QueryString ["c"] );

			int sortOrder;
			string name = Name.Text.Trim();
			object categoryImage = null;

			if ( CategoryImages.SelectedIndex > 0 )
			{
				categoryImage = CategoryImages.SelectedValue;
			}
			if ( !int.TryParse( SortOrder.Text.Trim(), out sortOrder ) )
			{
				// error...
				PageContext.AddLoadMessage( "Invalid value entered for sort order: must enter a number." );
				return;
			}
			if ( string.IsNullOrEmpty( name ) )
			{
				// error...
				PageContext.AddLoadMessage( "Must enter a value for the category name field." );
				return;
			}

			// save category
			DB.category_save( PageContext.PageBoardID, CategoryID, name, categoryImage, sortOrder );
			// remove category cache...
			PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ForumCategory ) );
			// redirect
			YafBuildLink.Redirect( ForumPages.admin_forums );
		}
	}
}
