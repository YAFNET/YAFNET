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
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;


namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for smilies_import.
	/// </summary>
	public partial class smilies_import : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Smilies Import", "" );

				BindData();
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
				dr ["FileName"] = "Select File (*.pak)";
				dt.Rows.Add( dr );

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( Request.MapPath( String.Format( "{0}images/emoticons", YafForumInfo.ForumFileRoot ) ) );
				System.IO.FileInfo [] files = dir.GetFiles( "*.pak" );
				long nFileID = 1;
				foreach ( System.IO.FileInfo file in files )
				{
					dr = dt.NewRow();
					dr ["FileID"] = nFileID++;
					dr ["FileName"] = file.Name;
					dt.Rows.Add( dr );
				}

				File.DataSource = dt;
				File.DataValueField = "FileID";
				File.DataTextField = "FileName";
			}
			DataBind();
		}

		private void import_Click( object sender, System.EventArgs e )
		{
			if ( long.Parse( File.SelectedValue ) < 1 )
			{
				PageContext.AddLoadMessage( "You must select a .pak file to import." );
				return;
			}

			string fileName = Request.MapPath( String.Format( "{0}images/emoticons/{1}", YafForumInfo.ForumRoot, File.SelectedItem.Text ) );
			string split = System.Text.RegularExpressions.Regex.Escape( "=+:" );

			using ( System.IO.StreamReader file = new System.IO.StreamReader( fileName ) )
			{
                int sortOrder = 1;
                
                // Delete existing smilies?
                if (DeleteExisting.Checked)
                {
                    YAF.Classes.Data.DB.smiley_delete(null);
                }
                else
                {
                    // Get max value of SortOrder
                    using (DataView dv = YAF.Classes.Data.DB.smiley_listunique(PageContext.PageBoardID).DefaultView)
                    {
                        dv.Sort = "SortOrder desc";
                        if (dv.Count > 0)
                        {
                            DataRowView dr = dv[0];
                            if (dr != null)
                            {
                                object o = dr["SortOrder"];
                                if (int.TryParse(o.ToString(), out sortOrder))
                                    sortOrder++;
                            }
                        }
                    }
                }
                                
				do
				{
					string line = file.ReadLine();
					if ( line == null )
						break;

					string [] lineSplit = System.Text.RegularExpressions.Regex.Split( line, split, System.Text.RegularExpressions.RegexOptions.None );

                    if (lineSplit.Length == 3)
                    {
                        YAF.Classes.Data.DB.smiley_save(null, PageContext.PageBoardID, lineSplit[2], lineSplit[0], lineSplit[1], sortOrder, 0);
                        sortOrder++;
                    }

				} while ( true );

				file.Close();

				// invalidate the cache...
				PageContext.Cache.Remove( YafCache.GetBoardCacheKey( Constants.Cache.Smilies ) );
				YAF.Classes.UI.ReplaceRulesCreator.ClearCache();
			}

			YafBuildLink.Redirect( ForumPages.admin_smilies );
		}

		private void cancel_Click( object sender, System.EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_smilies );
		}


		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			import.Click += new System.EventHandler( import_Click );
			cancel.Click += new System.EventHandler( cancel_Click );
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
