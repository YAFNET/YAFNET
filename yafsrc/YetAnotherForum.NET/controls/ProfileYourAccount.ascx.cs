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
namespace YAF.Controls
{
  #region Using

  using System;
  using System.Data;

  using Classes;

  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

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
    protected void Page_Load(object sender, EventArgs e)
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
      DataTable dt = DB.usergroup_list(this.PageContext.PageUserID);

      if (YafContext.Current.BoardSettings.UseStyledNicks)
      {
        new StyleTransform(YafContext.Current.Theme).DecodeStyleByTable(ref dt, false);
      }

      this.Groups.DataSource = dt;

      // Bind			
      this.DataBind();

      // TitleUserName.Text = HtmlEncode( userData.Membership.UserName );
      this.AccountEmail.Text = this.PageContext.CurrentUserData.Membership.Email;
      this.Name.Text = this.HtmlEncode(this.PageContext.CurrentUserData.Membership.UserName);
      this.Joined.Text = YafServices.DateTime.FormatDateTime(this.PageContext.CurrentUserData.Joined);
      this.NumPosts.Text = String.Format("{0:N0}", this.PageContext.CurrentUserData.NumPosts);

      this.DisplayNameHolder.Visible = PageContext.BoardSettings.EnableDisplayName;

      if (PageContext.BoardSettings.EnableDisplayName)
      {
        this.DisplayName.Text = PageContext.UserDisplayName.GetName(this.PageContext.PageUserID);
      }

      string avatarImg = YafServices.Avatar.GetAvatarUrlForCurrentUser();

      if (!String.IsNullOrEmpty(avatarImg))
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