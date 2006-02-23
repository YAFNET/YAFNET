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

namespace yaf.pages
{
	/// <summary>
	/// Summary description for moderate.
	/// </summary>
	public class mod_forumuser : ForumPage
	{
		protected controls.PageLinks PageLinks;
		protected DropDownList AccessMaskID, ToList;
		protected Button FindUsers, Update, Cancel;
		protected TextBox UserName;

		public mod_forumuser() : base("MOD_FORUMUSER")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!ForumModeratorAccess)
				Data.AccessDenied();

			if(!IsPostBack) 
			{
				FindUsers.Text = GetText("FIND");
				Update.Text = GetText("UPDATE");
				Cancel.Text = GetText("CANCEL");

				if(ForumControl.LockedForum==0)
				{
					PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
					PageLinks.AddLink(PageCategoryName,Forum.GetLink(Pages.forum,"c={0}",PageCategoryID));
				}
				PageLinks.AddForumLinks(PageForumID);
				PageLinks.AddLink(GetText("TITLE"),Utils.GetSafeRawUrl());
			
				BindData();
				if(Request.QueryString["u"]!=null) 
				{
					using(DataTable dt = DB.userforum_list(Request.QueryString["u"],PageForumID)) 
					{
						foreach(DataRow row in dt.Rows) 
						{
							UserName.Text = row["Name"].ToString();
							UserName.Enabled = false;
							FindUsers.Visible = false;
							AccessMaskID.Items.FindByValue(row["AccessMaskID"].ToString()).Selected = true;
						}
					}
				}
			}
		}

		private void BindData() 
		{
			AccessMaskID.DataSource = DB.accessmask_list(PageBoardID,null);
			AccessMaskID.DataValueField = "AccessMaskID";
			AccessMaskID.DataTextField = "Name";
			DataBind();
		}

		private void FindUsers_Click(object sender, System.EventArgs e) 
		{
			if(UserName.Text.Length<2) return;

			using(DataTable dt = DB.user_find(PageBoardID,true,UserName.Text,null)) 
			{
				if(dt.Rows.Count>0) 
				{
					ToList.DataSource = dt;
					ToList.DataValueField = "UserID";
					ToList.DataTextField = "Name";
					ToList.SelectedIndex = 0;
					ToList.Visible = true;
					UserName.Visible = false;
					FindUsers.Visible = false;
				} 
				DataBind();
			}
		}

		private void Update_Click(object sender, System.EventArgs e) 
		{
			if(UserName.Text.Length<=0) 
			{
				//TODO AddLoadMessage(GetText("need_to"));
				return;
			}
			if(ToList.Visible)
				UserName.Text = ToList.SelectedItem.Text;

			using(DataTable dt = DB.user_find(PageBoardID,false,UserName.Text,null)) 
			{
				if(dt.Rows.Count!=1) 
				{
					AddLoadMessage(GetText("NO_SUCH_USER"));
					return;
				} 
				else if((int)dt.Rows[0]["IsGuest"]>0) 
				{
					AddLoadMessage(GetText("NOT_GUEST"));
					return;	
				}

				DB.userforum_save(dt.Rows[0]["UserID"],PageForumID,AccessMaskID.SelectedValue);
				Forum.Redirect(Pages.moderate,"f={0}",PageForumID);
			}
		}
		
		private void Cancel_Click(object sender, System.EventArgs e) 
		{
			Forum.Redirect(Pages.moderate,"f={0}",PageForumID);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			FindUsers.Click += new EventHandler(FindUsers_Click);
			Update.Click += new EventHandler(Update_Click);
			Cancel.Click += new EventHandler(Cancel_Click);
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

	}
}
