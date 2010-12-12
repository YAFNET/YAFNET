<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.boardsettings" Codebehind="boardsettings.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				Current Board Settings</td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
				Board Setup</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<strong>Board Name:</strong><br />
				The name of the board.</td>
			<td class="post" width="50%">
				<asp:TextBox ID="Name" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Allow Threaded:</strong><br />
				Allow threaded view for posts.</td>
			<td class="post">
				<asp:CheckBox ID="AllowThreaded" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Theme:</strong><br />
				The theme to use on this board.</td>
			<td class="post">
				<asp:DropDownList ID="Theme" runat="server">
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Mobile Theme:</strong><br />
				The mobile theme to use on this board.</td>
			<td class="post">
				<asp:DropDownList ID="MobileTheme" runat="server">
                    <asp:ListItem Text="[None Selected]" Value=""></asp:ListItem>
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Allow Themed Logo :</strong><br />
				Gets logo from theme file (Does not work in portal).</td>
			<td class="post">
				<asp:CheckBox ID="AllowThemedLogo" runat="server"></asp:CheckBox></td>
		</tr>
        <tr>
			<td class="postheader">
				<strong>jQuery UI Theme:</strong><br />
				The jQuery UI Theme to use on this board for the Tabs and Accordion.</td>
			<td class="post">
				<asp:DropDownList ID="JqueryUITheme" runat="server">
				</asp:DropDownList></td>
		</tr>
        <tr>
			<td class="postheader">
				<strong>Use Google Hosted CDN jQuery UI CSS File?</strong><br />
				You can use the Google Hosted CSS Files, or instead use internal.</td>
			<td class="post">
				<asp:CheckBox ID="JqueryUIThemeCDNHosted" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Culture:</strong><br />
				The default culture &amp; language for this forum.</td>
			<td class="post">
				<asp:DropDownList ID="Culture" runat="server">
				</asp:DropDownList></td>
		</tr>        	
		<tr>
			<td class="postheader">
				<strong>Show Topic Default:</strong><br />
				The default board show topic interval selection.</td>
			<td class="post">
				<asp:DropDownList ID="ShowTopic" runat="server">
				</asp:DropDownList></td>
		</tr>		
		<tr>
			<td class="postheader">
				<strong>File Extensions List is:</strong><br />
				Is the list of file extensions allowed files or disallowed files (less secure)?</td>
			<td class="post">
				<asp:DropDownList ID="FileExtensionAllow" runat="server">
				</asp:DropDownList></td>
		</tr>	
       <tr id="PollGroupList" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel ID="PollGroupListLabel" runat="server" LocalizedTag="pollgroup_list" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:DropDownList ID="PollGroupListDropDown" runat="server" CssClass="edit" MaxLength="10" Width="400" />			
		</td>
	</tr> 
		<tr>
			<td class="postheader">
				<strong>Send Email Notification On User Register to Emails:</strong><br />
				Semi-colon (;) separated list of emails to send a notification to on user registration.</td>
			<td class="post">
				<asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" Width="100%"></asp:TextBox></td>
		</tr>			
		<tr>
			<td class="postheader">
				<strong>Email Moderators On New Moderated Post:</strong><br />
				Should all the moderators of a forum be notified if a new post that needs approval is created?</td>
			<td class="post">
				<asp:CheckBox ID="EmailModeratorsOnModeratedPost" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Allow Digest Email Sending for Users Once Daily:</strong><br />
				Reqired: board must have "YAF.BaseUrlMask" and "YAF.ForceScriptName" AppSettings defined in your web.config for digest to work.</td>
			<td class="post">
				<asp:CheckBox ID="AllowDigestEmail" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Default Send Digest "On" for New Users?</strong><br />
				When a new user account is created, default send digest to true?</td>
			<td class="post">
				<asp:CheckBox ID="DefaultSendDigestEmail" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Default Notification Setting:</strong><br />
				When a new user account is created, what notification setting does it default to?</td>
			<td class="post">
				<asp:DropDownList ID="DefaultNotificationSetting" runat="server">
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
