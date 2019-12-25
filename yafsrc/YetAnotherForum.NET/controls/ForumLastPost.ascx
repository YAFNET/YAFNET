<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost" CodeBehind="ForumLastPost.ascx.cs" %>

<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <div class="row">
        <div class="col">
            <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
                <YAF:LocalizedLabel runat="server" LocalizedTag="LASTPOST"></YAF:LocalizedLabel>: <asp:HyperLink ID="topicLink" runat="server" CssClass="mr-1"></asp:HyperLink>
            </asp:PlaceHolder>
            <asp:Label runat="server" ID="NewMessage" 
                       CssClass="mr-1"></asp:Label>
        </div>
    </div>
    <div class="row">
        <div class="col">
            <YAF:ThemeButton runat="server" ID="Info"
                             Icon="info-circle"
                             Type="OutlineInfo"
                             DataToggle="popover"
                             Size="Small"
                             CssClass="mt-1 mr-1 topic-link-popover">
            </YAF:ThemeButton>
            <YAF:ThemeButton runat="server" ID="LastTopicImgLink" 
                             Size="Small"
                             Icon="share-square"
                             Type="OutlineSecondary"
                             DataToggle="tooltip"
                             TitleLocalizedTag="GO_LAST_POST"
                             TextLocalizedTag="GO_LAST_POST"
                             CssClass="mt-1 mr-1"></YAF:ThemeButton>
            <YAF:ThemeButton runat="server" ID="ImageLastUnreadMessageLink" 
                             Size="Small"
                             Icon="book-reader"
                             Type="OutlineSecondary"
                             DataToggle="tooltip"
                             TitleLocalizedTag="GO_LASTUNREAD_POST"
                             TextLocalizedTag="GO_LASTUNREAD_POST"
                             CssClass="mt-1"></YAF:ThemeButton>
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="NoPostsPlaceHolder">
    <YAF:Alert runat="server" Type="info">
        <YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
    </YAF:Alert>
</asp:PlaceHolder>
