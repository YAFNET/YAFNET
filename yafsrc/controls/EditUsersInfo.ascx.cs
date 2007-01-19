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

namespace YAF.Controls
{
    public partial class EditUsersInfo : YAF.Classes.Base.BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IsHostAdminRow.Visible = PageContext.IsHostAdmin;

            if (!IsPostBack)
            {
                BindData();
                using (DataTable dt = YAF.Classes.Data.DB.user_list(PageContext.PageBoardID, Request.QueryString["u"], null))
                {
                    DataRow row = dt.Rows[0];
                    Name.Text = (string)row["Name"];
                    Email.Text = row["Email"].ToString();
										IsHostAdminX.Checked = ( ( int ) row ["Flags"] & ( int ) YAF.Classes.Data.UserFlags.IsHostAdmin ) == ( int ) YAF.Classes.Data.UserFlags.IsHostAdmin;
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
            RankID.DataSource = YAF.Classes.Data.DB.rank_list(PageContext.PageBoardID, null);
            RankID.DataValueField = "RankID";
            RankID.DataTextField = "Name";
            DataBind();
        }

        protected void Cancel_Click(object sender, System.EventArgs e)
        {
            YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users);
        }

        protected void Save_Click(object sender, System.EventArgs e)
        {
            YAF.Classes.Data.DB.user_adminsave(PageContext.PageBoardID, Request.QueryString["u"], Name.Text, Email.Text, IsHostAdminX.Checked, RankID.SelectedValue);
            YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_users);
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