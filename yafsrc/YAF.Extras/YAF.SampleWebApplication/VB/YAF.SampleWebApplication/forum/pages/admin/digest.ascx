<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.digest"
    CodeBehind="digest.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_DIGEST" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DIGEST_ENABLED"
                    LocalizedPage="ADMIN_DIGEST" />
            </td>
            <td class="post" width="75%">
                <b>
                    <asp:Label ID="DigestEnabled" runat="server"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DIGEST_LAST" LocalizedPage="ADMIN_DIGEST" />
            </td>
            <td class="post" width="75%">
                <b>
                    <asp:Label ID="LastDigestSendLabel" runat="server"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="Button2" runat="server" OnClick="ForceSend_Click" CssClass="pbutton">
                </asp:Button>
            </td>
        </tr>
    </table>
    <br />
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_DIGEST" />
            </td>
        </tr>
        <asp:PlaceHolder ID="DigestHtmlPlaceHolder" runat="server" Visible="false">
            <tr>
                <td class="postheader" width="25%">
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DIGEST_GENERATE"
                        LocalizedPage="ADMIN_DIGEST" />
                </td>
                <td class="post" width="75%">
                    <iframe id="DigestFrame" runat="server" style="width: 100%; height: 500px"></iframe>
                </td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="GenerateDigest" runat="server" OnClick="GenerateDigest_Click" CssClass="pbutton">
                </asp:Button>
            </td>
        </tr>
    </table>
    <br />
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_DIGEST" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <YAF:HelpLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DIGEST_EMAIL" LocalizedPage="ADMIN_DIGEST" />
            </td>
            <td class="post" width="75%">
                <asp:TextBox ID="TextSendEmail" runat="server" Style="width: 250px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <YAF:HelpLabel ID="LocalizedLabel8" runat="server" LocalizedTag="DIGEST_METHOD" LocalizedPage="ADMIN_DIGEST" />
            </td>
            <td class="post" width="75%">
                <asp:DropDownList ID="SendMethod" runat="server" Style="width: 250px">
                    <asp:ListItem Text="Direct" />
                    <asp:ListItem Text="Queued" Selected="True" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="TestSend" runat="server" OnClick="TestSend_Click" CssClass="pbutton">
                </asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
