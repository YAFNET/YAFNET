<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.admin"
    CodeBehind="admin.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
    <asp:PlaceHolder ID="UpdateHightlight" runat="server" Visible="false">
        <div class="ui-widget">
            <div class="ui-state-highlight ui-corner-all" style="margin-bottom: 20px; padding: 0 .7em;">
                <p>
                    <span class="ui-icon ui-icon-info" style="float: left; margin-right: .3em;"></span>
                    <asp:HyperLink ID="UpdateLinkHighlight" runat="server" Target="_blank"></asp:HyperLink>
                </p>
            </div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="UpdateWarning" runat="server" Visible="false">
			<div class="ui-state-error ui-corner-all" style="margin-bottom: 20px; padding: 0 .7em;"> 
				<p><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span> 
				<asp:HyperLink ID="UpdateLinkWarning" runat="server" Target="_blank"></asp:HyperLink>
			</div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="UnverifiedUsersHolder">
    <table width="100%" cellspacing="1" cellpadding="0" class="content">
        <tr>
             <td class="header1">
                        <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_ADMIN" />
                    </td>
        </tr>
        <tr>
            <td style="padding:0">
                
        <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
            <HeaderTemplate>
                <table style="width:100%"  cellspacing="1" cellpadding="0" class="tablesorter" id="UnverifiedUsers">
                <thead>
                <tr>
                    <th class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADMIN_NAME"
                            LocalizedPage="ADMIN_ADMIN" />
                    </th>
                    <th class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ADMIN_EMAIL"
                            LocalizedPage="ADMIN_ADMIN" />
                    </th>
                    <th class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="LOCATION" />
                    </th>
                    <th class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="ADMIN_JOINED"
                            LocalizedPage="ADMIN_ADMIN" />
                    </th>
                    <th class="header2">
                        &nbsp;
                    </th>
                </tr>
                    </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="post">
                        <YAF:UserLink ID="UnverifiedUserLink" UserID='<%# Eval("UserID") %>' Style='<%# Eval("Style") %>'
                            runat="server" />
                    </td>
                    <td class="post">
                        <%# Eval("Email") %>
                    </td>
                    <td class="post">
                        <%# this.SetLocation(Eval("Name").ToString())%>
                    </td>
                    <td class="post">
                        <%# this.Get<IDateTime>().FormatDateTime((DateTime)this.Eval("Joined")) %>
                    </td>
                    <td class="post">
                        <asp:LinkButton runat="server" CommandName="resendEmail" CommandArgument='<%# Eval("Email") + ";" + Eval("Name") %>' 
                            CssClass="yaflittlebutton">
                            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="ADMIN_RESEND_EMAIL"
                                LocalizedPage="ADMIN_ADMIN" />
                        </asp:LinkButton>
                        <asp:LinkButton OnLoad="Approve_Load" runat="server" CommandName="approve" CommandArgument='<%# Eval("UserID") %>'
                            CssClass="yaflittlebutton">
                            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="ADMIN_APPROVE"
                                LocalizedPage="ADMIN_ADMIN">
                            </YAF:LocalizedLabel>
                        </asp:LinkButton>
                        <asp:LinkButton OnLoad="Delete_Load" runat="server" CommandName="delete" CommandArgument='<%# Eval("UserID") %>' 
                            CssClass="yaflittlebutton">
                            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="ADMIN_DELETE"
                                LocalizedPage="ADMIN_ADMIN" />
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                </table>
                <div id="UnverifiedUsersPager" class=" tableSorterPager">
                        <a href="#" class="first pagelink"><span>&lt;&lt;</span></a>
                        <a href="#" class="prev pagelink"><span>&lt;</span></a>
                        <input type="text" class="pagedisplay"/>
                        <a href="#" class="next pagelink"><span>&gt;</span></a>
                        <a href="#" class="last pagelink"><span>&gt;&gt;</span></a>
                        <select class="pagesize">
		                    <option selected="selected"  value="10">10</option>
		                    <option value="20">20</option>
                        	<option value="30">30</option>
                        	<option  value="40">40</option>
                        </select>
                    </div>
                </td>
                <tr>
                    <td class="footer1">
                        <asp:Button OnLoad="ApproveAll_Load" CommandName="approveall" CssClass="pbutton"
                            runat="server" />
                        <asp:Button OnLoad="DeleteAll_Load" CommandName="deleteall" CssClass="pbutton" runat="server" />
                        <asp:TextBox ID="DaysOld" runat="server" MaxLength="5" Text="14" CssClass="Numeric" type="number"></asp:TextBox>
                    </td>
                </tr>
            </FooterTemplate>
        </asp:Repeater>
    </table>
    &nbsp;<br />
    </asp:PlaceHolder>
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="4">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_ADMIN" />
                <span runat="server" id="boardSelector" visible='<%# this.PageContext.IsHostAdmin %>'>
                    <asp:DropDownList ID="BoardStatsSelect" runat="server" DataTextField="Name" DataValueField="BoardID"
                        OnSelectedIndexChanged="BoardStatsSelect_Changed" AutoPostBack="true" CssClass="standardSelectMenu" />
                </span>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="25%">
                <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="NUM_POSTS"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post" width="25%">
                <asp:Label ID="NumPosts" runat="server"></asp:Label>
            </td>
            <td class="postheader" width="25%">
                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="POSTS_DAY"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post" width="25%">
                <asp:Label ID="DayPosts" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="NUM_TOPICS"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post">
                <asp:Label ID="NumTopics" runat="server"></asp:Label>
            </td>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="TOPICS_DAY"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post">
                <asp:Label ID="DayTopics" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="NUM_USERS"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post">
                <asp:Label ID="NumUsers" runat="server"></asp:Label>
            </td>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="USERS_DAY"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post">
                <asp:Label ID="DayUsers" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="BOARD_STARTED"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post">
                <asp:Label ID="BoardStart" runat="server"></asp:Label>
            </td>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="SIZE_DATABASE"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
            <td class="post">
                <asp:Label ID="DBSize" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="postfooter" colspan="4">
                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="STATS_DONTCOUNT"
                    LocalizedPage="ADMIN_ADMIN" />
            </td>
        </tr>
    </table>
    <p id="UpgradeNotice" runat="server" visible="false">
        <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="ADMIN_UPGRADE"
            LocalizedPage="ADMIN_ADMIN" />
    </p>
    &nbsp;<br />
    <table width="100%" cellspacing="1" cellpadding="0" class="content">
        <tr>
            <td class="header1" colspan="4">
                <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_ADMIN" />
            </td>
        </tr>
        <tr>
            <td style="padding:0">
                <asp:Repeater ID="ActiveList" runat="server">
                    <HeaderTemplate>
                        <table style="width:100%"  cellspacing="1" cellpadding="0" class="tablesorter" id="ActiveUsers">
                            <thead>
                            <tr>
                                <th class="header2">
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                        LocalizedTag="ADMIN_NAME" LocalizedPage="ADMIN_ADMIN" />
                                </th>
                                <th class="header2">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                        LocalizedTag="ADMIN_IPADRESS" LocalizedPage="ADMIN_ADMIN" />
                                </th>
                                <th class="header2">
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                        LocalizedTag="LOCATION" />
                                </th>
                                <th class="header2">
                                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                        LocalizedTag="BOARD_LOCATION" LocalizedPage="ADMIN_ADMIN" />
                                </th>
                            </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                            <tr>
                    <td class="post">
                        <YAF:UserLink ID="ActiveUserLink" UserID='<%# Eval("UserID") %>' CrawlerName='<%# this.Eval("IsCrawler").ToType<int>() > 0 ? Eval("Browser").ToString() : String.Empty %>'
                            Style='<%# Eval("Style") %>' runat="server" />
                    </td>
                    <td class="post">
                        <a id="A1" href='<%# this.Get<YafBoardSettings>().IPInfoPageURL.FormatWith(IPHelper.GetIp4Address(this.Eval("IP").ToString())) %>'
                            title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                            <%# IPHelper.GetIp4Address(Eval("IP").ToString())%></a>
                    </td>
                    <td class="post">
                        <%# this.SetLocation(Eval("UserName").ToString())%>
                    </td>
                    <td class="post">
                        <YAF:ActiveLocation ID="ActiveLocation2" UserID='<%# ((this.Eval("UserID") == DBNull.Value)? 0 : this.Eval("UserID")).ToType<int>() %>'
                            UserName='<%# Eval("UserName") %>' ForumPage='<%# Eval("ForumPage") %>' ForumID='<%# ((this.Eval("ForumID") == DBNull.Value)? 0 : this.Eval("ForumID")).ToType<int>() %>'
                            ForumName='<%# Eval("ForumName") %>' TopicID='<%# ((this.Eval("TopicID") == DBNull.Value)? 0 : this.Eval("TopicID")).ToType<int>() %>'
                            TopicName='<%# Eval("TopicName") %>' LastLinkOnly="false" runat="server">
                        </YAF:ActiveLocation>
                    </td>
                            </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                        <div id="ActiveUsersPager" class=" tableSorterPager">
                        <a href="#" class="first pagelink"><span>&lt;&lt;</span></a>
                        <a href="#" class="prev pagelink"><span>&lt;</span></a>
                        <input type="text" class="pagedisplay"/>
                        <a href="#" class="next pagelink"><span>&gt;</span></a>
                        <a href="#" class="last pagelink"><span>&gt;&gt;</span></a>
                        <select class="pagesize">
		                    <option selected="selected"  value="10">10</option>
		                    <option value="20">20</option>
                        	<option value="30">30</option>
                        	<option  value="40">40</option>
                        </select>
                    </div>
                    </td>
                    </FooterTemplate>
                </asp:Repeater>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
