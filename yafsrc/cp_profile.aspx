<%@ Page language="c#" Codebehind="cp_profile.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_profile" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<yaf:PageLinks runat="server" id="PageLinks"/>

<table width="100%" cellspacing=1 cellpadding=0 class=content >
  <TBODY>
	<tr>
		<td colspan=2 class=header1><%= GetText("control_panel") %>: <asp:label id="TitleUserName" runat="server"/></td>
	</tr>
	<tr>
		<td valign=top class=post width=150>
			<p style="FONT-WEIGHT:bold;BORDER-BOTTOM:black 1px solid"><%= GetText("messenger") %></p>
			<li><a href="cp_inbox.aspx"><%= GetText("inbox") %></a></li>
			<li><a href="cp_inbox.aspx?sent=1"><%= GetText("sentitems") %></a></li>
			<li><a href="pmessage.aspx"><%= GetText("new_message") %></a></li>
      
			<p></p>
			
			<p style="FONT-WEIGHT:bold;BORDER-BOTTOM:black 1px solid"><%= GetText("personal_profile") %></p>
			<li><a href="cp_editprofile.aspx"><%= GetText("edit_profile") %></a></li>
			<li><a href="cp_signature.aspx"><%= GetText("signature") %></a></li>
			<li><a href="cp_subscriptions.aspx"><%= GetText("subscriptions") %></a></li>
      
			<p></p>
			<p></p>
		</td>
		
<% // DefaultView %>
<td valign=top class=post>

<table align=center cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td colspan=3 class=header2><%= GetText("your_account") %></td>
	</tr>
	<tr>
		<td width="33%"><%= GetText("your_username") %></td>
		<td><asp:Label id=Name runat="server"/></td>
		<td valign="top" rowspan="5"><img runat="server" id="AvatarImage" align="right"/></td>
	</tr>
	<tr>
		<td><%= GetText("your_email") %></td>
		<td><asp:label id=AccountEmail runat="server"/></td>
	</tr>
	<tr>
		<td><%= GetText("numposts") %></td>
		<td><asp:Label id=NumPosts runat="server"/></td>
	</tr>
	<tr>
		<td><%= GetText("groups") %></td>
		<td>
			<asp:repeater id="Groups" runat="server">
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
			</asp:repeater>
		</td>
	</tr>
	<tr>
		<td><%= GetText("joined") %></td>
		<td><asp:Label id=Joined runat="server"/></td>
	</tr>
</table>

</td>

<% // end %>
</TR></TBODY></TABLE>

</form>
