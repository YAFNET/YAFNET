<%@ Control language="c#" CodeFile="admin.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.admin" %>




<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<asp:repeater id="ActiveList" runat="server">
		<HeaderTemplate>
			<table width="100%" cellspacing="1" cellpadding="0" class="content">
				<tr>
					<td class="header1" colspan="5">Who is Online</td>
				</tr>
				<tr>
					<td class="header2">Name</td>
					<td class="header2">IP Address</td>
					<td class="header2">Location</td>
					<td class="header2">Forum Location</td>
					<td class="header2">Topic Location</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post"><%# Eval("Name") %></td>
				<td class="post"><%# Eval("IP") %></td>
				<td class="post"><%# Eval("Location") %></td>
				<td class="post"><%# FormatForumLink(Eval("ForumID"),Eval("ForumName")) %></td>
				<td class="post"><%# FormatTopicLink(Eval("TopicID"),Eval("TopicName")) %></td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:repeater>
	<asp:repeater id="UserList" runat="server">
		<HeaderTemplate>
			<br />
			<table width="100%" cellspacing="1" cellpadding="0" class="content">
				<tr>
					<td class="header1" colspan="5">Unverified Users</td>
				</tr>
				<tr>
					<td class="header2">Name</td>
					<td class="header2">Email</td>
					<td class="header2">Location</td>
					<td class="header2">Joined</td>
					<td class="header2">&nbsp;</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post"><%# Eval("Name") %></td>
				<td class="post"><%# Eval("Email") %></td>
				<td class="post"><%# Eval("Location") %></td>
				<td class="post"><%# yaf_DateTime.FormatDateTime(Eval("Joined")) %></td>
				<td class="post">
					<asp:linkbutton onload="Approve_Load" runat="server" commandname="approve" commandargument='<%# Eval("UserID") %>' text="Approve"/>
					|
					<asp:linkbutton onload="Delete_Load" runat="server" commandname="delete" commandargument='<%# Eval("UserID") %>' text="Delete"/>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="5">
					<asp:button commandname="deleteall" title="Deletes all unverified users more than 2 days old"
						runat="server" text="Delete All" /></td>
			</tr>
			</table>
		</FooterTemplate>
	</asp:repeater>
	<br />
	<TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
		<TR>
			<TD class="header1" colSpan="4">Statistics</TD>
		</TR>
		<TR>
			<TD class="postheader" width="25%">Number of posts:</TD>
			<TD class="post" width="25%">
				<asp:label id="NumPosts" runat="server"></asp:label></TD>
			<TD class="postheader" width="25%">Posts per day:</TD>
			<TD class="post" width="25%">
				<asp:label id="DayPosts" runat="server"></asp:label></TD>
		</TR>
		<TR>
			<TD class="postheader">Number of topics:</TD>
			<TD class="post">
				<asp:label id="NumTopics" runat="server"></asp:label></TD>
			<TD class="postheader">Topics per day:</TD>
			<TD class="post">
				<asp:label id="DayTopics" runat="server"></asp:label></TD>
		</TR>
		<TR>
			<TD class="postheader">Number of users:</TD>
			<TD class="post">
				<asp:label id="NumUsers" runat="server"></asp:label></TD>
			<TD class="postheader">Users per day:</TD>
			<TD class="post">
				<asp:label id="DayUsers" runat="server"></asp:label></TD>
		</TR>
		<TR>
			<TD class="postheader">Board started:</TD>
			<TD class="post">
				<asp:label id="BoardStart" runat="server"></asp:label></TD>
			<TD class="postheader">Size of database:</TD>
			<TD class="post">
				<asp:label id="DBSize" runat="server"></asp:label></TD>
		</TR>
		<TR>
			<TD class="postfooter" colSpan="4">These statistics don't count deleted topics and 
				posts.</TD>
		</TR>
	</TABLE>
	<P id="UpgradeNotice" runat="server" visible="false">The installed version of Yet 
		Another Forum.net and the version of your database does not match. You should 
		go to <A href="install/" target='_"top"'>install</A> and update your database.
	</P>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
