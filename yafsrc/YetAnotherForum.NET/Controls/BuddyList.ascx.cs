/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Buddy List Control
    /// </summary>
    public partial class BuddyList : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   The parent control of the current control. (Used in rptBuddy_ItemCommand method)
        /// </summary>
        public Control Container { get; set; }

        /// <summary>
        ///   Gets or sets the user ID.
        /// </summary>
        public int CurrentUserID { get; set; }

        /// <summary>
        ///   Gets or sets the Determines what is th current mode of the control.
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        #endregion

        #region Methods

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
                        string.Format(
                            this.GetText("REMOVEBUDDY_NOTIFICATION"),
                            this.Get<IFriends>().Remove(e.CommandArgument.ToType<int>())),
                        MessageTypes.success);
                    this.CurrentUserID = this.PageContext.PageUserID;
                    break;
                case "approve":
                    this.PageContext.AddLoadMessage(
                        string.Format(
                            this.GetText("NOTIFICATION_BUDDYAPPROVED"),
                            this.Get<IFriends>().ApproveRequest(e.CommandArgument.ToType<int>(), false)),
                        MessageTypes.success);
                    break;
                case "approveadd":
                    this.PageContext.AddLoadMessage(
                        string.Format(
                            this.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"),
                            this.Get<IFriends>().ApproveRequest(e.CommandArgument.ToType<int>(), true)),
                        MessageTypes.success);
                    break;
                case "approveall":
                    this.Get<IFriends>().ApproveAllRequests(false);
                    this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_ALL_APPROVED"), MessageTypes.success);
                    break;
                case "approveaddall":
                    this.Get<IFriends>().ApproveAllRequests(true);
                    this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_ALL_APPROVED_ADDED"), MessageTypes.success);
                    break;
                case "deny":
                    this.Get<IFriends>().DenyRequest(e.CommandArgument.ToType<int>());
                    this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_BUDDYDENIED"), MessageTypes.info);
                    break;
                case "denyall":
                    this.Get<IFriends>().DenyAllRequests();
                    this.PageContext.AddLoadMessage(this.GetText("NOTIFICATION_ALL_DENIED"), MessageTypes.info);
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
            // 2: display the buddy list and ("Remove Buddy") buttons.
            // 3: display pending buddy list posted to current user and add ("approve","approve all", "deny",
            // "deny all","approve and add", "approve and add all") buttons.
            // 4: show the pending requests posted from the current user.
            switch (this.Mode)
            {
                case 2:
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        e.Item.FindControlAs<PlaceHolder>("pnlRemove").Visible = true;
                    }

                    break;
                case 3:
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        e.Item.FindControlAs<PlaceHolder>("pnlPending").Visible = true;
                    }

                    if (e.Item.ItemType == ListItemType.Footer)
                    {
                        if (this.rptBuddy.Items.Count > 0)
                        {
                            e.Item.FindControlAs<Panel>("Footer").Visible = true;
                        }
                    }

                    break;
                case 4:
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        e.Item.FindControlAs<PlaceHolder>("pnlRequests").Visible = true;
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

            // set the Data table
            var buddyListDataTable = this.Get<IFriends>().GetForUser(this.CurrentUserID);

            if (buddyListDataTable != null && buddyListDataTable.HasRows())
            {
                // get the view from the data table
                var buddyListDataView = buddyListDataTable.DefaultView;

                // In what mode should this control work?
                // Refer to "rptBuddy_ItemCreate" event for more info.
                buddyListDataView.RowFilter = this.Mode switch
                    {
                        1 => "Approved = 1",
                        2 => "Approved = 1",
                        3 => $"Approved = 0 AND FromUserID <> {this.CurrentUserID}",
                        4 => $"Approved = 0 AND FromUserID = {this.CurrentUserID}",
                        _ => buddyListDataView.RowFilter
                    };

                this.Pager.Count = buddyListDataView.Count;

                var pds = new PagedDataSource
                              {
                                  DataSource = buddyListDataView,
                                  AllowPaging = true,
                                  CurrentPageIndex = this.Pager.CurrentPageIndex,
                                  PageSize = this.Pager.PageSize
                              };

                this.rptBuddy.DataSource = pds;
            }

            this.DataBind();

            this.Count = this.rptBuddy.Items.Count;

            switch (this.Mode)
            {
                case 1:
                case 2:
                    this.Info.Controls.Add(new Literal { Text = $"<i class=\"fas fa-info text-info pr-1\"></i>{this.GetText("INFO_NO")}" });
                    break;
                case 3:
                case 4:
                    this.Info.Controls.Add(new Literal { Text = $"<i class=\"fas fa-check text-success pr-1\"></i>{this.GetText("INFO_PENDING")}" });
                    break;
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