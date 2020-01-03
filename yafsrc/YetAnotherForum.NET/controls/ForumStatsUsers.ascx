<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumStatsUsers" Codebehind="ForumStatsUsers.ascx.cs" %>

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
                            <p class="card-text"><asp:Label runat="server" ID="ActiveUserCount" /></p>
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
    </ContentTemplate>
</asp:UpdatePanel>
