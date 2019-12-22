<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.taskmanager"
    CodeBehind="taskmanager.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_TASKMANAGER" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-tasks fa-fw text-secondary"></i>&nbsp;<asp:Label ID="lblTaskCount" runat="server"></asp:Label>
                    </div>
                <div class="card-body">
        <asp:Repeater ID="taskRepeater" runat="server" OnItemCommand="TaskRepeaterItemCommand">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">
                            <%# this.Eval("Key") %>
                        </h5>
                        <small class="d-none d-md-block">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                LocalizedTag="DURATION" 
                                                LocalizedPage="ADMIN_TASKMANAGER" />:
                            <%# this.FormatTimeSpan(Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.Started)%>
                        </small>
                    </div>
                    <p class="mb-1">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="RUNNING" 
                                            LocalizedPage="ADMIN_TASKMANAGER" />: 
                        <asp:Label ID="Label2" runat="server" 
                                   CssClass='<%# this.GetItemColor(this.Eval("Value.IsRunning").ToType<bool>()) %>'><%# this.GetItemName(this.Eval("Value.IsRunning").ToType<bool>())%></asp:Label>
                    </p>
                    <asp:PlaceHolder ID="StopTaskHolder" runat="server" 
                                     Visible="<%# Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.IsStoppable() %>">
                        <small>
                            <YAF:ThemeButton ID="stop" runat="server"
                                             CommandName="stop" 
                                             CommandArgument='<%# this.Eval("Key") %>'
                                             TextLocalizedTag="STOP_TASK" TextLocalizedPage="ADMIN_TASKMANAGER"
                                             Icon="hand-paper" 
                                             Type="Danger" 
                                             Size="Small">
                            </YAF:ThemeButton>
                            <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                                <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                                 CommandName="stop" 
                                                 CommandArgument='<%# this.Eval("Key") %>'
                                                 TextLocalizedTag="STOP_TASK" TextLocalizedPage="ADMIN_TASKMANAGER"
                                                 Icon="hand-paper" 
                                                 Type="None"
                                                 CssClass="dropdown-item" />
                            </div>
                        </small>
                    </asp:PlaceHolder>
                </li>
            </ItemTemplate>
        </asp:Repeater>
                    </div>
            </div>
        </div>
    </div>


