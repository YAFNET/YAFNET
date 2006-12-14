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

namespace YAF.Pages.Admin
{
	/// <summary>
	/// Summary description for editgroup.
	/// </summary>
	public partial class editgroup : AdminPage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) 
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink( ForumPages.forum));
				PageLinks.AddLink("Administration",Forum.GetLink( ForumPages.admin_admin));
				PageLinks.AddLink("Groups","");
				
				BindData();
				if(Request.QueryString["i"] != null) 
				{
					NewGroupRow.Visible = false;
					using(DataTable dt = YAF.Classes.Data.DB.group_list(PageBoardID,Request.QueryString["i"])) 
					{
						DataRow row = dt.Rows[0];
						Name.Text = (string)row["Name"];
						IsAdminX.Checked = ((int)row["Flags"] & (int)YAF.Classes.Data.GroupFlags.IsAdmin)==(int)YAF.Classes.Data.GroupFlags.IsAdmin;
						IsStart.Checked = ((int)row["Flags"] & (int)YAF.Classes.Data.GroupFlags.IsStart)==(int)YAF.Classes.Data.GroupFlags.IsStart;
						IsModeratorX.Checked = ((int)row["Flags"] & (int)YAF.Classes.Data.GroupFlags.IsModerator)==(int)YAF.Classes.Data.GroupFlags.IsModerator;
					}
				} 
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

		private void BindData() {
			using(DataTable dt = new DataTable("Files")) 
			{
				dt.Columns.Add("FileID",typeof(long));
				dt.Columns.Add("FileName",typeof(string));
				DataRow dr = dt.NewRow();
				dr["FileID"] = 0;
				dr["FileName"] = "Select Rank Image";
				dt.Rows.Add(dr);
				
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(Request.MapPath(String.Format("{0}images/ranks",Data.ForumRoot)));
				System.IO.FileInfo[] files = dir.GetFiles("*.*");
				long nFileID = 1;
				foreach(System.IO.FileInfo file in files) 
				{
					string sExt = file.Extension.ToLower();
					if(sExt!=".png" && sExt!=".gif" && sExt!=".jpg")
						continue;
					
					dr = dt.NewRow();
					dr["FileID"] = nFileID++;
					dr["FileName"] = file.Name;
					dt.Rows.Add(dr);
				}
			}

			if(Request.QueryString["i"] != null) 
				AccessList.DataSource = YAF.Classes.Data.DB.forumaccess_group(Request.QueryString["i"]);

			DataBind();
		}

		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			Forum.Redirect( ForumPages.admin_groups);
		}

		protected void Save_Click(object sender, System.EventArgs e)
		{
			// Group
			long GroupID = 0;
			if(Request.QueryString["i"] != null) GroupID = long.Parse(Request.QueryString["i"]);
				
			GroupID = YAF.Classes.Data.DB.group_save(GroupID,PageBoardID,Name.Text,IsAdminX.Checked,IsStart.Checked,IsModeratorX.Checked,AccessMaskID.SelectedValue);

			// Access
			if(Request.QueryString["i"] != null) 
			{
				for(int i=0;i<AccessList.Items.Count;i++) 
				{
					RepeaterItem item = AccessList.Items[i];
					int ForumID = int.Parse(((Label)item.FindControl("ForumID")).Text);
					YAF.Classes.Data.DB.forumaccess_save(ForumID,GroupID,((DropDownList)item.FindControl("AccessmaskID")).SelectedValue);
				}
				Forum.Redirect( ForumPages.admin_groups);
			}

			// Done
			Forum.Redirect( ForumPages.admin_editgroup,"i={0}",GroupID);
		}

		protected void BindData_AccessMaskID(object sender, System.EventArgs e) 
		{
			((DropDownList)sender).DataSource = YAF.Classes.Data.DB.accessmask_list(PageBoardID,null);
			((DropDownList)sender).DataValueField = "AccessMaskID";
			((DropDownList)sender).DataTextField = "Name";
		}

		protected void SetDropDownIndex(object sender, System.EventArgs e) 
		{
			DropDownList list = (DropDownList)sender;
			list.Items.FindByValue(list.Attributes["value"]).Selected = true;
		}
	}
}
