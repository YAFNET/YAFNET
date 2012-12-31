<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.eventlog"
    CodeBehind="eventlog.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<script type="text/javascript">
    jQuery().ready(function () {
        $('div[id^=description]*').each(function (index) {
            $(this).width($('#headerSize').innerWidth()-20); //$(this).parent().width());
        });
    });
</script>

<YAF:AdminMenu runat="server" ID="AdminMenu1">
    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
    <table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
            <td class="header1" colspan="8">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOG" />
            </td>
        </tr>
        <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <tr class="header2" id="headerSize">
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
                        &nbsp;
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="postheader">
                    <td align="center">
                        <a name="event<%# Eval("EventLogID")%>" />
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
                        <a href="ShowHideEventLogDetails.html" id="Show<%# Eval("EventLogID") %>" onclick="javascript:jQuery('#eventDetails<%# Eval("EventLogID") %>').toggle(); return false;">
                             <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SHOW" LocalizedPage="ADMIN_EVENTLOG" /></a>
                                |
                                <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "EventLogID") %>'>
                                  <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="DELETE" />
                                </asp:LinkButton>
                    </td>
                </tr>
                <tr class="post" id="eventDetails<%# Eval("EventLogID") %>" style="display: none;">
                    <td colspan="5">
                        <div style="overflow: scroll;" id="description<%# Eval("EventLogID") %>">
                            <pre><%# Eval( "Description") %></pre>
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr class="footer1">
                    <td colspan="5" align="center">
                        <asp:Button runat="server" OnLoad="DeleteAll_Load" CssClass="pbutton" OnClick="DeleteAll_Click">
                        </asp:Button>
                    </td>
                </tr>
            </FooterTemplate>
        </asp:Repeater>
    </table>
    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
