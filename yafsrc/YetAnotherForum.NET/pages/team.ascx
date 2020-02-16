<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Team" Codebehind="Team.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>
<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-user-shield fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel 
                                                                 ID="LocalizedLabel2"
                                                                 runat="server" 
                                                                 LocalizedTag="Admins" LocalizedPage="TEAM" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="AdminsList" runat="server" OnItemDataBound="AdminsList_OnItemDataBound">
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action">
                             <div class="d-flex w-100 justify-content-between mb-3">
                                 <h5 class="mb-1 text-break">
                                     <asp:Image ID="AdminAvatar" runat="server"
                                                Width="40px" 
                                                Height="40px"
                                                CssClass="rounded img-fluid"/>
                                     <YAF:UserLink ID="AdminLink" runat="server" 
                                                   IsGuest="False" 
                                                   UserID='<%# this.Eval("UserID").ToType<int>() %>' 
                                                   Style='<%# this.Eval("Style") %>'  />
                                 </h5>
                                 <small>
                                     <span class="font-weight-bold">
                                         <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                             LocalizedTag="FORUMS" />:
                                     </span>
                                     <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="FORUMS_ALL" LocalizedPage="TEAM" />
                                 </small>
                             </div>
                            <small>
                                <div class="btn-group" role="group">
                                    <YAF:ThemeButton ID="PM" runat="server"
                                                     Size="Small" 
                                                     Visible="false" 
                                                     TextLocalizedPage="POSTS" TextLocalizedTag="PM"
                                                     TitleLocalizedPage="POSTS" TitleLocalizedTag="PM_TITLE"
                                                     Icon="envelope" Type="Secondary"/>
                                    <YAF:ThemeButton ID="Email" runat="server" 
                                                     Size="Small" Visible="false"
                                                     TextLocalizedPage="POSTS" TextLocalizedTag="EMAIL"
                                                     TitleLocalizedPage="POSTS" TitleLocalizedTag="EMAIL_TITLE"
                                                     Icon="at" Type="Secondary" />
                                    <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                                     TitleLocalizedPage="PROFILE" TitleLocalizedTag="ADMIN_USER"
                                                     TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE" 
                                                     Size="Small" Visible="false"
                                                     Icon="user-cog" Type="Secondary"
                                                     NavigateUrl='<%# BuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.Eval("UserID").ToType<int>() ) %>'>
                                    </YAF:ThemeButton>
                                </div>
                            </small>
                            </li>
			            </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
            </div>
        </div>
    </div>
    <div class="col" id="ModsTable" runat="server">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fas fa-user-secret fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel 
                                                                 ID="LocalizedLabel1" 
                                                                 runat="server" 
                                                                 LocalizedTag="MODS" LocalizedPage="TEAM" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="ModeratorsList" runat="server" OnItemDataBound="ModeratorsList_OnItemDataBound">
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action">
                             <div class="d-flex w-100 justify-content-between mb-3">
                                 <h5 class="mb-1 text-break">
                                     <asp:Image ID="ModAvatar" runat="server"
                                                Width="40px" 
                                                Height="40px"
                                                CssClass="rounded img-fluid"/>
                                     <YAF:UserLink ID="ModLink" runat="server" 
                                                   ReplaceName='<%#  this.Eval(this.Get<BoardSettings>().EnableDisplayName ? "DisplayName" : "Name").ToString() %>' 
                                                   UserID='<%# this.Eval("ModeratorID").ToType<int>() %>' 
                                                   IsGuest="False" 
                                                   Style='<%# this.Eval("Style") %>'  />
                                 </h5>
                             </div>
                                <p>
                                        <span class="font-weight-bold">
                                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                                LocalizedTag="FORUMS" />:
                                        </span>
                                    <div class="input-group">
                                        <span class="input-group-prepend">
                                            <YAF:ThemeButton ID="GoToForumButton" runat="server" 
                                                             Icon="external-link-alt" 
                                                             Type="Secondary"
                                                             TextLocalizedTag="GO" 
                                                             OnClick="GoToForum"></YAF:ThemeButton>
                                        </span>
                                        <asp:DropDownList ID="ModForums" runat="server" CssClass="select2-select form-control">
                                        </asp:DropDownList>
                                    </div>
                                </p>
                            <small>
                                <div class="btn-group" role="group">
                                    <YAF:ThemeButton ID="PM" runat="server" 
                                                     Size="Small" Visible="false" 
                                                     TextLocalizedPage="POSTS" TextLocalizedTag="PM" 
                                                     TitleLocalizedPage="POSTS" TitleLocalizedTag="PM_TITLE"
                                                     Icon="envelope" Type="Secondary" />
                                    <YAF:ThemeButton ID="Email" runat="server" 
                                                     Size="Small" Visible="false" 
                                                     TextLocalizedPage="POSTS" TextLocalizedTag="EMAIL"
                                                     TitleLocalizedPage="POSTS" TitleLocalizedTag="EMAIL_TITLE"
                                                     Icon="at" Type="Secondary" />
                                    <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                                     Size="Small" Visible="false"
                                                     TitleLocalizedPage="PROFILE" TitleLocalizedTag="ADMIN_USER"
                                                     TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE"
                                                     Icon="user-cog" Type="Secondary"
                                                     NavigateUrl='<%# BuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.Eval("ModeratorID").ToType<int>() ) %>'>
                                    </YAF:ThemeButton>
                                </div>
                            </small>
                            </li>
			            </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
            </div>
        </div>
    </div>
</div>