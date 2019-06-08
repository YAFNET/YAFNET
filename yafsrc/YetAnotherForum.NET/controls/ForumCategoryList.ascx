<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.ForumCategoryList"
    CodeBehind="ForumCategoryList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>

<%@ Register TagPrefix="YAF" TagName="ForumList" Src="ForumList.ascx" %>

<asp:UpdatePanel ID="UpdatePanelCategory" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:Repeater ID="CategoryList" runat="server">
                <ItemTemplate>
                    <div class="row">
                    <div class="col">
                    <div class="card mb-3">
                    <div class="card-header d-flex align-items-center">
                        <YAF:CollapseButton ID="CollapsibleImage" runat="server"
                                            PanelID='<%# "categoryPanel{0}".Fmt(DataBinder.Eval(Container.DataItem, "CategoryID")) %>'
                                            AttachedControlID="body" CssClass="pl-0">
                        </YAF:CollapseButton>
                        <div class="d-none d-md-block">
                            <i class="fas fa-folder fa-fw text-warning" aria-hidden="true"></i>&nbsp;<asp:Image ID="uxCategoryImage" 
                                                                                                                CssClass="category_image" 
                                                                                                                AlternateText=" " 
                                                                                                                ImageUrl='<%# "{0}{1}/{2}".Fmt(YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Categories, DataBinder.Eval(Container.DataItem, "CategoryImage")) %>'
                                                                             Visible='<%# !DataBinder.Eval(Container.DataItem, "CategoryImage").ToString().IsNotSet() %>'
                                                                             runat="server" />
                        </div>
                        <%# this.Page.HtmlEncode(DataBinder.Eval(Container.DataItem, "Name")) %>
                    </div>
                    <div class="card-body" id="body" runat="server">
                                <YAF:ForumList runat="server" 
                                               Visible="true" 
                                               ID="forumList" 
                                               DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>' />
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <div class="row mb-3">
                        <div class="col">
                            <div class="btn-group float-right" role="group" aria-label="Tools">
                                <YAF:ThemeButton runat="server" OnClick="MarkAll_Click" ID="MarkAll"
                                                 Type="Secondary"
                                                 Size="Small"
                                                 Icon="glasses"
                                                 TextLocalizedTag="MARKALL" />
                                <YAF:RssFeedLink ID="RssFeed1" runat="server"
                                                 FeedType="Forum" 
                                                 AdditionalParameters='<%# this.PageContext.PageCategoryID != 0 ? "c={0}".Fmt(this.PageContext.PageCategoryID) : null %>'
                                                 Visible="<%# this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ForumFeedAccess) %>" />
                            </div>
                        </div>
                    </div>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
</asp:UpdatePanel>
