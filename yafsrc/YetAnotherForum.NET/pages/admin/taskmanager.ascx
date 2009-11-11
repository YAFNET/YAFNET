<%@ Control Language="C#" AutoEventWireup="true" CodeFile="taskmanager.ascx.cs" Inherits="YAF.Pages.Admin.taskmanager" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="3">
			
			Task Manager -- <asp:Label ID="lblTaskCount" runat="server"></asp:Label>
			
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
		<asp:Repeater ID="taskRepeater" runat="server">
            <ItemTemplate>
                <tr>
                    <td>
                        <b>
                            <%# Eval("Key") %></b>
                    </td>                   
                    <td>
                        <%# Eval("Value.IsRunning") %>
                    </td>
                   <td>
                        <%# FormatTimeSpan( Container.DataItem ) %>
                    </td>                     
                </tr>
            </ItemTemplate>
        </asp:Repeater>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
