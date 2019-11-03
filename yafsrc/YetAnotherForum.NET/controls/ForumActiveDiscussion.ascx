<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumActiveDiscussion"
    CodeBehind="ForumActiveDiscussion.ascx.cs" %>

<div class="col">
<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:PlaceHolder runat="server" ID="ActiveDiscussionPlaceHolder">
                        <div class="card mb-3">
                            <div class="card-header">
                                <span class="fa-stack">
                                    <i class="fas fa-comments fa-2x fa-fw text-secondary mr-1"></i>
                                </span>
                                <YAF:LocalizedLabel runat="server" ID="ActiveDiscussionHeader"
                                                    LocalizedTag="ACTIVE_DISCUSSIONS" />
                            </div>
                            <div class="card-body">
                                <asp:Repeater runat="server" ID="LatestPosts" 
                                              OnItemDataBound="LatestPosts_ItemDataBound">
                                    <HeaderTemplate>
                                        <ul class="list-group list-group-flush">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li class="list-group-item px-0">
                                            <h6>
                                                    <asp:PlaceHolder runat="server" ID="PostIcon"></asp:PlaceHolder>
                                                    <asp:HyperLink ID="TextMessageLink" runat="server" CssClass="font-weight-bold" />&nbsp;
                                                    (<asp:HyperLink ID="ForumLink" runat="server" />)&nbsp;
                                                <YAF:ThemeButton runat="server" 
                                                                     ID="GoToLastPost" 
                                                                     Size="Small"
                                                                     Icon="share-square"
                                                                     Type="OutlineSecondary"
                                                                     TextLocalizedTag="GO_LAST_POST"
                                                                     TitleLocalizedTag="GO_LAST_POST"></YAF:ThemeButton>
                                                    <YAF:ThemeButton runat="server" 
                                                                     ID="GoToLastUnread" 
                                                                     Size="Small"
                                                                     Icon="book-reader"
                                                                     Type="OutlineSecondary"
                                                                     TextLocalizedTag="GO_LASTUNREAD_POST"
                                                                     TitleLocalizedTag="GO_LASTUNREAD_POST"></YAF:ThemeButton>
                                            </h6>
                                            <small>
                                                <YAF:LocalizedLabel ID="ByLabel" runat="server" 
                                                                       LocalizedTag="BY" 
                                                                       LocalizedPage="TOPICS" />
                                                <YAF:UserLink ID="LastUserLink"  runat="server" />
                                                <span class="fa-stack">
                                                    <i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>
                                                    <i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i>
                                                    <i class="fa fa-clock fa-badge text-secondary"></i>
                                                </span>
                                                <YAF:DisplayDateTime ID="LastPostDate" runat="server"
                                                                     Format="BothTopic" />
                                            </small> 
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>

                            <asp:Panel runat="server" ID="Footer" CssClass="card-footer" >
                                <div class="btn-group float-right" role="group" aria-label="Tools">
                                    <YAF:RssFeedLink ID="RssFeed" runat="server" FeedType="LatestPosts" />
                                </div>
                            </asp:Panel>
                        </div>
            </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>
</div>