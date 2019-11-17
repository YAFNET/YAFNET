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
   <title>YAF.NET <%# this.IsForumInstalled ? YAF.App_GlobalResources.Install.Upgrade : YAF.App_GlobalResources.Install.Installation %></title>
   <link href="../Content/InstallWizard.min.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="Form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
        <asp:Wizard ID="InstallWizard" runat="server" ActiveStepIndex="1" 
                    DisplaySideBar="False" 
                    OnActiveStepChanged="Wizard_ActiveStepChanged"
                    OnFinishButtonClick="Wizard_FinishButtonClick" 
                    OnPreviousButtonClick="Wizard_PreviousButtonClick"
                    OnNextButtonClick="Wizard_NextButtonClick">
            <StepStyle CssClass="wizStep" />
            <WizardSteps>
                <asp:WizardStep runat="server" Title="Welcome" ID="WizWelcome">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.WelcomeInstall %>
                    </h4>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.WelcomeInstallDesc %>
                    </p>
                    <p>
                        <%# YAF.App_GlobalResources.Install.WelcomeInstallText %>
                    </p>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Upgrade" ID="WizWelcomeUpgrade">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.WelcomeUpgrade %>
                    </h4>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.WelcomeUpgradeDesc %>
                    </p>
                    <ul class="standardList">
                        <li><%# YAF.App_GlobalResources.Install.CurrentVersion %>
                            <strong>YAF.NET v<asp:Literal runat="server" ID="CurrentVersionName"></asp:Literal></strong>
                        </li>
                        <li><%# YAF.App_GlobalResources.Install.UpgradeVersion %>
                            <strong>YAF.NET v<asp:Literal runat="server" ID="UpgradeVersionName"></asp:Literal></strong>
                        </li>
                    </ul>
                    <p>
                       
                    </p>
                   
                    <div class="warningMessage">
                        <%# YAF.App_GlobalResources.Install.WarningUpgrade %>
                    </div>
                    
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Validate Permissions" ID="WizValidatePermission">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.ValidatePermission %>
                    </h4>
                    <p>
                    <%# YAF.App_GlobalResources.Install.ValidatePermissionDesc %>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.ValidatePermissionText %>
                    </p>
                    <ul class="standardList">
                        <li>
                            <asp:Label ID="lblPermissionApp" runat="server" 
                                       CssClass="infoLabel float-right">
                                <%# YAF.App_GlobalResources.Install.Unchecked %>
                            </asp:Label>
                            <%# YAF.App_GlobalResources.Install.PermissionApp %>
                        </li>
                        <li>
                            <asp:Label ID="lblPermissionUpload" runat="server" 
                                       CssClass="infoLabel float-right">
                                <%# YAF.App_GlobalResources.Install.Unchecked %>
                            </asp:Label>
                            <%# YAF.App_GlobalResources.Install.PermissionUpload %>
                        </li>
                    </ul>
                    <YAF:ModernButton ID="btnTestPermissions" runat="server" 
                                      Text="<%# YAF.App_GlobalResources.Install.TestPermission %>" 
                                      CssClass="btn btn-info" 
                                      EnableLoadingAnimation="True"
                                      OnClick="TestPermissions_Click" 
                                      data-style="expand-left" />
                </asp:WizardStep> 
                <asp:WizardStep ID="WizCreatePassword" runat="server" Title="Create Config Password">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.CreatePassword %>
                    </h4>
                    <p><%# YAF.App_GlobalResources.Install.CreatePasswordText %>
                        
                    </p>
                    <p class="descriptionText"><%# YAF.App_GlobalResources.Install.PasswordFile %>
                        <asp:Label ID="lblConfigPasswordAppSettingFile" runat="server">app.config</asp:Label>
                    </p>
                    <div class="form-group">
                        <asp:TextBox ID="txtCreatePassword1" runat="server" 
                                     TextMode="Password"
                                     PlaceHolder="<%# YAF.App_GlobalResources.Install.EnterConfigPassword %>"
                                     LabelText="<%# YAF.App_GlobalResources.Install.ConfigPassword %>"
                                     CssClass="form-control"/>
                    </div>
                    <div class="form-group">
                        <asp:TextBox ID="txtCreatePassword2" runat="server" 
                                     TextMode="Password"
                                     PlaceHolder="<%# YAF.App_GlobalResources.Install.ReenterConfigPassword %>"
                                     LabelText="<%# YAF.App_GlobalResources.Install.ReenterConfigPassword %>"
                                     CssClass="form-control"/>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="<%# YAF.App_GlobalResources.Install.EnterConfigPassword %>" ID="WizEnterPassword">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.EnterConfigPassword %>
                    </h4>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.EnterConfigPasswordDesc %>
                    </p>
                    <div class="infoMessage">
                        <span class="infoLabel"><%# YAF.App_GlobalResources.Install.Note %></span> 
                        <%# YAF.App_GlobalResources.Install.UpgradeNote %>
                    </div>
                    <asp:TextBox ID="txtEnteredPassword" runat="server" TextMode="Password" Type="Password"
                                 PlaceHolder="<%# YAF.App_GlobalResources.Install.EnterConfigPassword %>" 
                                 RenderWrapper="True"
                                 CssClass="form-control"
                                 LabelText="Password"/>
                </asp:WizardStep>
                <asp:WizardStep ID="WizManuallySetPassword" runat="server" Title="Manually Set Config Password">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.ManuallyPassword %>
                    </h4>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.ManuallyPasswordDesc %>
                    </p>
                    <div class="errorMessage">
                        <span class="errorLabel"><%# YAF.App_GlobalResources.Install.Error %></span> <%# YAF.App_GlobalResources.Install.ManuallyPasswordError %>
                    </div>
                    <p><%# YAF.App_GlobalResources.Install.OpenFile %>
                        <strong>
                            <asp:Label runat="server" ID="lblAppSettingsFile2">web.config</asp:Label></strong>
                        <%# YAF.App_GlobalResources.Install.OpenFileDesc %>
                    </p>
                    <code>&lt;add key="YAF.ConfigPassword" value="<span style='color: #0000FF'>YourPassword</span>"/&gt;</code>
                    <br/>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Database Connection" ID="WizDatabaseConnection">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.DBConnection %>
                    </h4>
                    <div class="custom-control custom-radio">
                        <asp:RadioButtonList ID="rblYAFDatabase" runat="server" 
                                             AutoPostBack="true"
                                             OnSelectedIndexChanged="YafDatabaseSelectedIndexChanged"
                                             RepeatLayout="UnorderedList"
                                             CssClass="list-unstyled">
                            <asp:ListItem Selected="true" Value="existing" />
                            <asp:ListItem Value="create" />
                        </asp:RadioButtonList>
                    </div>
                    <asp:PlaceHolder ID="ExistingConnectionHolder" runat="server" Visible="true">
                        <h4>
                            <%# YAF.App_GlobalResources.Install.ConnectionExist %>
                        </h4>
                        <%# YAF.App_GlobalResources.Install.ConnectionExistDesc %>&nbsp;
                        <asp:DropDownList ID="lbConnections" runat="server" 
                                          CssClass="custom-select">
                        </asp:DropDownList>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="NewConnectionHolder" runat="server" Visible="false">
                        <h4>
                            <%# YAF.App_GlobalResources.Install.ConnectionNew %>
                        </h4>
                        <div class="infoMessage">
                            <span class="infoLabel"><%# YAF.App_GlobalResources.Install.Note %></span> 
                            <%# YAF.App_GlobalResources.Install.ConnectionNewNote %>"<asp:Label ID="lblConnStringAppSettingName"
                            runat="server" Text="yafnet" />".
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter1_Name" runat="server" 
                                       AssociatedControlID="Parameter1_Value" />
                            <asp:TextBox runat="server" ID="Parameter1_Value" 
                                         Text="(local)"
                                         Placeholder="<%# YAF.App_GlobalResources.Install.DataSource %>"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter2_Name" runat="server" 
                                       AssociatedControlID="Parameter2_Value" />
                            <asp:TextBox runat="server" ID="Parameter2_Value"
                                         CssClass="form-control"
                                         Placeholder="<%# YAF.App_GlobalResources.Install.DBName %>"/>
                        </div>
                        <asp:PlaceHolder runat="server" Visible="false">
                        <div class="form-group">
                            <asp:Label ID="Parameter3_Name" runat="server" 
                                       AssociatedControlID="Parameter3_Value" />
                            <asp:TextBox runat="server" ID="Parameter3_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter4_Name" runat="server" 
                                       AssociatedControlID="Parameter4_Value" />
                            <asp:TextBox runat="server" ID="Parameter4_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter5_Name" runat="server" 
                                       AssociatedControlID="Parameter5_Value" />
                            <asp:TextBox runat="server" ID="Parameter5_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter6_Name" runat="server" 
                                       AssociatedControlID="Parameter6_Value" />
                            <asp:TextBox runat="server" ID="Parameter6_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter7_Name" runat="server" 
                                       AssociatedControlID="Parameter7_Value" />
                            <asp:TextBox runat="server" ID="Parameter7_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter8_Name" runat="server" 
                                       AssociatedControlID="Parameter8_Value" />
                            <asp:TextBox runat="server" ID="Parameter8_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter9_Name" runat="server" 
                                       AssociatedControlID="Parameter9_Value" />
                            <asp:TextBox runat="server" ID="Parameter9_Value" 
                                         CssClass="form-control" />
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Parameter10_Name" runat="server" 
                                       AssociatedControlID="Parameter10_Value" />
                            <asp:TextBox runat="server" ID="Parameter10_Value" 
                                         CssClass="form-control" />
                        </div>
                        </asp:PlaceHolder>
                        
                        <div class="custom-control custom-checkbox">
                            <asp:CheckBox ID="Parameter11_Value" runat="server" 
                                          Checked="true" 
                                          Text="<%# YAF.App_GlobalResources.Install.Integrated %>"
                                          AutoPostBack="true" 
                                          OnCheckedChanged="Parameter11_Value_CheckChanged"/>
                        </div>
                        <asp:PlaceHolder runat="server" Visible="false">
                            <div class="form-group">
                                <asp:CheckBox ID="Parameter12_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter13_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter14_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter15_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter16_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter17_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter18_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                                <asp:CheckBox ID="Parameter19_Value" runat="server" 
                                              Checked="true" 
                                              AutoPostBack="true" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="DBUsernamePasswordHolder" Visible="false" runat="server">
                            <div class="form-group">
                                <asp:TextBox runat="server" ID="txtDBUserID" 
                                             Placeholder="<%# YAF.App_GlobalResources.Install.SqlUser %>" 
                                             RenderWrapper="True" 
                                             LabelText="<%# YAF.App_GlobalResources.Install.SqlUser %>"
                                             CssClass="form-control"/>
                            </div>
                            <div class="form-group">
                                <asp:TextBox runat="server" ID="txtDBPassword" 
                                             Placeholder="<%# YAF.App_GlobalResources.Install.SqlPass %>" 
                                             RenderWrapper="True" 
                                             LabelText="<%# YAF.App_GlobalResources.Install.SqlPass %>"
                                             CssClass="form-control"/>
                            </div>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                    <hr/>
                    <YAF:ModernButton ID="btnTestDBConnection" runat="server" 
                                      CssClass="btn btn-info" 
                                      EnableLoadingAnimation="True" 
                                      Text="<%# YAF.App_GlobalResources.Install.TestConnection %>"
                                      OnClick="TestDBConnection_Click" 
                                      OnClientClick="return true;" 
                                      data-style="expand-left" />
                    <asp:PlaceHolder ID="ConnectionInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblConnectionDetails" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Manually Modify Database Connection" ID="WizManualDatabaseConnection">
                    <asp:PlaceHolder ID="NoWriteAppSettingsHolder" runat="server" Visible="false">
                        <h4>
                            <%# YAF.App_GlobalResources.Install.NoWriteAppSettings %>
                        </h4>
                        <div class="errorMessage">
                            <span class="errorLabel">
                                <%# YAF.App_GlobalResources.Install.Error %>
                            </span> 
                            <%# YAF.App_GlobalResources.Install.NoWriteAppSettingsNote %>
                        </div>
                        <p>
                            <%# YAF.App_GlobalResources.Install.OpenFile %><strong>
                                <asp:Label runat="server" ID="lblAppSettingsFile">web.config</asp:Label></strong>
                            <%# YAF.App_GlobalResources.Install.OpenFileDesc %>
                        </p>
                        <code>&lt;add key="YAF.ConnectionStringName" value="<asp:Label runat="server" ID="lblConnectionStringName"></asp:Label>"/&gt;</code>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="NoWriteDBSettingsHolder" runat="server" Visible="false">
                        <h4>
                            <%# YAF.App_GlobalResources.Install.NoWriteConnSettings %>
                        </h4>
                        <div class="errorMessage">
                            <span class="errorLabel">
                                <%# YAF.App_GlobalResources.Install.Error %>
                            </span> 
                            <%# YAF.App_GlobalResources.Install.NoWriteConnSettingsNote %>
                        </div>
                        <p>
                            <%# YAF.App_GlobalResources.Install.OpenFile %><strong><asp:Label runat="server" ID="lblDBSettingsFile">db.config</asp:Label></strong>
                            <%# YAF.App_GlobalResources.Install.OpenFileDesc2 %>
                        </p>
                        <code>&lt;add name="<asp:Label runat="server" ID="lblDBConnStringName" />" connectionString="<asp:Label runat="server" ID="lblDBConnStringValue" />" /&gt;</code>
                    </asp:PlaceHolder>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.NextReady %>
                    </p>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Test Settings" ID="WizTestSettings">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.TestSettings %>
                    </h4>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.TestSettingsDesc %>
                    </p>
                    <p>
                        <%# YAF.App_GlobalResources.Install.TestSettingsText %>
                    </p>
                    <h4>
                        <%# YAF.App_GlobalResources.Install.ConnectionTest %>
                    </h4>
                    <YAF:ModernButton ID="btnTestDBConnectionManual" runat="server" 
                                      CssClass="btn btn-info" 
                                      EnableLoadingAnimation="True" 
                                      Text="<%# YAF.App_GlobalResources.Install.ConnectionTest %>" 
                                      OnClick="TestDBConnectionManual_Click" 
                                      data-style="expand-left" />
                    <asp:PlaceHolder ID="ManualConnectionInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblConnectionDetailsManual" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                    <hr />
                    <h4>
                        <%# YAF.App_GlobalResources.Install.MailTest %>
                    </h4>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.MailTestDesc %>
                    </p>
                    <div class="form-group">
                        <asp:TextBox ID="txtTestFromEmail" runat="server"
                                     Placeholder="<%# YAF.App_GlobalResources.Install.FromEmail %>" 
                                     RenderWrapper="True" 
                                     Type="Email"
                                     LabelText="<%# YAF.App_GlobalResources.Install.FromEmail %>"
                                     CssClass="form-control"/>
                    </div>
                    <div class="form-group">
                    <asp:TextBox ID="txtTestToEmail" runat="server" 
                                 Placeholder="<%# YAF.App_GlobalResources.Install.ToEmail %>" 
                                 RenderWrapper="True" 
                                 Type="Email"
                                 LabelText="<%# YAF.App_GlobalResources.Install.ToEmail %>"
                                 CssClass="form-control"/>
                    </div>
                    <YAF:ModernButton ID="btnTestSmtp" runat="server" 
                                      Text="<%# YAF.App_GlobalResources.Install.TestEmail %>" 
                                      CssClass="btn btn-info" 
                                      EnableLoadingAnimation="True" 
                                      OnClick="TestSmtp_Click" 
                                      data-style="expand-left" />
                    <asp:PlaceHolder ID="SmtpInfoHolder" runat="server" Visible="false">
                        <hr/>
                        <asp:Literal ID="lblSmtpTestDetails" runat="server"></asp:Literal>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Upgrade Database" ID="WizInitDatabase">
                    <h4>
                            <%# this.IsForumInstalled ? YAF.App_GlobalResources.Install.Upgrade : YAF.App_GlobalResources.Install.Initialize %> <%# YAF.App_GlobalResources.Install.Database %>
                    </h4>
                    <p class="descriptionText">
                        <%# this.IsForumInstalled ? YAF.App_GlobalResources.Install.NextUpgradeDb : YAF.App_GlobalResources.Install.NextInitDb %>
                    </p>
                    <asp:PlaceHolder runat="server"  Visible="<%# this.IsForumInstalled %>">
                    <div class="custom-control custom-checkbox">
                        <asp:CheckBox ID="UpgradeExtensions" Checked="True" runat="server"  />
                        <label for="<%# this.UpgradeExtensions.ClientID %>">
                            <%# YAF.App_GlobalResources.Install.UpgradeExtensions %>
                        </label>
                    </div>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Create Forum" ID="WizCreateForum">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.CreateBoard %>
                    </h4>
                    <div class="form-group">
                        <asp:TextBox ID="TheForumName" runat="server" 
                                     Placeholder="<%# YAF.App_GlobalResources.Install.BoardName %>"
                                     RenderWrapper="True" 
                                     LabelText="Board Name"
                                     CssClass="form-control"/>
                    </div>
                    <div class="form-group">
                        <asp:Label id="Label6" runat="server" 
                                   AssociatedControlId="TimeZones">
                            <%# YAF.App_GlobalResources.Install.TimeZone %>
                        </asp:Label>
                        <asp:DropDownList ID="TimeZones" runat="server" 
                                          DataTextField="Name" 
                                          DataValueField="Value" 
                                          CssClass="custom-select" />
                    </div>
                    <div class="form-group">
                        <asp:Label id="Label7" runat="server" 
                                   AssociatedControlId="Culture">
                            <%# YAF.App_GlobalResources.Install.Culture %>
                        </asp:Label>
                        <asp:DropDownList ID="Culture" runat="server" 
                                          CssClass="custom-select" />
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlId="ForumEmailAddress">
                            <%# YAF.App_GlobalResources.Install.ForumEmail %>
                        </asp:Label>
                    <asp:TextBox ID="ForumEmailAddress" runat="server"
                                 Placeholder="<%# YAF.App_GlobalResources.Install.ForumEmail %>"  
                                 RenderWrapper="True"
                                 LabelText="Forum Email"
                                 Type="Email"
                                 CssClass="form-control"/>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlId="ForumBaseUrlMask">
                            <%# YAF.App_GlobalResources.Install.ForumUrl %>
                        </asp:Label>
                        <asp:TextBox ID="ForumBaseUrlMask" runat="server" 
                                     Placeholder="<%# YAF.App_GlobalResources.Install.ForumUrl %>" 
                                     RenderWrapper="True" 
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
                            <asp:ListItem Selected="true" Value="create"/>
                            <asp:ListItem Value="existing" />
                        </asp:RadioButtonList>
                    </div>
                    <asp:PlaceHolder ID="ExistingUserHolder" runat="server" Visible="false">
                        <div class="form-group">
                            <asp:TextBox ID="ExistingUserName" runat="server" 
                                         Placeholder="<%# YAF.App_GlobalResources.Install.ExistingUserName %>" 
                                         RenderWrapper="True" 
                                         LabelText="Existing User Name"
                                         CssClass="form-control"/>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="CreateAdminUserHolder" runat="server">
                        <div class="form-group">
                            <asp:TextBox ID="UserName" runat="server"
                                         Placeholder="<%# YAF.App_GlobalResources.Install.AdminName %>" 
                                         RenderWrapper="True" 
                                         LabelText="Admin User Name"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:TextBox ID="AdminEmail" runat="server" 
                                         Placeholder="<%# YAF.App_GlobalResources.Install.AdminEmail %>" 
                                         RenderWrapper="True" 
                                         LabelText="Admin E-mail"
                                         Type="Email"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:TextBox ID="Password1" runat="server" 
                                         Placeholder="<%# YAF.App_GlobalResources.Install.AdminPassword %>" 
                                         RenderWrapper="True" 
                                         LabelText="Admin Password"
                                         TextMode="Password" 
                                         Type="Password"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:TextBox ID="Password2" runat="server"
                                         Placeholder="<%# YAF.App_GlobalResources.Install.AdminPassword2 %>" 
                                         RenderWrapper="True" 
                                         LabelText="Confirm Password"
                                         TextMode="Password" 
                                         Type="Password"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:TextBox runat="server" ID="SecurityQuestion" 
                                         Placeholder="<%# YAF.App_GlobalResources.Install.SecurityQuestion %>" 
                                         RenderWrapper="True" 
                                         LabelText="Security Question"
                                         CssClass="form-control"/>
                        </div>
                        <div class="form-group">
                            <asp:TextBox runat="server" ID="SecurityAnswer" 
                                         Placeholder="<%# YAF.App_GlobalResources.Install.SecurityAnswer %>" 
                                         RenderWrapper="True" 
                                         LabelText="Security Answer"
                                         CssClass="form-control"/>
                        </div>
                    </asp:PlaceHolder>
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Migrate Users" ID="WizMigrateUsers">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.Migrate %>
                    </h4>
                    <p>
                        <asp:Label ID="lblMigrateUsersCount" runat="server" Text="0"></asp:Label>
                        <%# YAF.App_GlobalResources.Install.MigrateCount %>
                    </p>
                    <p class="descriptionText">
                        <%# YAF.App_GlobalResources.Install.MigrateDesc %>
                    </p> 
                    <div class="infoMessage">
                        <span class="infoLabel">
                            <%# YAF.App_GlobalResources.Install.Note %>
                        </span> 
                        <%# YAF.App_GlobalResources.Install.MigrateNote %>
                    </div>
                    <asp:CheckBox ID="skipMigration" runat="server" 
                                  Text="<%# YAF.App_GlobalResources.Install.MigrateSkip %>"
                                  Visible="False" />
                </asp:WizardStep>
                <asp:WizardStep runat="server" Title="Migrating Users..." ID="WizMigratingUsers">
                    <h4>
                        <%# YAF.App_GlobalResources.Install.MigrateUsers %>
                    </h4>
                    <%# YAF.App_GlobalResources.Install.MigrateUsersText %>
                    <asp:UpdatePanel ID="LoadingCheckPanel" runat="server">
                        <ContentTemplate>
                            <asp:Timer ID="UpdateStatusTimer" runat="server" Interval="5000" OnTick="UpdateStatusTimer_Tick" />
                            <div style="text-align: center" class="infoMessage">
                                <div class="fa-3x">
                                    <i class="fas fa-spinner fa-pulse"></i>
                                </div>
                                <br />
                                <strong>
                                    <%# YAF.App_GlobalResources.Install.Migrate %>
                                </strong>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </asp:WizardStep>
                <asp:WizardStep runat="server" StepType="Finish" Title="Finished" ID="WizFinished">
                    <h4>
                        <%# this.IsForumInstalled ? YAF.App_GlobalResources.Install.Upgrade : YAF.App_GlobalResources.Install.Setup%> <%# YAF.App_GlobalResources.Install.Finished %>
                    </h4>
                    <p class="descriptionText"><%# YAF.App_GlobalResources.Install.FinishDesc %></p>
                    <p><%# this.IsForumInstalled ? YAF.App_GlobalResources.Install.UpgradeFinish : YAF.App_GlobalResources.Install.InitFinish%></p>
                </asp:WizardStep>
            </WizardSteps>
            <FinishNavigationTemplate>
                <YAF:ModernButton ID="FinishPreviousButton" runat="server" 
                                  CausesValidation="False" 
                                  CommandName="MovePrevious"
                                  Text="<%# YAF.App_GlobalResources.Install.Previous %>" 
                                  CssClass="btn btn-secondary" />
                <YAF:ModernButton ID="FinishButton" runat="server" 
                                  CssClass="btn btn-success" 
                                  CommandName="MoveComplete"
                                  Text="<%# YAF.App_GlobalResources.Install.Finish %>" />
            </FinishNavigationTemplate>
            <LayoutTemplate>
                <div class="yafWizard modal fade" data-backdrop="false">
                    <div class="modal-dialog">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="myModalLabel">
                                   <svg style="height:50px"  xmlns="http://www.w3.org/2000/svg" viewBox="0 0 4615.94 2142.98">
                                       <title>YAFLogo</title>
                                       <g id="Layer_2" data-name="Layer 2">
                                           <path d="M1879.13,59.73,1058.13.54C944-7.68,844,79,835.72,193.05l-18,249.73L909,435.24l17-235.69c4.62-64.07,61.63-113.41,125.7-108.78l821,59.17c64.06,4.63,113.38,61.64,108.78,125.69l-45.5,631.18a115.28,115.28,0,0,1-11,41.55L2016,964a206.23,206.23,0,0,0,10.1-50.65l45.51-631.17C2079.84,168,1993.23,67.94,1879.13,59.73Z" transform="translate(0 0)" style="fill:#4e4596;fill-rule:evenodd"/>
                                           <path d="M1063.38,1617.35q3.16-20.26,6.34-40.55l-240.41,28.69-207.44,155.2-15.35-128.61L373.37,1659.9c-83.83,10-161-50.73-171-134.56l-94.12-788.6c-10-83.83,50.74-161,134.56-171l1025.74-122.4c83.83-10,161,50.72,171,134.56s20,168,30.07,252L1579.58,847,1546,565.17c-17-142.56-147.56-245.27-290.12-228.26L230.1,459.32C87.55,476.33-15.16,606.88,1.84,749.44L96,1538c17,142.56,147.57,245.27,290.12,228.26l126.76-15.13,24.63,206.51,333-249.19,193.56-23.1A206.21,206.21,0,0,1,1063.38,1617.35Z" transform="translate(0 0)" style="fill:#3a60aa"/>
                                           <path d="M2221.77,958.74,1589.4,859.92l-110-17.18-70.93-11.08c-113-17.67-220,60.35-237.61,173.37q-45.68,292.37-91.37,584.69l-6.33,40.55a206.22,206.22,0,0,0,.68,68c15.13,85.08,82.85,155.52,172.71,169.58l429.15,67.06,258,208,25.58-163.72,100.5,15.7c113,17.66,220-60.36,237.6-173.38l97.71-625.23C2412.8,1083.32,2334.78,976.4,2221.77,958.74Zm84,223.64-97.7,625.23c-9.91,63.44-70.83,107.91-134.27,98l-189.87-29.66-15.36,98.31-154.92-124.92-453.09-70.81c-48.68-7.61-86.17-45.22-96.52-91a115.41,115.41,0,0,1-1.48-43.29q5.14-32.84,10.27-65.67,43.73-279.78,87.44-559.58c9.92-63.46,70.82-107.89,134.27-98l96,15,110,17.19,607.26,94.89C2271.25,1058,2315.69,1118.94,2305.77,1182.38Z" transform="translate(0 0)" style="fill:#d43342"/>
                                       </g>
                                       <g id="Ebene_2" data-name="Ebene 2">
                                           <path d="M2916,1274.49c0,95.2-79.1,173.6-175,173.6v-65.8c60.21,0,108.5-48.3,108.5-107.8v-32.9c-29.4,24.5-67.2,38.5-108.5,38.5-95.89,0-172.19-79.8-174.29-175V934.29h65.1v171.5c0,60.2,49,108.5,109.19,108.5a108.14,108.14,0,0,0,108.5-108.5V934.29H2916Z" transform="translate(0 0)" style="fill:#3a60aa"/>
                                           <path d="M3373.84,1279.39c-38.5-6.3-67.9-36.4-88.9-67.2-32.2,42.7-82.6,68.6-139.3,68.6-96.6,0-175-78.4-175-175.7,0-96.6,78.4-174.3,175-174.3,97.3,0,174.3,77.7,174.3,174.3v14.7c0,39.2,21.7,77,53.9,93.8Zm-228.2-282.1c-60.9,0-109.2,47.6-109.2,107.8,0,60.9,48.3,109.9,109.2,109.9s108.5-49,108.5-109.9C3254.14,1044.89,3206.54,997.29,3145.64,997.29Z" transform="translate(0 0)" style="fill:#3a60aa"/>
                                           <path d="M3462.73,932.89l-.7.7h98.7v65.8H3462l.7,281.4h-65.1V931.49a173.87,173.87,0,0,1,174.3-174.3v65.1C3511.73,822.29,3462.73,872.69,3462.73,932.89Z" transform="translate(0 0)" style="fill:#3a60aa"/>
                                           <path d="M3590.82,1240.8a46.38,46.38,0,0,1,46.14-46.14q18.79,0,32.47,13.67t13.67,32.47a46.28,46.28,0,0,1-78.78,32.81Q3590.82,1260.28,3590.82,1240.8Z" transform="translate(0 0)" style="fill:#444342"/>
                                           <path d="M3704.43,827.91l322,330.95V845.34h59.48V1301l-322-329.48v309.32h-59.47Z" transform="translate(0 0)" style="fill:#444342"/>
                                           <path d="M4117.46,845.34h241.31v59.47H4176.25V1013.5h182.52v58.79H4176.25V1222h182.52v58.79H4117.46Z" transform="translate(0 0)" style="fill:#444342"/>
                                           <path d="M4349.68,845.34h266.26v59.47H4512v376h-58.79v-376H4349.68Z" transform="translate(0 0)" style="fill:#444342"/>
                                       </g>
                                   </svg>
                                    <%# this.IsForumInstalled ? YAF.App_GlobalResources.Install.Upgrade : YAF.App_GlobalResources.Install.Installation%> Wizard
                                </h5>
                                <span>
                                    <asp:DropDownList ID="Languages" runat="server" 
                                                      AutoPostBack="true"
                                                      CssClass="custom-select"
                                                      Visible='<%# !YAF.Configuration.Config.IsDotNetNuke %>'>
                                        <asp:ListItem Text="Arabic" Value="ar"/>
                                        <asp:ListItem Text="Chinese (Simplified)" Value="zh-CN"/>
                                        <asp:ListItem Text="Chinese (Traditional)" Value="zh-TW"/>
                                        <asp:ListItem Text="Croatian" Value="hr" />
                                        <asp:ListItem Text="Czech" Value="cs" />
                                        <asp:ListItem Text="Danish" Value="da" />
                                        <asp:ListItem Text="Dutch" Value="nl" />
                                        <asp:ListItem Text="English" Value="en-US" />
                                        <asp:ListItem Text="Estonian" Value="et" />
                                        <asp:ListItem Text="Finnish" Value="fi" />
                                        <asp:ListItem Text="French" Value="fr"/>
                                        <asp:ListItem Text="German" Value="de-DE" />
                                        <asp:ListItem Text="Hebrew" Value="iw" />
                                        <asp:ListItem Text="Italian" Value="it"/>
                                        <asp:ListItem Text="Lithuanian" Value="lt"/>
                                        <asp:ListItem Text="Norwegian" Value="no"/>
                                        <asp:ListItem Text="Persian" Value="fa"/>
                                        <asp:ListItem Text="Polish" Value="pl"/>
                                        <asp:ListItem Text="Portuguese" Value="pt"/>
                                        <asp:ListItem Text="Romanian" Value="ro"/>
                                        <asp:ListItem Text="Russian" Value="ru"/>
                                        <asp:ListItem Text="Slovak" Value="sk"/>
                                        <asp:ListItem Text="Spanish" Value="es"/>
                                        <asp:ListItem Text="Swedish" Value="sv"/>
                                        <asp:ListItem Text="Turkish" Value="tr"/>
                                        <asp:ListItem Text="Vietnamese" Value="vi"/>
                                    </asp:DropDownList>
                                </span>
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
                        CommandName="MoveNext" Text="<%# YAF.App_GlobalResources.Install.Next %>" data-style="expand-left" />
            </StartNavigationTemplate>
            <StepNavigationTemplate>
                    <YAF:ModernButton ID="StepPreviousButton" runat="server" 
                                      CssClass="btn btn-secondary" 
                                      Visible="false"
                                      CausesValidation="False"
                                      CommandName="MovePrevious" 
                                      Text="<%# YAF.App_GlobalResources.Install.Previous %>" />
                    <YAF:ModernButton ID="StepNextButton" runat="server" 
                                      CssClass="btn btn-primary" 
                                      EnableLoadingAnimation="True"
                                      OnClientClick="return true;" 
                                      CommandName="MoveNext" 
                                      Text="<%# YAF.App_GlobalResources.Install.Next %>"
                                      data-style="expand-left" />
            </StepNavigationTemplate>
        </asp:Wizard>
    </form>
    <script src="../Scripts/InstallWizard.comb.min.js" type="text/javascript" async></script>
</body>
</html>