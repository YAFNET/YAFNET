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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.moderate
{
    /// <summary>
    /// Summary description for _default.
    /// </summary>
    public partial class index : YAF.Classes.Base.ForumPage
    {

        public index()
            : base("MODERATE_DEFAULT")
        {
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!PageContext.IsModerator)
                yaf_BuildLink.AccessDenied();

            if (!IsPostBack)
            {
                PageLinks.AddLink(PageContext.BoardSettings.Name, YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.forum));
                PageLinks.AddLink(GetText("MODERATE_DEFAULT", "TITLE"), "");
                BindData();
            }
        }

        private void BindData()
        {
            using (DataSet ds = YAF.Classes.Data.DB.forum_moderatelist(PageContext.PageUserID, PageContext.PageBoardID))
                CategoryList.DataSource = ds.Tables["yaf_Category"];
            DataBind();
        }

        protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "viewunapprovedposts":
                    YAF.Classes.Utils.yaf_BuildLink.Redirect(YAF.Classes.Utils.ForumPages.moderate_unapprovedposts, "f={0}", e.CommandArgument);
                    break;
                case "viewreportedposts":
                    YAF.Classes.Utils.yaf_BuildLink.Redirect(YAF.Classes.Utils.ForumPages.moderate_reportedposts, "f={0}", e.CommandArgument);
                    break;
                case "viewreportedspam":
                    YAF.Classes.Utils.yaf_BuildLink.Redirect(YAF.Classes.Utils.ForumPages.moderate_reportedspam, "f={0}", e.CommandArgument);
                    break;
            }
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
