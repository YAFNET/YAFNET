<%@ Control Language="c#" CodeFile="eventlog.ascx.cs" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.eventlog" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <asp:UpdatePanel ID="eventUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <table class="content" width="100%" cellspacing="1" cellpadding="0">
                <tr>
                    <td class="header1" colspan="8">
                        Event Log</td>
                </tr>
                <asp:Repeater runat="server" ID="List">
                    <HeaderTemplate>
                        <tr class="header2">
                            <td width="1%">
                                Type</td>
                            <td>
                                User</td>
                            <td>
                                Time</td>
                            <td>
                                Source</td>
                            <td>
                                &nbsp;</td>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr class="postheader">
                            <td align="center">
                                <%# EventImageCode(Container.DataItem) %>
                            </td>
                            <td>
                                <%# Eval( "Name") %>
                            </td>
                            <td>
                                <%# Eval( "EventTime") %>
                            </td>
                            <td>
                                <%# Eval( "Source") %>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="showbutton" CommandName="show">Show</asp:LinkButton>
                                |
                                <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "EventLogID") %>'>Delete</asp:LinkButton>
                            </td>
                        </tr>
                        <tr class="post" runat="server" visible="false" id="details">
                            <td colspan="5">
                                <pre style="overflow: scroll"><%# Eval( "Description") %></pre>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr class="footer1">
                            <td colspan="5">
                                <asp:Button runat="server" OnLoad="DeleteAll_Load" OnClick="DeleteAll_Click" Text="Delete All" /></td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </table>
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
        </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
