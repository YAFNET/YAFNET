<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.movetopic" Codebehind="movetopic.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="50%">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="select_forum" />
		</td>
		<td class="post" width="50%">
			<asp:DropDownList ID="ForumList" runat="server" DataValueField="ForumID" DataTextField="Title" />
		</td>
    </tr>
		<tr id="trLeaveLink" runat="server">
			<td class="postheader" width="50%">
				<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="LEAVE_POINTER" />
			</td>
			<td class="post" width="50%">
				<asp:CheckBox ID="LeavePointer" runat="server" />
			</td>
		</tr>
	<tr>
	</tr>
		<tr id="trLeaveLinkDays" runat="server">
			<td class="postheader" width="50%">
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="POINTER_DAYS" />
			</td>
			<td class="post" width="50%">
				<asp:TextBox ID="LinkDays" runat="server" />
			</td>
		</tr>
	<tr>   

		<td class="footer1" colspan="2" align="center">
			<asp:Button ID="Move" CssClass="pbutton" runat="server" OnClick="Move_Click" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
