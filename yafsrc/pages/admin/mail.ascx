<%@ Control language="c#" Codebehind="mail.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.mail" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width=100%>
  <tr>
    <td class=header1 colspan="2">Compose Email</td></tr>
  <tr>
    <td class=postheader>To:</td>
    <td class=post><asp:dropdownlist id=ToList runat="server" DataValueField="GroupID" DataTextField="Name"></asp:dropdownlist></td></tr>
  <tr>
    <td class=postheader>Subject:</td>
    <td class=post><asp:textbox id=Subject runat="server" CssClass="edit"></asp:textbox></td></tr>
  <tr>
    <td class=postheader vAlign=top>Message:</td>
    <td class=post><asp:textbox id=Body runat="server" TextMode="MultiLine" CssClass="edit" Rows="16"></asp:textbox></td></tr>
  <tr>
    <td class=postfooter align=middle colspan="2"><asp:button id=Send runat="server" Text="Send" onclick="Send_Click"></asp:button></td></tr></table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
