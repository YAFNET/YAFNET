<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumActiveDiscussion"
    CodeBehind="ForumActiveDiscussion.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<asp:UpdatePanel ID="UpdateStatsPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table border="0" class="content activeDiscussionContent" cellspacing="1" cellpadding="0" width="100%">
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
                            <td class="post" style="padding-left:10px">
                                <asp:Image ID="NewPostIcon" runat="server" CssClass="topicStatusIcon" />
                                &nbsp;<strong><asp:HyperLink ID="TextMessageLink" runat="server" /></strong>
                                &nbsp;<YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" LocalizedPage="TOPICS" />
                                &nbsp;<YAF:UserLink ID="LastUserLink"  runat="server" />&nbsp;(<asp:HyperLink ID="ForumLink" runat="server" />)
                            </td>
                            <td class="post" style="width: 30em; text-align: right;">                            
                                <YAF:DisplayDateTime ID="LastPostDate" runat="server" Format="BothTopic" />
                                <asp:HyperLink ID="ImageMessageLink" runat="server">
                                    <YAF:ThemeImage ID="LastPostedImage" runat="server" Style="border: 0" />
                                </asp:HyperLink>
                                <asp:HyperLink ID="ImageLastUnreadMessageLink" runat="server">
                                 <YAF:ThemeImage ID="LastUnreadImage" runat="server"  Style="border: 0" />
                                </asp:HyperLink>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td class="footer1" align="right" colspan="2">
                        <YAF:RssFeedLink ID="RssFeed" runat="server" FeedType="LatestPosts"  TitleLocalizedTag="RSSICONTOOLTIPACTIVE" />&nbsp; 
                        <YAF:RssFeedLink ID="AtomFeed" runat="server" FeedType="LatestPosts" IsAtomFeed="true" ImageThemeTag="ATOMFEED" TextLocalizedTag="ATOMFEED" TitleLocalizedTag="ATOMICONTOOLTIPFORUM" />                           
                    </td>
                </tr>
            </asp:PlaceHolder>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
