<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.eventlog" Codebehind="eventlog.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="AdminMenu1">
    <asp:UpdatePanel ID="eventUpdate" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <table class="content" width="100%" cellspacing="1" cellpadding="0">
                <tr>
                    <td class="header1" colspan="8">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOG" />
                    </td>
                </tr>

                <asp:Repeater runat="server" ID="List">
                    <HeaderTemplate>
                        <tr class="header2">
                            <td width="1%">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TYPE" LocalizedPage="ADMIN_EVENTLOG" />
                            </td>
                            <td>
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USER" LocalizedPage="ADMIN_EVENTLOG" />
                            </td>
                            <td>
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TIME" LocalizedPage="ADMIN_EVENTLOG" />
                            </td>
                            <td>
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SOURCE" LocalizedPage="ADMIN_EVENTLOG" />
                            </td>
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
                                <%# HtmlEncode(Eval( "Name")) %>
                            </td>
                            <td>
                                <%# Eval( "EventTime") %>
                            </td>
                            <td>
                                <%# HtmlEncode(Eval( "Source")) %>
                            </td>
                            <td>
                                <asp:LinkButton runat="server" ID="showbutton" CommandName="show">
                                  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SHOW" LocalizedPage="ADMIN_EVENTLOG" />
                                </asp:LinkButton>
                                |
                                <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "EventLogID") %>'>
                                  <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="DELETE" />
                                </asp:LinkButton>
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
                            <td colspan="5" align="center">
                                <asp:Button runat="server" OnLoad="DeleteAll_Load" CssClass="pbutton" OnClick="DeleteAll_Click"></asp:Button>
                             </td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </table>
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
        </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
