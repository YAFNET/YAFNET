<%@ Page language="c#" Codebehind="cp_profile.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_profile" %>

<form runat="server">

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=UserLink runat="server"/>
</p>

<table width="100%" cellspacing=1 cellpadding=0 class=content >
  <TBODY>
	<tr>
		<td colspan=2 class=header1><%= GetText("cp_profile_control_panel") %>: <asp:label id="TitleUserName" runat="server"/></td>
	</tr>
	<tr>
		<td valign=top class=post width=150>
			<p style="FONT-WEIGHT:bold;BORDER-BOTTOM:black 1px solid"><%= GetText("cp_profile_messenger") %></p>
			<li><a href="cp_inbox.aspx"><%= GetText("cp_profile_inbox") %></a></li>
			<li><a href="cp_inbox.aspx?sent=1"><%= GetText("cp_profile_sentitems") %></a></li>
			<li><a href="pmessage.aspx"><%= GetText("cp_profile_new_message") %></a></li>
      
			<p></p>
			
			<p style="FONT-WEIGHT:bold;BORDER-BOTTOM:black 1px solid"><%= GetText("cp_profile_personal_profile") %></p>
			<li><a href="cp_editprofile.aspx"><%= GetText("cp_profile_edit_profile") %></a></li>
			<li><a href="cp_signature.aspx"><%= GetText("cp_profile_signature") %></a></li>
			<li><a href="cp_subscriptions.aspx"><%= GetText("cp_profile_subscriptions") %></a></li>
      
			<p></p>
			<p></p>
		</td>
		
<% // DefaultView %>
<td valign=top class=post>

<table align=center cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td colspan=3 class=header2><%= GetText("cp_profile_your_account") %></td>
	</tr>
	<tr>
		<td width="33%"><%= GetText("cp_profile_your_username") %></td>
		<td><asp:Label id=Name runat="server"/></td>
		<td valign="top" rowspan="5"><img runat="server" id="AvatarImage" align="right"/></td>
	</tr>
	<tr>
		<td><%= GetText("cp_profile_your_email") %></td>
		<td><asp:label id=AccountEmail runat="server"/></td>
	</tr>
	<tr>
		<td><%= GetText("cp_profile_numposts") %></td>
		<td><asp:Label id=NumPosts runat="server"/></td>
	</tr>
	<tr>
		<td><%= GetText("cp_profile_groups") %></td>
		<td>
			<asp:repeater id="Groups" runat="server">
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
			</asp:repeater>
		</td>
	</tr>
	<tr>
		<td><%= GetText("cp_profile_joined") %></td>
		<td><asp:Label id=Joined runat="server"/></td>
	</tr>
</table>

</td>

<% // end %>
</TR></TBODY></TABLE>

</form>
