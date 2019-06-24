<%@ Page Language="c#" AutoEventWireup="True" Inherits="YAF.Install._default" CodeBehind="default.aspx.cs" %>

<!doctype html>
<html lang="en">
<head runat="server" id="YafHead">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <meta http-equiv="x-ua-compatible" content="ie=edge">
    <meta id="YafMetaScriptingLanguage" http-equiv="Content-Script-Type" runat="server"
        name="scriptlanguage" content="text/javascript" />
    <meta id="YafMetaStyles" http-equiv="Content-Style-Type" runat="server" name="styles"
        content="text/css" />
   <title>YAF.NET <%# this.IsForumInstalled ? "Upgrade" : "Installation"%></title>
   <link href="../Content/InstallWizard.min.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="Form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
        <asp:Wizard ID="InstallWizard" runat="server" ActiveStepIndex="1" 
            DisplaySideBar="False" OnActiveStepChanged="Wizard_ActiveStepChanged"
            OnFinishButtonClick="Wizard_FinishButtonClick" OnPreviousButtonClick="Wizard_PreviousButtonClick"
            OnNextButtonClick="Wizard_NextButtonClick">
            <StepStyle CssClass="wizStep" />
            <WizardSteps>
                <asp:WizardStep runat="server" Title="Welcome" ID="WizWelcome">
                    <h4>
                        Welcome to the YAF.NET Installation
                    </h4>
                    <p class="descriptionText">
                        Click "Next" to start the Install process.
                    </p>
                    <p>
                        This Installer will guide you through the steps required to Install YetAnotherForum.NET (YAF.NET). 
                    </p>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Upgrade" ID="WizWelcomeUpgrade">
                    <h4>
                        Welcome to the YAF.NET Upgrade Process
                    </h4>
                    <p class="descriptionText">
                        Click "Next" to start the Upgrade process.
                    </p>
                    <ul class="standardList">
                        <li>Current Version: <strong>YAF.NET v<asp:Literal runat="server" ID="CurrentVersionName"></asp:Literal></strong></li>
                        <li>Upgrade Version: <strong>YAF.NET v<asp:Literal runat="server" ID="UpgradeVersionName"></asp:Literal></strong></li>
                    </ul>
                    <p>
                       
                    </p>
                   
                    <div class="warningMessage">
                        You are about to upgrade YAF.NET to the most recent version. Before proceeding with upgrade process please ensure that...
                        <ul>
                            <li>you created a backup of your existing installation</li>
                        </ul>
                    </div>
                    
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Validate Permissions" ID="WizValidatePermission">
                    <h4>
                        Validate Permissions
                    </h4>
                    <p>
                        YetAnotherForum.NET Installation is easier when the Application has write access to your .config
                        files. 
                    <p class="descriptionText">
                        Click below to test if permissions are correct. If they are not, you can
                        still use the installation wizard.
                    </p>
                    <ul class="standardList">
                        <li>
                            <asp:Label ID="lblHostingTrust" runat="server" CssClass="infoLabel float-right">Unchecked</asp:Label>
                            Asp.Net Hosting Trust Level (if trust level is not "Full" or "Unrestricted" you will need to manually
                            modify all configuration files) ...
                        </li>
                        <li>
                            <asp:Label ID="lblPermissionApp" runat="server" CssClass="infoLabel float-right">Unchecked</asp:Label>
                            YAF.NET Has Write Access to Root Application Directory ("~/") ...
                        </li>
                        <li>
                            <asp:Label ID="lblPermissionUpload" runat="server" CssClass="infoLabel float-right">Unchecked</asp:Label>
                            YAF.NET Has Write Access to "~/Upload" directory ...
                        </li>
                    </ul>
                    <YAF:ModernButton ID="btnTestPermissions" runat="server" Text="Test Permissions" CssClass="btn btn-info" 
                        EnableLoadingAnimation="True"
                        OnClick="TestPermissions_Click" data-style="expand-left" />
                </asp:WizardStep> 
                <asp:WizardStep ID="WizCreatePassword" runat="server" Title="Create Config Password">
                    <h4>
                        Create Config Password
                    </h4>

                    <p>
                        Since this is the first time you install or upgrade this version of the forum, you
                        need to create a configuration password for security. 
                    </p>
                    <p class="descriptionText">This password is stored with your appSettings in the
                        <asp:Label ID="lblConfigPasswordAppSettingFile" runat="server">app.config</asp:Label>
                        file.
                    </p>
                    <div class="form-group">
                    <asp:TextBox ID="txtCreatePassword1" runat="server" TextMode="Password"
                        PlaceHolder="Enter the Config Password"
                        LabelText="Config Password"
                                 CssClass="form-control"/>
                    </div>
                    <div class="form-group"><asp:TextBox ID="txtCreatePassword2" runat="server" TextMode="Password"
                        PlaceHolder="Re-Enter the Config Password"
                        LabelText="Verify Password"
                                 CssClass="form-control"/>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Enter Config Password" ID="WizEnterPassword">
                    <h4>
                        Enter Config Password
                    </h4>
                    <p class="descriptionText">
                        Enter the configuration password to upgrade or manually install the forum.
                    </p>
                    <div class="infoMessage">
                        <span class="infoLabel">Note</span> 
                        If this is an upgrade, please enter the configuration ad you created when the forum was first installed, not the admin user password.
                    </div>
                    <asp:TextBox ID="txtEnteredPassword" runat="server" TextMode="Password" Type="Password"
                                 PlaceHolder="Enter the Config Password" 
                                 RenderWrapper="True"
                                 CssClass="form-control"
                                 LabelText="Password"/>
                </asp:WizardStep>
                <asp:WizardStep ID="WizManuallySetPassword" runat="server" Title="Manually Set Config Password">
                    <h4>
                        No Write Access to App Settings
                    </h4>
                    <p class="descriptionText">
                        Click "Next" when you are ready.
                    </p>
                    <div class="errorMessage">
                        <span class="errorLabel">Error</span> Unable to modify your config file to write the installation password due to a permission issue.
                    </div>
                    <p>
                        Please open the <strong>
                            <asp:Label runat="server" ID="lblAppSettingsFile2">web.config</asp:Label></strong>
                        file in the application root directory and add/update the following key in the &lt;AppSettings&gt;
                        section:
                    </p>
                    <code>&lt;add key="YAF.ConfigPassword" value="<span style='color: #0000FF'>YourPassword</span>"/&gt;</code>
                    <br/>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Database Connection" ID="WizDatabaseConnection">
                    <h4>
                            YAF.NET Database Connection
                    </h4>
                    <div class="custom-control custom-radio">
                        <asp:RadioButtonList ID="rblYAFDatabase" runat="server" 
                                             AutoPostBack="true"
                                             OnSelectedIndexChanged="YafDatabaseSelectedIndexChanged"
                                             RepeatLayout="UnorderedList"
                                             CssClass="list-unstyled">
                            <asp:ListItem Text="Use Existing DB Connection String" Selected="true" Value="existing"></asp:ListItem>
                            <asp:ListItem Text="Create New DB Connection String" Value="create"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <asp:PlaceHolder ID="ExistingConnectionHolder" runat="server" Visible="true">
                        <h4>
                            Select Existing Connection String
                        </h4>
                        Select SQL Server Database Connection String:&nbsp;
                        <asp:DropDownList ID="lbConnections" runat="server" CssClass="custom-select">
                        </asp:DropDownList>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="NewConnectionHolder" runat="server" Visible="false">
                        <h4>
                            Create New Connection String
                        </h4>
                        <div class="infoMessage">
                            <span class="infoLabel">Note</span> 
                            Connection String will be saved as "<asp:Label ID="lblConnStringAppSettingName"
                            runat="server" Text="yafnet" />".
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter1_Name" runat="server" AssociatedControlID="Parameter1_Value" />
                            <asp:TextBox runat="server" ID="Parameter1_Value" Text="(local)" 
                                Placeholder="Enter the Data Source (Name or address of the sql server)"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter2_Name" runat="server" AssociatedControlID="Parameter2_Value" />
                            <asp:TextBox runat="server" ID="Parameter2_Value"
                                         CssClass="form-control"
                                Placeholder="Enter the Database Name"/>
                        </div>
                        <asp:PlaceHolder runat="server" Visible="false">
                        <div class="form-group">
                            <asp:Label ID="Parameter3_Name" runat="server" AssociatedControlID="Parameter3_Value" />
                            <asp:TextBox runat="server" ID="Parameter3_Value" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter4_Name" runat="server" AssociatedControlID="Parameter4_Value" />
                            <asp:TextBox runat="server" ID="Parameter4_Value" CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter5_Name" runat="server" AssociatedControlID="Parameter5_Value" />
                            <asp:TextBox runat="server" ID="Parameter5_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter6_Name" runat="server" AssociatedControlID="Parameter6_Value" />
                            <asp:TextBox runat="server" ID="Parameter6_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter7_Name" runat="server" AssociatedControlID="Parameter7_Value" />
                            <asp:TextBox runat="server" ID="Parameter7_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter8_Name" runat="server" AssociatedControlID="Parameter8_Value" />
                            <asp:TextBox runat="server" ID="Parameter8_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter9_Name" runat="server" AssociatedControlID="Parameter9_Value" />
                            <asp:TextBox runat="server" ID="Parameter9_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter10_Name" runat="server" AssociatedControlID="Parameter10_Value" />
                            <asp:TextBox runat="server" ID="Parameter10_Value" CssClass="standardTextInput" />
                        </div>
                        </asp:PlaceHolder>
                        
                        <div class="custom-control custom-checkbox">
                            <asp:CheckBox ID="Parameter11_Value" runat="server" 
                                          Checked="true" 
                                          Text="Use Integrated Security"
                                          AutoPostBack="true" 
                                          OnCheckedChanged="Parameter11_Value_CheckChanged"/>
                        </div>
                        <asp:PlaceHolder runat="server" Visible="false">
                            <div class="form-group">
                            <asp:CheckBox ID="Parameter12_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter13_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter14_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter15_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter16_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter17_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter18_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter19_Value" runat="server" Checked="true" AutoPostBack="true" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="DBUsernamePasswordHolder" Visible="false" runat="server">
                            <div class="form-group">
                            <asp:TextBox runat="server" ID="txtDBUserID" 
                                Placeholder="Enter the SQL User Name" RenderWrapper="True" 
                                LabelText="User ID"
                                         CssClass="form-control"/>
                            </div>
                            <div class="form-group">
                            <asp:TextBox runat="server" ID="txtDBPassword" 
                                Placeholder="Enter the SQL Password" RenderWrapper="True" 
                                LabelText="Password"
                                         CssClass="form-control"/>
                            </div>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                    <hr/>
                    <YAF:ModernButton ID="btnTestDBConnection" runat="server" CssClass="btn btn-info" EnableLoadingAnimation="True" Text="Test Connection"
                        OnClick="TestDBConnection_Click" OnClientClick="return true;" data-style="expand-left" />
                    <asp:PlaceHolder ID="ConnectionInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblConnectionDetails" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Manually Modify Database Connection" ID="WizManualDatabaseConnection">
                    <asp:PlaceHolder ID="NoWriteAppSettingsHolder" runat="server" Visible="false">
                        <h4>
                            No Write Access to Settings
                        </h4>
                        <div class="errorMessage">
                            <span class="errorLabel">Error</span> Unable to modify your config file to set Database Connection due to no write permission.
                        </div>
                        <p>
                            Please open the <strong>
                                <asp:Label runat="server" ID="lblAppSettingsFile">web.config</asp:Label></strong>
                            file in the application root directory and add/update the following key in the &lt;AppSettings&gt;
                            section:
                        </p>
                        <code>&lt;add key="YAF.ConnectionStringName" value="<asp:Label runat="server" ID="lblConnectionStringName"></asp:Label>"/&gt;</code>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="NoWriteDBSettingsHolder" runat="server" Visible="false">
                        <h4>
                            No Write Access to Database Connection Strings
                        </h4>
                        <div class="errorMessage">
                            <span class="errorLabel">Error</span> Unable to modify your config file to set Database Connection string due to no write
                            permission.
                        </div>
                        <p>
                            Please open the <strong><asp:Label runat="server" ID="lblDBSettingsFile">db.config</asp:Label></strong> file
                            in the application root directory and add/update the following key in the &lt;connectionStrings&gt;
                            section:
                        </p>
                        <code>&lt;add name="<asp:Label runat="server" ID="lblDBConnStringName" />" connectionString="<asp:Label runat="server" ID="lblDBConnStringValue" />" /&gt;</code>
                    </asp:PlaceHolder>
                    <p class="descriptionText">
                        Click "Next" when you are ready.</p>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Test Settings" ID="WizTestSettings">
                    <h4>
                            Test Settings
                    </h4>
                    <p class="descriptionText">
                        Click the "Previous" button to edit the database configuration.
                    </p>
                    <p>You may want to test your YAF configuration settings here before proceeding.</p>
                    <h4>
                        Database Connection Test
                    </h4>
                    <YAF:ModernButton ID="btnTestDBConnectionManual" runat="server" CssClass="btn btn-info" EnableLoadingAnimation="True" 
                        Text="Test Database Connection" OnClick="TestDBConnectionManual_Click" data-style="expand-left" />
                    <asp:PlaceHolder ID="ManualConnectionInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblConnectionDetailsManual" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                    <hr />
                    <h4>
                        Mail (SMTP) Sending Test
                    </h4>
                    <p class="descriptionText">
                        Sends a test Message to the Email address that is defined as Send to address
                    </p>
                    <div class="form-group">
                        <asp:TextBox ID="txtTestFromEmail" runat="server"
                                     Placeholder="Enter the from Email Address" RenderWrapper="True" Type="Email"
                                     LabelText="Send From Email Address"
                                     CssClass="form-control"/>
                    </div>
                    <div class="form-group">
                    <asp:TextBox ID="txtTestToEmail" runat="server" 
                        Placeholder="Enter the to Email Address" RenderWrapper="True" Type="Email"
                        LabelText="Send To Email Address"
                                 CssClass="form-control"/>
                    </div>
                    <YAF:ModernButton ID="btnTestSmtp" runat="server" Text="Test Smtp Settings" CssClass="btn btn-info" 
                        EnableLoadingAnimation="True" OnClick="TestSmtp_Click" data-style="expand-left" />
                    <asp:PlaceHolder ID="SmtpInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblSmtpTestDetails" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Upgrade Database" ID="WizInitDatabase">
                    <h4>
                            <%# this.IsForumInstalled ? "Upgrade" : "Initialize"%> Database
                    </h4>
                    <p class="descriptionText">
                        Clicking "Next" will <%# this.IsForumInstalled ? "upgrade" : "initialize"%> your database to the latest version.
                    </p>
                    <asp:PlaceHolder runat="server"  Visible="<%# this.IsForumInstalled %>">
                    <div class="custom-control custom-checkbox">
                        <asp:CheckBox ID="UpgradeExtensions" Checked="True" runat="server"  />
                        <label for="<%# this.UpgradeExtensions.ClientID %>">
                            Upgrade BBCode Extensions, File Extensions and Spam Words
                        </label>
                    </div>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Create Forum" ID="WizCreateForum">
                    <h4>
                        Create New Board
                    </h4>
                    <asp:TextBox ID="TheForumName" runat="server" 
                        Placeholder="Enter the name of the new Board"
                        RenderWrapper="True" LabelText="Board Name"
                                 CssClass="form-control"/>
                    <div class="form-group">
                        <asp:Label AssociatedControlId="TimeZones" id="Label6" 
                            runat="server">Guest User Time Zone</asp:Label>
                        <asp:DropDownList ID="TimeZones" runat="server" DataTextField="Name" DataValueField="Value" 
                            CssClass="custom-select" />
                    </div>
                    <div class="form-group">
                        <asp:Label AssociatedControlId="Culture" id="Label7" 
                            runat="server">Guest User Culture (Language)</asp:Label>
                        <asp:DropDownList ID="Culture" runat="server" 
                            CssClass="custom-select" />
                    </div>
                    <div class="form-group">
                        <asp:Label AssociatedControlId="ForumEmailAddress" 
                                   runat="server">Forum Email Address</asp:Label>
                    <asp:TextBox ID="ForumEmailAddress" runat="server"
                                 Placeholder="Enter the forum email address"  
                                 RenderWrapper="True"
                                 LabelText="Forum Email"
                                 Type="Email"
                                 CssClass="form-control"/>
                    </div>
                    <div class="form-group">
                        <asp:Label AssociatedControlId="ForumBaseUrlMask" 
                                   runat="server">Forum Base Url Mask</asp:Label>
                    <asp:TextBox ID="ForumBaseUrlMask" runat="server" 
                        Placeholder="Enter the Base Url mask for the forum" RenderWrapper="True" 
                        LabelText="Forum Base Url Mask"
                        Type="Url"
                                 CssClass="form-control"/>
                    </div>
                    <hr/>
                    <div class="custom-control custom-radio">
                        <asp:RadioButtonList ID="UserChoice" runat="server" 
                                             AutoPostBack="true"
                                             OnSelectedIndexChanged="UserChoice_SelectedIndexChanged"
                                             RepeatLayout="UnorderedList"
                                             CssClass="list-unstyled">
                            <asp:ListItem Text="Create New Admin User" Selected="true" Value="create"></asp:ListItem>
                            <asp:ListItem Text="Use Existing User" Value="existing"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <asp:PlaceHolder ID="ExistingUserHolder" runat="server" Visible="false">
                        <div class="form-group">
                    <asp:TextBox ID="ExistingUserName" runat="server" 
                        Placeholder="Enter the name of the existing user to make the admin" RenderWrapper="True" 
                        LabelText="Existing User Name"
                                 CssClass="form-control"/>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="CreateAdminUserHolder" runat="server">
                        <div class="form-group"><asp:TextBox ID="UserName" runat="server"
                        Placeholder="Enter the name of the admin user" RenderWrapper="True" 
                        LabelText="Admin User Name"
                                 CssClass="form-control"/>
                        </div>
                        <div class="form-group"><asp:TextBox ID="AdminEmail" runat="server" 
                        Placeholder="Enter the administrators email address" RenderWrapper="True" 
                        LabelText="Admin E-mail"
                        Type="Email"
                                 CssClass="form-control"/>
                        </div>
                        <div class="form-group"> <asp:TextBox ID="Password1" runat="server" 
                        Placeholder="Enter the password of the admin user." RenderWrapper="True" 
                        LabelText="Admin Password"
                        TextMode="Password" Type="Password"
                                 CssClass="form-control"/>
                        </div>
                        <div class="form-group"> <asp:TextBox ID="Password2" runat="server"
                        Placeholder="Verify the password" RenderWrapper="True" 
                        LabelText="Confirm Password"
                        TextMode="Password" Type="Password"
                                 CssClass="form-control"/>
                        </div>
                        <div class="form-group"><asp:TextBox runat="server" ID="SecurityQuestion" 
                        Placeholder="The question you will be asked when you need to retrieve your lost password" 
                        RenderWrapper="True" 
                        LabelText="Security Question"
                                 CssClass="form-control"/>
                        </div>
                        <div class="form-group"><asp:TextBox runat="server" ID="SecurityAnswer" 
                        Placeholder="The answer to the security question" RenderWrapper="True" 
                        LabelText="Security Answer"
                                 CssClass="form-control"/>
                        </div>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Migrate Users" ID="WizMigrateUsers">
                    <h4>
                        Migrate Roles and Users
                    </h4>
                    <p>
                        <asp:Label ID="lblMigrateUsersCount" runat="server" Text="0"></asp:Label>
                        user(s) found in your forum database.</p>
                    <p class="descriptionText">
                        Click "Next" to start the upgrade (migration) of all roles and users from your old
                        Yet Another Forum.NET database to the .NET Provider Model.
                    </p> 
                    <div class="infoMessage">
                        <span class="infoLabel">Note</span> 
                        The migration task can take a <strong>very</strong> long time depending on how many users are in your forum.
                    </div>
                    <asp:CheckBox ID="skipMigration" runat="server" Text="Disable Migration (not recommended)"
                        Visible="False" />
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Migrating Users..." ID="WizMigratingUsers">
                    <h4>
                        Migrating Users...
                    </h4>
                    Please do not navigate away from this page while the migration is in progress. It
                    can take a very long time to perform the migration.
                    <asp:UpdatePanel ID="LoadingCheckPanel" runat="server">
                        <ContentTemplate>
                            <asp:Timer ID="UpdateStatusTimer" runat="server" Interval="5000" OnTick="UpdateStatusTimer_Tick" />
                            <div style="text-align: center" class="infoMessage">
                                <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
                                <br />
                                <strong>Migrating Roles and Users...</strong>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:WizardStep>
                <asp:WizardStep runat="server" StepType="Finish" Title="Finished" ID="WizFinished">
                    <h4>
                        <%# this.IsForumInstalled ? "Upgrade" : "Setup"%> Finished
                    </h4>
                    <p class="descriptionText">Clicking Finish will take you to the Forum main page.</p>
                    <p>Your forum has now been <%# this.IsForumInstalled ? "upgraded" : "Setup"%> to the latest version.</p>
                </asp:WizardStep>
            </WizardSteps>
            <FinishNavigationTemplate>
                <YAF:ModernButton ID="FinishPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                        Text="Previous" CssClass="btn btn-secondary" />
                <YAF:ModernButton ID="FinishButton" runat="server" CssClass="btn btn-success" CommandName="MoveComplete"
                        Text="Finish" />
            </FinishNavigationTemplate>
            <LayoutTemplate>
                <div class="yafWizard modal fade" data-backdrop="false">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title" id="myModalLabel"><img src="../Content/Images/YafLogoSmall.png" 
                                    width="100" height="50" alt="small YAF.NET logo"/> <%# this.IsForumInstalled ? "Upgrade" : "Installation"%> Wizard
                                </h4>
                                <asp:PlaceHolder ID="headerPlaceHolder" runat="server" />
                            </div>
                            <div class="modal-body">
                                <asp:PlaceHolder ID="ErrorMessage" runat="server" Visible="false">
                                    <div class="warningMessageDismissable">
                                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                                        <asp:Literal runat="server" ID="ErrorMessageContent"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="WizardStepPlaceHolder" runat="server" />
                            </div>
                            <div class="modal-footer">
                                <asp:PlaceHolder ID="navigationPlaceHolder" runat="server" />
                            </div>
                        </div>
                    </div>
                </div>
            </LayoutTemplate>
            <StartNavigationTemplate>
                    <YAF:ModernButton ID="StartNextButton" CssClass="btn btn-primary" EnableLoadingAnimation="True" runat="server" 
                        CommandName="MoveNext" Text="Next" data-style="expand-left" />
            </StartNavigationTemplate>
            <StepNavigationTemplate>
                    <YAF:ModernButton ID="StepPreviousButton" runat="server" CssClass="btn btn-secondary" Visible="false"
                        CausesValidation="False" CommandName="MovePrevious" Text="Previous" />
                    <YAF:ModernButton ID="StepNextButton" runat="server" CssClass="btn btn-primary" EnableLoadingAnimation="True"
                        OnClientClick="return true;" CommandName="MoveNext" Text="Next" data-style="expand-left" />
            </StepNavigationTemplate>
        </asp:Wizard>
    </form>
    <script src="../Scripts/InstallWizard.comb.min.js" type="text/javascript" async></script>
</body>
</html>