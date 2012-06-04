/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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

namespace YAF.Install
{
	#region Using

	using System;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using System.Drawing;
	using System.IO;
	using System.Linq;
	using System.Security.Permissions;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using YAF.Classes;
	using YAF.Classes.Data;
	using YAF.Classes.Data.Import;
	using YAF.Core;
	using YAF.Core.Tasks;
	using YAF.Types;
	using YAF.Types.Interfaces;
	using YAF.Utils;
	using YAF.Utils.Helpers;

	#endregion

	/// <summary>
	/// The Install Page.
	/// </summary>
	public partial class _default : Page, IHaveServiceLocator
	{
		#region Constants and Fields

		/// <summary>
		///   The _db version before upgrade.
		/// </summary>
		protected int _dbVersionBeforeUpgrade;

		/// <summary>
		///   The _app password key.
		/// </summary>
		private const string _AppPasswordKey = "YAF.ConfigPassword";

		/// <summary>
		///   The _bbcode import.
		/// </summary>
		private const string _BbcodeImport = "bbCodeExtensions.xml";

		/// <summary>
		///   The _file import.
		/// </summary>
		private const string _FileImport = "fileExtensions.xml";

		/// <summary>
		///   The Topic Status Import File.
		/// </summary>
		private const string _TopicStatusImport = "TopicStatusList.xml";

		/// <summary>
		///   The _config.
		/// </summary>
		private readonly ConfigHelper _config = new ConfigHelper();

		/// <summary>
		///   The _load message.
		/// </summary>
		private string _loadMessage = string.Empty;

		#endregion

		#region Enums

		/// <summary>
		/// The update db failure type.
		/// </summary>
		private enum UpdateDBFailureType
		{
			/// <summary>
			///   The none.
			/// </summary>
			None,

			/// <summary>
			///   The app settings write.
			/// </summary>
			AppSettingsWrite,

			/// <summary>
			///   The connection string write.
			/// </summary>
			ConnectionStringWrite
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets ServiceLocator.
		/// </summary>
		public IServiceLocator ServiceLocator
		{
			get
			{
				return YafContext.Current.ServiceLocator;
			}
		}

		/// <summary>
		///   Gets CurrentConnString.
		/// </summary>
		private string CurrentConnString
		{
			get
			{
				if (this.rblYAFDatabase.SelectedValue == "existing")
				{
					string connName = this.lbConnections.SelectedValue;

					return connName.IsSet() ? ConfigurationManager.ConnectionStrings[connName].ConnectionString : string.Empty;
				}

				var connectionStringBuilder = this.Get<DbProviderFactory>().CreateConnectionStringBuilder();

				connectionStringBuilder["Data Source"] = this.Parameter1_Value.Text.Trim();
				connectionStringBuilder["Initial Catalog"] = this.Parameter1_Value.Text.Trim();
				connectionStringBuilder["integrated Security"] = this.Parameter1_Value.Text.Trim();

				return MsSqlDbAccess.GetConnectionString(
						this.Parameter1_Value.Text.Trim(),
						this.Parameter2_Value.Text.Trim(),
						this.Parameter3_Value.Text.Trim(),
						this.Parameter4_Value.Text.Trim(),
						this.Parameter5_Value.Text.Trim(),
						this.Parameter6_Value.Text.Trim(),
						this.Parameter7_Value.Text.Trim(),
						this.Parameter8_Value.Text.Trim(),
						this.Parameter9_Value.Text.Trim(),
						this.Parameter10_Value.Text.Trim(),
						this.Parameter11_Value.Checked,
						this.Parameter12_Value.Checked,
						this.Parameter13_Value.Checked,
						this.Parameter14_Value.Checked,
						this.Parameter15_Value.Checked,
						this.Parameter16_Value.Checked,
						this.Parameter17_Value.Checked,
						this.Parameter18_Value.Checked,
						this.Parameter19_Value.Checked,
						this.txtDBUserID.Text.Trim(),
						this.txtDBPassword.Text.Trim());
			}
		}

		/// <summary>
		///   Gets or sets CurrentWizardStepID.
		/// </summary>
		private string CurrentWizardStepID
		{
			get
			{
				return this.InstallWizard.WizardSteps[this.InstallWizard.ActiveStepIndex].ID;
			}

			set
			{
				int index = this.IndexOfWizardID(value);
				if (index >= 0)
				{
					this.InstallWizard.ActiveStepIndex = index;
				}
			}
		}

		/// <summary>
		///   Gets a value indicating whether IsInstalled.
		/// </summary>
		private bool IsInstalled
		{
			get
			{
				return this._config.GetConfigValueAsString(_AppPasswordKey).IsSet();
			}
		}

		/// <summary>
		///   Gets PageBoardID.
		/// </summary>
		private int PageBoardID
		{
			get
			{
				try
				{
					return int.Parse(Config.BoardID);
				}
				catch
				{
					return 1;
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// The get membership error message.
		/// </summary>
		/// <param name="status">
		/// The status.
		/// </param>
		/// <returns>
		/// The get membership error message.
		/// </returns>
		[NotNull]
		public string GetMembershipErrorMessage(MembershipCreateStatus status)
		{
			switch (status)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "Username already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A username for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return
							"The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return
							"The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return
							"An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// The page_ init.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Init([NotNull] object sender, [NotNull] EventArgs e)
		{
			string connectionString = Config.ConnectionString;

			if (YafContext.Current.Vars.ContainsKey("ConnectionString"))
			{
				connectionString = YafContext.Current.Vars["ConnectionString"] as string;
			}

			// set the connection manager to the dynamic...
			this.Get<IDbAccess>().ConnectionString = connectionString;
		}

		/// <summary>
		/// The parameter 11_ value_ check changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Parameter11_Value_CheckChanged([NotNull] object sender, [NotNull] EventArgs e)
		{
			this.DBUsernamePasswordHolder.Visible = !this.Parameter11_Value.Checked;
		}

		/// <summary>
		/// The render.
		/// </summary>
		/// <param name="writer">
		/// The writer.
		/// </param>
		protected override void Render([NotNull] HtmlTextWriter writer)
		{
			base.Render(writer);

			if (this._loadMessage != string.Empty)
			{
				writer.WriteLine("<script language='javascript'>");
				writer.WriteLine("onload = function() {");
				writer.WriteLine("	alert('{0}');", this._loadMessage);
				writer.WriteLine("}");
				writer.WriteLine("</script>");
			}
		}

		/// <summary>
		/// The update status timer_ tick.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void UpdateStatusTimer_Tick([NotNull] object sender, [NotNull] EventArgs e)
		{
			// see if the migration is done....
			if (this.Get<ITaskModuleManager>().IsTaskRunning(MigrateUsersTask.TaskName))
			{
				// proceed...
				return;
			}

			if (this.Session["InstallWizardFinal"] == null)
			{
				this.Session.Add("InstallWizardFinal", true);
			}

			// done here...
			this.Response.Redirect("default.aspx");
		}

		/// <summary>
		/// The user choice_ selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void UserChoice_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
		{
			switch (this.UserChoice.SelectedValue)
			{
				case "create":
					this.ExistingUserHolder.Visible = false;
					this.CreateAdminUserHolder.Visible = true;
					break;
				case "existing":
					this.ExistingUserHolder.Visible = true;
					this.CreateAdminUserHolder.Visible = false;
					break;
			}
		}

		/// <summary>
		/// The wizard_ active step changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Wizard_ActiveStepChanged([NotNull] object sender, [NotNull] EventArgs e)
		{
			bool previousVisible = false;

			switch (this.CurrentWizardStepID)
			{
				case "WizCreatePassword":
					if (this._config.TrustLevel == AspNetHostingPermissionLevel.High &&
							this._config.AppSettingsFull != null)
					{
						this.lblConfigPasswordAppSettingFile.Text = this._config.AppSettingsFull.File;
					}
					else
					{
						this.lblConfigPasswordAppSettingFile.Text = "(Unknown! YAF default is app.config)";
					}

					if (this.IsInstalled)
					{
						// no need for this setup if IsInstalled...
						this.InstallWizard.ActiveStepIndex++;
					}

					break;
				case "WizCreateForum":
					if (LegacyDb.GetIsForumInstalled())
					{
						this.InstallWizard.ActiveStepIndex++;
					}

					break;
				case "WizMigrateUsers":
					if (!this.IsInstalled)
					{
						// no migration because it's a new install...
						this.CurrentWizardStepID = "WizFinished";
					}
					else
					{
						object version = this.Cache["DBVersion"] ?? LegacyDb.GetDBVersion();

						if (((int)version) >= 30 || ((int)version) == -1)
						{
							// migration is NOT needed...
							this.CurrentWizardStepID = "WizFinished";
						}

						this.Cache.Remove("DBVersion");
					}

					// get user count
					if (this.CurrentWizardStepID == "WizMigrateUsers")
					{
						this.lblMigrateUsersCount.Text =
								LegacyDb.user_list(this.PageBoardID, null, true).Rows.Count.ToString();
					}

					break;
				case "WizDatabaseConnection":
					previousVisible = true;

					// fill with connection strings...
					this.lblConnStringAppSettingName.Text = Config.ConnectionStringName;
					this.FillWithConnectionStrings();
					break;
				case "WizManualDatabaseConnection":
					if (this._config.TrustLevel == AspNetHostingPermissionLevel.High &&
							this._config.AppSettingsFull != null)
					{
						this.lblAppSettingsFile.Text = this._config.AppSettingsFull.File;
					}
					else
					{
						this.lblAppSettingsFile.Text = "(Unknown! YAF default is app.config)";
					}

					previousVisible = true;
					break;
				case "WizManuallySetPassword":
					if (this._config.TrustLevel == AspNetHostingPermissionLevel.High &&
							this._config.AppSettingsFull != null)
					{
						this.lblAppSettingsFile2.Text = this._config.AppSettingsFull.File;
					}
					else
					{
						this.lblAppSettingsFile2.Text = "(Unknown! YAF default is app.config)";
					}

					break;
				case "WizTestSettings":
					previousVisible = true;
					break;
				case "WizValidatePermission":
					break;
				case "WizMigratingUsers":

					// disable the next button...
					var btnNext =
							this.InstallWizard.FindControlAs<Button>("StepNavigationTemplateContainerID$StepNextButton");
					if (btnNext != null)
					{
						btnNext.Enabled = false;
					}

					break;
			}

			var btnPrevious =
					this.InstallWizard.FindControlAs<Button>("StepNavigationTemplateContainerID$StepPreviousButton");

			if (btnPrevious != null)
			{
				btnPrevious.Visible = previousVisible;
			}
		}

		/// <summary>
		/// The wizard_ finish button click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Wizard_FinishButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
		{
			// reset the board settings...
			YafContext.Current.BoardSettings = null;

			/* if (Config.IsDotNetNuke)
{
	// Redirect back to the portal main page.
	string rPath = YafForumInfo.ForumClientFileRoot;
	int pos = rPath.IndexOf("/", 2);
	rPath = rPath.Substring(0, pos);
	this.Response.Redirect(rPath);
}
else
{*/
			this.Response.Redirect("~/");
			//}
		}

		/// <summary>
		/// The wizard_ next button click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		/// <exception cref="ApplicationException">
		/// </exception>
		protected void Wizard_NextButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
		{
			e.Cancel = true;

			switch (this.CurrentWizardStepID)
			{
				case "WizValidatePermission":
					e.Cancel = false;
					break;
				case "WizDatabaseConnection":

					// save the database settings...
					UpdateDBFailureType type = this.UpdateDatabaseConnection();
					e.Cancel = false;

					switch (type)
					{
						case UpdateDBFailureType.None:
							this.CurrentWizardStepID = "WizTestSettings";
							break;
						case UpdateDBFailureType.AppSettingsWrite:
							this.NoWriteAppSettingsHolder.Visible = true;
							break;
						case UpdateDBFailureType.ConnectionStringWrite:
							this.NoWriteDBSettingsHolder.Visible = true;
							this.lblDBConnStringName.Text = Config.ConnectionStringName;
							this.lblDBConnStringValue.Text = this.CurrentConnString;
							break;
					}

					break;
				case "WizManualDatabaseConnection":
					e.Cancel = false;
					break;
				case "WizCreatePassword":
					if (this.txtCreatePassword1.Text.Trim() == string.Empty)
					{
						this.AddLoadMessage("Please enter a configuration password.");
						break;
					}

					if (this.txtCreatePassword2.Text != this.txtCreatePassword1.Text)
					{
						this.AddLoadMessage("Verification is not the same as your password.");
						break;
					}

					e.Cancel = false;

					if (this._config.TrustLevel == AspNetHostingPermissionLevel.High &&
							this._config.WriteAppSetting(_AppPasswordKey, this.txtCreatePassword1.Text))
					{
						// advance to the testing section since the password is now set...
						this.CurrentWizardStepID = "WizDatabaseConnection";
					}
					else
					{
						this.CurrentWizardStepID = "WizManuallySetPassword";
					}

					break;
				case "WizManuallySetPassword":
					if (this.IsInstalled)
					{
						e.Cancel = false;
					}
					else
					{
						this.AddLoadMessage(
								"You must update your appSettings with the YAF.ConfigPassword Key to continue. NOTE: The key name is case sensitive.");
					}

					break;
				case "WizTestSettings":
					e.Cancel = false;
					break;
				case "WizEnterPassword":
					if (this._config.GetConfigValueAsString(_AppPasswordKey) ==
							FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtEnteredPassword.Text, "md5") ||
							this._config.GetConfigValueAsString(_AppPasswordKey) == this.txtEnteredPassword.Text.Trim())
					{
						e.Cancel = false;

						// move to test settings...
						this.CurrentWizardStepID = "WizTestSettings";
					}
					else
					{
						this.AddLoadMessage("Wrong password!");
					}

					break;
				case "WizCreateForum":
					if (this.CreateForum())
					{
						e.Cancel = false;
					}

					break;
				case "WizInitDatabase":
					if (this.UpgradeDatabase(this.FullTextSupport.Checked))
					{
						e.Cancel = false;
					}

					break;
				case "WizMigrateUsers":

					// migrate users/roles only if user does not want to skip
					if (!this.skipMigration.Checked)
					{
						RoleMembershipHelper.SyncRoles(this.PageBoardID);

						// start the background migration task...
						this.Get<ITaskModuleManager>().Start<MigrateUsersTask>(this.PageBoardID);
					}

					e.Cancel = false;
					break;
				case "WizFinished":
					break;
				default:
					throw new ApplicationException(
                        "Installation Wizard step not handled: {0}".FormatWith(
                            this.InstallWizard.WizardSteps[e.CurrentStepIndex].ID));
			}
		}

		/// <summary>
		/// The wizard_ previous button click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Wizard_PreviousButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
		{
			if (this.CurrentWizardStepID == "WizTestSettings")
			{
				this.CurrentWizardStepID = "WizDatabaseConnection";
			}

			e.Cancel = false;

			//// go back only from last step (to user/roles migration)
			// if ( e.CurrentStepIndex == ( InstallWizard.WizardSteps.Count - 1 ) )
			// InstallWizard.MoveTo( InstallWizard.WizardSteps[e.CurrentStepIndex - 1] );
			// else
			// // othwerise cancel action
			// e.Cancel = true;
		}

		/// <summary>
		/// The btn test db connection manual_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void btnTestDBConnectionManual_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			// attempt to connect DB...
			string message;

			if (!TestDatabaseConnection(out message))
			{
				UpdateInfoPanel(
						this.ManualConnectionInfoHolder,
						this.lblConnectionDetailsManual,
              "Failed to connect:<br /><br />{0}".FormatWith(message),
						"errorinfo");
			}
			else
			{
				UpdateInfoPanel(
						this.ManualConnectionInfoHolder,
						this.lblConnectionDetailsManual,
						"Connection Succeeded",
						"successinfo");
			}
		}

		/// <summary>
		/// The btn test db connection_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
    protected void TestDBConnection_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			// attempt to connect selected DB...
			YafContext.Current["ConnectionString"] = this.CurrentConnString;
			string message;

			if (!TestDatabaseConnection(out message))
			{
				UpdateInfoPanel(
						this.ConnectionInfoHolder,
						this.lblConnectionDetails,
            "Failed to connect:<br /><br />{0}".FormatWith(message),
						"errorinfo");
			}
			else
			{
				UpdateInfoPanel(
						this.ConnectionInfoHolder, this.lblConnectionDetails, "Connection Succeeded", "successinfo");
			}

			// we're done with it...
			YafContext.Current.Vars.Remove("ConnectionString");
		}

		/// <summary>
		/// The btn test permissions_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
    protected void TestPermissions_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			UpdateStatusLabel(this.lblPermissionApp, 1);
			UpdateStatusLabel(this.lblPermissionUpload, 1);
			UpdateStatusLabel(this.lblHostingTrust, 1);

			UpdateStatusLabel(this.lblPermissionApp, DirectoryHasWritePermission(this.Server.MapPath("~/")) ? 2 : 0);
			UpdateStatusLabel(
					this.lblPermissionUpload,
					DirectoryHasWritePermission(this.Server.MapPath(YafBoardFolders.Current.Uploads)) ? 2 : 0);

			UpdateStatusLabel(
					this.lblHostingTrust, this._config.TrustLevel == AspNetHostingPermissionLevel.High ? 2 : 0);

			this.lblHostingTrust.Text = this._config.TrustLevel.GetStringValue();
		}

		/// <summary>
		/// The btn test smtp_ click.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
    protected void TestSmtp_Click([NotNull] object sender, [NotNull] EventArgs e)
		{
			try
			{
				this.Get<ISendMail>().Send(
						this.txtTestFromEmail.Text.Trim(),
						this.txtTestToEmail.Text.Trim(),
						"Test Email From Yet Another Forum.NET",
						"The email sending appears to be working from your YAF installation.");

				// success
				UpdateInfoPanel(
						this.SmtpInfoHolder,
						this.lblSmtpTestDetails,
						"Mail Sent. Verify it's received at your entered email address.",
						"successinfo");
			}
			catch (Exception x)
			{
				UpdateInfoPanel(
						this.SmtpInfoHolder,
						this.lblSmtpTestDetails,
            "Failed to connect:<br /><br />{0}".FormatWith(x.Message),
						"errorinfo");
			}
		}

		/// <summary>
		/// The rbl yaf database_ selected index changed.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
    protected void YAFDatabase_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
		{
			switch (this.rblYAFDatabase.SelectedValue)
			{
				case "create":
					this.ExistingConnectionHolder.Visible = false;
					this.NewConnectionHolder.Visible = true;
					break;
				case "existing":
					this.ExistingConnectionHolder.Visible = true;
					this.NewConnectionHolder.Visible = false;
					break;
			}
		}

		/// <summary>
		/// Validates write permission in a specific directory. Should be moved to YAF.Classes.Utils.
		/// </summary>
		/// <param name="directory">
		/// </param>
		/// <returns>
		/// The directory has write permission.
		/// </returns>
		private static bool DirectoryHasWritePermission([NotNull] string directory)
		{
			bool hasWriteAccess;

			try
			{
				// see if we have permission
				var fp = new FileIOPermission(FileIOPermissionAccess.Write, directory);
				fp.Demand();

				hasWriteAccess = true;
			}
			catch
			{
				hasWriteAccess = false;
			}

			return hasWriteAccess;
		}

		/// <summary>
		/// Tests database connection. Can probably be moved to DB class.
		/// </summary>
		/// <param name="exceptionMessage">
		/// </param>
		/// <returns>
		/// The test database connection.
		/// </returns>
    private static bool TestDatabaseConnection([NotNull] out string exceptionMessage)
		{
			return this.Get<Types.Interfaces.IDbAccess>().TestConnection(out exceptionMessage);
		}

		/// <summary>
		/// The update info panel.
		/// </summary>
		/// <param name="infoHolder">
		/// The info holder.
		/// </param>
		/// <param name="detailsLabel">
		/// The details label.
		/// </param>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="cssClass">
		/// The css class.
		/// </param>
		private static void UpdateInfoPanel(
				[NotNull] Control infoHolder, [NotNull] Label detailsLabel, [NotNull] string info, [NotNull] string cssClass)
		{
			infoHolder.Visible = true;
			detailsLabel.Text = info;
			detailsLabel.CssClass = cssClass;
		}

		/// <summary>
		/// The update status label.
		/// </summary>
		/// <param name="theLabel">
		/// The the label.
		/// </param>
		/// <param name="status">
		/// The status.
		/// </param>
		private static void UpdateStatusLabel([NotNull] Label theLabel, int status)
		{
			switch (status)
			{
				case 0:
					theLabel.Text = "No";
					theLabel.ForeColor = Color.Red;
					break;
				case 1:
					theLabel.Text = "Unchcked";
					theLabel.ForeColor = Color.Gray;
					break;
				case 2:
					theLabel.Text = "YES";
					theLabel.ForeColor = Color.Green;
					break;
			}
		}

		/// <summary>
		/// The add load message.
		/// </summary>
		/// <param name="msg">
		/// The msg.
		/// </param>
		private void AddLoadMessage([NotNull] string msg)
		{
			msg = msg.Replace("\\", "\\\\");
			msg = msg.Replace("'", "\\'");
			msg = msg.Replace("\r\n", "\\r\\n");
			msg = msg.Replace("\n", "\\n");
			msg = msg.Replace("\"", "\\\"");
			this._loadMessage += msg + "\\n\\n";
		}

		/// <summary>
		/// The create forum.
		/// </summary>
		/// <returns>
		/// The create forum.
		/// </returns>
		private bool CreateForum()
		{
			if (LegacyDb.GetIsForumInstalled())
			{
				this.AddLoadMessage("Forum is already installed.");
				return false;
			}

			if (this.TheForumName.Text.Length == 0)
			{
				this.AddLoadMessage("You must enter a forum name.");
				return false;
			}

			if (this.ForumEmailAddress.Text.Length == 0)
			{
				this.AddLoadMessage("You must enter a forum email address.");
				return false;
			}

			MembershipUser user;

			if (this.UserChoice.SelectedValue == "create")
			{
				if (this.UserName.Text.Length == 0)
				{
					this.AddLoadMessage("You must enter the admin user name,");
					return false;
				}

				if (this.AdminEmail.Text.Length == 0)
				{
					this.AddLoadMessage("You must enter the administrators email address.");
					return false;
				}

				if (this.Password1.Text.Length == 0)
				{
					this.AddLoadMessage("You must enter a password.");
					return false;
				}

				if (this.Password1.Text != this.Password2.Text)
				{
					this.AddLoadMessage("The passwords must match.");
					return false;
				}

				// create the admin user...
				MembershipCreateStatus status;
				user = this.Get<MembershipProvider>().CreateUser(
						this.UserName.Text,
						this.Password1.Text,
						this.AdminEmail.Text,
						this.SecurityQuestion.Text,
						this.SecurityAnswer.Text,
						true,
						null,
						out status);
				if (status != MembershipCreateStatus.Success)
				{
                this.AddLoadMessage("Create Admin User Failed: {0}".FormatWith(this.GetMembershipErrorMessage(status)));
					return false;
				}
			}
			else
			{
				// try to get data for the existing user...
				user = UserMembershipHelper.GetUser(this.ExistingUserName.Text.Trim());

				if (user == null)
				{
					this.AddLoadMessage(
							"Existing user name is invalid and does not represent a current user in the membership store.");
					return false;
				}
			}

			try
			{
            string prefix = Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty;

				// add administrators and registered if they don't already exist...
				if (!RoleMembershipHelper.RoleExists("{0}Administrators".FormatWith(prefix)))
				{
					RoleMembershipHelper.CreateRole("{0}Administrators".FormatWith(prefix));
				}

				if (!RoleMembershipHelper.RoleExists("{0}Registered".FormatWith(prefix)))
				{
					RoleMembershipHelper.CreateRole("{0}Registered".FormatWith(prefix));
				}

				if (!RoleMembershipHelper.IsUserInRole(user.UserName, "{0}Administrators".FormatWith(prefix)))
				{
					RoleMembershipHelper.AddUserToRole(user.UserName, "{0}Administrators".FormatWith(prefix));
				}

				// logout administrator...
				FormsAuthentication.SignOut();
				DataTable cult = StaticDataHelper.Cultures();
				string langFile = "english.xml";
				foreach (DataRow drow in
                cult.Rows.Cast<DataRow>().Where(drow => drow["CultureTag"].ToString() == this.Culture.SelectedValue))
				{
					langFile = (string)drow["CultureFile"];
				}

				LegacyDb.system_initialize(
						this.TheForumName.Text,
						this.TimeZones.SelectedValue,
						this.Culture.SelectedValue,
						langFile,
						this.ForumEmailAddress.Text,
						string.Empty,
						user.UserName,
						user.Email,
						user.ProviderUserKey,
                Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

				LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);
				LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

				// vzrus: uncomment it to not keep install/upgrade objects in db for a place and better security
				// YAF.Classes.Data.DB.system_deleteinstallobjects();

				this.ImportData();
			}
			catch (Exception x)
			{
				this.AddLoadMessage(x.Message);
				return false;
			}

			return true;
		}

		/// <summary>
		/// The execute script.
		/// </summary>
		/// <param name="scriptFile">
		/// The script file.
		/// </param>
		/// <param name="useTransactions">
		/// The use transactions.
		/// </param>
		/// <exception cref="Exception">
		/// </exception>
		/// <exception cref="IOException"><c>IOException</c>.</exception>
		private void ExecuteScript([NotNull] string scriptFile, bool useTransactions)
		{
			string script;

			string fileName = this.Request.MapPath(scriptFile);

			try
			{
				using (var file = new StreamReader(fileName))
				{
          script = "{0}\r\n".FormatWith(file.ReadToEnd());

					file.Close();
				}
			}
			catch (FileNotFoundException)
			{
				return;
			}
			catch (Exception x)
			{
				throw new IOException("Failed to read {0}".FormatWith(fileName), x);
			}

			LegacyDb.system_initialize_executescripts(script, scriptFile, useTransactions);
		}

		/// <summary>
		/// The fill with connection strings.
		/// </summary>
		private void FillWithConnectionStrings()
		{
			if (this.lbConnections.Items.Count != 0)
			{
				return;
			}

			foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
			{
				this.lbConnections.Items.Add(connectionString.Name);
			}

			ListItem item = this.lbConnections.Items.FindByText("yafnet");

			if (item != null)
			{
				item.Selected = true;
			}
		}

		/// <summary>
		/// The fix access.
		/// </summary>
		/// <param name="bGrant">
		/// The b grant.
		/// </param>
		private void FixAccess(bool bGrant)
		{
			LegacyDb.system_initialize_fixaccess(bGrant);
		}

		/// <summary>
		/// The index of wizard id.
		/// </summary>
		/// <param name="id">
		/// The id.
		/// </param>
		/// <returns>
		/// The index of wizard id.
		/// </returns>
		private int IndexOfWizardID([NotNull] string id)
		{
			var step = this.InstallWizard.FindWizardControlRecursive(id) as WizardStepBase;

			if (step != null)
			{
				return this.InstallWizard.WizardSteps.IndexOf(step);
			}

			return -1;
		}

		/// <summary>
		/// The On PreRender event.
		/// </summary>
		/// <param name="e">
		/// the Event Arguments
		/// </param>
		protected override void OnPreRender(EventArgs e)
		{
			YafContext.Current.PageElements.RegisterJQueryUI(this.Page.Header);
			base.OnPreRender(e);
		}

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
		{
			if (this.IsPostBack)
			{
				return;
			}

			if (this.Session["InstallWizardFinal"] != null)
			{
				this.CurrentWizardStepID = "WizFinished";
				this.Session.Remove("InstallWizardFinal");
			}
			else
			{
				this.Cache["DBVersion"] = LegacyDb.GetDBVersion();

				this.CurrentWizardStepID = this.IsInstalled ? "WizEnterPassword" : "WizValidatePermission";

				// "WizCreatePassword"
				if (!this.IsInstalled)
				{
					// fake the board settings
					YafContext.Current.BoardSettings = new YafBoardSettings();
				}

				this.FullTextSupport.Visible = LegacyDb.FullTextSupported;

				this.TimeZones.DataSource = StaticDataHelper.TimeZones("english.xml");

				this.Culture.DataSource = StaticDataHelper.Cultures();
				this.Culture.DataValueField = "CultureTag";
				this.Culture.DataTextField = "CultureNativeName";

				this.DataBind();

				this.TimeZones.Items.FindByValue("0").Selected = true;
				if (this.Culture.Items.Count > 0)
				{
					this.Culture.Items.FindByValue("en-US").Selected = true;
				}

				Enumerable.Range(1, 20).Select(x => this.FindControlRecursiveAs<TextBox>(string.Format("Parameter{0}_Value", x))).Where(
					t => t != null).ForEach(c => c.Visible = false);

				Enumerable.Range(1, 20).Select(x => this.FindControlRecursiveAs<CheckBox>(string.Format("Parameter{0}_Value", x))).Where(
					t => t != null).ForEach(c => c.Visible = false);

				this.DBUsernamePasswordHolder.Visible = LegacyDb.PasswordPlaceholderVisible;

				foreach (var param in LegacyDb.DbUIParameters)
				{
					var label = this.FindControlRecursiveAs<Label>(string.Format("Parameter{0}_Name", param.ID));

					if (label != null)
					{
						label.Text = param.Label;
					}

					var textBox = this.FindControlRecursiveAs<TextBox>(string.Format("Parameter{0}_Value", param.ID));

					if (textBox != null)
					{
						textBox.Text = param.DefaultValue;
						textBox.Visible = param.Visible;					
					}

					var checkBox = this.FindControlRecursiveAs<CheckBox>(string.Format("Parameter{0}_Value", param.ID));

					if (checkBox != null)
					{
						checkBox.Text = param.Label;
						checkBox.Checked = param.DefaultValue.ToType<bool>();
						checkBox.Visible = param.Visible;
					}
				}

				// Hide New User on DNN
				if (Config.IsDotNetNuke)
				{
					UserChoice.SelectedIndex = 1;
					UserChoice.Items[0].Enabled = false;

					this.ExistingUserHolder.Visible = true;
					this.CreateAdminUserHolder.Visible = false;
				}
			}
		}

		/// <summary>
		/// The update database connection.
		/// </summary>
		/// <returns>
		/// </returns>
		private UpdateDBFailureType UpdateDatabaseConnection()
		{
			if (this.rblYAFDatabase.SelectedValue == "existing" && this.lbConnections.SelectedIndex >= 0)
			{
				string selectedConnection = this.lbConnections.SelectedValue;
				if (selectedConnection != Config.ConnectionStringName)
				{
					try
					{
						// have to write to the appSettings...
						if (!this._config.WriteAppSetting("YAF.ConnectionStringName", selectedConnection))
						{
							this.lblConnectionStringName.Text = selectedConnection;

							// failure to write App Settings..
							return UpdateDBFailureType.AppSettingsWrite;
						}
					}
					catch
					{
						return UpdateDBFailureType.AppSettingsWrite;
					}
				}
			}
			else if (this.rblYAFDatabase.SelectedValue == "create")
			{
				try
				{
					if (
							!this._config.WriteConnectionString(
									Config.ConnectionStringName, this.CurrentConnString, LegacyDb.ProviderAssemblyName))
					{
						// failure to write db Settings..
						return UpdateDBFailureType.ConnectionStringWrite;
					}
				}
				catch
				{
					return UpdateDBFailureType.ConnectionStringWrite;
				}
			}

			return UpdateDBFailureType.None;
		}

		/// <summary>
		/// The upgrade database.
		/// </summary>
		/// <param name="fullText">
		/// The full text.
		/// </param>
		/// <returns>
		/// The upgrade database.
		/// </returns>
		private bool UpgradeDatabase(bool fullText)
		{
			{
				// try
				this.FixAccess(false);

				foreach (string script in LegacyDb.ScriptList)
				{
					this.ExecuteScript(script, true);
				}

				this.FixAccess(true);

				int prevVersion = LegacyDb.GetDBVersion();

				LegacyDb.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

				// Ederon : 9/7/2007
				// resync all boards - necessary for propr last post bubbling
				LegacyDb.board_resync();

				// upgrade providers...
				Providers.Membership.DB.Current.UpgradeMembership(prevVersion, YafForumInfo.AppVersion);

				if (LegacyDb.GetIsForumInstalled() && prevVersion < 30)
				{
					ImportData();
				}

				if (LegacyDb.GetIsForumInstalled() && prevVersion < 42)
				{
					// un-html encode all topic subject names...
					LegacyDb.unencode_all_topics_subjects(t => Server.HtmlDecode(t));
				}

        if (LegacyDb.GetIsForumInstalled() && prevVersion < 49)
        {				
            // Reset The UserBox Template
            this.Get<YafBoardSettings>().UserBox = Constants.UserBox.DisplayTemplateDefault;
				    ((YafLoadBoardSettings)this.Get<YafBoardSettings>()).SaveRegistry();
			}

        // vzrus: uncomment it to not keep install/upgrade objects in DB and for better security 
        // DB.system_deleteinstallobjects();
      }
			/*catch ( Exception x )
{
	AddLoadMessage( x.Message );
	return false;
}*/

			// attempt to apply fulltext support if desired
			if (fullText && LegacyDb.FullTextSupported)
			{
				try
				{
					this.ExecuteScript(LegacyDb.FullTextScript, false);
				}
				catch (Exception x)
				{
					// just a warning...
            this.AddLoadMessage("Warning: FullText Support wasn't installed: {0}".FormatWith(x.Message));
				}
			}

			// run custom script...
			this.ExecuteScript("custom.sql", true);

			return true;
		}

		private void ImportData()
		{
			if (File.Exists(this.Request.MapPath(_BbcodeImport)))
			{
				// import into board...
				using (var bbcodeStream = new StreamReader(this.Request.MapPath(_BbcodeImport)))
				{
					this.Get<DataImport>().BBCodeExtensionImport(this.PageBoardID, bbcodeStream.BaseStream);
					bbcodeStream.Close();
				}
			}

			// load default extensions if available...
			if (File.Exists(this.Request.MapPath(_FileImport)))
			{
				// import into board...
				using (var fileExtStream = new StreamReader(this.Request.MapPath(_FileImport)))
				{
					this.Get<DataImport>().FileExtensionImport(this.PageBoardID, fileExtStream.BaseStream);
					fileExtStream.Close();
				}
			}

			// load default topic status if available...
			if (File.Exists(this.Request.MapPath(_TopicStatusImport)))
			{
				// import into board...
				using (var topicStatusStream = new StreamReader(this.Request.MapPath(_TopicStatusImport)))
				{
					this.Get<DataImport>().TopicStatusImport(this.PageBoardID, topicStatusStream.BaseStream);
					topicStatusStream.Close();
				}
			}
		}

		#endregion
	}
}
