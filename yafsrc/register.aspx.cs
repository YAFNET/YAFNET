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
using System.Data.SqlClient;
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
	/// Summary description for register.
	/// </summary>
	public class register : BasePage
	{
		protected System.Web.UI.WebControls.TextBox UserName;
		protected System.Web.UI.WebControls.TextBox Password;
		protected System.Web.UI.WebControls.TextBox Email;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator1;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator2;
		protected System.Web.UI.WebControls.CompareValidator CompareValidator1;
		protected System.Web.UI.WebControls.Button ForumRegister;
		protected System.Web.UI.WebControls.RequiredFieldValidator RequiredFieldValidator3;
		protected System.Web.UI.WebControls.HyperLink HomeLink;
		protected System.Web.UI.WebControls.TextBox Location;
		protected System.Web.UI.WebControls.TextBox HomePage;
		protected System.Web.UI.WebControls.DropDownList TimeZones;
		protected System.Web.UI.WebControls.TextBox Password2;
		protected Button cancel;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			HomeLink.Text = ForumName;
			HomeLink.NavigateUrl = BaseDir;
			if(!IsPostBack) {
				TimeZones.DataSource = Data.TimeZones();
				DataBind();
				TimeZones.Items.FindByValue("0").Selected = true;
			}
		}

		private void cancel_Click(object sender,EventArgs e) {
			Response.Redirect(BaseDir);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			cancel.Click += new EventHandler(cancel_Click);
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
			this.ForumRegister.Click += new System.EventHandler(this.ForumRegister_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		static public string CreatePassword(int length) {
			string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			string res = "";
			Random rnd = new Random();
			while(0<length--)
				res += valid[rnd.Next(valid.Length)];
			return res;
		}

		private void ForumRegister_Click(object sender, System.EventArgs e)
		{
			bool EmailVerification = (bool)pageinfo["EmailVerification"];
			if(IsValid) 
			{
				string hashinput = DateTime.Now.ToString() + Email.Text + CreatePassword(20);
				string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput,"md5");

				using(SqlConnection conn = DataManager.GetConnection()) 
				{
					using(SqlTransaction trans = conn.BeginTransaction()) 
					{
						try 
						{
							using(SqlCommand cmd = new SqlCommand("yaf_user_find",conn)) 
							{
								cmd.Transaction = trans;
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.Parameters.Add("@UserName",UserName.Text);
								cmd.Parameters.Add("@Email",Email.Text);
								DataTable dtUser = DataManager.GetData(cmd);
								if(dtUser.Rows.Count>0) 
								{
									AddLoadMessage("Your username or email is already registered.");
									return;
								}
							}
							using(SqlCommand cmd = new SqlCommand("yaf_user_save",conn)) 
							{
								cmd.Transaction = trans;
								cmd.Connection = conn;
								cmd.CommandType = CommandType.StoredProcedure;
								int UserID = 0;
								cmd.Parameters.Add("@UserID",UserID);
								cmd.Parameters.Add("@UserName",UserName.Text);
								cmd.Parameters.Add("@Password",FormsAuthentication.HashPasswordForStoringInConfigFile(Password.Text,"md5"));
								cmd.Parameters.Add("@Email",Email.Text);
								cmd.Parameters.Add("@Hash",hash);
								cmd.Parameters.Add("@Location",Location.Text);
								cmd.Parameters.Add("@HomePage",HomePage.Text);
								cmd.Parameters.Add("@TimeZone",TimeZones.SelectedItem.Value);
								cmd.Parameters.Add("@Approved",!EmailVerification);
								cmd.ExecuteNonQuery();
							}

							if(EmailVerification) 
							{
								//  Build a MailMessage
								string body = ReadTemplate("verifyemail.txt");
								body = body.Replace("{link}",String.Format("http://{2}{1}approve.aspx?k={0}",hash,BaseDir,Request.ServerVariables["SERVER_NAME"]));
								body = body.Replace("{key}",hash);
								body = body.Replace("{forumname}",ForumName);
								body = body.Replace("{forumlink}",String.Format("http://{0}{1}",Request.ServerVariables["SERVER_NAME"],BaseDir));

								SendMail(ForumEmail,Email.Text,String.Format("{0} email verification",ForumName),body);
								AddLoadMessage("A mail has been sent. Check your inbox and click the link in the mail.");
								trans.Commit();
							} 
							else 
							{
								trans.Commit();
								FormsAuthentication.RedirectFromLoginPage(UserName.Text, false);
							}
						}
						catch(Exception x) 
						{
							trans.Rollback();
							AddLoadMessage(x.Message);
						}
					}
				}
			}
		}
	}
}
