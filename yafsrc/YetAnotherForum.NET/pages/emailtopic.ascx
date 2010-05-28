<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.emailtopic" Codebehind="emailtopic.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="to" />
        </td>
        <td class="post">
            <asp:TextBox ID="EmailAddress" runat="server" CssClass="edit" Style="Width:300px"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="subject" />
        </td>
        <td class="post">
            <asp:TextBox ID="Subject" runat="server" CssClass="edit" Style="Width:300px"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="postheader" valign="top">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="message" />
        </td>
        <td class="post" valign="top">
            <asp:TextBox ID="Message" runat="server" CssClass="edit" TextMode="MultiLine" Rows="12" Style="Width:100%"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="SendEmail" runat="server" OnClick="SendEmail_Click" /></td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
