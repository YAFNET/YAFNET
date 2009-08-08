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
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class editaccessmask : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load( object sender, System.EventArgs e )
		{
			if ( !IsPostBack )
			{
				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Access Masks", "" );

				BindData();
				if ( Request.QueryString ["i"] != null )
				{
					using (DataTable dt = YAF.Classes.Data.DB.accessmask_list(PageContext.PageBoardID, Request.QueryString["i"]))
					{
						DataRow row = dt.Rows[0];
						AccessFlags flags = new AccessFlags(row["Flags"]);
						Name.Text = (string)row["Name"];
						ReadAccess.Checked = flags.ReadAccess;
						PostAccess.Checked = flags.PostAccess;
						ReplyAccess.Checked = flags.ReplyAccess;
						PriorityAccess.Checked = flags.PriorityAccess;
						PollAccess.Checked = flags.PollAccess;
						VoteAccess.Checked = flags.VoteAccess;
						ModeratorAccess.Checked = flags.ModeratorAccess;
						EditAccess.Checked = flags.EditAccess;
						DeleteAccess.Checked = flags.DeleteAccess;
						UploadAccess.Checked = flags.UploadAccess;
						DownloadAccess.Checked = flags.DownloadAccess;
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
				UploadAccess.Checked,
				DownloadAccess.Checked
				);

			YafCache.Current.Remove( YafCache.GetBoardCacheKey( Constants.Cache.ForumModerators ) );

			YafBuildLink.Redirect( ForumPages.admin_accessmasks );
		}

		protected void Cancel_Click( object sender, System.EventArgs e )
		{
			YafBuildLink.Redirect( ForumPages.admin_accessmasks );
		}
	}
}
