<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.MyTopicsList" Codebehind="MyTopicsList.ascx.cs" %>
<table class="command" cellspacing="0" cellpadding="0" width="100%" style="padding-bottom: 10px;">
    <tr>
        <td align="right">
            <YAF:LocalizedLabel ID="SinceLabel" runat="server" LocalizedTag="SINCE" />
            <asp:DropDownList ID="Since" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Since_SelectedIndexChanged" />
        </td>
    </tr>
</table>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" width="1%">
            &nbsp;
        </td>
        <td class="header1" align="left">
            <YAF:LocalizedLabel ID="TopicsLabel" runat="server" LocalizedTag="TOPICS" />
        </td>
        <td class="header1" align="left" width="20%">
            <YAF:LocalizedLabel ID="StarterLabel" runat="server" LocalizedTag="TOPIC_STARTER" />
        </td>
        <td class="header1" align="center" width="7%">
            <YAF:LocalizedLabel ID="RepliesLabel" runat="server" LocalizedTag="REPLIES" />
        </td>
        <td class="header1" align="center" width="7%">
            <YAF:LocalizedLabel ID="ViewsLabel" runat="server" LocalizedTag="VIEWS" />
        </td>
        <td class="header1" align="center" width="20%">
            <YAF:LocalizedLabel ID="LastPostLabel" runat="server" LocalizedTag="LASTPOST" />
        </td>
    </tr>
    <asp:Repeater ID="TopicList" runat="server">
        <ItemTemplate>
            <%# PrintForumName((System.Data.DataRowView)Container.DataItem) %>
            <YAF:TopicLine ID="TopicLine1" runat="server" AltLastPost="<%# this.lastPostImageTT %>" FindUnread="true" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
    </asp:Repeater>
    <tr>
        <td class="footer1" align="right" width="100%" colspan="6">
            <YAF:RssFeedLink ID="RssFeed" runat="server" Visible="<%# PageContext.BoardSettings.ShowRSSLink %>" />            
        </td>
    </tr>
</table>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
