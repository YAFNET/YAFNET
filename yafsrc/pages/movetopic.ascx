<%@ Control Language="c#" CodeFile="movetopic.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.movetopic" %>




<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
<tr>
        <td class="header1" colspan="2">
            <%= GetText("title") %>
        </td>
</tr>
<tr>
        <td class="postheader" width="50%">
            <%= GetText("select_forum") %>
        </td>
        <td class="post" width="50%">
            <asp:DropDownList ID="ForumList" runat="server" DataValueField="ForumID" DataTextField="Title" />
	</td>
    <tr>
        <td class="postheader" width="50%">
           <%= GetText("LEAVE_POINTER")%>
        </td>
        <td class="post" width="50%">
            <asp:CheckBox ID="LeavePointer" runat="server" />
        </td>
    </tr>
</tr>
<tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="Move" runat="server" OnClick="Move_Click" />
	</td>
</tr>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
