<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.SpamLog"
    CodeBehind="SpamLog.ascx.cs" %>


<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <div class="row justify-content-between align-items-center">
                        <div class="col-auto">
                            <YAF:IconHeader runat="server"
                                            IconName="shield-alt"
                                            LocalizedPage="ADMIN_SPAMLOG"></YAF:IconHeader>
                        </div>
                    <div class="col-auto">
                        <div class="btn-toolbar" role="toolbar">
                            <div class="input-group input-group-sm mr-2" role="group">
                                <div class="input-group-text">
                                    <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                </div>
                                <asp:DropDownList runat="server" ID="PageSize"
                                                  AutoPostBack="True"
                                                  OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                  CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                            <div class="btn-group btn-group-sm" role="group">
                         <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Size="Small"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        <div class="dropdown-menu dropdown-menu-right dropdown-menu-lg-left">
                            <div class="px-3 py-1">
                               <div class="mb-3">
                        <YAF:HelpLabel ID="SinceDateLabel" runat="server" 
                                       AssociatedControlID="SinceDate"
                                       LocalizedPage="ADMIN_EVENTLOG" LocalizedTag="SINCEDATE" />
                     
                        <div class='input-group mb-3 date datepickerinput'>
                            <button class="btn btn-secondary datepickerbutton" type="button">
                                <i class="fa fa-calendar-day fa-fw"></i>
                            </button>
                            <asp:TextBox ID="SinceDate" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="ToDateLabel" runat="server" 
                                       AssociatedControlID="ToDate"
                                       LocalizedPage="ADMIN_EVENTLOG" LocalizedTag="TODATE" />
                         
                        <div class='input-group mb-3 date datepickerinput'>
                            <button class="btn btn-secondary datepickerbutton" type="button">
                                <i class="fa fa-calendar-day fa-fw"></i>
                            </button>
                            <asp:TextBox ID="ToDate" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                                <div class="mb-3">
                                    <YAF:ThemeButton ID="ApplyButton" runat="server"
                                                     Type="Primary" 
                                                     CssClass="btn-block"
                                                     OnClick="ApplyButtonClick"
                                                     TextLocalizedPage="ADMIN_EVENTLOG" 
                                                     TextLocalizedTag="APPLY" 
                                                     Icon="check"></YAF:ThemeButton>
                                </div>
                            </div>
                            </div>
                                </div>
                            </div>
                    </div>
                        </div>
            </div>
                <div class="card-body">
        <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between text-break" 
                         onclick="javascript:document.querySelector('<%# ".btn-toggle-{0}".Fmt(this.Eval("EventLogID")) %>').click();">
                        <h5 class="mb-1">
                            <asp:HiddenField ID="EventTypeID" Value='<%# this.Eval("Type")%>' runat="server"/>
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                                               LocalizedTag="SOURCE" 
                                                                               LocalizedPage="ADMIN_EVENTLOG" />:&nbsp;
                            <%# Container.DataItemToField<string>("Source").IsSet() ? this.HtmlEncode(this.Eval( "Source")) : "N/A" %>
                        </h5>
                        <small class="d-none d-md-block">
                            <YAF:Icon runat="server" 
                                      IconName="calendar-day"
                                      IconNameBadge="clock"></YAF:Icon>
                            <%# this.Get<IDateTime>().FormatDateTimeTopic(Container.DataItemToField<DateTime>("EventTime")) %>
                        </small>
                    </div>
                    <p class="mb-1" 
                       onclick="javascript:document.querySelector('<%# ".btn-toggle-{0}".Fmt(this.Eval("EventLogID")) %>').click();">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EVENTLOG" />:
                        </span>
                        <%# this.UserLink(Container.DataItem) %>
                    </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <YAF:ThemeButton runat="server"
                                             Type="Info"
                                             Size="Small"
                                             TextLocalizedTag="SHOW" TextLocalizedPage="ADMIN_EVENTLOG"
                                             Icon="caret-square-down"
                                             CssClass='<%# "btn-toggle-{0}".Fmt(this.Eval("EventLogID")) %>'
                                             DataToggle="collapse"
                                             DataTarget='<%# "eventDetails{0}".Fmt(this.Eval("EventLogID")) %>'>
                            </YAF:ThemeButton>
                            <YAF:ThemeButton runat="server" 
                                             Type="Danger"
                                             Size="Small"
                                             CommandName="delete" CommandArgument='<%# this.Eval( "EventLogID") %>'
                                             ReturnConfirmText='<%# this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE") %>'
                                             Icon="trash" 
                                             TextLocalizedTag="DELETE">
                            </YAF:ThemeButton>
                        </div>
                    </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton runat="server" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         CommandName="delete" CommandArgument='<%# this.Eval( "EventLogID") %>'
                                         ReturnConfirmText='<%# this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton runat="server" 
                                         Type="None" 
                                         CssClass="dropdown-item"
                                         Icon="trash" 
                                         OnClick="DeleteAllClick" 
                                         TextLocalizedPage="ADMIN_EVENTLOG" TextLocalizedTag="DELETE_ALLOWED"
                                         ReturnConfirmText='<%#this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE_ALL") %>'>
                        </YAF:ThemeButton>
                    </div>
                    <div class="collapse mt-3" id="eventDetails<%# this.Eval("EventLogID") %>">
                        <div class="card card-body py-0">
                            <pre class="pre-scrollable">
                                <code>
                                    <%# this.HtmlEncode(this.Eval( "Description")) %>
                                </code>
                               </pre>
                        </div>
                    </div>
                </li>
            </ItemTemplate>
            <FooterTemplate>
               </ul>
            </FooterTemplate>
        </asp:Repeater>
                    <YAF:Alert runat="server" ID="NoInfo" 
                               Type="success" 
                               Visible="False">
                        <i class="fa fa-check fa-fw text-success"></i>
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NO_ENTRY"></YAF:LocalizedLabel>
                    </YAF:Alert>
                </div>
            <asp:Panel CssClass="card-footer text-center" runat="server" ID="Footer" Visible="<%# this.List.Items.Count > 0 %>">
                <YAF:ThemeButton runat="server" 
                                 Type="Danger"
                                 Icon="trash" 
                                 OnClick="DeleteAllClick" 
                                 TextLocalizedPage="ADMIN_EVENTLOG" TextLocalizedTag="DELETE_ALLOWED"
                                 ReturnConfirmText='<%#this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE_ALL") %>'>
                </YAF:ThemeButton>
            </asp:Panel>
        </div>
        </div>
    </div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTopPageChange"/>
    </div>
</div>