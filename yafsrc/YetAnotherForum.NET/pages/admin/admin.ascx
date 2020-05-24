<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.admin"
    CodeBehind="admin.ascx.cs" %>

<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
                <div class="row">
                <div class="col-xl-12">
                    <h1>Dashboard</h1>
                </div>
            </div>
    <asp:PlaceHolder ID="UpdateHightlight" runat="server" Visible="false">
        <YAF:Alert runat="server" Type="info">
            <YAF:Icon runat="server" IconName="box-open" IconType="text-info"></YAF:Icon>
            <YAF:LocalizedLabel runat="server"
                                LocalizedTag="NEW_VERSION"></YAF:LocalizedLabel>
            <YAF:ThemeButton ID="UpdateLinkHighlight" runat="server" 
                             TextLocalizedTag="UPGRADE_VERSION"
                             Type="Info"
                             Icon="cloud-download-alt"></YAF:ThemeButton>
        </YAF:Alert>
    </asp:PlaceHolder>
    <div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header form-inline">
                            <i class="fa fa-tachometer-alt fa-fw text-secondary pr-1"></i>
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                                LocalizedTag="HEADER3" 
                                                LocalizedPage="ADMIN_ADMIN" />&nbsp;
                            <asp:DropDownList ID="BoardStatsSelect" runat="server" 
                                              DataTextField="Name" 
                                              DataValueField="ID"
                                              OnSelectedIndexChanged="BoardStatsSelectChanged" 
                                              AutoPostBack="true" 
                                              CssClass="custom-select" 
                                              Width="300" />
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4 mb-xl-0">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-muted mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" 
                                                                            LocalizedTag="NUM_POSTS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 font-weight-bold mb-0">
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
                                            <p class="mt-3 mb-0 text-muted small">
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
                                    <div class="card mb-4 mb-xl-0">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-muted mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" 
                                                                            LocalizedTag="NUM_TOPICS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 font-weight-bold mb-0">
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
                                            <p class="mt-3 mb-0 text-muted small">
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
                                    <div class="card mb-4 mb-xl-0">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-muted mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" 
                                                                            LocalizedTag="NUM_USERS"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 font-weight-bold mb-0">
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
                                            <p class="mt-3 mb-0 text-muted small">
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
                            <div class="card mb-4 mb-xl-0">
                                <div class="card-body">
                                    <div class="row">
                                        <div class="col">
                                            <h5 class="card-title text-uppercase text-muted mb-0">
                                                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                                                    LocalizedTag="BOARD_STARTED"
                                                                    LocalizedPage="ADMIN_ADMIN" />
                                            </h5>
                                            <span class="h2 font-weight-bold mb-0">
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
                                    <p class="mt-3 mb-0 text-muted small">
                                        <span class="text-nowrap">
                                            <asp:Label ID="BoardStart" runat="server"></asp:Label>
                                        </span>
                                    </p>
                                </div>
                            </div>
                        </div>
                                <div class="col-xl-3 col-lg-6">
                                    <div class="card mb-4 mb-xl-0">
                                        <div class="card-body">
                                            <div class="row">
                                                <div class="col">
                                                    <h5 class="card-title text-uppercase text-muted mb-0">
                                                        <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                                            LocalizedTag="SIZE_DATABASE"
                                                                            LocalizedPage="ADMIN_ADMIN" />
                                                    </h5>
                                                    <span class="h2 font-weight-bold mb-0">
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
                        <div class="card-footer text-muted">
                            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server"
                                                LocalizedTag="STATS_DONTCOUNT" 
                                                LocalizedPage="ADMIN_ADMIN" />
                        </div>
                    </div>
             </div>
    </div>
    <p id="UpgradeNotice" runat="server" visible="false">
        <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" 
                            LocalizedTag="ADMIN_UPGRADE"
                            LocalizedPage="ADMIN_ADMIN" />
    </p>
<div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-users fa-fw text-secondary pr-1"></i>
                            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" 
                                                LocalizedTag="HEADER1" 
                                                LocalizedPage="ADMIN_ADMIN" />
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="ActiveList" runat="server">
                    <HeaderTemplate>
                        <div class="table-responsive">
                        <table class="table tablesorter table-bordered table-striped" id="ActiveUsers">
                            <thead class="thead-light">
                            <tr>
                                <th>
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                        LocalizedTag="ADMIN_NAME" LocalizedPage="ADMIN_ADMIN" />
                                </th>
                                <th>
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                        LocalizedTag="ADMIN_IPADRESS" LocalizedPage="ADMIN_ADMIN" />
                                </th>
                                <th>
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                        LocalizedTag="LOCATION" />
                                </th>
                                <th>
                                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                        LocalizedTag="BOARD_LOCATION" LocalizedPage="ADMIN_ADMIN" />
                                </th>
                            </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                            <tr>
                    <td>
                        <YAF:UserLink ID="ActiveUserLink" 
                                      UserID='<%# this.Eval("UserID") %>' 
                                      CrawlerName='<%# this.Eval("IsCrawler").ToType<int>() > 0 ? this.Eval("Browser").ToString() : string.Empty %>'
                                      Style='<%# this.Eval("Style") %>' runat="server" />
                    </td>
                    <td>
                        <a id="A1" href='<%# string.Format(this.Get<BoardSettings>().IPInfoPageURL, IPHelper.GetIp4Address(this.Eval("IP").ToString())) %>'
                            title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                            <%# IPHelper.GetIp4Address(this.Eval("IP").ToString())%></a>
                    </td>
                    <td>
                        <%# this.SetLocation(this.Eval("UserName").ToString())%>
                    </td>
                    <td>
                        <YAF:ActiveLocation ID="ActiveLocation2" 
                                            UserID='<%# (this.Eval("UserID") == DBNull.Value? 0 : this.Eval("UserID")).ToType<int>() %>' 
                                            UserName='<%# this.Eval("UserName") %>' 
                                            ForumPage='<%# this.Eval("ForumPage") %>' 
                                            ForumID='<%# (this.Eval("ForumID") == DBNull.Value? 0 : this.Eval("ForumID")).ToType<int>() %>'
                                            ForumName='<%# this.Eval("ForumName") %>' TopicID='<%# (this.Eval("TopicID") == DBNull.Value? 0 : this.Eval("TopicID")).ToType<int>() %>'
                                            TopicName='<%# this.Eval("TopicName") %>' LastLinkOnly="false" runat="server">
                        </YAF:ActiveLocation>
                    </td>
                            </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                        </div>
                        <div id="ActiveUsersPager" class=" tableSorterPager form-inline">
                            <select class="pagesize custom-select custom-select-sm">
		                        <option selected="selected" value="10">10</option>
		                        <option value="20">20</option>
                        	    <option value="30">30</option>
                        	    <option value="40">40</option>
                            </select>
                            &nbsp;
                            <div class="btn-group"  role="group">
                                <a href="#" class="first btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-left"></i></span></a>
                                <a href="#" class="prev btn btn-secondary btn-sm"><span><i class="fas fa-angle-left"></i></span></a>
                                <input type="button" class="pagedisplay  btn btn-secondary btn-sm disabled"  style="width:150px" />
                                <a href="#" class="next btn btn-secondary btn-sm"><span><i class="fas fa-angle-right"></i></span></a>
                                <a href="#" class="last btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-right"></i></span></a>
                            </div>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
    </div>
                   </div>
                 </div>
            </div>



    <asp:PlaceHolder runat="server" ID="UnverifiedUsersHolder">
        <div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-user-plus fa-fw text-secondary pr-1"></i>
                            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" 
                                                LocalizedTag="HEADER2" 
                                                LocalizedPage="ADMIN_ADMIN" />
                        </div>
                        <div class="card-body">
                                    <asp:Repeater ID="UserList" runat="server" 
                                                  OnItemCommand="UserListItemCommand">
            <HeaderTemplate>
                <div class="table-responsive">
                <table class="table tablesorter table-bordered table-striped" id="UnverifiedUsers">
                <thead class="thead-light">
                <tr>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADMIN_NAME"
                            LocalizedPage="ADMIN_ADMIN" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ADMIN_EMAIL"
                            LocalizedPage="ADMIN_ADMIN" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="LOCATION" />
                    </th>
                    <th>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="ADMIN_JOINED"
                            LocalizedPage="ADMIN_ADMIN" />
                    </th>
                </tr>
                    </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <YAF:UserLink ID="UnverifiedUserLink" 
                                      UserID='<%# this.Eval("UserID") %>' 
                                      Style='<%# this.Eval("Style") %>'
                            runat="server" />
                    </td>
                    <td>
                        <%# this.Eval("Email") %>
                    </td>
                    <td>
                        <%# this.SetLocation(this.Eval("Name").ToString())%>
                    </td>
                    <td>
                        <%# this.Get<IDateTime>().FormatDateTime((DateTime)this.Eval("Joined")) %>
                    </td>
                    <td>
                        <YAF:ThemeButton ID="Manage" runat="server"
                                         CssClass="dropdown-toggle"
                                         Type="Secondary"
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
                                         CommandArgument='<%# this.Eval("UserID") %>'
                                         Type="None"
                                         CssClass="dropdown-item"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_APPROVE") %>'
                                         Icon="check" 
                                         TextLocalizedTag="ADMIN_APPROVE">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server" 
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval("UserID") %>'
                                         Type="None"
                                         CssClass="dropdown-item" 
                                         ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="ADMIN_DELETE">
                        </YAF:ThemeButton>

					    </div>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
                </div>
                    <div id="UnverifiedUsersPager" class=" tableSorterPager form-inline">
                        <select class="pagesize custom-select custom-select-sm">
		                        <option selected="selected" value="10">10</option>
		                        <option value="20">20</option>
                        	    <option value="30">30</option>
                        	    <option value="40">40</option>
                            </select>
                            &nbsp;
                        <div class="btn-group"  role="group">
                            <a href="#" class="first  btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-left"></i></span></a>
                            <a href="#" class="prev  btn btn-secondary btn-sm"><span><i class="fas fa-angle-left"></i></span></a>
                            <input type="button" class="pagedisplay  btn btn-secondary btn-sm disabled"  style="width:150px" />
                            <a href="#" class="next btn btn-secondary btn-sm"><span><i class="fas fa-angle-right"></i></span></a>
                            <a href="#" class="last  btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-right"></i></span></a>
                        </div>
                    </div>
               </div>
                <div class="card-footer form-inline">
                    <YAF:ThemeButton runat="server" 
                                     CommandName="approveall" 
                                     Type="Primary" 
                                     Icon="check" 
                                     TextLocalizedTag="APROVE_ALL" 
                                     CssClass="mr-1"
                                     ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_APROVE_ALL") %>'/>
                    
                    <YAF:ThemeButton runat="server"
                                     CommandName="deleteall" 
                                     Type="Danger" 
                                     Icon="trash" 
                                     TextLocalizedTag="DELETE_ALL" 
                                     ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE_ALL") %>'
                                     CssClass="mr-1"/>
                    <asp:TextBox ID="DaysOld" runat="server" 
                                 MaxLength="5" 
                                 Text="14" 
                                 CssClass="form-control"
                                 TextMode="Number">
                    </asp:TextBox>
                </div>
            </FooterTemplate>
        </asp:Repeater>
                        </div>
                 </div>
          </div>
        </div>
    </asp:PlaceHolder>