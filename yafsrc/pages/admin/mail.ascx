<%@ Control language="c#" Codebehind="mail.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.mail" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

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
    <td class=postfooter align=middle colSpan=2><asp:button id=Send runat="server" Text="Send"></asp:button></TD></TR></TABLE>

</yaf:adminmenu>

<yaf:savescrollpos runat="server"/>
