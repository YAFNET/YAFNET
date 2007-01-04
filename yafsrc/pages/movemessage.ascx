<%@ Control language="c#" Codebehind="movemessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.movemessage" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2"><%= GetText("title") %></td>
	</tr>
	<tr>
		<td class="postheader" width="50%"><%= GetText("select_forum_moveto") %></td>
		<td class="post" width="50%">
			<asp:DropDownList id="ForumList" runat="server" AutoPostBack="True" onselectedindexchanged="ForumList_SelectedIndexChanged" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="50%"><%= GetText("select_theme_moveto")%></td>
		<td class="post" width="50%">
			<asp:DropDownList id="TopicsList" runat="server" onselectedindexchanged="TopicsList_SelectedIndexChanged" />
		</td>
	</tr>
	<tr>
		<td class="footer1" colspan="2" align="center">
			<asp:Button id="Move" runat="server" onclick="Move_Click" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="50%"><%= GetText("new_theme") %></td>
		<td class="post" width="50%">
			<asp:textbox id="ThemeSubject" runat="server" cssclass="edit" />
		</td>
	</tr>
	<tr>
		<td class="footer1" colspan="2" align="center">
			<asp:Button id="CreateAndMove" runat="server" onclick="CreateAndMove_Click" />
		</td>
	</tr>
</table>
<YAF:SmartScroller id="SmartScroller1" runat="server" />
