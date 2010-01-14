/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System;
  using System.Configuration;
  using System.Drawing;
  using System.IO;
  using System.Security.Permissions;
  using System.Web;
  using System.Web.Security;
  using System.Web.UI;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Data.Import;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for install.
  /// </summary>
  public partial class _default : Page
  {
    /// <summary>
    /// The _app password key.
    /// </summary>
    private const string _appPasswordKey = "YAF.ConfigPassword";

    /// <summary>
    /// The _bbcode import.
    /// </summary>
    private const string _bbcodeImport = "bbCodeExtensions.xml";

    /// <summary>
    /// The _file import.
    /// </summary>
    private const string _fileImport = "fileExtensions.xml";

    /// <summary>
    /// The _config.
    /// </summary>
    private ConfigHelper _config = new ConfigHelper();

    /// <summary>
    /// The _db version before upgrade.
    /// </summary>
    protected int _dbVersionBeforeUpgrade;

    /// <summary>
    /// The _load message.
    /// </summary>
    private string _loadMessage = string.Empty;

    #region Properties

    /// <summary>
    /// Gets a value indicating whether IsInstalled.
    /// </summary>
    private bool IsInstalled
    {
      get
      {
        return !String.IsNullOrEmpty(this._config.GetConfigValueAsString(_appPasswordKey));
      }
    }

    /// <summary>
    /// Gets PageBoardID.
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

    /// <summary>
    /// Gets or sets CurrentWizardStepID.
    /// </summary>
    private string CurrentWizardStepID
    {
      get
      {
        return this.InstallWizard.WizardSteps[this.InstallWizard.ActiveStepIndex].ID;
      }

      set
      {
        int index = IndexOfWizardID(value);
        if (index >= 0)
        {
          this.InstallWizard.ActiveStepIndex = index;
        }
      }
    }

    /// <summary>
    /// Gets CurrentConnString.
    /// </summary>
    private string CurrentConnString
    {
      get
      {
        if (this.rblYAFDatabase.SelectedValue == "existing")
        {
          string connName = this.lbConnections.SelectedValue;

          if (!String.IsNullOrEmpty(connName))
          {
            // pull from existing connection string...
            return ConfigurationManager.ConnectionStrings[connName].ConnectionString;
          }

          return string.Empty;
        }

        return YafDBAccess.GetConnectionString(
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

    #endregion

    #region Event Handling

    /// <summary>
    /// The page_ init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Init(object sender, EventArgs e)
    {
      // set the connection manager to the dynamic...
      YafDBAccess.Current.SetConnectionManagerAdapter<YafDynamicDBConnManager>();
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
    private void Page_Load(object sender, EventArgs e)
    {
      YafContext.Current.PageElements.RegisterJQuery(Page.Header);

      if (!IsPostBack)
      {
        if (Session["InstallWizardFinal"] != null)
        {
          CurrentWizardStepID = "WizFinished";
          Session.Remove("InstallWizardFinal");
        }
        else
        {
          Cache["DBVersion"] = DB.DBVersion;

          CurrentWizardStepID = IsInstalled ? "WizEnterPassword" : "WizValidatePermission"; // "WizCreatePassword"

          if (!IsInstalled)
          {
            // fake the board settings
            YafContext.Current.BoardSettings = new YafBoardSettings();
          }

          this.TimeZones.DataSource = StaticDataHelper.TimeZones("english.xml");
          DataBind();
          this.TimeZones.Items.FindByValue("0").Selected = true;
          this.FullTextSupport.Visible = DB.FullTextSupported;

          this.DBUsernamePasswordHolder.Visible = DB.PasswordPlaceholderVisible;

          // Connection string parameters text boxes
          this.Parameter1_Name.Text = DB.Parameter1_Name;
          this.Parameter1_Value.Text = DB.Parameter1_Value;
          this.Parameter1_Value.Visible = DB.Parameter1_Visible;

          this.Parameter2_Name.Text = DB.Parameter2_Name;
          this.Parameter2_Value.Text = DB.Parameter2_Value;
          this.Parameter2_Value.Visible = DB.Parameter2_Visible;

          this.Parameter3_Name.Text = DB.Parameter3_Name;
          this.Parameter3_Value.Text = DB.Parameter3_Value;
          this.Parameter3_Value.Visible = DB.Parameter3_Visible;

          this.Parameter4_Name.Text = DB.Parameter4_Name;
          this.Parameter4_Value.Text = DB.Parameter4_Value;
          this.Parameter4_Value.Visible = DB.Parameter4_Visible;

          this.Parameter5_Name.Text = DB.Parameter5_Name;
          this.Parameter5_Value.Text = DB.Parameter5_Value;
          this.Parameter5_Value.Visible = DB.Parameter5_Visible;

          this.Parameter6_Name.Text = DB.Parameter6_Name;
          this.Parameter6_Value.Text = DB.Parameter6_Value;
          this.Parameter6_Value.Visible = DB.Parameter6_Visible;

          this.Parameter7_Name.Text = DB.Parameter7_Name;
          this.Parameter7_Value.Text = DB.Parameter7_Value;
          this.Parameter7_Value.Visible = DB.Parameter7_Visible;

          this.Parameter8_Name.Text = DB.Parameter8_Name;
          this.Parameter8_Value.Text = DB.Parameter8_Value;
          this.Parameter8_Value.Visible = DB.Parameter8_Visible;

          this.Parameter9_Name.Text = DB.Parameter9_Name;
          this.Parameter9_Value.Text = DB.Parameter9_Value;
          this.Parameter9_Value.Visible = DB.Parameter9_Visible;

          this.Parameter10_Name.Text = DB.Parameter10_Name;
          this.Parameter10_Value.Text = DB.Parameter10_Value;
          this.Parameter10_Value.Visible = DB.Parameter10_Visible;

          // Connection string parameters  check boxes
          this.Parameter11_Value.Text = DB.Parameter11_Name;
          this.Parameter11_Value.Checked = DB.Parameter11_Value;
          this.Parameter11_Value.Visible = DB.Parameter11_Visible;

          this.Parameter12_Value.Text = DB.Parameter12_Name;
          this.Parameter12_Value.Checked = DB.Parameter12_Value;
          this.Parameter12_Value.Visible = DB.Parameter12_Visible;

          this.Parameter13_Value.Text = DB.Parameter13_Name;
          this.Parameter13_Value.Checked = DB.Parameter13_Value;
          this.Parameter13_Value.Visible = DB.Parameter13_Visible;

          this.Parameter14_Value.Text = DB.Parameter14_Name;
          this.Parameter14_Value.Checked = DB.Parameter14_Value;
          this.Parameter14_Value.Visible = DB.Parameter14_Visible;

          this.Parameter15_Value.Text = DB.Parameter15_Name;
          this.Parameter15_Value.Checked = DB.Parameter15_Value;
          this.Parameter15_Value.Visible = DB.Parameter15_Visible;

          this.Parameter16_Value.Text = DB.Parameter16_Name;
          this.Parameter16_Value.Checked = DB.Parameter16_Value;
          this.Parameter16_Value.Visible = DB.Parameter16_Visible;

          this.Parameter17_Value.Text = DB.Parameter17_Name;
          this.Parameter17_Value.Checked = DB.Parameter17_Value;
          this.Parameter17_Value.Visible = DB.Parameter17_Visible;

          this.Parameter18_Value.Text = DB.Parameter18_Name;
          this.Parameter18_Value.Checked = DB.Parameter18_Value;
          this.Parameter18_Value.Visible = DB.Parameter18_Visible;

          this.Parameter19_Value.Text = DB.Parameter19_Name;
          this.Parameter19_Value.Checked = DB.Parameter19_Value;
          this.Parameter19_Value.Visible = DB.Parameter19_Visible;
        }
      }
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
    protected void UserChoice_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.UserChoice.SelectedValue == "create")
      {
        this.ExistingUserHolder.Visible = false;
        this.CreateAdminUserHolder.Visible = true;
      }
      else if (this.UserChoice.SelectedValue == "existing")
      {
        this.ExistingUserHolder.Visible = true;
        this.CreateAdminUserHolder.Visible = false;
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
    protected void Wizard_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
      // reset the board settings...
      YafContext.Current.BoardSettings = null;

      if (Config.IsDotNetNuke)
      {
        // Redirect back to the portal main page.
        string rPath = YafForumInfo.ForumRoot;
        int pos = rPath.IndexOf("/", 2);
        rPath = rPath.Substring(0, pos);
        Response.Redirect(rPath);
      }
      else
      {
        Response.Redirect("~/");
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
    protected void Wizard_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
      if (CurrentWizardStepID == "WizTestSettings")
      {
        CurrentWizardStepID = "WizDatabaseConnection";
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
    /// The wizard_ active step changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Wizard_ActiveStepChanged(object sender, EventArgs e)
    {
      bool previousVisible = false;

      switch (CurrentWizardStepID)
      {
        case "WizCreatePassword":
          if (this._config.TrustLevel == AspNetHostingPermissionLevel.High && this._config.AppSettingsFull != null)
          {
            this.lblConfigPasswordAppSettingFile.Text = this._config.AppSettingsFull.File;
          }
          else
          {
            this.lblConfigPasswordAppSettingFile.Text = "(Unknown! YAF default is app.config)";
          }

          if (IsInstalled)
          {
            // no need for this setup if IsInstalled...
            this.InstallWizard.ActiveStepIndex++;
          }

          break;
        case "WizCreateForum":
          if (DB.IsForumInstalled)
          {
            this.InstallWizard.ActiveStepIndex++;
          }

          break;
        case "WizMigrateUsers":
          if (!IsInstalled)
          {
            // no migration because it's a new install...
            CurrentWizardStepID = "WizFinished";
          }
          else
          {
            object version = Cache["DBVersion"] ?? DB.DBVersion;

            if (((int) version) >= 30 || ((int) version) == -1)
            {
              // migration is NOT needed...
              CurrentWizardStepID = "WizFinished";
            }

            Cache.Remove("DBVersion");
          }

          // get user count
          if (CurrentWizardStepID == "WizMigrateUsers")
          {
            this.lblMigrateUsersCount.Text = DB.user_list(PageBoardID, null, true).Rows.Count.ToString();
          }

          break;
        case "WizDatabaseConnection":
          previousVisible = true;

          // fill with connection strings...
          this.lblConnStringAppSettingName.Text = Config.ConnectionStringName;
          FillWithConnectionStrings();
          break;
        case "WizManualDatabaseConnection":
          if (this._config.TrustLevel == AspNetHostingPermissionLevel.High && this._config.AppSettingsFull != null)
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
          if (this._config.TrustLevel == AspNetHostingPermissionLevel.High && this._config.AppSettingsFull != null)
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
          var btnNext = ControlHelper.FindControlAs<Button>(this.InstallWizard, "StepNavigationTemplateContainerID$StepNextButton");
          if (btnNext != null)
          {
            btnNext.Enabled = false;
          }

          break;
      }

      var btnPrevious = ControlHelper.FindControlAs<Button>(this.InstallWizard, "StepNavigationTemplateContainerID$StepPreviousButton");

      if (btnPrevious != null)
      {
        btnPrevious.Visible = previousVisible;
      }
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
    protected void Wizard_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
      e.Cancel = true;

      switch (CurrentWizardStepID)
      {
        case "WizValidatePermission":
          e.Cancel = false;
          break;
        case "WizDatabaseConnection":

          // save the database settings...
          UpdateDBFailureType type = UpdateDatabaseConnection();
          e.Cancel = false;

          if (type == UpdateDBFailureType.None)
          {
            // jump to test settings...
            CurrentWizardStepID = "WizTestSettings";
          }
          else
          {
            // failure -- show the next section but specify which errors...
            if (type == UpdateDBFailureType.AppSettingsWrite)
            {
              this.NoWriteAppSettingsHolder.Visible = true;
            }
            else if (type == UpdateDBFailureType.ConnectionStringWrite)
            {
              this.NoWriteDBSettingsHolder.Visible = true;
              this.lblDBConnStringName.Text = Config.ConnectionStringName;
              this.lblDBConnStringValue.Text = CurrentConnString;
            }
          }

          break;
        case "WizManualDatabaseConnection":
          e.Cancel = false;
          break;
        case "WizCreatePassword":
          if (this.txtCreatePassword1.Text.Trim() == string.Empty)
          {
            AddLoadMessage("Please enter a configuration password.");
            break;
          }

          if (this.txtCreatePassword2.Text != this.txtCreatePassword1.Text)
          {
            AddLoadMessage("Verification is not the same as your password.");
            break;
          }

          e.Cancel = false;

          if (this._config.TrustLevel == AspNetHostingPermissionLevel.High && this._config.WriteAppSetting(_appPasswordKey, this.txtCreatePassword1.Text))
          {
            // advance to the testing section since the password is now set...
            CurrentWizardStepID = "WizDatabaseConnection";
          }
          else
          {
            CurrentWizardStepID = "WizManuallySetPassword";
          }

          break;
        case "WizManuallySetPassword":
          if (IsInstalled)
          {
            e.Cancel = false;
          }
          else
          {
            AddLoadMessage("You must update your appSettings with the YAF.ConfigPassword Key to continue. NOTE: The key name is case sensitive.");
          }

          break;
        case "WizTestSettings":
          e.Cancel = false;
          break;
        case "WizEnterPassword":
          if (this._config.GetConfigValueAsString("YAF.ConfigPassword") ==
              FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtEnteredPassword.Text, "md5") ||
              this._config.GetConfigValueAsString("YAF.ConfigPassword") == this.txtEnteredPassword.Text.Trim())
          {
            e.Cancel = false;

            // move to test settings...
            CurrentWizardStepID = "WizTestSettings";
          }
          else
          {
            AddLoadMessage("Wrong password!");
          }

          break;
        case "WizCreateForum":
          if (CreateForum())
          {
            e.Cancel = false;
          }

          break;
        case "WizInitDatabase":
          if (UpgradeDatabase(this.FullTextSupport.Checked))
          {
            e.Cancel = false;
          }

          break;
        case "WizMigrateUsers":

          // migrate users/roles only if user does not want to skip
          if (!this.skipMigration.Checked)
          {
            RoleMembershipHelper.SyncRoles(PageBoardID);

            // start the background migration task...
            MigrateUsersTask.Start(PageBoardID);
          }

          e.Cancel = false;
          break;
        case "WizFinished":
          break;
        default:
          throw new ApplicationException("Installation Wizard step not handled: " + this.InstallWizard.WizardSteps[e.CurrentStepIndex].ID);
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
    protected void rblYAFDatabase_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (this.rblYAFDatabase.SelectedValue == "create")
      {
        this.ExistingConnectionHolder.Visible = false;
        this.NewConnectionHolder.Visible = true;
      }
      else if (this.rblYAFDatabase.SelectedValue == "existing")
      {
        this.ExistingConnectionHolder.Visible = true;
        this.NewConnectionHolder.Visible = false;
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
    protected void btnTestDBConnection_Click(object sender, EventArgs e)
    {
      // attempt to connect selected DB...
      YafContext.Current["ConnectionString"] = CurrentConnString;
      string message;

      if (!TestDatabaseConnection(out message))
      {
        UpdateInfoPanel(this.ConnectionInfoHolder, this.lblConnectionDetails, "Failed to connect:<br/><br/>" + message, "errorinfo");
      }
      else
      {
        UpdateInfoPanel(this.ConnectionInfoHolder, this.lblConnectionDetails, "Connection Succeeded", "successinfo");
      }

      // we're done with it...
      YafContext.Current.Vars.Remove("ConnectionString");
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
    protected void btnTestDBConnectionManual_Click(object sender, EventArgs e)
    {
      // attempt to connect DB...
      string message;

      if (!TestDatabaseConnection(out message))
      {
        UpdateInfoPanel(this.ManualConnectionInfoHolder, this.lblConnectionDetailsManual, "Failed to connect:<br/><br/>" + message, "errorinfo");
      }
      else
      {
        UpdateInfoPanel(this.ManualConnectionInfoHolder, this.lblConnectionDetailsManual, "Connection Succeeded", "successinfo");
      }
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
    protected void Parameter11_Value_CheckChanged(object sender, EventArgs e)
    {
      this.DBUsernamePasswordHolder.Visible = !this.Parameter11_Value.Checked;
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
    protected void btnTestPermissions_Click(object sender, EventArgs e)
    {
      UpdateStatusLabel(this.lblPermissionApp, 1);
      UpdateStatusLabel(this.lblPermissionUpload, 1);
      UpdateStatusLabel(this.lblHostingTrust, 1);

      UpdateStatusLabel(this.lblPermissionApp, DirectoryHasWritePermission(Server.MapPath("~/")) ? 2 : 0);
      UpdateStatusLabel(this.lblPermissionUpload, DirectoryHasWritePermission(Server.MapPath(YafBoardFolders.Current.Uploads)) ? 2 : 0);

      if (this._config.TrustLevel == AspNetHostingPermissionLevel.High)
      {
        UpdateStatusLabel(this.lblHostingTrust, 2);
      }
      else
      {
        UpdateStatusLabel(this.lblHostingTrust, 0);
      }

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
    protected void btnTestSmtp_Click(object sender, EventArgs e)
    {
      try
      {
        YafServices.SendMail.Send(
          this.txtTestFromEmail.Text.Trim(), 
          this.txtTestToEmail.Text.Trim(), 
          "Test Email From Yet Another Forum.NET", 
          "The email sending appears to be working from your YAF installation.");

        // success
        UpdateInfoPanel(this.SmtpInfoHolder, this.lblSmtpTestDetails, "Mail Sent. Verify it's received at your entered email address.", "successinfo");
      }
      catch (Exception x)
      {
        UpdateInfoPanel(this.SmtpInfoHolder, this.lblSmtpTestDetails, "Failed to connect:<br/><br/>" + x.Message, "errorinfo");
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
    protected void UpdateStatusTimer_Tick(object sender, EventArgs e)
    {
      // see if the migration is done....
      if (YafTaskModule.Current.IsTaskRunning(MigrateUsersTask.TaskName))
      {
        // proceed...
        return;
      }

      if (Session["InstallWizardFinal"] == null)
      {
        Session.Add("InstallWizardFinal", true);
      }

      // done here...
      Response.Redirect("default.aspx");
    }

    #endregion

    #region Event Helper Functions

    /// <summary>
    /// Tests database connection. Can probably be moved to DB class.
    /// </summary>
    /// <param name="exceptionMessage">
    /// </param>
    /// <returns>
    /// The test database connection.
    /// </returns>
    private static bool TestDatabaseConnection(out string exceptionMessage)
    {
      return YafDBAccess.TestConnection(out exceptionMessage);
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
          if (!this._config.WriteConnectionString(Config.ConnectionStringName, CurrentConnString, DB.ProviderAssemblyName))
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
    private static void UpdateInfoPanel(Control infoHolder, Label detailsLabel, string info, string cssClass)
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
    private static void UpdateStatusLabel(Label theLabel, int status)
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
    /// The fill with connection strings.
    /// </summary>
    private void FillWithConnectionStrings()
    {
      if (this.lbConnections.Items.Count == 0)
      {
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
    }

    /// <summary>
    /// Validates write permission in a specific directory. Should be moved to YAF.Classes.Utils.
    /// </summary>
    /// <param name="directory">
    /// </param>
    /// <returns>
    /// The directory has write permission.
    /// </returns>
    private static bool DirectoryHasWritePermission(string directory)
    {
      bool hasWriteAccess = false;

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
        FixAccess(false);

        foreach (string script in DB.ScriptList)
        {
          ExecuteScript(script, true);
        }

        FixAccess(true);

        int prevVersion = DB.DBVersion;

        DB.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

        // Ederon : 9/7/2007
        // resync all boards - necessary for propr last post bubbling
        DB.board_resync();

        // upgrade providers...
        Providers.Membership.DB.Current.UpgradeMembership(prevVersion, YafForumInfo.AppVersion);

        if (DB.IsForumInstalled && prevVersion < 30)
        {
          // load default bbcode if available...
          if (File.Exists(Request.MapPath(_bbcodeImport)))
          {
            // import into board...
            using (var bbcodeStream = new StreamReader(Request.MapPath(_bbcodeImport)))
            {
              DataImport.BBCodeExtensionImport(PageBoardID, bbcodeStream.BaseStream);
              bbcodeStream.Close();
            }
          }

          // load default extensions if available...
          if (File.Exists(Request.MapPath(_fileImport)))
          {
            // import into board...
            using (var fileExtStream = new StreamReader(Request.MapPath(_fileImport)))
            {
              DataImport.FileExtensionImport(PageBoardID, fileExtStream.BaseStream);
              fileExtStream.Close();
            }
          }
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
      if (fullText && DB.FullTextSupported)
      {
        try
        {
          ExecuteScript(DB.FullTextScript, false);
        }
        catch (Exception x)
        {
          // just a warning...
          AddLoadMessage("Warning: FullText Support wasn't installed: " + x.Message);
        }
      }

      return true;
    }

    /// <summary>
    /// The create forum.
    /// </summary>
    /// <returns>
    /// The create forum.
    /// </returns>
    private bool CreateForum()
    {
      if (DB.IsForumInstalled)
      {
        AddLoadMessage("Forum is already installed.");
        return false;
      }

      if (this.TheForumName.Text.Length == 0)
      {
        AddLoadMessage("You must enter a forum name.");
        return false;
      }

      if (this.ForumEmailAddress.Text.Length == 0)
      {
        AddLoadMessage("You must enter a forum email address.");
        return false;
      }

      MembershipUser user = null;

      if (this.UserChoice.SelectedValue == "create")
      {
        if (this.UserName.Text.Length == 0)
        {
          AddLoadMessage("You must enter the admin user name,");
          return false;
        }

        if (this.AdminEmail.Text.Length == 0)
        {
          AddLoadMessage("You must enter the administrators email address.");
          return false;
        }

        if (this.Password1.Text.Length == 0)
        {
          AddLoadMessage("You must enter a password.");
          return false;
        }

        if (this.Password1.Text != this.Password2.Text)
        {
          AddLoadMessage("The passwords must match.");
          return false;
        }

        // create the admin user...
        MembershipCreateStatus status;
        user = YafContext.Current.CurrentMembership.CreateUser(
          this.UserName.Text, this.Password1.Text, this.AdminEmail.Text, this.SecurityQuestion.Text, this.SecurityAnswer.Text, true, null, out status);
        if (status != MembershipCreateStatus.Success)
        {
          AddLoadMessage(string.Format("Create Admin User Failed: {0}", GetMembershipErrorMessage(status)));
          return false;
        }
      }
      else
      {
        // try to get data for the existing user...
        user = UserMembershipHelper.GetUser(this.ExistingUserName.Text.Trim());

        if (user == null)
        {
          AddLoadMessage("Existing user name is invalid and does not represent a current user in the membership store.");
          return false;
        }
      }

      try
      {
        // add administrators and registered if they don't already exist...
        if (!RoleMembershipHelper.RoleExists("Administrators"))
        {
          RoleMembershipHelper.CreateRole("Administrators");
        }

        if (!RoleMembershipHelper.RoleExists("Registered"))
        {
          RoleMembershipHelper.CreateRole("Registered");
        }

        if (!RoleMembershipHelper.IsUserInRole(user.UserName, "Administrators"))
        {
          RoleMembershipHelper.AddUserToRole(user.UserName, "Administrators");
        }

        // logout administrator...
        FormsAuthentication.SignOut();
        DB.system_initialize(
          this.TheForumName.Text, this.TimeZones.SelectedItem.Value, this.ForumEmailAddress.Text, string.Empty, user.UserName, user.Email, user.ProviderUserKey);
        DB.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);
        DB.system_updateversion(YafForumInfo.AppVersion, YafForumInfo.AppVersionName);

        // vzrus: uncomment it to not keep install/upgrade objects in db for a place and better security
        // YAF.Classes.Data.DB.system_deleteinstallobjects();
        // load default bbcode if available...
        if (File.Exists(Request.MapPath(_bbcodeImport)))
        {
          // import into board...
          using (var bbcodeStream = new StreamReader(Request.MapPath(_bbcodeImport)))
          {
            DataImport.BBCodeExtensionImport(PageBoardID, bbcodeStream.BaseStream);
            bbcodeStream.Close();
          }
        }

        // load default extensions if available...
        if (File.Exists(Request.MapPath(_fileImport)))
        {
          // import into board...
          using (var fileExtStream = new StreamReader(Request.MapPath(_fileImport)))
          {
            DataImport.FileExtensionImport(PageBoardID, fileExtStream.BaseStream);
            fileExtStream.Close();
          }
        }
      }
      catch (Exception x)
      {
        AddLoadMessage(x.Message);
        return false;
      }

      return true;
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
    private int IndexOfWizardID(string id)
    {
      var step = ControlHelper.FindWizardControlRecursive(this.InstallWizard, id) as WizardStepBase;

      if (step != null)
      {
        return this.InstallWizard.WizardSteps.IndexOf(step);
      }

      return -1;
    }

    /// <summary>
    /// The update db failure type.
    /// </summary>
    private enum UpdateDBFailureType
    {
      /// <summary>
      /// The none.
      /// </summary>
      None, 

      /// <summary>
      /// The app settings write.
      /// </summary>
      AppSettingsWrite, 

      /// <summary>
      /// The connection string write.
      /// </summary>
      ConnectionStringWrite
    }

    #endregion

    #region General Helper Functions

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
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
    /// The get membership error message.
    /// </summary>
    /// <param name="status">
    /// The status.
    /// </param>
    /// <returns>
    /// The get membership error message.
    /// </returns>
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
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }

    /// <summary>
    /// The add load message.
    /// </summary>
    /// <param name="msg">
    /// The msg.
    /// </param>
    private void AddLoadMessage(string msg)
    {
      msg = msg.Replace("\\", "\\\\");
      msg = msg.Replace("'", "\\'");
      msg = msg.Replace("\r\n", "\\r\\n");
      msg = msg.Replace("\n", "\\n");
      msg = msg.Replace("\"", "\\\"");
      this._loadMessage += msg + "\\n\\n";
    }

    #endregion

    #region DB Helper Functions

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
    private void ExecuteScript(string scriptFile, bool useTransactions)
    {
      string script = null;
      try
      {
        using (var file = new StreamReader(Request.MapPath(scriptFile)))
        {
          script = file.ReadToEnd() + "\r\n";

          file.Close();
        }
      }
      catch (FileNotFoundException)
      {
        return;
      }
      catch (Exception x)
      {
        throw new Exception("Failed to read " + scriptFile, x);
      }

      DB.system_initialize_executescripts(script, scriptFile, useTransactions);
    }

    /// <summary>
    /// The fix access.
    /// </summary>
    /// <param name="bGrant">
    /// The b grant.
    /// </param>
    private void FixAccess(bool bGrant)
    {
      DB.system_initialize_fixaccess(bGrant);
    }

    #endregion
  }
}