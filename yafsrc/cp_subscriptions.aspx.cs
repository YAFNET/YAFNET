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

namespace yaf
{
	/// <summary>
	/// Summary description for cp_subscriptions.
	/// </summary>
	public class cp_subscriptions : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink UserLink;
		protected System.Web.UI.WebControls.Button UnsubscribeForums;
		protected System.Web.UI.WebControls.Repeater ForumList;
		protected System.Web.UI.WebControls.Button UnsubscribeTopics;
		protected System.Web.UI.WebControls.Repeater TopicList;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));
			
			if(!IsPostBack) {
				BindData();

				HomeLink.NavigateUrl = BaseDir;
				HomeLink.Text = ForumName;
				UserLink.NavigateUrl = "cp_profile.aspx";
				UserLink.Text = User.Identity.Name;
			}
		}

		private void BindData() {
			ForumList.DataSource = DB.watchforum_list(PageUserID);
			TopicList.DataSource = DB.watchtopic_list(PageUserID);
			DataBind();
		}

		protected string FormatLastPosted(object o) {
			DataRowView row = (DataRowView)o;

			if(row["LastPosted"].ToString().Length==0)
				return "&nbsp;";

			string html = String.Format("{0} by <a href=\"profile.aspx?u={1}\">{2}</a> <a href=\"posts.aspx?m={3}#{3}\"><img src=\"{4}\"'></a>",
				FormatDateTime((DateTime)row["LastPosted"]),
				row["LastUserID"],
				row["LastUserName"],
				row["LastMessageID"],
				ThemeFile("icon_latest_reply.gif")
				);
			return html;
		}

		protected string FormatLastPostedForum(object o) {
			DataRowView row = (DataRowView)o;

			if(row["LastPosted"].ToString().Length==0)
				return "&nbsp;";

			string html = String.Format("{0} by <a href=\"profile.aspx?u={1}\">{2}</a> <a href=\"posts.aspx?m={3}#{3}\"><img src=\"{4}\"'></a>",
				FormatDateTime((DateTime)row["LastPosted"]),
				row["LastUserID"],
				row["LastUserName"],
				row["LastMessageID"],
				ThemeFile("icon_latest_reply.gif")
				);
			return html;
		}

		private void UnsubscribeTopics_Click(object sender, System.EventArgs e) {
			bool NoneChecked = true;
			for(int i=0;i<TopicList.Items.Count;i++) {
				CheckBox ctrl = (CheckBox)TopicList.Items[i].FindControl("unsubx");
				Label lbl = (Label)TopicList.Items[i].FindControl("ttid");
				if(ctrl.Checked) {
					DB.watchtopic_delete(lbl.Text);
					NoneChecked = false;
				}
			}
			if(NoneChecked)
				AddLoadMessage("Please select topics to unsubscribe from first.");
			else
				BindData();
		}

		private void UnsubscribeForums_Click(object sender, System.EventArgs e) {
			bool NoneChecked = true;
			for(int i=0;i<ForumList.Items.Count;i++) {
				CheckBox ctrl = (CheckBox)ForumList.Items[i].FindControl("unsubf");
				Label lbl = (Label)ForumList.Items[i].FindControl("tfid");
				if(ctrl.Checked) {
					DB.watchforum_delete(lbl.Text);
					NoneChecked = false;
				}
			}
			if(NoneChecked)
				AddLoadMessage("Please select forums to unsubscribe from first.");
			else
				BindData();
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
			this.UnsubscribeForums.Click += new System.EventHandler(this.UnsubscribeForums_Click);
			this.UnsubscribeTopics.Click += new System.EventHandler(this.UnsubscribeTopics_Click);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
