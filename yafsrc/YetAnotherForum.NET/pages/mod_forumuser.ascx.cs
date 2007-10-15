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

namespace YAF.Pages // YAF.Pages
{
	/// <summary>
	/// Summary description for moderate.
	/// </summary>
	public partial class mod_forumuser : YAF.Classes.Base.ForumPage
	{

		public mod_forumuser() : base("MOD_FORUMUSER")
		{
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!PageContext.ForumModeratorAccess)
				YafBuildLink.AccessDenied();

			if(!IsPostBack) 
			{
				FindUsers.Text = GetText("FIND");
				Update.Text = GetText("UPDATE");
				Cancel.Text = GetText("CANCEL");

				if(PageContext.Settings.LockedForum==0)
				{
					PageLinks.AddLink(PageContext.BoardSettings.Name,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum));
					PageLinks.AddLink(PageContext.PageCategoryName,YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.forum,"c={0}",PageContext.PageCategoryID));
				}
				PageLinks.AddForumLinks(PageContext.PageForumID);
				PageLinks.AddLink(GetText("TITLE"),"");
			
				BindData();
				if(Request.QueryString["u"]!=null) 
				{
					using(DataTable dt = YAF.Classes.Data.DB.userforum_list(Request.QueryString["u"],PageContext.PageForumID)) 
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
			AccessMaskID.DataSource = YAF.Classes.Data.DB.accessmask_list(PageContext.PageBoardID,null);
			AccessMaskID.DataValueField = "AccessMaskID";
			AccessMaskID.DataTextField = "Name";
			DataBind();
		}

		private void FindUsers_Click(object sender, System.EventArgs e) 
		{
			if(UserName.Text.Length<2) return;

			using(DataTable dt = YAF.Classes.Data.DB.user_find(PageContext.PageBoardID,true,UserName.Text,null)) 
			{
				if(dt.Rows.Count>0) 
				{
					ToList.DataSource = dt;
					ToList.DataValueField = "UserID";
					ToList.DataTextField = "Name";
					//ToList.SelectedIndex = 0;
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
				PageContext.AddLoadMessage( GetText( "NO_SUCH_USER" ) );
				return;
			}
			if(ToList.Visible)
				UserName.Text = ToList.SelectedItem.Text;

			using(DataTable dt = YAF.Classes.Data.DB.user_find(PageContext.PageBoardID,false,UserName.Text,null)) 
			{
				if(dt.Rows.Count!=1) 
				{
					PageContext.AddLoadMessage(GetText("NO_SUCH_USER"));
					return;
				} 
				else if((int)dt.Rows[0]["IsGuest"]>0) 
				{
					PageContext.AddLoadMessage(GetText("NOT_GUEST"));
					return;	
				}

				YAF.Classes.Data.DB.userforum_save(dt.Rows[0]["UserID"],PageContext.PageForumID,AccessMaskID.SelectedValue);
				YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.moderate,"f={0}",PageContext.PageForumID);
			}
		}
		
		private void Cancel_Click(object sender, System.EventArgs e) 
		{
			YAF.Classes.Utils.YafBuildLink.Redirect( YAF.Classes.Utils.ForumPages.moderate,"f={0}",PageContext.PageForumID);
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
		}
		#endregion

	}
}
