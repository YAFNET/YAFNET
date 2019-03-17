<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ForumLastPost" CodeBehind="ForumLastPost.ascx.cs" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
    <h6>
        <asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
            <YAF:LocalizedLabel runat="server" LocalizedTag="LASTPOST"></YAF:LocalizedLabel>:
            <asp:HyperLink ID="topicLink" runat="server"></asp:HyperLink>
        </asp:PlaceHolder>
        <asp:Label runat="server" ID="NewMessage"></asp:Label>
        <br/>
        <asp:HyperLink runat="server" ID="LastTopicImgLink"></asp:HyperLink>
        <asp:HyperLink runat="server" ID="ImageLastUnreadMessageLink"></asp:HyperLink>
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
