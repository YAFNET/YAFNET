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

namespace yaf.install
{
	/// <summary>
	/// Summary description for install.
	/// </summary>
	public class _default : System.Web.UI.Page
	{
		enum Step 
		{
			Welcome = 0,
			Config,
			Connect,
			Database,
			Forum,
			Finished
		};

		private int InstalledVersion = 0;
		private Step CurStep = Step.Welcome;
		protected System.Web.UI.WebControls.Button back, next, finish;
		protected System.Web.UI.WebControls.Label cursteplabel;
		protected System.Web.UI.HtmlControls.HtmlTable stepWelcome, stepConfig, stepConnect, stepDatabase, stepForum, stepFinished;
		protected PlaceHolder ConfigSample;
		// Forum
		protected System.Web.UI.WebControls.TextBox TheForumName, UserName, Password1, Password2, AdminEmail, ForumEmailAddress, SmptServerAddress;
		protected System.Web.UI.WebControls.DropDownList TimeZones;

		private	string	m_loadMessage	= "";
		private string[]	m_scripts	= new string[]
		{
			"version-0.7.0_sql",	//  1
			"version-0.7.1_sql",	//  2
			"version-0.8.0_sql",	//  3
			"version-0.8.1_sql",	//  4
			"version-0.8.2_sql",	//  5
			"version-0.9.0_sql",	//  6
			"version-0.9.1_sql",	//  7
			"version-0.9.2_sql",	//  8
			"version-0.9.3_sql",	//  9
			"version-0.9.4_sql",	// 10
			"version-0.9.5_sql",	// 11
			"version-0.9.6_sql",	// 12
			"version-0.9.7_sql",	// 13
			"version-0.9.8_sql",	// 14
			"version-0.9.9.sql"		// 15
		};

		void AddLoadMessage(string msg)
		{
			msg = msg.Replace("\\","\\\\");
			msg = msg.Replace("'","\\'");
			msg = msg.Replace("\r\n","\\r\\n");
			msg = msg.Replace("\n","\\n");
			msg = msg.Replace("\"","\\\"");
			m_loadMessage += msg + "\\n\\n";
		}

		public _default() {
			InstalledVersion = GetCurrentVersion();
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!IsPostBack) {
				if(InstalledVersion >= Data.AppVersion) {
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

		public static int GetCurrentVersion() 
		{
			try
			{
				// get newer version from registry
				using (DataTable dt = DB.registry_list("Version"))
					foreach(DataRow row in dt.Rows)
						return Convert.ToInt32(row["Value"]);
			}
			catch(Exception)
			{
			}

			// attempt to get older version information
			try 
			{
				using(DataTable dt = DB.system_list())
					foreach(DataRow row in dt.Rows) 
						return Convert.ToInt32(row["Version"]);
			}
			catch(Exception) 
			{
			}
			return 0;
		}

		private void finish_Click(object sender,System.EventArgs e) {
			if(Config.IsDotNetNuke)
			{
				//Redirect back to the portal main page.
				string rPath = Data.ForumRoot;
				int pos = rPath.IndexOf("/",2);
				rPath = rPath.Substring(0,pos);
				Response.Redirect(rPath);
			}
			else
			{
				Response.Redirect(Data.ForumRoot);
			}
		}

		private void next_Click(object sender,System.EventArgs e) 
		{
			if(CurStep == Step.Config)
			{
				Config config = Config.ConfigSection;
				ConfigSample.Visible = true;
				if(config==null)
				{
					AddLoadMessage("Web.config is missing the configuration/yafnet section.");
					return;
				}
				if(config["connstr"]==null)
				{
					AddLoadMessage("Web.config is missing configuration/yafnet/connstr");
					return;
				}
				if(config["root"]==null)
				{
					AddLoadMessage("Web.config is missing configuration/yafnet/root");
					return;
				}
				if(config["language"]==null)
				{
					AddLoadMessage("Web.config is missing configuration/yafnet/language");
					return;
				}
				if(config["theme"]==null)
				{
					AddLoadMessage("Web.config is missing configuration/yafnet/theme");
					return;
				}

				ConfigSample.Visible = false;
				//if(config["connstr"]==null
			} 
			else if(CurStep == Step.Connect) 
			{
				try 
				{
					using(SqlConnection conn=DB.GetConnection()) 
					{
						conn.Close();
					}
				}
				catch(Exception x) 
				{
					AddLoadMessage(String.Format("Connection failed. Modify Web.config and try again.\n\nThe error message was:\n\n{0}",x.Message));
					return;
				}
			} 
			else if(CurStep == Step.Database) 
			{
				try 
				{
					if(InstalledVersion>0 && InstalledVersion<14) 
					{
						AddLoadMessage("You must upgrade to version 0.9.8 before installing this version.");
						return;
					}
					FixAccess(false);

					for(int i=InstalledVersion;i<m_scripts.Length;i++)
						ExecuteScript(m_scripts[i]);

					FixAccess(true);

					using(SqlCommand cmd = new SqlCommand("yaf_system_updateversion")) 
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@Version",Data.AppVersion);
						cmd.Parameters.Add("@VersionName",Data.AppVersionName);
						DB.ExecuteNonQuery(cmd);
					}
				}
				catch(Exception x) 
				{
					AddLoadMessage(x.Message);
					return;
				}
			} 
			else if(CurStep == Step.Forum) 
			{
				if(TheForumName.Text.Length==0) 
				{
					AddLoadMessage("You must enter a forum name.");
					return;
				}
				if(ForumEmailAddress.Text.Length == 0) 
				{
					AddLoadMessage("You must enter a forum email address.");
					return;
				}
				if(SmptServerAddress.Text.Length == 0) 
				{
					AddLoadMessage("You must enter a smtp server.");
					return;
				}
				if(UserName.Text.Length==0) 
				{
					AddLoadMessage("You must enter the admin user name,");
					return;
				}
				if(AdminEmail.Text.Length == 0) 
				{
					AddLoadMessage("You must enter the administrators email address.");
					return;
				}
				if(Password1.Text.Length==0) 
				{
					AddLoadMessage("You must enter a password.");
					return;
				}
				if(Password1.Text != Password2.Text) 
				{
					AddLoadMessage("The passwords must match.");
					return;
				}
				try 
				{
					using(SqlCommand cmd = new SqlCommand("yaf_system_initialize")) 
					{
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

					using(SqlCommand cmd = new SqlCommand("yaf_system_updateversion")) 
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@Version",Data.AppVersion);
						cmd.Parameters.Add("@VersionName",Data.AppVersionName);
						DB.ExecuteNonQuery(cmd);
					}
				}
				catch(Exception x) 
				{
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
				case Step.Config:
					stepConfig.Visible = false;
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
				case Step.Config:
					stepConfig.Visible = true;
					back.Enabled = true;
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

		override protected void OnInit(EventArgs e)
		{
			this.Load += new System.EventHandler(this.Page_Load);
			back.Click += new System.EventHandler(back_Click);
			next.Click += new System.EventHandler(next_Click);
			finish.Click += new System.EventHandler(finish_Click);
			base.OnInit(e);
		}
		
		protected override void Render(System.Web.UI.HtmlTextWriter writer) 
		{
			base.Render(writer);
			if(m_loadMessage!="")
			{
				writer.WriteLine("<script language='javascript'>");
				writer.WriteLine("onload = function() {");
				writer.WriteLine("	alert('{0}');",m_loadMessage);
				writer.WriteLine("}");
				writer.WriteLine("</script>");
			}
		}

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
			catch(System.IO.FileNotFoundException) 
			{
				return;
			}
			catch(Exception x) 
			{
				throw new Exception("Failed to read "+sScriptFile,x);
			}

			string[] statements = System.Text.RegularExpressions.Regex.Split(sScript, "\\sGO\\s", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

			using(SqlConnection conn = DB.GetConnection()) 
			{
				using(SqlTransaction trans = conn.BeginTransaction(DB.IsolationLevel)) 
				{
					foreach(string sql0 in statements) 
					{
						string sql = sql0.Trim();

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

		private void FixAccess(bool bGrant) 
		{
			using(SqlConnection conn = DB.GetConnection()) 
			{
				using(SqlTransaction trans=conn.BeginTransaction(DB.IsolationLevel)) 
				{
					using(SqlDataAdapter da=new SqlDataAdapter("select Name,IsUserTable = OBJECTPROPERTY(id, N'IsUserTable'),IsScalarFunction = OBJECTPROPERTY(id, N'IsScalarFunction'),IsProcedure = OBJECTPROPERTY(id, N'IsProcedure'),IsView = OBJECTPROPERTY(id, N'IsView') from dbo.sysobjects where Name like 'yaf_%'",conn)) 
					{
						da.SelectCommand.Transaction = trans;
						using(DataTable dt=new DataTable("sysobjects")) 
						{
							da.Fill(dt);
							using(SqlCommand cmd=conn.CreateCommand()) 
							{
								cmd.Transaction = trans;
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = "select current_user";
								string userName = (string)cmd.ExecuteScalar();

								if(bGrant) 
								{
									cmd.CommandType = CommandType.Text;
									foreach(DataRow row in dt.Select("IsProcedure=1 or IsScalarFunction=1")) 
									{
										cmd.CommandText = string.Format("grant execute on {0} to {1}",row["Name"],userName);
										cmd.ExecuteNonQuery();
									}
									foreach(DataRow row in dt.Select("IsUserTable=1 or IsView=1")) 
									{
										cmd.CommandText = string.Format("grant select on {0} to {1}",row["Name"],userName);
										cmd.ExecuteNonQuery();
									}
								} 
								else 
								{
									cmd.CommandText = "sp_changeobjectowner";
									cmd.CommandType = CommandType.StoredProcedure;
									foreach(DataRow row in dt.Select("IsUserTable=1")) 
									{
										cmd.Parameters.Clear();
										cmd.Parameters.Add("@objname",row["Name"]);
										cmd.Parameters.Add("@newowner","dbo");
										try
										{
											cmd.ExecuteNonQuery();
										}
										catch(SqlException)
										{
										}
									}
									foreach(DataRow row in dt.Select("IsView=1")) 
									{
										cmd.Parameters.Clear();
										cmd.Parameters.Add("@objname",row["Name"]);
										cmd.Parameters.Add("@newowner","dbo");
										try
										{
											cmd.ExecuteNonQuery();
										}
										catch(SqlException)
										{
										}
									}
								}
							}
						}
					}
					trans.Commit();
				}
			}
		}
	}
}
