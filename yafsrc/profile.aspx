<%@ Page language="c#" Codebehind="profile.aspx.cs" AutoEventWireup="false" Inherits="yaf.profile" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" align=center width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class="header1" colspan=2>
			<%= GetText("profile") %> <asp:Label id=UserName runat="server"/> 
		</td>
	</tr>
	<tr>
		<td class="header2" colspan=2><%= GetText("about") %></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("username") %></td>
		<td class=post><asp:Label id=Name runat="server"/>&nbsp;<a href='pmessage.aspx?u=<%= Request.QueryString["u"].ToString() %>'>PM</a></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("groups") %></td>
		<td class=post>
			<asp:repeater id="Groups" runat="server">
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
			</asp:repeater>
		</td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("rank") %></td>
		<td class=post><asp:Label id=Rank runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("joined") %></td>
		<td class=post><asp:Label id=Joined runat="server"/></td>
	</tr>
	<tr id=EmailRow runat=server visible=false>
		<td class=postheader><%= GetText("email") %></td>
		<td class=post><asp:label id=Email runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("lastvisit") %></td>
		<td class=post><asp:label id=LastVisit runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan=2><%= GetText("statistics") %></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("numposts") %></td>
		<td class=post><asp:label id=NumPosts runat="server"/></td>
	</tr>

	<asp:placeholder runat="server" id="ModeratorInfo" visible="false">
	<tr>
		<td class="header2" colspan=2><%= GetText("admin") %></td>
	</tr>
	<tr runat="server" id="SuspendedRow">
		<td class=postheader><%= GetText("ENDS") %></td>
		<td class=post>
			<%= GetSuspendedTo() %>&nbsp;<asp:button runat="server" id="RemoveSuspension"/>
		</td>	
	</tr>
	<tr>
		<td class=postheader>Suspend User:</td>
		<td class=post>
			<asp:textbox runat="server" id="SuspendCount" style="width:60px"/>&nbsp;<asp:dropdownlist runat="server" id="SuspendUnit"/>&nbsp;<asp:button runat="server" id="Suspend"/>
		</td>	
	</tr>
	</asp:placeholder>


	<asp:repeater id="LastPosts" runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2 colSpan=2><%= GetText("last10") %></td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr class=postheader>
			<td class=small align=left colspan="2">
				<b><%= GetText("topic") %></b> <a href='posts.aspx?t=<%# DataBinder.Eval(Container.DataItem,"TopicID") %>'><%# DataBinder.Eval(Container.DataItem,"Subject") %></a><br/>
				<b><%= GetText("posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
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
