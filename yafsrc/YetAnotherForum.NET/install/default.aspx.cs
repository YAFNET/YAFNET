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
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Security;
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

		private const string _bbcodeImport = "bbCodeExtensions.xml";
		private const string _fileImport = "fileExtensions.xml";

		#region Properties
		private bool IsInstalled
		{
			get
			{
				return !String.IsNullOrEmpty( Config.GetConfigValueAsString( "YAF.configPassword" ) );
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
					InstallWizard.MoveTo( InstallWizard.WizardSteps[index] );
				}
			}
		}

		private Configuration _webConfig = null;
		private Configuration WebConfig
		{
			get
			{
				if ( _webConfig == null )
					_webConfig = WebConfigurationManager.OpenWebConfiguration( "~/" );

				return _webConfig;
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

			if ( !IsPostBack )
			{
				Cache["DBVersion"] = DB.DBVersion;

				CurrentWizardStepID = IsInstalled ? "WizEnterPassword" : "WizCreatePassword";//"WizDatabaseConnection";

				if ( !IsInstalled )
				{
					// fake the board settings
					YafContext.Current.BoardSettings = new YafBoardSettings();
				}

				TimeZones.DataSource = StaticDataHelper.TimeZones( "english.xml" );

				DataBind();

				TimeZones.Items.FindByValue( "0" ).Selected = true;

				// see if we need to do migration again...

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

		protected void Password_Postback( object sender, System.EventArgs e )
		{
			// prepare even arguments for calling Wizard_NextButtonClick event handler
			WizardNavigationEventArgs eventArgs = new WizardNavigationEventArgs( InstallWizard.ActiveStepIndex, InstallWizard.ActiveStepIndex + 1 );

			// call it
			Wizard_NextButtonClick( sender, eventArgs );

			// move to next step only if it wasn't cancelled within next button event handler
			if ( !eventArgs.Cancel ) InstallWizard.MoveTo( InstallWizard.WizardSteps[eventArgs.NextStepIndex] );
		}

		protected void Wizard_FinishButtonClick( object sender, WizardNavigationEventArgs e )
		{
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
			// go back only from last step (to user/roles migration)
			if ( e.CurrentStepIndex == ( InstallWizard.WizardSteps.Count - 1 ) )
				InstallWizard.MoveTo( InstallWizard.WizardSteps[e.CurrentStepIndex - 1] );
			else
				// othwerise cancel action
				e.Cancel = true;
		}

		protected void Wizard_ActiveStepChanged( object sender, EventArgs e )
		{
			if ( CurrentWizardStepID == "WizCreatePassword" && !IsInstalled )
			{
				InstallWizard.ActiveStepIndex++;
			}
			else if ( CurrentWizardStepID == "WizCreateForum" && DB.IsForumInstalled )
			{
				InstallWizard.ActiveStepIndex++;
			}
			else if ( CurrentWizardStepID == "WizMigrateUsers" )
			{
				if ( !IsInstalled )
				{
					// no migration because it's a new install...
					InstallWizard.ActiveStepIndex++;
				}
				else
				{
					object version = Cache["DBVersion"] ?? DB.DBVersion;

					if ( ( (int)version ) >= 30 || ( (int)version ) == -1 )
					{
						// migration is NOT needed...
						InstallWizard.ActiveStepIndex++;
					}

					Cache.Remove( "DBVersion" );
				}
			}
		}

		protected void Wizard_Load( object sender, EventArgs e )
		{
			switch ( CurrentWizardStepID )
			{
				case "WizDatabaseConnection":
					// fill with connection strings...
					FillWithConnectionStrings();
					break;
				case "WizValidatePermission":
					//ValidatePermissionStep();
					break;
			}
		}

		protected void Wizard_NextButtonClick( object sender, WizardNavigationEventArgs e )
		{
			e.Cancel = true;

			switch ( InstallWizard.WizardSteps[e.CurrentStepIndex].ID )
			{
				case "WizDatabaseConnection":
					e.Cancel = false;
					break;
				case "WizCreatePassword":
					if ( TextBox1.Text == string.Empty )
					{
						AddLoadMessage( "Missing configuration password." );
						break;
					}

					if ( TextBox2.Text != TextBox1.Text )
					{
						AddLoadMessage( "Password not verified." );
						break;
					}

					try
					{
						AppSettingsSection appSettings = WebConfig.GetSection( "appSettings" ) as AppSettingsSection;

						if ( appSettings != null )
						{
							if ( appSettings.Settings["YAF.ConfigPassword"] != null )
							{
								appSettings.Settings.Remove( "YAF.ConfigPassword" );
							}

							appSettings.Settings.Add( "YAF.ConfigPassword", TextBox1.Text );
						}

						WebConfig.Save( ConfigurationSaveMode.Modified );
						e.Cancel = false;
					}
					catch
					{
						// just a warning now...
						throw new Exception(
							"Cannot save the YAF.ConfigPassword to the app.config file. Please verify that the ASPNET user has write access permissions to the app.config file. Or modify the app.config \"YAF.ConfigPassword\" key with a plaintext password and try again." );
					}

					break;
				case "WizEnterPassword":
					if ( Config.GetConfigValueAsString( "YAF.ConfigPassword" ) ==
							 FormsAuthentication.HashPasswordForStoringInConfigFile( TextBox3.Text, "md5" ) ||
							 Config.GetConfigValueAsString( "YAF.ConfigPassword" ) == TextBox3.Text.Trim() )
						e.Cancel = false;
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
						RoleMembershipHelper.SyncUsers( PageBoardID );
					}
					e.Cancel = false;
					break;
				case "WizFinished":
					YafContext.Current.BoardSettings = null;
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

			try
			{
				using ( YafDBConnManager connection = YafDBAccess.Current.GetConnectionManager())
				{
					// attempt to connect to the db...
					SqlConnection conn = connection.OpenDBConnection;
				}

				// success
				UpdateConnectionInfo( "Connection Succeeded", "successinfo" );
			}
			catch ( Exception x )
			{
				// unable to connect...
				UpdateConnectionInfo( "Failed to connect:<br/><br/>" + x.Message, "errorinfo" );
			}
		}

		protected void chkDBIntegratedSecurity_CheckChanged( object sender, EventArgs e )
		{
			DBUsernamePasswordHolder.Visible = !chkDBIntegratedSecurity.Checked;
		}

		#endregion

		#region Event Helper Functions
		private bool UpdateDatabaseConnection()
		{
			if ( rblYAFDatabase.SelectedValue == "existing" && lbConnections.SelectedIndex >= 0 )
			{
				string selectedConnection = lbConnections.SelectedValue;

				if ( selectedConnection != "yafnet" )
				{
					// make sure permissions allow modifying the AppSettings...
					if ( !HasAppSettingWritePermission() )
					{
						NoWriteAppSettingsHolder.Visible = true;
						AppSettingsSection appSettings = WebConfig.GetSection( "appSettings" ) as AppSettingsSection;
						if ( appSettings != null ) lblAppSettingsFile.Text = appSettings.File;
						lblConnectionStringName.Text = selectedConnection;
					}
				}
			}

			return false;
		}

		private void UpdateConnectionInfo(string info, string cssClass)
		{
			ConnectionInfoHolder.Visible = true;
			lblConnectionDetails.Text = info;
			lblConnectionDetails.CssClass = cssClass;
		}

		private void FillWithConnectionStrings()
		{
			if ( lbConnections.Items.Count == 0 )
			{
				foreach ( ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings )
				{
					lbConnections.Items.Add( connectionString.Name );
				}
			}
		}

		private string CurrentConnString
		{
			get
			{
				SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();

				if ( rblYAFDatabase.SelectedValue == "existing" )
				{
					string connName = lbConnections.SelectedValue;

					if ( !String.IsNullOrEmpty( connName ))
					{
						// pull from existing connection string...
						return ConfigurationManager.ConnectionStrings[connName].ConnectionString;
					}

					return string.Empty;
				}

				connBuilder.DataSource = txtDBDataSource.Text.Trim();
				connBuilder.InitialCatalog = txtDBInitialCatalog.Text.Trim();

				if ( chkDBIntegratedSecurity.Checked )
				{
					connBuilder.IntegratedSecurity = chkDBIntegratedSecurity.Checked;
				}
				else
				{
					connBuilder.UserID = txtDBUserID.Text.Trim();
					connBuilder.Password = txtDBPassword.Text.Trim();
				}

				return connBuilder.ConnectionString;
			}
		}

		private bool HasConnectionStringWritePermission()
		{
			ConnectionStringsSection connStrings = WebConfig.GetSection( "ConnectionStrings" ) as ConnectionStringsSection;
			const string testKeyStr = "YAF.TestString";

			if ( connStrings == null )
			{
				return false;
			}

			bool hasWriteAccess = false;
			try
			{
				if ( connStrings.ConnectionStrings[testKeyStr] != null )
				{
					connStrings.ConnectionStrings.Remove( testKeyStr );
				}

				connStrings.ConnectionStrings.Add( new ConnectionStringSettings(testKeyStr,"TestConnectionString;") );

				WebConfig.Save( ConfigurationSaveMode.Modified );

				// remove the test key immediately...
				if ( connStrings.ConnectionStrings[testKeyStr] != null )
				{
					connStrings.ConnectionStrings.Remove( testKeyStr );
				}

				WebConfig.Save( ConfigurationSaveMode.Modified );

				hasWriteAccess = true;
			}
			catch
			{
				hasWriteAccess = false;
			}

			return hasWriteAccess;
		}

		private bool HasAppSettingWritePermission()
		{
			AppSettingsSection appSettings = WebConfig.GetSection( "appSettings" ) as AppSettingsSection;
			const string testKeyStr = "YAF.TestWrite";

			if ( appSettings == null )
			{
				return false;
			}

			bool hasWriteAccess = false;
			try
			{
				if ( appSettings.Settings[testKeyStr] != null )
				{
					appSettings.Settings.Remove( testKeyStr );
				}

				appSettings.Settings.Add( testKeyStr, "TestKey" );

				WebConfig.Save( ConfigurationSaveMode.Modified );

				// remove the test key immediately...
				if ( appSettings.Settings[testKeyStr] != null )
				{
					appSettings.Settings.Remove( testKeyStr );
				}

				WebConfig.Save( ConfigurationSaveMode.Modified );

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
				YAF.Providers.Membership.DB.UpgradeMembership( prevVersion, YafForumInfo.AppVersion );

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
			WizardStepBase step = InstallWizard.FindControl( id ) as WizardStepBase;

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
