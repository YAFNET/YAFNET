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
	/// Summary description for inbox.
	/// </summary>
	public partial class cp_inbox : ForumPage
	{
		public cp_inbox() : base("CP_INBOX")
		{
		}

		private void SetSort(string field,bool asc) 
		{
			if(ViewState["SortField"]!=null && (string)ViewState["SortField"]==field) 
			{
				ViewState["SortAsc"] = !(bool)ViewState["SortAsc"];
			} 
			else 
			{
				ViewState["SortField"] = field;
				ViewState["SortAsc"] = asc;
			}
		}

		private void SubjectLink_Click(object sender, System.EventArgs e) 
		{
			SetSort("Subject",true);
			BindData();
		}

		private void FromLink_Click(object sender, System.EventArgs e) 
		{
			if(IsSentItems)
				SetSort("ToUser",true);
			else
				SetSort("FromUser",true);
			BindData();
		}

		private void DateLink_Click(object sender, System.EventArgs e) 
		{
			SetSort("Created",false);
			BindData();
		}

		protected void DeleteSelected_Load(object sender, System.EventArgs e) 
		{
			((Button)sender).Attributes["onclick"] = String.Format("return confirm('{0}')",GetText("confirm_delete"));
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.IsAuthenticated)
			{
				if(User.CanLogin)
					Forum.Redirect(Pages.login,"ReturnUrl={0}",Utils.GetSafeRawUrl());
				else
					Forum.Redirect(Pages.forum);
			}
			
			if(!IsPostBack) 
			{
				SetSort("Created",false);
				IsSentItems = Request.QueryString["sent"]!=null;
				BindData();

				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(PageUserName,Forum.GetLink(Pages.cp_profile));
				PageLinks.AddLink(GetText(IsSentItems ? "sentitems" : "title"),Utils.GetSafeRawUrl());

				SubjectLink.Text = GetText("subject");
				FromLink.Text = GetText(IsSentItems ? "to" : "from");
				DateLink.Text = GetText("date");
			}
		}

		protected bool IsSentItems 
		{
			get 
			{
				return (bool)ViewState["IsSentItems"];
			}
			set 
			{
				ViewState["IsSentItems"] = value;
			}
		}

		private void BindData() {
			object toUserID = null;
			object fromUserID = null;
			if(IsSentItems)
				fromUserID = PageUserID;
			else
				toUserID = PageUserID;
			using(DataView dv = DB.pmessage_list(toUserID,fromUserID,null).DefaultView) 
			{
				dv.Sort = String.Format("{0} {1}",ViewState["SortField"],(bool)ViewState["SortAsc"] ? "asc" : "desc");
				Inbox.DataSource = dv;
				DataBind();
			}
			if(IsSentItems)
				SortFrom.Visible = (string)ViewState["SortField"] == "ToUser";
			else
				SortFrom.Visible = (string)ViewState["SortField"] == "FromUser";
			SortFrom.Src = GetThemeContents("SORT",(bool)ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
			SortSubject.Visible = (string)ViewState["SortField"] == "Subject";
			SortSubject.Src = GetThemeContents("SORT",(bool)ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
			SortDate.Visible = (string)ViewState["SortField"] == "Created";
			SortDate.Src = GetThemeContents("SORT",(bool)ViewState["SortAsc"] ? "ASCENDING" : "DESCENDING");
		}

		protected string FormatBody(object o) {
			DataRowView row = (DataRowView)o;
			return FormatMsg.ForumCodeToHtml(this,(string)row["Body"]);
		}

		private void Inbox_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			if(e.CommandName == "delete") {
				long nItemCount = 0;
				foreach(RepeaterItem item in Inbox.Items) 
				{
					if(((CheckBox)item.FindControl("ItemCheck")).Checked) 
					{
						DB.userpmessage_delete(((Label)item.FindControl("UserPMessageID")).Text);
						nItemCount++;
					}
				}

				//TODO DB.pmessage_delete(e.CommandArgument);
				BindData();
				if(nItemCount==1)
					AddLoadMessage(GetText("msgdeleted1"));
				else
					AddLoadMessage(String.Format(GetText("msgdeleted2"),nItemCount));
			}
		}

		protected string GetImage(object o) 
		{
			if((bool)((DataRowView)o)["IsRead"]) 
				return GetThemeContents("ICONS","TOPIC");
			else
				return GetThemeContents("ICONS","TOPIC_NEW");
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			SubjectLink.Click += new EventHandler(SubjectLink_Click);
			FromLink.Click += new EventHandler(FromLink_Click);
			DateLink.Click += new EventHandler(DateLink_Click);
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
			this.Inbox.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.Inbox_ItemCommand);
		}
		#endregion
	}
}
