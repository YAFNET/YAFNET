<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.digest"
    CodeBehind="digest.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                Digest
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <b>Digest Enabled:</b><br />
                Change in your Board Settings.
            </td>
            <td class="post" width="75%">
                <b>
                    <asp:Label ID="DigestEnabled" runat="server"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <b>Last Digest Send:</b><br />
                The last time (based on server time) the digest task was run.
            </td>
            <td class="post" width="75%">
                <b>
                    <asp:Label ID="LastDigestSendLabel" runat="server"></asp:Label></b>
            </td>
        </tr>
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="Button2" runat="server" Text="Force Digest Send" OnClientClick="return confirm('Are you sure you want to schedule a digest send right now?');"
                    OnClick="ForceSend_Click"></asp:Button>
            </td>
        </tr>
    </table>
    <br />
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                View Current Digest
            </td>
        </tr>
        <asp:PlaceHolder ID="DigestHtmlPlaceHolder" runat="server" Visible="false">
        <tr>
            <td class="postheader" width="25%">
                <b>Digest:</b><br />
                Generate Digest for this admin account and currently selected theme. (May not actually look like this rendered in an email client.)
            </td>
            <td class="post" width="75%">
                <iframe id="DigestFrame" runat="server" style="width:100%;height:500px"></iframe>
            </td>
        </tr>     
        </asp:PlaceHolder>   
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="GenerateDigest" runat="server" Text="Generate Digest" OnClick="GenerateDigest_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
    <br />
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                Send Test Digest
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <b>Email:</b><br />
                Send a test digest to the email.
            </td>
            <td class="post" width="75%">
                <asp:TextBox ID="TextSendEmail" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <b>Send Method:</b><br />
                Type of System Send to Test.
            </td>
            <td class="post" width="75%">
                <asp:DropDownList ID="SendMethod" runat="server">
                    <asp:ListItem Text="Direct" />
                    <asp:ListItem Text="Queued" Selected="True" />
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postfooter" colspan="2" align="center">
                <asp:Button ID="TestSend" runat="server" Text="Send Test" OnClick="TestSend_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
