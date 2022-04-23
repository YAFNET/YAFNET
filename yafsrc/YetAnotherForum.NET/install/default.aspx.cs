/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Install;

#region Using

using System.Configuration;
using System.Globalization;
using System.Security.Permissions;
using System.Web.UI;
using YAF.App_GlobalResources;
using YAF.Core.Context;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models.Identity;
using YAF.Types.Models;

#endregion

/// <summary>
///     The Install Page.
/// </summary>
public partial class _default : BasePage, IHaveServiceLocator
{
    #region Fields

    /// <summary>
    ///     The _config.
    /// </summary>
    private readonly ConfigHelper config = new ();

    /// <summary>
    ///     The _load message.
    /// </summary>
    private string loadMessage = string.Empty;

    #endregion

    #region Public Properties

    /// <summary>
    ///     Gets the install service.
    /// </summary>
    public InstallService InstallService => this.Get<InstallService>();

    /// <summary>
    /// Gets a value indicating whether is forum installed.
    /// </summary>
    public bool IsForumInstalled => this.InstallService.IsForumInstalled;

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
    ///     Gets CurrentConnString.
    /// </summary>
    private string CurrentConnString
    {
        get
        {
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

        string DynamicConnectionString() => this.CurrentConnString.IsSet() ? this.CurrentConnString : previousProvider();

        this.DbAccess.Information.ConnectionString = DynamicConnectionString;
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
        if (!this.InstallService.TestDatabaseConnection(out var message))
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
        if (!this.InstallService.TestDatabaseConnection(out var message))
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
        this.CheckWritePermission();
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
            this.Get<IMailService>().Send(
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
            case "WizDatabaseConnection":
                previousVisible = true;

                // fill with connection strings...
                this.FillWithConnectionStrings();
                break;
            case "WizManualDatabaseConnection":
                this.lblAppSettingsFile.Text = "app.config";

                previousVisible = true;
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
                }

                break;
            case "WizManualDatabaseConnection":
                e.Cancel = false;
                break;
            case "WizTestSettings":
                e.Cancel = false;
                break;
            case "WizCreateForum":
                if (this.CreateForum())
                {
                    e.Cancel = false;
                }

                break;
            case "WizInitDatabase":
                if (this.InstallService.InitializeDatabase())
                {
                    e.Cancel = false;
                }

                break;
            case "WizWelcome":

                e.Cancel = false;

                this.CheckWritePermission();

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
            $"<div class=\"alert alert-{cssClass}\"><span class=\"badge bg-{cssClass}\">{detailsTitle}</span> {info}</div>";
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
                theLabel.CssClass = "badge bg-danger float-end";
                break;
            case 1:
                theLabel.Text = Install.Unchecked;
                theLabel.CssClass = "badge bg-info float-end";
                break;
            case 2:
                theLabel.Text = Install.Yes;
                theLabel.CssClass = "badge bg-success float-end";
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
        if (this.TheForumName.Text.IsNotSet())
        {
            this.ShowErrorMessage(Install.ErrorBoardName);
            return false;
        }

        if (this.ForumEmailAddress.Text.IsNotSet())
        {
            this.ShowErrorMessage(Install.ErrorForumEmail);
            return false;
        }

        AspNetUsers user;
        var applicationId = Guid.NewGuid();

        if (this.UserChoice.SelectedValue == "create")
        {
            if (this.UserName.Text.IsNotSet())
            {
                this.ShowErrorMessage(Install.ErrorUserName);
                return false;
            }

            if (this.AdminEmail.Text.IsNotSet())
            {
                this.ShowErrorMessage(Install.ErrorUserEmail);
                return false;
            }

            if (this.Password1.Text.IsNotSet())
            {
                this.ShowErrorMessage(Install.ErrorPassword);
                return false;
            }

            if (this.Password1.Text != this.Password2.Text)
            {
                this.ShowErrorMessage(Install.PasswordNoMatch);
                return false;
            }

            // fake the board settings
            BoardContext.Current.BoardSettings ??= new BoardSettings();

            // create the admin user...
            user = new AspNetUsers
                       {
                           Id = Guid.NewGuid().ToString(),
                           ApplicationId = applicationId,
                           UserName = this.UserName.Text,
                           LoweredUserName = this.UserName.Text,
                           Email = this.AdminEmail.Text,
                           IsApproved = true
                       };

            var result = this.Get<IAspNetUsersHelper>().Create(user, this.Password1.Text);

            if (!result.Succeeded)
            {
                this.ShowErrorMessage($"{Install.ErrorUserCreate} - {result.Errors.FirstOrDefault()}");
                return false;
            }
        }
        else
        {
            // try to get data for the existing user...
            user = this.Get<IAspNetUsersHelper>().GetUserByName(this.ExistingUserName.Text.Trim());

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
            if (!this.Get<IAspNetRolesHelper>().RoleExists($"{prefix}Administrators"))
            {
                this.Get<IAspNetRolesHelper>().CreateRole($"{prefix}Administrators");
            }

            if (!this.Get<IAspNetRolesHelper>().RoleExists($"{prefix}Registered Users"))
            {
                this.Get<IAspNetRolesHelper>().CreateRole($"{prefix}Registered Users");
            }

            if (!this.Get<IAspNetRolesHelper>().IsUserInRole(user, $"{prefix}Administrators"))
            {
                this.Get<IAspNetRolesHelper>().AddUserToRole(user, $"{prefix}Administrators");
            }

            // logout administrator...
            this.Get<IAspNetUsersHelper>().SignOut();

            // init forum...
            this.InstallService.InitializeForum(
                applicationId,
                this.TheForumName.Text,
                this.Cultures.SelectedValue,
                this.ForumEmailAddress.Text,
                "YAFLogo.svg",
                this.ForumBaseUrlMask.Text,
                user.UserName,
                user.Email,
                user.Id);
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
        if (Config.IsDotNetNuke)
        {
            this.Get<HttpResponseBase>().Redirect("~/");
            return;
        }

        if (this.InstallService.IsForumInstalled)
        {
            this.Get<HttpResponseBase>().Redirect("~/");
            return;
        }

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

            this.CurrentWizardStepID = "WizWelcome";

            if (!this.IsForumInstalled)
            {
                // fake the board settings
                BoardContext.Current.BoardSettings = new BoardSettings();
            }

            this.Cultures.DataSource = StaticDataHelper.Cultures();
            this.Cultures.DataValueField = "CultureTag";
            this.Cultures.DataTextField = "CultureNativeName";

            this.UserChoice.Items[0].Text = Install.CreateUser;
            this.UserChoice.Items[1].Text = Install.ExistingUser;

            this.DataBind();

            if (this.Cultures.Items.Count > 0)
            {
                this.Cultures.Items.FindByValue("en-US").Selected = true;
            }

            this.ForumBaseUrlMask.Text = BaseUrlBuilder.GetBaseUrlFromVariables();

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

        return UpdateDBFailureType.None;
    }

    private void CheckWritePermission()
    {
        UpdateStatusLabel(this.lblPermissionApp, 1);
        UpdateStatusLabel(this.lblPermissionUpload, 1);

        UpdateStatusLabel(this.lblPermissionApp, DirectoryHasWritePermission(this.Server.MapPath("~/")) ? 2 : 0);
        UpdateStatusLabel(
            this.lblPermissionUpload,
            DirectoryHasWritePermission(this.Server.MapPath(this.Get<BoardFolders>().Uploads)) ? 2 : 0);
    }

    #endregion
}