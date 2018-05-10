<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_subscriptions"CodeBehind="cp_subscriptions.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="PreferencesUpdatePanel" runat="server">
    <ContentTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td class="header1" colspan="2">
                    <YAF:LocalizedLabel ID="LocalizedLabel100" runat="server" LocalizedTag="TITLE" />
                </td>
            </tr>
            <tr>
                <td class="postheader">
                    <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" LocalizedTag="NOTIFICATIONSELECTION" />
                </td>
                <td class="post">
                    <asp:RadioButtonList ID="rblNotificationType" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="rblNotificationType_SelectionChanged">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr runat="server" id="DailyDigestRow">
                <td class="postheader">
                    <YAF:LocalizedLabel ID="LocalizedLabel199" runat="server" LocalizedTag="DAILY_DIGEST" />
                </td>
                <td class="post">
                    <asp:CheckBox ID="DailyDigestEnabled" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="PMNotificationRow">
                <td class="postheader">
                    <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedPage="CP_EDITPROFILE"
                        LocalizedTag="PM_EMAIL_NOTIFICATION" />
                </td>
                <td class="post">
                    <asp:CheckBox ID="PMNotificationEnabled" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="footer1" colspan="2" align="right">
                    <asp:Button ID="SaveUser" runat="server" OnClick="SaveUser_Click" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
<br />
<asp:UpdatePanel ID="SubscriptionsUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:PlaceHolder ID="SubscribeHolder" runat="server">
            <table class="content" cellspacing="1" cellpadding="0" width="100%">
                <tr>
                    <td class="header1" colspan="5">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="forums" />
                    </td>
                </tr>
                <tr>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="forum" />
                    </td>
                    <td class="header2" align="center">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="topics" />
                    </td>
                    <td class="header2" align="center">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="replies" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="lastpost" />
                    </td>
                    <td class="header2">
                        &nbsp;
                    </td>
                </tr>
                <asp:Repeater ID="ForumList" runat="server">
                    <ItemTemplate>
                        <asp:Label ID="tfid" runat="server" Text='<%# Container.DataItemToField<int>("WatchForumID") %>'
                            Visible="false" />
                        <tr>
                            <td class="post">
                                <a href="<%# YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}",  Container.DataItemToField<int>("ForumID"))%>">
                                    <%# HtmlEncode(Container.DataItemToField<string>("ForumName"))%></a>
                            </td>
                            <td class="post" align="center">
                                <%# Container.DataItemToField<int>("Topics")%>
                            </td>
                            <td class="post" align="center">
                                <%# FormatForumReplies(Container.DataItem) %>
                            </td>
                            <td class="post">
                                <%# FormatLastPosted(Container.DataItem) %>
                            </td>
                            <td class="post" align="center">
                                <asp:CheckBox ID="unsubf" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td class="footer1" colspan="5" align="right">
                        <asp:Button ID="UnsubscribeForums" runat="server" OnClick="UnsubscribeForums_Click" />
                    </td>
                </tr>
            </table>
            <br />
            <YAF:Pager ID="PagerTop" runat="server" PageSize="25" OnPageChange="PagerTop_PageChange"
                UsePostBack="True" />
            <table class="content" cellspacing="1" cellpadding="0" width="100%">
                <tr>
                    <td class="header1" colspan="5">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="topics" />
                    </td>
                </tr>
                <tr>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="topic" />
                    </td>
                    <td class="header2" align="center">
                        <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="replies" />
                    </td>
                    <td class="header2" align="center">
                        <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="views" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="lastpost" />
                    </td>
                    <td class="header2">
                        &nbsp;
                    </td>
                </tr>
                <asp:Repeater ID="TopicList" runat="server">
                    <ItemTemplate>
                        <asp:Label ID="ttid" runat="server" Text='<%# Container.DataItemToField<int>("WatchTopicID") %>'
                            Visible="false" />
                        <tr>
                            <td class="post">
                                <a href="<%# YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "t={0}", Container.DataItemToField<int>("TopicID"))%>">
                                    <%# HtmlEncode(Container.DataItemToField<string>("TopicName"))%></a>
                            </td>
                            <td class="post" align="center">
                                <%# Container.DataItemToField<int>("Replies")%>
                            </td>
                            <td class="post" align="center">
                                <%# Container.DataItemToField<int>("Views")%>
                            </td>
                            <td class="post">
                                <%# FormatLastPosted(Container.DataItem) %>
                            </td>
                            <td class="post" align="center">
                                <asp:CheckBox ID="unsubx" runat="server" />
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td class="footer1" colspan="5" align="right">
                        <asp:Button ID="UnsubscribeTopics" runat="server" OnClick="UnsubscribeTopics_Click" />
                    </td>
                </tr>
            </table>
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
        </asp:PlaceHolder>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
