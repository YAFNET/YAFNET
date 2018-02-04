<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.spamlog"
    CodeBehind="spamlog.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<script type="text/javascript">
function toggleItem(detailId)
{
    var show = '<i class="fa fa-caret-square-down fa-fw"></i>&nbsp;<%# this.GetText("ADMIN_EVENTLOG", "SHOW")%>';
    var hide = '<i class="fa fa-caret-square-up fa-fw"></i>&nbsp;<%# this.GetText("ADMIN_EVENTLOG", "HIDE")%>';

	jQuery('#Show'+ detailId).html($('#Show'+ detailId).html() == show ? hide : show);

	jQuery('#eventDetails' + detailId).slideToggle('slow');

	return false;

}
</script>

<YAF:AdminMenu runat="server" ID="AdminMenu1">
        <div class="row">
            <div class="col-xl-12">
                <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMLOG" /></h1>
            </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-shield-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMLOG" />
            </div>
                <div class="card-body">
                    <h4>
                        <YAF:HelpLabel ID="SinceDateLabel" runat="server" LocalizedPage="ADMIN_EVENTLOG" LocalizedTag="SINCEDATE" />
                    </h4>
                    <div class='input-group mb-3 date datepickerinput'>
                        <span class="input-group-prepend">
                            <button class="btn btn-secondary datepickerbutton" type="button">
                                <i class="fa fa-calendar fa-fw"></i>
                            </button>
                        </span>
                            <asp:TextBox ID="SinceDate" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                   <hr />
                    <h4>
                <YAF:HelpLabel ID="ToDateLabel" runat="server" LocalizedPage="ADMIN_EVENTLOG" LocalizedTag="TODATE" />
                        </h4>
                    <div class='input-group mb-3 date datepickerinput'>
                        <span class="input-group-prepend">
                            <button class="btn btn-secondary datepickerbutton" type="button">
                                <i class="fa fa-calendar fa-fw"></i>
                            </button>
                        </span>
                            <asp:TextBox ID="ToDate" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                   <hr />
        </div>
                <div class="card-footer text-lg-center">
                <YAF:ThemeButton ID="ApplyButton" Type="Primary" OnClick="ApplyButtonClick"
                    TextLocalizedPage="ADMIN_EVENTLOG" TextLocalizedTag="APPLY" Icon="check" runat="server"></YAF:ThemeButton>
            </div>
           </div>
         </div>
    </div>
        <div class="row">
        <div class="col-xl-12">
                <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTopPageChange" />
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-shield-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMLOG" />
            </div>
                <div class="card-body">
                     <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive"><table class="table">
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td colspan="5">
                      <div onclick="javascript:toggleItem(<%# this.Eval("EventLogID") %>);">
                        <div class="table-responsive"><table class="table">
                          <tr class="table-<%# this.EventCssClass(Container.DataItem) %>">
                            <td>
                              <a name="event<%# this.Eval("EventLogID")%>" ></a>
                              <asp:HiddenField ID="EventTypeID" Value='<%# this.Eval("Type")%>' runat="server"/>
                            </td>
                            <td>
                                <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EVENTLOG" />:</strong>&nbsp;
                                <%# this.HtmlEncode(this.Eval( "UserName")).IsSet() ? this.HtmlEncode(this.Eval( "UserName")) : "N/A" %>&nbsp;
                              <strong><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TYPE" LocalizedPage="ADMIN_EVENTLOG" />:</strong>&nbsp;
                                <%# this.HtmlEncode(this.Eval( "Name")).IsSet() ? this.HtmlEncode(this.Eval( "Name")) : "N/A" %>&nbsp;
                               <strong><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TIME" LocalizedPage="ADMIN_EVENTLOG" />:</strong>&nbsp;
                              <%# this.Get<IDateTime>().FormatDateTimeTopic(Container.DataItemToField<DateTime>("EventTime")) %>&nbsp;
                              <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SOURCE" LocalizedPage="ADMIN_EVENTLOG" />:</strong>&nbsp;
                              <%# this.HtmlEncode(this.Eval( "Source")).IsSet() ? this.HtmlEncode(this.Eval( "Source")) : "N/A" %>
                            </td>
                            <td>
                                <span class="float-right">
                                    <a class="showEventItem btn btn-info btn-sm" href="#event<%# this.Eval("EventLogID")%>" id="Show<%# this.Eval("EventLogID") %>"><i class="fa fa-caret-square-down fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SHOW" LocalizedPage="ADMIN_EVENTLOG" /></a>&nbsp;&nbsp;
                                    <YAF:ThemeButton runat="server" CssClass="deleteEventItem btn btn-danger btn-sm" 
                                                     CommandName="delete" CommandArgument='<%# this.Eval( "EventLogID") %>'
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE") %>'
                                                     Icon="Trash" TextLocalizedTag="DELETE">
                                    </YAF:ThemeButton>
                                </span>
                            </td>
                          </tr>
                        </table>
                            </div>
                      </div>
                      <div class="EventDetails" id="eventDetails<%# this.Eval("EventLogID") %>" style="display: none;margin:0;padding:0;">
                            <pre class="pre-scrollable">
                                <code>
                                    <%# this.HtmlEncode(this.Eval( "Description")) %>
                                </code>
                            </pre>
                        </div>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table></div>
                </div>
                <div class="card-footer text-lg-center">
                        <YAF:ThemeButton runat="server" Visible="<%# this.List.Items.Count > 0 %>" Type="Primary"
                            Icon="trash" OnClick="DeleteAllClick" 
                                         TextLocalizedPage="ADMIN_EVENTLOG" TextLocalizedTag="DELETE_ALLOWED"
                                         ReturnConfirmText='<%#this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE_ALL") %>'>
                        </YAF:ThemeButton>
                </div>
            </FooterTemplate>
        </asp:Repeater>
                    </div>
    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
