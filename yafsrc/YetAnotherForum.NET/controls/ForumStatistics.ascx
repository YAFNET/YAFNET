<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumStatistics.ascx.cs"
	Inherits="YAF.Controls.ForumStatistics" %>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="2">
			<asp:ImageButton runat="server" ID="expandInformation" BorderWidth="0" ImageAlign="Baseline"
				OnClick="expandInformation_Click" />&nbsp;&nbsp;<YAF:LocalizedLabel ID="InformationHeader"
					runat="server" LocalizedTag="INFORMATION" />
		</td>
	</tr>
	<asp:PlaceHolder runat="server" ID="InformationPlaceHolder">
		<tr>
			<td class="header2" colspan="2">
				<YAF:LocalizedLabel ID="ActiveUsersLabel" runat="server" LocalizedTag="ACTIVE_USERS" />
			</td>
		</tr>
		<tr>
			<td class="post" width="1%">
				<YAF:ThemeImage ID="ForumUsersImage" runat="server" ThemeTag="FORUM_USERS" />
			</td>
			<td class="post">
				<asp:Label runat="server" ID="ActiveUserCount" />
				<br />
				<asp:Label runat="server" ID="MostUsersCount" />
				<br />
				<asp:Repeater runat="server" ID="ActiveList">
					<ItemTemplate>
						<YAF:UserLink ID="ProfileLink" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>'
							UserName='<%# Eval("Name").ToString() %>' />
					</ItemTemplate>
					<SeparatorTemplate>
						,
					</SeparatorTemplate>
				</asp:Repeater>
			</td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
				<YAF:LocalizedLabel ID="StatsHeader" runat="server" LocalizedTag="STATS" />
			</td>
		</tr>
		<tr>
			<td class="post" width="1%">
				<YAF:ThemeImage ID="ForumStatsImage" runat="server" ThemeTag="FORUM_STATS" /></td>
			<td class="post">
				<asp:Label ID="StatsPostsTopicCount" runat="server" />
				<br />
				<asp:PlaceHolder runat="server" ID="StatsLastPostHolder" Visible="False">
					<asp:Label ID="StatsLastPost" runat="server" />&nbsp;<YAF:UserLink ID="LastPostUserLink"
						runat="server" />.
					<br />
				</asp:PlaceHolder>
				<asp:Label ID="StatsMembersCount" runat="server" />
				<br />
				<asp:Label ID="StatsNewestMember" runat="server" />&nbsp;<YAF:UserLink ID="NewestMemberUserLink"
					runat="server" />.
			</td>
		</tr>
	</asp:PlaceHolder>
</table>
