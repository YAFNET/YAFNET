<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.printtopic" Codebehind="printtopic.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="Posts" runat="server">
    <ItemTemplate>
        <table class="print" width="100%" cellspacing="0" cellpadding="0">
            <tr>
                <td class="printheader">
                    <%# GetPrintHeader(Container.DataItem) %>
                </td>
            </tr>
            <tr>
                <td class="printbody">
                    <%# GetPrintBody(Container.DataItem) %>
                </td>
            </tr>
        </table>
    </ItemTemplate>
    <SeparatorTemplate>
        <br />
    </SeparatorTemplate>
</asp:Repeater>
