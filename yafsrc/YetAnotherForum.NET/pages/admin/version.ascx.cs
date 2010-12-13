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

namespace YAF.Pages.Admin
{
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;
  using YAF.RegisterForum;

  /// <summary>
  /// Summary description for register.
  /// </summary>
  public partial class version : AdminPage
  {
    /// <summary>
    /// The _last version.
    /// </summary>
    private long _lastVersion;

    /// <summary>
    /// The _last version date.
    /// </summary>
    private DateTime _lastVersionDate;

    /// <summary>
    /// Gets LastVersion.
    /// </summary>
    protected string LastVersion
    {
      get
      {
        return YafForumInfo.AppVersionNameFromCode(this._lastVersion);
      }
    }

    /// <summary>
    /// Gets LastVersionDate.
    /// </summary>
    protected string LastVersionDate
    {
      get
      {
        return this.Get<IDateTime>().FormatDateShort(this._lastVersionDate);
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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Version Check", string.Empty);
      }
      {
        // try
        using (var reg = new Register())
        {
          this._lastVersion = reg.LatestVersion();
          this._lastVersionDate = reg.LatestVersionDate();
        }
      }
      {
        // catch ( Exception )
        // _lastVersion = 0;
      }

      this.Upgrade.Visible = this._lastVersion > YafForumInfo.AppVersionCode;


      DataBind();
    }
  }
}