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
using System.Globalization;

namespace yaf
{
	/// <summary>
	/// Summary description for install.
	/// </summary>
	public class install : BasePage
	{
		enum Step 
		{
			Welcome = 0,
			Connect,
			Database,
			Forum,
			Finished
		};

		private int InstalledVersion = 0;
		private Step CurStep = Step.Welcome;
		protected System.Web.UI.WebControls.Button back, next, finish;
		protected System.Web.UI.WebControls.Label cursteplabel;
		protected System.Web.UI.HtmlControls.HtmlTable stepWelcome, stepConnect, stepDatabase, stepForum, stepFinished;
		// Forum
		protected System.Web.UI.WebControls.TextBox TheForumName, UserName, Password1, Password2, AdminEmail, ForumEmailAddress, SmptServerAddress;
		protected System.Web.UI.WebControls.DropDownList TimeZones;

		public install() {
			NoDataBase = true;
			InstalledVersion = GetCurrentVersion();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				if(InstalledVersion >= AppVersion) {
					LeaveStep(CurStep);
					CurStep = Step.Finished;
					EnterStep(CurStep);
				}

				cursteplabel.Text = ((int)CurStep).ToString();
				TimeZones.DataSource = Data.TimeZones();
				DataBind();
				TimeZones.Items.FindByValue("0").Selected = true;
			} else {
				CurStep = (Step)int.Parse(cursteplabel.Text);
			}
		}

		private void back_Click(object sender,System.EventArgs e) {
			LeaveStep(CurStep);
			EnterStep(--CurStep);
			cursteplabel.Text = ((int)CurStep).ToString();
		}

		private int GetCurrentVersion() {
			try {
				using(DataTable dt = DB.system_list()) {
					if(dt.Rows.Count == 0)
						return 0;
					return (int)dt.Rows[0]["Version"];
				}
			}
			catch(Exception) {
				return 0;
			}
		}

		private void finish_Click(object sender,System.EventArgs e) {
			Response.Redirect(BaseDir);
		}

		private void next_Click(object sender,System.EventArgs e) {
			if(CurStep == Step.Connect) {
				using(SqlConnection conn = DB.GetConnection()) {
					if(conn==null) {
						AddLoadMessage("Connection failed. Modify Web.config and try again.");
						return;
					}
				}
			} else if(CurStep == Step.Database) {
				try {
					for(long i=InstalledVersion;i<AppVersion;i++) 
						ExecuteScript(String.Format("install/version{0}.sql",i+1));

					using(SqlCommand cmd = new SqlCommand("yaf_system_updateversion")) 
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@Version",AppVersion);
						cmd.Parameters.Add("@VersionName",AppVersionName);
						DB.ExecuteNonQuery(cmd);
					}
				}
				catch(Exception x) {
					AddLoadMessage(x.Message);
					return;
				}
			} else if(CurStep == Step.Forum) {
				if(TheForumName.Text.Length==0) {
					AddLoadMessage("You must enter a forum name.");
					return;
				}
				if(ForumEmailAddress.Text.Length == 0) {
					AddLoadMessage("You must enter a forum email address.");
					return;
				}
				if(SmptServerAddress.Text.Length == 0) {
					AddLoadMessage("You must enter a smtp server.");
					return;
				}
				if(UserName.Text.Length==0) {
					AddLoadMessage("You must enter the admin user name,");
					return;
				}
				if(AdminEmail.Text.Length == 0) {
					AddLoadMessage("You must enter the administrators email address.");
					return;
				}
				if(Password1.Text.Length==0) {
					AddLoadMessage("You must enter a password.");
					return;
				}
				if(Password1.Text != Password2.Text) {
					AddLoadMessage("The passwords must match.");
					return;
				}
				try {
					using(SqlCommand cmd = new SqlCommand("yaf_system_initialize")) {
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@Name",TheForumName.Text);
						cmd.Parameters.Add("@TimeZone",TimeZones.SelectedItem.Value);
						cmd.Parameters.Add("@ForumEmail",ForumEmailAddress.Text);
						cmd.Parameters.Add("@SmtpServer",SmptServerAddress.Text);
						cmd.Parameters.Add("@User",UserName.Text);
						cmd.Parameters.Add("@UserEmail",AdminEmail.Text);
						cmd.Parameters.Add("@Password",System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(Password1.Text,"md5"));
						DB.ExecuteNonQuery(cmd);
					}
				}
				catch(Exception x) {
					AddLoadMessage(x.Message);
					return;
				}
			}
			LeaveStep(CurStep);
			if(CurStep == Step.Database && InstalledVersion > 0)
				CurStep = Step.Finished;
			else
				++CurStep;

			EnterStep(CurStep);
			cursteplabel.Text = ((int)CurStep).ToString();
		}

		private void LeaveStep(Step step) {
			switch(step) {
				case Step.Welcome:
					stepWelcome.Visible = false;
					break;
				case Step.Connect:
					stepConnect.Visible = false;
					break;
				case Step.Database:
					stepDatabase.Visible = false;
					break;
				case Step.Forum:
					stepForum.Visible = false;
					break;
			}
		}

		private void EnterStep(Step step) {
			switch(step) {
				case Step.Welcome:
					stepWelcome.Visible = true;
					back.Enabled = false;
					next.Enabled = true;
					break;
				case Step.Connect:
					stepConnect.Visible = true;
					back.Enabled = true;
					next.Enabled = true;
					break;
				case Step.Database:
					stepDatabase.Visible = true;
					back.Enabled = true;
					next.Enabled = true;
					break;
				case Step.Forum:
					stepForum.Visible = true;
					back.Enabled = false;
					next.Enabled = true;
					break;
				case Step.Finished:
					stepFinished.Visible = true;
					back.Enabled = false;
					next.Enabled = false;
					finish.Enabled = true;
					break;
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			back.Click += new System.EventHandler(back_Click);
			next.Click += new System.EventHandler(next_Click);
			finish.Click += new System.EventHandler(finish_Click);
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
			this.Load += new System.EventHandler(this.Page_Load);
		}
		#endregion

		private void ExecuteScript(string sScriptFile) 
		{
			string sScript = null;
			try 
			{
				using(System.IO.StreamReader file = new System.IO.StreamReader(Request.MapPath(sScriptFile))) 
				{
					sScript = file.ReadToEnd();
					file.Close();
				}
			}
			catch(Exception x) 
			{
				throw new Exception("Failed to read script file",x);
			}

			string[] statements = System.Text.RegularExpressions.Regex.Split(sScript, "\\sGO\\s", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			using(SqlConnection conn = DB.GetConnection()) 
			{
				using(SqlTransaction trans = conn.BeginTransaction()) 
				{
					foreach(string sql0 in statements) 
					{
						string sql = sql0.Trim();
						sql = sql.Replace("[dbo].","");
						try 
						{
							if(sql.ToLower().IndexOf("setuser")>=0)
								continue;

							if(sql.Length>0) 
							{
								using(SqlCommand cmd = new SqlCommand()) 
								{
									cmd.Transaction = trans;
									cmd.Connection = conn;
									cmd.CommandType = CommandType.Text;
									cmd.CommandText = sql.Trim();
									cmd.ExecuteNonQuery();
								}
							}
						}
						catch(Exception x) 
						{
							trans.Rollback();
							throw new Exception(String.Format("FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}",sScriptFile,sql,x.Message));
						}
					}
					trans.Commit();
				}
			}
		}
	}
}
