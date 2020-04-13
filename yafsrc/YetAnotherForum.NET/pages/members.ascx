<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Members" CodeBehind="Members.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2>
            <YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" />
        </h2>
    </div>
</div>

<div class="row mb-3">
    <div class="col-xl-12">
        <YAF:Pager runat="server" ID="Pager" OnPageChange="Pager_PageChange" />
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between">
                    <div class="col-md-3">
                        <YAF:Icon runat="server"
                                  IconName="users"
                                  IconType="text-secondary"></YAF:Icon>
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                            LocalizedTag="TITLE" />
                    </div>
                    <div class="col-md-3 mt-1">
                        <div class="btn-toolbar" role="toolbar">
                            <div class="btn-group mr-2" role="group" aria-label="sort">
                                <YAF:ThemeButton ID="Sort" runat="server"
                                     CssClass="dropdown-toggle"
                                     Type="Secondary"
                                     DataToggle="dropdown"
                                     TextLocalizedTag="SORT_BY"
                                     Icon="sort" />
                    <div class="dropdown-menu">
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
                            <div class="btn-group dropleft" role="group" aria-label="Filters">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        
                        <div class="dropdown-menu">
                            <div class="px-3 py-1" style="min-width:max-content">
                                <div class="form-row">
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Group">
                                            <YAF:LocalizedLabel ID="SearchRolesLocalizedLabel" runat="server" 
                                                                LocalizedTag="Search_Role" />
                                        </asp:Label>
                                        <asp:DropDownList ID="Group" runat="server" 
                                                          CssClass="select2-select">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Ranks">
                                            <YAF:LocalizedLabel ID="SearchRankLocalizedLabel" runat="server" 
                                                                LocalizedTag="Search_Rank" />
                                        </asp:Label>
                                        <asp:DropDownList ID="Ranks" runat="server" 
                                                          CssClass="select2-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-row">
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="NumPostDDL">
                                            <YAF:LocalizedLabel ID="NumPostsLabel" runat="server" 
                                                                LocalizedTag="NUMPOSTS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="NumPostDDL" runat="server" 
                                                          CssClass="select2-select">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="NumPostsTB">&nbsp;</asp:Label>
                                        <asp:TextBox ID="NumPostsTB" runat="server"
                                                     CssClass="form-control"
                                                     TextMode="Number"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="UserSearchName">
                                        <YAF:LocalizedLabel ID="SearchMemberLocalizedLabel" runat="server" 
                                                            LocalizedTag="Search_Member" />
                                    </asp:Label>
                                    <asp:TextBox ID="UserSearchName" runat="server" 
                                                 CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <YAF:ThemeButton ID="SearchByUserName" runat="server"
                                                     OnClick="Search_Click"
                                                     TextLocalizedTag="BTNSEARCH"
                                                     Type="Primary"
                                                     Icon="search">
                                    </YAF:ThemeButton>
                                    &nbsp;
                                    <YAF:ThemeButton ID="ResetUserSearch" runat="server"
                                                     OnClick="Reset_Click"
                                                     TextLocalizedTag="CLEAR"
                                                     Type="Secondary"
                                                     Icon="trash">
                                    </YAF:ThemeButton>
                                </div>
                                
                                </div>
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
                                        <img src="<%# this.GetAvatarUrlFileName(this.Eval("UserID").ToType<int>(), this.Eval("Avatar").ToString(), this.Eval("AvatarImage").ToString().IsSet(), this.Eval("Email").ToString()) %>" alt="<%# this.HtmlEncode(DataBinder.Eval(Container.DataItem,"Name").ToString()) %>"
                                             title="<%# this.HtmlEncode(this.Eval(this.Get<BoardSettings>().EnableDisplayName ? "DisplayName" : "Name").ToString()) %>" 
                                             class="rounded img-fluid" />
                                        <YAF:UserLink ID="UserProfileLink" runat="server" 
                                                      IsGuest="False" 
                                                      ReplaceName='<%# this.Eval(this.Get<BoardSettings>().EnableDisplayName ? "DisplayName" : "Name").ToString() %>' 
                                                      UserID='<%# this.Eval("UserID").ToType<int>() %>'
                                                      Style='<%# this.Eval("Style") %>' />
                                    </h5>
                                    <small class="d-none d-md-block">
                                        <strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                    LocalizedTag="JOINED"
                                                                    LocalizedPage="POSTS"/>:</strong>
                                        <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %>
                                    </small>
                                </div>
                                <p class="mb-1">
                                    <ul class="list-inline">
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="RANK"></YAF:LocalizedLabel></strong>
                                            <%# this.Eval("RankName") %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="POSTS" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# "{0:N0}".Fmt(((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="LAST_VISIT" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %>
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

<YAF:Pager runat="server" LinkedPager="Pager" OnPageChange="Pager_PageChange" />
