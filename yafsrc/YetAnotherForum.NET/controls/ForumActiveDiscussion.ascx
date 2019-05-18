<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumActiveDiscussion"
    CodeBehind="ForumActiveDiscussion.ascx.cs" %>
<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:PlaceHolder runat="server" ID="ActiveDiscussionPlaceHolder">
                        <div class="card mb-3">
                            <div class="card-header">
                                <i class="fa fa-comments fa-fw"></i>&nbsp;<YAF:LocalizedLabel
                                                                              ID="ActiveDiscussionHeader" runat="server" 
                                                                              LocalizedTag="ACTIVE_DISCUSSIONS" />
                            </div>
                            <div class="card-body">
                                <asp:Repeater runat="server" ID="LatestPosts" 
                                              OnItemDataBound="LatestPosts_ItemDataBound">
                                    <HeaderTemplate>
                                        <ul class="list-group list-group-flush">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li class="list-group-item">
                                                <h5>
                                                    <asp:PlaceHolder runat="server" ID="PostIcon"></asp:PlaceHolder>
                                                    <asp:Label runat="server" ID="NewMessage" 
                                                               Visible="False"
                                                               CssClass="badge badge-success">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                            LocalizedTag="NEW_POSTS" />
                                                    </asp:Label>
                                                    &nbsp;<asp:HyperLink ID="TextMessageLink" runat="server" CssClass="font-weight-bold" />
                                                    (<asp:HyperLink ID="ForumLink" runat="server" />)
                                                    <YAF:ThemeButton runat="server" 
                                                                     ID="GoToLastPost" 
                                                                     Size="Small"
                                                                     Icon="share-square"
                                                                     Type="OutlineSecondary"
                                                                     CssClass="mt-1"
                                                                     TextLocalizedTag="GO_LAST_POST"></YAF:ThemeButton>
                                                    <YAF:ThemeButton runat="server" 
                                                                     ID="GoToLastUnread" 
                                                                     Size="Small"
                                                                     Icon="book-reader"
                                                                     Type="OutlineSecondary"
                                                                     CssClass="mt-1"
                                                                     TextLocalizedTag="GO_LASTUNREAD_POST"></YAF:ThemeButton>
                                                </h5>
                                                <small>
                                                    <YAF:LocalizedLabel ID="ByLabel" runat="server" 
                                                                        LocalizedTag="BY" 
                                                                        LocalizedPage="TOPICS" />
                                                    &nbsp;<YAF:UserLink ID="LastUserLink"  runat="server" />&nbsp;
                                                    <i class="fa fa-calendar fa-fw"></i>&nbsp;<YAF:DisplayDateTime ID="LastPostDate" runat="server" 
                                                                                                                   Format="BothTopic" />
                                                </small>
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                            <div class="card-footer">
                                <div class="btn-group float-right" role="group" aria-label="Tools">
                                    <YAF:RssFeedLink ID="RssFeed" runat="server" FeedType="LatestPosts" />
                                </div>
                            </div>
                        </div>
            </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>
