<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Control language="c#" Codebehind="boardsettings.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boardsettings" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellSpacing="1" cellPadding="0" width="100%">
		<tr>
			<td class="header1" colSpan="2">Forum Settings</td>
		</tr>
		<tr>
			<td class="header2" colSpan="2">Forum Setup</td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><b>Forum Name:</b><br />
				The name of the forum.</td>
			<td class="post" width="50%">
				<asp:textbox id="Name" runat="server" Width="300"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader"><b>Allow Threaded:</b><br />
				Allow threaded view for posts.</td>
			<td class="post">
				<asp:checkbox id="AllowThreaded" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><b>Theme:</b><br />
				The theme to use on this board.</td>
			<td class="post">
				<asp:dropdownlist id="Theme" runat="server"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader"><b>Allow Themed Logo :</b><br />
				Gets logo from theme file (Does not work in portal).</td>
			<td class="post">
				<asp:checkbox id="AllowThemedLogo" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><b>Language:</b><br />
				The default board language.</td>
			<td class="post">
				<asp:dropdownlist id="Language" runat="server"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader"><b>Show Topic Default:</b><br />
				The default board show topic interval selection.</td>
			<td class="post">
				<asp:dropdownlist id="ShowTopic" runat="server"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colSpan="2">
				<asp:Button id="Save" runat="server" Text="Save" onclick="Save_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat="server" />
