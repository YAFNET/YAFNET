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

namespace yaf.cp
{
	/// <summary>
	/// Summary description for inbox.
	/// </summary>
	public class cp_message : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink UserLink, InboxLink, ThisLink;
		protected System.Web.UI.WebControls.Repeater Inbox;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));
			
			if(!IsPostBack) {
				BindData();

				HomeLink.NavigateUrl = BaseDir;
				HomeLink.Text = ForumName;
				UserLink.NavigateUrl = "cp_profile.aspx";
				UserLink.Text = PageUserName;
				ThisLink.NavigateUrl = Request.RawUrl;
			}
		}

		private void BindData() {
			using(DataTable dt = DB.pmessage_list(PageUserID,false,Request.QueryString["m"])) 
			{
				foreach(DataRow row in dt.Rows) 
				{
					if((int)row["ToUserID"]!=PageUserID && (int)row["FromUserID"]!=PageUserID)
						Data.AccessDenied();

					if((int)row["ToUserID"]==PageUserID) 
					{
						InboxLink.NavigateUrl = "cp_inbox.aspx";
						InboxLink.Text = GetText("inbox");
					} 
					else 
					{
						InboxLink.NavigateUrl = "cp_inbox.aspx?sent=1";
						InboxLink.Text = GetText("sentitems");
					}
					ThisLink.Text = row["Subject"].ToString();
				}
				Inbox.DataSource = dt;
			}
			DataBind();
			DB.pmessage_markread(PageUserID,Request.QueryString["m"]);
		}

		protected string FormatBody(object o) {
			DataRowView row = (DataRowView)o;
			string body = row["Body"].ToString();
			if(body.IndexOf('<')<0) 
			{
				FormatMsg fmt = new FormatMsg(this);
				body = fmt.FormatMessage(body);
			}
			return body;
		}

		private void Inbox_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			if(e.CommandName == "delete") {
				DB.pmessage_delete(e.CommandArgument);
				BindData();
				AddLoadMessage(GetText("msg_deleted"));
			} else if(e.CommandName == "reply") {
				Response.Redirect(String.Format("pmessage.aspx?p={0}",e.CommandArgument));
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
			this.Inbox.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.Inbox_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
