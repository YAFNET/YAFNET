<%@ Control language="c#" Codebehind="profile.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.profile" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" align=center width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class="header1" colspan=2>
		<%= GetText("profile") %> <asp:Label id=UserName runat="server"/> 
	</td>
</tr>
				<tr class="post"><td colspan="2">
					<asp:hyperlink runat='server' id='Pm'/>
					<asp:hyperlink runat='server' id='Email'/>
					<asp:hyperlink runat='server' id='Home'/>
					<asp:hyperlink runat='server' id='Blog'/>
					<asp:hyperlink runat="server" id='Weblog'/>
					<asp:hyperlink runat='server' id='Msn'/>
					<asp:hyperlink runat='server' id='Yim'/>
					<asp:hyperlink runat='server' id='Aim'/>
					<asp:hyperlink runat='server' id='Icq'/>
				</td></tr>


<tr class="post">
	<td width="50%" valign="top" rowspan="2">
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
</tr>
<tr class="post">
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
<tr class="post"><td valign="top">
	<table width="100%" cellspacing="1" cellpadding="0">
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
	</table>
</td><td valign="top">
	<table width="100%" cellspacing="1" cellpadding="0">
	<tr class="header2">
		<td class="header2" colspan="2">Forum Access</td>
	</tr>
	<asp:literal runat="server" id="AccessMaskRow"/>
	</table>
</td></tr>
</asp:placeholder>

<tr class="post"><td colspan="2">
	<asp:repeater id="LastPosts" runat="server">
	<HeaderTemplate>
		<table width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class=header2 colSpan=2><%= GetText("last10") %></td>
		</tr>
	</HeaderTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
	<ItemTemplate>
		<tr class=postheader>
			<td class=small align=left colspan="2">
				<b><%= GetText("topic") %></b> <a href='<%# yaf.Forum.GetLink(yaf.Pages.posts,"t={0}",DataBinder.Eval(Container.DataItem,"TopicID")) %>'><%# DataBinder.Eval(Container.DataItem,"Subject") %></a><br/>
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

</td></tr>
</table>

<yaf:savescrollpos runat="server"/>
