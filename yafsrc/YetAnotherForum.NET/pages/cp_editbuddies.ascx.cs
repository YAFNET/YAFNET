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
namespace YAF.Pages
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;
  using YAF.Controls;

  #endregion

  /// <summary>
  /// The cp_editbuddies.
  /// </summary>
  public partial class cp_editbuddies : ForumPageRegistered
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the cp_editbuddies class.
    /// </summary>
    public cp_editbuddies()
      : base("CP_EDITBUDDIES")
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Called when the page loads
    /// </summary>
    /// <param name="sender">
    /// </param>
    /// <param name="e">
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.IsPostBack)
      {
        string displayName = this.PageContext.CurrentUserData.DisplayName;
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(
          this.HtmlEncode(!string.IsNullOrEmpty(displayName) ? displayName : this.PageContext.PageUserName), 
          YafBuildLink.GetLink(ForumPages.cp_profile));
        this.PageLinks.AddLink(this.PageContext.Localization.GetText("BUDDYLIST_TT"), string.Empty);
      }

      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.BuddiesTabs.Views["BuddyListTab"].Text = this.GetText("CP_EDITBUDDIES", "BUDDYLIST");
      this.BuddiesTabs.Views["PendingRequestsTab"].Text = this.GetText("CP_EDITBUDDIES", "PENDING_REQUESTS");
      this.BuddiesTabs.Views["YourRequestsTab"].Text = this.GetText("CP_EDITBUDDIES", "YOUR_REQUESTS");
      this.InitializeBuddyList(BuddyList1, 2);
      this.InitializeBuddyList(PendingBuddyList, 3);
      this.InitializeBuddyList(BuddyRequested, 4);
    }

    /// <summary>
    /// Initializes the values of BuddyList control's properties.
    /// </summary>
    /// <param name="customBuddyList">
    /// The custom BuddyList control.
    /// </param>
    /// <param name="Mode">
    /// The mode of this BuddyList.
    /// </param>
    private void InitializeBuddyList(BuddyList customBuddyList, int Mode)
    {
      customBuddyList.CurrentUserID = this.PageContext.PageUserID;
      customBuddyList.Mode = Mode;
      customBuddyList.Container = this;
    }

    #endregion
  }
}