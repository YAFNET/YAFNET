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
	/// Summary description for pmessage.
	/// </summary>
	public class pmessage : BasePage
	{
		protected System.Web.UI.WebControls.TextBox Subject;
		protected rte.rte Editor;
		protected System.Web.UI.WebControls.TextBox To;
		protected System.Web.UI.HtmlControls.HtmlTableRow ToRow;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.Button Cancel;
		protected System.Web.UI.WebControls.Button Save;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			Editor.EnableRTE = true;

			if(!User.Identity.IsAuthenticated)
				Response.Redirect(String.Format("login.aspx?ReturnUrl={0}",Request.RawUrl));

			if(!IsPostBack) {

				BindData();
				HomeLink.NavigateUrl = BaseDir;
				HomeLink.Text = ForumName;
				Save.Text = GetText("Save");
				Cancel.Text = GetText("Cancel");

				int ToUserID = 0;

				if(Request.QueryString["p"] != null) {
					using(DataTable dt = DB.pmessage_list(null,null,Request.QueryString["p"])) {
						DataRow row = dt.Rows[0];
						Subject.Text = (string)row["Subject"];
						if(Subject.Text.Substring(0,4) != "Re: ")
							Subject.Text = "Re: " + Subject.Text;

						ToUserID = (int)row["FromUserID"];

						Editor.Text = String.Format("[quote={0}]{1}[/quote]",row["FromUser"],row["Body"]);
					}
				} 
				if(Request.QueryString["u"] != null)
					ToUserID = int.Parse(Request.QueryString["u"].ToString());

				if(ToUserID!=0) {
					using(DataTable dt = DB.user_list(ToUserID,true)) 
					{
						To.Text = (string)dt.Rows[0]["Name"];
						To.Enabled = false;
					}
				}
			}
		}

		private void BindData() {
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
			this.Save.Click += new System.EventHandler(this.Save_Click);
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Save_Click(object sender, System.EventArgs e) {
			if(To.Text.Length<=0) {
				AddLoadMessage(GetText("need_to"));
				return;
			}
			if(Subject.Text.Length<=0) {
				AddLoadMessage(GetText("need_subject"));
				return;
			}
			if(Editor.Text.Length<=0) {
				AddLoadMessage(GetText("need_message"));
				return;
			}

			string body = Editor.Text;
			if(!Editor.IsRTEBrowser) 
				body = FormatMsg.ForumCodeToHtml(this,Server.HtmlEncode(body));

			DB.pmessage_save(User.Identity.Name,To.Text,Subject.Text,body);
			Response.Redirect("cp_profile.aspx");
		}

		private void Cancel_Click(object sender, System.EventArgs e) {
			Response.Redirect("cp_profile.aspx");
		}
	}
}
