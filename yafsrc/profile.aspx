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

<tr class="post">
	<td width="50%" valign="top">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2"><%= GetText("about") %></td>
				</tr>
				<tr>
					<td width="50%" class="postheader"><%= GetText("username") %></td>
					<td width="50%" class="post"><asp:Label id="Name" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("groups") %></td>
					<td class="post">
						<asp:repeater id="Groups" runat="server">
							<ItemTemplate><%# DataBinder.Eval(Container.DataItem,"Name") %></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
						</asp:repeater>
					</td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("rank") %></td>
					<td class="post"><asp:Label id="Rank" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("realname") %></td>
					<td class="post" runat="server" id="RealName"/>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("website") %></td>
					<td class="post">
						<asp:hyperlink runat="server" id="HomePage" target="_blank" /></td>
				</tr>
				<TR>
					<TD class="postheader"><%= GetText("weblog") %></TD>
					<TD class="post"><asp:hyperlink runat="server" id="Weblog" target="_blank" /></TD>
				</TR>
				<tr>
					<td class="postheader"><%= GetText("location") %></td>
					<td class="post"><asp:label id="Location" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("occupation") %></td>
					<td class="post" runat="server" id="Occupation"/>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("interests") %></td>
					<td class="post" runat="server" id="Interests"/>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("gender") %></td>
					<td class="post" runat="server" id="Gender"/>
				</tr>
			</table>
	</td>
	<td width="50%" valign="top">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2"><%= GetText("contact") %></td>
				</tr>
				<tr id="EmailRow" runat="server" visible="false">
					<td width="50%" class="postheader"><%= GetText("email") %></td>
					<td width="50%" class="post"><asp:label id="Email" runat="server" /></td>
				</tr>
				<tr>
					<td width="50%" class="postheader"><%= GetText("pm") %></td>
					<td width="50%" class="post"><a href='pmessage.aspx?u=<%= Request.QueryString["u"].ToString() %>'>PM</a></td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("msn") %></td>
					<td class="post" runat="server" id="MSN"/>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("yim") %></td>
					<td class="post" runat="server" id="YIM"/>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("aim") %></td>
					<td class="post" runat="server" id="AIM"/>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("icq") %></td>
					<td class="post" runat="server" id="ICQ"/>
				</tr>
			</table>
	</td>
</tr>
<tr class="post">
	<td width="50%" valign="top">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2"><%= GetText("statistics") %></td>
				</tr>
				<tr>
					<td width="50%" class="postheader"><%= GetText("joined") %></td>
					<td width="50%" class="post"><asp:Label id="Joined" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("lastvisit") %></td>
					<td class="post"><asp:label id="LastVisit" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader"><%= GetText("numposts") %></td>
					<td class="post" runat="server" id="Stats"/>
				</tr>
			</table>
	</td>
	<td width="50%" valign="top">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2"><%= GetText("avatar") %></td>
				</tr>
				<tr>
					<td class="post" colspan="2" align="center">
						<asp:Image id="Avatar" runat="server"/>
					</td>
				</tr>
			</table>
	</td>
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

<yaf:savescrollpos runat="server"/>
</form>
