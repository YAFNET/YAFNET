<%@ Control language="c#" Inherits="yaf.pages.admin.mail" CodeFile="mail.ascx.cs" CodeFileBaseClass="yaf.AdminPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width=100%>
  <tr>
    <td class=header1 colSpan=2>Compose Email</TD></TR>
  <tr>
    <td class=postheader>To:</TD>
    <td class=post><asp:dropdownlist id=ToList runat="server" DataValueField="GroupID" DataTextField="Name"></asp:dropdownlist></TD></TR>
  <tr>
    <td class=postheader>Subject:</TD>
    <td class=post><asp:textbox id=Subject runat="server" CssClass="edit"></asp:textbox></TD></TR>
  <tr>
    <td class=postheader vAlign=top>Message:</TD>
    <td class=post><asp:textbox id=Body runat="server" TextMode="MultiLine" CssClass="edit" Rows="16"></asp:textbox></TD></TR>
  <tr>
    <td class=postfooter align=middle colSpan=2><asp:button id=Send runat="server" Text="Send" onclick="Send_Click"></asp:button></TD></TR></TABLE>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
