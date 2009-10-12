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
	/// Summary description for editgroup.
	/// </summary>
	public partial class editrank : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Ranks", "" );

				BindData();
				if ( Request.QueryString ["r"] != null )
				{
					using ( DataTable dt = YAF.Classes.Data.DB.rank_list( PageContext.PageBoardID, Request.QueryString ["r"] ) )
					{
						DataRow row = dt.Rows [0];
						RankFlags flags = new RankFlags( row ["Flags"] );
						Name.Text = ( string ) row ["Name"];
						IsStart.Checked = flags.IsStart;
						IsLadder.Checked = flags.IsLadder;
						MinPosts.Text = row ["MinPosts"].ToString();
                        PMLimit.Text = row["PMLimit"].ToString();
                        Style.Text = row["Style"].ToString();
                        RankPriority.Text = row["SortOrder"].ToString();
                       
						ListItem item = RankImage.Items.FindByText( row ["RankImage"].ToString() );
						if ( item != null )
						{
							item.Selected = true;
							Preview.Src = String.Format( "{0}images/ranks/{1}", YafForumInfo.ForumRoot, row ["RankImage"] ); //path corrected
						}
						else
						{
							Preview.Src = String.Format( "{0}images/spacer.gif", YafForumInfo.ForumRoot );
						}
					}
				}
				else
				{
					Preview.Src = String.Format( "{0}images/spacer.gif", YafForumInfo.ForumRoot );
				}
               
			}
			RankImage.Attributes ["onchange"] = String.Format(
					"getElementById('{1}_ctl01_Preview').src='{0}images/ranks/' + this.value",
					YafForumInfo.ForumRoot,
					this.Parent.ID
					);
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

		private void BindData()
		{
			using ( DataTable dt = new DataTable( "Files" ) )
			{
				dt.Columns.Add( "FileID", typeof( long ) );
				dt.Columns.Add( "FileName", typeof( string ) );
				dt.Columns.Add( "Description", typeof( string ) );
				DataRow dr = dt.NewRow();
				dr ["FileID"] = 0;
				dr ["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
				dr ["Description"] = "Select Rank Image";
				dt.Rows.Add( dr );

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( Request.MapPath( String.Format( "{0}images/ranks", YafForumInfo.ForumFileRoot ) ) );
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

				RankImage.DataSource = dt;
				RankImage.DataValueField = "FileName";
				RankImage.DataTextField = "Description";
			}
			DataBind();
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_ranks );
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
            if (!ValidationHelper.IsValidInt(PMLimit.Text.Trim()))
            {
                PageContext.AddLoadMessage("You should enter integer value for pmessage number.");
                return;
            }
            if (!ValidationHelper.IsValidInt(RankPriority.Text.Trim()))
            {
                PageContext.AddLoadMessage("Rank Priority should be small integer.");
                return;
            }

			// Group
			int RankID = 0;
			if ( Request.QueryString ["r"] != null ) RankID = int.Parse( Request.QueryString ["r"] );

			object rankImage = null;
			if ( RankImage.SelectedIndex > 0 )
				rankImage = RankImage.SelectedValue;
            YAF.Classes.Data.DB.rank_save(RankID, PageContext.PageBoardID, Name.Text, IsStart.Checked, IsLadder.Checked, MinPosts.Text, rankImage, Convert.ToInt32(PMLimit.Text), Style.Text.Trim(), RankPriority.Text.Trim());

			YafBuildLink.Redirect( ForumPages.admin_ranks );
		}
	}
}
