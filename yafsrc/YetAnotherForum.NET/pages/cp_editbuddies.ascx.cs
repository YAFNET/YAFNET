/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Pages
{
    #region Using

    using System;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The cp_editbuddies.
    /// </summary>
    public partial class cp_editbuddies : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the cp_editbuddies class.
        /// </summary>
        public cp_editbuddies()
            : base("CP_EDITBUDDIES")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJQuery();
            YafContext.Current.PageElements.RegisterJQueryUI();

            YafContext.Current.PageElements.RegisterJsBlock(
                "yafBuddiesTabsJs",
                JavaScriptBlocks.JqueryUITabsLoadJs(this.BuddiesTabs.ClientID, this.hidLastTab.ClientID, false));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));
                this.PageLinks.AddLink(
                    this.Get<YafBoardSettings>().EnableDisplayName
                        ? this.PageContext.CurrentUserData.DisplayName
                        : this.PageContext.PageUserName,
                    YafBuildLink.GetLink(ForumPages.cp_profile));
                this.PageLinks.AddLink(this.GetText("BUDDYLIST_TT"), string.Empty);
            }

            this.BindData();
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.InitializeBuddyList(this.BuddyList1, 2);
            this.InitializeBuddyList(this.PendingBuddyList, 3);
            this.InitializeBuddyList(this.BuddyRequested, 4);
        }

        /// <summary>
        /// Initializes the values of BuddyList control's properties.
        /// </summary>
        /// <param name="customBuddyList">
        /// The custom BuddyList control.
        /// </param>
        /// <param name="mode">
        /// The mode of this BuddyList.
        /// </param>
        private void InitializeBuddyList([NotNull] BuddyList customBuddyList, int mode)
        {
            customBuddyList.CurrentUserID = this.PageContext.PageUserID;
            customBuddyList.Mode = mode;
            customBuddyList.Container = this;
        }

        #endregion
    }
}