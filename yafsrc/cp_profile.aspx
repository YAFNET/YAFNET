<%@ Page language="c#" Codebehind="cp_profile.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_profile" %>

<form runat="server">

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=UserLink runat="server">UserLink</asp:hyperlink>
</p>

<table width="100%" cellspacing=1 cellpadding=0 class=content >
  <TBODY>
	<tr>
		<td colspan=2 class=header1>Control Panel: <asp:label id="TitleUserName" runat=server></asp:label></td>
	</tr>
	<tr>
		<td valign=top class=post width=150>
			<p style="FONT-WEIGHT:bold;BORDER-BOTTOM:black 1px solid">Messenger:</p>
			<li><a href="cp_inbox.aspx">Inbox</a></li>
			<li><a href="cp_sentitems.aspx">Sent Items</a></li>
			<li><a href="pmessage.aspx">New Message</a></li>
      
			<p></p>
			
			<p style="FONT-WEIGHT:bold;BORDER-BOTTOM:black 1px solid">Personal Profile:</p>
			<li><a href="cp_editprofile.aspx">Edit Profile</a></li>
			<li><a href="cp_signature.aspx">Signature</a></li>
			<li><a href="cp_subscriptions.aspx">Subscriptions</a></li>
      
			<p></p>
			<p></p>
		</td>
		
<% // DefaultView %>
<td valign=top class=post>

<table align=center cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td colspan=2 class=header2>Your Account</td>
	</tr>
	<tr>
		<td width="33%">Your user name:</td>
		<td><asp:Label id=Name runat="server"/></td>
	</tr>
	<tr>
		<td>Your email address:</td>
		<td><asp:label id=AccountEmail runat="server"/></td>
	</tr>
	<tr>
		<td>Number of posts:</td>
		<td><asp:Label id=NumPosts runat="server"/></td>
	</tr>
	<tr>
		<td>You are a member of these groups:</td>
		<td>
			<asp:repeater id="Groups" runat="server">
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
			</asp:repeater>
		</td>
	</tr>
	<tr>
		<td>When you joined:</td>
		<td><asp:Label id=Joined runat="server"/></td>
	</tr>
</table>

</td>

<% // end %>
</TR></TBODY></TABLE>

</form>
