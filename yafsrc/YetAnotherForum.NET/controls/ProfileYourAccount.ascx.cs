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

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The profile your account.
  /// </summary>
  public partial class ProfileYourAccount : BaseUserControl
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
      if (!this.IsPostBack)
      {
        this.BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      DataTable dt = LegacyDb.usergroup_list(this.PageContext.PageUserID);

      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        this.Get<IStyleTransform>().DecodeStyleByTable(dt, false);
      }

      this.Groups.DataSource = dt;

      // Bind			
      this.DataBind();

      // TitleUserName.Text = HtmlEncode( userData.Membership.UserName );
      this.AccountEmail.Text = this.PageContext.CurrentUserData.Membership.Email;
      this.Name.Text = this.HtmlEncode(this.PageContext.CurrentUserData.Membership.UserName);
      this.Joined.Text = this.Get<IDateTime>().FormatDateTime(this.PageContext.CurrentUserData.Joined);
      this.NumPosts.Text = "{0:N0}".FormatWith(this.PageContext.CurrentUserData.NumPosts);

      this.DisplayNameHolder.Visible = this.PageContext.BoardSettings.EnableDisplayName;

      if (this.PageContext.BoardSettings.EnableDisplayName)
      {
        this.DisplayName.Text =
          this.HtmlEncode(this.Get<IUserDisplayName>().GetName(this.PageContext.PageUserID));
      }

      string avatarImg = this.Get<IAvatars>().GetAvatarUrlForCurrentUser();

      if (avatarImg.IsSet())
      {
        this.AvatarImage.ImageUrl = avatarImg;
      }
      else
      {
        this.AvatarImage.Visible = false;
      }
    }

    #endregion
  }
}