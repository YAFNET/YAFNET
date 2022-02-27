<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Users"
    CodeBehind="Users.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/UsersImport.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<asp:PlaceHolder runat="server" ID="SearchResults" Visible="False">
    <div class="row">
        <div class="col-xl-12">

        <div class="card mb-3">
            <div class="card-header">
                <div class="row justify-content-between align-items-center">
                    <div class="col-auto">
                        <YAF:IconHeader runat="server"
                                        IconName="users"
                                        LocalizedPage="ADMIN_USERS"></YAF:IconHeader>
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
                            <div class="btn-group btn-group-sm me-2 mb-1" role="group" aria-label="tools">
                                <YAF:ThemeButton runat="server"
                                                 CssClass="dropdown-toggle"
                                                 DataToggle="dropdown"
                                                 Size="Small"
                                                 Type="Secondary"
                                                 Icon="tools"
                                                 TextLocalizedTag="TOOLS" />
                                <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                                    <div class="px-3 py-1 dropdown-sm">
                                        <div class="mb-3">
                                            <YAF:HelpLabel runat="server"
                                                           AssociatedControlID="YearsOld"
                                                           LocalizedTag="LOCK_INACTIVE"></YAF:HelpLabel>
                                            <div class="input-group">
                                                <asp:TextBox ID="YearsOld" runat="server"
                                                             MaxLength="5"
                                                             Text="5"
                                                             CssClass="form-control"
                                                             TextMode="Number">
                                                </asp:TextBox>
                                                <div class="input-group-text">
                                                    <YAF:LocalizedLabel runat="server" LocalizedTag="YEARS"></YAF:LocalizedLabel>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="mb-3 d-grid gap-2">
                                            <YAF:ThemeButton runat="server"
                                                             Type="Danger"
                                                             Icon="trash"
                                                             TextLocalizedTag="LOCK_INACTIVE"
                                                             TitleLocalizedTag="LOCK_INACTIVE_HELP"
                                                             OnClick="LockAccountsClick"/>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="btn-group btn-group-sm mb-1" role="group" aria-label="sort">
                                <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Size="Small"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                                <div class="dropdown-menu dropdown-menu-end dropdown-menu-lg-start">
                                    <div class="px-3 py-1 dropdown-sm">
                                <div class="row">
                                    <div class="mb-3 col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="name">
                                            <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server"
                                                                LocalizedTag="NAME_CONTAINS"
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:TextBox ID="name" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="mb-3 col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Email">
                                            <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server"
                                                                LocalizedTag="EMAIL_CONTAINS"
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:TextBox ID="Email" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="mb-3 col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="group">
                                            <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server"
                                                                LocalizedTag="ROLE"
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="group" runat="server"
                                                          CssClass="form-select"
                                                          data-placeholder='<%# this.GetText("ADMIN_USERS", "FILTER_BY_GROUP") %>'>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mb-3 col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="rank">
                                            <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server"
                                                                LocalizedTag="RANK"
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="rank" runat="server"
                                                          CssClass="form-select"
                                                          data-placeholder='<%# this.GetText("ADMIN_USERS", "FILTER_BY_RANK") %>'>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="mb-3 col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Since">
                                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                                                LocalizedTag="FILTER"
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="Since" runat="server"
                                                          CssClass="form-select"/>
                                    </div>
                                    <div class="mb-3 col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="SuspendedOnly">
                                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                                LocalizedTag="SUSPENDED_ONLY"
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <div class="form-check form-switch">
                                            <asp:CheckBox ID="SuspendedOnly" runat="server" Text="&nbsp;"/>
                                        </div>
                                    </div>

                                </div>
                                <div class="mb-3">
                                    <YAF:ThemeButton ID="search" runat="server"
                                                     OnClick="SearchClick"
                                                     CssClass="me-2"
                                                     Type="Primary"
                                                     Icon="search"
                                                     TextLocalizedTag="SEARCH"
                                                     TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
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
                    <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserListItemCommand">
                        <HeaderTemplate>
                            <ul class="list-group">
                        </HeaderTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <ItemTemplate>
                            <li class="list-group-item list-group-item-action list-group-item-menu">
                                <div class="d-flex w-100 justify-content-between">
                                    <h5 class="mb-1 text-break">
                                        <%# this.GetIsUserDisabledLabel(((PagedUser)Container.DataItem).Flags)%>
                                        <asp:LinkButton ID="NameEdit" runat="server"
                                                        CommandName="edit"
                                                        CommandArgument="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                        Text="<%# this.HtmlEncode( ((PagedUser)Container.DataItem).Name) %>" />
                                        (<asp:LinkButton ID="DisplayNameEdit" runat="server"
                                                         CommandName="edit"
                                                         CommandArgument="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                         Text="<%# this.HtmlEncode( ((PagedUser)Container.DataItem).DisplayName) %>" />)
                                    </h5>
                                    <small class="d-none d-md-block">
                                        <span style="font-weight:bold">
                                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                                LocalizedTag="TITLE_SUSPENDED"
                                                                LocalizedPage="INFO" />&nbsp;:&nbsp;
                                        </span>
                                        <asp:Label runat="server" ID="Suspended" CssClass='<%# ((PagedUser)Container.DataItem).Suspended.HasValue ? "badge bg-danger" : "badge bg-success"  %>'><%# this.GetSuspendedString(((PagedUser)Container.DataItem).Suspended.ToString())%></asp:Label>
                                    </small>
                                </div>
                                <p class="mb-1">
                                    <ul class="list-inline">
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="EMAIL" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# ((PagedUser)Container.DataItem).Email %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="RANK"></YAF:LocalizedLabel> </strong>
                                            <%# this.HtmlEncode(((PagedUser)Container.DataItem).RankName) %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="POSTS" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# ((PagedUser)Container.DataItem).NumPosts %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="LAST_VISIT" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# this.Get<IDateTimeService>().FormatDateTime(((PagedUser)Container.DataItem).LastVisit) %>
                                        </li>
                                        <li class="list-inline-item" runat="server"
                                            Visible="<%# ((PagedUser)Container.DataItem).Profile_FacebookId.IsSet() %>">
                                            <span title='<%# this.GetText("ADMIN_EDITUSER", "FACEBOOK_USER_HELP") %>'>
                                                <YAF:Icon runat="server"
                                                          IconName="facebook"
                                                          IconStyle="fab"
                                                          IconType="text-info" />
                                            </span>
                                        </li>
                                        <li class="list-inline-item" runat="server"
                                            Visible="<%#((PagedUser)Container.DataItem).Profile_TwitterId.IsSet() %>" >
                                            <span title='<%# this.GetText("ADMIN_EDITUSER", "TWITTER_USER_HELP") %>'>

                                                <YAF:Icon runat="server"
                                                          IconName="twitter"
                                                          IconStyle="fab"
                                                          IconType="text-info" />
                                            </span>
                                        </li>
                                        <li class="list-inline-item" runat="server"
                                            Visible="<%# ((PagedUser)Container.DataItem).Profile_GoogleId.IsSet() %>">
                                            <span title='<%# this.GetText("ADMIN_EDITUSER", "GOOGLE_USER_HELP") %>'>
                                                <YAF:Icon runat="server"
                                                          IconName="google"
                                                          IconStyle="fab"
                                                          IconType="text-info" />
                                            </span>
                                        </li>
                                    </ul>
                                </p>
                                <small>
                                    <div class="btn-group btn-group-sm">
                                        <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                                         Type="Info"
                                                         Size="Small"
                                                         CommandName="edit"
                                                         CommandArgument="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                         TextLocalizedTag="EDIT"
                                                         TitleLocalizedTag="EDIT"
                                                         Icon="edit">
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                                         ReturnConfirmText='<%# this.GetText("ADMIN_USERS", "CONFIRM_DELETE") %>'
                                                         Type="Danger"
                                                         Size="Small"
                                                         CommandName="delete"
                                                         CommandArgument="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                         TextLocalizedTag="DELETE"
                                                         TitleLocalizedTag="DELETE"
                                                         Icon="trash"
                                                         Visible="<%# ((PagedUser)Container.DataItem).IsGuest == false && !YAF.Configuration.Config.IsDotNetNuke %>">
                                        </YAF:ThemeButton>
                                    </div>
                                </small>
                                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                                    <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                                     Type="None"
                                                     CssClass="dropdown-item"
                                                     CommandName="edit"
                                                     CommandArgument="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                     TextLocalizedTag="EDIT"
                                                     TitleLocalizedTag="EDIT"
                                                     Icon="edit">
                                    </YAF:ThemeButton>
                                    <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_USERS", "CONFIRM_DELETE") %>'
                                                     Type="None"
                                                     CssClass="dropdown-item"
                                                     CommandName="delete"
                                                     CommandArgument="<%# ((PagedUser)Container.DataItem).UserID %>"
                                                     TextLocalizedTag="DELETE"
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash"
                                                     Visible="<%# ((PagedUser)Container.DataItem).IsGuest == false && !YAF.Configuration.Config.IsDotNetNuke %>">
                                    </YAF:ThemeButton>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <YAF:Alert runat="server" ID="NoInfo"
                               Type="info"
                               Visible="False">
                        <i class="fa fa-exclamation-triangle fa-fw"></i>
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NO_ENTRY"></YAF:LocalizedLabel>
                    </YAF:Alert>
                </div>
                <div class="card-footer text-center">
                    <asp:PlaceHolder runat="server" ID="ImportAndSyncHolder">
                        <YAF:ThemeButton id="NewUser" runat="server"
                                         CssClass="mt-1 me-1"
                                         OnClick="NewUserClick"
                                         Type="Primary"
                                         Icon="plus-square"
                                         TextLocalizedTag="NEW_USER"
                                         TextLocalizedPage="ADMIN_USERS">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton id="SyncUsers" runat="server"
                                         CssClass="mt-1 me-1 btn-spinner"
                                         OnClick="SyncUsersClick"
                                         Type="Secondary"
                                         Icon="sync"
                                         TextLocalizedTag="SYNC_ALL"
                                         TextLocalizedPage="ADMIN_USERS"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_USERS", "CONFIRM_SYNC") %>'>
                        </YAF:ThemeButton>
                        <YAF:ThemeButton id="ImportUsers" runat="server"
                                         CssClass="mt-1 me-1"
                                         Icon="upload"
                                         DataTarget="UsersImportDialog"
                                         DataToggle="modal"
                                         Type="Info"
                                         TextLocalizedTag="IMPORT"
                                         TextLocalizedPage="ADMIN_USERS">
                        </YAF:ThemeButton>
                    </asp:PlaceHolder>
                    <YAF:ThemeButton id="ExportUsersXml" runat="server"
                                     CssClass="mt-1 me-1"
                                     OnClick="ExportUsersXmlClick"
                                     Type="Warning"
                                     Icon="download"
                                     TextLocalizedTag="EXPORT_XML"
                                     TextLocalizedPage="ADMIN_USERS">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton id="ExportUsersCsv" runat="server"
                                     CssClass="mt-1"
                                     OnClick="ExportUsersCsvClick"
                                     Type="Warning"
                                     Icon="download"
                                     TextLocalizedTag="EXPORT_CSV"
                                     TextLocalizedPage="ADMIN_USERS">
                    </YAF:ThemeButton>
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
    </asp:PlaceHolder>

<modal:Import ID="ImportDialog" runat="server" />