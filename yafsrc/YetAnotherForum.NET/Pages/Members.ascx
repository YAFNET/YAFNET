<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Members" CodeBehind="Members.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconName="users"></YAF:IconHeader>
                    </div>
                    <div class="col-auto">
                        <div class="btn-toolbar" role="toolbar">
                            <div class="input-group input-group-sm me-2 mb-1" role="group">
                                <div class="input-group-text">
                                    <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                </div>
                                <asp:DropDownList runat="server" ID="PageSize"
                                                  AutoPostBack="True"
                                                  OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                  CssClass="form-select">
                                </asp:DropDownList>
                            </div>
                            <div class="btn-group me-2 mb-1" role="group" aria-label="Filters">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Size="Small"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        
                        <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                            <div class="px-3 py-1">
                                <div class="mb-3">
                                    <asp:Label runat="server" AssociatedControlID="Group">
                                        <YAF:LocalizedLabel ID="SearchRolesLocalizedLabel" runat="server" 
                                                            LocalizedTag="Search_Role" />
                                    </asp:Label>
                                    <asp:DropDownList ID="Group" runat="server" 
                                                      CssClass="select2-select">
                                    </asp:DropDownList>
                                </div>
                                <div class="mb-3">
                                    <asp:Label runat="server" AssociatedControlID="Ranks">
                                        <YAF:LocalizedLabel ID="SearchRankLocalizedLabel" runat="server" 
                                                            LocalizedTag="Search_Rank" />
                                    </asp:Label>
                                    <asp:DropDownList ID="Ranks" runat="server" 
                                                      CssClass="select2-select">
                                    </asp:DropDownList>
                                </div>
                                <div class="mb-3">
                                    <asp:Label runat="server" AssociatedControlID="NumPostDDL">
                                        <YAF:LocalizedLabel ID="NumPostsLabel" runat="server" 
                                                            LocalizedTag="NUMPOSTS" />
                                    </asp:Label>
                                    <asp:DropDownList ID="NumPostDDL" runat="server" 
                                                      CssClass="select2-select">
                                    </asp:DropDownList>
                                </div>
                                <div class="mb-3">
                                    <asp:TextBox ID="NumPostsTB" runat="server"
                                                 CssClass="form-control"
                                                 TextMode="Number"></asp:TextBox>
                                </div>
                                <div class="mb-3">
                                    <asp:Label runat="server" AssociatedControlID="UserSearchName">
                                        <YAF:LocalizedLabel ID="SearchMemberLocalizedLabel" runat="server" 
                                                            LocalizedTag="Search_Member" />
                                    </asp:Label>
                                    <asp:TextBox ID="UserSearchName" runat="server" 
                                                 CssClass="form-control"></asp:TextBox>
                                </div>
                                <YAF:ThemeButton ID="SearchByUserName" runat="server"
                                                 OnClick="Search_Click"
                                                 CssClass="me-2"
                                                 TextLocalizedTag="BTNSEARCH"
                                                 Type="Primary"
                                                 Icon="search">
                                </YAF:ThemeButton>
                                <YAF:ThemeButton ID="ResetUserSearch" runat="server"
                                                 OnClick="Reset_Click"
                                                 TextLocalizedTag="CLEAR"
                                                 Type="Secondary"
                                                 Icon="trash">
                                </YAF:ThemeButton>
                                
                                </div>
                            </div>
                        </div>
                            <div class="btn-group btn-group-sm mb-1" role="group" aria-label="sort">
                                <YAF:ThemeButton ID="Sort" runat="server"
                                                 CssClass="dropdown-toggle"
                                                 Size="Small"
                                                 Type="Secondary"
                                                 DataToggle="dropdown"
                                                 TextLocalizedTag="SORT_BY"
                                                 Icon="sort" />
                                <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                                     <YAF:ThemeButton ID="SortUserNameAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="UserNameAsc_Click"
                                         TextLocalizedTag="USERNAME_ASC"/>
                        <YAF:ThemeButton ID="SortUserNameDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="UserNameDesc_Click"
                                         TextLocalizedTag="USERNAME_DESC"/>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="SortRankAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="RankAsc_Click"
                                         TextLocalizedTag="RANK_ASC" />
                        <YAF:ThemeButton ID="SortRankDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="RankDesc_Click"
                                         TextLocalizedTag="RANK_DESC" />
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="SortJoinedAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="JoinedAsc_Click"
                                         TextLocalizedTag="JOINED_ASC" />
                        <YAF:ThemeButton ID="SortJoinedDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="JoinedDesc_Click"
                                         TextLocalizedTag="JOINED_DESC" />
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="SortPostsAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="PostsAsc_Click"
                                         TextLocalizedTag="POSTS_ASC" />
                        <YAF:ThemeButton ID="SortPostsDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="PostsDesc_Click"
                                         TextLocalizedTag="POSTS_DESC" />
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="SortLastVisitAsc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="LastVisitAsc_Click"
                                         TextLocalizedTag="LASTVISIT_ASC" />
                        <YAF:ThemeButton ID="SortLastVisitDesc" runat="server"
                                         CssClass="dropdown-item"
                                         Type="None" 
                                         OnClick="LastVisitDesc_Click"
                                         TextLocalizedTag="LASTVISIT_DESC" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <YAF:AlphaSort ID="AlphaSort1" runat="server" />
                <asp:Repeater ID="MemberList" runat="server">
                            <HeaderTemplate>
                                <ul class="list-group">
                            </HeaderTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                            <ItemTemplate>
                                <li class="list-group-item list-group-item-action">
                                <div class="d-flex w-100 justify-content-between">
                                    <h5 class="mb-1 text-break">
                                        <img src="<%# this.GetAvatarUrlFileName(((PagedUser)Container.DataItem).UserID, ((PagedUser)Container.DataItem).Avatar, ((PagedUser)Container.DataItem).AvatarImage != null, ((PagedUser)Container.DataItem).Email) %>" alt="<%# this.HtmlEncode(((PagedUser)Container.DataItem).Name) %>"
                                             title="<%# this.HtmlEncode(this.PageContext.BoardSettings.EnableDisplayName ? ((PagedUser)Container.DataItem).DisplayName : ((PagedUser)Container.DataItem).Name) %>" 
                                             class="rounded img-fluid"
                                             style="max-height: 50px; max-width:50px"/>
                                        <YAF:UserLink ID="UserProfileLink" runat="server" 
                                                      Suspended="<%# ((PagedUser)Container.DataItem).Suspended %>"
                                                      IsGuest="False" 
                                                      ReplaceName="<%# this.PageContext.BoardSettings.EnableDisplayName ? ((PagedUser)Container.DataItem).DisplayName : ((PagedUser)Container.DataItem).Name %>" 
                                                      UserID="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                      Style="<%# ((PagedUser)Container.DataItem).UserStyle %>" />
                                    </h5>
                                    <small class="d-none d-md-block">
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                    LocalizedTag="JOINED"
                                                                    LocalizedPage="POSTS"/>:</strong>
                                        <%# this.Get<IDateTimeService>().FormatDateLong(((PagedUser)Container.DataItem).Joined) %>
                                    </small>
                                </div>
                                <p class="mb-1">
                                    <ul class="list-inline">
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="RANK"></YAF:LocalizedLabel></strong>
                                            <%# ((PagedUser)Container.DataItem).RankName %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="POSTS" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# "{0:N0}".Fmt(((PagedUser)Container.DataItem).NumPosts) %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="LAST_VISIT" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# this.Get<IDateTimeService>().FormatDateLong(((PagedUser)Container.DataItem).LastVisit) %>
                                        </li>
                                    </ul>
                                </p>
                            </li>
                            </ItemTemplate>
                        </asp:Repeater>
            </div>
        </div>
    </div>
</div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager runat="server" ID="Pager"
                   OnPageChange="Pager_PageChange" />
    </div>
</div>