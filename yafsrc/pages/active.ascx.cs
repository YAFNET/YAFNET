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
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks1;
		protected System.Web.UI.HtmlControls.HtmlTableCell PageLinks2;
		protected System.Web.UI.WebControls.DropDownList Since;
		protected controls.PageLinks PageLinks;
		protected string LastForumName = "";

		public active() : base("ACTIVE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				PageLinks.AddLink(Config.ForumSettings.Name,Forum.GetLink(Pages.forum));
				PageLinks.AddLink(GetText("TITLE"),Request.RawUrl);

				Since.Items.Add(new ListItem(String.Format(GetText("last_visit"),FormatDateTime(DateTime.Parse(Session["lastvisit"].ToString()))),"0"));
				Since.Items.Add(new ListItem(GetText("last_hour"),"-1"));
				Since.Items.Add(new ListItem(GetText("last_two_hours"),"-2"));
				Since.Items.Add(new ListItem(GetText("last_day"),"1"));
				Since.Items.Add(new ListItem(GetText("last_two_days"),"2"));
				Since.Items.Add(new ListItem(GetText("last_week"),"7"));
				Since.Items.Add(new ListItem(GetText("last_two_weeks"),"14"));
				Since.Items.Add(new ListItem(GetText("last_month"),"31"));

				if(Request.QueryString["k"] != null)
					Since.Items.FindByValue(Request.QueryString["k"]).Selected = true;
			}
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
				SinceDate = DateTime.Parse(Session["lastvisit"].ToString());


			PagedDataSource pds = new PagedDataSource();
			pds.AllowPaging = true;
			
			pds.DataSource = DB.topic_active(PageUserID,SinceDate).DefaultView;

			pds.PageSize = 15;
			if(Request.QueryString["p"] != null)
				pds.CurrentPageIndex = int.Parse(Request.QueryString["p"]);
			else
				pds.CurrentPageIndex = 0;

			TopicList.DataSource = pds;

			DataBind();

			if(pds.PageCount>1) {
				PageLinks1.InnerHtml = String.Format("{0} Pages:",pds.PageCount);
				for(int i=0;i<pds.PageCount;i++) {
					if(i==pds.CurrentPageIndex) {
						PageLinks1.InnerHtml += String.Format(" [{0}]",i+1);
					} else {
						PageLinks1.InnerHtml += String.Format(" <a href=\"{1}\">{0}</a>",i+1,Forum.GetLink(Pages.active,"p={0}&k={1}",i,SinceValue));
					}
				}
				PageLinks2.InnerHtml = PageLinks1.InnerHtml;
				PageLinks1.Visible = true;
				PageLinks2.Visible = true;
			} else {
				PageLinks1.Visible = false;
				PageLinks2.Visible = false;
			}
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
