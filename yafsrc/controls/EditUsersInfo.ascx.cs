/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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

namespace yaf.controls
{
    public partial class EditUsersInfo : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IsHostAdminRow.Visible = ForumPage.IsHostAdmin;

            if (!IsPostBack)
            {
                BindData();
                using (DataTable dt = DB.user_list(ForumPage.PageBoardID, Request.QueryString["u"], null))
                {
                    DataRow row = dt.Rows[0];
                    Name.Text = (string)row["Name"];
                    Email.Text = row["Email"].ToString();
                    IsHostAdminX.Checked = ((int)row["Flags"] & (int)UserFlags.IsHostAdmin) == (int)UserFlags.IsHostAdmin;
                    Joined.Text = row["Joined"].ToString();
                    LastVisit.Text = row["LastVisit"].ToString();
                    ListItem item = RankID.Items.FindByValue(row["RankID"].ToString());
                    if (item != null)
                        item.Selected = true;
                }
            }
        }

        private void BindData()
        {
            RankID.DataSource = DB.rank_list(ForumPage.PageBoardID, null);
            RankID.DataValueField = "RankID";
            RankID.DataTextField = "Name";
            DataBind();
        }

        protected void Cancel_Click(object sender, System.EventArgs e)
        {
            Forum.Redirect(Pages.admin_users);
        }

        protected void Save_Click(object sender, System.EventArgs e)
        {
            DB.user_adminsave(ForumPage.PageBoardID, Request.QueryString["u"], Name.Text, Email.Text, IsHostAdminX.Checked, RankID.SelectedValue);
            Forum.Redirect(Pages.admin_users);
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}