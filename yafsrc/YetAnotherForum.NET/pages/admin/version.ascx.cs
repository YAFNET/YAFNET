/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages.Admin
{
  #region Using

  using System;

  using YAF.Core;
  using RegisterV2;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for register.
  /// </summary>
  public partial class version : AdminPage
  {
    #region Constants and Fields

    /// <summary>
    ///   The _last version.
    /// </summary>
    private long _lastVersion;

    /// <summary>
    ///   The _last version date.
    /// </summary>
    private DateTime _lastVersionDate;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets LastVersion.
    /// </summary>
    protected string LastVersion
    {
      get
      {
        return YafForumInfo.AppVersionNameFromCode(this._lastVersion);
      }
    }

    /// <summary>
    ///   Gets LastVersionDate.
    /// </summary>
    protected string LastVersionDate
    {
      get
      {
        return this.Get<IDateTime>().FormatDateShort(this._lastVersionDate);
      }
    }

    #endregion

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
      if (!this.IsPostBack)
      {
          using (var reg = new RegisterV2())
          {
              this._lastVersion = reg.LatestVersion();
              this._lastVersionDate = reg.LatestVersionDate();
          }

          this.Upgrade.Visible = this._lastVersion > YafForumInfo.AppVersionCode;


        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink(this.GetText("ADMIN_VERSION", "TITLE"), string.Empty);

          this.Page.Header.Title = "{0} - {1}".FormatWith(
              this.GetText("ADMIN_ADMIN", "Administration"), 
              this.GetText("ADMIN_VERSION", "TITLE"));

          this.RunningVersion.Text = this.GetTextFormatted(
              "RUNNING_VERSION",
              YafForumInfo.AppVersionName,
              this.Get<IDateTime>().FormatDateShort(YafForumInfo.AppVersionDate));

           this.LatestVersion.Text = this.GetTextFormatted(
             "LATEST_VERSION",
             this.LastVersion, 
             this.LastVersionDate);
      }

      this.DataBind();
    }

    #endregion
  }
}