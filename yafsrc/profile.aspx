<%@ Page language="c#" Codebehind="profile.aspx.cs" AutoEventWireup="false" Inherits="yaf.profile" %>
<meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0">

<form runat="server">

<p class="navlinks">
	<asp:hyperlink runat="server" id="HomeLink"/>
	&#187; <asp:hyperlink runat="server" id="MembersLink"/>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<table class="content" align=center width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class="header1" colspan=2>
			<%= GetText("profile_profile") %> <asp:Label id=UserName runat="server"/> 
			
		</td>
	</tr>
	<tr>
		<td class="header2" colspan=2><%= GetText("profile_about") %></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("profile_username") %></td>
		<td class=post><asp:Label id=Name runat="server"/>&nbsp;<a href='pmessage.aspx?u=<%= Request.QueryString["u"].ToString() %>'>PM</a></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("profile_groups") %></td>
		<td class=post>
			<asp:repeater id="Groups" runat="server">
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
			</asp:repeater>
		</td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("profile_rank") %></td>
		<td class=post><asp:Label id=Rank runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("profile_joined") %></td>
		<td class=post><asp:Label id=Joined runat="server"/></td>
	</tr>
	<tr id=EmailRow runat=server visible=false>
		<td class=postheader><%= GetText("profile_email") %></td>
		<td class=post><asp:label id=Email runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("profile_lastvisit") %></td>
		<td class=post><asp:label id=LastVisit runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan=2><%= GetText("profile_statistics") %></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("profile_numposts") %></td>
		<td class=post><asp:label id=NumPosts runat="server"/></td>
	</tr>

	<asp:repeater id="LastPosts" runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2 colSpan=2><%= GetText("profile_last10") %></td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr class=postheader>
			<td class=small align=left colspan="2">
				<b><%= GetText("profile_topic") %></b> <a href='posts.aspx?t=<%# DataBinder.Eval(Container.DataItem,"TopicID") %>'><%# DataBinder.Eval(Container.DataItem,"Subject") %></a><br/>
				<b><%= GetText("profile_posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
			</td>
		</tr>
		<tr class=post>
			<td valign=top class="message" colspan="2">
				<%# FormatBody(Container.DataItem) %>
			</td>
		</tr>
	</ItemTemplate>
	</asp:repeater>
</table>

</form>
