<%@ Page language="c#" Codebehind="profile.aspx.cs" AutoEventWireup="false" Inherits="yaf.profile" %>
<meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie3-2nav3-0">

<form runat="server">

<p class="navlinks">
	<asp:hyperlink runat="server" id="HomeLink"></asp:hyperlink>
	&#187; <a href="members.aspx">Members</a>
</p>

<table class="content" align=center width="100%" cellspacing=1 cellpadding=0>
	<tr><td class="header1" colspan=2>Profile: <asp:Label id=UserName runat="server">Label</asp:Label> </td></tr>
	<tr>
		<td class="header2" colspan=2>About</td>
	</tr>
	<tr>
		<td class=postheader>User Name:</td>
		<td class=post><asp:Label id=Name runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader>Groups:</td>
		<td class=post>
			<asp:repeater id="Groups" runat="server">
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
			</asp:repeater>
		</td>
	</tr>
	<tr>
		<td class=postheader>Rank:</td>
		<td class=post><asp:Label id=Rank runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader>Joined:</td>
		<td class=post><asp:Label id=Joined runat="server"/></td>
	</tr>
	<tr id=EmailRow runat=server visible=false>
		<td class=postheader>E-mail:</td>
		<td class=post><asp:label id=Email runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader>Last Visit:</td>
		<td class=post><asp:label id=LastVisit runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan=2>Statistics</td>
	</tr>
	<tr>
		<td class=postheader>Number of Posts:</td>
		<td class=post><asp:label id=NumPosts runat="server"/></td>
	</tr>
</table>

</form>
