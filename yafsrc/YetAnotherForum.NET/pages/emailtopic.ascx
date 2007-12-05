<%@ Control Language="c#" CodeFile="emailtopic.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.emailtopic" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <%= GetText("title") %>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <%= GetText("to") %>
        </td>
        <td class="post">
            <asp:TextBox ID="EmailAddress" runat="server" CssClass="edit"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="postheader">
            <%= GetText("subject") %>
        </td>
        <td class="post">
            <asp:TextBox ID="Subject" runat="server" CssClass="edit"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="postheader" valign="top">
            <%= GetText("message") %>
        </td>
        <td class="post" valign="top">
            <asp:TextBox ID="Message" runat="server" CssClass="edit" TextMode="MultiLine" Rows="12"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="SendEmail" runat="server" OnClick="SendEmail_Click" /></td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
