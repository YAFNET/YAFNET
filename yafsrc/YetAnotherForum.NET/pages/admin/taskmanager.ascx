<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.taskmanager"
    CodeBehind="taskmanager.ascx.cs" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="YAF.Core.Tasks" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="3">
                <asp:Label ID="lblTaskCount" runat="server"></asp:Label>
            </td>
        </tr>
        <tr class="header2">
            <td>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_NNTPSERVERS" />
            </td>
            <td>
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="RUNNING" LocalizedPage="ADMIN_TASKMANAGER" />
            </td>
            <td>
               <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DURATION" LocalizedPage="ADMIN_TASKMANAGER" />
            </td>
        </tr>
        <asp:Repeater ID="taskRepeater" runat="server" OnItemCommand="taskRepeater_ItemCommand">
            <ItemTemplate>
                <tr>
                    <td>
                        <strong>
                            <%# Eval("Key") %></strong>
                            <asp:PlaceHolder ID="StopTaskHolder" runat="server" Visible="<%# Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.IsStoppable() %>">
                            [<asp:LinkButton ID="stop" runat="server" CommandName="stop" CommandArgument='<%# Eval("Key") %>'><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="STOP_TASK" LocalizedPage="ADMIN_TASKMANAGER" /></asp:LinkButton>]
                        </asp:PlaceHolder>
                    </td>
                    <td>
                    <asp:Label ID="Label2" runat="server" ForeColor='<%# GetItemColor(Eval("Value.IsRunning").ToType<bool>()) %>'><%# GetItemName(Eval("Value.IsRunning").ToType<bool>())%></asp:Label>
                    </td>
                    <td>
                        <%# FormatTimeSpan(Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.Started)%>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
