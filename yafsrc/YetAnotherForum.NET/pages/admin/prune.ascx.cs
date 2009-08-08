/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for prune.
	/// </summary>
	public partial class prune : YAF.Classes.Core.AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				PageLinks.AddLink(PageContext.BoardSettings.Name,YafBuildLink.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",YafBuildLink.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("Prune","");

				days.Text = "60";
				BindData();
			}
		}

		private void BindData() {
			forumlist.DataSource = YAF.Classes.Data.DB.forum_listread(PageContext.PageBoardID,PageContext.PageUserID,null,null);
			forumlist.DataValueField = "ForumID";
			forumlist.DataTextField = "Forum";
			DataBind();
			forumlist.Items.Insert(0,new ListItem("All Forums","0"));
		}

		private void commit_Click(object sender,EventArgs e) {
            int Count = YAF.Classes.Data.DB.topic_prune(PageContext.PageBoardID, forumlist.SelectedValue, days.Text, permDeleteChkBox.Checked);
			PageContext.AddLoadMessage(String.Format("{0} topic(s) deleted.",Count));
		}


		protected void PruneButton_Load(object sender, System.EventArgs e)
		{
			((Button)sender).Attributes["onclick"] = String.Format("return confirm('{0}')", "Do you really want to prune topics? This process is irreversible.");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			commit.Click += new EventHandler(commit_Click);
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
