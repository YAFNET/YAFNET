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

namespace yaf.pages.admin
{
	/// <summary>
	/// Summary description for members.
	/// </summary>
	public class users : AdminPage
	{
		protected System.Web.UI.WebControls.Repeater UserList;
		protected controls.PageLinks PageLinks;
		protected LinkButton NewUser;
		protected DropDownList group, rank;
		protected Button search;
		protected yaf.controls.AdminMenu Adminmenu1;
		protected yaf.controls.SmartScroller SmartScroller1;
		protected TextBox name;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink(Pages.admin_admin));
				PageLinks.AddLink("Users",Forum.GetLink(Pages.admin_users));

				using(DataTable dt=DB.group_list(PageBoardID,null)) 
				{
					DataRow newRow = dt.NewRow();
					newRow["Name"] = string.Empty;
					newRow["GroupID"] = DBNull.Value;
					dt.Rows.InsertAt(newRow,0);

					group.DataSource = dt;
					group.DataTextField = "Name";
					group.DataValueField = "GroupID";
					group.DataBind();
				}

				using(DataTable dt=DB.rank_list(PageBoardID,null)) 
				{
					DataRow newRow = dt.NewRow();
					newRow["Name"] = string.Empty;
					newRow["RankID"] = DBNull.Value;
					dt.Rows.InsertAt(newRow,0);

					rank.DataSource = dt;
					rank.DataTextField = "Name";
					rank.DataValueField = "RankID";
					rank.DataBind();
				}
			}
		}

		override protected void OnInit(EventArgs e)
		{
			this.UserList.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.UserList_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);
			// Added BAI 07.01.2003
			this.NewUser.Click += new System.EventHandler(this.NewUser_Click);
			// END Added BAI 07.01.2003    
			search.Click += new EventHandler(search_Click);
			base.OnInit(e);
		}
	
		protected void Delete_Load(object sender, System.EventArgs e) 
		{
			((LinkButton)sender).Attributes["onclick"] = "return confirm('Delete this user?')";
		}

		private void BindData() 
		{
			using(DataTable dt=
					  DB.user_list(PageBoardID,null,null,
					  group.SelectedIndex<=0 ? null : group.SelectedValue,
					  rank.SelectedIndex<=0 ? null : rank.SelectedValue
					  )) 
			{
				using(DataView dv=dt.DefaultView) 
				{
					if(name.Text.Trim().Length>0)
						dv.RowFilter = string.Format("Name like '%{0}%'",name.Text.Trim());
					UserList.DataSource = dv;
					UserList.DataBind();
				}
			}
		}

		private void UserList_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			switch(e.CommandName) {
				case "edit":
					Forum.Redirect(Pages.admin_edituser,"u={0}",e.CommandArgument);
					break;
				case "delete":
					if(PageUserID==int.Parse(e.CommandArgument.ToString()))
					{
						AddLoadMessage("You can't delete yourself.");
						return;
					}
					if(e.CommandArgument.ToString()=="1")
					{
						AddLoadMessage("You can't delete the Admin.");
						return;
					}
					DB.user_delete(e.CommandArgument);
					BindData();
					break;
			}
		}
		// Added BAI 07.01.2003
		private void NewUser_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect(Pages.admin_reguser);
		}
		// END Added BAI 07.01.2003

		private void search_Click(object sender, EventArgs e)
		{
			BindData();
		}

		private void InitializeComponent()
		{
		
		}

		protected bool BitSet(object _o,int bitmask) 
		{
			int i = (int)_o;
			return (i & bitmask)!=0;
		}
	}
}
