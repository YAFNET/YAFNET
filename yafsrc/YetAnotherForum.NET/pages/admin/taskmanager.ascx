<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.taskmanager"
    CodeBehind="taskmanager.ascx.cs" %>
<%@ Import Namespace="YAF.Core.Tasks" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_TASKMANAGER" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-tasks fa-fw"></i>&nbsp;<asp:Label ID="lblTaskCount" runat="server"></asp:Label>
                    </div>
                <div class="card-body">
              <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
                            <thead>
                               <tr>
            <th>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_NNTPSERVERS" />
            </th>
            <th>
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="RUNNING" LocalizedPage="ADMIN_TASKMANAGER" />
            </th>
            <th>
               <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DURATION" LocalizedPage="ADMIN_TASKMANAGER" />
            </th>

</tr>
            </thead>
        <asp:Repeater ID="taskRepeater" runat="server" OnItemCommand="TaskRepeaterItemCommand">
            <ItemTemplate>
                <tr>
                    <td>
                        <strong>
                            <%# this.Eval("Key") %></strong>
                            <asp:PlaceHolder ID="StopTaskHolder" runat="server" Visible="<%# Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.IsStoppable() %>">
                            <YAF:ThemeButton ID="stop" runat="server" 
                                             CommandName="stop" CommandArgument='<%# this.Eval("Key") %>'
                                             TextLocalizedTag="STOP_TASK" TextLocalizedPage="ADMIN_TASKMANAGER"
                                             Icon="hand-paper" Type="Danger" CssClass="btn-sm">
                            </YAF:ThemeButton>
                        </asp:PlaceHolder>
                    </td>
                    <td>
                    <asp:Label ID="Label2" runat="server" CssClass='<%# this.GetItemColor(this.Eval("Value.IsRunning").ToType<bool>()) %>'><%# this.GetItemName(this.Eval("Value.IsRunning").ToType<bool>())%></asp:Label>
                    </td>
                    <td>
                        <%# this.FormatTimeSpan(Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.Started)%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table></div>
                    </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
