/* Yet Another Forum.NET
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
  // YAF.Pages
  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for activeusers.
  /// </summary>
  public partial class activeusers : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="activeusers"/> class.
    /// </summary>
    public activeusers()
      : base("ACTIVEUSERS")
    {
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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);

        string key = YafCache.GetBoardCacheKey(Constants.Cache.UsersOnlineStatus);
        DataTable activeUsers = PageContext.Cache.GetItem(
          key, (double) YafContext.Current.BoardSettings.OnlineStatusCacheTimeout, () => YafServices.DBBroker.GetActiveList(true));

        // remove hidden users...
        foreach (DataRow row in activeUsers.Rows)
        {
          if (Convert.ToBoolean(row["IsHidden"]) && !PageContext.IsAdmin && !(PageContext.PageUserID == Convert.ToInt32(row["UserID"])))
          {
            // remove this active user...
            row.Delete();
          }
        }

        activeUsers.AcceptChanges();

        this.UserList.DataSource = activeUsers;
        DataBind();
      }
    }
  }
}