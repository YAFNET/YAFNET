<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PrintTopic" Codebehind="PrintTopic.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="Posts" runat="server">
    <ItemTemplate>
        <table class="table table-bordered">
            <thead>
                <tr>
                    <th scope="col">
                        <%#
    this.GetPrintHeader(Container.DataItem) %>
                    </th>
                </tr>
            </thead>
            <tbody>
            <tr>
                <td>
                    <%#
    this.GetPrintBody(Container.DataItem) %>
                </td>
            </tr>
            </tbody>
        </table>
    </ItemTemplate>
    <SeparatorTemplate>
        <br />
    </SeparatorTemplate>
</asp:Repeater>
