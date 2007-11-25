<%@ Control Language="c#" CodeFile="rules.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.rules" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td class="header1" align="center">
            Forum Rules and Policies</td>
    </tr>
    <tr>
        <td>
            <asp:Label ID="ForumRules" runat="server" /></td>
    </tr>
    <tr>
        <td align="center">
            <asp:Button ID="Accept" runat="server" Text="Accept" OnClick="Accept_Click" />
            <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" />
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
