<%@ Control Language="c#" CodeFile="topics.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.topics" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="ForumList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="TopicLineMobile" Src="TopicLineMobile.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
<asp:PlaceHolder runat="server" ID="SubForums" Visible="false">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr class="header1">
            <td colspan="6" class="headersub">
                <%=GetSubForumTitle()%>
            </td>
        </tr>
        <YAF:ForumList runat="server" ID="ForumList" />
    </table>
</asp:PlaceHolder>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="Pager" UsePostBack="False" />
        </td>
        <td>
            <YAF:ThemeButton ID="NewTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" OnClick="NewTopic_Click" />
        </td>
    </tr>
</table>
<table class="content" width="100%">
    <tr class="topicTitle">
        <td class="header1" colspan="6">
            <asp:Label ID="PageTitle" runat="server"></asp:Label>
        </td>
    </tr>
    <tr class="topicSubTitle">
        <td class="header2" width="1%">
            &nbsp;
        </td>
        <td class="header2" align="left">
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" />
        </td>
    </tr>
    <asp:Repeater ID="Announcements" runat="server">
        <ItemTemplate>
            <YAF:TopicLineMobile runat="server" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater ID="TopicList" runat="server">
        <ItemTemplate>
            <YAF:TopicLineMobile runat="server" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <YAF:TopicLineMobile runat="server" IsAlt="True" DataRow="<%# Container.DataItem %>" />
        </AlternatingItemTemplate>
    </asp:Repeater>
    <YAF:ForumUsers runat="server" />
    <tr>
        <td align="center" colspan="6" class="footer1">
            <table cellspacing="0" cellpadding="0" width="100%">
                <tr>
                    <td width="1%" style="white-space: nowrap">
                        <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="showtopics" />
                        <asp:DropDownList ID="ShowList" runat="server" AutoPostBack="True" />
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="WatchForum" runat="server" /><span id="WatchForumID" runat="server"
                            visible="false" /> |
                        <asp:LinkButton runat="server" ID="MarkRead" />
                        <span id="RSSLinkSpacer" runat="server">|</span>
                        <asp:HyperLink ID="RssFeed" runat="server" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
    <tr>
        <td align="left">
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="False" />
        </td>
        <td>
            <YAF:ThemeButton ID="NewTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
                TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" OnClick="NewTopic_Click" />
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div id="DivForumJump">
        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
        &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
    </div>
</asp:PlaceHolder>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
