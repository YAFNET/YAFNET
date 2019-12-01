<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumStatistics" Codebehind="ForumStatistics.ascx.cs" %>


<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional" class="col">
    <ContentTemplate>
        <asp:PlaceHolder runat="server" ID="InformationPlaceHolder">
                    <div class="row">
                        <div class="col">
                            <div class="card mb-3">
                                <div class="card-header d-flex align-items-center">
                                    <span class="fa-stack">
                                        <i class="fas fa-users fa-2x fa-fw text-secondary"></i>
                                    </span>
                                    &nbsp;<YAF:LocalizedLabel ID="ActiveUsersLabel" runat="server" LocalizedTag="ACTIVE_USERS" />
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
                    </div>
                    <asp:PlaceHolder runat="server" ID="RecentUsersPlaceHolder" Visible="True">
                        <div class="row">
                            <div class="col">
                                <div class="card mb-3">
                                    <div class="card-header d-flex align-items-center">
                                        <span class="fa-stack">
                                            <i class="fas fa-users fa-2x fa-fw text-secondary"></i>
                                        </span>
                                        &nbsp;<YAF:LocalizedLabel ID="RecentUsersLabel" runat="server" LocalizedTag="RECENT_USERS" />
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
                        </div>
                    </asp:PlaceHolder>
                    <div class="row">
                        <div class="col">
                            <YAF:MostActiveUsers ID="MostActiveList" runat="server" DisplayNumber="10" />
                        </div>
                    </div>
            </asp:PlaceHolder>
        
                <div class="card mb-3">
                    <div class="card-header d-flex align-items-center">
                        <span class="fa-stack">
                            <i class="fas fa-chart-bar fa-2x fa-fw text-secondary"></i>
                        </span>
                        &nbsp;<YAF:LocalizedLabel ID="StatsHeader" runat="server" LocalizedTag="STATS" />
                    </div>
                    <div class="card-body">
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item px-0">
                                <asp:Label ID="StatsPostsTopicCount" runat="server" />
                            </li>
                            <asp:PlaceHolder runat="server" ID="StatsLastPostHolder" Visible="False">
                                <li class="list-group-item px-0">
                                    <asp:Label ID="StatsLastPost" runat="server" />&nbsp;<YAF:UserLink ID="LastPostUserLink" runat="server" />
                                </li>
                            </asp:PlaceHolder>
                            <li class="list-group-item px-0">
                                <asp:Label ID="StatsMembersCount" runat="server" />
                            </li>
                            <li class="list-group-item px-0">
                                <asp:Label ID="StatsNewestMember" runat="server" />:&nbsp;<YAF:UserLink ID="NewestMemberUserLink" runat="server" />
                            </li>
                            <asp:PlaceHolder runat="server" ID="AntiSpamStatsHolder">
                            <li class="list-group-item px-0">
                                <h6>
                                    <YAF:LocalizedLabel runat="server"
                                                        LocalizedTag="STATS_SPAM">
                                    </YAF:LocalizedLabel>
                                </h6>
                            </li>
                                <li class="list-group-item px-0">
                                    <YAF:LocalizedLabel runat="server" ID="StatsSpamDenied"
                                                        LocalizedTag="STATS_SPAM_DENIED"></YAF:LocalizedLabel>
                                </li>
                            <li class="list-group-item px-0">
                                <YAF:LocalizedLabel runat="server" ID="StatsSpamBanned"
                                                    LocalizedTag="STATS_SPAM_BANNED"></YAF:LocalizedLabel>
                            </li>
                            <li class="list-group-item px-0">
                                <YAF:LocalizedLabel runat="server" ID="StatsSpamReported"
                                                    LocalizedTag="STATS_SPAM_REPORTED"></YAF:LocalizedLabel>
                            </li>
                            </asp:PlaceHolder>
                        </ul>
                    </div>
                </div>
    </ContentTemplate>
</asp:UpdatePanel>
