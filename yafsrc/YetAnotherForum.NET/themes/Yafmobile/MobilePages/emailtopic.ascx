<%@ Control Language="c#" CodeFile="emailtopic.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.emailtopic" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="to" />
            <br />
            <asp:TextBox ID="EmailAddress" runat="server" Width="95%" ></asp:TextBox></td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="subject" />
            <br />
            <asp:TextBox ID="Subject" runat="server" Width="95%"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="postheader" valign="top">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="message" />
            <br />
            <asp:TextBox ID="Message" runat="server" TextMode="MultiLine" Height="300px" Width="95%"></asp:TextBox></td>
    </tr>
    <tr>
        <td class="footer1" align="center">
            <asp:Button ID="SendEmail" runat="server" OnClick="SendEmail_Click" /></td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
