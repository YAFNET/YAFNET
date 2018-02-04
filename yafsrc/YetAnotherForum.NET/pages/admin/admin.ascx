<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.admin"
    CodeBehind="admin.ascx.cs" %>

<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
                <div class="row">
                <div class="col-xl-12">
                    <h1>Dashboard</h1>
                </div>
            </div>
    <asp:PlaceHolder ID="UpdateHightlight" runat="server" Visible="false">
        <div class="alert alert-info" role="alert">
            <asp:HyperLink ID="UpdateLinkHighlight" runat="server" Target="_blank"></asp:HyperLink>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="UpdateWarning" runat="server" Visible="false">
        <div class="alert alert-danger" role="alert">
            <asp:HyperLink ID="UpdateLinkWarning" runat="server" Target="_blank"></asp:HyperLink>
        </div>
    </asp:PlaceHolder>
    <div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header form-inline">
                            <i class="fa fa-tachometer-alt fa-fw"></i> <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_ADMIN" />&nbsp;
                <span runat="server" id="boardSelector" visible='<%# this.PageContext.IsHostAdmin %>'>
                    <asp:DropDownList ID="BoardStatsSelect" runat="server" DataTextField="Name" DataValueField="BoardID"
                        OnSelectedIndexChanged="BoardStatsSelectChanged" AutoPostBack="true" CssClass="custom-select" Width="300" />
                </span>
                        </div>
                        <div class="card-body">
                            <div class="card-columns">
                    <div class="card mb-3 text-white bg-primary">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-comment fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="NUM_POSTS"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"><asp:Label ID="NumPosts" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                     <div class="card mb-3 text-white bg-primary">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-comment fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="POSTS_DAY"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"><asp:Label ID="DayPosts" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>
                     <div class="card mb-3 text-white bg-primary">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-comments fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="NUM_TOPICS"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"><asp:Label ID="NumTopics" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-3 text-white bg-primary">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-comments fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="TOPICS_DAY"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"><asp:Label ID="DayTopics" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-3 text-white bg-success">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-user fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="NUM_USERS"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"> <asp:Label ID="NumUsers" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-3 text-white bg-success">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-users fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="USERS_DAY"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"><asp:Label ID="DayUsers" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-3 text-white bg-warning">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-life-ring fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div> <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="BOARD_STARTED"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"><asp:Label ID="BoardStart" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card mb-3  text-white bg-danger">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-sm-3">
                                    <i class="fa fa-database fa-3x"></i>
                                </div>
                                <div class="col-sm-9 text-right">
                                    <div><YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="SIZE_DATABASE"
                    LocalizedPage="ADMIN_ADMIN" /></div>
                                    <h4 class="card-title"> <asp:Label ID="DBSize" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                    </div>

                            </div>

                        </div>

                        <div class="card-footer text-muted">
                            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server"
                                LocalizedTag="STATS_DONTCOUNT" LocalizedPage="ADMIN_ADMIN" />
                        </div>

                 </div>
            </div>
    </div>
    <p id="UpgradeNotice" runat="server" visible="false">
        <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="ADMIN_UPGRADE"
            LocalizedPage="ADMIN_ADMIN" />
    </p>



    <div class="row">
             <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header">
                            <i class="fa fa-users fa-fw"></i> <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_ADMIN" />
                        </div>
                        <div class="card-body">
                            <asp:Repeater ID="ActiveList" runat="server">
                    <HeaderTemplate>
                        <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div>
                        <div class="table-responsive">
                        <table class="table tablesorter" id="ActiveUsers">
                            <thead>
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
                        <YAF:UserLink ID="ActiveUserLink" UserID='<%# this.Eval("UserID") %>' CrawlerName='<%# this.Eval("IsCrawler").ToType<int>() > 0 ? this.Eval("Browser").ToString() : String.Empty %>'
                            Style='<%# this.Eval("Style") %>' runat="server" />
                    </td>
                    <td>
                        <a id="A1" href='<%# this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(IPHelper.GetIp4Address(this.Eval("IP").ToString())) %>'
                            title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                            <%# IPHelper.GetIp4Address(this.Eval("IP").ToString())%></a>
                    </td>
                    <td>
                        <%# this.SetLocation(this.Eval("UserName").ToString())%>
                    </td>
                    <td>
                        <YAF:ActiveLocation ID="ActiveLocation2" UserID='<%# ((this.Eval("UserID") == DBNull.Value)? 0 : this.Eval("UserID")).ToType<int>() %>'
                            UserName='<%# this.Eval("UserName") %>' ForumPage='<%# this.Eval("ForumPage") %>' ForumID='<%# ((this.Eval("ForumID") == DBNull.Value)? 0 : this.Eval("ForumID")).ToType<int>() %>'
                            ForumName='<%# this.Eval("ForumName") %>' TopicID='<%# ((this.Eval("TopicID") == DBNull.Value)? 0 : this.Eval("TopicID")).ToType<int>() %>'
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
                            <select class="pagesize custom-select">
		                        <option selected="selected"  value="10">10</option>
		                        <option value="20">20</option>
                        	    <option value="30">30</option>
                        	    <option  value="40">40</option>
                            </select>
                            &nbsp;
                            <div class="btn-group"  role="group">
                                <a href="#" class="first  btn btn-secondary btn-sm"><span>&lt;&lt;</span></a>
                                <a href="#" class="prev  btn btn-secondary btn-sm"><span>&lt;</span></a>
                                <input type="text" class="pagedisplay  btn btn-secondary btn-sm"  style="width:150px" />
                                <a href="#" class="next btn btn-secondary btn-sm"><span>&gt;</span></a>
                                <a href="#" class="last  btn btn-secondary btn-sm"><span>&gt;&gt;</span></a>
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
                            <i class="fa fa-user-plus fa-fw"></i> <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_ADMIN" />
                        </div>
                        <div class="card-body">
                                    <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserListItemCommand">
            <HeaderTemplate>
                                        <div class="alert alert-info d-sm-none" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="float-right"><i class="fa fa-hand-point-left fa-fw"></i></span>
                        </div>
                <div class="table-responsive">
                <table class="table tablesorter" id="UnverifiedUsers">
                <thead>
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
                    <th>
                        &nbsp;
                    </th>
                </tr>
                    </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <YAF:UserLink ID="UnverifiedUserLink" UserID='<%# this.Eval("UserID") %>' Style='<%# this.Eval("Style") %>'
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
					    <span class="float-right">
                        <YAF:ThemeButton runat="server" CommandName="resendEmail" CommandArgument='<%# this.Eval("Email") + ";" + this.Eval("Name") %>' 
                            Icon="share" TextLocalizedTag="ADMIN_RESEND_EMAIL"
                            Type="Info" CssClass="btn-sm">
                        </YAF:ThemeButton>&nbsp;
                        <YAF:ThemeButton runat="server" CommandName="approve" CommandArgument='<%# this.Eval("UserID") %>'
                            Type="Primary" CssClass="btn-sm" ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_APPROVE") %>'
                            Icon="check" TextLocalizedTag="ADMIN_APPROVE">
                        </YAF:ThemeButton>&nbsp;
                        <YAF:ThemeButton runat="server" CommandName="delete" CommandArgument='<%# this.Eval("UserID") %>'
                            Type="Danger" CssClass="btn-sm" ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE_ALL") %>'
                            Icon="trash" TextLocalizedTag="ADMIN_DELETE">
                        </YAF:ThemeButton>

					    </span>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
                </div>
                    <div id="UnverifiedUsersPager" class=" tableSorterPager form-inline">
                        <select class="pagesize custom-select">
		                        <option selected="selected"  value="10">10</option>
		                        <option value="20">20</option>
                        	    <option value="30">30</option>
                        	    <option  value="40">40</option>
                            </select>
                            &nbsp;
                            <div class="btn-group"  role="group">
                                <a href="#" class="first  btn btn-secondary btn-sm"><span>&lt;&lt;</span></a>
                                <a href="#" class="prev  btn btn-secondary btn-sm"><span>&lt;</span></a>
                                <input type="text" class="pagedisplay  btn btn-secondary btn-sm"  style="width:150px" />
                                <a href="#" class="next btn btn-secondary btn-sm"><span>&gt;</span></a>
                                <a href="#" class="last  btn btn-secondary btn-sm"><span>&gt;&gt;</span></a>
                            </div>
                    </div>
               </div>
                <div class="card-footer form-inline">
                    <YAF:ThemeButton CommandName="approveall" Type="Primary" CssClass="btn-sm" 
                        Icon="check" TextLocalizedTag="APROVE_ALL" ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_APROVE_ALL") %>'
                        runat="server"/>&nbsp;
                    <YAF:ThemeButton CommandName="deleteall" Type="Danger" CssClass="btn-sm" 
                        Icon="trash" TextLocalizedTag="DELETE_ALL" ReturnConfirmText='<%# this.GetText("ADMIN_ADMIN", "CONFIRM_DELETE_ALL") %>'
                        runat="server" />&nbsp;
                    <asp:TextBox ID="DaysOld" runat="server" MaxLength="5" Text="14" CssClass="form-control Numeric" 
                        TextMode="Number"></asp:TextBox>
                </div>
            </FooterTemplate>
        </asp:Repeater>


                        </div>
                 </div>
          </div>
        </div>
    </asp:PlaceHolder>



</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
