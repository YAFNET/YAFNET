<%@ Control Language="c#" Codebehind="movetopic.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.movetopic" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
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
</tr>
<tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="Move" runat="server" OnClick="Move_Click" />
	</td>
</tr>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
