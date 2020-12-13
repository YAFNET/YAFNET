<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost" CodeBehind="ForumLastPost.ascx.cs" %>

<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
        <YAF:ThemeButton ID="topicLink" runat="server"
                         Icon="comment"
                         IconColor="text-secondary"
                         IconCssClass="far"
                         Type="Link"
                         CssClass="fw-bold p-0"
                         DataToggle="tooltip"
                         TitleLocalizedTag="VIEW_TOPIC"
                         TitleLocalizedPage="COMMON"
                         DataContent="tooltip" />
    </asp:PlaceHolder>
    <asp:Label runat="server" ID="NewMessage" 
               CssClass="me-1" />
    <YAF:ThemeButton runat="server" ID="Info"
                     Icon="info-circle"
                     IconColor="text-secondary"
                     Type="Link"
                     DataToggle="popover"
                     Size="Small"
                     CssClass="topic-link-popover">
    </YAF:ThemeButton>
</asp:PlaceHolder>

<asp:PlaceHolder runat="server" ID="NoPostsPlaceHolder">
    <span class="font-italic">
        <YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
    </span>
</asp:PlaceHolder>
