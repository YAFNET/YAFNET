<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Forums" Codebehind="Forums.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Models" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <div class="row justify-content-between align-items-center">
                        <div class="col-auto">
                            <YAF:IconHeader runat="server"
                                            IconName="comments"
                                            LocalizedTag="admin_forums"
                                            LocalizedPage="ADMINMENU"></YAF:IconHeader>
                        </div>
                        <div class="col-auto">
                        <div class="btn-toolbar" role="toolbar">
                           
                            <div class="btn-group btn-group-sm me-2 mb-1" role="group" aria-label="tools">
                                <YAF:ThemeButton runat="server"
                                                 CssClass="dropdown-toggle"
                                                 DataToggle="dropdown"
                                                 Size="Small"
                                                 Type="Secondary"
                                                 Icon="tools"
                                                 TextLocalizedTag="TOOLS" />
                                <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                                     <YAF:ThemeButton ID="SortCategoriesAsc" runat="server"
                                                      CssClass="dropdown-item"
                                                      Icon="arrow-down-a-z"
                                                      OnClick="SortCategoriesAscending"
                                                      ReturnConfirmTag="CONFIRM_SORTING"
                                                      Type="None"
                                                      TextLocalizedTag="SORT_CATEGORIES_ASC"/>
                                    <YAF:ThemeButton ID="SortCategoriesDesc" runat="server"
                                                     CssClass="dropdown-item"
                                                     Icon="arrow-down-z-a"
                                                     OnClick="SortCategoriesDescending"
                                                     ReturnConfirmTag="CONFIRM_SORTING"
                                                     Type="None"
                                                     TextLocalizedTag="SORT_CATEGORIES_DESC"/>
                                    <div class="dropdown-divider"></div>
                                    <YAF:ThemeButton ID="SortForumsAsc" runat="server"
                                                     CssClass="dropdown-item"
                                                     Icon="arrow-down-a-z"
                                                     OnClick="SortForumsAscending"
                                                     ReturnConfirmTag="CONFIRM_SORTING"
                                                     Type="None"
                                                     TextLocalizedTag="SORT_FORUMS_ASC"/>
                                    <YAF:ThemeButton ID="SortForumsDesc" runat="server"
                                                     CssClass="dropdown-item"
                                                     Icon="arrow-down-z-a"
                                                     OnClick="SortForumsDescending"
                                                     ReturnConfirmTag="CONFIRM_SORTING"
                                                     Type="None"
                                                     TextLocalizedTag="SORT_FORUMS_DESC"/>
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>
                </div>
                <div class="card-body">
        <asp:Repeater ID="CategoryList" runat="server" OnItemCommand="CategoryList_ItemCommand" OnItemDataBound="CategoryList_OnItemDataBound">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action active list-group-item-menu">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1"><i class="fa fa-folder fa-fw pe-1"></i><%# this.HtmlEncode(((Category)Container.DataItem).Name)%></h5>
                    <small class="d-none d-md-block">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="SORT_ORDER">
                        </YAF:LocalizedLabel>&nbsp;
                        <%# ((Category)Container.DataItem).SortOrder %>
                    </small>
                </div>
                <small>
                    <div class="btn-group btn-group-sm">
                        <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                         Type="Info"
                                         Size="Small"
                                         CommandName="edit" CommandArgument="<%# ((Category)Container.DataItem).ID %>"
                                         TitleLocalizedTag="EDIT" Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                         Type="Danger"
                                         Size="Small"
                                         ReturnConfirmTag="CONFIRM_DELETE_CAT"
                                         CommandName="delete" CommandArgument="<%# ((Category)Container.DataItem).ID %>"
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                    </div>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         CommandName="edit" CommandArgument="<%# ((Category)Container.DataItem).ID %>"
                                         TitleLocalizedTag="EDIT" Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         ReturnConfirmTag="CONFIRM_DELETE_CAT"
                                         CommandName="delete" CommandArgument="<%# ((Category)Container.DataItem).ID %>"
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="NewCategory" runat="server"
                                         OnClick="NewCategory_Click"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="plus-square"
                                         TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
                        <YAF:ThemeButton ID="NewForum" runat="server"
                                         OnClick="NewForum_Click"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="plus-square"
                                         TextLocalizedTag="NEW_FORUM"
                                         TextLocalizedPage="ADMIN_FORUMS"></YAF:ThemeButton>
                    </div>
                </small>
                </li>
                <asp:Repeater ID="ForumList" OnItemCommand="ForumList_ItemCommand" runat="server">
                    <ItemTemplate>
                        <li class="list-group-item list-group-item-action list-group-item-menu">
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">
                                    <i class="fa fa-comments fa-fw me-2"></i><%# ((YAF.Types.Models.Forum)Container.DataItem).ParentID.HasValue ? "---" : "-" %> <%# this.HtmlEncode(((YAF.Types.Models.Forum)Container.DataItem).Name) %>
                                </h5>
                                <small class="d-none d-md-block">
                                    <YAF:LocalizedLabel runat="server" LocalizedTag="SORT_ORDER" />&nbsp;
                                    <%# ((YAF.Types.Models.Forum)Container.DataItem).SortOrder %>
                                </small>
                            </div>
                            <p class="mb-1">
                                <%# this.HtmlEncode(((YAF.Types.Models.Forum)Container.DataItem).Description) %>
                            </p>
                            <small>
                                <div class="btn-group btn-group-sm">
                                    <YAF:ThemeButton ID="btnEdit"
                                                     Type="Info"
                                                     Size="Small"
                                                     CommandName="edit" CommandArgument="<%# ((YAF.Types.Models.Forum)Container.DataItem).ID %>"
                                                     TextLocalizedTag="EDIT"
                                                     TitleLocalizedTag="EDIT"
                                                     Icon="edit" runat="server"></YAF:ThemeButton>
                                    <YAF:ThemeButton ID="btnDuplicate"
                                                     Type="Info"
                                                     Size="Small"
                                                     CommandName="copy" CommandArgument="<%# ((YAF.Types.Models.Forum)Container.DataItem).ID %>"
                                                     TextLocalizedTag="COPY"
                                                     TitleLocalizedTag="COPY"
                                                     Icon="copy" runat="server"></YAF:ThemeButton>
                                    <YAF:ThemeButton ID="btnDelete" Type="Danger" Size="Small"
                                                     CommandName="delete" CommandArgument="<%# ((YAF.Types.Models.Forum)Container.DataItem).ID %>"
                                                     TextLocalizedTag="DELETE"
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash" runat="server"></YAF:ThemeButton>
                                </div>
                            </small>
                            <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                                <YAF:ThemeButton ID="ThemeButton3"
                                                 Type="None"
                                                 CssClass="dropdown-item"
                                                 CommandName="edit" CommandArgument="<%# ((YAF.Types.Models.Forum)Container.DataItem).ID %>"
                                                 TextLocalizedTag="EDIT"
                                                 TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButton4"
                                                 Type="None"
                                                 CssClass="dropdown-item"
                                                 CommandName="copy" CommandArgument="<%# ((YAF.Types.Models.Forum)Container.DataItem).ID %>"
                                                 TextLocalizedTag="COPY"
                                                 TitleLocalizedTag="COPY" Icon="copy" runat="server"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="ThemeButton5"
                                                 Type="None"
                                                 CssClass="dropdown-item"
                                                 CommandName="delete" CommandArgument="<%# ((YAF.Types.Models.Forum)Container.DataItem).ID %>"
                                                 TextLocalizedTag="DELETE"
                                                 TitleLocalizedTag="DELETE" Icon="trash" runat="server"></YAF:ThemeButton>
                                <div class="dropdown-divider"></div>
                                <YAF:ThemeButton ID="NewCategory" runat="server"
                                                 OnClick="NewCategory_Click"
                                                 Type="None"
                                                 CssClass="dropdown-item"
                                                 Icon="plus-square"
                                                 TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
                                <YAF:ThemeButton ID="NewForum" runat="server"
                                                 OnClick="NewForum_Click"
                                                 Type="None"
                                                 CssClass="dropdown-item"
                                                 Icon="plus-square"
                                                 TextLocalizedTag="NEW_FORUM"
                                                 TextLocalizedPage="ADMIN_FORUMS"></YAF:ThemeButton>
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                <YAF:ThemeButton ID="NewCategory" runat="server"
                                 OnClick="NewCategory_Click"
                                 Type="Primary"
                                 Icon="plus-square"
                                 TextLocalizedTag="NEW_CATEGORY"></YAF:ThemeButton>
                <YAF:ThemeButton ID="NewForum" runat="server"
                                 OnClick="NewForum_Click"
                                 Type="Primary"
                                 Icon="plus-square"
                                 TextLocalizedTag="NEW_FORUM"
                                 TextLocalizedPage="ADMIN_FORUMS"></YAF:ThemeButton>

                </div>
            </div>
        </div>
    </div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTopPageChange"
                   UsePostBack="True" />
    </div>
</div>