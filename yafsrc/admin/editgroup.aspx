<%@ Page language="c#" Codebehind="editgroup.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.editgroup" %>



<form runat="server">
<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class=header1 colspan=11>Edit Group</td>
</tr>
<tr>
	<td class=postheader colspan=4><b>Name:</b><br/>Name of this group.</td>
	<td class=post colspan=7><asp:textbox style="width:300px" id=Name runat="server"/></td>
</tr>
<tr>
	<td class=postheader colspan=4><b>Is Admin:</b><br/>Means that users in this group are admins.</td>
	<td class=post colspan=7><asp:checkbox id=IsAdminX runat="server"></asp:checkbox></td>
</tr>
<tr>
	<td class=postheader colspan=4><b>Is Guest:</b><br/>Means that users in this group are guests (anonymous). Only one group should have this checked.</td>
	<td class=post colspan=7><asp:checkbox id=IsGuestGroup runat="server"></asp:checkbox></td>
</tr>
<tr>
	<td class=postheader colspan=4><b>Is Start:</b><br/>If this is checked, all new users will be a member of this group.</td>
	<td class=post colspan=7><asp:checkbox id=IsStart runat="server"></asp:checkbox></td>
</tr>
<tr>
	<td class=postheader colspan=4><b>Is Forum Moderator:</b><br/>When this is checked, members of this group will have some admin access rights.</td>
	<td class=post colspan=7><asp:checkbox id=IsModeratorX runat="server"></asp:checkbox></td>
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
			<asp:label id=ForumID visible=false runat="server" text='<%# DataBinder.Eval(Container.DataItem, "ForumID") %>'></asp:label>
			<b><%# DataBinder.Eval(Container.DataItem, "ForumName") %></b><br>
			Category: <%# DataBinder.Eval(Container.DataItem, "CategoryName") %>
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
    <td class=postfooter align=middle colspan=11><asp:button id=Save runat="server" Text="Save"></asp:button>&nbsp; 
<asp:button id=Cancel runat="server" Text="Cancel"></asp:button></td></tr></table></form>
