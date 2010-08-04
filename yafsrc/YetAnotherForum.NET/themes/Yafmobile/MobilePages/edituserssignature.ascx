<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edituserssignature.ascx.cs"
    Inherits="YAF.Controls.EditUsersSignature" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr runat="server" id="trHeader">
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel runat="server" LocalizedPage="CP_SIGNATURE" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td class="post" id="EditorLine" runat="server">
        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="signature" />
            <!-- editor goes here -->
        </td>
    </tr>
    <tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="save" CssClass="pbutton" runat="server" />
            <asp:Button ID="cancel" CssClass="pbutton" runat="server" />
        </td>
    </tr>
</table>
