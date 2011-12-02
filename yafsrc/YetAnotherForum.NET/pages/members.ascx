<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.members" CodeBehind="members.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table cellspacing="0" cellpadding="0" class="content" width="100%">
    <tr>
        <td class="header1" colspan="4">
            <YAF:LocalizedLabel ID="SearchMembersLocalizedLabel" runat="server" LocalizedTag="Search_Members" />
        </td>
    </tr>
    <tr class="header2">
        <td>
            <YAF:LocalizedLabel ID="SearchRolesLocalizedLabel" runat="server" LocalizedTag="Search_Role" />
        </td>
        <td>
            <YAF:LocalizedLabel ID="SearchRankLocalizedLabel" runat="server" LocalizedTag="Search_Rank" />
        </td>
        <td>
            <YAF:LocalizedLabel ID="SearchMemberLocalizedLabel" runat="server" LocalizedTag="Search_Member" />
        </td>
    </tr>
    <tr class="post">
        <td>
            <asp:DropDownList ID="Group" runat="server" Width="95%">
            </asp:DropDownList>
        </td>
        <td>
            <asp:DropDownList ID="Ranks" runat="server" Width="95%">
            </asp:DropDownList>
        </td>
        <td>
            <asp:TextBox ID="UserSearchName" runat="server" Width="95%"></asp:TextBox>
        </td>
    </tr>
    <tr class="post">
        <td colspan="3">
            <YAF:LocalizedLabel ID="NumPostsLabel" runat="server" LocalizedTag="NUMPOSTS" />
            &nbsp;
            <asp:DropDownList ID="NumPostDDL" runat="server" Width="200px">
            </asp:DropDownList>
            &nbsp;
            <asp:TextBox ID="NumPostsTB" runat="server" Width="70px"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="footer1" colspan="4" style="text-align: center">
            <asp:Button ID="SearchByUserName" runat="server" OnClick="Search_Click" Text='<%# this.GetText("BTNSEARCH") %>'  CssClass="pbutton">
            </asp:Button>
            &nbsp;
            <asp:Button ID="ResetUserSearch" runat="server" OnClick="Reset_Click" Text='<%# this.GetText("Clear") %>'  CssClass="pbutton">
            </asp:Button>
        </td>
    </tr>
</table>
<br />
<YAF:AlphaSort ID="AlphaSort1" runat="server" />
<YAF:Pager runat="server" ID="Pager" OnPageChange="Pager_PageChange" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="6">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td class="header2">
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="Avatar" />
        </td>
        <td class="header2">
            <img runat="server" id="SortUserName" alt="Sort User Name" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="UserName" OnClick="UserName_Click" />
        </td>
        <td class="header2">
            <img runat="server" id="SortRank" alt="Sort Rank" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="Rank" Enabled="false" OnClick="Rank_Click" />
        </td>
        <td class="header2">
            <img runat="server" id="SortJoined" alt="Sort Joined" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="Joined" OnClick="Joined_Click" />
        </td>
        <td class="header2" style="text-align:center">
            <img runat="server" id="SortPosts" alt="Sort Posts" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="Posts" OnClick="Posts_Click" />
        </td>
        <td class="header2">
            <img runat="server" id="SortLastVisit" alt="Sort Last Visit" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="LastVisitLB" OnClick="LastVisitLB_Click" />
        </td>
    </tr>
    <asp:Repeater ID="MemberList" runat="server">
        <ItemTemplate>
            <tr>
                <td class="post">
                    <img src="<%# this.GetAvatarUrlFileName(this.Eval("UserID").ToType<int>(), Eval("Avatar").ToString(),Eval("AvatarImage").ToString().IsSet(), Eval("Email").ToString()) %>" alt="<%# DataBinder.Eval(Container.DataItem,"Name").ToString() %>"
                        title="<%# DataBinder.Eval(Container.DataItem,"DisplayName").ToString().IsSet() ? DataBinder.Eval(Container.DataItem,"DisplayName").ToString(): DataBinder.Eval(Container.DataItem,"Name").ToString() %>" class="avatarimage" />
                </td>
                <td class="post">
                    <YAF:UserLink ID="UserProfileLink" runat="server"  UserID='<%# this.Eval("UserID").ToType<int>() %>'
                        Style='<%# Eval("Style") %>' />
                </td>
                <td class="post">
                    <%# Eval("RankName") %>
                </td>
                <td class="post">
                    <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %>
                </td>
                <td class="post" style="text-align:center">
                    <%# "{0:N0}".FormatWith(((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %>
                </td>
                <td class="post">
                    <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
<YAF:Pager runat="server" LinkedPager="Pager" OnPageChange="Pager_PageChange" />
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
