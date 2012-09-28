/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj?rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Extensions;

    #endregion

    /// <summary>
    /// The thanks list mode.
    /// </summary>
    public enum ThanksListMode
    {
        /// <summary>
        ///   The from user.
        /// </summary>
        FromUser,

        /// <summary>
        ///   The to user.
        /// </summary>
        ToUser
    }

    /// <summary>
    /// Summary description for buddies.
    /// </summary>
    public partial class ViewThanksList : BaseUserControl
    {
        /* Data Fields */

        /* Properties */
        #region Constants and Fields

        /// <summary>
        ///   The _count.
        /// </summary>
        private int _count;

        #endregion

        #region Properties

        /// <summary>
        ///   Determines what is th current mode of the control.
        /// </summary>
        public ThanksListMode CurrentMode { get; set; }

        /// <summary>
        ///   The Thanks Info.
        /// </summary>
        public DataTable ThanksInfo { get; set; }

        /// <summary>
        ///   The User ID.
        /// </summary>
        public int UserID { get; set; }

        #endregion

        // keeps count

        /* Event Handlers */

        /* Methods */
        #region Public Methods

        /// <summary>
        /// The bind data.
        /// </summary>
        public void BindData()
        {
            this._count = 0;

            if (!this.ThanksInfo.Columns.Contains("MessageThanksNumber"))
            {
                this.ThanksInfo.Columns.Add("MessageThanksNumber", typeof(int));
            }

            // now depending on mode filter the table
            var thanksData = this.ThanksInfo.AsEnumerable();

            if (!thanksData.Any())
            {
                this.NoResults.Visible = true;
                return;
            }

            if (this.CurrentMode == ThanksListMode.FromUser)
            {
                thanksData = thanksData.Where(x => x.Field<int>("ThanksFromUserID") == this.UserID);
            }
            else if (this.CurrentMode == ThanksListMode.ToUser)
            {
                foreach (var dr in thanksData)
                {
                    // update the message count
                    int messageThanksNumber;
                    dr["MessageThanksNumber"] = int.TryParse(dr["MessageThanksNumber"].ToString(), out messageThanksNumber)
                                                ? dr["MessageThanksNumber"]
                                                : thanksData.Count(x => x.Field<int>("ThanksToUserID") == this.UserID &&
                                                                        x.Field<int>("MessageID") == (int)dr["MessageID"]);
                }

                thanksData = thanksData.Where(x => x.Field<int>("ThanksToUserID") == this.UserID);

                // Remove duplicates.
                this.DistinctMessageID(thanksData);

                // Sort by the ThanksNumber (Descensing)
                thanksData = thanksData.OrderByDescending(x => x.Field<int>("MessageThanksNumber"));

                // Update the datatable with changes
                this.ThanksInfo.AcceptChanges();
            }

            // TODO : page size definable?
            this.PagerTop.PageSize = 15;

            // set datasource of repeater
            this.ThanksRes.DataSource = thanksData.ToList().GetPaged(this.PagerTop);

            // data bind controls
            this.DataBind();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns <see langword="true"/> if the count is odd
        /// </summary>
        /// <returns>
        /// The is odd.
        /// </returns>
        protected bool IsOdd()
        {
            return (this._count++ % 2) == 0;
        }

        /* Methods */

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
        /// Handles the ItemCreated event of the ThanksRes control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> 
        ///   instance containing the event data.
        /// </param>
        protected void ThanksRes_ItemCreated([NotNull] object sender, [NotNull] RepeaterItemEventArgs e)
        {
            // In what mode should this control work?
            // 1: Just display the buddy list
            // 2: display the buddy list and ("Remove Buddy") buttons.
            // 3: display pending buddy list posted to current user and add ("approve","approve all", "deny",
            // "deny all","approve and add", "approve and add all") buttons.
            // 4: show the pending requests posted from the current user.
            switch (this.CurrentMode)
            {
                case ThanksListMode.FromUser:
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        var thanksNumberCell = (HtmlTableCell)e.Item.FindControl("ThanksNumberCell");
                        thanksNumberCell.Visible = false;
                    }

                    break;
                case ThanksListMode.ToUser:
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        var nameCell = (HtmlTableCell)e.Item.FindControl("NameCell");
                        nameCell.Visible = false;
                    }

                    break;
            }
        }

        /// <summary>
        /// Removes rows with duplicate MessageIDs.
        /// </summary>
        /// <param name="thanksData">
        /// The thanks data.
        /// </param>
        private void DistinctMessageID([NotNull] IEnumerable<DataRow> thanksData)
        {
            int previousId = 0;
            int tempId;

            foreach (var dr in thanksData.OrderBy(x => x.Field<int>("MessageID")))
            {
                tempId = dr.Field<int>("MessageID");
                if (dr.Field<int>("MessageID") == previousId)
                {
                    dr.Delete();
                }

                previousId = tempId;
            }
        }

        #endregion
    }
}