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

namespace YAF.Controls
{
    using System;
    using System.Data;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using YAF.Classes;
    using YAF.Classes.Core;
    using YAF.Classes.Data;
    using YAF.Classes.UI;
    using YAF.Classes.Utils;
    using YAF.Utilities;
    /// <summary>
    /// Summary description for buddies.
    /// </summary>
    public partial class BuddyList : BaseUserControl
    {
        /// <summary>
        /// the control mode.
        /// </summary>
        private int _controlMode;

        /// <summary>
        /// the _parent.
        /// </summary>
        private Control _container;

        /// <summary>
        /// The parent control of the current control. (Used in rptBuddy_ItemCommand method)
        /// </summary>
        public Control Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }
        /// <summary>
        /// Determines what is th current mode of the control.
        /// </summary>
        public int Mode
        {
            get
            {
                return this._controlMode;
            }
            set
            {
                this._controlMode = value;
            }

        }
        /// <summary>
        /// the current user id.
        /// </summary>
        private int _currentUserID;

        /// <summary>
        /// Gets the user ID.
        /// </summary>
        public int CurrentUserID
        {
            get
            {
                return this._currentUserID;
            }
            set
            {
                this._currentUserID = value;
            }
        }
        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetSort("Name", true);

                this.UserName.Text = PageContext.Localization.GetText("username");
                this.Rank.Text = PageContext.Localization.GetText("rank");
                this.Joined.Text = PageContext.Localization.GetText("members", "joined");
                this.Posts.Text = PageContext.Localization.GetText("posts");
                this.Location.Text = PageContext.Localization.GetText("location");
                if (Mode == 4)
                {
                    this.LastColumn.Text = PageContext.Localization.GetText("REQUEST_DATE");
                }
                BindData();
            }
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
        protected void Deny_Load(object sender, EventArgs e)
        {
            ((LinkButton)sender).Attributes["onclick"] = "return confirm('Deny this request?')";
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
        protected void Remove_Load(object sender, EventArgs e)
        {
            ((LinkButton)sender).Attributes["onclick"] = "return confirm('Remove Buddy?')";
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
        protected void DenyAll_Load(object sender, EventArgs e)
        {
            ((Button)sender).Attributes["onclick"] = "return confirm('Delete all Unapproved requests more than 14 days old?')";
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
        protected void ApproveAll_Load(object sender, EventArgs e)
        {
            ((Button)sender).Attributes["onclick"] = "return confirm('Approve all requests?')";
        }

        /// <summary>
        /// The approve add all_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ApproveAddAll_Load(object sender, EventArgs e)
        {
            ((Button)sender).Attributes["onclick"] = "return confirm('Approve all requests adn add them to your buddy list too?')";
        }


        /// <summary>
        /// protects from script in "location" field	    
        /// </summary>
        /// <param name="svalue">
        /// </param>
        /// <returns>
        /// The get string safely.
        /// </returns>
        protected string GetStringSafely(object svalue)
        {
            return svalue == null ? string.Empty : HtmlEncode(svalue.ToString());
        }

        /// <summary>
        /// Helper function for setting up the current sort on the memberlist view
        /// </summary>
        /// <param name="field">
        /// </param>
        /// <param name="asc">
        /// </param>
        private void SetSort(string field, bool asc)
        {
            if (ViewState["SortField"] != null && (string)ViewState["SortField"] == field)
            {
                ViewState["SortAscending"] = !(bool)ViewState["SortAscending"];
            }
            else
            {
                ViewState["SortField"] = field;
                ViewState["SortAscending"] = asc;
            }
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
        protected void UserName_Click(object sender, EventArgs e)
        {
            SetSort("Name", true);
            BindData();
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
        protected void Joined_Click(object sender, EventArgs e)
        {
            SetSort("Joined", true);
            BindData();
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
        protected void Posts_Click(object sender, EventArgs e)
        {
            SetSort("NumPosts", false);
            BindData();
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
        protected void Rank_Click(object sender, EventArgs e)
        {
            SetSort("RankName", true);
            BindData();
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
        protected void Requested_Click(object sender, EventArgs e)
        {
            SetSort("Requested", true);
            BindData();
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
        protected void Pager_PageChange(object sender, EventArgs e)
        {
            BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.Pager.PageSize = 20;
            // set the Datatable
            DataTable buddyListDataTable = YafBuddies.GetBuddiesForUser(CurrentUserID);


            if ((buddyListDataTable != null) && (buddyListDataTable.Rows.Count > 0))
            {
                // get the view from the datatable
                DataView buddyListDataView = buddyListDataTable.DefaultView;
                
                // In what mode should this control work?
                // Refer to "rptBuddy_ItemCreate" event for more info.
                switch (Mode)
                {
                    case 1:
                        case 2:
                        buddyListDataView.RowFilter = string.Format("Approved = 1", CurrentUserID);
                        break;
                    case 3:
                        buddyListDataView.RowFilter = string.Format("Approved = 0 AND FromUserID <> {0}", CurrentUserID);
                        break;
                    case 4:
                        buddyListDataView.RowFilter = string.Format("Approved = 0 AND FromUserID = {0}", CurrentUserID);
                        break;
                }


                char selectedLetter = this.AlphaSort1.CurrentLetter;

                // handle dataview filtering
                if (selectedLetter != char.MinValue)
                {
                    if (selectedLetter == '#')
                    {
                        string filter = string.Empty;
                        foreach (char letter in PageContext.Localization.GetText("LANGUAGE", "CHARSET"))
                        {
                            if (filter == string.Empty)
                            {
                                filter = string.Format("Name not like '{0}%'", letter);
                            }
                            else
                            {
                                filter += string.Format("and Name not like '{0}%'", letter);
                            }
                        }

                        buddyListDataView.RowFilter = filter;
                    }
                    else
                    {
                        buddyListDataView.RowFilter = string.Format("Name like '{0}%'", selectedLetter);
                    }
                }

                this.Pager.Count = buddyListDataView.Count;

                // create paged data source for the buddylist
                buddyListDataView.Sort = String.Format("{0} {1}", ViewState["SortField"], (bool)ViewState["SortAscending"] ? "asc" : "desc");
                var pds = new PagedDataSource();
                pds.DataSource = buddyListDataView;
                pds.AllowPaging = true;
                pds.CurrentPageIndex = this.Pager.CurrentPageIndex;
                pds.PageSize = this.Pager.PageSize;

                this.rptBuddy.DataSource = pds;
            }
            DataBind();
            // handle the sort fields at the top
            // TODO: make these "sorts" into controls
            ForumPage fp = new ForumPage();
            this.SortUserName.Visible = (string)ViewState["SortField"] == "Name";
            this.SortUserName.Src = fp.GetThemeContents("SORT", (bool)ViewState["SortAscending"] ? "ASCENDING" : "DESCENDING");
            this.SortRank.Visible = (string)ViewState["SortField"] == "RankName";
            this.SortRank.Src = this.SortUserName.Src;
            this.SortJoined.Visible = (string)ViewState["SortField"] == "Joined";
            this.SortJoined.Src = this.SortUserName.Src;
            this.SortPosts.Visible = (string)ViewState["SortField"] == "NumPosts";
            this.SortPosts.Src = this.SortUserName.Src;
        }

        protected void rptBuddy_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            // In what mode should this control work?
            // 1: Just display the buddy list
            // 2: display the buddy list and ("Remove Buddy") buttons.
            // 3: display pending buddy list posted to current user and add ("approve","approve all", "deny",
            //    "deny all","approve and add", "approve and add all") buttons.
            // 4: show the pending requests posted from the current user.
            switch (Mode)
            {
                case 1:
                    tdLastColHdr.Visible = false;
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        HtmlTableCell lastColumn = (HtmlTableCell)e.Item.FindControl("tdLastCol");
                        lastColumn.Visible = false;
                    }
                    break;
                case 2:
                    tdLastColHdr.Visible = true;
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        Panel pnlLastCol = (Panel)e.Item.FindControl("pnlRemove");
                        pnlLastCol.Visible = true;
                    }
                    break;
                case 3:
                    tdLastColHdr.Visible = true;
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        Panel pnlLastCol = (Panel)e.Item.FindControl("pnlPending");
                        pnlLastCol.Visible = true;
                    }
                    if (e.Item.ItemType == ListItemType.Footer)
                    {
                        if (rptBuddy.Items.Count > 0)
                        {
                            HtmlTableRow rptFooter = (HtmlTableRow)e.Item.FindControl("rptFooter");
                            rptFooter.Visible = true;
                        }
                    }
                    break;
                case 4:
                    tdLastColHdr.Visible = true;
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        Panel pnlLastCol = (Panel)e.Item.FindControl("pnlRequests");
                        pnlLastCol.Visible = true;
                    }
                    break;
            }
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
        protected void rptBuddy_ItemCommand(object sender, CommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    PageContext.AddLoadMessage(string.Format(PageContext.Localization.GetText("REMOVEBUDDY_NOTIFICATION"), YafBuddies.RemoveBuddy(Convert.ToInt32(e.CommandArgument))));
                    CurrentUserID = PageContext.PageUserID;
                    break;
                case "approve":
                    PageContext.AddLoadMessage(string.Format(PageContext.Localization.GetText("NOTIFICATION_BUDDYAPPROVED"), YafBuddies.ApproveBuddyRequest(Convert.ToInt32(e.CommandArgument), false)));
                    break;
                case "approveadd":
                    PageContext.AddLoadMessage(string.Format(PageContext.Localization.GetText("NOTIFICATION_BUDDYAPPROVED_MUTUAL"), YafBuddies.ApproveBuddyRequest(Convert.ToInt32(e.CommandArgument), true)));
                    break;
                case "approveall":
                    YafBuddies.ApproveAllBuddyRequests(false);
                    PageContext.AddLoadMessage(PageContext.Localization.GetText("NOTIFICATION_ALL_APPROVED"));
                    break;
                case "approveaddall":
                    YafBuddies.ApproveAllBuddyRequests(true);
                    PageContext.AddLoadMessage(PageContext.Localization.GetText("NOTIFICATION_ALL_APPROVED_ADDED"));
                    break;
                case "deny":
                    YafBuddies.DenyBuddyRequest(Convert.ToInt32(e.CommandArgument));
                    PageContext.AddLoadMessage(PageContext.Localization.GetText("NOTIFICATION_BUDDYDENIED"));
                    break;
                case "denyall":
                    YafBuddies.DenyAllBuddyRequests();
                    PageContext.AddLoadMessage(PageContext.Localization.GetText("NOTIFICATION_ALL_DENIED"));
                    break;
            }

            // Update all buddy list controls in cp_editbuddies.ascx page.
            UpdateBuddyList(ControlHelper.FindControlRecursiveAs<BuddyList>(Container, "BuddyList1"), 2);
            UpdateBuddyList(ControlHelper.FindControlRecursiveAs<BuddyList>(Container, "PendingBuddyList"), 3);
            UpdateBuddyList(ControlHelper.FindControlRecursiveAs<BuddyList>(Container, "BuddyRequested"), 4);
        }
        
        /// <summary>
        /// Initializes the values of BuddyList control's properties and calls the BindData() 
        /// method of the control.
        /// </summary>
        /// <param name="CustomBuddyList">
        /// The BuddyList control
        /// </param>
        /// <param name="BuddyListMode">
        /// The mode of this BuddyList.
        /// </param>
        private void UpdateBuddyList(BuddyList customBuddyList, int BuddyListMode)
        {
            customBuddyList.Mode = BuddyListMode;
            customBuddyList.CurrentUserID = this.CurrentUserID;
            customBuddyList.Container = this.Container;
            customBuddyList.BindData();
        }
    }
}
