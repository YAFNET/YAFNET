<%@ Control Language="c#" Codebehind="topics.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.topics" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" TagName="ForumList" Src="../controls/ForumList.ascx" %>
<yaf:PageLinks runat="server" ID="PageLinks" />
<asp:PlaceHolder runat="server" ID="SubForums" Visible="false">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr class="header1">
            <td colspan="5">
                <%=GetSubForumTitle()%>
            </td>
        </tr>
        <tr class="header2">
            <td width="1%">
                &nbsp;</td>
            <td align="left">
                <%# GetText("FORUM") %>
            </td>
            <td align="center" width="7%">
                <%# GetText("topics") %>
            </td>
            <td align="center" width="7%">
                <%# GetText("posts") %>
            </td>
            <td align="center" width="25%">
                <%# GetText("lastpost") %>
            </td>
        </tr>
        <yaf:ForumList runat="server" ID="ForumList" />
    </table>
</asp:PlaceHolder>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td class="navlinks" align="left">
            <yaf:Pager runat="server" ID="Pager" />
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
        <td class="header2" align="middle" width="7%">
            <%# GetText("replies") %>
        </td>
        <td class="header2" align="middle" width="7%">
            <%# GetText("views") %>
        </td>
        <td class="header2" align="middle" width="25%">
            <%# GetText("lastpost") %>
        </td>
    </tr>
    <asp:Repeater ID="Announcements" runat="server">
        <ItemTemplate>
            <yaf:TopicLine runat="server" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater ID="TopicList" runat="server">
        <ItemTemplate>
            <yaf:TopicLine runat="server" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <yaf:TopicLine runat="server" IsAlt="True" DataRow="<%# Container.DataItem %>" />
        </AlternatingItemTemplate>
    </asp:Repeater>
    <yaf:ForumUsers runat="server" />
    <tr>
        <td align="middle" colspan="6" class="footer1">
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
            <yaf:Pager runat="server" LinkedPager="Pager" />
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
            <yaf:ForumJump runat="server" />
        </td>
    </tr>
    <tr>
        <td valign="top">
            <yaf:IconLegend runat="server" />
        </td>
        <td align="right">
            <table cellspacing="1" cellpadding="1">
                <tr>
                    <td align="right" valign="top" class="smallfont">
                        <yaf:PageAccess runat="server" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<yaf:SmartScroller ID="SmartScroller1" runat="server" />
