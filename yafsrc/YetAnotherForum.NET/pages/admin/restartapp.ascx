<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.restartapp" Codebehind="restartapp.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="adminmenu1" runat="server">
	<table width="100%" cellspacing="0" cellpadding="0" class="content">
		<tr>
			<td class="header1">
				Restart Application
			</td>
		</tr>
		<tr class="post">
			<td>
				<p>
					Restarting the application will reload all .config files.
				</p>
			</td>
		</tr>
		<tr>
			<td colspan="2" class="postfooter" align="center">
				<asp:Button ID="RestartApp" runat="server" Text="Restart Application" CssClass="pbutton"
					OnClick="RestartApp_Click"></asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
