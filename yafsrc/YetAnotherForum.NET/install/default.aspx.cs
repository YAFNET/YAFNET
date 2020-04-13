/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Install
{
    #region Using

    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using YAF.App_GlobalResources;
    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Tasks;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     The Install Page.
    /// </summary>
    public partial class _default : BasePage, IHaveServiceLocator
    {
        #region Constants

        /// <summary>
        ///     The app settings password key.
        /// </summary>
        private const string AppPasswordKey = "YAF.ConfigPassword";

        #endregion

        #region Fields

        /// <summary>
        ///     The _config.
        /// </summary>
        private readonly ConfigHelper config = new ConfigHelper();

        /// <summary>
        ///     The _load message.
        /// </summary>
        private string loadMessage = string.Empty;

        /// <summary>
        /// The _is forum installed.
        /// </summary>
        private bool? isForumInstalled;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the install upgrade service.
        /// </summary>
        public InstallUpgradeService InstallUpgradeService => this.Get<InstallUpgradeService>();

        /// <summary>
        ///     Gets a value indicating whether IsInstalled.
        /// </summary>
        public bool IsConfigPasswordSet => this.config.GetConfigValueAsString(AppPasswordKey).IsSet();

        /// <summary>
        /// Gets a value indicating whether is forum installed.
        /// </summary>
        public bool IsForumInstalled => (this.isForumInstalled ?? (this.isForumInstalled = this.InstallUpgradeService.IsForumInstalled))
            .Value;

        /// <summary>
        ///     Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the database access.
        /// </summary>
        /// <value>
        /// The database access.
        /// </value>
        public IDbAccess DbAccess => this.Get<IDbAccess>();

        /// <summary>
        /// Gets the page board identifier.
        /// </summary>
        /// <value>
        /// The page board identifier.
        /// </value>
        private static int PageBoardID
        {
            get
            {
                try
                {
                    return Config.BoardID.ToType<int>();
                }
                catch
                {
                    return 1;
                }
            }
        }

        /// <summary>
        ///     Gets CurrentConnString.
        /// </summary>
        private string CurrentConnString
        {
            get
            {
                if (this.rblYAFDatabase.SelectedValue != "existing")
                {
                    return DbInformationHelper.BuildConnectionString(
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

                var connName = this.lbConnections.SelectedValue;

                return connName.IsSet()
                           ? ConfigurationManager.ConnectionStrings[connName].ConnectionString
                           : string.Empty;
            }
        }

        /// <summary>
        ///     Gets or sets CurrentWizardStepID.
        /// </summary>
        private string CurrentWizardStepID
        {
            get => this.InstallWizard.WizardSteps[this.InstallWizard.ActiveStepIndex].ID;

            set
            {
                var index = this.IndexOfWizardId(value);
                if (index >= 0)
                {
                    this.InstallWizard.ActiveStepIndex = index;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the membership error message.
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
            return status switch
                {
                    MembershipCreateStatus.DuplicateUserName => Install.DuplicateUserName,
                    MembershipCreateStatus.DuplicateEmail => Install.DuplicateEmail,
                    MembershipCreateStatus.InvalidPassword => Install.InvalidPassword,
                    MembershipCreateStatus.InvalidEmail => Install.InvalidEmail,
                    MembershipCreateStatus.InvalidAnswer => Install.InvalidAnswer,
                    MembershipCreateStatus.InvalidQuestion => Install.InvalidQuestion,
                    MembershipCreateStatus.InvalidUserName => Install.InvalidUserName,
                    MembershipCreateStatus.ProviderError => Install.ProviderError,
                    MembershipCreateStatus.UserRejected => Install.UserRejected,
                    _ => Install.UnknownError
                };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Initialize event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // set the connection string provider...
            var previousProvider = this.Get<IDbAccess>().Information.ConnectionString;

            string DynamicConnectionString()
            {
                if (BoardContext.Current.Vars.ContainsKey("ConnectionString"))
                {
                    return BoardContext.Current.Vars["ConnectionString"] as string;
                }

                return previousProvider();
            }

            this.DbAccess.Information.ConnectionString = DynamicConnectionString;
        }

        /// <summary>
        /// The parameter 11_ value_ check changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Parameter11_Value_CheckChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.DBUsernamePasswordHolder.Visible = !this.Parameter11_Value.Checked;
        }

        /// <summary>
        /// Initializes the <see cref="T:System.Web.UI.HtmlTextWriter" /> object and calls on the child controls of the <see cref="T:System.Web.UI.Page" /> to render.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> that receives the page content.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        /// <summary>
        /// Test's the DB Connection
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void TestDBConnectionManual_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // attempt to connect DB...
            if (!this.InstallUpgradeService.TestDatabaseConnection(out var message))
            {
                UpdateInfoPanel(
                    this.ManualConnectionInfoHolder,
                    this.lblConnectionDetailsManual,
                    Install.ConnectionDetails,
                    $"{Install.ConnectionFailed} {message}",
                    "danger");
            }
            else
            {
                UpdateInfoPanel(
                    this.ManualConnectionInfoHolder,
                    this.lblConnectionDetailsManual,
                    Install.ConnectionDetails,
                    Install.ConnectionSuccess,
                    "success");
            }
        }

        /// <summary>
        /// Test's the DB Connection
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void TestDBConnection_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // attempt to connect selected DB...
            BoardContext.Current["ConnectionString"] = this.CurrentConnString;

            if (!this.InstallUpgradeService.TestDatabaseConnection(out var message))
            {
                UpdateInfoPanel(
                    this.ConnectionInfoHolder,
                    this.lblConnectionDetails,
                    Install.ConnectionDetails,
                    $"{Install.ConnectionFailed} {message}",
                    "danger");
            }
            else
            {
                UpdateInfoPanel(
                    this.ConnectionInfoHolder,
                    this.lblConnectionDetails,
                    Install.ConnectionDetails,
                    Install.ConnectionSuccess,
                    "success");
            }

            // we're done with it...
            BoardContext.Current.Vars.Remove("ConnectionString");
        }

        /// <summary>
        /// Test's the Application Permissions
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void TestPermissions_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            UpdateStatusLabel(this.lblPermissionApp, 1);
            UpdateStatusLabel(this.lblPermissionUpload, 1);

            UpdateStatusLabel(this.lblPermissionApp, DirectoryHasWritePermission(this.Server.MapPath("~/")) ? 2 : 0);
            UpdateStatusLabel(
                this.lblPermissionUpload,
                DirectoryHasWritePermission(this.Server.MapPath(BoardFolders.Current.Uploads)) ? 2 : 0);
        }

        /// <summary>
        /// Send a test email
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void TestSmtp_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                this.Get<ISendMail>().Send(
                    this.txtTestFromEmail.Text.Trim(),
                    this.txtTestToEmail.Text.Trim(),
                    this.txtTestFromEmail.Text.Trim(),
                    Install.SmtpTestSubject,
                    Install.SmtpTestBody);

                // success
                UpdateInfoPanel(
                    this.SmtpInfoHolder,
                    this.lblSmtpTestDetails,
                    Install.SmtpTestDetails,
                    Install.SmtpTestSuccess,
                    "success");
            }
            catch (Exception x)
            {
                UpdateInfoPanel(
                    this.SmtpInfoHolder,
                    this.lblSmtpTestDetails,
                    Install.SmtpTestDetails,
                    $"{Install.ConnectionFailed} {x.Message}",
                    "danger");
            }
        }

        /// <summary>
        /// Handles the Tick event of the UpdateStatusTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
            try
            {
                this.Get<HttpResponseBase>().Redirect(BuildLink.GetLink(ForumPages.forum));
            }
            catch (Exception)
            {
                this.Get<HttpResponseBase>().Redirect("default.aspx");
            }
        }

        /// <summary>
        /// The user choice_ selected index changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
        /// Handles the ActiveStepChanged event of the Wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Wizard_ActiveStepChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var previousVisible = false;

            switch (this.CurrentWizardStepID)
            {
                case "WizCreatePassword":
                    this.lblConfigPasswordAppSettingFile.Text = "app.config";

                    if (this.IsConfigPasswordSet)
                    {
                        // no need for this setup if IsInstalled...
                        this.InstallWizard.ActiveStepIndex++;

                        if (!this.IsForumInstalled)
                        {
                            // Skip enter the password on a new install when
                            // the app.config password is already set
                            this.CurrentWizardStepID = "WizDatabaseConnection";
                        }
                    }

                    break;
                case "WizCreateForum":
                    if (this.InstallUpgradeService.IsForumInstalled)
                    {
                        this.InstallWizard.ActiveStepIndex++;
                    }

                    break;
                case "WizMigrateUsers":
                    if (!this.IsConfigPasswordSet)
                    {
                        // no migration because it's a new install...
                        this.CurrentWizardStepID = "WizFinished";
                    }
                    else
                    {
                        var version = (this.Cache["DBVersion"] ?? this.GetRepository<Registry>().GetDbVersion()).ToType<int>();

                        if (version >= 30 || version == -1)
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
                            this.GetRepository<User>().ListAsDataTable(PageBoardID, null, true).Rows.Count.ToString();
                    }

                    break;
                case "WizDatabaseConnection":
                    previousVisible = true;

                    // fill with connection strings...
                    this.lblConnStringAppSettingName.Text = Config.ConnectionStringName;
                    this.FillWithConnectionStrings();
                    break;
                case "WizManualDatabaseConnection":
                    this.lblAppSettingsFile.Text = "app.config";

                    previousVisible = true;
                    break;
                case "WizManuallySetPassword":
                    this.lblAppSettingsFile2.Text = "app.config";

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
        /// Handles the FinishButtonClick event of the Wizard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WizardNavigationEventArgs"/> instance containing the event data.</param>
        protected void Wizard_FinishButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
        {
            // reset the board settings...
            BoardContext.Current.BoardSettings = null;

            this.Get<HttpResponseBase>().Redirect("~/");
        }

        /// <summary>
        /// The wizard_ next button click.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Web.UI.WebControls.WizardNavigationEventArgs"/> instance containing the event data.
        /// </param>
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
                    var type = this.UpdateDatabaseConnection();
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
                    if (this.txtCreatePassword1.Text.IsNotSet())
                    {
                        this.ShowErrorMessage(Install.EnterConfigPassword);
                        break;
                    }

                    if (this.txtCreatePassword2.Text != this.txtCreatePassword1.Text)
                    {
                        this.ShowErrorMessage(Install.PasswordNoMatch);
                        break;
                    }

                    e.Cancel = false;

                    this.CurrentWizardStepID =
                        this.config.WriteAppSetting(AppPasswordKey, this.txtCreatePassword1.Text)
                            ? "WizDatabaseConnection"
                            : "WizManuallySetPassword";

                    break;
                case "WizManuallySetPassword":
                    if (this.IsConfigPasswordSet)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        this.ShowErrorMessage(
                            Install.ErrorConfigPassword);
                    }

                    break;
                case "WizTestSettings":
                    e.Cancel = false;
                    break;
                case "WizEnterPassword":
                    if (this.config.GetConfigValueAsString(AppPasswordKey)
                        == FormsAuthentication.HashPasswordForStoringInConfigFile(this.txtEnteredPassword.Text, "md5")
                        || this.config.GetConfigValueAsString(AppPasswordKey) == this.txtEnteredPassword.Text.Trim())
                    {
                        e.Cancel = false;

                        // move to upgrade..
                        this.CurrentWizardStepID = this.IsForumInstalled ? "WizWelcomeUpgrade" : "WizDatabaseConnection";

                        var versionName = this.GetRepository<Registry>().GetDbVersionName();
                        var version = this.GetRepository<Registry>().GetDbVersion();

                        this.CurrentVersionName.Text = version < 0
                                                           ? "New"
                                                           : $"{versionName} ({version})";
                        this.UpgradeVersionName.Text = $"{BoardInfo.AppVersionName} ({BoardInfo.AppVersion})";
                    }
                    else
                    {
                        this.ShowErrorMessage(Install.ErrorWrongPassword);
                    }

                    break;
                case "WizCreateForum":
                    if (this.CreateForum())
                    {
                        e.Cancel = false;
                    }

                    break;
                case "WizInitDatabase":
                    if (this.InstallUpgradeService.UpgradeDatabase(
                        this.UpgradeExtensions.Checked))
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
                        this.Get<ITaskModuleManager>().Start<MigrateUsersTask>(PageBoardID);
                    }

                    e.Cancel = false;
                    break;
                case "WizWelcomeUpgrade":

                    e.Cancel = false;

                    // move to upgrade..
                    this.CurrentWizardStepID = "WizInitDatabase";
                    break;
                case "WizWelcome":

                    e.Cancel = false;

                    // move to upgrade..
                    this.CurrentWizardStepID = "WizValidatePermission";
                    break;
                case "WizFinished":
                    break;
                default:
                    throw new ApplicationException(
                        $"Installation Wizard step not handled: {this.InstallWizard.WizardSteps[e.CurrentStepIndex].ID}");
            }
        }

        /// <summary>
        /// The wizard_ previous button click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="WizardNavigationEventArgs" /> instance containing the event data.</param>
        protected void Wizard_PreviousButtonClick([NotNull] object sender, [NotNull] WizardNavigationEventArgs e)
        {
            if (this.CurrentWizardStepID == "WizTestSettings")
            {
                this.CurrentWizardStepID = "WizDatabaseConnection";
            }

            e.Cancel = false;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the YAFDatabase control.
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        protected void YafDatabaseSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
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
        /// The directory.
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
        /// The update info panel.
        /// </summary>
        /// <param name="infoHolder">The info holder.</param>
        /// <param name="detailsLiteral">The details literal.</param>
        /// <param name="detailsTitle">The details title.</param>
        /// <param name="info">The info.</param>
        /// <param name="cssClass">The CSS class.</param>
        private static void UpdateInfoPanel(
            [NotNull] Control infoHolder,
            [NotNull] ITextControl detailsLiteral,
            [NotNull] string detailsTitle,
            [NotNull] string info,
            [NotNull] string cssClass)
        {
            infoHolder.Visible = true;

            detailsLiteral.Text =
                $"<div class=\"alert alert-{cssClass}\"><span class=\"badge badge-{cssClass}\">{detailsTitle}</span> {info}</div>";
        }

        /// <summary>
        /// Updates the status label.
        /// </summary>
        /// <param name="theLabel">The label that's gone be updated.</param>
        /// <param name="status">The status.</param>
        private static void UpdateStatusLabel([NotNull] Label theLabel, int status)
        {
            switch (status)
            {
                case 0:
                    theLabel.Text = Install.No;
                    theLabel.CssClass = "badge badge-danger float-right";
                    break;
                case 1:
                    theLabel.Text = Install.Unchecked;
                    theLabel.CssClass = "badge badge-info float-right";
                    break;
                case 2:
                    theLabel.Text = Install.Yes;
                    theLabel.CssClass = "badge badge-success float-right";
                    break;
            }
        }

        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <param name="msg">The message.</param>
        private void ShowErrorMessage([NotNull] string msg)
        {
            msg = msg.Replace("\\", "\\\\");
            msg = msg.Replace("'", "\\'");
            msg = msg.Replace("\r\n", "<br /><br />");
            msg = msg.Replace("\n", "<br />");
            msg = msg.Replace("\"", "\\\"");
            this.loadMessage += msg;

            if (!this.loadMessage.IsSet())
            {
                return;
            }

            var errorMessage = this.InstallWizard.FindControlAs<PlaceHolder>("ErrorMessage");
            var errorMessageContent = this.InstallWizard.FindControlAs<Literal>("ErrorMessageContent");

            errorMessage.Visible = true;
            errorMessageContent.Text = this.loadMessage;

            this.loadMessage = string.Empty;
        }

        /// <summary>
        ///     Creates the forum.
        /// </summary>
        /// <returns>
        ///     The create forum.
        /// </returns>
        private bool CreateForum()
        {
            if (this.InstallUpgradeService.IsForumInstalled)
            {
                this.ShowErrorMessage(Install.ErrorBoardInstalled);
                return false;
            }

            if (this.TheForumName.Text.Length == 0)
            {
                this.ShowErrorMessage(Install.ErrorBoardName);
                return false;
            }

            if (this.ForumEmailAddress.Text.Length == 0)
            {
                this.ShowErrorMessage(Install.ErrorForumEmail);
                return false;
            }

            MembershipUser user;

            if (this.UserChoice.SelectedValue == "create")
            {
                if (this.UserName.Text.Length == 0)
                {
                    this.ShowErrorMessage(Install.ErrorUserName);
                    return false;
                }

                if (this.AdminEmail.Text.Length == 0)
                {
                    this.ShowErrorMessage(Install.ErrorUserEmail);
                    return false;
                }

                if (this.Password1.Text.Length == 0)
                {
                    this.ShowErrorMessage(Install.ErrorPassword);
                    return false;
                }

                if (this.Password1.Text != this.Password2.Text)
                {
                    this.ShowErrorMessage(Install.PasswordNoMatch);
                    return false;
                }

                // create the admin user...
                user = this.Get<MembershipProvider>()
                    .CreateUser(
                        this.UserName.Text,
                        this.Password1.Text,
                        this.AdminEmail.Text,
                        this.SecurityQuestion.Text,
                        this.SecurityAnswer.Text,
                        true,
                        null,
                        out var status);

                if (status != MembershipCreateStatus.Success)
                {
                    this.ShowErrorMessage($"{Install.ErrorUserCreate} {this.GetMembershipErrorMessage(status)}");
                    return false;
                }
            }
            else
            {
                // try to get data for the existing user...
                user = UserMembershipHelper.GetUser(this.ExistingUserName.Text.Trim());

                if (user == null)
                {
                    this.ShowErrorMessage(Install.ErrorUserNotFound);
                    return false;
                }
            }

            try
            {
                var prefix = Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty;

                // add administrators and registered if they don't already exist...
                if (!RoleMembershipHelper.RoleExists($"{prefix}Administrators"))
                {
                    RoleMembershipHelper.CreateRole($"{prefix}Administrators");
                }

                if (!RoleMembershipHelper.RoleExists($"{prefix}Registered"))
                {
                    RoleMembershipHelper.CreateRole($"{prefix}Registered");
                }

                if (!RoleMembershipHelper.IsUserInRole(user.UserName, $"{prefix}Administrators"))
                {
                    RoleMembershipHelper.AddUserToRole(user.UserName, $"{prefix}Administrators");
                }

                // logout administrator...
                FormsAuthentication.SignOut();

                // init forum...
                this.InstallUpgradeService.InitializeForum(
                    this.TheForumName.Text,
                    this.TimeZones.SelectedValue,
                    this.Culture.SelectedValue,
                    this.ForumEmailAddress.Text,
                    "YAFLogo.svg",
                    this.ForumBaseUrlMask.Text,
                    user.UserName,
                    user.Email,
                    user.ProviderUserKey);
            }
            catch (Exception x)
            {
                this.ShowErrorMessage(x.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        ///     The fill with connection strings.
        /// </summary>
        private void FillWithConnectionStrings()
        {
            if (this.lbConnections.Items.Count != 0)
            {
                return;
            }

            ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().ForEach(
                connectionString => this.lbConnections.Items.Add(connectionString.Name));

            var item = this.lbConnections.Items.FindByText("yafnet");

            if (item != null)
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// Indexes the of wizard ID.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The index of wizard id.
        /// </returns>
        private int IndexOfWizardId([NotNull] string id)
        {
            if (this.InstallWizard.FindWizardControlRecursive(id) is WizardStepBase step)
            {
                return this.InstallWizard.WizardSteps.IndexOf(step);
            }

            return -1;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            var errorMessage = this.InstallWizard.FindControlAs<PlaceHolder>("ErrorMessage"); 

            if (this.loadMessage.IsNotSet())
            {
                errorMessage.Visible = false;
            }

            if (this.IsPostBack)
            {
                this.DataBind();
                return;
            }

            var languages = this.InstallWizard.FindControlAs<DropDownList>("Languages");

            if (languages.Items.FindByValue(CultureInfo.CurrentCulture.Name) != null)
            {
                languages.Items.FindByValue(CultureInfo.CurrentCulture.Name).Selected = true;
            }

            if (this.Session["InstallWizardFinal"] != null)
            {
                this.CurrentWizardStepID = "WizFinished";
                this.Session.Remove("InstallWizardFinal");
            }
            else
            {
                this.Cache["DBVersion"] = this.GetRepository<Registry>().GetDbVersion();

                this.CurrentWizardStepID = this.IsConfigPasswordSet && this.IsForumInstalled ? "WizEnterPassword" : "WizWelcome";

                // "WizCreatePassword"
                if (!this.IsConfigPasswordSet)
                {
                    // fake the board settings
                    BoardContext.Current.BoardSettings = new BoardSettings();
                }

                this.TimeZones.DataSource = StaticDataHelper.TimeZones();

                this.Culture.DataSource = StaticDataHelper.Cultures();
                this.Culture.DataValueField = "CultureTag";
                this.Culture.DataTextField = "CultureNativeName";

                this.rblYAFDatabase.Items[0].Text = Install.ExistConnection;
                this.rblYAFDatabase.Items[1].Text = Install.NewConnection;

                this.UserChoice.Items[0].Text = Install.CreateUser;
                this.UserChoice.Items[1].Text = Install.ExistingUser;

                this.DataBind();

                this.TimeZones.Items.FindByValue(TimeZoneInfo.Local.Id).Selected = true;

                if (this.Culture.Items.Count > 0)
                {
                    this.Culture.Items.FindByValue("en-US").Selected = true;
                }

                this.ForumBaseUrlMask.Text = BaseUrlBuilder.GetBaseUrlFromVariables();

                this.DBUsernamePasswordHolder.Visible = false;

                // Connection string parameters text boxes
                Enumerable.Range(1, 20).ForEach(
                    paramNumber =>
                        {
                            var param = this.DbAccess.Information.DbConnectionParameters.FirstOrDefault(
                                p => p.ID == paramNumber);

                            var label = this.FindControlRecursiveAs<Label>($"Parameter{paramNumber}_Name");
                            if (label != null)
                            {
                                label.Text = param != null ? param.Name : string.Empty;
                            }

                            var control = this.FindControlRecursive($"Parameter{paramNumber}_Value");

                            switch (control)
                            {
                                case TextBox textBox when param != null:
                                    textBox.Text = param.Value;
                                    textBox.Visible = true;
                                    break;
                                case TextBox textBox:
                                    textBox.Text = string.Empty;
                                    textBox.Visible = false;
                                    break;
                                case CheckBox checkBox when param != null:
                                    checkBox.Checked = param.Value.ToType<bool>();
                                    checkBox.Visible = true;
                                    break;
                                case CheckBox checkBox:
                                    checkBox.Checked = false;
                                    checkBox.Visible = false;
                                    break;
                            }
                        });

                // Hide New User on DNN
                if (!Config.IsDotNetNuke)
                {
                    return;
                }

                this.UserChoice.SelectedIndex = 1;
                this.UserChoice.Items[0].Enabled = false;

                this.ExistingUserHolder.Visible = true;
                this.CreateAdminUserHolder.Visible = false;
            }
        }

        /// <summary>
        ///     Updates the database connection.
        /// </summary>
        /// <returns>
        ///     The update database connection.
        /// </returns>
        private UpdateDBFailureType UpdateDatabaseConnection()
        {
            switch (this.rblYAFDatabase.SelectedValue)
            {
                case "existing" when this.lbConnections.SelectedIndex >= 0:
                    {
                        var selectedConnection = this.lbConnections.SelectedValue;
                        if (selectedConnection == Config.ConnectionStringName)
                        {
                            return UpdateDBFailureType.None;
                        }

                        try
                        {
                            // have to write to the appSettings...
                            if (!this.config.WriteAppSetting("YAF.ConnectionStringName", selectedConnection))
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

                        break;
                    }

                case "create":
                    try
                    {
                        if (
                            !this.config.WriteConnectionString(
                                Config.ConnectionStringName,
                                this.CurrentConnString,
                                this.DbAccess.Information.ProviderName))
                        {
                            // failure to write db Settings..
                            return UpdateDBFailureType.ConnectionStringWrite;
                        }
                    }
                    catch
                    {
                        return UpdateDBFailureType.ConnectionStringWrite;
                    }

                    break;
            }

            return UpdateDBFailureType.None;
        }

        #endregion
    }
}