/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;

  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for buddies.
  /// </summary>
  public partial class BuddyList : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   The parent control of the current control. (Used in rptBuddy_ItemCommand method)
    /// </summary>
    public Control Container { get; set; }

    /// <summary>
    ///   Gets the user ID.
    /// </summary>
    public int CurrentUserID { get; set; }

    /// <summary>
    ///   Determines what is th current mode of the control.
    /// </summary>
    public int Mode { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// The approve add all_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ApproveAddAll_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((Button)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(
          this.GetText("CP_EDITBUDDIES", "NOTIFICATION_APPROVEALLADD"));
    }

    /// <summary>
    /// The approve all_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void ApproveAll_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((Button)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(
          this.GetText("CP_EDITBUDDIES", "NOTIFICATION_APPROVEALL"));
    }

    /// <summary>
    /// The deny all_  load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DenyAll_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((Button)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(
          this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE_OLD_UNAPPROVED"));
    }

    /// <summary>
    /// The deny_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Deny_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(this.GetText("CP_EDITBUDDIES", "NOTIFICATION_DENY"));
    }

    /// <summary>
    /// protects from script in "location" field
    /// </summary>
    /// <param name="svalue">
    /// </param>
    /// <returns>
    /// The get string safely.
    /// </returns>
    protected string GetStringSafely([NotNull] object svalue)
    {
      return svalue == null ? string.Empty : this.HtmlEncode(svalue.ToString());
    }

    /// <summary>
    /// The joined_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Joined_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.SetSort("Joined", true);
      this.BindData();
    }

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.SetSort("Name", true);

        this.UserName.Text = this.GetText("username");
        this.Rank.Text = this.GetText("rank");
        this.Joined.Text = this.GetText("members", "joined");
        this.Posts.Text = this.GetText("CP_EDITBUDDIES", "POSTS");
        this.Location.Text = this.HtmlEncode(this.GetText("POSTS", "LOCATION"));
        if (this.Mode == 4)
        {
            this.LastColumn.Text = this.GetText("REQUEST_DATE");
        }

        this.BindData();
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// The posts_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Posts_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.SetSort("NumPosts", false);
      this.BindData();
    }

    /// <summary>
    /// The rank_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Rank_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.SetSort("RankName", true);
      this.BindData();
    }

    /// <summary>
    /// The remove_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Remove_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((LinkButton)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith(
          this.GetText("CP_EDITBUDDIES", "NOTIFICATION_REMOVE"));
    }

    /// <summary>
    /// The requested_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Requested_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.SetSort("Requested", true);
      this.BindData();
    }

    /// <summary>
    /// The user name_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UserName_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.SetSort("Name", true);
      this.BindData();
    }

    /// <summary>
    /// The ItemCommand method for the link buttons in the last column.
    /// </summary>
    /// <param name="sender">
    /// the sender.
    /// </param>
    /// <param name="e">
    /// the e.
    /// </param>
    protected void rptBuddy_ItemCommand([NotNull] object sender, [NotNull] CommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "remove":
          this.PageContext.AddLoadMessage(
            this.GetText("REMOVEBUDDY_NOTIFICATION").FormatWith(
              this.Get<IBuddy>().Remove(Convert.ToInt32(e.CommandArgument))));
          this.CurrentUserID = this.PageContext.PageUserID;
          break;
        case "approve":
          this.PageContext.AddLoadMessage(
            this.GetText("NOTIFICATION_BUDDYAPPROVED").FormatWith(
              this.Get<IBuddy>().ApproveRequest(Convert.ToInt32(e.CommandArgument), false)));
          break;
        case "approveadd":
          this.PageContext.AddLoadMessage(
            this.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL").FormatWith(
              this.Get<IBuddy>().ApproveRequest(Convert.ToInt32(e.CommandArgument), true)));
          break;
        case "approveall":
          this.Get<IBuddy>().ApproveAllRequests(false);
          this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_ALL_APPROVED"));
          break;
        case "approveaddall":
          this.Get<IBuddy>().ApproveAllRequests(true);
          this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_ALL_APPROVED_ADDED"));
          break;
        case "deny":
          this.Get<IBuddy>().DenyRequest(Convert.ToInt32(e.CommandArgument));
          this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_BUDDYDENIED"));
          break;
        case "denyall":
          this.Get<IBuddy>().DenyAllRequests();
          this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_ALL_DENIED"));
          break;
      }

      // Update all buddy list controls in cp_editbuddies.ascx page.
      this.UpdateBuddyList(this.Container.FindControlRecursiveAs<BuddyList>("BuddyList1"), 2);
      this.UpdateBuddyList(this.Container.FindControlRecursiveAs<BuddyList>("PendingBuddyList"), 3);
      this.UpdateBuddyList(this.Container.FindControlRecursiveAs<BuddyList>("BuddyRequested"), 4);
    }

    /// <summary>
    /// The rpt buddy_ item created.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void rptBuddy_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
    {
      // In what mode should this control work?
      // 1: Just display the buddy list
      // 2: display the buddy list and ("Remove Buddy") buttons.
      // 3: display pending buddy list posted to current user and add ("approve","approve all", "deny",
      // "deny all","approve and add", "approve and add all") buttons.
      // 4: show the pending requests posted from the current user.
      switch (this.Mode)
      {
        case 1:
          this.tdLastColHdr.Visible = false;
          if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
          {
            var lastColumn = (HtmlTableCell)e.Item.FindControl("tdLastCol");
            lastColumn.Visible = false;
          }

          break;
        case 2:
          this.tdLastColHdr.Visible = true;
          if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
          {
            var pnlLastCol = (Panel)e.Item.FindControl("pnlRemove");
            pnlLastCol.Visible = true;
          }

          break;
        case 3:
          this.tdLastColHdr.Visible = true;
          if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
          {
            var pnlLastCol = (Panel)e.Item.FindControl("pnlPending");
            pnlLastCol.Visible = true;
          }

          if (e.Item.ItemType == ListItemType.Footer)
          {
            if (this.rptBuddy.Items.Count > 0)
            {
              var rptFooter = (HtmlTableRow)e.Item.FindControl("rptFooter");
              rptFooter.Visible = true;
            }
          }

          break;
        case 4:
          this.tdLastColHdr.Visible = true;
          if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
          {
            var pnlLastCol = (Panel)e.Item.FindControl("pnlRequests");
            pnlLastCol.Visible = true;
          }

          break;
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.Pager.PageSize = 20;

      // set the Datatable
      var buddyListDataTable = this.Get<IBuddy>().GetForUser(this.CurrentUserID);

      if ((buddyListDataTable != null) && buddyListDataTable.HasRows())
      {
        // get the view from the datatable
        var buddyListDataView = buddyListDataTable.DefaultView;

        // In what mode should this control work?
        // Refer to "rptBuddy_ItemCreate" event for more info.
        switch (this.Mode)
        {
          case 1:
          case 2:
            buddyListDataView.RowFilter = "Approved = 1".FormatWith(this.CurrentUserID);
            break;
          case 3:
            buddyListDataView.RowFilter = "Approved = 0 AND FromUserID <> {0}".FormatWith(this.CurrentUserID);
            break;
          case 4:
            buddyListDataView.RowFilter = "Approved = 0 AND FromUserID = {0}".FormatWith(this.CurrentUserID);
            break;
        }

        this.Pager.Count = buddyListDataView.Count;

        // create paged data source for the buddylist
        buddyListDataView.Sort = "{0} {1}".FormatWith(
          this.ViewState["SortField"], (bool)this.ViewState["SortAscending"] ? "asc" : "desc");
        var pds = new PagedDataSource();
        pds.DataSource = buddyListDataView;
        pds.AllowPaging = true;
        pds.CurrentPageIndex = this.Pager.CurrentPageIndex;
        pds.PageSize = this.Pager.PageSize;

        this.rptBuddy.DataSource = pds;
      }

      this.DataBind();

      // handle the sort fields at the top
      // TODO: make these "sorts" into controls
      this.SortUserName.Visible = (string)this.ViewState["SortField"] == "Name";
      this.SortUserName.Src = this.Get<ITheme>().GetItem(
        "SORT", (bool)this.ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING");
      this.SortRank.Visible = (string)this.ViewState["SortField"] == "RankName";
      this.SortRank.Src = this.SortUserName.Src;
      this.SortJoined.Visible = (string)this.ViewState["SortField"] == "Joined";
      this.SortJoined.Src = this.SortUserName.Src;
      this.SortPosts.Visible = (string)this.ViewState["SortField"] == "NumPosts";
      this.SortPosts.Src = this.SortUserName.Src;
    }

    /// <summary>
    /// Helper function for setting up the current sort on the memberlist view
    /// </summary>
    /// <param name="field">
    /// </param>
    /// <param name="asc">
    /// </param>
    private void SetSort([NotNull] string field, bool asc)
    {
      if (this.ViewState["SortField"] != null && (string)this.ViewState["SortField"] == field)
      {
        this.ViewState["SortAscending"] = !(bool)this.ViewState["SortAscending"];
      }
      else
      {
        this.ViewState["SortField"] = field;
        this.ViewState["SortAscending"] = asc;
      }
    }

    /// <summary>
    /// Initializes the values of BuddyList control's properties and calls the BindData() 
    ///   method of the control.
    /// </summary>
    /// <param name="customBuddyList">
    /// The BuddyList control
    /// </param>
    /// <param name="BuddyListMode">
    /// The mode of this BuddyList.
    /// </param>
    private void UpdateBuddyList([NotNull] BuddyList customBuddyList, int BuddyListMode)
    {
      customBuddyList.Mode = BuddyListMode;
      customBuddyList.CurrentUserID = this.CurrentUserID;
      customBuddyList.Container = this.Container;
      customBuddyList.BindData();
    }

    #endregion
  }
}