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
	public class cp_message : ForumPage
	{
		protected System.Web.UI.WebControls.Repeater Inbox;
		protected controls.PageLinks PageLinks;

		public cp_message() : base("CP_MESSAGE")
		{
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.User.Identity.IsAuthenticated)
				Forum.Redirect(Pages.login,"ReturnUrl={0}",Request.RawUrl);
			
			if(!IsPostBack) {
				BindData();
			}
		}

		private void BindData() {
			using(DataTable dt = DB.pmessage_list(PageUserID,false,Request.QueryString["m"])) 
			{
				foreach(DataRow row in dt.Rows) 
				{
					if((int)row["ToUserID"]!=PageUserID && (int)row["FromUserID"]!=PageUserID)
						Data.AccessDenied();

					PageLinks.AddLink(ForumName,Forum.GetLink(Pages.forum));
					PageLinks.AddLink(PageUserName,Forum.GetLink(Pages.cp_profile));
					if((int)row["ToUserID"]==PageUserID) 
						PageLinks.AddLink(GetText("INBOX"),Forum.GetLink(Pages.cp_inbox));
					else 
						PageLinks.AddLink(GetText("SENTITEMS"),Forum.GetLink(Pages.cp_inbox,"sent=1"));
					PageLinks.AddLink(row["Subject"].ToString(),Request.RawUrl);
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
				body = FormatMsg.ForumCodeToHtml(this,body);
			}

			return FormatMsg.FetchURL(this,body);
		}

		private void Inbox_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e) {
			if(e.CommandName == "delete") {
				DB.pmessage_delete(e.CommandArgument);
				BindData();
				AddLoadMessage(GetText("msg_deleted"));
			} else if(e.CommandName == "reply") {
				Forum.Redirect(Pages.pmessage,"p={0}",e.CommandArgument);
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
