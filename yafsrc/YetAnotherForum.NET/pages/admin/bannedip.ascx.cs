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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using YAF.Classes;
using YAF.Utils.Helpers;

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for bannedip.
  /// </summary>
  public partial class bannedip : AdminPage
  {
    #region Methods

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

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

        this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1}".FormatWith(
           this.GetText("ADMIN_ADMIN", "Administration"),
           this.GetText("ADMIN_BANNEDIP", "TITLE"));

        this.BindData();
    }

      /// <summary>
      /// The Add_Load
      /// </summary>
      /// <param name="sender">
      /// The sender.
      /// </param>
      /// <param name="e">
      /// The e.
      /// </param>
      protected void Add_Load([NotNull] object sender, [NotNull] EventArgs e)
      {
          var addButton = (Button)sender;
          
          addButton.Text = this.GetText("ADMIN_BANNEDIP", "ADD_IP");
      }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void list_ItemCommand([NotNull] object sender, [NotNull] RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "add":
                YafBuildLink.Redirect(ForumPages.admin_bannedip_edit);
                break;
            case "edit":
                YafBuildLink.Redirect(ForumPages.admin_bannedip_edit, "i={0}", e.CommandArgument);
                break;
            case "delete":
                string ip = GetIPFromID(e.CommandArgument);
                LegacyDb.bannedip_delete(e.CommandArgument);
                this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);
                this.BindData();
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_BANNEDIP", "MSG_REMOVEBAN"));
                this.Get<ILogger>().IpBanLifted(this.PageContext.PageUserID, " YAF.Pages.Admin.bannedip", "IP or mask {0} was deleted by {1}.".FormatWith(ip, this.Get<YafBoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName));
                break;
        }
    }
    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerTop_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
        // rebind
        this.BindData();
    }
      /// <summary>
      ///  Helper to get mask from ID.
      ///  </summary>
      ///  <param name="ID"></param>
      ///  <returns></returns>
      private string GetIPFromID(object ID)
      {
          foreach (RepeaterItem ri in this.list.Items)
          {
              var chid = ((Label)ri.FindControl("MaskBox")).Text;
              var fid = ((HiddenField)ri.FindControl("fID")).Value;
              if (ID.ToString() == fid)
              {
                  return chid;
                  break;
              }
          }
          return null;
      }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.PagerTop.PageSize = this.Get<YafBoardSettings>().MemberListPageSize;
        var dt = LegacyDb.bannedip_list(this.PageContext.PageBoardID, null, this.PagerTop.CurrentPageIndex, this.PagerTop.PageSize);
        this.list.DataSource = dt;
        if (dt != null && dt.Rows.Count > 0)
        {
            this.PagerTop.Count = dt.AsEnumerable().First().Field<int>("TotalRows");
        }
        else
        {
            this.PagerTop.Count = 0;
        }
        
        this.DataBind();
    }

    #endregion
  }
}