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
				<b>Board Name:</b><br />
				The name of the board.</td>
			<td class="post" width="50%">
				<asp:TextBox ID="Name" runat="server" Width="100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Allow Threaded:</b><br />
				Allow threaded view for posts.</td>
			<td class="post">
				<asp:CheckBox ID="AllowThreaded" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Theme:</b><br />
				The theme to use on this board.</td>
			<td class="post">
				<asp:DropDownList ID="Theme" runat="server">
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Mobile Theme:</b><br />
				The mobile theme to use on this board.</td>
			<td class="post">
				<asp:DropDownList ID="MobileTheme" runat="server">
				</asp:DropDownList></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Allow Themed Logo :</b><br />
				Gets logo from theme file (Does not work in portal).</td>
			<td class="post">
				<asp:CheckBox ID="AllowThemedLogo" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Language:</b><br />
				The default board language.</td>
			<td class="post">
				<asp:DropDownList ID="Language" runat="server">
				</asp:DropDownList></td>
		</tr>		
		<tr>
			<td class="postheader">
				<b>Show Topic Default:</b><br />
				The default board show topic interval selection.</td>
			<td class="post">
				<asp:DropDownList ID="ShowTopic" runat="server">
				</asp:DropDownList></td>
		</tr>		
		<tr>
			<td class="postheader">
				<b>File Extensions List is:</b><br />
				Is the list of file extensions allowed files or disallowed files (less secure)?</td>
			<td class="post">
				<asp:DropDownList ID="FileExtensionAllow" runat="server">
				</asp:DropDownList></td>
		</tr>		
		<tr>
			<td class="postheader">
				<b>Send Email Notification On User Register to Emails:</b><br />
				Semi-colon (;) separated list of emails to send a notification to on user registration.</td>
			<td class="post">
				<asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" Width="100%"></asp:TextBox></td>
		</tr>			
		
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
