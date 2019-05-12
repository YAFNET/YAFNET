<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost" CodeBehind="ForumLastPost.ascx.cs" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <h6>
        <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
            <YAF:LocalizedLabel runat="server" LocalizedTag="LASTPOST"></YAF:LocalizedLabel>:
            <asp:HyperLink ID="topicLink" runat="server"></asp:HyperLink>
        </asp:PlaceHolder>
        <asp:Label runat="server" ID="NewMessage"></asp:Label>
        <br/>
        <YAF:ThemeButton runat="server" ID="LastTopicImgLink" 
                         Size="Small"
                         Icon="share-square"
                         Type="OutlineSecondary"
                         TextLocalizedTag="GO_LAST_POST"
                         CssClass="mt-1 mr-1"></YAF:ThemeButton>
        <YAF:ThemeButton runat="server" ID="ImageLastUnreadMessageLink" 
                         Size="Small"
                         Icon="book-reader"
                         Type="OutlineSecondary"
                         TextLocalizedTag="GO_LASTUNREAD_POST"
                         CssClass="mt-1"></YAF:ThemeButton>
    </h6>
    <hr/>
    <h6><YAF:UserLink ID="ProfileUserLink" runat="server" />

        &nbsp;<i class="fa fa-calendar-alt fa-fw"></i>&nbsp;
        <YAF:DisplayDateTime ID="LastPostDate" runat="server" Format="BothTopic" />
    </h6>
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="NoPostsPlaceHolder">
    <YAF:Alert runat="server" Type="info">
        <YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
    </YAF:Alert>
</asp:PlaceHolder>
