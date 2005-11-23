<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="boardsettings.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.boardsettings" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
		<TR>
			<TD class="header1" colSpan="2">Forum Settings</TD>
		</TR>
		<TR>
			<TD class="header2" colSpan="2">Forum Setup</TD>
		</TR>
		<TR>
			<TD class="postheader" width="50%"><B>Forum Name:</B><BR>
				The name of the forum.</TD>
			<TD class="post" width="50%">
				<asp:textbox id="Name" runat="server" Width="300"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Allow Threaded:</B><BR>
				Allow threaded view for posts.</TD>
			<TD class="post">
				<asp:checkbox id="AllowThreaded" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Theme:</B><BR>
				The theme to use on this board.</TD>
			<TD class="post">
				<asp:dropdownlist id="Theme" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Language:</B><BR>
				The default board language.</TD>
			<TD class="post">
				<asp:dropdownlist id="Language" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Show Topic Default:</B><BR>
				The default board show topic interval selection.</TD>
			<TD class="post">
				<asp:dropdownlist id="ShowTopic" runat="server"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="postfooter" align="center" colSpan="2">
				<asp:Button id="Save" runat="server" Text="Save"></asp:Button></TD>
		</TR>
	</TABLE>
</yaf:adminmenu>
<yaf:SmartScroller id="SmartScroller1" runat="server" />
