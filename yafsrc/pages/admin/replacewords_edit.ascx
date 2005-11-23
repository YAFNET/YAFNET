<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>
<%@ Control language="c#" Inherits="yaf.pages.admin.replacewords_edit" CodeFile="replacewords_edit.ascx.cs" CodeFileBaseClass="yaf.AdminPage" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
		<TR>
			<TD class="header1" colSpan="2">Add/Edit Word Replace</TD>
		</TR>
		<TR>
			<TD class="postheader" width="50%"><B>Bad Word</B></TD>
			<TD class="post" width="50%">
				<asp:textbox id="badword" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader" width="50%"><B>Good Word</B></TD>
			<TD class="post" width="50%">
				<asp:textbox id="goodword" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postfooter" align="center" colSpan="2">
				<asp:button id="save" runat="server" text="Save"></asp:button>
				<asp:button id="cancel" runat="server" text="Cancel"></asp:button></TD>
		</TR>
	</TABLE>
</yaf:adminmenu>
<yaf:SmartScroller id="SmartScroller1" runat = "server" />
