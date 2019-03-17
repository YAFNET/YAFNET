<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumStatistics" Codebehind="ForumStatistics.ascx.cs" %>


<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="row">
        <asp:PlaceHolder runat="server" ID="InformationPlaceHolder">
                <div class="col">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="ActiveUsersLabel" runat="server" LocalizedTag="ACTIVE_USERS" />
                        </div>
                        <div class="card-body">
                            <asp:Label runat="server" ID="ActiveUserCount" />
                            <YAF:ActiveUsers ID="ActiveUsers1" runat="server">
                            </YAF:ActiveUsers>
                        </div>
                        <div class="card-footer">
                            <small class="text-muted">
                                <asp:Label runat="server" ID="MostUsersCount" />
                            </small>
                        </div>
                    </div>
                </div>

            <asp:PlaceHolder runat="server" ID="RecentUsersPlaceHolder" Visible="True" >
                <div class="col">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-users fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="RecentUsersLabel" runat="server" LocalizedTag="RECENT_USERS" />
                        </div>
                        <div class="card-body">
                            <YAF:ActiveUsers ID="RecentUsers" runat="server" InstantId="RecentUsersOneDay" Visible="False">
                            </YAF:ActiveUsers>
                        </div>
                        <div class="card-footer">
                            <small class="text-muted">
                                <asp:Label runat="server" ID="RecentUsersCount" />
                            </small>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>

            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-chart-bar fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="StatsHeader" runat="server" LocalizedTag="STATS" />
                    </div>
                    <div class="card-body">
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">
                                 <asp:Label ID="StatsPostsTopicCount" runat="server" />
                            </li>
                            <asp:PlaceHolder runat="server" ID="StatsLastPostHolder" Visible="False">
                            <li class="list-group-item">
                                <asp:Label ID="StatsLastPost" runat="server" />&nbsp;<YAF:UserLink ID="LastPostUserLink" runat="server" />.
                            </li>
                            </asp:PlaceHolder>
                            <li class="list-group-item">
                                <asp:Label ID="StatsMembersCount" runat="server" />
                            </li>
                            <li class="list-group-item">
                                <asp:Label ID="StatsNewestMember" runat="server" />&nbsp;<YAF:UserLink ID="NewestMemberUserLink"
                                                                                                                               runat="server" />.
                            </li>
                            <asp:PlaceHolder ID="BirthdayUsers" runat="server" Visible="false">
                                <li class="list-group-item">
                                    <asp:Label ID="StatsTodaysBirthdays" runat="server" />
                                </li>
                           </asp:PlaceHolder>
                        </ul>
                       
                    </div>
                </div>
            </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
