<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.topics" Codebehind="topics.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="../controls/ForumList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="TopicLine" Src="../controls/TopicLine.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
<asp:PlaceHolder runat="server" ID="SubForums" Visible="false">
    <table class="content subForum" width="100%">
        <tr class="topicTitle">
            <th colspan="6" class="header1">
                <%=GetSubForumTitle()%>
            </th>
        </tr>
        <tr class="topicSubTitle">
            <th width="1%" class="header2">
                &nbsp;
            </th>
            <th class="header2 headerForum">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FORUM" />
            </th>
            <th width="15%" runat="server" class="header2 headerModerators" visible="<%# PageContext.BoardSettings.ShowModeratorList && PageContext.BoardSettings.ShowModeratorListAsColumn %>">
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="moderators" />
            </th>
            <th width="4%" class="header2 headerTopics">
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="topics" />
            </th>
            <th width="4%" class="header2 headerPosts">
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="posts" />
            </th>
            <th width="25%" class="header2 headerLastPost">
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="lastpost" />
            </th>
        </tr>
        <YAF:ForumList AltLastPost="<%# this.LastPostImageTT %>" runat="server" ID="ForumList" />
    </table>
</asp:PlaceHolder>
<table class="command" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="Pager" UsePostBack="False" />
        </td>
        <td>
            <YAF:ThemeButton ID="moderate1" runat="server" CssClass="yafcssbigbutton rightItem button-moderate"
                TextLocalizedTag="BUTTON_MODERATE" TitleLocalizedTag="BUTTON_MODERATE_TT" />
            <YAF:ThemeButton ID="NewTopic1" runat="server" CssClass="yafcssbigbutton rightItem button-newtopic"
                TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" OnClick="NewTopic_Click" />
        </td>
    </tr>
</table>
<table class="content" width="100%">
    <tr class="topicTitle">
        <th class="header1" colspan="6">
            <asp:Label ID="PageTitle" runat="server"></asp:Label>
        </th>
    </tr>
    <tr class="topicSubTitle">
        <th class="header2" width="1%">
            &nbsp;
        </th>
        <th class="header2 headerTopic" align="left">
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" />
        </th>
        <th class="header2 headerReplies" align="right" width="7%">
            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="replies" />
        </th>
        <th class="header2 headerViews" align="right" width="7%">
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="views" />
        </th>
        <th class="header2 headerLastPost" align="left" width="15%">
            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="lastpost" />
        </th>
    </tr>
    <asp:Repeater ID="Announcements" runat="server">
        <ItemTemplate>
            <YAF:TopicLine runat="server" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater ID="TopicList" runat="server">
        <ItemTemplate>
            <YAF:TopicLine runat="server" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" />
        </ItemTemplate>
        <AlternatingItemTemplate>
            <YAF:TopicLine runat="server" IsAlt="True" AltLastPost="<%# this.LastPostImageTT %>" DataRow="<%# Container.DataItem %>" />
        </AlternatingItemTemplate>
    </asp:Repeater>
    <YAF:ForumUsers runat="server" />
    <tr>
        <td align="center" colspan="6" class="footer1">
            <table cellspacing="0" cellpadding="0" width="100%">
                <tr>
                    <td width="1%" style="white-space: nowrap">
                        <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="showtopics" />
                        <asp:DropDownList ID="ShowList" runat="server" AutoPostBack="True" CssClass="standardSelectMenu" />
                    </td>
                    <td align="right">
                        <asp:LinkButton ID="WatchForum" runat="server" /><span id="WatchForumID" runat="server"
                            visible="false" /><span id="delimiter1" runat="server" visible="<%# this.WatchForum.Text.Length > 0 %>"> | </span>
                        <asp:LinkButton runat="server" ID="MarkRead" />
                        <YAF:RssFeedLink ID="RssFeed" runat="server" FeedType="Topics" ShowSpacerBefore="true"
                            Visible="<%# PageContext.BoardSettings.ShowRSSLink && this.Get<IPermissions>().Check(PageContext.BoardSettings.TopicsFeedAccess) %>" TitleLocalizedTag="RSSICONTOOLTIPFORUM" />  
                          <YAF:RssFeedLink ID="AtomFeed" runat="server" FeedType="Topics" ShowSpacerBefore="true" IsAtomFeed="true" Visible="<%# PageContext.BoardSettings.ShowAtomLink && this.Get<IPermissions>().Check(PageContext.BoardSettings.TopicsFeedAccess) %>" ImageThemeTag="ATOMFEED" TextLocalizedTag="ATOMFEED" TitleLocalizedTag="ATOMICONTOOLTIPACTIVE" />                            
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
            <YAF:ThemeButton ID="moderate2" runat="server" CssClass="yafcssbigbutton rightItem button-moderate"
                TextLocalizedTag="BUTTON_MODERATE" TitleLocalizedTag="BUTTON_MODERATE_TT" />
            <YAF:ThemeButton ID="NewTopic2" runat="server" CssClass="yafcssbigbutton rightItem button-newtopic"
                TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" OnClick="NewTopic_Click" />
        </td>
    </tr>
</table>
<asp:PlaceHolder ID="ForumSearchHolder" runat="server">
<div id="ForumSearchDiv">
        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SEARCH_FORUM" />
        &nbsp;<asp:TextBox id="forumSearch" runat="server"></asp:TextBox>
        &nbsp;<YAF:ThemeButton ID="forumSearchOK" runat="server" CssClass="yaflittlebutton"
                TextLocalizedTag="OK" TitleLocalizedTag="OK_TT" OnClick="ForumSearch_Click" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
    <div id="DivForumJump">
        <YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
        &nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
    </div>
</asp:PlaceHolder>
<div class="clearItem"></div>
<div id="DivIconLegend">
    <YAF:IconLegend ID="IconLegend1" runat="server" />
</div>
<div id="DivPageAccess" class="smallfont">
    <YAF:PageAccess ID="PageAccess1" runat="server" />
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
