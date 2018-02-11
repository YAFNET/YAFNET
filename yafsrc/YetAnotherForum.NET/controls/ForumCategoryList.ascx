<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.ForumCategoryList"
    CodeBehind="ForumCategoryList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="ForumList.ascx" %>
<asp:UpdatePanel ID="UpdatePanelCategory" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table class="content" width="100%">
            <asp:Repeater ID="CategoryList" runat="server">
                <HeaderTemplate>
                    <tr class="forumRowTitle">
                        <th colspan="2" align="left" class="header1 headerForum">
                            <YAF:LocalizedLabel ID="ForumHeaderLabel" runat="server" LocalizedTag="FORUM" />
                        </th>
                        <th id="Td1" class="header1 headerModerators" width="15%" runat="server" visible="<%# PageContext.BoardSettings.ShowModeratorList && PageContext.BoardSettings.ShowModeratorListAsColumn %>">
                            <YAF:LocalizedLabel ID="ModeratorsHeaderLabel" runat="server" LocalizedTag="MODERATORS" />
                        </th>
                        <th class="header1 headerTopics" width="4%">
                            <YAF:LocalizedLabel ID="TopicsHeaderLabel" runat="server" LocalizedTag="TOPICS" />
                        </th>
                        <th class="header1 headerPosts" width="4%">
                            <YAF:LocalizedLabel ID="PostsHeaderLabel" runat="server" LocalizedTag="POSTS" />
                        </th>
                        <th class="header1 headerLastPost" width="25%">
                            <YAF:LocalizedLabel ID="LastPostHeaderLabel" runat="server" LocalizedTag="LASTPOST" />
                        </th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="forumRowCat header2">
                        <td colspan="<%# ColumnCount() %>">
                            <YAF:CollapsibleImage ID="CollapsibleImage" runat="server" BorderWidth="0" ImageAlign="Bottom"
                                PanelID='<%# "categoryPanel" + DataBinder.Eval(Container.DataItem, "CategoryID").ToString() %>'
                                AttachedControlID="forumList" ToolTip='<%# this.GetText("COMMON", "SHOWHIDE") %>' />
                            &nbsp;&nbsp; <a href='<%# YAF.Utils.YafBuildLink.GetLink(ForumPages.forum,"c={0}",DataBinder.Eval(Container.DataItem, "CategoryID")) %>'
                                title='<%# this.GetText("COMMON", "VIEW_CATEGORY") %>'>
                                <asp:Image ID="uxCategoryImage" CssClass="category_image" AlternateText=" " ImageUrl='<%# YafForumInfo.ForumClientFileRoot + YafBoardFolders.Current.Categories + "/" + DataBinder.Eval(Container.DataItem, "CategoryImage") %>'
                                    Visible='<%# !DataBinder.Eval(Container.DataItem, "CategoryImage").ToString().IsNotSet() %>'
                                    runat="server" />
                                <%# Page.HtmlEncode(DataBinder.Eval(Container.DataItem, "Name")) %>
                            </a>
                        </td>
                    </tr>
                    <YAF:ForumList runat="server" Visible="true" ID="forumList" DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>' />
                </ItemTemplate>
                <FooterTemplate>
                    <tr class="forumRowFoot footer1">
                        <td colspan="<%# ColumnCount() %>" align="right">
                            <div class="btn-group" role="group" aria-label="Tools">
                            <YAF:ThemeButton runat="server" OnClick="MarkAll_Click" ID="MarkAll" 
                                             Type="Secondary"
                                             CssClass="btn-sm"
                                             Icon="flag-checkered"
                                             TextLocalizedTag="MARKALL" />
                            <YAF:RssFeedLink ID="RssFeed1" runat="server" 
                                             FeedType="Forum" 
                                             AdditionalParameters='<%# this.PageContext.PageCategoryID != 0 ? string.Format("c={0}", this.PageContext.PageCategoryID) : null %>'
                                             Visible="<%# this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ForumFeedAccess) %>" />
                            </div>
                        </td>
                    </tr>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
