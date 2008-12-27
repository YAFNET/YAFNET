/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Controls
{
    public partial class EditUsersInfo : YAF.Classes.Base.BaseUserControl
    {
        /// <summary>
        /// Gets user ID of edited user.
        /// </summary>
        protected int CurrentUserID
        {
            get
            {
                return (int)this.PageContext.QueryIDs["u"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            IsHostAdminRow.Visible = PageContext.IsHostAdmin;
            if (!IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            RankID.DataSource = YAF.Classes.Data.DB.rank_list(PageContext.PageBoardID, null);
            RankID.DataValueField = "RankID";
            RankID.DataTextField = "Name";
            RankID.DataBind();

            using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, CurrentUserID, null))
            {
                DataRow row = dt.Rows[0];
                UserFlags userFlags = new UserFlags(row["Flags"]);

                Name.Text = (string)row["Name"];
                Email.Text = row["Email"].ToString();
                IsHostAdminX.Checked = userFlags.IsHostAdmin;
                IsApproved.Checked = userFlags.IsApproved;
                IsGuestX.Checked = userFlags.IsGuest;
                IsCaptchaExcluded.Checked = userFlags.IsCaptchaExcluded;
                IsExcludedFromActiveUsers.Checked = userFlags.IsActiveExcluded;
                Joined.Text = row["Joined"].ToString();
                LastVisit.Text = row["LastVisit"].ToString();
                ListItem item = RankID.Items.FindByValue(row["RankID"].ToString());

                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        protected void Save_Click(object sender, System.EventArgs e)
        {
            // update the membership too...
            if (!IsGuestX.Checked)
            {
                MembershipUser user = Membership.GetUser(Name.Text);

                if (Email.Text.Trim() != user.Email)
                {
                    user.IsApproved = IsApproved.Checked;
                    // update the e-mail here too...
                    user.Email = Email.Text.Trim();
                    System.Web.Security.Membership.UpdateUser(user);
                }
            }

            UserFlags userFlags = new UserFlags();
            userFlags.IsHostAdmin = IsHostAdminX.Checked;
            userFlags.IsGuest = IsGuestX.Checked;
            userFlags.IsCaptchaExcluded = IsCaptchaExcluded.Checked;
            userFlags.IsActiveExcluded = IsExcludedFromActiveUsers.Checked;
            userFlags.IsApproved = IsApproved.Checked;
            YAF.Classes.Data.DB.user_adminsave(PageContext.PageBoardID, CurrentUserID, Name.Text, Email.Text, userFlags.BitValue, RankID.SelectedValue);
            BindData();
        }
    }
}