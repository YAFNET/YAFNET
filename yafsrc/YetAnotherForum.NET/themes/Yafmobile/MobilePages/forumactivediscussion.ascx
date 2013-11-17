<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="../../../controls/forumactivediscussion.ascx.cs"
    Inherits="YAF.Controls.ForumActiveDiscussion" %>
<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table border="0" class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td class="header1" colspan="2">
                    <YAF:CollapsibleImage ID="CollapsibleImage" runat="server" BorderWidth="0" Style="vertical-align: middle"
                        PanelID='ActiveDiscussions' AttachedControlID="ActiveDiscussionPlaceHolder" />&nbsp;&nbsp;<YAF:LocalizedLabel
                            ID="ActiveDiscussionHeader" runat="server" LocalizedTag="ACTIVE_DISCUSSIONS" />
                </td>
            </tr>
            <asp:PlaceHolder runat="server" ID="ActiveDiscussionPlaceHolder">
                <tr>
                    <td class="header2" colspan="2">
                        <YAF:LocalizedLabel ID="LatestPostsHeader" runat="server" LocalizedTag="LATEST_POSTS" />
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="LatestPosts" OnItemDataBound="LatestPosts_ItemDataBound">
                    <ItemTemplate>
                        <tr>
                            <td class="post">
                             <asp:Image ID="NewPostIcon" runat="server" style="border: 0;width:16px;height:16px" />
                                &nbsp;<strong><asp:HyperLink ID="TextMessageLink" runat="server" /></strong> &nbsp;<YAF:LocalizedLabel
                                    ID="ByLabel" runat="server" LocalizedTag="BY" />
                                &nbsp;<YAF:UserLink ID="LastUserLink" runat="server" />
                                &nbsp;(<asp:HyperLink ID="ForumLink" runat="server" />)
                            </td>
                            <asp:PlaceHolder ID="MobileNoShow" runat="server"  Visible="false">
                            <td class="post" style="width: 30em; text-align: right;">    
                             <YAF:DisplayDateTime ID="LastPostDate" runat="server" Format="BothTopic" />    
                              <asp:HyperLink ID="ImageMessageLink" runat="server">        
                               <YAF:ThemeImage ID="LastPostedImage" runat="server" LocalizedTitlePage="DEFAULT"
                                 LocalizedTitleTag="GO_LAST_POST" Style="border: 0" />    
                              </asp:HyperLink>     
                              <asp:HyperLink ID="ImageLastUnreadMessageLink" runat="server">      
                                  <YAF:ThemeImage ID="LastUnreadImage" runat="server" 
                                  LocalizedTitlePage="DEFAULT"           
                                   LocalizedTitleTag="GO_LASTUNREAD_POST" Style="border: 0" />    
                              </asp:HyperLink>  </td>
                            </asp:PlaceHolder>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td class="footer1" align="right" colspan="2">
                        <YAF:RssFeedLink ID="RssFeed" runat="server" FeedType="LatestPosts" TitleLocalizedTag="RSSICONTOOLTIPACTIVE" />
                        &nbsp;
                        <YAF:RssFeedLink ID="AtomFeed" runat="server" FeedType="LatestPosts" IsAtomFeed="true"
                            ImageThemeTag="ATOMFEED" TextLocalizedTag="ATOMFEED" TitleLocalizedTag="ATOMICONTOOLTIPFORUM" />
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
