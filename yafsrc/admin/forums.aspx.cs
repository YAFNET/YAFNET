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

namespace yaf.admin
{
	/// <summary>
	/// Summary description for forums.
	/// </summary>
	public class forums : AdminPage
	{
		protected System.Web.UI.WebControls.LinkButton NewForum;
		protected System.Web.UI.WebControls.LinkButton NewCategory;
		protected System.Web.UI.WebControls.Repeater CategoryList;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
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
			using(DataSet ds = DB.ds_forumadmin(PageBoardID))
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
			this.NewCategory.Click += new System.EventHandler(this.NewCategory_Click);
			this.NewForum.Click += new System.EventHandler(this.NewForum_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		protected void ForumList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					Response.Redirect(String.Format("editforum.aspx?f={0}",e.CommandArgument));
					break;
				case "delete":
					DB.forum_delete(e.CommandArgument);
					BindData();
					break;
			}
		}

		private void NewForum_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("editforum.aspx");
		}

		private void CategoryList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
			switch(e.CommandName) 
			{
				case "edit":
					Response.Redirect(String.Format("editcategory.aspx?c={0}",e.CommandArgument));
					break;
				case "delete":
					if(DB.category_delete(e.CommandArgument))
						BindData();
					else
						AddLoadMessage("You cannot delete this Category as it has at least one forum assigned to it.\nTo move forums click on \"Edit\" and change the category the forum is assigned to.");
					break;
			}
		}

		private void NewCategory_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("editcategory.aspx");
		}
	}
}
