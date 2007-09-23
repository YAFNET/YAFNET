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
	/// Summary description for WebForm1.
	/// </summary>
	public partial class editaccessmask : YAF.Classes.Base.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin ) );
				PageLinks.AddLink( "Access Masks", "" );

				BindData();
				if ( Request.QueryString ["i"] != null )
				{
					using ( DataTable dt = YAF.Classes.Data.DB.accessmask_list( PageContext.PageBoardID, Request.QueryString ["i"] ) )
					{
						DataRow row = dt.Rows [0];
						Name.Text = ( string ) row ["Name"];
						ReadAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.ReadAccess );
						PostAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.PostAccess );
						ReplyAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.ReplyAccess );
						PriorityAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.PriorityAccess );
						PollAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.PollAccess );
						VoteAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.VoteAccess );
						ModeratorAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.ModeratorAccess );
						EditAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.EditAccess );
						DeleteAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.DeleteAccess );
						UploadAccess.Checked = General.BinaryAnd( row ["Flags"], AccessFlags.UploadAccess );
					}
				}
			}
		}

		private void BindData()
		{
			DataBind();
		}

		protected void Save_Click( object sender, System.EventArgs e )
		{
			// Forum
			object accessMaskID = null;
			if ( Request.QueryString ["i"] != null )
				accessMaskID = Request.QueryString ["i"];

			YAF.Classes.Data.DB.accessmask_save( accessMaskID,
				PageContext.PageBoardID,
				Name.Text,
				ReadAccess.Checked,
				PostAccess.Checked,
				ReplyAccess.Checked,
				PriorityAccess.Checked,
				PollAccess.Checked,
				VoteAccess.Checked,
				ModeratorAccess.Checked,
				EditAccess.Checked,
				DeleteAccess.Checked,
				UploadAccess.Checked );
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_accessmasks );
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_accessmasks );
		}
	}
}
