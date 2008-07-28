<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reindex.ascx.cs" Inherits="YAF.Pages.Admin.reindex" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
    <table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
            <td class="header1">
                YAF DB Index Statistics</td>
        </tr>
        <tr class="post">
            <td>
                <asp:TextBox ID="txtIndexStatistics" runat="server" TextMode="MultiLine" Width="100%"
                    Height="400px"></asp:TextBox>
            </td>
        </tr>
        <tr class="footer1">
            <td>
                <asp:Button ID="btnGetStats" runat="server" Text="View YAF Table Index Statistics"
                    OnClick="btnGetStats_Click" />
                <asp:Button ID="btnReindex" runat="server" Text="Reindex YAF Tables" OnClientClick="return confirm('Are you sure you want to reindex all YAF tables? The operation may make the DB inaccessible and may take a little while.');"
                    OnClick="btnReindex_Click" />                    
                    </td>
        </tr>
    </table>
</YAF:AdminMenu>
