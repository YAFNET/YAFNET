<%@ Control Language="c#" CodeFile="profile.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.profile" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendUser" Src="../controls/EditUsersSuspend.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" align="center" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			<%= GetText("profile") %>
			<asp:Label ID="UserName" runat="server" />
		</td>
	</tr>
	<tr class="post">
		<td colspan="2">
			<asp:HyperLink runat="server" ID="Pm" />
			<asp:HyperLink runat="server" ID="Email" />
			<asp:HyperLink runat="server" ID="Home" />
			<asp:HyperLink runat="server" ID="Blog" />
			<asp:HyperLink runat="server" ID="Msn" />
			<asp:HyperLink runat="server" ID="Yim" />
			<asp:HyperLink runat="server" ID="Aim" />
			<asp:HyperLink runat="server" ID="Icq" />
		</td>
	</tr>
	<tr class="post">
		<td width="50%" valign="top" rowspan="2">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2">
						<%= GetText("about") %>
					</td>
				</tr>
				<tr>
					<td width="50%" class="postheader">
						<b>
							<%= GetText("username") %>
						</b>
					</td>
					<td width="50%" class="post">
						<asp:Label ID="Name" runat="server" /></td>
				</tr>
				<tr runat="server" id="userGroupsRow">
					<td class="postheader">
						<%= GetText("groups") %>
					</td>
					<td class="post">
						<asp:Repeater ID="Groups" runat="server">
							<ItemTemplate>
								<%# Container.DataItem %>
							</ItemTemplate>
							<SeparatorTemplate>
								,
							</SeparatorTemplate>
						</asp:Repeater>
					</td>
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("rank") %>
					</td>
					<td class="post">
						<asp:Label ID="Rank" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("realname") %>
					</td>
					<td class="post" runat="server" id="RealName" />
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("location") %>
					</td>
					<td class="post">
						<asp:Label ID="Location" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("occupation") %>
					</td>
					<td class="post" runat="server" id="Occupation" />
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("interests") %>
					</td>
					<td class="post" runat="server" id="Interests" />
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("gender") %>
					</td>
					<td class="post" runat="server" id="Gender" />
				</tr>
			</table>
		</td>
		<td width="50%" valign="top">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2">
						<%= GetText("statistics") %>
					</td>
				</tr>
				<tr>
					<td width="50%" class="postheader">
						<%= GetText("joined") %>
					</td>
					<td width="50%" class="post">
						<asp:Label ID="Joined" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("lastvisit") %>
					</td>
					<td class="post">
						<asp:Label ID="LastVisit" runat="server" /></td>
				</tr>
				<tr>
					<td class="postheader">
						<%= GetText("numposts") %>
					</td>
					<td class="post" runat="server" id="Stats" />
				</tr>
			</table>
		</td>
	</tr>
	<tr class="post">
		<td width="50%" valign="top">
			<table align="center" width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2">
						<%= GetText("avatar") %>
					</td>
				</tr>
				<tr>
					<td class="post" colspan="2" align="center">
						<asp:Image ID="Avatar" runat="server" CssClass="avatarimage" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<asp:PlaceHolder runat="server" ID="ModeratorInfo" Visible="false">
		<tr class="post">
			<td valign="top">
				<uc1:SuspendUser runat="server" ID="SuspendUserControl" />
				<uc1:SignatureEdit runat="server" ID="SignatureEditControl" />
				<div id="AdminUser" runat="server">
					<a href='<%# YafBuildLink.GetLink(ForumPages.admin_edituser,"u={0}", Request.QueryString.Get("u")) %>'>
						Administer User</a>
				</div>
			</td>
			<td valign="top">
				<table width="100%" cellspacing="1" cellpadding="0">
					<tr class="header2">
						<td class="header2" colspan="2">
							Forum Access</td>
					</tr>
					<asp:Literal runat="server" ID="AccessMaskRow" />
				</table>
			</td>
		</tr>
	</asp:PlaceHolder>
	<tr class="post">
		<td colspan="2">
			<table width="100%" cellspacing="1" cellpadding="0">
				<tr>
					<td class="header2" colspan="2">
						<asp:ImageButton runat="server" ID="expandLast10" BorderWidth="0" ImageAlign="Baseline"
							OnClick="expandLast10_Click" />&nbsp;&nbsp;<%= GetText("last10") %></td>
				</tr>
				<asp:Repeater ID="LastPosts" runat="server">
					<ItemTemplate>
						<tr class="postheader">
							<td class="small" align="left" colspan="2">
								<b>
									<%= GetText("topic") %>
								</b><a href='<%# YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"t={0}",DataBinder.Eval(Container.DataItem,"TopicID")) %>'>
									<%# General.BadWordReplace(Convert.ToString(DataBinder.Eval(Container.DataItem,"Subject"))) %>
								</a>
								<br />
								<b>
									<%= GetText("posted") %>
								</b>
								<%# YafDateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
							</td>
						</tr>
						<tr class="post">
							<td valign="top" class="message" colspan="2">
								<%# FormatBody(Container.DataItem) %>
							</td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</table>
		</td>
	</tr>
</table>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
