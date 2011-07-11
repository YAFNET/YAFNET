<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.movemessage" Codebehind="movemessage.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
		</td>
	</tr>
    <tr>
      <td colspan="2" class="header2">
        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="MOVE_TITLE" />
      </td>
    </tr>
	<tr>
		<td class="postheader" width="50%">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="select_forum_moveto" />
		</td>
		<td class="post" width="50%">
			<asp:DropDownList ID="ForumList" runat="server" CssClass="edit" AutoPostBack="True" OnSelectedIndexChanged="ForumList_SelectedIndexChanged" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="50%">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="select_topic_moveto" />
		</td>
		<td class="post" width="50%">
			<asp:DropDownList ID="TopicsList" runat="server" CssClass="edit" OnSelectedIndexChanged="TopicsList_SelectedIndexChanged" />
		</td>
	</tr>
	<tr>
		<td class="footer1" colspan="2" align="center">
			<asp:Button ID="Move" runat="server" CssClass="pbutton" OnClick="Move_Click" />
		</td>
	</tr>
    <tr>
      <td colspan="2" class="header2">
        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="SPLIT_TITLE" />
      </td>
    </tr>
	<tr>
		<td class="postheader" width="50%">
			<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="new_topic" />
		</td>
		<td class="post" width="50%">
			<asp:TextBox ID="TopicSubject" runat="server" CssClass="edit" />
		</td>
	</tr>
	<tr>
		<td class="footer1" colspan="2" align="center">
			<asp:Button ID="CreateAndMove" CssClass="pbutton" runat="server" OnClick="CreateAndMove_Click" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
