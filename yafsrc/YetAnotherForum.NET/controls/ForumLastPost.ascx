<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost"
    CodeBehind="ForumLastPost.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
        <asp:HyperLink ID="topicLink" ToolTip='<%# this.GetText("COMMON", "VIEW_TOPIC") %>' CssClass="forumTopicLink" runat="server"></asp:HyperLink>
        <br />
    </asp:PlaceHolder>
    <YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" />
    <YAF:UserLink ID="ProfileUserLink" runat="server" />
    &nbsp;<asp:HyperLink ID="LastTopicImgLink" runat="server">
        <YAF:ThemeImage ID="Icon" runat="server" />
    </asp:HyperLink>
      <asp:HyperLink ID="ImageLastUnreadMessageLink" runat="server">
                                 <YAF:ThemeImage ID="LastUnreadImage" runat="server" Style="border: 0" />
                                </asp:HyperLink>
    <br />
    <YAF:DisplayDateTime ID="LastPostDate" runat="server" Format="BothTopic" />
</asp:PlaceHolder>
<YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
