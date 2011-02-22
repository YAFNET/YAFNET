/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
  #region Using

  using System;
  using System.Data;
  using System.Data.SqlTypes;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utilities;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for members.
  /// </summary>
  public partial class users : AdminPage
  {
    // Construction 
    #region Public Methods

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // add confirmation method on click
      ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_USERS", "CONFIRM_DELETE"));
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
    public void NewUser_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // redirect to create new user page
      YafBuildLink.Redirect(ForumPages.admin_reguser);
    }

    /// <summary>
    /// The user list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void UserList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":

          // we are going to edit user - redirect to edit page
          YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
          break;
        case "delete":

          // we are deleting user
          if (this.PageContext.PageUserID == int.Parse(e.CommandArgument.ToString()))
          {
            // deleting yourself isn't an option
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_USERS", "MSG_SELF_DELETE"));
            return;
          }

          // get user(s) we are about to delete                
          using (DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, e.CommandArgument, DBNull.Value))
          {
            // examine each if he's possible to delete
            foreach (DataRow row in dt.Rows)
            {
              if (SqlDataLayerConverter.VerifyInt32(row["IsGuest"]) > 0)
              {
                // we cannot detele guest
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_USERS", "MSG_DELETE_GUEST"));
                return;
              }

                if ((row["IsAdmin"] == DBNull.Value || SqlDataLayerConverter.VerifyInt32(row["IsAdmin"]) <= 0) &&
                    (row["IsHostAdmin"] == DBNull.Value || row["IsHostAdmin"].ToType<int>() <= 0))
                {
                    continue;
                }

                // admin are not deletable either
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_USERS", "MSG_DELETE_ADMIN"));
                return;
            }
          }

          // all is good, user can be deleted
          UserMembershipHelper.DeleteUser(e.CommandArgument.ToType<int>());

          // rebind data
          this.BindData();

          // quit case
          break;
      }
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
    public void search_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // re-bind data
      this.BindData();
    }

    #endregion

    #region Methods

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
    /// The bit set boolean value.
    /// </returns>
    protected bool BitSet([NotNull] object _o, int bitmask)
    {
      var i = (int)_o;
      return (i & bitmask) != 0;
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    protected override void CreatePageLinks()
    {
      // link to board index
      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // link to administration index
      this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

      // current page label (no link)
      this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), string.Empty);

      this.Page.Header.Title = "{0} - {1}".FormatWith(
         this.GetText("ADMIN_ADMIN", "Administration"),
         this.GetText("ADMIN_USERS", "TITLE"));
    }

    /// <summary>
    /// Initializes dropdown with options to filter results by date.
    /// </summary>
    protected void InitSinceDropdown()
    {
      // value 0, for since last visted
      this.Since.Items.Add(
        new ListItem(
          "Last visit at {0}".FormatWith(
            this.Get<IDateTime>().FormatDateTime(this.Get<IYafSession>().LastVisit)), 
          "0"));

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

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.PageContext.PageElements.RegisterJQuery();
      this.PageContext.PageElements.RegisterJsResourceInclude("blockUIJs", "js/jquery.blockUI.js");

      this.Page.Form.DefaultButton = this.search.UniqueID;

      this.search.Focus();

      if (this.IsPostBack)
      {
        return;
      }

      // create page links
      this.CreatePageLinks();

      this.search.Text = this.GetText("ADMIN_USERS", "SEARCH");

      this.NewUser.Text = this.GetText("ADMIN_USERS", "NEW_USER");
      this.SyncUsers.Text = this.GetText("ADMIN_USERS", "SYNC_ALL");

      ControlHelper.AddOnClickConfirmDialog(this.SyncUsers, this.GetText("ADMIN_USERS", "CONFIRM_SYNC"));

      // intialize since filter items
      this.InitSinceDropdown();

      // set since filter to last item "All time"
      this.Since.SelectedIndex = this.Since.Items.Count - 1;
      this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loader.gif");

      // get list of user groups for filtering
      using (DataTable dt = LegacyDb.group_list(this.PageContext.PageBoardID, null))
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
      using (DataTable dt = LegacyDb.rank_list(this.PageContext.PageBoardID, null))
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

      // TODO : page size difinable?
      this.PagerTop.PageSize = 25;

      // Hide "New User" Button on DotNetNuke
      if (Config.IsDotNetNuke)
      {
        this.NewUser.Visible = false;
      }
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
    protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      // rebind
      this.BindData();
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
    protected void Since_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
    {
      // Set the controls' pager index to 0.
      this.PagerTop.CurrentPageIndex = 0;

      // re-bind data
      this.BindData();
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
    protected void SyncUsers_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // start...
      SyncMembershipUsersTask.Start(this.PageContext.PageBoardID);

      // enable timer...
      this.UpdateStatusTimer.Enabled = true;

      // show blocking ui...
      this.PageContext.PageElements.RegisterJsBlockStartup(
        "BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("SyncUsersMessage"));
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
    protected void UpdateStatusTimer_Tick([NotNull] object sender, [NotNull] EventArgs e)
    {
      // see if the migration is done...
      if (this.Get<ITaskModuleManager>().IsTaskRunning(SyncMembershipUsersTask.TaskName))
      {
        // continue...
        return;
      }

      this.UpdateStatusTimer.Enabled = false;

      // done here...
      YafBuildLink.Redirect(ForumPages.admin_users);
    }

    /* Methods */

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      // default since date is now
      DateTime sinceDate = DateTime.UtcNow;

      // default since option is "since last visit"
      int sinceValue = 0;

      // is any "since"option selected
      if (this.Since.SelectedItem != null)
      {
        // get selected value
        sinceValue = int.Parse(this.Since.SelectedItem.Value);

        // sinceDate = DateTime.UtcNow;
        // no need to do it again (look above)
        // decrypt selected option
        if (sinceValue == 9999)
        {
          // all
          // get all, from the beginning
          sinceDate = (DateTime)SqlDateTime.MinValue;
        }
        else if (sinceValue > 0)
        {
          // days
          // get posts newer then defined number of days
          sinceDate = DateTime.UtcNow - TimeSpan.FromDays(sinceValue);
        }
        else if (sinceValue < 0)
        {
          // hours
          // get posts newer then defined number of hours
          sinceDate = DateTime.UtcNow + TimeSpan.FromHours(sinceValue);
        }
      }

      // we want to filter topics since last visit
      if (sinceValue == 0)
      {
        sinceDate = this.Get<IYafSession>().LastVisit;
      }

      // we are going to page results
      var pds = new PagedDataSource { AllowPaging = true, PageSize = this.PagerTop.PageSize };

      // page size defined by pager's size

      // get users, eventually filter by groups or ranks
      using (
        DataTable dt = LegacyDb.user_list(
          this.PageContext.PageBoardID, 
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
            dv.RowFilter =
              "(Name LIKE '%{0}%' OR DisplayName LIKE '%{0}%') AND Email LIKE '%{1}%'".FormatWith(
                this.name.Text.Trim(), this.Email.Text.Trim());
          }

          // filter by date of registration
          if (sinceValue != 9999)
          {
            dv.RowFilter += "{1}Joined > '{0}'".FormatWith(
              sinceDate.ToString(), dv.RowFilter.IsNotSet() ? string.Empty : " AND ");
          }

          // set pager and datasource
          this.PagerTop.Count = dv.Count;
          pds.DataSource = dv;

          // page to render
          pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;

          // if we are above total number of pages, select last
          if (pds.CurrentPageIndex >= pds.PageCount)
          {
            pds.CurrentPageIndex = pds.PageCount - 1;
          }

          // bind list
          this.UserList.DataSource = pds;
          this.UserList.DataBind();
        }
      }
    }

    #endregion
  }
}