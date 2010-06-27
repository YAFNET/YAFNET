<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost"
    CodeBehind="ForumLastPost.ascx.cs" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
        <asp:HyperLink ID="topicLink" CssClass="forumTopicLink" runat="server"></asp:HyperLink>
        <br />
    </asp:PlaceHolder>
    <YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" />
    <YAF:UserLink ID="ProfileUserLink" runat="server" />
    &nbsp;<asp:HyperLink ID="LastTopicImgLink" runat="server">
        <YAF:ThemeImage ID="Icon" runat="server" />
    </asp:HyperLink>
    <br />
    <asp:Label ID="LastPosted" runat="server" />
</asp:PlaceHolder>
<YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
