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
	using System.Web.UI.WebControls;
	using YAF.Classes;
	using YAF.Classes.Core;
	using YAF.Classes.Data;
	using YAF.Classes.Utils;
	using YAF.Utilities;

	/// <summary>
	/// Summary description for members.
	/// </summary>
	public partial class users : AdminPage
	{

		/* Construction */
		#region Overridden Methods

		/// <summary>
		/// Creates navigation page links on top of forum (breadcrumbs).
		/// </summary>
		protected override void CreatePageLinks()
		{
			// link to board index
			this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
			// link to administration index
			this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
			// current page label (no link)
			this.PageLinks.AddLink("Users", string.Empty);
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
			PageContext.PageElements.RegisterJQuery();
			PageContext.PageElements.RegisterJsResourceInclude("blockUIJs", "js/jquery.blockUI.js");

			if (!IsPostBack)
			{
				// create page links
				CreatePageLinks();

				// intialize since filter items
				InitSinceDropdown();
				// set since filter to last item "All time"
				Since.SelectedIndex = Since.Items.Count - 1;

				this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loading-white.gif");

				// get list of user groups for filtering
				using (DataTable dt = DB.group_list(PageContext.PageBoardID, null))
				{
					// add empty item for no filtering
					DataRow newRow = dt.NewRow();
					newRow["Name"] = string.Empty;
					newRow["GroupID"] = DBNull.Value;
					dt.Rows.InsertAt(newRow, 0);

					this.group.DataSource = dt;
					this.group.DataTextField = "Name";
					this.group.DataValueField = "GroupID";
					this.group.DataBind();
				}

				// get list of user ranks for filtering
				using (DataTable dt = DB.rank_list(PageContext.PageBoardID, null))
				{
					// add empty for for no filtering
					DataRow newRow = dt.NewRow();
					newRow["Name"] = string.Empty;
					newRow["RankID"] = DBNull.Value;
					dt.Rows.InsertAt(newRow, 0);

					this.rank.DataSource = dt;
					this.rank.DataTextField = "Name";
					this.rank.DataValueField = "RankID";
					this.rank.DataBind();
				}

				/// TODO : page size difinable?
				this.PagerTop.PageSize = 25;
			}
		}

		#endregion

		#region Controls Events

		/// <summary>
		/// The user list_ item command.
		/// </summary>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		public void UserList_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "edit":
					// we are going to edit user - redirect to edit page
					YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
					break;
				case "delete":
					// we are deleting user

					if (PageContext.PageUserID == int.Parse(e.CommandArgument.ToString()))
					{
						// deleting yourself isn't an option
						PageContext.AddLoadMessage("You can't delete yourself.");
						return;
					}

					// get user(s) we are about to delete
					using (DataTable dt = DB.user_list(PageContext.PageBoardID, e.CommandArgument, DBNull.Value))
					{
						// examine each if he's possible to delete
						foreach (DataRow row in dt.Rows)
						{
							if (SqlDataLayerConverter.VerifyInt32(row["IsGuest"]) > 0)
							{
								// we cannot detele guest
								PageContext.AddLoadMessage("You can't delete the Guest.");
								return;
							}

							if ((row["IsAdmin"] != DBNull.Value && SqlDataLayerConverter.VerifyInt32(row["IsAdmin"]) > 0) ||
								 (row["IsHostAdmin"] != DBNull.Value && Convert.ToInt32(row["IsHostAdmin"]) > 0))
							{
								// admin are not deletable either
								PageContext.AddLoadMessage("You can't delete the Admin.");
								return;
							}
						}
					}

					// all is good, user can be deleted
					UserMembershipHelper.DeleteUser(Convert.ToInt32(e.CommandArgument));
					// rebind data
					BindData();

					// quit case
					break;
			}
		}


		/// <summary>
		/// The since_ selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Since_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Set the controls' pager index to 0.
			PagerTop.CurrentPageIndex = 0;

			// re-bind data
			BindData();
		}


		/// <summary>
		/// The delete_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		public void Delete_Load(object sender, EventArgs e)
		{
			// add confirmation method on click
			ControlHelper.AddOnClickConfirmDialog(sender, "Delete this user?");
		}


		/// <summary>
		/// The new user_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		public void NewUser_Click(object sender, EventArgs e)
		{
			// redirect to create new user page
			YafBuildLink.Redirect(ForumPages.admin_reguser);
		}


		/// <summary>
		/// The search_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		public void search_Click(object sender, EventArgs e)
		{
			// re-bind data
			BindData();
		}


		/// <summary>
		/// The pager top_ page change.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void PagerTop_PageChange(object sender, EventArgs e)
		{
			// rebind
			BindData();
		}


		/// <summary>
		/// The sync users_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void SyncUsers_Click(object sender, EventArgs e)
		{
			// start...
			SyncMembershipUsersTask.Start(PageContext.PageBoardID);

			// enable timer...
			this.UpdateStatusTimer.Enabled = true;

			// show blocking ui...
			PageContext.PageElements.RegisterJsBlockStartup("BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("SyncUsersMessage"));
		}


		/// <summary>
		/// The update status timer_ tick.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void UpdateStatusTimer_Tick(object sender, EventArgs e)
		{
			// see if the migration is done....
			if (YafTaskModule.Current.IsTaskRunning(SyncMembershipUsersTask.TaskName))
			{
				// continue...
				return;
			}

			this.UpdateStatusTimer.Enabled = false;

			// done here...
			YafBuildLink.Redirect(ForumPages.admin_users);
		}

		#endregion


		/* Methods */

		#region Data Binding

		/// <summary>
		/// The bind data.
		/// </summary>
		private void BindData()
		{
			// default since date is now
			DateTime sinceDate = DateTime.Now;
			// default since option is "since last visit"
			int sinceValue = 0;

			// is any "since"option selected
			if (this.Since.SelectedItem != null)
			{
				// get selected value
				sinceValue = int.Parse(this.Since.SelectedItem.Value);
				//sinceDate = DateTime.Now;	// no need to do it again (look above)

				// decrypt selected option
				if (sinceValue == 9999)		// all
				{
					// get all, from the beginning
					sinceDate = (DateTime)(System.Data.SqlTypes.SqlDateTime.MinValue);
				}
				else if (sinceValue > 0)	// days
				{
					// get posts newer then defined number of days
					sinceDate = DateTime.Now - TimeSpan.FromDays(sinceValue);
				}
				else if (sinceValue < 0)	// hours
				{
					// get posts newer then defined number of hours
					sinceDate = DateTime.Now + TimeSpan.FromHours(sinceValue);
				}
			}

			// we want to filter topics since last visit
			if (sinceValue == 0) sinceDate = Mession.LastVisit;

			// we are going to page results
			var pds = new PagedDataSource();
			pds.AllowPaging = true;
			// page size defined by pager's size
			pds.PageSize = this.PagerTop.PageSize;

			// get users, eventually filter by groups or ranks
			using (
			  DataTable dt = DB.user_list(
				 PageContext.PageBoardID,
				 null,
				 null,
				 this.group.SelectedIndex <= 0 ? null : this.group.SelectedValue,
                 this.rank.SelectedIndex <= 0 ? null : this.rank.SelectedValue, 
                 false))
			{
				using (DataView dv = dt.DefaultView)
				{
					// filter by name or email
					if (this.name.Text.Trim().Length > 0 || (this.Email.Text.Trim().Length > 0))
					{
						dv.RowFilter = String.Format(
							"Name LIKE '%{0}%' AND Email LIKE '%{1}%'",
								this.name.Text.Trim(),
								this.Email.Text.Trim()
							);
					}

					// filter by date of registration
					if (sinceValue != 9999)
					{
						dv.RowFilter += String.Format(
							"{1}Joined > '{0}'",
								sinceDate.ToString(),
								String.IsNullOrEmpty(dv.RowFilter) ? "" : " AND "
							);
					}

					// set pager and datasource
					this.PagerTop.Count = dv.Count;
					pds.DataSource = dv;
					// page to render
					pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
					// if we are above total number of pages, select last
					if (pds.CurrentPageIndex >= pds.PageCount) pds.CurrentPageIndex = pds.PageCount - 1;

					// bind list
					this.UserList.DataSource = pds;
					this.UserList.DataBind();
				}
			}
		}


		/// <summary>
		/// Initializes dropdown with options to filter results by date.
		/// </summary>
		protected void InitSinceDropdown()
		{
			// value 0, for since last visted
			this.Since.Items.Add(new ListItem(String.Format("Last visit at {0}", YafServices.DateTime.FormatDateTime(Mession.LastVisit)), "0"));
			// negative values for hours backward
			this.Since.Items.Add(new ListItem("Last hour", "-1"));
			this.Since.Items.Add(new ListItem("Last two hours", "-2"));
			// positive values for days backward
			this.Since.Items.Add(new ListItem("Last day", "1"));
			this.Since.Items.Add(new ListItem("Last two days", "2"));
			this.Since.Items.Add(new ListItem("Last week", "7"));
			this.Since.Items.Add(new ListItem("Last two weeks", "14"));
			this.Since.Items.Add(new ListItem("Last month", "31"));
			// all time (i.e. no filter)
			this.Since.Items.Add(new ListItem("All time", "9999"));
		}

		#endregion

		#region Data Checking Helpers

		/// <summary>
		/// The bit set.
		/// </summary>
		/// <param name="_o">
		/// The _o.
		/// </param>
		/// <param name="bitmask">
		/// The bitmask.
		/// </param>
		/// <returns>
		/// The bit set.
		/// </returns>
		protected bool BitSet(object _o, int bitmask)
		{
			var i = (int)_o;
			return (i & bitmask) != 0;
		}

		#endregion
	}
}