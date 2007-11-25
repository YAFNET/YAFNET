<%@ Control Language="c#" CodeFile="info.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.info" %>
<table width="100%" height="50%" border="0">
    <tr>
        <td valign="middle" align="center">
            <table class="content" align="center" width="35%" cellspacing="0" cellpadding="0">
                <tr>
                    <td class="post" valign='top'>
                        <table width="100%" cellspacing="0" cellpadding="0">
                            <tr>
                                <td nowrap class="header2">
                                    <asp:Label runat="server" ID="Title" Text="Information" /></td>
                            </tr>
                            <tr>
                                <td class="post">
                                    <asp:Label runat="server" ID="Info" /></td>
                            </tr>
                            <tr>
                                <td class="post">
                                    <asp:HyperLink runat="server" ID="Continue" /></td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller id="SmartScroller1" runat="server" />
</div>
