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
  using System.Web.Security;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.EventProxies;
  using YAF.Types.Extensions;
  using YAF.Types.Flags;
  using YAF.Types.Interfaces;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// The edit users info.
  /// </summary>
  public partial class EditUsersInfo : BaseUserControl
  {
    #region Properties

    /// <summary>
    ///   Gets user ID of edited user.
    /// </summary>
    protected int CurrentUserID
    {
      get
      {
        return this.PageContext.QueryIDs["u"].ToType<int>();
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
        this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

        this.IsHostAdminRow.Visible = this.PageContext.IsHostAdmin;

        if (this.IsPostBack)
        {
            return;
        }

        this.Save.Text = this.Get<ILocalization>().GetText("COMMON", "SAVE");

        this.BindData();
    }

      /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      // Update the Membership
      if (!this.IsGuestX.Checked)
      {
         MembershipUser user = UserMembershipHelper.GetUser(this.Name.Text.Trim());
        
         string userName = this.Get<MembershipProvider>().GetUserNameByEmail(this.Email.Text.Trim());
         if (userName != user.UserName)
         {
             this.PageContext.AddLoadMessage(this.GetText("PROFILE", "BAD_EMAIL"));
             return;
         }
        if (this.Email.Text.Trim() != user.Email )
        {
          // update the e-mail here too...
          user.Email = this.Email.Text.Trim();
        }

        // Update IsApproved
        user.IsApproved = this.IsApproved.Checked;
       
        this.Get<MembershipProvider>().UpdateUser(user);
      }
      else
      {
        if (!this.IsApproved.Checked)
        {
          this.PageContext.AddLoadMessage(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_GUEST_APPROVED"));
          return;
        }
      }

      var userFlags = new UserFlags
        {
          IsHostAdmin = this.IsHostAdminX.Checked, 
          IsGuest = this.IsGuestX.Checked, 
          IsCaptchaExcluded = this.IsCaptchaExcluded.Checked, 
          IsActiveExcluded = this.IsExcludedFromActiveUsers.Checked, 
          IsApproved = this.IsApproved.Checked
        };

      LegacyDb.user_adminsave(
        this.PageContext.PageBoardID, 
        this.CurrentUserID, 
        this.Name.Text.Trim(), 
        this.DisplayName.Text.Trim(), 
        this.Email.Text.Trim(), 
        userFlags.BitValue, 
        this.RankID.SelectedValue);

      this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.RankID.DataSource = LegacyDb.rank_list(this.PageContext.PageBoardID, null);
      this.RankID.DataValueField = "RankID";
      this.RankID.DataTextField = "Name";
      this.RankID.DataBind();

      using (DataTable dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserID, null))
      {
        DataRow row = dt.Rows[0];
        var userFlags = new UserFlags(row["Flags"]);

        this.Name.Text = (string)row["Name"];
        this.DisplayName.Text = row.Field<string>("DisplayName");
        this.Email.Text = row["Email"].ToString();
        this.IsHostAdminX.Checked = userFlags.IsHostAdmin;
        this.IsApproved.Checked = userFlags.IsApproved;
        this.IsGuestX.Checked = userFlags.IsGuest;
        this.IsCaptchaExcluded.Checked = userFlags.IsCaptchaExcluded;
        this.IsExcludedFromActiveUsers.Checked = userFlags.IsActiveExcluded;
        this.Joined.Text = row["Joined"].ToString();
        this.IsFacebookUser.Checked = row["IsFacebookUser"].ToType<bool>();
        this.IsTwitterUser.Checked = row["IsTwitterUser"].ToType<bool>();
        this.LastVisit.Text = row["LastVisit"].ToString();
        ListItem item = this.RankID.Items.FindByValue(row["RankID"].ToString());

        if (item != null)
        {
          item.Selected = true;
        }
      }
    }

    #endregion
  }
}