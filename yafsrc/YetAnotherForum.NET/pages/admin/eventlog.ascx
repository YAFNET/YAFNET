<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.eventlog"
    CodeBehind="eventlog.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
   
<YAF:PageLinks runat="server" ID="PageLinks" />

<script type="text/javascript">
function toggleItem(detailId)
{
	var show = '<%# this.GetText("ADMIN_EVENTLOG", "SHOW")%>';
	var hide = '<%# this.GetText("ADMIN_EVENTLOG", "HIDE")%>';
	
	$('#Show'+ detailId).text($('#Show'+ detailId).text() == show ? hide : show);
	
	jQuery('#eventDetails' + detailId).slideToggle('slow'); 

	return false;
	
}
</script>

<YAF:AdminMenu runat="server" ID="AdminMenu1">
    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
    <table class="content" width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td class="header1" colspan="3">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOG" />
            </td>
        </tr>
        <tr class="header2">
            <td>
                <YAF:HelpLabel ID="SinceDateLabel" runat="server" LocalizedPage="ADMIN_EVENTLOG" LocalizedTag="SINCEDATE" Suffix=":" />&nbsp;
                <asp:TextBox ID="SinceDate" runat="server" CssClass="edit"></asp:TextBox>
            </td>
            <td>
                <YAF:HelpLabel ID="ToDateLabel" runat="server" LocalizedPage="ADMIN_EVENTLOG" Suffix=":" LocalizedTag="TODATE" />&nbsp;
                <asp:TextBox ID="ToDate" runat="server" CssClass="edit"></asp:TextBox>
            </td>
            <td>
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedPage="ADMIN_EVENTLOG" Suffix=":" LocalizedTag="TYPES" />&nbsp;
                <asp:DropDownList ID="Types" runat="server" CssClass="edit"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="3" class="footer1" style="text-align:center">
                <YAF:ThemeButton ID="ApplyButton" CssClass="yaflittlebutton" OnClick="ApplyButton_Click" TextLocalizedPage="ADMIN_EVENTLOG" TextLocalizedTag="APPLY" runat="server"></YAF:ThemeButton>
            </td>
        </tr>
    </table>
    <br/>
    <table class="content" width="100%" cellspacing="0" cellpadding="0">
        <tr>
            <td class="header1" colspan="8">
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOG" />
            </td>
        </tr>
        <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <tr class="header2" id="headerSize">
                    <td width="1%">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TYPE" LocalizedPage="ADMIN_EVENTLOG" />
                    </td>
                    <td width="5%">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USER" LocalizedPage="ADMIN_EVENTLOG" />
                    </td>
                    <td width="8%">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TIME" LocalizedPage="ADMIN_EVENTLOG" />
                    </td>
                    <td>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SOURCE" LocalizedPage="ADMIN_EVENTLOG" />
                    </td>
                    <td>&nbsp;
                        
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td colspan="5">
                      <div class="<%# EventCssClass(Container.DataItem) %> ui-corner-all eventItem" onclick="javascript:toggleItem(<%# Eval("EventLogID") %>);">
                        <table style="padding:0;margin:0;">
                          <tr>
                            <td width="1%">
                              <a name="event<%# Eval("EventLogID")%>" ></a>
                              <%# EventImageCode(Container.DataItem) %>
                              <asp:HiddenField ID="EventTypeID" Value='<%# Eval("Type")%>' runat="server"/>
                            </td>
                            <td width="5%">
                              <%# HtmlEncode(Eval( "Name")) %>
                            </td>
                            <td width="8%">
                              <%# Eval( "EventTime") %>
                            </td>
                            <td>
                              <%# HtmlEncode(Eval( "Source")) %>
                            </td>
                            <td class="rightItem">
                              <a class="showEventItem" href="#event<%# Eval("EventLogID")%>" id="Show<%# Eval("EventLogID") %>"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SHOW" LocalizedPage="ADMIN_EVENTLOG" /></a>&nbsp;|&nbsp;<asp:LinkButton runat="server" OnLoad="Delete_Load" CssClass="deleteEventItem" CommandName="delete" CommandArgument='<%# Eval( "EventLogID") %>'>
                                  <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="DELETE" />
                                </asp:LinkButton>
                            </td>
                          </tr>
                        </table>     
                      </div>
                      <div class="EventDetails" id="eventDetails<%# Eval("EventLogID") %>" style="display: none;margin:0;padding:0;">  
                            <pre><%# Eval( "Description") %></pre>
                        </div>   
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                <tr class="footer1">
                    <td colspan="5" align="center">
                        <YAF:ThemeButton runat="server" Visible="<%# this.List.Items.Count > 0 %>" OnLoad="DeleteAll_Load" CssClass="yaflittlebutton" OnClick="DeleteAll_Click" TextLocalizedPage="ADMIN_EVENTLOG" TextLocalizedTag="DELETE_ALLOWED">
                        </YAF:ThemeButton>
                    </td>
                </tr>
            </FooterTemplate>
        </asp:Repeater>
    </table>
    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
