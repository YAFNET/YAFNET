<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumStatistics" Codebehind="ForumStatistics.ascx.cs" %>
<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="content forumStatisticsContent" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td class="header1" colspan="2">
                    <YAF:CollapsibleImage ID="CollapsibleImage" runat="server" BorderWidth="0" Style="vertical-align: middle"
                        PanelID='InformationPanel' AttachedControlID="InformationPlaceHolder" />&nbsp;&nbsp;<YAF:LocalizedLabel
                            ID="InformationHeader" runat="server" LocalizedTag="INFORMATION" />
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
                        <YAF:ActiveUsers ID="ActiveUsers1" runat="server">
						</YAF:ActiveUsers>
                    </td>
                </tr>
                <asp:PlaceHolder runat="server" ID="RecentUsersPlaceHolder" Visible="False" >
                <tr>
                    <td class="header2" colspan="2">
                        <YAF:LocalizedLabel ID="RecentUsersLabel" runat="server" LocalizedTag="RECENT_USERS" />
                    </td>
                </tr>
                <tr>
                    <td class="post" width="1%">
                        <YAF:ThemeImage ID="ThemeImage1" runat="server" ThemeTag="FORUM_USERS" />
                    </td>
                    <td class="post">
                        <asp:Label runat="server" ID="RecentUsersCount" />
                        <br />
                        <YAF:ActiveUsers ID="RecentUsers" runat="server" InstantId="RecentUsersOneDay" Visible="False">
						</YAF:ActiveUsers>
                    </td>
                </tr>
                </asp:PlaceHolder>
                <tr>
                    <td class="header2" colspan="2">
                        <YAF:LocalizedLabel ID="StatsHeader" runat="server" LocalizedTag="STATS" />
                    </td>
                </tr>
                <tr>
                    <td class="post" width="1%">
                        <YAF:ThemeImage ID="ForumStatsImage" runat="server" ThemeTag="FORUM_STATS" />
                    </td> 
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
                        <br />
                        <asp:PlaceHolder ID="BirthdayUsers" runat="server" Visible="false">
                          <asp:Label ID="StatsTodaysBirthdays" runat="server" />
                        </asp:PlaceHolder>
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
