<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="replacewords_edit.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.replacewords_edit" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
		<TR>
			<TD class="header1" colSpan="2">Edit Word Replace</TD>
		</TR>
		<TR>
			<TD class="postheader" width="50%"><B>Badword</B></TD>
			<TD class="post" width="50%">
				<asp:textbox id="badword" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader" width="50%"><B>Goodword</B></TD>
			<TD class="post" width="50%">
				<asp:textbox id="goodword" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="footer1" align="center" colSpan="2">
				<asp:button id="save" runat="server" text="Save"></asp:button>
				<asp:button id="cancel" runat="server" text="Cancel"></asp:button></TD>
		</TR>
	</TABLE>
</yaf:adminmenu>
<yaf:savescrollpos runat="server" id="Savescrollpos1" />
