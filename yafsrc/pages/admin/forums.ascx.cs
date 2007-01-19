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
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for forums.
	/// </summary>
	public partial class forums : YAF.Classes.Base.AdminPage
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
				PageLinks.AddLink("Administration",YAF.Classes.Utils.yaf_BuildLink.GetLink( YAF.Classes.Utils.ForumPages.admin_admin));
				PageLinks.AddLink("Forums","");

				BindData();
			}
		}

		protected void DeleteCategory_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this category?')";
		}

		protected void DeleteForum_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this forum?')";
		}

		private void BindData() 
		{
			using(DataSet ds = YAF.Classes.Data.DB.ds_forumadmin(PageContext.PageBoardID))
				CategoryList.DataSource = ds.Tables["yaf_Category"];
			DataBind();
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
			this.CategoryList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.CategoryList_ItemCommand);

		}
		#endregion

		protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editforum,"f={0}",e.CommandArgument);
					break;
				case "delete":
					YAF.Classes.Data.DB.forum_delete(e.CommandArgument);
					BindData();
					break;
			}
		}

		protected void NewForum_Click(object sender, System.EventArgs e)
		{
			YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editforum);
		}

		private void CategoryList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editcategory,"c={0}",e.CommandArgument);
					break;
				case "delete":
					if(YAF.Classes.Data.DB.category_delete(e.CommandArgument))
						BindData();
					else
						PageContext.AddLoadMessage("You cannot delete this Category as it has at least one forum assigned to it.\nTo move forums click on \"Edit\" and change the category the forum is assigned to.");
					break;
			}
		}

		protected void NewCategory_Click(object sender, System.EventArgs e)
		{
			YAF.Classes.Utils.yaf_BuildLink.Redirect( YAF.Classes.Utils.ForumPages.admin_editcategory);
		}
	}
}
