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
// written by vzrus
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
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for forums.
  /// </summary>
    public partial class pageaccesslist : AdminPage
  {
    /* Construction */
    #region Methods


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
      this.PageLinks.AddLink(this.GetText("ADMIN_PAGEACCESSLIST", "TITLE"), string.Empty);

      this.Page.Header.Title = "{0} - {1}".FormatWith(
         this.GetText("ADMIN_ADMIN", "Administration"),
         this.GetText("ADMIN_PAGEACCESSLIST", "TITLE"));
    }

    /* Event Handlers */

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
          YafBuildLink.Redirect(ForumPages.admin_pageaccessedit, "u={0}", e.CommandArgument);
          break;
      }
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
      // list admins but not host admins
        this.List.DataSource = LegacyDb.admin_pageaccesslist(null, true);
        this.DataBind();
    }

    #endregion
  }
}