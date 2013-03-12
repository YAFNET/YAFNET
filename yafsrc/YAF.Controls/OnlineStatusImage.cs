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
  using System.Data;
  using System.Linq;
  using System.Web.UI;

  using YAF.Core;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Core.Services;
  using YAF.Types.Interfaces.Data;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Provides an Online/Offline status for a YAF User
  /// </summary>
  public class OnlineStatusImage : ThemeImage
  {
    #region Properties

    /// <summary>
    ///   The userid of the user.
    /// </summary>
    public int UserID
    {
      get
      {
        return ViewState["UserID"].ToType<int>();
      }

      set
      {
        ViewState["UserID"] = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter output)
    {
      this.LocalizedTitlePage = "POSTS";

      if (Visible)
      {
        DataTable activeUsers = this.Get<IDataCache>().GetOrSet(
          Constants.Cache.UsersOnlineStatus,
          () => this.Get<YafDbBroker>().GetActiveList(false, YafContext.Current.BoardSettings.ShowCrawlersInActiveList),
          TimeSpan.FromMilliseconds((double)YafContext.Current.BoardSettings.OnlineStatusCacheTimeout));

        if (activeUsers.AsEnumerable().Any(x => x.Field<int>("UserId") == this.UserID && !x.Field<bool>("IsHidden")))
        {
          // online
          this.LocalizedTitleTag = "USERONLINESTATUS";
          this.ThemeTag = "USER_ONLINE";
          this.Alt = "Online";
        }
        else
        {
          // offline
          this.LocalizedTitleTag = "USEROFFLINESTATUS";
          this.ThemeTag = "USER_OFFLINE";
          this.Alt = "Offline";
        }
      }

      base.Render(output);
    }

    #endregion
  }
}