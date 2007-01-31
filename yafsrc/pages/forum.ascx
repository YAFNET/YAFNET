<%@ Control Language="c#" Codebehind="forum.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.forum" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="../controls/ForumList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div id="Welcome" runat="server">
    <div id="divTimeNow">
        <asp:Label ID="TimeNow" runat="server" /></div>
    <div id="divTimeLastVisit">
        <asp:Label ID="TimeLastVisit" runat="server" /></div>
    <div id="divUnreadMsgs">
        <asp:HyperLink runat="server" ID="UnreadMsgs" Visible="false" /></div>
</div>
<br />
<asp:Repeater ID="CategoryList" runat="server" OnItemCommand="categoryList_ItemCommand" OnItemDataBound="CategoryList_ItemDataBound">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
            <tr>
                <td class="header1" width="1%">
                    &nbsp;</td>
                <td class="header1" align="left">
                    <%# GetText("FORUM") %>
                </td>
                <td class="header1" align="center" width="7%">
                    <%# GetText("topics") %>
                </td>
                <td class="header1" align="center" width="7%">
                    <%# GetText("posts") %>
                </td>
                <td class="header1" align="center" width="25%">
                    <%# GetText("lastpost") %>
                </td>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td class="header2" colspan="5">
                <asp:ImageButton runat="server" ID="expandCategory" BorderWidth="0" ImageAlign="Baseline" CommandName="panel" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CategoryID") %>'>
                </asp:ImageButton>
                &nbsp;&nbsp; <a href='<%# YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.forum,"c={0}",DataBinder.Eval(Container.DataItem, "CategoryID")) %>'>
                    <%# DataBinder.Eval(Container.DataItem, "Name") %>
                </a>
            </td>
        </tr>
        <YAF:ForumList runat="server" Visible="true" ID="forumList" DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>' />
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
<br />
<table border="0" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td width="65%" valign="top">
            <table class="content" cellspacing="1" cellpadding="0" width="100%">
                <tr>
                    <td class="header1" colspan="2">
                        <asp:ImageButton runat="server" ID="expandInformation" BorderWidth="0" ImageAlign="Baseline" OnClick="expandInformation_Click" />&nbsp;&nbsp;<%= GetText("INFORMATION") %></td>
                </tr>
                <tbody id="InformationTBody" runat="server">
                    <tr>
                        <td class="header2" colspan="2">
                            <%= GetText("ACTIVE_USERS") %>
                        </td>
                    </tr>
                    <tr>
                        <td class="post" width="1%">
                            <img src="<%# GetThemeContents("ICONS","FORUM_USERS") %>" alt="" /></td>
                        <td class="post">
                            <asp:Label runat="server" ID="activeinfo" /><br />
                            <asp:Repeater runat="server" ID="ActiveList">
                                <ItemTemplate>
                                    <a href='<%#YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID"))%>'>
                                        <%# Server.HtmlEncode(Convert.ToString(DataBinder.Eval(Container.DataItem, "Name"))) %>
                                    </a>
                                </ItemTemplate>
                                <SeparatorTemplate>
                                    ,
                                </SeparatorTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                    <tr>
                        <td class="header2" colspan="2">
                            <%= GetText("STATS") %>
                        </td>
                    </tr>
                    <tr>
                        <td class="post" width="1%">
                            <img src="<%# GetThemeContents("ICONS","FORUM_STATS") %>" alt="" /></td>
                        <td class="post">
                            <asp:Label ID="Stats" runat="server">Label</asp:Label></td>
                    </tr>
                </tbody>
            </table>
        </td>
        <td width="10">
            &nbsp;</td>
        <td width="35%" valign="top" height="100%">
            <table border="0" class="content" cellspacing="1" cellpadding="0" width="100%">
                <tr>
                    <td class="header1" colspan="2">
                        <asp:ImageButton runat="server" ID="expandActiveDiscussions" BorderWidth="0" ImageAlign="Baseline" OnClick="expandActiveDiscussions_Click" />&nbsp;&nbsp;<%= GetText("ACTIVE_DISCUSSIONS") %></td>
                </tr>
                <tbody id="ActiveDiscussionTBody" runat="server">
                    <tr>
                        <td class="header2" colspan="2">
                            <%= GetText("LATEST_POSTS") %>
                        </td>
                    </tr>
                    <tr>
                        <td class="post" valign="top">
                            <asp:Repeater runat="server" ID="LatestPosts">
                                <ItemTemplate>
                                    &nbsp;<a href='<%#YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"m={0}#{0}",DataBinder.Eval(Container.DataItem, "LastMessageID"))%>'><%# YAF.Classes.Utils.General.BadWordReplace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Topic"))) %></a>
                                    <a href="<%#YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"m={0}#{0}",DataBinder.Eval(Container.DataItem, "LastMessageID"))%>">
                                        <img src="<%# GetThemeContents("ICONS","ICON_LATEST") %>" border="0" alt=""></a><br />
                                </ItemTemplate>
                            </asp:Repeater>
                        </td>
                    </tr>
                </tbody>
            </table>
        </td>
    </tr>
</table>
<table style="padding: 2px; margin: 2px" width="100%">
    <tr>
        <td>
            <img align="middle" src="<% =GetThemeContents("ICONS","FORUM_NEW") %>" alt="" />
            <%# GetText("ICONLEGEND","New_Posts") %>
            <img align="middle" src="<% =GetThemeContents("ICONS","FORUM") %>" alt="" />
            <%# GetText("ICONLEGEND","No_New_Posts") %>
            <img align="middle" src="<% =GetThemeContents("ICONS","FORUM_LOCKED") %>" alt="" />
            <%# GetText("ICONLEGEND","Forum_Locked") %>
        </td>
        <td align="right">
            <asp:LinkButton runat="server" OnClick="MarkAll_Click" ID="MarkAll" /></td>
    </tr>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
