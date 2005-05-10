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
	/// Summary description for active.
	/// </summary>
	public class active : ForumPage
	{
		protected System.Web.UI.WebControls.Repeater TopicList;
		protected System.Web.UI.WebControls.DropDownList ForumJump;
		protected System.Web.UI.WebControls.DropDownList Since;
		protected System.Web.UI.WebControls.HyperLink RssFeed;
		protected controls.PageLinks PageLinks;
		protected controls.Pager Pager;
		protected string LastForumName = "";

		public active() : base("ACTIVE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
 			// RssFeed.NavigateUrl = String.Format("{0}default.aspx?g=rsstopic&pg=active", Data.ForumRoot);
			if (BoardSettings.ShowRSSLink)
			{
				RssFeed.NavigateUrl = Forum.GetLink(Pages.rsstopic, "pg=active");
				RssFeed.Text = GetText("RSSFEED");
				RssFeed.Visible = true;
			}
			else
			{
				RssFeed.Visible = false;
			}

			if(!IsPostBack)
			{
				PageLinks.AddLink(BoardSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(GetText("TITLE"),Utils.GetSafeRawUrl());

				Since.Items.Add(new ListItem(String.Format(GetText("last_visit"),FormatDateTime(Mession.LastVisit)),"0"));
				Since.Items.Add(new ListItem(GetText("last_hour"),"-1"));
				Since.Items.Add(new ListItem(GetText("last_two_hours"),"-2"));
				Since.Items.Add(new ListItem(GetText("last_day"),"1"));
				Since.Items.Add(new ListItem(GetText("last_two_days"),"2"));
				Since.Items.Add(new ListItem(GetText("last_week"),"7"));
				Since.Items.Add(new ListItem(GetText("last_two_weeks"),"14"));
				Since.Items.Add(new ListItem(GetText("last_month"),"31"));
			}
			BindData();
		}

		private void Pager_PageChange(object sender,EventArgs e)
		{
			BindData();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			Pager.PageChange += new EventHandler(Pager_PageChange);
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
			this.Since.SelectedIndexChanged += new System.EventHandler(this.Since_SelectedIndexChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void BindData() {
			DateTime SinceDate = DateTime.Now;
			int SinceValue = 0;

			if(Since.SelectedItem != null) {
				SinceValue = int.Parse(Since.SelectedItem.Value);
				SinceDate = DateTime.Now;
				if(SinceValue>0)
					SinceDate = DateTime.Now - TimeSpan.FromDays(SinceValue);
				else if(SinceValue<0)
					SinceDate = DateTime.Now + TimeSpan.FromHours(SinceValue);
			}
			if(SinceValue==0)
				SinceDate = Mession.LastVisit;


			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;

			DataView dv = DB.topic_active(PageBoardID,PageUserID,SinceDate,ForumControl.CategoryID).DefaultView;
			pds.DataSource = dv;
			Pager.Count = dv.Count;
			Pager.PageSize = 15;
			pds.PageSize = Pager.PageSize;

			pds.CurrentPageIndex = Pager.CurrentPageIndex;
			TopicList.DataSource = pds;

			DataBind();
		}

		protected string PrintForumName(DataRowView row) {
			string ForumName = (string)row["ForumName"];
			string html = "";
			if(ForumName!=LastForumName) {
				html = String.Format("<tr><td class=header2 colspan=6><a href=\"{1}\">{0}</a></td></tr>",ForumName,Forum.GetLink(Pages.topics,"f={0}",row["ForumID"]));
				LastForumName = ForumName;
			}
			return html;
		}

		private void Since_SelectedIndexChanged(object sender, System.EventArgs e) {
			BindData();
		}
	}
}
