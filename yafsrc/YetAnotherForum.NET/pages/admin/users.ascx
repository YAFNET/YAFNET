<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.users"
    CodeBehind="users.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="System.Drawing" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
<div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_USERS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
    <asp:PlaceHolder runat="server" ID="SearchResults" Visible="False">

    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" UsePostBack="True" />
    <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-user fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_USERS" />
                    <div class="input-group pull-right user-search-dropdown">
                        <asp:TextBox ID="name" runat="server" CssClass="form-control"></asp:TextBox>
                        <div class="input-group-btn">
                            <asp:LinkButton ID="search" runat="server" OnClick="Search_Click" CssClass="btn btn-success"></asp:LinkButton>
                            <button type="button" class="btn btn-secondary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <span class="sr-only">Toggle Dropdown</span>
                            </button>
                            <ul class="dropdown-menu dropdown-menu-right">
                                <li class="form-group dropdown-item">
                                    <label for='<%= this.Email.ClientID %>'>
                                        <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="EMAIL_CONTAINS" LocalizedPage="ADMIN_USERS" />
                                    </label>
                                    <asp:TextBox ID="Email" runat="server" CssClass="form-control"></asp:TextBox>
                                </li>
                                <li class="form-group dropdown-item">
                                    <label for='<%= this.group.ClientID %>'>
                                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="ROLE" LocalizedPage="ADMIN_USERS" />
                                    </label>
                                    <asp:DropDownList ID="group" runat="server" CssClass="form-control" data-placeholder='<%# this.GetText("ADMIN_USERS", "FILTER_BY_GROUP") %>'>
                                    </asp:DropDownList>
                                </li>
                                <li class="form-group dropdown-item">
                                    <label for='<%= this.rank.ClientID %>'>
                                        <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="RANK" LocalizedPage="ADMIN_USERS" />
                                    </label>
                                    <asp:DropDownList ID="rank" runat="server" CssClass="form-control" data-placeholder='<%# this.GetText("ADMIN_USERS", "FILTER_BY_RANK") %>'>
                                    </asp:DropDownList>
                                </li>
                                <li class="form-group dropdown-item">
                                    <label for='<%= this.Since.ClientID %>'>
                                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="FILTER" LocalizedPage="ADMIN_USERS" />
                                    </label>
                                    <asp:DropDownList ID="Since" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="Since_SelectedIndexChanged"
                                        CssClass="form-control"/>
                                </li>
                                <li class="form-group dropdown-item">
                                    <label for='<%= this.SuspendedOnly.ClientID %>'>
                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SUSPENDED_ONLY" LocalizedPage="ADMIN_USERS" />
                                    </label>
                                    <asp:CheckBox CssClass="form-control" ID="SuspendedOnly" runat="server" AutoPostBack="True"
                                        OnSelectedIndexChanged="Since_SelectedIndexChanged"/>
                                  </li>
                            </ul>
                        </div>
                    </div>
                </div>
                <div class="card-block">
                                     <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                     <table class="table">
        <tr>
            <thead>
            <th colspan="4">
                <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="USER_NAME" LocalizedPage="ADMIN_USERS" />&nbsp;
                (<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="DISPLAY_NAME" LocalizedPage="ADMIN_USERS" />)
            </th>
            </thead>
        </tr>
        <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
            <ItemTemplate>
                <tr>
                    <td>
                        <asp:LinkButton ID="NameEdit" runat="server" CommandName="edit" CommandArgument='<%# this.Eval("UserID") %>'
                            Text='<%# this.HtmlEncode( this.Eval("Name").ToString() ) %>' />
                            <br />
                        (<asp:LinkButton ID="DisplayNameEdit" runat="server" CommandName="edit" CommandArgument='<%# this.Eval("UserID") %>'
                            Text='<%# this.HtmlEncode( this.Eval("DisplayName").ToString() ) %>' />)
                    </td>

                    <td>
                        <div>
                            <span style="font-weight:bold"><YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="EMAIL" LocalizedPage="ADMIN_USERS" /> :</span> <%# DataBinder.Eval(Container.DataItem,"Email") %>&nbsp;|&nbsp;
                            <span style="font-weight:bold"><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="RANK" /> </span> <%# this.Eval("RankName") %>
                            <span style="font-weight:bold"><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="POSTS" LocalizedPage="ADMIN_USERS" /> :</span> <%# this.Eval( "NumPosts") %>&nbsp;|&nbsp;
                            <span style="font-weight:bold"><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="LAST_VISIT" LocalizedPage="ADMIN_USERS" /> :</span> <%# this.Get<IDateTime>().FormatDateTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %>&nbsp;&nbsp;
                            <span id="FacebookUser" class="FacebookIcon" runat="server" Visible='<%# this.Eval("IsFacebookUser").ToType<bool>() %>' title='<%# this.GetText("ADMIN_EDITUSER", "FACEBOOK_USER_HELP") %>'>&nbsp;</span>
                            <span id="TwitterUser" class="TwitterIcon" runat="server" Visible='<%# this.Eval("IsTwitterUser").ToType<bool>() %>' title='<%# this.GetText("ADMIN_EDITUSER", "TWITTER_USER_HELP") %>'>&nbsp;</span>
                            <span id="GoogleUser" class="GoogleIcon" runat="server" Visible='<%# this.Eval("IsGoogleUser").ToType<bool>() %>' title='<%# this.GetText("ADMIN_EDITUSER", "GOOGLE_USER_HELP") %>'>&nbsp;</span>
                        </div>
                        <div style="padding-top:5px">
                            <span style="font-weight:bold"><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE_SUSPENDED" LocalizedPage="INFO" />&nbsp;:&nbsp;</span>
                            <asp:Label runat="server" ID="Suspended" CssClass='<%# this.Eval("Suspended") == DBNull.Value ? "tag tag-success" : "tag tag-danger" %>'><%# this.GetSuspendedString(this.Eval("Suspended").ToString())%></asp:Label>
                        </div>
                    </td>
                    <td>
                        <span class="pull-right">
                       <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="btn btn-info btn-sm" CommandName='edit' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'
                           TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" Icon="edit" runat="server"></YAF:ThemeButton>
                       <YAF:ThemeButton ID="ThemeButtonDelete" OnLoad="Delete_Load" CssClass="btn btn-danger btn-sm" CommandName='delete' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'
                           TextLocalizedTag="DELETE" TitleLocalizedTag="DELETE" Icon="trash" Visible='<%# DataBinder.Eval(Container.DataItem, "IsGuest").ToType<bool>() == false && !YAF.Classes.Config.IsDotNetNuke %>' runat="server"></YAF:ThemeButton>
                    </span>
                            </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
                         </table></div>
                </div>
                <div class="card-footer text-lg-center">
                <asp:PlaceHolder runat="server" ID="ImportAndSyncHolder">
                    <asp:LinkButton id="NewUser" OnClick="NewUser_Click" runat="server" CssClass="btn btn-primary"></asp:LinkButton>
                &nbsp;
                    <asp:LinkButton id="SyncUsers" OnClick="SyncUsers_Click" runat="server" CssClass="btn btn-secondary"></asp:LinkButton>
                &nbsp;
                    <asp:LinkButton id="ImportUsers" OnClick="ImportUsers_Click" runat="server" CssClass="btn btn-info"></asp:LinkButton>
                &nbsp;
                </asp:PlaceHolder>
                    <asp:LinkButton id="ExportUsersXml" OnClick="ExportUsersXml_Click" runat="server" CssClass="btn btn-warning"></asp:LinkButton>
                &nbsp;
                    <asp:LinkButton id="ExportUsersCsv" OnClick="ExportUsersCsv_Click" runat="server" CssClass="btn btn-warning"></asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
    </asp:PlaceHolder>
</YAF:AdminMenu>

<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000"
            OnTick="UpdateStatusTimer_Tick" />
    </ContentTemplate>
</asp:UpdatePanel>
<div>
    <div id="SyncUsersMessage" style="display: none" class="ui-overlay">
        <div class="ui-widget ui-widget-content ui-corner-all">
            <h2>
                Syncing Users</h2>
            <p>
                Please do not navigate away from this page while the sync is in progress...</p>
            <div align="center">
                <asp:Image ID="LoadingImage" runat="server" alt="Processing..." />
            </div>
            <br />
        </div>
    </div>
</div>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
