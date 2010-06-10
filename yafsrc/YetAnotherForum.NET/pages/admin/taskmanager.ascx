<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.taskmanager"
    CodeBehind="taskmanager.ascx.cs" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="YAF.Classes.Core.Tasks" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="3">
                Task Manager:
                <asp:Label ID="lblTaskCount" runat="server"></asp:Label>
                Task(s) Running
            </td>
        </tr>
        <tr class="header2">
            <td>
                Name
            </td>
            <td>
                Is Running
            </td>
            <td>
                Duration
            </td>
        </tr>
        <asp:Repeater ID="taskRepeater" runat="server" OnItemCommand="taskRepeater_ItemCommand">
            <ItemTemplate>
                <tr>
                    <td>
                        <b>
                            <%# Eval("Key") %></b>
                            <asp:PlaceHolder ID="StopTaskHolder" runat="server" Visible="<%# Container.ToDataItemType<KeyValuePair<string, IBackgroundTask>>().Value.IsStoppable() %>">
                            [<asp:LinkButton ID="stop" Text="Stop Task" runat="server" CommandName="stop" CommandArgument=<%# Eval("Key") %>></asp:LinkButton>]
                        </asp:PlaceHolder>
                    </td>
                    <td>
                        <%# Eval("Value.IsRunning") %>
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
