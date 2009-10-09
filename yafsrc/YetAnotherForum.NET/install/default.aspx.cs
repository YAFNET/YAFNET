/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using YAF.Classes;
using YAF.Classes.Core;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Install
{
	/// <summary>
	/// Summary description for install.
	/// </summary>
	public partial class _default : System.Web.UI.Page
	{
		protected int _dbVersionBeforeUpgrade;
		private string _loadMessage = "";

		private const string _appPasswordKey = "YAF.ConfigPassword";

		private const string _bbcodeImport = "bbCodeExtensions.xml";
		private const string _fileImport = "fileExtensions.xml";

		private ConfigHelper _config = new ConfigHelper();

		#region Properties
		private bool IsInstalled
		{
			get
			{
				return !String.IsNullOrEmpty( _config.GetConfigValueAsString( _appPasswordKey ) );
			}
		}

		private int PageBoardID
		{
			get
			{
				try
				{
					return int.Parse( YAF.Classes.Config.BoardID );
				}
				catch
				{
					return 1;
				}
			}
		}

		private string CurrentWizardStepID
		{
			get
			{
				return InstallWizard.WizardSteps[InstallWizard.ActiveStepIndex].ID;
			}
			set
			{
				int index = IndexOfWizardID( value );
				if ( index >= 0 )
				{
					InstallWizard.ActiveStepIndex = index;
				}
			}
		}

		private string CurrentConnString
		{

			get
			{
				if ( rblYAFDatabase.SelectedValue == "existing" )
				{
					string connName = lbConnections.SelectedValue;

					if ( !String.IsNullOrEmpty( connName ) )
					{
						// pull from existing connection string...
						return ConfigurationManager.ConnectionStrings[connName].ConnectionString;
					}

					return string.Empty;
				}

				return YafDBAccess.GetConnectionString
						( Parameter1_Value.Text.Trim(),
						Parameter2_Value.Text.Trim(),
						Parameter3_Value.Text.Trim(),
						Parameter4_Value.Text.Trim(),
						Parameter5_Value.Text.Trim(),
						Parameter6_Value.Text.Trim(),
						Parameter7_Value.Text.Trim(),
						Parameter8_Value.Text.Trim(),
						Parameter9_Value.Text.Trim(),
						Parameter10_Value.Text.Trim(),
						Parameter11_Value.Checked,
						Parameter12_Value.Checked,
						Parameter13_Value.Checked,
						Parameter14_Value.Checked,
						Parameter15_Value.Checked,
						Parameter16_Value.Checked,
						Parameter17_Value.Checked,
						Parameter18_Value.Checked,
						Parameter19_Value.Checked,
						txtDBUserID.Text.Trim(),
						txtDBPassword.Text.Trim() );

			}
		}

		#endregion

		#region Event Handling
		protected void Page_Init( object sender, EventArgs e )
		{
			// set the connection manager to the dynamic...
			YafDBAccess.Current.SetConnectionManagerAdapter<YAF.Classes.Core.YafDynamicDBConnManager>();
		}

		private void Page_Load( object sender, System.EventArgs e )
		{
			YafContext.Current.PageElements.RegisterJQuery( Page.Header );

			if ( !IsPostBack )
			{
				if ( Session["InstallWizardFinal"] != null )
				{
					CurrentWizardStepID = "WizFinished";
					Session.Remove( "InstallWizardFinal" );
				}
				else
				{
					Cache["DBVersion"] = DB.DBVersion;

					CurrentWizardStepID = IsInstalled ? "WizEnterPassword" : "WizValidatePermission"; //"WizCreatePassword"

					if ( !IsInstalled )
					{
						// fake the board settings
						YafContext.Current.BoardSettings = new YafBoardSettings();
					}

					TimeZones.DataSource = StaticDataHelper.TimeZones( "english.xml" );
					DataBind();
					TimeZones.Items.FindByValue( "0" ).Selected = true;
					FullTextSupport.Visible = DB.FullTextSupported;

					DBUsernamePasswordHolder.Visible = DB.PasswordPlaceholderVisible;

					// Connection string parameters text boxes
					Parameter1_Name.Text = DB.Parameter1_Name;
					Parameter1_Value.Text = DB.Parameter1_Value;
					Parameter1_Value.Visible = DB.Parameter1_Visible;

					Parameter2_Name.Text = DB.Parameter2_Name;
					Parameter2_Value.Text = DB.Parameter2_Value;
					Parameter2_Value.Visible = DB.Parameter2_Visible;

					Parameter3_Name.Text = DB.Parameter3_Name;
					Parameter3_Value.Text = DB.Parameter3_Value;
					Parameter3_Value.Visible = DB.Parameter3_Visible;

					Parameter4_Name.Text = DB.Parameter4_Name;
					Parameter4_Value.Text = DB.Parameter4_Value;
					Parameter4_Value.Visible = DB.Parameter4_Visible;

					Parameter5_Name.Text = DB.Parameter5_Name;
					Parameter5_Value.Text = DB.Parameter5_Value;
					Parameter5_Value.Visible = DB.Parameter5_Visible;

					Parameter6_Name.Text = DB.Parameter6_Name;
					Parameter6_Value.Text = DB.Parameter6_Value;
					Parameter6_Value.Visible = DB.Parameter6_Visible;

					Parameter7_Name.Text = DB.Parameter7_Name;
					Parameter7_Value.Text = DB.Parameter7_Value;
					Parameter7_Value.Visible = DB.Parameter7_Visible;

					Parameter8_Name.Text = DB.Parameter8_Name;
					Parameter8_Value.Text = DB.Parameter8_Value;
					Parameter8_Value.Visible = DB.Parameter8_Visible;

					Parameter9_Name.Text = DB.Parameter9_Name;
					Parameter9_Value.Text = DB.Parameter9_Value;
					Parameter9_Value.Visible = DB.Parameter9_Visible;

					Parameter10_Name.Text = DB.Parameter10_Name;
					Parameter10_Value.Text = DB.Parameter10_Value;
					Parameter10_Value.Visible = DB.Parameter10_Visible;

					//  Connection string parameters  check boxes

					Parameter11_Value.Text = DB.Parameter11_Name;
					Parameter11_Value.Checked = DB.Parameter11_Value;
					Parameter11_Value.Visible = DB.Parameter11_Visible;

					Parameter12_Value.Text = DB.Parameter12_Name;
					Parameter12_Value.Checked = DB.Parameter12_Value;
					Parameter12_Value.Visible = DB.Parameter12_Visible;

					Parameter13_Value.Text = DB.Parameter13_Name;
					Parameter13_Value.Checked = DB.Parameter13_Value;
					Parameter13_Value.Visible = DB.Parameter13_Visible;

					Parameter14_Value.Text = DB.Parameter14_Name;
					Parameter14_Value.Checked = DB.Parameter14_Value;
					Parameter14_Value.Visible = DB.Parameter14_Visible;

					Parameter15_Value.Text = DB.Parameter15_Name;
					Parameter15_Value.Checked = DB.Parameter15_Value;
					Parameter15_Value.Visible = DB.Parameter15_Visible;

					Parameter16_Value.Text = DB.Parameter16_Name;
					Parameter16_Value.Checked = DB.Parameter16_Value;
					Parameter16_Value.Visible = DB.Parameter16_Visible;

					Parameter17_Value.Text = DB.Parameter17_Name;
					Parameter17_Value.Checked = DB.Parameter17_Value;
					Parameter17_Value.Visible = DB.Parameter17_Visible;

					Parameter18_Value.Text = DB.Parameter18_Name;
					Parameter18_Value.Checked = DB.Parameter18_Value;
					Parameter18_Value.Visible = DB.Parameter18_Visible;

					Parameter19_Value.Text = DB.Parameter19_Name;
					Parameter19_Value.Checked = DB.Parameter19_Value;
					Parameter19_Value.Visible = DB.Parameter19_Visible;
				}
			}
		}

		protected void UserChoice_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( UserChoice.SelectedValue == "create" )
			{
				ExistingUserHolder.Visible = false;
				CreateAdminUserHolder.Visible = true;
			}
			else if ( UserChoice.SelectedValue == "existing" )
			{
				ExistingUserHolder.Visible = true;
				CreateAdminUserHolder.Visible = false;
			}
		}

		protected void Wizard_FinishButtonClick( object sender, WizardNavigationEventArgs e )
		{
			// reset the board settings...
			YafContext.Current.BoardSettings = null;

			if ( YAF.Classes.Config.IsDotNetNuke )
			{
				//Redirect back to the portal main page.
				string rPath = YafForumInfo.ForumRoot;
				int pos = rPath.IndexOf( "/", 2 );
				rPath = rPath.Substring( 0, pos );
				Response.Redirect( rPath );
			}
			else
				Response.Redirect( "~/" );
		}

		protected void Wizard_PreviousButtonClick( object sender, WizardNavigationEventArgs e )
		{

			if ( CurrentWizardStepID == "WizTestSettings" )
			{
				CurrentWizardStepID = "WizDatabaseConnection";
			}

			e.Cancel = false;

			//// go back only from last step (to user/roles migration)
			//if ( e.CurrentStepIndex == ( InstallWizard.WizardSteps.Count - 1 ) )
			//  InstallWizard.MoveTo( InstallWizard.WizardSteps[e.CurrentStepIndex - 1] );
			//else
			//  // othwerise cancel action
			//  e.Cancel = true;
		}

		protected void Wizard_ActiveStepChanged( object sender, EventArgs e )
		{
			bool previousVisible = false;

			switch ( CurrentWizardStepID )
			{
				case "WizCreatePassword":
					if ( _config.AppSettings != null ) lblConfigPasswordAppSettingFile.Text = _config.AppSettings.File;
					if ( IsInstalled )
					{
						// no need for this setup if IsInstalled...
						InstallWizard.ActiveStepIndex++;
					}
					break;
				case "WizCreateForum":
					if ( DB.IsForumInstalled )
					{
						InstallWizard.ActiveStepIndex++;
					}
					break;
				case "WizMigrateUsers":
					if ( !IsInstalled )
					{
						// no migration because it's a new install...
						CurrentWizardStepID = "WizFinished";
					}
					else
					{
						object version = Cache["DBVersion"] ?? DB.DBVersion;

						if ( ( (int)version ) >= 30 || ( (int)version ) == -1 )
						{
							// migration is NOT needed...
							CurrentWizardStepID = "WizFinished";
						}

						Cache.Remove( "DBVersion" );
					}
					// get user count
					if ( CurrentWizardStepID == "WizMigrateUsers" ) lblMigrateUsersCount.Text = DB.user_list( PageBoardID, null, true ).Rows.Count.ToString();
					break;
				case "WizDatabaseConnection":
					previousVisible = true;
					// fill with connection strings...
					lblConnStringAppSettingName.Text = Config.ConnectionStringName;
					FillWithConnectionStrings();
					break;
				case "WizManualDatabaseConnection":
					if ( _config.AppSettings != null ) lblAppSettingsFile.Text = _config.AppSettings.File;
					previousVisible = true;
					break;
				case "WizManuallySetPassword":
					if ( _config.AppSettings != null ) lblAppSettingsFile2.Text = _config.AppSettings.File;
					break;
				case "WizTestSettings":
					previousVisible = true;
					break;
				case "WizValidatePermission":
					//ValidatePermissionStep();
					break;
				case "WizMigratingUsers":
					// disable the next button...
					Button btnNext = ControlHelper.FindControlAs<Button>( InstallWizard, "StepNavigationTemplateContainerID$StepNextButton" );
					if ( btnNext != null )
						btnNext.Enabled = false;
					break;
			}

			Button btnPrevious = ControlHelper.FindControlAs<Button>( InstallWizard, "StepNavigationTemplateContainerID$StepPreviousButton" );

			if ( btnPrevious != null )
				btnPrevious.Visible = previousVisible;
		}

		protected void Wizard_NextButtonClick( object sender, WizardNavigationEventArgs e )
		{
			e.Cancel = true;

			switch ( CurrentWizardStepID )
			{
				case "WizValidatePermission":
					e.Cancel = false;
					break;
				case "WizDatabaseConnection":
					// save the database settings...
					UpdateDBFailureType type = UpdateDatabaseConnection();
					e.Cancel = false;

					if ( type == UpdateDBFailureType.None )
					{
						// jump to test settings...
						CurrentWizardStepID = "WizTestSettings";
					}
					else
					{
						// failure -- show the next section but specify which errors...
						if ( type == UpdateDBFailureType.AppSettingsWrite )
						{
							NoWriteAppSettingsHolder.Visible = true;
						}
						else if ( type == UpdateDBFailureType.ConnectionStringWrite )
						{
							NoWriteDBSettingsHolder.Visible = true;
							lblDBConnStringName.Text = Config.ConnectionStringName;
							lblDBConnStringValue.Text = CurrentConnString;
						}
					}
					break;
				case "WizManualDatabaseConnection":
					e.Cancel = false;
					break;
				case "WizCreatePassword":
					if ( txtCreatePassword1.Text.Trim() == string.Empty )
					{
						AddLoadMessage( "Please enter a configuration password." );
						break;
					}

					if ( txtCreatePassword2.Text != txtCreatePassword1.Text )
					{
						AddLoadMessage( "Verification is not the same as your password." );
						break;
					}

					e.Cancel = false;

					if ( _config.WriteAppSetting( _appPasswordKey, txtCreatePassword1.Text ) )
					{
						// advance to the testing section since the password is now set...
						CurrentWizardStepID = "WizDatabaseConnection";
					}
					break;
				case "WizManuallySetPassword":
					if ( IsInstalled ) e.Cancel = false;
					else
					{
						AddLoadMessage( "You must update your appSettings with the YAF.ConfigPassword Key to continue. NOTE: The key name is case sensitive." );
					}
					break;
				case "WizTestSettings":
					e.Cancel = false;
					break;
				case "WizEnterPassword":
					if ( _config.GetConfigValueAsString( "YAF.ConfigPassword" ) ==
							 FormsAuthentication.HashPasswordForStoringInConfigFile( txtEnteredPassword.Text, "md5" ) ||
							 _config.GetConfigValueAsString( "YAF.ConfigPassword" ) == txtEnteredPassword.Text.Trim() )
					{
						e.Cancel = false;
						// move to test settings...
						CurrentWizardStepID = "WizTestSettings";
					}
					else
						AddLoadMessage( "Wrong password!" );
					break;
				case "WizCreateForum":
					if ( CreateForum() )
						e.Cancel = false;
					break;
				case "WizInitDatabase":
					if ( UpgradeDatabase( FullTextSupport.Checked ) )
						e.Cancel = false;
					break;
				case "WizMigrateUsers":
					// migrate users/roles only if user does not want to skip
					if ( !skipMigration.Checked )
					{
						RoleMembershipHelper.SyncRoles( PageBoardID );
						// start the background migration task...
						MigrateUsersTask.Start( PageBoardID );
					}
					e.Cancel = false;
					break;
				case "WizFinished":
					break;
				default:
					throw new ApplicationException( "Installation Wizard step not handled: " + InstallWizard.WizardSteps[e.CurrentStepIndex].ID );
			}
		}

		protected void rblYAFDatabase_SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( rblYAFDatabase.SelectedValue == "create" )
			{
				ExistingConnectionHolder.Visible = false;
				NewConnectionHolder.Visible = true;
			}
			else if ( rblYAFDatabase.SelectedValue == "existing" )
			{
				ExistingConnectionHolder.Visible = true;
				NewConnectionHolder.Visible = false;
			}
		}

		protected void btnTestDBConnection_Click( object sender, EventArgs e )
		{
			// attempt to connect selected DB...
			YafContext.Current["ConnectionString"] = CurrentConnString;
			string message;

			if ( !TestDatabaseConnection( out message ) )
			{
				UpdateInfoPanel( ConnectionInfoHolder, lblConnectionDetails, "Failed to connect:<br/><br/>" + message, "errorinfo" );
			}
			else
			{
				UpdateInfoPanel( ConnectionInfoHolder, lblConnectionDetails, "Connection Succeeded", "successinfo" );
			}

			// we're done with it...
			YafContext.Current.Vars.Remove( "ConnectionString" );
		}

		protected void btnTestDBConnectionManual_Click( object sender, EventArgs e )
		{
			// attempt to connect DB...
			string message;

			if ( !TestDatabaseConnection( out message ) )
			{
				UpdateInfoPanel( ManualConnectionInfoHolder, lblConnectionDetailsManual, "Failed to connect:<br/><br/>" + message, "errorinfo" );
			}
			else
			{
				UpdateInfoPanel( ManualConnectionInfoHolder, lblConnectionDetailsManual, "Connection Succeeded", "successinfo" );
			}

		}

		protected void Parameter11_Value_CheckChanged( object sender, EventArgs e )
		{
			DBUsernamePasswordHolder.Visible = !Parameter11_Value.Checked;
		}

		protected void btnTestPermissions_Click( object sender, EventArgs e )
		{
			UpdateStatusLabel( lblPermissionApp, 1 );
			UpdateStatusLabel( lblPermissionUpload, 1 );

			UpdateStatusLabel( lblPermissionApp, DirectoryHasWritePermission( Server.MapPath( "~/" ) ) ? 2 : 0 );
			UpdateStatusLabel( lblPermissionUpload, DirectoryHasWritePermission( Server.MapPath( Config.UploadDir ) ) ? 2 : 0 );
		}

		protected void btnTestSmtp_Click( object sender, EventArgs e )
		{
			try
			{
				YafServices.SendMail.Send( txtTestFromEmail.Text.Trim(), txtTestToEmail.Text.Trim(), "Test Email From Yet Another Forum.NET", "The email sending appears to be working from your YAF installation." );
				// success
				UpdateInfoPanel( SmtpInfoHolder, lblSmtpTestDetails, "Mail Sent. Verify it's received at your entered email address.", "successinfo" );
			}
			catch ( Exception x )
			{
				UpdateInfoPanel( SmtpInfoHolder, lblSmtpTestDetails, "Failed to connect:<br/><br/>" + x.Message, "errorinfo" );
			}
		}

		protected void UpdateStatusTimer_Tick( object sender, EventArgs e )
		{
			// see if the migration is done....
			if ( YafTaskModule.Current.TaskManager.ContainsKey( MigrateUsersTask.TaskName ) && YafTaskModule.Current.TaskManager[MigrateUsersTask.TaskName].IsRunning )
			{
				// proceed...
				return;
			}

			if ( Session["InstallWizardFinal"] == null )
				Session.Add( "InstallWizardFinal", true );

			// done here...
			Response.Redirect( "default.aspx" );
		}


		#endregion

		#region Event Helper Functions
		/// <summary>
		/// Tests database connection. Can probably be moved to DB class.
		/// </summary>
		/// <param name="exceptionMessage"></param>
		/// <returns></returns>       

		private static bool TestDatabaseConnection( out string exceptionMessage )
		{
			return YafDBAccess.TestConnection( out exceptionMessage );
		}

		enum UpdateDBFailureType
		{
			None,
			AppSettingsWrite,
			ConnectionStringWrite
		}

		private UpdateDBFailureType UpdateDatabaseConnection()
		{
			if ( rblYAFDatabase.SelectedValue == "existing" && lbConnections.SelectedIndex >= 0 )
			{
				string selectedConnection = lbConnections.SelectedValue;
				if ( selectedConnection != Config.ConnectionStringName )
				{
					// have to write to the appSettings...
					if ( !_config.WriteAppSetting( "YAF.ConnectionStringName", selectedConnection ) )
					{
						lblConnectionStringName.Text = selectedConnection;
						// failure to write App Settings..
						return UpdateDBFailureType.AppSettingsWrite;
					}
				}
			}
			else if ( rblYAFDatabase.SelectedValue == "create" )
			{
				if ( !_config.WriteConnectionString( Config.ConnectionStringName, CurrentConnString, DB.ProviderAssemblyName ) )
				{
					// failure to write db Settings..
					return UpdateDBFailureType.ConnectionStringWrite;
				}
			}

			return UpdateDBFailureType.None;
		}

		private static void UpdateInfoPanel( Control infoHolder, Label detailsLabel, string info, string cssClass )
		{
			infoHolder.Visible = true;
			detailsLabel.Text = info;
			detailsLabel.CssClass = cssClass;
		}

		private static void UpdateStatusLabel( Label theLabel, int status )
		{
			switch ( status )
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

		private void FillWithConnectionStrings()
		{
			if ( lbConnections.Items.Count == 0 )
			{
				foreach ( ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings )
				{
					lbConnections.Items.Add( connectionString.Name );
				}

				ListItem item = lbConnections.Items.FindByText( "yafnet" );
				if ( item != null )
				{
					item.Selected = true;
				}
			}
		}

		/// <summary>
		/// Validates write permission in a specific directory. Should be moved to YAF.Classes.Utils.
		/// </summary>
		/// <param name="directory"></param>
		/// <returns></returns>
		private static bool DirectoryHasWritePermission( string directory )
		{
			bool hasWriteAccess = false;

			try
			{
				// see if we have permission
				FileIOPermission fp = new FileIOPermission( FileIOPermissionAccess.Write, directory );
				fp.Demand();

				hasWriteAccess = true;
			}
			catch
			{
				hasWriteAccess = false;
			}

			return hasWriteAccess;
		}

		private bool UpgradeDatabase( bool fullText )
		{
			//try
			{
				FixAccess( false );

				foreach ( string script in DB.ScriptList )
				{
					ExecuteScript( script, true );
				}

				FixAccess( true );

				int prevVersion = DB.DBVersion;

				DB.system_updateversion( YafForumInfo.AppVersion, YafForumInfo.AppVersionName );

				// Ederon : 9/7/2007
				// resync all boards - necessary for propr last post bubbling
				DB.board_resync();

				// upgrade providers...
				YAF.Providers.Membership.DB.Current.UpgradeMembership( prevVersion, YafForumInfo.AppVersion );

				if ( DB.IsForumInstalled && prevVersion < 30 )
				{
					// load default bbcode if available...
					if ( File.Exists( Request.MapPath( _bbcodeImport ) ) )
					{
						// import into board...
						using ( StreamReader bbcodeStream = new StreamReader( Request.MapPath( _bbcodeImport ) ) )
						{
							YAF.Classes.Data.Import.DataImport.BBCodeExtensionImport( PageBoardID, bbcodeStream.BaseStream );
							bbcodeStream.Close();
						}
					}

					// load default extensions if available...
					if ( File.Exists( Request.MapPath( _fileImport ) ) )
					{
						// import into board...
						using ( StreamReader fileExtStream = new StreamReader( Request.MapPath( _fileImport ) ) )
						{
							YAF.Classes.Data.Import.DataImport.FileExtensionImport( PageBoardID, fileExtStream.BaseStream );
							fileExtStream.Close();
						}
					}
				}

				//vzrus: uncomment it to not keep install/upgrade objects in DB and for better security 
				//DB.system_deleteinstallobjects();
			}
			/*catch ( Exception x )
			{
				AddLoadMessage( x.Message );
				return false;
			}*/

			// attempt to apply fulltext support if desired
			if ( fullText && DB.FullTextSupported )
			{
				try
				{
					ExecuteScript( DB.FullTextScript, false );
				}
				catch ( Exception x )
				{
					// just a warning...
					AddLoadMessage( "Warning: FullText Support wasn't installed: " + x.Message );
				}
			}

			return true;
		}

		private bool CreateForum()
		{
			if ( DB.IsForumInstalled )
			{
				AddLoadMessage( "Forum is already installed." );
				return false;
			}
			if ( TheForumName.Text.Length == 0 )
			{
				AddLoadMessage( "You must enter a forum name." );
				return false;
			}
			if ( ForumEmailAddress.Text.Length == 0 )
			{
				AddLoadMessage( "You must enter a forum email address." );
				return false;
			}

			MembershipUser user = null;

			if ( UserChoice.SelectedValue == "create" )
			{
				if ( UserName.Text.Length == 0 )
				{
					AddLoadMessage( "You must enter the admin user name," );
					return false;
				}
				if ( AdminEmail.Text.Length == 0 )
				{
					AddLoadMessage( "You must enter the administrators email address." );
					return false;
				}
				if ( Password1.Text.Length == 0 )
				{
					AddLoadMessage( "You must enter a password." );
					return false;
				}
				if ( Password1.Text != Password2.Text )
				{
					AddLoadMessage( "The passwords must match." );
					return false;
				}

				// create the admin user...
				MembershipCreateStatus status;
				user = YafContext.Current.CurrentMembership.CreateUser( UserName.Text, Password1.Text, AdminEmail.Text, SecurityQuestion.Text, SecurityAnswer.Text, true, null, out status );
				if ( status != MembershipCreateStatus.Success )
				{
					AddLoadMessage( string.Format( "Create Admin User Failed: {0}", GetMembershipErrorMessage( status ) ) );
					return false;
				}
			}
			else
			{
				// try to get data for the existing user...
				user = UserMembershipHelper.GetUser( ExistingUserName.Text.Trim() );

				if ( user == null )
				{
					AddLoadMessage( "Existing user name is invalid and does not represent a current user in the membership store." );
					return false;
				}
			}

			try
			{
				// add administrators and registered if they don't already exist...
				if ( !RoleMembershipHelper.RoleExists( "Administrators" ) )
				{
					RoleMembershipHelper.CreateRole( "Administrators" );
				}
				if ( !RoleMembershipHelper.RoleExists( "Registered" ) )
				{
					RoleMembershipHelper.CreateRole( "Registered" );
				}
				if ( !RoleMembershipHelper.IsUserInRole( user.UserName, "Administrators" ) )
				{
					RoleMembershipHelper.AddUserToRole( user.UserName, "Administrators" );
				}

				// logout administrator...
				FormsAuthentication.SignOut();
				YAF.Classes.Data.DB.system_initialize( TheForumName.Text, TimeZones.SelectedItem.Value, ForumEmailAddress.Text, "", user.UserName, user.Email, user.ProviderUserKey );
				YAF.Classes.Data.DB.system_updateversion( YafForumInfo.AppVersion, YafForumInfo.AppVersionName ); DB.system_updateversion( YafForumInfo.AppVersion, YafForumInfo.AppVersionName );
				//vzrus: uncomment it to not keep install/upgrade objects in db for a place and better security
				//YAF.Classes.Data.DB.system_deleteinstallobjects();
				// load default bbcode if available...
				if ( File.Exists( Request.MapPath( _bbcodeImport ) ) )
				{
					// import into board...
					using ( StreamReader bbcodeStream = new StreamReader( Request.MapPath( _bbcodeImport ) ) )
					{
						YAF.Classes.Data.Import.DataImport.BBCodeExtensionImport( PageBoardID, bbcodeStream.BaseStream );
						bbcodeStream.Close();
					}
				}

				// load default extensions if available...
				if ( File.Exists( Request.MapPath( _fileImport ) ) )
				{
					// import into board...
					using ( StreamReader fileExtStream = new StreamReader( Request.MapPath( _fileImport ) ) )
					{
						YAF.Classes.Data.Import.DataImport.FileExtensionImport( PageBoardID, fileExtStream.BaseStream );
						fileExtStream.Close();
					}
				}
			}
			catch ( Exception x )
			{
				AddLoadMessage( x.Message );
				return false;
			}
			return true;
		}

		private int IndexOfWizardID( string id )
		{
			WizardStepBase step = ControlHelper.FindWizardControlRecursive( InstallWizard, id ) as WizardStepBase;

			if ( step != null )
			{
				return InstallWizard.WizardSteps.IndexOf( step );
			}

			return -1;
		}

		#endregion

		#region General Helper Functions
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			base.Render( writer );
			if ( _loadMessage != "" )
			{
				writer.WriteLine( "<script language='javascript'>" );
				writer.WriteLine( "onload = function() {" );
				writer.WriteLine( "	alert('{0}');", _loadMessage );
				writer.WriteLine( "}" );
				writer.WriteLine( "</script>" );
			}
		}

		public string GetMembershipErrorMessage( MembershipCreateStatus status )
		{
			switch ( status )
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
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}

		void AddLoadMessage( string msg )
		{
			msg = msg.Replace( "\\", "\\\\" );
			msg = msg.Replace( "'", "\\'" );
			msg = msg.Replace( "\r\n", "\\r\\n" );
			msg = msg.Replace( "\n", "\\n" );
			msg = msg.Replace( "\"", "\\\"" );
			_loadMessage += msg + "\\n\\n";
		}
		#endregion

		#region DB Helper Functions
		private void ExecuteScript( string scriptFile, bool useTransactions )
		{
			string script = null;
			try
			{
				using ( System.IO.StreamReader file = new System.IO.StreamReader( Request.MapPath( scriptFile ) ) )
				{
					script = file.ReadToEnd() + "\r\n";

					file.Close();
				}
			}
			catch ( System.IO.FileNotFoundException )
			{
				return;
			}
			catch ( Exception x )
			{
				throw new Exception( "Failed to read " + scriptFile, x );
			}

			DB.system_initialize_executescripts( script, scriptFile, useTransactions );
		}

		private void FixAccess( bool bGrant )
		{
			DB.system_initialize_fixaccess( bGrant );
		}
		#endregion
	}
}
