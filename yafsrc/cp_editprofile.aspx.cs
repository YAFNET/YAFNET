/* Copyright (C) 2003 Bjørnar Henden
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
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;
using System.Globalization;

namespace yaf
{
	/// <summary>
	/// Summary description for cp_editprofile.
	/// </summary>
	public class cp_editprofile : BasePage
	{
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.HyperLink UserLink;
		protected System.Web.UI.WebControls.TextBox Location;
		protected System.Web.UI.WebControls.TextBox HomePage;
		protected System.Web.UI.WebControls.DropDownList TimeZones;
		protected System.Web.UI.WebControls.TextBox Avatar;
		protected System.Web.UI.WebControls.TextBox OldPassword;
		protected System.Web.UI.WebControls.TextBox NewPassword1;
		protected System.Web.UI.WebControls.TextBox NewPassword2;
		protected System.Web.UI.WebControls.Button UpdateProfile;
		protected System.Web.UI.WebControls.TextBox Email;
		private bool bUpdateEmail = false;

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
			DataRow row;
			TimeZones.DataSource = Data.TimeZones();
			DataBind();

			using(SqlCommand cmd = new SqlCommand("yaf_user_list")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",pageinfo["UserID"]);
				using(DataTable dt = DataManager.GetData(cmd)) {
					row = dt.Rows[0];
				}
			}

			Location.Text = row["Location"].ToString();
			HomePage.Text = row["HomePage"].ToString();
			TimeZones.Items.FindByValue(row["TimeZone"].ToString()).Selected = true;
			Avatar.Text = row["Avatar"].ToString();
			Email.Text = row["Email"].ToString();
		}

		private void UpdateProfile_Click(object sender, System.EventArgs e) {
			if(bUpdateEmail && (bool)pageinfo["EmailVerification"]) {
				string hashinput = DateTime.Now.ToString() + Email.Text + register.CreatePassword(20);
				string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput,"md5");

				// Email Body
				System.Text.StringBuilder msg = new System.Text.StringBuilder();
				msg.AppendFormat("Hello {0}.\r\n\r\n",User.Identity.Name);
				msg.AppendFormat("You have requested to change your email address to {0}\r\n\r\n",Email.Text);
				msg.AppendFormat("To change your address you will have to click on the following link:\r\n");
				msg.AppendFormat("http://{2}{1}approve.aspx?k={0}\r\n\r\n",hash,BaseDir,Request.ServerVariables["SERVER_NAME"]);
				msg.AppendFormat("Your approval key is: {0}\r\n\r\n",hash);
				msg.AppendFormat("Visit {0} at http://{1}{2}",ForumName,Request.ServerVariables["SERVER_NAME"],BaseDir);

				using(SqlCommand cmd = new SqlCommand("yaf_checkemail_save")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@UserID",PageUserID);
					cmd.Parameters.Add("@Hash",hash);
					cmd.Parameters.Add("@Email",Email.Text);
					DataManager.ExecuteNonQuery(cmd);
		
					//  Build a MailMessage
					System.Web.Mail.MailMessage mailMessage = new System.Web.Mail.MailMessage();
					mailMessage.From = ForumEmail;
					mailMessage.To = Email.Text;
					mailMessage.Subject = "Changed email";
					mailMessage.BodyFormat = System.Web.Mail.MailFormat.Text;
					mailMessage.Body = msg.ToString();
					System.Web.Mail.SmtpMail.SmtpServer = SmtpServer;
					System.Web.Mail.SmtpMail.Send(mailMessage);
				
					AddLoadMessage(String.Format("A mail has been sent to {0}.\n\nYou will need to verify your new email address by\nopening the link in the email before your email will be modified.",Email.Text));
				}
			}

			if(OldPassword.Text.Length > 0) {
				if(NewPassword1.Text.Length==0 || NewPassword2.Text.Length==0) {
					AddLoadMessage("Password can't be empty.");
					return;
				}
				if(NewPassword1.Text != NewPassword2.Text) {
					AddLoadMessage("New passwords doesn't match.");
					return;
				}

				string oldpw = FormsAuthentication.HashPasswordForStoringInConfigFile(OldPassword.Text,"md5");
				string newpw = FormsAuthentication.HashPasswordForStoringInConfigFile(NewPassword1.Text,"md5");

				using(SqlCommand cmd = new SqlCommand("yaf_user_changepassword")) {
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Add("@UserID",PageUserID);
					cmd.Parameters.Add("@OldPassword",oldpw);
					cmd.Parameters.Add("@NewPassword",newpw);
					bool Success = (bool)DataManager.ExecuteScalar(cmd);
					if(!Success) {
						AddLoadMessage("Old password was wrong.");
						return;
					}
				}
			}

			using(SqlCommand cmd = new SqlCommand("yaf_user_save")) {
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.Add("@UserID",pageinfo["UserID"]);
				cmd.Parameters.Add("@Location",Location.Text);
				cmd.Parameters.Add("@HomePage",HomePage.Text);
				cmd.Parameters.Add("@TimeZone",int.Parse(TimeZones.SelectedItem.Value));
				cmd.Parameters.Add("@Avatar",Avatar.Text);
				if(!(bool)pageinfo["EmailVerification"])
					cmd.Parameters.Add("@Email",Email.Text);
				DataManager.ExecuteNonQuery(cmd);
			}
			Response.Redirect("cp_profile.aspx");
		}

		private void Email_TextChanged(object sender, System.EventArgs e) {
			bUpdateEmail = true;
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
			this.UpdateProfile.Click += new System.EventHandler(this.UpdateProfile_Click);
			this.Email.TextChanged += new System.EventHandler(this.Email_TextChanged);
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion
	}
}
