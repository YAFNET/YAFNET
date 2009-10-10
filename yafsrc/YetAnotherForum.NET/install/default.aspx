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
			border-bottom: solid 1px #999;
			padding-bottom: 3px;
		}
		.wizNav
		{
			border-top: solid 1px #999;
			padding-top: 3px;
		}
		.errorinfo
		{
			color: Red;
			font-weight: bold;
		}
		.successinfo
		{
			color: Green;
			font-weight: bold;
		}
		.wizButton
		{
			color: #284E98;
			background-color: White;
			border-color: #999999;
			border-width: 1px;
			border-style: Solid;
			font-family: Verdana;
			font-size: 10pt;
			padding: 5px;
			margin: 5px;
		}
		.wizButton:hover
		{
			background-color: #ddd;
			color: #000;
		}
		.wizLoader
		{
			background: url(../resources/images/loading-white.gif) white no-repeat center center;
			z-index: 9999;
		}
		.wizMain td
		{
			padding: 10px;			
		}
		.wizMain
		{
			margin: 0 auto;
		}
	</style>
</head>
<body>
	<form runat="server">

	<script type="text/javascript">
		jQuery('form').submit(function() {
		jQuery('.wizStep').animate({ opacity: '0.4' }, 'fast');
		jQuery('.wizMain').addClass('wizLoader');			
		});	
	</script>

	<asp:ScriptManager ID="ScriptManager1" runat="server">
	</asp:ScriptManager>
	<div id="wizContainer">
		<asp:Wizard ID="InstallWizard" runat="server" ActiveStepIndex="1" BackColor="#EEEEEE"
			BorderColor="#999999" BorderWidth="1px" Font-Names="Verdana" Font-Size="9pt" Width="650px"
			CssClass="wizMain" DisplaySideBar="False" OnActiveStepChanged="Wizard_ActiveStepChanged"
			OnFinishButtonClick="Wizard_FinishButtonClick" OnPreviousButtonClick="Wizard_PreviousButtonClick"
			OnNextButtonClick="Wizard_NextButtonClick">
			<StepStyle CssClass="wizStep" />
			<WizardSteps>
				<asp:WizardStep runat="server" Title="Validate Permissions" ID="WizValidatePermission">
					<h4 class="lined">
						Validate Permissions</h4>
					<p>
						YetAnotherForum.NET Installation is easier when YAF has write access to your .config
						files. Click below to test is permissions are correct. If they are not, you can
						still use the installation wizard.
					</p>
					<ul>
						<li>Asp.Net Hosting Trust Level (if trust level is not "Full" you will need to manually modify all configuration files)... <b>
							<asp:Label ID="lblHostingTrust" runat="server" ForeColor="Gray">Unchecked</asp:Label></b></li>					
						<li>YAF Has Write Access to Root Application Directory ("~/")... <b>
							<asp:Label ID="lblPermissionApp" runat="server" ForeColor="Gray">Unchecked</asp:Label></b></li>
						<li>YAF Has Write Access to "~/Upload" directory... <b>
							<asp:Label ID="lblPermissionUpload" ForeColor="Gray" runat="server">Unchecked</asp:Label></b></li>
					</ul>
					<br />
					<asp:Button ID="btnTestPermissions" runat="server" Text="Test Permissions" CssClass="wizButton"
						OnClick="btnTestPermissions_Click" />
				</asp:WizardStep>
				<asp:WizardStep ID="WizCreatePassword" runat="server" Title="Create Config Password">
					<h4 class="lined">
						Create Config Password</h4>
					<p>
						Since this is the first time you install or upgrade this version of the forum, you
						need to create a configuration password for security. This password is stored with your appSettings
						in the <asp:Label ID="lblConfigPasswordAppSettingFile" runat="server">app.config</asp:Label> file.</p>
					<p>
						Config Password:<br />
						<asp:TextBox ID="txtCreatePassword1" runat="server" Width="100%" TextMode="Password"></asp:TextBox>
					</p>
					<p>
						Verify Password:<br />
						<asp:TextBox ID="txtCreatePassword2" runat="server" Width="100%" TextMode="Password"></asp:TextBox>
					</p>
				</asp:WizardStep>
				<asp:WizardStep runat="server" Title="Enter Config Password" ID="WizEnterPassword">
					<h4 class="lined">Enter Config Password</h4>
					<p>
						Enter the configuration password to upgrade or manually install the forum.</p>
					<p>
						If this is an upgrade, please enter the configuration password you created when
						the forum was first installed, not the admin user password.</p>
					<br />
					Password:<br />
					<asp:TextBox ID="txtEnteredPassword" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
				</asp:WizardStep>				
				<asp:WizardStep ID="WizManuallySetPassword" runat="server" Title="Manually Set Config Password">
					<h4 class="lined">
						No Write Access to App Settings</h4>
					<p>
						Unable to modify your config file to write the installation password due to permission.</p>
					<p>
						Please open the <b>
							<asp:Label runat="server" ID="lblAppSettingsFile2">web.config</asp:Label></b> file
						in the application root directory and add/update the following key in the &lt;AppSettings&gt;
						section:
					</p>
					<blockquote>
						<b>&lt;add key="YAF.ConfigPassword" value="<span style='color:#0000FF'>YourPassword</span>"
							/&gt;</b></blockquote>	
							
					<p>
						<a href="http://wiki.yetanotherforum.net/TroubleShooting Write Permission.ashx">See this Topic in the YAF Wiki for more Information</a>
					</p>		
					
					<p>Click "Next" when you are ready.</p>					
				</asp:WizardStep>				
				<asp:WizardStep runat="server" Title="Database Connection" ID="WizDatabaseConnection">
					<h3 class="lined">
						YAF Database Connection</h3>
					<asp:RadioButtonList ID="rblYAFDatabase" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblYAFDatabase_SelectedIndexChanged">
						<asp:ListItem Text="Use Existing DB Connection String" Selected="true" Value="existing"></asp:ListItem>
						<asp:ListItem Text="Create New DB Connection String" Value="create"></asp:ListItem>
					</asp:RadioButtonList>
					<br />
					<asp:PlaceHolder ID="ExistingConnectionHolder" runat="server" Visible="true">
						<h4 class="lined">
							Select Existing</h4>
						Select SQL Server Database Connection String:&nbsp;
						<asp:DropDownList ID="lbConnections" runat="server">
						</asp:DropDownList>
						<br />
						<br />
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="NewConnectionHolder" runat="server" Visible="false">
						<h4 class="lined">
							Create New Connection String</h4>
						Note: Connection String will be saved as "<asp:Label ID="lblConnStringAppSettingName"
							runat="server" Text="yafnet" />".
						<br />			
						<br />
						<asp:Label ID="Parameter1_Name" runat="server" Width="500px" />
						<br />											
						<asp:TextBox runat="server" ID="Parameter1_Value" Text="(local)" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter2_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter2_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter3_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter3_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter4_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter4_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter5_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter5_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter6_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter6_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter7_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter7_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter8_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter8_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter9_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter9_Value" Width="500px" /><br />
						<br />
						<asp:Label ID="Parameter10_Name" runat="server" Width="500px" />
						<asp:TextBox runat="server" ID="Parameter10_Value" Width="500px" /><br />
						<br />						
						<asp:CheckBox ID="Parameter11_Value" runat="server" Checked="true" Text="Use Integrated Security"
							AutoPostBack="true" OnCheckedChanged="Parameter11_Value_CheckChanged" />
						<asp:CheckBox ID="Parameter12_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />	
						<asp:CheckBox ID="Parameter13_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />	
						<asp:CheckBox ID="Parameter14_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />		
						<asp:CheckBox ID="Parameter15_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />	
						<asp:CheckBox ID="Parameter16_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />	
						<asp:CheckBox ID="Parameter17_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />
						<asp:CheckBox ID="Parameter18_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />	
						<asp:CheckBox ID="Parameter19_Value" runat="server" Checked="true" 
							AutoPostBack="true"  />						
						<br />
						<br />
						<asp:PlaceHolder ID="DBUsernamePasswordHolder" Visible="false" runat="server">User
							ID:<br />
							<asp:TextBox runat="server" ID="txtDBUserID" Width="500px" /><br />
							<br />
							Password:<br />
							<asp:TextBox runat="server" ID="txtDBPassword" Width="500px" /><br />
							<br />
						</asp:PlaceHolder>
					</asp:PlaceHolder>
					<h4 class="lined">
						Test Database Connection</h4>
					<asp:Button ID="btnTestDBConnection" runat="server" CssClass="wizButton" Text="Test Connection"
						OnClick="btnTestDBConnection_Click" OnClientClick="return true;" />
					<asp:PlaceHolder ID="ConnectionInfoHolder" runat="server" Visible="false">
						<h4 class="lined">
							Connection Details</h4>
						<div>
							<asp:Label ID="lblConnectionDetails" runat="server"></asp:Label>
						</div>
					</asp:PlaceHolder>
				</asp:WizardStep>
				<asp:WizardStep runat="server" Title="Manually Modify Database Connection" ID="WizManualDatabaseConnection">
					<asp:PlaceHolder ID="NoWriteAppSettingsHolder" runat="server" Visible="false">
						<h4 class="lined">
							No Write Access to Settings</h4>
						<p>
							Unable to modify your config file to set Database Connection due to no write permission.</p>
						<p>
							Please open the <b>
								<asp:Label runat="server" ID="lblAppSettingsFile">web.config</asp:Label></b> file
							in the application root directory and add/update the following key in the &lt;AppSettings&gt;
							section:
						</p>
						<blockquote>
							<b>&lt;add key="YAF.ConnectionStringName" value="<asp:Label runat="server" ID="lblConnectionStringName"></asp:Label>"
								/&gt;</b></blockquote>
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="NoWriteDBSettingsHolder" runat="server" Visible="false">
						<h4 class="lined">
							No Write Access to Database Connection Strings</h4>
						<p>
							Unable to modify your config file to set Database Connection string due to no write
							permission.</p>
						<p>
							Please open the <b>
								<asp:Label runat="server" ID="lblDBSettingsFile">db.config</asp:Label></b> file
							in the application root directory and add/update the following key in the &lt;connectionStrings&gt;
							section:
						</p>
						<blockquote>
							<b>&lt;add name="<asp:Label runat="server" ID="lblDBConnStringName" />" connectionString="<asp:Label
								runat="server" ID="lblDBConnStringValue" />" /&gt; </b>
						</blockquote>
					</asp:PlaceHolder>
					<p>
						<a href="http://wiki.yetanotherforum.net/TroubleShooting Write Permission.ashx">See this Topic in the YAF Wiki for more Information</a>
					</p>					
					<p>Click "Next" when you are ready.</p>
				</asp:WizardStep>				
				<asp:WizardStep runat="server" Title="Test Settings" ID="WizTestSettings">
					<p>You may want to test your YAF configuration settings here before proceeding.</p>
					<h4 class="lined">
						Database Connection Test</h4>
					<p>Click the Previous button to edit the database configuration.</p>
					<asp:Button ID="btnTestDBConnectionManual" runat="server" CssClass="wizButton" Text="Test Database Connection"
						OnClick="btnTestDBConnectionManual_Click" />
					<asp:PlaceHolder ID="ManualConnectionInfoHolder" runat="server" Visible="false">
						<h4 class="lined">
							Connection Details</asp:Literal></h4>
						<div>
							<asp:Label ID="lblConnectionDetailsManual" runat="server"></asp:Label>
						</div>
					</asp:PlaceHolder>
					<h4 class="lined">
						Mail (SMPT) Sending Test</h4>
					Send From Email Address:
					<asp:TextBox ID="txtTestFromEmail" runat="server"></asp:TextBox>
					<br />
					Send To Email Address:
					<asp:TextBox ID="txtTestToEmail" runat="server"></asp:TextBox>
					<br />
					<br />
					<asp:Button ID="btnTestSmtp" runat="server" Text="Test Smtp Settings" CssClass="wizButton"
						OnClick="btnTestSmtp_Click" />
					<asp:PlaceHolder ID="SmtpInfoHolder" runat="server" Visible="false">
						<h4 class="lined">
							SMTP Test Details</asp:Literal></h4>
						<div>
							<asp:Label ID="lblSmtpTestDetails" runat="server"></asp:Label>
						</div>
					</asp:PlaceHolder>
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
					<strong>Migrate Roles and Users</strong>
					<p>
						<asp:Label ID="lblMigrateUsersCount" runat="server" Text="0"></asp:Label>
						user(s) in your forum database.</p>
					<p>
						Click "Next" to start the upgrade (migration) of all roles and users from your old
						Yet Another Forum.NET database to the .NET Provider Model. The migration task can
						take a <b>very</b> long time depending on how many users are in your forum.</p>
					<asp:CheckBox ID="skipMigration" runat="server" Text="Disable Migration (not recommended)"
						Visible="False" />
				</asp:WizardStep>
				<asp:WizardStep runat="server" Title="Migrating Users..." ID="WizMigratingUsers">
					<h4>
						Migrating Users...</h4>
					Please do not navigate away from this page while the migration is in progress. It
					can take a very long time to perform the migration.
					<asp:UpdatePanel ID="LoadingCheckPanel" runat="server">
						<ContentTemplate>
							<asp:Timer ID="UpdateStatusTimer" runat="server" Interval="5000" OnTick="UpdateStatusTimer_Tick" />
							<div align="center">
								<asp:Image ID="LoadingImage" runat="server" ImageUrl="../resources/images/loading-white.gif" />
								<br />
								<b>Migrating Roles and Users...</b>
							</div>
						</ContentTemplate>
					</asp:UpdatePanel>
				</asp:WizardStep>
				<asp:WizardStep runat="server" StepType="Finish" Title="Finished" ID="WizFinished">
					<strong>Setup/Upgrade Finished</strong><br />
					<br />
					Your forum has now been setup or upgraded to the latest version.
				</asp:WizardStep>
			</WizardSteps>
			<SideBarButtonStyle BackColor="#507CD1" Font-Names="Verdana" ForeColor="White" />
			<FinishNavigationTemplate>
				<div class="wizNav">
					<asp:Button ID="FinishPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious"
						Text="Previous" CssClass="wizButton" />
					<asp:Button ID="FinishButton" runat="server" CssClass="wizButton" CommandName="MoveComplete"
						Text="Finish" />
				</div>
			</FinishNavigationTemplate>
			<HeaderStyle BackColor="White" BorderStyle="Solid" BorderWidth="1px" Font-Bold="True"
				BorderColor="#999999" Font-Size="0.9em" ForeColor="Black" HorizontalAlign="Center" />
			<HeaderTemplate>
				<img src="../images/YAFLogo.jpg" alt="YAF Logo" /><br />
				Installation Wizard
			</HeaderTemplate>
			<StartNavigationTemplate>
				<div class="wizNav">
					<asp:Button ID="StartNextButton" CssClass="wizButton" runat="server" CommandName="MoveNext"
						Text="Next" />
				</div>
			</StartNavigationTemplate>
			<StepNavigationTemplate>
				<div class="wizNav">
					<asp:Button ID="StepPreviousButton" runat="server" CssClass="wizButton" Visible="false"
						CausesValidation="False" CommandName="MovePrevious" Text="Previous" />
					<asp:Button ID="StepNextButton" runat="server" CssClass="wizButton" OnClientClick="return true;"
						CommandName="MoveNext" Text="Next" />
				</div>
			</StepNavigationTemplate>
		</asp:Wizard>
	</div>
	</form>
</body>
</html>
