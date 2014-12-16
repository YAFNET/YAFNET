<%@ Page Language="c#" AutoEventWireup="True" Inherits="YAF.Install._default" CodeBehind="default.aspx.cs" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>

<!doctype html>
<html lang="en">
<head runat="server" id="YafHead">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta id="YafMetaScriptingLanguage" http-equiv="Content-Script-Type" runat="server"
        name="scriptlanguage" content="text/javascript" />
    <meta id="YafMetaStyles" http-equiv="Content-Style-Type" runat="server" name="styles"
        content="text/css" />
   <title>YAF.NET <%# IsForumInstalled ? "Upgrade" : "Installation"%></title>
   <link href="../Content/InstallWizard.min.css" rel="stylesheet" type="text/css" />
   <link rel="shortcut icon" href="../favicon.ico" />
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
                            <asp:Label ID="lblHostingTrust" runat="server" CssClass="infoLabel pull-right">Unchecked</asp:Label>
                            Asp.Net Hosting Trust Level (if trust level is not "Full" or "Unrestricted" you will need to manually
                            modify all configuration files) ...
                        </li>
                        <li>
                            <asp:Label ID="lblPermissionApp" runat="server" CssClass="infoLabel pull-right">Unchecked</asp:Label>
                            YAF.NET Has Write Access to Root Application Directory ("~/") ...
                        </li>
                        <li>
                            <asp:Label ID="lblPermissionUpload" runat="server" CssClass="infoLabel pull-right">Unchecked</asp:Label>
                            YAF.NET Has Write Access to "~/Upload" directory ...
                        </li>
                    </ul>
                    <YAF:ModernButton ID="btnTestPermissions" runat="server" Text="Test Permissions" CssClass="buttonInfo" 
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
                    <YAF:ModernTextBox ID="txtCreatePassword1" runat="server" TextMode="Password" Type="Password" 
                        PlaceHolder="Enter the Config Password" RenderWrapper="True" 
                        LabelText="Config Password"/>
                    <YAF:ModernTextBox ID="txtCreatePassword2" runat="server" TextMode="Password" Type="Password"  
                        PlaceHolder="Re-Enter the Config Password" RenderWrapper="True" 
                        LabelText="Verify Password"/>
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
                        If this is an upgrade, please enter the configuration password you created when the forum was first installed, not the admin user password.
                    </div>
                    <YAF:ModernTextBox ID="txtEnteredPassword" runat="server" TextMode="Password" Type="Password" 
                        PlaceHolder="Enter the Config Password" RenderWrapper="True" 
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
                    <div class="formElement radio">
                        <asp:RadioButtonList ID="rblYAFDatabase" runat="server" AutoPostBack="true" 
                            OnSelectedIndexChanged="YAFDatabase_SelectedIndexChanged">
                            <asp:ListItem Text="Use Existing DB Connection String" Selected="true" Value="existing"></asp:ListItem>
                            <asp:ListItem Text="Create New DB Connection String" Value="create"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <asp:PlaceHolder ID="ExistingConnectionHolder" runat="server" Visible="true">
                        <h4>
                            Select Existing Connection String
                        </h4>
                        Select SQL Server Database Connection String:&nbsp;
                        <asp:DropDownList ID="lbConnections" runat="server" CssClass="selectConnection">
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
                        <div class="formElement">
                            <asp:Label ID="Parameter1_Name" runat="server" AssociatedControlID="Parameter1_Value" />
                            <YAF:ModernTextBox runat="server" ID="Parameter1_Value" Text="(local)" 
                                Placeholder="Enter the Data Source (Name or address of the sql server)"/>
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter2_Name" runat="server" AssociatedControlID="Parameter2_Value" />
                            <YAF:ModernTextBox runat="server" ID="Parameter2_Value" 
                                Placeholder="Enter the Database Name"/>
                        </div>
                        <asp:PlaceHolder runat="server" Visible="false">
                        <div class="formElement">
                            <asp:Label ID="Parameter3_Name" runat="server" AssociatedControlID="Parameter3_Value" />
                            <asp:TextBox runat="server" ID="Parameter3_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter4_Name" runat="server" AssociatedControlID="Parameter4_Value" />
                            <asp:TextBox runat="server" ID="Parameter4_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter5_Name" runat="server" AssociatedControlID="Parameter5_Value" />
                            <asp:TextBox runat="server" ID="Parameter5_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter6_Name" runat="server" AssociatedControlID="Parameter6_Value" />
                            <asp:TextBox runat="server" ID="Parameter6_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter7_Name" runat="server" AssociatedControlID="Parameter7_Value" />
                            <asp:TextBox runat="server" ID="Parameter7_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter8_Name" runat="server" AssociatedControlID="Parameter8_Value" />
                            <asp:TextBox runat="server" ID="Parameter8_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter9_Name" runat="server" AssociatedControlID="Parameter9_Value" />
                            <asp:TextBox runat="server" ID="Parameter9_Value" CssClass="standardTextInput" />
                        </div>
                        <div class="formElement">
                            <asp:Label ID="Parameter10_Name" runat="server" AssociatedControlID="Parameter10_Value" />
                            <asp:TextBox runat="server" ID="Parameter10_Value" CssClass="standardTextInput" />
                        </div>
                        </asp:PlaceHolder>
                        
                        <div class="checkbox">
                            <asp:CheckBox ID="Parameter11_Value" runat="server" Checked="true" Text="Use Integrated Security"
                            AutoPostBack="true" OnCheckedChanged="Parameter11_Value_CheckChanged" />
                        </div>
                        <asp:PlaceHolder runat="server" Visible="false">
                            <asp:CheckBox ID="Parameter12_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter13_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter14_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter15_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter16_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter17_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter18_Value" runat="server" Checked="true" AutoPostBack="true" />
                            <asp:CheckBox ID="Parameter19_Value" runat="server" Checked="true" AutoPostBack="true" />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="DBUsernamePasswordHolder" Visible="false" runat="server">
                            <YAF:ModernTextBox runat="server" ID="txtDBUserID" 
                                Placeholder="Enter the SQL User Name" RenderWrapper="True" 
                                LabelText="User ID"/>
                            <YAF:ModernTextBox runat="server" ID="txtDBPassword" 
                                Placeholder="Enter the SQL Password" RenderWrapper="True" 
                                LabelText="Password"/>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                    <hr/>
                    <YAF:ModernButton ID="btnTestDBConnection" runat="server" CssClass="buttonInfo" EnableLoadingAnimation="True" Text="Test Connection"
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
                    <YAF:ModernButton ID="btnTestDBConnectionManual" runat="server" CssClass="buttonInfo" EnableLoadingAnimation="True" 
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
                    <YAF:ModernTextBox ID="txtTestFromEmail" runat="server"
                        Placeholder="Enter the from Email Address" RenderWrapper="True" Type="Email"
                        LabelText="Send From Email Address"/>
                    <YAF:ModernTextBox ID="txtTestToEmail" runat="server" 
                        Placeholder="Enter the to Email Address" RenderWrapper="True" Type="Email"
                        LabelText="Send To Email Address"/>
                    <YAF:ModernButton ID="btnTestSmtp" runat="server" Text="Test Smtp Settings" CssClass="buttonInfo" 
                        EnableLoadingAnimation="True" OnClick="TestSmtp_Click" data-style="expand-left" />
                    <asp:PlaceHolder ID="SmtpInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblSmtpTestDetails" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Upgrade Database" ID="WizInitDatabase">
                    <h4>
                            <%# IsForumInstalled ? "Upgrade" : "Initialize"%> Database
                    </h4>
                    <p class="descriptionText">
                        Clicking "Next" will <%# IsForumInstalled ? "upgrade" : "initialize"%> your database to the latest version.
                    </p>
                    <div class="checkbox">
                        <asp:CheckBox ID="FullTextSupport" runat="server" 
                            Text="Attempt to Install FullText Search Support" />
                    </div>
                    <div class="checkbox">
                        <asp:CheckBox ID="UpgradeExtensions" Checked="True" Visible="<%# IsForumInstalled %>" runat="server" 
                            Text="Upgrade BBCode Extensions, File Extensions, Topic Status Lists and Spam Words" />
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Create Forum" ID="WizCreateForum">
                    <h4>
                        Create New Board
                    </h4>
                    <YAF:ModernTextBox ID="TheForumName" runat="server" 
                        Placeholder="Enter the name of the new Board"
                        RenderWrapper="True" LabelText="Board Name"/>
                    <div class="formElement">
                        <asp:Label AssociatedControlId="TimeZones" id="Label6" 
                            runat="server">Guest User Time Zone</asp:Label>
                        <asp:DropDownList ID="TimeZones" runat="server" DataTextField="Name" DataValueField="Value" 
                            CssClass="selectTimeZone" data-width="100%" />
                    </div>
                    <div class="formElement">
                        <asp:Label AssociatedControlId="Culture" id="Label7" 
                            runat="server">Guest User Culture (Language)</asp:Label>
                        <asp:DropDownList ID="Culture" runat="server" 
                            CssClass="selectCulture" data-width="100%" />
                    </div>
                    <YAF:ModernTextBox ID="ForumEmailAddress" runat="server" 
                        Placeholder="Enter the forum email address"  RenderWrapper="True"
                        LabelText="Forum Email"
                        Type="Email"/>
                    <YAF:ModernTextBox ID="ForumBaseUrlMask" runat="server" 
                        Placeholder="Enter the Base Url mask for the forum" RenderWrapper="True" 
                        LabelText="Forum Base Url Mask"
                        Type="Url"/>
                    <hr/>
                    <div class="formElement radio">
                        <asp:RadioButtonList ID="UserChoice" runat="server" AutoPostBack="true" 
                            OnSelectedIndexChanged="UserChoice_SelectedIndexChanged">
                            <asp:ListItem Text="Create New Admin User" Selected="true" Value="create"></asp:ListItem>
                            <asp:ListItem Text="Use Existing User" Value="existing"></asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <asp:PlaceHolder ID="ExistingUserHolder" runat="server" Visible="false">
                    <YAF:ModernTextBox ID="ExistingUserName" runat="server" 
                        Placeholder="Enter the name of the existing user to make the admin" RenderWrapper="True" 
                        LabelText="Existing User Name"/>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="CreateAdminUserHolder" runat="server">
                    <YAF:ModernTextBox ID="UserName" runat="server"
                        Placeholder="Enter the name of the admin user" RenderWrapper="True" 
                        LabelText="Admin User Name"/>
                    <YAF:ModernTextBox ID="AdminEmail" runat="server" 
                        Placeholder="Enter the administrators email address" RenderWrapper="True" 
                        LabelText="Admin E-mail"
                        Type="Email"/>
                    <YAF:ModernTextBox ID="Password1" runat="server" 
                        Placeholder="Enter the password of the admin user." RenderWrapper="True" 
                        LabelText="Admin Password"
                        TextMode="Password" Type="Password" />
                    <YAF:ModernTextBox ID="Password2" runat="server"
                        Placeholder="Verify the password" RenderWrapper="True" 
                        LabelText="Confirm Password"
                        TextMode="Password" Type="Password" />
                    <YAF:ModernTextBox runat="server" ID="SecurityQuestion" 
                        Placeholder="The question you will be asked when you need to retrieve your lost password" 
                        RenderWrapper="True" 
                        LabelText="Security Question"/>
                    <YAF:ModernTextBox runat="server" ID="SecurityAnswer" 
                        Placeholder="The answer to the security question" RenderWrapper="True" 
                        LabelText="Security Answer"/>
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
                                <asp:Image ID="LoadingImage" runat="server" ImageUrl="../Content/images/loader.gif" />
                                <br />
                                <strong>Migrating Roles and Users...</strong>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:WizardStep>
                <asp:WizardStep runat="server" StepType="Finish" Title="Finished" ID="WizFinished">
                    <h4>
                        <%# IsForumInstalled ? "Upgrade" : "Setup"%> Finished
                    </h4>
                    <p class="descriptionText">Clicking Finish will take you to the Forum main page.</p>
                    <p>Your forum has now been <%# IsForumInstalled ? "upgraded" : "Setup"%> to the latest version.</p>
                </asp:WizardStep>
            </WizardSteps>
            <FinishNavigationTemplate>
                <YAF:ModernButton ID="FinishPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
                        Text="Previous" CssClass="buttonStandard" />
                <YAF:ModernButton ID="FinishButton" runat="server" CssClass="buttonSuccess" CommandName="MoveComplete"
                        Text="Finish" />
            </FinishNavigationTemplate>
            <LayoutTemplate>
                <div class="yafWizard modal fade" data-backdrop="false">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h4 class="modal-title" id="myModalLabel"><img src="../Images/YafLogoSmall.png" 
                                    width="100" height="50" alt="small YAF.NET logo"/> <%# IsForumInstalled ? "Upgrade" : "Installation"%> Wizard
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
                    <YAF:ModernButton ID="StartNextButton" CssClass="buttonPrimary" EnableLoadingAnimation="True" runat="server" 
                        CommandName="MoveNext" Text="Next" data-style="expand-left" />
            </StartNavigationTemplate>
            <StepNavigationTemplate>
                    <YAF:ModernButton ID="StepPreviousButton" runat="server" CssClass="buttonStandard" Visible="false"
                        CausesValidation="False" CommandName="MovePrevious" Text="Previous" />
                    <YAF:ModernButton ID="StepNextButton" runat="server" CssClass="buttonPrimary" EnableLoadingAnimation="True"
                        OnClientClick="return true;" CommandName="MoveNext" Text="Next" data-style="expand-left" />
            </StepNavigationTemplate>
        </asp:Wizard>
    </form>
    <script src="../Scripts/InstallWizard.comb.min.js" type="text/javascript" async></script>
</body>
</html>