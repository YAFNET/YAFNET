<%@ Page Language="c#" CodeFile="default.aspx.cs" AutoEventWireup="True" Inherits="YAF.Install._default" %>

<html>
<head>
	<title>Yet Another Forum.NET Installation</title>
	<link type="text/css" rel="stylesheet" href="../resources/forum.css" />
	<link type="text/css" rel="stylesheet" href="../themes/FlatEarth/theme.css" />
</head>
<body>
	<form runat="server">
		<div align="center">
			<asp:Wizard ID="InstallWizard" runat="server" ActiveStepIndex="0" BackColor="#EFF3FB"
				BorderColor="#B5C7DE" BorderWidth="1px" Font-Names="Verdana" Font-Size="Small"
				Width="480px" CellPadding="8" DisplaySideBar="False" OnActiveStepChanged="Wizard_ActiveStepChanged"
				OnFinishButtonClick="Wizard_FinishButtonClick" OnPreviousButtonClick="Wizard_PreviousButtonClick"
				OnNextButtonClick="Wizard_NextButtonClick">
				<StepStyle Font-Size="0.8em" ForeColor="#333333" />
				<SideBarStyle BackColor="#507CD1" Font-Size="0.9em" VerticalAlign="Top" />
				<NavigationButtonStyle BackColor="White" BorderColor="#507CD1" BorderStyle="Solid"
					BorderWidth="1px" Font-Names="Verdana" Font-Size="0.8em" ForeColor="#284E98" />
				<WizardSteps>
					<asp:WizardStep runat="server" StepType="Start" Title="Create Config Password">
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
					<asp:WizardStep runat="server" StepType="Start" Title="Enter Config Password">
						<strong>Enter Config Password</strong><br />
						<br />
						You need to enter the configuration password to upgrade the forum. This is the configuration
						password you created when the forum was first installed, not the admin user password.<br />
						<br />
						Password:<br />
						<asp:TextBox ID="TextBox3" runat="server" TextMode="Password" Width="100%"></asp:TextBox>
					</asp:WizardStep>
					<asp:WizardStep runat="server" StepType="Start" Title="Upgrade Database">
						<strong>Upgrade Database</strong><br />
						<br />
						Clicking next will upgrade your database to the latest version.</asp:WizardStep>
					<asp:WizardStep runat="server" StepType="Start" Title="Create Forum">
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
							<asp:TextBox ID="SmptServerAddress" runat="server" />
							The name of a smtp server used to send emails.
						</p>
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
					</asp:WizardStep>
					<asp:WizardStep runat="server" StepType="Start" Title="Migrate Users">
						<strong>Migrate Roles and Users</strong><br />
						<br />
						Clicking next will migrate all roles and users from Yet Another Forum.net to ASP.NET.
						This means that all of the users in your forum database will be converted to users
						in the ASP.NET DB with new passwords mailed to their e-mail address.<br />
						<br />
						If you don't want to migrate any existing users to the ASP.NET DB you can skip this
						step.<br />
						&nbsp;<br />
						<asp:CheckBox ID="skipMigration" runat="server" Text=" Skip Migration" />
					</asp:WizardStep>
					<asp:WizardStep runat="server" StepType="Finish" Title="Finished">
						<strong>Upgrade Finished</strong><br />
						<br />
						Your forum has now been upgraded to the latest version.
					</asp:WizardStep>
					<asp:WizardStep runat="server" StepType="Start" Title="Database Connection">
						<strong>Database Connection</strong><br />
						<br />
						TODO... Enter/Test DB Connection...
					</asp:WizardStep>					
				</WizardSteps>
				<SideBarButtonStyle BackColor="#507CD1" Font-Names="Verdana" ForeColor="White" />
				<HeaderStyle BackColor="#284E98" BorderColor="#EFF3FB" BorderStyle="Solid" BorderWidth="2px"
					Font-Bold="True" Font-Size="0.9em" ForeColor="White" HorizontalAlign="Center" />
			</asp:Wizard>
		</div>
		<YAF:SmartScroller runat="server" ID="scroller" />
	</form>
</body>
</html>
