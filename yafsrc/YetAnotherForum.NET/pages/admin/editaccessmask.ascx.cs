/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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

namespace YAF.Pages.Admin
{
	using System;
	using System.Data;

	using YAF.Classes;
	using YAF.Classes.Core;
	using YAF.Classes.Data;
	using YAF.Classes.Utils;

	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class editaccessmask : AdminPage
	{
		/* Construction */
		#region Overridden Methods

		/// <summary>
		/// Creates navigation page links on top of forum (breadcrumbs).
		/// </summary>
		protected override void CreatePageLinks()
		{
			// beard index
			this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			// administration index
			this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
			// current page label (no link)
			this.PageLinks.AddLink("Access Masks", string.Empty);
		}

		#endregion

		
		/* Event Handlers */

		#region Page Events

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// create page links
				CreatePageLinks();

				// bind data
				BindData();
			}
		}

		#endregion

		#region Control Events

		/// <summary>
		/// The save_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Save_Click(object sender, EventArgs e)
		{
			// retrieve access mask ID from parameter (if applicable)
			object accessMaskID = null;
			if (Request.QueryString.GetFirstOrDefault("i") != null) accessMaskID = Request.QueryString.GetFirstOrDefault("i");

            if (this.Name.Text.Trim().Length <= 0)
            {
                PageContext.AddLoadMessage("You must enter a name for the Access Mask.");
                return;
            }

            short sortOrder = 0;

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                PageContext.AddLoadMessage("The Sort Order value should be a positive integer from 0 to 32767.");
                return;
            }            

            if (!short.TryParse(this.SortOrder.Text.Trim(), out sortOrder))
            {
                PageContext.AddLoadMessage("You must enter a number value from 0 to 32767 for sort order.");
                return;
            }

			// save it
			DB.accessmask_save(
			  accessMaskID,
			  PageContext.PageBoardID,
			  this.Name.Text,
			  this.ReadAccess.Checked,
			  this.PostAccess.Checked,
			  this.ReplyAccess.Checked,
			  this.PriorityAccess.Checked,
			  this.PollAccess.Checked,
			  this.VoteAccess.Checked,
			  this.ModeratorAccess.Checked,
			  this.EditAccess.Checked,
			  this.DeleteAccess.Checked,
			  this.UploadAccess.Checked,
			  this.DownloadAccess.Checked,
              sortOrder);

			// clear cache
			PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));

			// get back to access masks administration
			YafBuildLink.Redirect(ForumPages.admin_accessmasks);
		}


		/// <summary>
		/// The cancel_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Cancel_Click(object sender, EventArgs e)
		{
			// get back to access masks administration
			YafBuildLink.Redirect(ForumPages.admin_accessmasks);
		}

		#endregion


		/* Methods */
		#region Data Binding

		/// <summary>
		/// The bind data.
		/// </summary>
		private void BindData()
		{
			if (Request.QueryString.GetFirstOrDefault("i") != null)
			{
				// load access mask
				using (DataTable dt = DB.accessmask_list(PageContext.PageBoardID, Request.QueryString.GetFirstOrDefault("i")))
				{
					// we need just one
					DataRow row = dt.Rows[0];

					// get access mask properties
					this.Name.Text = (string)row["Name"];
					this.SortOrder.Text = row["SortOrder"].ToString();

					// get flags
					var flags = new AccessFlags(row["Flags"]);
					this.ReadAccess.Checked = flags.ReadAccess;
					this.PostAccess.Checked = flags.PostAccess;
					this.ReplyAccess.Checked = flags.ReplyAccess;
					this.PriorityAccess.Checked = flags.PriorityAccess;
					this.PollAccess.Checked = flags.PollAccess;
					this.VoteAccess.Checked = flags.VoteAccess;
					this.ModeratorAccess.Checked = flags.ModeratorAccess;
					this.EditAccess.Checked = flags.EditAccess;
					this.DeleteAccess.Checked = flags.DeleteAccess;
					this.UploadAccess.Checked = flags.UploadAccess;
					this.DownloadAccess.Checked = flags.DownloadAccess;
				}
			}

			DataBind();
		}
		
		#endregion
	}
}