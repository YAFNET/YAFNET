<%@ Control Language="c#" CodeFile="topics.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.topics" %>

<%@ Register TagPrefix="YAF" TagName="ForumList" Src="../controls/ForumList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:PlaceHolder runat="server" ID="SubForums" Visible="false">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr class="header1">
            <td colspan="6">
                <%=GetSubForumTitle()%>
            </td>
        </tr>
        <tr class="header2">
            <td width="1%">
                &nbsp;</td>
            <td align="left">
                <%# GetText("FORUM") %>
            </td>
            <td align="center" width="15%">
                <%# GetText("moderators") %>
            </td>
            <td align="center" width="4%">
                <%# GetText("topics") %>
            </td>
            <td align="center" width="4%">
                <%# GetText("posts") %>
            </td>
            <td align="center" width="25%">
                <%# GetText("lastpost") %>
            </td>
        </tr>
        <YAF:ForumList runat="server" ID="ForumList" />
    </table>
</asp:PlaceHolder>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td class="navlinks" align="left">
            <YAF:Pager runat="server" ID="Pager" UsePostBack="False" />
        </td>
        <td align="right">
            <asp:LinkButton ID="moderate1" runat="server" CssClass="imagelink" />
            <asp:LinkButton ID="NewTopic1" runat="server" CssClass="imagelink" />
        </td>
    </tr>
</table>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" colspan="6">
            <asp:Label ID="PageTitle" runat="server"></asp:Label></td>
    </tr>
    <tr>
        <td class="header2" width="1%">
            &nbsp;</td>
        <td class="header2" align="left">
            <%# GetText("topics") %>
        </td>
        <td class="header2" align="left" width="20%">
            <%# GetText("topic_starter") %>
        </td>
        <td class="header2" align="center" width="7%">
            <%# GetText("replies") %>
        </td>
        <td class="header2" align="center" width="7%">
            <%# GetText("views") %>
        </td>
        <td class="header2" align="center" width="25%">
            <%# GetText("lastpost") %>
        </td>
    </tr>
    <asp:Repeater ID="Announcements" runat="server">
        <ItemTemplate>
            <YAF:TopicLine runat="server" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater ID="TopicList" runat="server">
        <ItemTemplate>
            <YAF:TopicLine runat="server" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <YAF:TopicLine runat="server" IsAlt="True" DataRow="<%# Container.DataItem %>" />
        </AlternatingItemTemplate>
    </asp:Repeater>
    <YAF:Forumusers runat="server" />
    <tr>
        <td align="center" colspan="6" class="footer1">
            <table cellspacing="0" cellpadding="0" width="100%">
                <tr>
                    <td width="1%">
                        <nobr>
                            <%# GetText("showtopics") %>
                            <asp:DropDownList ID="ShowList" runat="server" AutoPostBack="True" />
                        </nobr>
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="WatchForum" runat="server" /><span id="WatchForumID" runat="server" visible="false" /> |
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
        <td align="left" class="navlinks">
            <YAF:Pager runat="server" LinkedPager="Pager" UsePostBack="False" />
        </td>
        <td align="right">
            <asp:LinkButton ID="moderate2" runat="server" CssClass="imagelink" />
            <asp:LinkButton ID="NewTopic2" runat="server" CssClass="imagelink" />
        </td>
    </tr>
</table>
<table width="100%" cellspacing="0" cellpadding="0">
    <tr id="ForumJumpLine" runat="Server">
        <td align="right" colspan="2">
            <%# GetText("Forum_Jump") %>
            <YAF:ForumJump runat="server" />
        </td>
    </tr>
    <tr>
        <td valign="top">
            <YAF:IconLegend runat="server" />
        </td>
        <td align="right">
            <table cellspacing="1" cellpadding="1">
                <tr>
                    <td align="right" valign="top" class="smallfont">
                        <YAF:PageAccess runat="server" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
