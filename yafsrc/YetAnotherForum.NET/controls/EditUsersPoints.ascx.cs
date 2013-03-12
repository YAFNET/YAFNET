/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users points.
    /// </summary>
    public partial class EditUsersPoints : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets user ID of edited user.
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                return this.PageContext.QueryIDs["u"].ToType<int>();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add points_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AddPoints_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            LegacyDb.user_addpoints(this.CurrentUserID, null, this.txtAddPoints.Text);

            this.BindData();
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
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            if (this.IsPostBack)
            {
                return;
            }

            this.Button1.Text = this.Get<ILocalization>().GetText("COMMON", "GO");
            this.btnAddPoints.Text = this.Get<ILocalization>().GetText("COMMON", "GO");
            this.btnUserPoints.Text = this.Get<ILocalization>().GetText("COMMON", "GO");

            this.BindData();
        }

        /// <summary>
        /// The remove points_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RemovePoints_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            LegacyDb.user_removepoints(this.CurrentUserID, null, this.txtRemovePoints.Text);
            this.BindData();
        }

        /// <summary>
        /// The set user points_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SetUserPoints_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            LegacyDb.user_setpoints(this.CurrentUserID, this.txtUserPoints.Text);
            this.BindData();
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            this.ltrCurrentPoints.Text = LegacyDb.user_getpoints(this.CurrentUserID).ToString();
        }

        #endregion
    }
}