<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost" CodeBehind="ForumLastPost.ascx.cs" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
        <asp:HyperLink ID="topicLink" CssClass="forumTopicLink" runat="server"></asp:HyperLink>
    </asp:PlaceHolder>
    &nbsp;<asp:HyperLink ID="LastTopicImgLink" runat="server">
        <YAF:ThemeImage ID="Icon" runat="server" />
    </asp:HyperLink>
    <asp:HyperLink ID="ImageLastUnreadMessageLink" runat="server">
            <YAF:ThemeImage ID="LastUnreadImage" runat="server" Style="border: 0" />
    </asp:HyperLink>
    <br />
    <YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" LocalizedPage="TOPICS" />
    <YAF:UserLink ID="ProfileUserLink" runat="server" />
    <br />
    <YAF:DisplayDateTime ID="LastPostDate" runat="server" Format="BothTopic" />
</asp:PlaceHolder>
<YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
