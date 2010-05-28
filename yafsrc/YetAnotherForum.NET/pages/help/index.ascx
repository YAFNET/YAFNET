<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.help.index" Codebehind="index.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:HelpMenu runat="server">
	<table class="content" width="100%" cellspacing="0" cellpadding="0">
		<tr>
			<td class="post" valign="top">
				<table width="100%" cellspacing="0" cellpadding="0">
					<tr>
						<td nowrap class="header2">
							<b>Search Help Topics</b>
						</td>
					</tr>
					<tr>
						<td nowrap class="post">
							Enter keywords to search for:
							<asp:TextBox runat="server" ID="search" />
							<asp:Button runat="server" ID="DoSearch" Text="Search" />
						</td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
</YAF:HelpMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
