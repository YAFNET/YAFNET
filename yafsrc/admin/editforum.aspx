<%@ Page language="c#" Codebehind="editforum.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.editforum" %>

<form runat="server">
<table class=content cellSpacing=1 cellPadding=0 width="100%">
  <TBODY>
  <tr>
    <td class=header1 colSpan=11>Edit Forum: <asp:label id=ForumNameTitle runat=server></asp:label></td></tr>
  <tr>
    <td class=postheader colspan=4><b>Category:</b><br>What category to put the forum under.</td>
    <td class=post colSpan=7><asp:dropdownlist id=CategoryList runat="server" DataTextField="Name" DataValueField="CategoryID"></asp:dropdownlist></td></tr>
  <tr>
    <td class=postheader colspan=4><b>Name:</b><br>The name of the forum.</td>
    <td class=post colSpan=7><asp:textbox id=Name runat="server" cssclass=edit></asp:textbox></td></tr>
  <tr>
    <td class=postheader colspan=4><b>Description:</b><br>A description of the forum.</td>
    <td class=post colSpan=7><asp:textbox id=Description runat="server" cssclass=edit></asp:textbox></td></tr>
  <tr>
    <td class=postheader colspan=4><b>SortOrder:</b><br>Sort order under this category.</td>
    <td class=post colSpan=7><asp:textbox id=SortOrder runat="server"></asp:textbox></td></tr>
  <tr>
    <td class=postheader colspan=4><b>Hide if no access:</b><br>Means that the forum will be hidden when the user don't have read access to it.</td>
    <td class=post colSpan=7><asp:checkbox id=HideNoAccess runat="server"></asp:checkbox></td></tr>
  <tr>
    <td class=postheader colspan=4><b>Locked:</b><br>If the forum is locked, no one can post or reply in this forum.</td>
    <td class=post colSpan=7><asp:checkbox id=Locked runat="server"></asp:checkbox></td></tr>
    
	<tr>
		<td class=postheader colspan=4><b>Is Test:</b><br>If this is checked, posts in this forum will not count in the ladder system.</td>
		<td class=post colSpan=7><asp:checkbox id="IsTest" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<td class=postheader colspan=4><b>Moderated:</b><br/>If the forum is moderated, posts have to be approved by a moderator.</td>
		<td class=post colspan=7><asp:checkbox id="Moderated" runat="server"/></td>
	</tr>
	<tr runat="server" id="TemplateRow">
		<td class="postheader" colspan="4"><b>Template Forum:</b><br/>Create access rights based on this forum.</td>
		<td class="post" colspan="7"><asp:dropdownlist id="TemplateID" runat="server"/></td>
	</tr>

    <asp:repeater id=AccessList runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header1 colspan=11>Access</td>
		</tr>
		<tr>
			<td class=header2>Group</td>
			<td class=header2 width=7%>Read</td>
			<td class=header2 width=7%>Post</td>
			<td class=header2 width=7%>Reply</td>
			<td class=header2 width=7%>Edit</td>
			<td class=header2 width=7%>Delete</td>
			<td class=header2 width=7%>Priority</td>
			<td class=header2 width=7%>Create Poll</td>
			<td class=header2 width=7%>Vote</td>
			<td class=header2 width=7%>Moderator</td>
			<td class=header2 width=7%>Upload</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post>
				<asp:label id=GroupID visible=false runat="server" text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'></asp:label>
				<%# DataBinder.Eval(Container.DataItem, "GroupName") %>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=ReadAccess checked='<%# DataBinder.Eval(Container.DataItem, "ReadAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=PostAccess checked='<%# DataBinder.Eval(Container.DataItem, "PostAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=ReplyAccess checked='<%# DataBinder.Eval(Container.DataItem, "ReplyAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=EditAccess checked='<%# DataBinder.Eval(Container.DataItem, "EditAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=DeleteAccess checked='<%# DataBinder.Eval(Container.DataItem, "DeleteAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=PriorityAccess checked='<%# DataBinder.Eval(Container.DataItem, "PriorityAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=PollAccess checked='<%# DataBinder.Eval(Container.DataItem, "PollAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=VoteAccess checked='<%# DataBinder.Eval(Container.DataItem, "VoteAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=ModeratorAccess checked='<%# DataBinder.Eval(Container.DataItem, "ModeratorAccess") %>'></asp:checkbox>
			</td>
			<td align=center class=post>
				<asp:checkbox runat="server" id=UploadAccess checked='<%# DataBinder.Eval(Container.DataItem, "UploadAccess") %>'></asp:checkbox>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>
  <tr>
    <td class=postfooter align=middle colSpan=11><asp:button id=Save runat="server" Text="Save"></asp:button>&nbsp;
<asp:Button id=Cancel runat="server" Text="Cancel"></asp:Button></td></tr></TBODY></table></form>
