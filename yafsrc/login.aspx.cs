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
using System.Web.Security;

namespace yaf
{
	/// <summary>
	/// Summary description for login.
	/// </summary>
	public class login : BasePage
	{
		protected System.Web.UI.WebControls.TextBox UserName;
		protected System.Web.UI.WebControls.TextBox Password;
		protected System.Web.UI.WebControls.CheckBox AutoLogin;
		protected System.Web.UI.WebControls.Button ForumLogin;
		protected System.Web.UI.WebControls.Label ErrorMsg;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.HtmlControls.HtmlTable LoginView;
		protected System.Web.UI.HtmlControls.HtmlTable RecoverView;
		protected System.Web.UI.WebControls.LinkButton LostPassword;
		protected System.Web.UI.WebControls.TextBox LostUserName;
		protected System.Web.UI.WebControls.TextBox LostEmail;
		protected System.Web.UI.WebControls.Button Recover;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			HomeLink.Text = ForumName;
			HomeLink.NavigateUrl = BaseDir;
			
			LostPassword.Click += new System.EventHandler(LostPassword_Click);
			Recover.Click += new System.EventHandler(Recover_Click);
		}

		private void LostPassword_Click(object sender,EventArgs e) {
			LoginView.Visible = false;
			RecoverView.Visible = true;
		}
		private void Recover_Click(object sender,EventArgs e) {
			if(LostEmail.Text.Length==0 || LostUserName.Text.Length==0) {
				AddLoadMessage("Both user name and email address must be entered.");
				return;
			}

			string newpw = register.CreatePassword(8);

			if(!DB.user_recoverpassword(LostUserName.Text,LostEmail.Text,FormsAuthentication.HashPasswordForStoringInConfigFile(newpw,"md5"))) {
				AddLoadMessage("Wrong user name or email address.");
				return;
			}

			// Email Body
			System.Text.StringBuilder msg = new System.Text.StringBuilder();
			msg.AppendFormat("Hello {0}.\r\n\r\n",LostUserName.Text);
			msg.AppendFormat("Here is your new password: {0}\r\n\r\n",newpw);
			msg.AppendFormat("Visit {0} at {1}",ForumName,ForumURL);
			
			SendMail(ForumEmail,LostEmail.Text,"New password",msg.ToString());

			AddLoadMessage("A mail has been sent with the your new password.");
			LoginView.Visible = true;
			RecoverView.Visible = false;
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
			this.ForumLogin.Click += new System.EventHandler(this.ForumLogin_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void ForumLogin_Click(object sender, System.EventArgs e)
		{
			string sPassword = FormsAuthentication.HashPasswordForStoringInConfigFile(Password.Text,"md5");
			if(DB.user_login(UserName.Text,sPassword)) {
				//FormsAuthentication.RedirectFromLoginPage(UserName.Text, AutoLogin.Checked);
				FormsAuthentication.SetAuthCookie(UserName.Text, AutoLogin.Checked);
				Response.Redirect(BaseDir);
			} else {
				ErrorMsg.Visible = true;
				ErrorMsg.Text = "The user name or password entered is incorrect. Please try again.";
			}
		}
	}
}
