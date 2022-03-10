<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ActiveDiscussion"
    CodeBehind="ActiveDiscussion.ascx.cs" %>

<asp:PlaceHolder runat="server" ID="ActiveDiscussionPlaceHolder">
    <div class="card mb-3">
        <div class="card-header d-flex align-items-center">
            <YAF:IconHeader runat="server"
                            IconName="comments"
                            IconSize="fa-2x"
                            LocalizedTag="ACTIVE_DISCUSSIONS" />
            </div>
        <asp:Repeater runat="server" ID="LatestPosts"
                      OnItemDataBound="LatestPosts_ItemDataBound">
            <HeaderTemplate>
                <ul class="list-group list-group-flush">
                </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item pt-2 list-group-item-action">
                    <YAF:ThemeButton runat="server"
                                     ID="TextMessageLink"
                                     Icon="comment"
                                     IconCssClass="far"
                                     IconColor="text-secondary"
                                     Type="Link"
                                     CssClass="fw-bold p-0 d-inline"
                                     DataToggle="tooltip" />
                    <asp:Label runat="server" ID="PostIcon" Visible="False" />

                    <YAF:ThemeButton runat="server"
                                     ID="ForumLink"
                                     Type="Link"
                                     CssClass="p-0 d-inline" />
                    <YAF:ThemeButton runat="server" ID="Info"
                                     Icon="info-circle"
                                     IconColor="text-secondary"
                                     Type="Link"
                                     DataToggle="popover"
                                     CssClass="topic-link-popover p-0 d-inline"/>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
        <asp:Panel runat="server" ID="Footer"
                   CssClass="card-footer">
            <div class="btn-group float-end" role="group" aria-label="Tools">
                <YAF:RssFeedLink ID="RssFeed" runat="server" />
            </div>
        </asp:Panel>
    </div>
</asp:PlaceHolder>