<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.users"
    CodeBehind="users.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Types.Flags" %>

<%@ Register TagPrefix="modal" TagName="Import" Src="../../Dialogs/UsersImport.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ADMIN_USERS" LocalizedPage="ADMINMENU" /></h1>
    </div>
    </div>
<asp:PlaceHolder runat="server" ID="SearchResults" Visible="False">
    <div class="row">
        <div class="col-xl-12">

    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTopPageChange" UsePostBack="True" />
    <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-user fa-fw text-secondary pr-1"></i>
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                        LocalizedTag="TITLE" 
                                        LocalizedPage="ADMIN_USERS" />
                    <div class="float-right">
                        <YAF:ThemeButton runat="server"
                                         CssClass="dropdown-toggle"
                                         DataToggle="dropdown"
                                         Type="Secondary"
                                         Icon="filter"
                                         TextLocalizedTag="FILTER_DROPDOWN"
                                         TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                        <div class="dropdown-menu dropdown-menu-right dropdown-menu-lg-left">
                            <div class="px-3 py-1">
                                <div class="form-row">
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="name">
                                            <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" 
                                                                LocalizedTag="NAME_CONTAINS" 
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:TextBox ID="name" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Email">
                                            <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" 
                                                                LocalizedTag="EMAIL_CONTAINS" 
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:TextBox ID="Email" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="form-row">
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="group">
                                            <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                                                LocalizedTag="ROLE" 
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="group" runat="server" 
                                                          CssClass="custom-select" 
                                                          data-placeholder='<%# this.GetText("ADMIN_USERS", "FILTER_BY_GROUP") %>'>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="rank">
                                            <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" 
                                                                LocalizedTag="RANK" 
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="rank" runat="server" 
                                                          CssClass="custom-select" 
                                                          data-placeholder='<%# this.GetText("ADMIN_USERS", "FILTER_BY_RANK") %>'>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="form-row">
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Since">
                                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                                LocalizedTag="FILTER" 
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <asp:DropDownList ID="Since" runat="server"
                                                          CssClass="custom-select"/>
                                    </div>
                                    <div class="form-group col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="SuspendedOnly">
                                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                LocalizedTag="SUSPENDED_ONLY" 
                                                                LocalizedPage="ADMIN_USERS" />
                                        </asp:Label>
                                        <div class="custom-control custom-switch">
                                            <asp:CheckBox ID="SuspendedOnly" runat="server" Text="&nbsp;"/>
                                        </div>
                                    </div>

                                </div>
                                <div class="form-group">
                                    <YAF:ThemeButton ID="search" runat="server" 
                                                     OnClick="SearchClick" 
                                                     Type="Primary"
                                                     Icon="search" 
                                                     TextLocalizedTag="SEARCH" 
                                                     TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>&nbsp;
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
                                        <%# this.GetIsUserDisabledLabel(this.Eval("Flags"))%>
                                        <asp:LinkButton ID="NameEdit" runat="server" CommandName="edit" CommandArgument='<%# this.Eval("UserID") %>'
                                                        Text='<%# this.HtmlEncode( this.Eval("Name").ToString() ) %>' />
                                        (<asp:LinkButton ID="DisplayNameEdit" runat="server" CommandName="edit" CommandArgument='<%# this.Eval("UserID") %>'
                                                         Text='<%# this.HtmlEncode( this.Eval("DisplayName").ToString() ) %>' />)
                                    </h5>
                                    <small class="d-none d-md-block">
                                        <span style="font-weight:bold">
                                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE_SUSPENDED" LocalizedPage="INFO" />&nbsp;:&nbsp;
                                        </span>
                                        <asp:Label runat="server" ID="Suspended" CssClass='<%# this.Eval("Suspended") == DBNull.Value ? "badge badge-success" : "badge badge-danger" %>'><%# this.GetSuspendedString(this.Eval("Suspended").ToString())%></asp:Label>
                                    </small>
                                </div>
                                <p class="mb-1">
                                    <ul class="list-inline">
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="EMAIL" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# DataBinder.Eval(Container.DataItem,"Email") %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="RANK"></YAF:LocalizedLabel> </strong>
                                            <%# this.Eval("RankName") %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="POSTS" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# this.Eval( "NumPosts") %>
                                        </li>
                                        <li class="list-inline-item">
                                            <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="LAST_VISIT" LocalizedPage="ADMIN_USERS" />:</strong>
                                            <%# this.Get<IDateTime>().FormatDateTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %>
                                        </li>
                                        <li class="list-inline-item">
                                            <span id="FacebookUser" class="FacebookIcon" runat="server" 
                                                  Visible='<%# this.Eval("IsFacebookUser").ToType<bool>() %>' 
                                                  title='<%# this.GetText("ADMIN_EDITUSER", "FACEBOOK_USER_HELP") %>'>&nbsp;</span>
                                        </li>
                                        <li class="list-inline-item">
                                            <span id="TwitterUser" class="TwitterIcon" runat="server" 
                                                  Visible='<%# this.Eval("IsTwitterUser").ToType<bool>() %>' 
                                                  title='<%# this.GetText("ADMIN_EDITUSER", "TWITTER_USER_HELP") %>'>&nbsp;</span>
                                        </li>
                                        <li class="list-inline-item">
                                            <span id="GoogleUser" class="GoogleIcon" runat="server" 
                                                  Visible='<%# this.Eval("IsGoogleUser").ToType<bool>() %>' 
                                                  title='<%# this.GetText("ADMIN_EDITUSER", "GOOGLE_USER_HELP") %>'>&nbsp;</span>
                                        </li>
                                    </ul>
                                </p>
                                <small>
                                    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server" 
                                                     Type="Info" 
                                                     Size="Small" 
                                                     CommandName="edit" 
                                                     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'
                                                     TextLocalizedTag="EDIT" 
                                                     TitleLocalizedTag="EDIT" 
                                                     Icon="edit">
                                    </YAF:ThemeButton>
                                    <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_USERS", "CONFIRM_DELETE") %>' 
                                                     Type="Danger" 
                                                     Size="Small" 
                                                     CommandName="delete" 
                                                     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'
                                                     TextLocalizedTag="DELETE" 
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash" 
                                                     Visible='<%# DataBinder.Eval(Container.DataItem, "IsGuest").ToType<bool>() == false && !Config.IsDotNetNuke %>'>
                                    </YAF:ThemeButton>
                                </small>
                                <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                                    <YAF:ThemeButton ID="ThemeButton1" runat="server" 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     CommandName="edit" 
                                                     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'
                                                     TextLocalizedTag="EDIT" 
                                                     TitleLocalizedTag="EDIT" 
                                                     Icon="edit">
                                    </YAF:ThemeButton>
                                    <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                                     ReturnConfirmText='<%# this.GetText("ADMIN_USERS", "CONFIRM_DELETE") %>' 
                                                     Type="None" 
                                                     CssClass="dropdown-item"
                                                     CommandName="delete" 
                                                     CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'
                                                     TextLocalizedTag="DELETE" 
                                                     TitleLocalizedTag="DELETE"
                                                     Icon="trash" 
                                                     Visible='<%# DataBinder.Eval(Container.DataItem, "IsGuest").ToType<bool>() == false && !Config.IsDotNetNuke %>'>
                                    </YAF:ThemeButton>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <YAF:Alert runat="server" ID="NoInfo" 
                               Type="warning" 
                               Visible="False">
                        <i class="fa fa-exclamation-triangle fa-fw text-warning"></i>
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NO_ENTRY"></YAF:LocalizedLabel>
                    </YAF:Alert>
                </div>
                <div class="card-footer text-center">
                    <asp:PlaceHolder runat="server" ID="ImportAndSyncHolder">
                    <YAF:ThemeButton id="NewUser" runat="server"
                                     CssClass="mt-1 mr-1"
                                     OnClick="NewUserClick" 
                                     Type="Primary"
                                     Icon="plus-square" 
                                     TextLocalizedTag="NEW_USER" 
                                     TextLocalizedPage="ADMIN_USERS">
                    </YAF:ThemeButton>
                        <YAF:ThemeButton id="SyncUsers" runat="server" 
                                     CssClass="mt-1 mr-1"
                                     OnClick="SyncUsersClick" 
                                     Type="Secondary"
                                     Icon="sync" 
                                     TextLocalizedTag="SYNC_ALL" 
                                     TextLocalizedPage="ADMIN_USERS" 
                                     ReturnConfirmText='<%# this.GetText("ADMIN_USERS", "CONFIRM_SYNC") %>'>
                    </YAF:ThemeButton>
                        <YAF:ThemeButton id="ImportUsers" runat="server" 
                                     CssClass="mt-1 mr-1"
                                     Icon="upload" 
                                     DataTarget="UsersImportDialog"  
                                     DataToggle="modal" 
                                     Type="Info"
                                     TextLocalizedTag="IMPORT" 
                                     TextLocalizedPage="ADMIN_USERS">
                    </YAF:ThemeButton>
                    </asp:PlaceHolder>
                    <YAF:ThemeButton id="ExportUsersXml" runat="server" 
                                     CssClass="mt-1 mr-1"
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
    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
    </asp:PlaceHolder>

<div class="row">
    <div class="col">
        <div class="card">
            <div class="card-header">
                <i class="fas fa-tools text-secondary mr-1"></i>
                <YAF:LocalizedLabel runat="server"
                                    LocalizedTag="TOOLS"></YAF:LocalizedLabel>
            </div>
            <div class="card-body">
                <div class="form-group">
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
                        <div class="input-group-append">
                            <div class="input-group-text">
                                <YAF:LocalizedLabel runat="server" LocalizedTag="YEARS"></YAF:LocalizedLabel>
                            </div>
                        </div>
                    </div>
                </div>
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

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000"
            OnTick="UpdateStatusTimerTick" />
    </ContentTemplate>
</asp:UpdatePanel>
<div>
    <div id="SyncUsersMessage" style="display: none">
		<div class="card text-white bg-danger mb-3 text-center">
		    <div class="card-body">
		        <blockquote class="blockquote mb-0 card-body">
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="SYNC_TITLE" LocalizedPage="ADMIN_USERS" />
                    </p>
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SYNC_MSG" LocalizedPage="ADMIN_USERS" />
                    </p>
                    <footer>
                        <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
                    </footer>
                </blockquote>
            </div>
        </div>
    </div>
</div>


<modal:Import ID="ImportDialog" runat="server" />