<%@ Page language="c#" Codebehind="members.aspx.cs" AutoEventWireup="false" Inherits="yaf.members" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<table class=command><tr><td class=navlinks id=PageLinks1 runat=server></td></tr></table>

<table class=content width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class=header1 colspan=4><%= GetText("members_title") %></td>
	</tr>
	<tr>
		<td class=header2><asp:linkbutton runat=server id="UserName"/></td>
		<td class=header2><asp:linkbutton runat=server id="Rank"/></td>
		<td class=header2><asp:linkbutton runat=server id="Joined"/></td>
		<td class=header2 align=center><asp:linkbutton runat=server id="Posts"/></td>
	</tr>
	
	<asp:repeater id=MemberList runat=server>
		<ItemTemplate>
			<tr>
				<td class=post><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem,"UserID") %>'><%# DataBinder.Eval(Container.DataItem,"Name") %></a></td>
				<td class=post><%# DataBinder.Eval(Container.DataItem,"RankName") %></td>
				<td class=post><%# FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %></td>
				<td class=post align=center><%# String.Format(CustomCulture,"{0:N0}",((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %></td>
			</tr>
		</ItemTemplate>
	</asp:repeater>
</table>

<asp:linkbutton id=GoPage runat=server visible=false/>

<table class=command><tr><td class=navlinks id=PageLinks2 runat=server></td></tr></table>

</form>
