﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.users"
    CodeBehind="users.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Core.BBCode" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
    <table cellspacing="0" cellpadding="0" class="content" width="100%">
        <tr>
            <td class="header1" colspan="4">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_USERS" />
            </td>
        </tr>
        <tr class="header2">
            <td>
                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="ROLE" LocalizedPage="ADMIN_USERS" />
            </td>
            <td>
                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="RANK" LocalizedPage="ADMIN_USERS" />
            </td>
            <td>
                <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="NAME_CONTAINS" LocalizedPage="ADMIN_USERS" />
            </td>
            <td>
                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="EMAIL_CONTAINS" LocalizedPage="ADMIN_USERS" />
            </td>
        </tr>
        <tr class="post">
            <td>
                <asp:DropDownList ID="group" runat="server" Width="95%">
                </asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="rank" runat="server" Width="95%">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="name" runat="server" Width="95%"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="Email" runat="server" Width="95%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="post" colspan="3" align="right">
                <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="FILTER" LocalizedPage="ADMIN_USERS" />
            </td>
            <td>
                <asp:DropDownList ID="Since" runat="server" Width="95%" AutoPostBack="True" OnSelectedIndexChanged="Since_SelectedIndexChanged" />
            </td>
        </tr>
        <tr>
            <td class="footer1" colspan="4" align="center">
                <asp:Button ID="search" runat="server" OnClick="Search_Click" CssClass="pbutton"></asp:Button>
            </td>
        </tr>
    </table>
    <br />
    <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" UsePostBack="True" />
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="10">
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_USERS" />
            </td>
        </tr>
        <tr>
            <td class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="USER_NAME" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="DISPLAY_NAME" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="EMAIL" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
               <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="RANK" />
            </td>
            <td class="header2" align="center">
                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="POSTS" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2" align="center">
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="APPROVED" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="LAST_VISIT" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="FACEBOOK_USER" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TWITTER_USER" LocalizedPage="ADMIN_USERS" />
            </td>
            <td class="header2">
                &nbsp;
            </td>
        </tr>
        <asp:Repeater ID="UserList" runat="server" OnItemCommand="UserList_ItemCommand">
            <ItemTemplate>
                <tr>
                    <td class="post">
                        <asp:LinkButton ID="NameEdit" runat="server" CommandName="edit" CommandArgument='<%# Eval("UserID") %>'
                            Text='<%# this.HtmlEncode( Eval("Name").ToString() ) %>' />
                    </td>
                    <td class="post">
                        <asp:LinkButton ID="DisplayNameEdit" runat="server" CommandName="edit" CommandArgument='<%# Eval("UserID") %>'
                            Text='<%# this.HtmlEncode( Eval("DisplayName").ToString() ) %>' />
                    </td>
                    <td class="post">
                        <%# DataBinder.Eval(Container.DataItem,"Email") %>
                    </td>
                    <td class="post">
                        <%# Eval("RankName") %>
                    </td>
                    <td class="post" align="center">
                        <%# Eval( "NumPosts") %>
                    </td>
                    <td class="post" align="center">
                        <%# BitSet(Eval( "Flags"),2) %>
                    </td>
                    <td class="post">
                        <%# this.Get<IDateTime>().FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %>
                    </td>
                    <td class="post">
                        <%# Eval("IsFacebookUser")%>
                    </td>
                    <td class="post">
                        <%# Eval("IsTwitterUser")%>
                    </td>
                    <td class="post" align="center">
                       <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>
                       <YAF:ThemeButton ID="ThemeButtonDelete" OnLoad="Delete_Load" CssClass="yaflittlebutton" CommandName='delete' CommandArgument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' TitleLocalizedTag="DELETE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server"></YAF:ThemeButton>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr>
            <td class="footer1" colspan="9" align="center">
                <strong>
                    <asp:Button id="NewUser" OnClick="NewUser_Click" runat="server" CssClass="pbutton"></asp:Button></strong>
                | 
                    <asp:Button id="SyncUsers" OnClick="SyncUsers_Click" runat="server" CssClass="pbutton"></asp:Button>
                |
                    <asp:Button id="ImportUsers" OnClick="ImportUsers_Click" runat="server" CssClass="pbutton"></asp:Button>
                |
                    <asp:Button id="ExportUsersXml" OnClick="ExportUsersXml_Click" runat="server" CssClass="pbutton"></asp:Button>
                |
                    <asp:Button id="ExportUsersCsv" OnClick="ExportUsersCsv_Click" runat="server" CssClass="pbutton"></asp:Button>
            </td>
        </tr>
    </table>
    <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" UsePostBack="True" />
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
