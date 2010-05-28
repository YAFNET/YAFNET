<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.ranks" Codebehind="ranks.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="6">
				Ranks</td>
		</tr>
		<asp:Repeater ID="RankList" OnItemCommand="RankList_ItemCommand" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header2">
						Name</td>
					<td class="header2">
						Is Start</td>
					<td class="header2">
						Is Ladder</td>
					<td class="header2">
						PM limit</td>	
					<td class="header2">
						Command</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# Eval( "Name") %>
					</td>
					<td class="post">
						<%# BitSet(Eval( "Flags"),1) %>
					</td>
					<td class="post">
						<%# LadderInfo(Container.DataItem) %>
					</td>
					<td class="post">
						<%# Eval( "PMLimit" ) %>
					</td>
					<td class="post">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "RankID") %>'>Edit</asp:LinkButton>						|
						<asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "RankID") %>'>Delete</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="6">
				<asp:LinkButton ID="NewRank" runat="server" Text="New Rank" OnClick="NewRank_Click" /></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
