<%@ Page Language="c#" CodeFile="default.aspx.cs" AutoEventWireup="True" Inherits="YAF.Install._default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"> 
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" id="YafHead">
	<title>Yet Another Forum.NET Installation</title>
	<link type="text/css" rel="stylesheet" href="../resources/css/forum.css" />
	<link type="text/css" rel="stylesheet" href="../themes/CleanSlate/theme.css" />
	<style type="text/css">
		.lined
		{
			border-bottom: solid 1px #444;
			padding-bottom: 3px;
		}
		.errorinfo
		{
			color:Red;
			font-weight:bold
		}
		.successinfo
		{
			color:Green;
			font-weight:bold
		}		
	</style>
</head>
<body>
	<form runat="server">
		<div align="center">
			<asp:Wizard ID="InstallWizard" runat="server" ActiveStepIndex="0" BackColor="#EFF3FB"
				BorderColor="#B5C7DE" BorderWidth="1px" Font-Names="Verdana" Font-Size="Small"
				Width="650px" CellPadding="8" DisplaySideBar="False" OnActiveStepChanged="Wizard_ActiveStepChanged"
				OnFinishButtonClick="Wizard_FinishButtonClick" OnPreviousButtonClick="Wizard_PreviousButtonClick"
				OnNextButtonClick="Wizard_NextButtonClick" OnLoad="Wizard_Load">
				<StepStyle Font-Size="0.8em" ForeColor="#333333" />
				<SideBarStyle BackColor="#507CD1" Font-Size="0.9em" VerticalAlign="Top" />
				<NavigationButtonStyle BackColor="White" BorderColor="#507CD1" BorderStyle="Solid"
					BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284E98" />
				<WizardSteps>
					<asp:WizardStep runat="server" Title="Database Connection" ID="WizDatabaseConnection">
						<h3>YAF Database Connection</h3>
				    <asp:RadioButtonList ID="rblYAFDatabase" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblYAFDatabase_SelectedIndexChanged">
				        <asp:ListItem Text="Use Existing DB Connection String" Selected="true" Value="existing"></asp:ListItem>
				        <asp:ListItem Text="Create New DB Connection String" Value="create"></asp:ListItem>
				    </asp:RadioButtonList>	
						<br />
						
						<asp:Placeholder id="ExistingConnectionHolder" runat="server" Visible="true">						
						<h4 class="lined">Select Existing</h4>
						Select SQL Server Database Connection String:&nbsp;
						<asp:DropDownList ID="lbConnections" runat="server"></asp:DropDownList><br /><br />		
						</asp:Placeholder>
						
						<asp:Placeholder id="NewConnectionHolder" runat="server" Visible="false">	
						<h4 class="lined">Create New Connection String</h4>
						Note: Connection String will be saved as "yafnet".
						<br /><br />
						Data Source:<br/>
						<asp:textbox runat="server" id="txtDBDataSource" text="(local)" Width="500px" /><br/>
						<br />
						Initial Catalog:<br/>
						<asp:textbox runat="server" id="txtDBInitialCatalog" Width="500px"/><br/><br/>
						
						<asp:CheckBox ID="chkDBIntegratedSecurity" runat="server" Checked="true" Text="Use Integrated Security" AutoPostBack="true" OnCheckedChanged="chkDBIntegratedSecurity_CheckChanged" />
						<br/><br/>
						<asp:PlaceHolder ID="DBUsernamePasswordHolder" Visible="false" runat="server">													
							User ID:<br/>
							<asp:textbox runat="server" id="txtDBUserID" Width="500px"/><br/><br/>
							Password:<br/>
							<asp:textbox runat="server" id="txtDBPassword" Width="500px"/><br/><br/>
						</asp:PlaceHolder>
						</asp:Placeholder>

						<h4 class="lined">Test Database Connection</h4>
						<asp:Button ID="btnTestDBConnection" runat="server" Text="Test Connection" 
							OnClick="btnTestDBConnection_Click" />
							
						<asp:PlaceHolder ID="ConnectionInfoHolder" runat="server" Visible="false">
						<h4 class="lined">Connection Details</asp:Literal></h4>
						<div>
							<asp:Label ID="lblConnectionDetails" runat="server"></asp:Label>
						</div>
						</asp:PlaceHolder>
			
					</asp:WizardStep>			
					<asp:WizardStep runat="server" Title="Manually Modify Database Connection" ID="WizManualDatabaseConnection">
						<asp:PlaceHolder ID="NoWriteAppSettingsHolder" runat="server" Visible="false">
						<h4 class="lined">No Write Access to Application Settings</h4>
				    <p>Unable to modify AppSettings in your config file to set Database Connection due to no write access.</p>
				    <p>Please open the <b><asp:Label runat="server" ID="lblAppSettingsFile">web.config</asp:Label></b> file in the application root directory and add/update the following key: </p>
				    <blockquote><b>&gt;add key="YAF.ConnectionStringName" value="<asp:Label runat="server" ID="lblConnectionStringName"></asp:Label>" /&lt;</b></blockquote>
				    </asp:PlaceHolder>
			
					</asp:WizardStep>								
					<asp:WizardStep runat="server" Title="Validate Permissions" ID="WizValidatePermission">					  
						<strong>Validating Permissions</strong><br />
						<br />
						Validating file system permission so installation can continue (~/ is your web root):<br />
						
						<ul>
						    <li>AppSettings exist and are available... <b><asp:Label ID="lblPermissionAppSettings" runat="server" ForeColor="Gray">Unchecked</asp:Label></b></li>
						    <li>Has Write Access to Application Configuration (defaults to "~/app.config")... <b><asp:Label ID="lblPermissionApp" runat="server" ForeColor="Gray">Unchecked</asp:Label></b></li>
						    <li>Has Write Access to Database Configuration (defaults to "~/db.config")... <b><asp:Label ID="lblPermissionDB" ForeColor="Gray" runat="server">Unchecked</asp:Label></b></li>
						    <li>Has Write Access to "~/Upload" directory... <b><asp:Label ID="lblPermissionUpload" ForeColor="Gray" runat="server">Unchecked</asp:Label></b></li>
						</ul>
						
                    </asp:WizardStep>
					<asp:WizardStep runat="server" Title="Create Config Password" ID="WizCreatePassword">
						<strong>Create Config Password</strong><br />
						<br />
						Since this is the first time you install or upgrade this version of the forum, you
						need to create a configuration password. This password is stored in your web.config
						file and needs to be entered every time you want to upgrade the forum.<br />
						<br />
						Config Password:<br />
						<asp:TextBox ID="TextBox1" runat="server" Width="100%" TextMode="Password"></asp:TextBox>
						<br />
						<br />
						Verify Password:<br />
						<asp:TextBox ID="TextBox2" runat="server" Width="100%" TextMode="Password"></asp:TextBox>
					</asp:WizardStep>
					<asp:WizardStep runat="server" Title="Enter Config Password" ID="WizEnterPassword">
						<strong>Enter Config Password</strong><br />
						<br />
						You need to enter the configuration password to upgrade the forum. This is the configuration
						password you created when the forum was first installed, not the admin user password.<br />
						<br />
						Password:<br />
						<asp:TextBox ID="TextBox3" runat="server" TextMode="Password" Width="100%" OnTextChanged="Password_Postback"></asp:TextBox>
					</asp:WizardStep>
					<asp:WizardStep runat="server" Title="Upgrade Database" ID="WizInitDatabase">
						<strong>Initialize/Upgrade Database</strong><br />
						<br />
						Clicking next will initalize/upgrade your database to the latest version.<br />
                        <br />
                        <asp:CheckBox ID="FullTextSupport" runat="server" Text="Attempt to Install FullText Search Support" />
                    </asp:WizardStep>
					<asp:WizardStep runat="server" Title="Create Forum" ID="WizCreateForum">
						<strong>Create Forum</strong><br />
						<p>
							Forum Name:<br />
							<asp:TextBox ID="TheForumName" runat="server" />
							The name of your forum.
						</p>
						<p>
							Time Zone:<br />
							<asp:DropDownList ID="TimeZones" runat="server" DataTextField="Name" DataValueField="Value" />
						</p>
						<p>
							Forum Email:<br />
							<asp:TextBox ID="ForumEmailAddress" runat="server" />
							The official forum email address.
						</p>
						<p>
							SMTP Server:<br />
							Set SMTP server settings in the mail.config file.
						</p>
						<p>
						    <asp:RadioButtonList ID="UserChoice" runat="server" AutoPostBack="true" OnSelectedIndexChanged="UserChoice_SelectedIndexChanged">
						        <asp:ListItem Text="Create New Admin User" Selected="true" Value="create"></asp:ListItem>
						        <asp:ListItem Text="Use Existing User" Value="existing"></asp:ListItem>
						    </asp:RadioButtonList>
						</p>
						<asp:PlaceHolder ID="ExistingUserHolder" runat="server" Visible="false">
						<p>
							Existing User Name:<br />
							<asp:TextBox ID="ExistingUserName" runat="server" />
							The name of the existing user to make the admin.
						</p>
						</asp:PlaceHolder>
						<asp:PlaceHolder ID="CreateAdminUserHolder" runat="server">
						<p>
							Admin User Name:<br />
							<asp:TextBox ID="UserName" runat="server" />
							The name of the admin user.
						</p>
						<p>
							Admin E-mail:<br />
							<asp:TextBox ID="AdminEmail" runat="server" />
							The administrators email address.
						</p>
						<p>
							Admin Password:<br />
							<asp:TextBox ID="Password1" runat="server" TextMode="Password" />
							The password of the admin user.
						</p>
						<p>
							Confirm Password:<br />
							<asp:TextBox ID="Password2" runat="server" TextMode="Password" />
							Verify the password.
						</p>
						<p>
							Security Question:<br />
							<asp:TextBox runat="server" ID="SecurityQuestion" />
							The question you will be asked when you need to retrieve your lost password.
						</p>
						<p>
							Security Answer:<br />
							<asp:TextBox runat="server" ID="SecurityAnswer" />
							The answer to the security question.
						</p>
						</asp:PlaceHolder>
					</asp:WizardStep>
					<asp:WizardStep runat="server" Title="Migrate Users" ID="WizMigrateUsers">
						<strong>Migrate Roles and Users</strong><br />
						<br />
						Clicking next will migrate all roles and users from your old Yet Another Forum.NET database
						to the newer database format.<br />
						<br />
						<asp:CheckBox ID="skipMigration" runat="server" Text="Skip Migration" visible="False"/>
					</asp:WizardStep>
					<asp:WizardStep runat="server" StepType="Finish" Title="Finished" ID="WizFinished">
						<strong>Setup/Upgrade Finished</strong><br />
						<br />
						Your forum has now been setup or upgraded to the latest version.
					</asp:WizardStep>			
				</WizardSteps>
					
				
				<SideBarButtonStyle BackColor="#507CD1" Font-Names="Verdana" ForeColor="White" />
				<HeaderStyle BackColor="White" BorderStyle="Solid" BorderWidth="2px"
					Font-Bold="True" Font-Size="0.9em" ForeColor="Black" HorizontalAlign="Center" />
			    <HeaderTemplate>
                    <img src="../images/YAFLogo.jpg" alt="YAF Logo" /><br />Installation Wizard
                </HeaderTemplate>
			</asp:Wizard>
		</div>
	</form>
</body>
</html>
