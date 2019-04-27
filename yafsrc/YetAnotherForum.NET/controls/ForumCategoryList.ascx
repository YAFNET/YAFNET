<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.ForumCategoryList"
    CodeBehind="ForumCategoryList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<%@ Register TagPrefix="YAF" TagName="ForumList" Src="ForumList.ascx" %>

<asp:UpdatePanel ID="UpdatePanelCategory" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
            <asp:Repeater ID="CategoryList" runat="server">
                <ItemTemplate>
                    <div class="row">
                    <div class="col">
                    <div class="card mb-3">
                    <div class="card-header">
                        <YAF:CollapseButton ID="CollapsibleImage" runat="server"
                                            PanelID='<%# "categoryPanel" + DataBinder.Eval(Container.DataItem, "CategoryID") %>'
                                            AttachedControlID="body">
                        </YAF:CollapseButton>
                        <i class="fa fa-folder fa-fw"></i>&nbsp;<asp:Image ID="uxCategoryImage" CssClass="category_image" AlternateText=" " ImageUrl='<%# YafForumInfo.ForumClientFileRoot + YafBoardFolders.Current.Categories + "/" + DataBinder.Eval(Container.DataItem, "CategoryImage") %>'
                                                                             Visible='<%# !DataBinder.Eval(Container.DataItem, "CategoryImage").ToString().IsNotSet() %>'
                                                                             runat="server" />
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
                                                 AdditionalParameters='<%# this.PageContext.PageCategoryID != 0 ? string.Format("c={0}", this.PageContext.PageCategoryID) : null %>'
                                                 Visible="<%# this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ForumFeedAccess) %>" />
                            </div>
                        </div>
                    </div>
            </FooterTemplate>
        </asp:Repeater>
    </ContentTemplate>
</asp:UpdatePanel>
