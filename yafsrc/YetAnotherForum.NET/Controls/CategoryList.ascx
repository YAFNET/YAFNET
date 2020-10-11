<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.CategoryList"
    CodeBehind="CategoryList.ascx.cs" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<%@ Register TagPrefix="YAF" TagName="ForumList" Src="ForumList.ascx" %>

<asp:Repeater ID="Categories" runat="server">
    <ItemTemplate>
                    <div class="row">
                        <div class="col">
                            <div class="card mb-3">
                                <div class="card-header d-flex align-items-center">
                                    <YAF:CollapseButton ID="CollapsibleImage" runat="server"
                                                        PanelID='<%# "categoryPanel{0}".Fmt(((ForumRead)Container.DataItem).CategoryID) %>'
                                                        AttachedControlID="body" 
                                                        CssClass="pl-0">
                                    </YAF:CollapseButton>
                                    <div class="d-none d-md-block icon-category">
                                        <%#  this.GetCategoryImage((ForumRead)Container.DataItem) %>
                                    </div>
                                    <%# this.HtmlEncode(((ForumRead)Container.DataItem).Category) %>
                                </div>
                                <div class="card-body" id="body" runat="server">
                                    <YAF:ForumList runat="server" 
                                                   Visible="true" 
                                                   ID="forumList"
                                                   DataSource="<%# this.GetForums((ForumRead)Container.DataItem) %>"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <div class="d-flex flex-row-reverse mb-3">
                        <div>
                           <div class="btn-group" role="group" aria-label="Tools">
                                <YAF:ThemeButton runat="server" ID="WatchForum"
                                                 OnClick="WatchAllClick" 
                                                 Type="Secondary" 
                                                 Size="Small"
                                                 Icon="eye"
                                                 TextLocalizedTag="WATCHFORUM_ALL"
                                                 TitleLocalizedTag="WATCHFORUM_ALL_HELP"
                                                 CommandArgument="<%# this.PageContext.PageCategoryID != 0 ? this.PageContext.PageCategoryID.ToString() : null %>"
                                                 Visible="<%# !this.PageContext.IsGuest %>"/>
                                <YAF:ThemeButton runat="server" ID="MarkAll"
                                                 OnClick="MarkAllClick" 
                                                 Type="Secondary"
                                                 Size="Small"
                                                 Icon="glasses"
                                                 TextLocalizedTag="MARKALL"
                                                 CommandArgument="<%# this.PageContext.PageCategoryID != 0 ? this.PageContext.PageCategoryID.ToString() : null %>"/>
                                <YAF:RssFeedLink ID="RssFeed1" runat="server"
                                                 FeedType="Forum" 
                                                 AdditionalParameters='<%# this.PageContext.PageCategoryID != 0 ? "c={0}&name={1}".Fmt(this.PageContext.PageCategoryID,this.PageContext.PageCategoryName) : null %>'
                                                 Visible="<%# this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ForumFeedAccess) %>" />
                            </div>
                        </div>
                    </div>
            </FooterTemplate>
</asp:Repeater>