<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumStatistics" Codebehind="ForumStatistics.ascx.cs" %>

<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional" class="col">
    <ContentTemplate>
        <div class="card mb-3">
            <div class="card-header">
                <span class="fa-stack">
                    <i class="fas fa-chart-bar fa-2x fa-fw text-secondary"></i>
                </span>
                &nbsp;<YAF:LocalizedLabel ID="StatsHeader" runat="server" LocalizedTag="STATS" />
            </div>
            <div class="row">
                    <div class="col">
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">
                                <asp:Label ID="StatsPostsTopicCount" runat="server" />
                            </li>
                            <asp:PlaceHolder runat="server" ID="StatsLastPostHolder" Visible="False">
                                <li class="list-group-item">
                                    <asp:Label ID="StatsLastPost" runat="server" />&nbsp;<YAF:UserLink ID="LastPostUserLink" runat="server" />
                                </li>
                            </asp:PlaceHolder>
                            <li class="list-group-item">
                                <asp:Label ID="StatsMembersCount" runat="server" />
                            </li>
                            <li class="list-group-item">
                                <asp:Label ID="StatsNewestMember" runat="server" />:&nbsp;<YAF:UserLink ID="NewestMemberUserLink" runat="server" />
                            </li>
                        </ul>
                    </div>
                    <asp:PlaceHolder runat="server" ID="AntiSpamStatsHolder">
                        <div class="col-md-6">
                            <ul class="list-group list-group-flush">
                                <li class="list-group-item">
                                    <h6>
                                        <YAF:LocalizedLabel runat="server"
                                                            LocalizedTag="STATS_SPAM">
                                        </YAF:LocalizedLabel>
                                    </h6>
                                </li>
                                <li class="list-group-item">
                                    <YAF:LocalizedLabel runat="server" ID="StatsSpamDenied"
                                                        LocalizedTag="STATS_SPAM_DENIED"></YAF:LocalizedLabel>
                                </li>
                                <li class="list-group-item">
                                    <YAF:LocalizedLabel runat="server" ID="StatsSpamBanned"
                                                        LocalizedTag="STATS_SPAM_BANNED"></YAF:LocalizedLabel>
                                </li>
                                <li class="list-group-item">
                                    <YAF:LocalizedLabel runat="server" ID="StatsSpamReported"
                                                        LocalizedTag="STATS_SPAM_REPORTED"></YAF:LocalizedLabel>
                                </li>
                            </ul>
                        </div>
                    </asp:PlaceHolder>
                </div>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
