<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Admin"
    CodeBehind="Admin.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Core.Helpers" %>
<%@ Import Namespace="ServiceStack.Text" %>

<YAF:PageLinks ID="PageLinks" runat="server" />
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel18" runat="server"
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_ADMIN" /></h1>
    </div>
</div>
    <asp:PlaceHolder ID="UpdateHightlight" runat="server" Visible="false">
        <YAF:Alert runat="server" Type="info">
            <h6 class="alert-heading">
                <YAF:Icon runat="server" IconName="box-open" IconType="text-info"></YAF:Icon>
                <YAF:LocalizedLabel runat="server"
                                    LocalizedTag="NEW_VERSION"></YAF:LocalizedLabel>
                <YAF:ThemeButton ID="UpdateLinkHighlight" runat="server"
                                 TextLocalizedTag="UPGRADE_VERSION"
                                 Type="Info"
                                 Icon="cloud-download-alt"></YAF:ThemeButton>
            </h6>
        </YAF:Alert>
    </asp:PlaceHolder>

<div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header">
                            <div class="row row-cols-md-auto align-items-center">
                                <div class="col-12">
                                    <YAF:Icon runat="server"
                                              IconName="tachometer-alt"/>
                                </div>
                                <div class="col-12">
                                    <div class="input-group">
                                        <div class="input-group-text" id="btnGroupAddon">
                                            <YAF:LocalizedLabel runat="server"
                                                                LocalizedTag="HEADER3"
                                                                LocalizedPage="ADMIN_ADMIN" />
                                        </div>
                                        <asp:DropDownList ID="BoardStatsSelect" runat="server"
                                                          DataTextField="Name"
                                                          DataValueField="ID"
                                                          OnSelectedIndexChanged="BoardStatsSelectChanged"
                                                          AutoPostBack="true"
                                                          CssClass="form-select" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                                                            LocalizedTag="NUM_CATEGORIES"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 fw-bold mb-0">
                                                        <asp:Label ID="NumCategories" runat="server"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="col-auto">
                                                    <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                        <i class="fas fa-circle fa-stack-2x text-info"></i>
                                                        <i class="fas fa-folder fa-stack-1x fa-inverse"></i>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                                            LocalizedTag="NUM_FORUMS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 fw-bold mb-0">
                                                        <asp:Label ID="NumForums" runat="server"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="col-auto">
                                                    <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                        <i class="fas fa-circle fa-stack-2x text-dark"></i>
                                                        <i class="fas fa-comments fa-stack-1x fa-inverse"></i>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server"
                                                                            LocalizedTag="NUM_TOPICS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 fw-bold mb-0">
                                                        <asp:Label ID="NumTopics" runat="server"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="col-auto">
                                                    <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                        <i class="fas fa-circle fa-stack-2x text-secondary"></i>
                                                        <i class="fas fa-comments fa-stack-1x fa-inverse"></i>
                                                    </span>
                                                </div>
                                            </div>
                                            <p class="mt-3 mb-0 text-body-secondary small">
                                                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server"
                                                                    LocalizedTag="TOPICS_DAY"
                                                                    LocalizedPage="ADMIN_ADMIN" />
                                                <span class="text-nowrap">
                                                    <asp:Label ID="DayTopics" runat="server"></asp:Label>
                                                </span>
                                            </p>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server"
                                                                            LocalizedTag="NUM_POSTS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 fw-bold mb-0">
                                                        <asp:Label ID="NumPosts" runat="server"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="col-auto">
                                                    <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                        <i class="fas fa-circle fa-stack-2x text-primary"></i>
                                                        <i class="fas fa-comment fa-stack-1x fa-inverse"></i>
                                                    </span>
                                                </div>
                                            </div>
                                            <p class="mt-3 mb-0 text-body-secondary small">
                                                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server"
                                                                    LocalizedTag="POSTS_DAY"
                                                                    LocalizedPage="ADMIN_ADMIN" />
                                                <span class="text-nowrap">
                                                    <asp:Label ID="DayPosts" runat="server"></asp:Label>
                                                </span>
                                            </p>
                                        </div>
                                    </div>
                                </div>
                            <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server"
                                                                            LocalizedTag="NUM_USERS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 fw-bold mb-0">
                                                        <asp:Label ID="NumUsers" runat="server"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="col-auto">
                                                    <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                        <i class="fas fa-circle fa-stack-2x text-success"></i>
                                                        <i class="fas fa-users fa-stack-1x fa-inverse"></i>
                                                    </span>
                                                </div>
                                            </div>
                                            <p class="mt-3 mb-0 text-body-secondary small">
                                                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server"
                                                                    LocalizedTag="USERS_DAY"
                                                                    LocalizedPage="ADMIN_ADMIN" />
                                                <span class="text-nowrap">
                                                    <asp:Label ID="DayUsers" runat="server"></asp:Label>
                                                </span>
                                            </p>
                                        </div>
                                    </div>
                                </div>
                        <div class="col-xl-3 col-lg-6">
                            <div class="card mb-4">
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col">
                                            <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server"
                                                                    LocalizedTag="BOARD_STARTED"
                                                                    LocalizedPage="ADMIN_ADMIN" />
                                            </h5>
                                            <span class="h2 fw-bold mb-0">
                                                <asp:Label ID="BoardStartAgo" runat="server"></asp:Label>
                                            </span>
                                        </div>
                                        <div class="col-auto">
                                            <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                <i class="fas fa-circle fa-stack-2x text-warning"></i>
                                                <i class="fas fa-globe fa-stack-1x fa-inverse"></i>
                                            </span>
                                        </div>
                                    </div>
                                    <p class="mt-3 mb-0 text-body-secondary small">
                                        <span class="text-nowrap">
                                            <asp:Label ID="BoardStart" runat="server"></asp:Label>
                                        </span>
                                    </p>
                                </div>
                            </div>
                        </div>
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-body-secondary mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server"
                                                                            LocalizedTag="SIZE_DATABASE"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 fw-bold mb-0">
                                                        <asp:Label ID="DBSize" runat="server"></asp:Label>
                                                    </span>
                                                </div>
                                                <div class="col-auto">
                                                    <span class="fa-stack fa-2x" style="vertical-align: top;">
                                                        <i class="fas fa-circle fa-stack-2x text-danger"></i>
                                                        <i class="fas fa-database fa-stack-1x fa-inverse"></i>
                                                    </span>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            </div>
                        <div class="card-footer text-body-secondary">
                            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server"
                                                LocalizedTag="STATS_DONTCOUNT"
                                                LocalizedPage="ADMIN_ADMIN" />
                        </div>
                    </div>
             </div>
    </div>

<div class="row">
    <div class="col">
                    <div class="card mb-3">
                        <div class="card-header">
                            <div class="row justify-content-between align-items-center">
                                <div class="col-auto">
                                    <YAF:IconHeader runat="server"
                                                    IconName="users"
                                                    LocalizedTag="HEADER1"
                                                    LocalizedPage="ADMIN_ADMIN"></YAF:IconHeader>
                                </div>
                                <div class="col-auto">
                                    <div class="input-group input-group-sm me-2" role="group">
                                        <div class="input-group-text">
                                            <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                        </div>
                                        <asp:DropDownList runat="server" ID="PageSize"
                                                          AutoPostBack="True"
                                                          OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                          CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="ActiveList" runat="server">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item list-group-item-action  d-flex justify-content-between align-items-start">
                        <div class="align-items-baseline">
                            <div class="me-2">
                                <YAF:UserLabel ID="ActiveUserLink"
                                               ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as ActiveUser).UserDisplayName : (Container.DataItem as ActiveUser).UserName %>"
                                               UserID="<%# (Container.DataItem as ActiveUser).UserID %>"
                                               CrawlerName="<%# (Container.DataItem as ActiveUser).ActiveFlags.IsCrawler ? (Container.DataItem as ActiveUser).Browser : string.Empty %>"
                                               Style="<%# (Container.DataItem as ActiveUser).UserStyle %>" runat="server" />
                            </div>
                            <div class="me-2">
                                <span class="fw-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                                        LocalizedTag="ADMIN_IPADRESS" LocalizedPage="ADMIN_ADMIN" />
                                </span>
                                <a id="A1" href="<%# string.Format(this.PageBoardContext.BoardSettings.IPInfoPageURL, IPHelper.GetIpAddressAsString((Container.DataItem as ActiveUser).IP)) %>"
                                   title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                                    <%# IPHelper.GetIpAddressAsString((Container.DataItem as ActiveUser).IP)%></a>
                            </div>
                            <div>
                                <span class="fw-bold">
                                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                                        LocalizedTag="BOARD_LOCATION" LocalizedPage="ADMI<N_ADMIN" />
                                </span>
                                <YAF:ActiveLocation ID="ActiveLocation2"
                                                    UserID="<%# (Container.DataItem as ActiveUser).UserID %>"
                                                    Location="<%#(Container.DataItem as ActiveUser).Location %>"
                                                    ForumPage="<%# (Container.DataItem as ActiveUser).ForumPage %>"
                                                    ForumID="<%# (Container.DataItem as ActiveUser).ForumID ?? 0 %>"
                                                    ForumName="<%# (Container.DataItem as ActiveUser).ForumName %>"
                                                    TopicID="<%# (Container.DataItem as ActiveUser).TopicID ?? 0 %>"
                                                    TopicName="<%# (Container.DataItem as ActiveUser).TopicName %>" LastLinkOnly="false" runat="server">
                                </YAF:ActiveLocation>
                            </div>
                        </div>
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
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTopChange" />
    </div>
</div>

    <asp:PlaceHolder runat="server" ID="UnverifiedUsersHolder">
        <div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header">
                            <div class="row justify-content-between align-items-center">
                                <div class="col-auto">
                                    <YAF:IconHeader runat="server"
                                                    IconName="user-plus"
                                                    LocalizedTag="HEADER2"
                                                    LocalizedPage="ADMIN_ADMIN"></YAF:IconHeader>
                                </div>
                                <div class="col-auto">
                                    <div class="input-group input-group-sm me-2" role="group">
                                        <div class="input-group-text">
                                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SHOW" />:
                                        </div>
                                        <asp:DropDownList runat="server" ID="PageSizeUnverified"
                                                          AutoPostBack="True"
                                                          OnSelectedIndexChanged="PageSizeUnverifiedSelectedIndexChanged"
                                                          CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="UserList" runat="server"
                                          OnItemCommand="UserListItemCommand">
                                <HeaderTemplate>
                                    <ul class="list-group">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="list-group-item list-group-item-action d-flex justify-content-between align-items-start">
                                    <div class="align-items-baseline">
                                        <div class="me-2">
                                            <span class="fw-bold">
                                                <%# this.Eval(this.PageBoardContext.BoardSettings.EnableDisplayName ? "DisplayName" : "Name") %>
                                            </span>
                                        </div>
                                        <div class="me-2">
                                            <span class="fw-bold">
                                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="ADMIN_JOINED"
                                                                    LocalizedPage="ADMIN_ADMIN" />:
                                            </span>
                                            <%# this.Get<IDateTimeService>().FormatDateTime((DateTime)this.Eval("Joined")) %>
                                        </div>
                                        <div>
                                            <span class="fw-bold">
                                                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ADMIN_EMAIL"
                                                                    LocalizedPage="ADMIN_ADMIN" />:
                                            </span>
                                            <%# this.Eval("Email") %>
                                        </div>
                                    </div>
                <small>
                   <YAF:ThemeButton ID="Manage" runat="server"
                                    CssClass="dropdown-toggle"
                                    Type="Secondary"
                                    Size="Small"
                                    DataToggle="dropdown"
                                    Icon="ellipsis-v" />
                        <div class="dropdown-menu">
                        <YAF:ThemeButton runat="server"
                                         CommandName="resendEmail"
                                         CommandArgument='<%# "{0};{1}".Fmt(this.Eval("Email"), this.Eval("Name")) %>'
                                         Icon="share"
                                         TextLocalizedTag="ADMIN_RESEND_EMAIL"
                                         Type="None"
                                         CssClass="dropdown-item">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server"
                                         CommandName="approve"
                                         CommandArgument='<%# this.Eval("ID") %>'
                                         Type="None"
                                         CssClass="dropdown-item"
                                         Icon="check"
                                         TextLocalizedTag="ADMIN_APPROVE">
                        </YAF:ThemeButton>
                            <YAF:ThemeButton runat="server"
                                             CommandName="edit"
                                             CommandArgument='<%# this.Eval("ID") %>'
                                             Type="None"
                                             CssClass="dropdown-item"
                                             Icon="user-edit"
                                             TextLocalizedTag="EDITUSER">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton runat="server"
                                         CommandName="delete"
                                         CommandArgument='<%# this.Eval("ID") %>'
                                         Type="None"
                                         CssClass="dropdown-item"
                                         ReturnConfirmTag="CONFIRM_DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="ADMIN_DELETE">
                        </YAF:ThemeButton>

                        </div>
                </small>
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate>
                                </ul>
                                <YAF:Alert runat="server" ID="NoInfo"
                                           Type="success"
                                           Visible="<%# this.UserList.Items.Count == 0 %>">
                                    <YAF:Icon runat="server" IconName="check" />
                                    <YAF:LocalizedLabel runat="server"
                                                        LocalizedTag="NO_ENTRY"></YAF:LocalizedLabel>
                                </YAF:Alert>
                                </div>
                                <div class="card-footer" runat="server" Visible="<%# this.UserList.Items.Count != 0 %>">
                                    <div class="d-lg-flex">
                                        <div>
                                            <YAF:ThemeButton runat="server"
                                                             CommandName="approveall"
                                                             Type="Primary"
                                                             Icon="check"
                                                             TextLocalizedTag="APROVE_ALL"
                                                             CssClass="mb-1"
                                                             ReturnConfirmTag="CONFIRM_APROVE_ALL"/>
                                            <YAF:ThemeButton runat="server"
                                                             CommandName="deleteall"
                                                             Type="Danger"
                                                             Icon="trash"
                                                             TextLocalizedTag="DELETE_ALL"
                                                             ReturnConfirmTag="CONFIRM_DELETE_ALL"
                                                             CssClass="me-1 mb-1"/>
                                        </div>
                                        <div>
                                            <div class="input-group">
                                                <asp:TextBox ID="DaysOld" runat="server"
                                                             MaxLength="5"
                                                             Text="14"
                                                             CssClass="form-control"
                                                             TextMode="Number">
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                </FooterTemplate>
                            </asp:Repeater>
                            
                        </div>
                 </div>
          </div>
        </div>
    <div class="row justify-content-end">
        <div class="col-auto">
            <YAF:Pager ID="PagerUnverified" runat="server"
                       OnPageChange="PagerUnverifiedChange" />
        </div>
    </div>
    </asp:PlaceHolder>