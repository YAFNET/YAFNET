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

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Drawing;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for forums.
  /// </summary>
  public partial class accessmasks : AdminPage
  {
    /* Construction */
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
    /// The bit set.
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
      // board index
      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));

      // administration index
      this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

      // current page label (no link)
      this.PageLinks.AddLink(this.GetText("ADMIN_ACCESSMASKS", "TITLE"), string.Empty);

      this.Page.Header.Title = "{0} - {1}".FormatWith(
         this.GetText("ADMIN_ADMIN", "Administration"),
         this.GetText("ADMIN_ACCESSMASKS", "TITLE"));
    }

    /* Event Handlers */

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // add on click confirm dialog
      ControlHelper.AddOnClickConfirmDialog(sender, this.GetText("ADMIN_ACCESSMASKS", "CONFIRM_DELETE"));
    }

    /// <summary>
    /// Format access mask setting color formatting.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Set access mask flags are rendered green if true, and if not red
    /// </returns>
    protected Color GetItemColor(bool enabled)
    {
        // show enabled flag red
        return enabled ? Color.Green : Color.Red;
    }

    /// <summary>
    /// Get a user friendly item name.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Item Name.
    /// </returns>
    protected string GetItemName(bool enabled)
    {
        return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
    }

      /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void List_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":

          // redirect to editing page
          YafBuildLink.Redirect(ForumPages.admin_editaccessmask, "i={0}", e.CommandArgument);
          break;
        case "delete":

          // attmempt to delete access masks
          if (LegacyDb.accessmask_delete(e.CommandArgument))
          {
            // remove cache of forum moderators
            this.Get<IDataCache>().Remove(Constants.Cache.ForumModerators);
            this.BindData();
          }
          else
          {
            // used masks cannot be deleted
            this.PageContext.AddLoadMessage(this.GetText("ADMIN_ACCESSMASKS", "MSG_NOT_DELETE"));
          }

          // quit switch
          break;
      }
    }

    /// <summary>
    /// The new_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void New_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // redirect to page for access mask creation
      YafBuildLink.Redirect(ForumPages.admin_editaccessmask);
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
        if (this.IsPostBack)
        {
            return;
        }

        this.New.Text = this.GetText("ADMIN_ACCESSMASKS", "NEW_MASK");

        // create links
        this.CreatePageLinks();

        // bind data
        this.BindData();
    }

    /* Methods */

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      // list all access masks for this boeard
        this.List.DataSource = LegacyDb.accessmask_list(this.PageContext.PageBoardID, null);
      this.DataBind();
    }

    #endregion
  }
}