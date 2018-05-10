<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.BuddyList" Codebehind="BuddyList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:Pager runat="server" ID="Pager" OnPageChange="Pager_PageChange" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header2">
            <img runat="server" id="SortUserName" alt="Sort User Name" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="UserName" OnClick="UserName_Click" />
        </td>
        <td class="header2">
            <img runat="server" id="SortRank" alt="Sort Rank" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="Rank" OnClick="Rank_Click" />
        </td>
        <td class="header2">
            <img runat="server" id="SortJoined" alt="Sort Joined" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="Joined" OnClick="Joined_Click" />
        </td>
        <td class="header2" align="center">
            <img runat="server" id="SortPosts" alt="Sort Posts" style="vertical-align: middle" />
            <asp:LinkButton runat="server" ID="Posts" OnClick="Posts_Click" />
        </td>
        <td class="header2">
            <asp:Label runat="server" ID="Location" />
        </td>
        <td class="header2" runat="server" id="tdLastColHdr">
            <asp:LinkButton runat="server" ID="LastColumn" OnClick="Requested_Click" />
        </td>
    </tr>
    <asp:Repeater ID="rptBuddy" runat="server" OnItemCreated="rptBuddy_ItemCreated" OnItemCommand="rptBuddy_ItemCommand">
        <ItemTemplate>
            <tr>
                <td class="post">
                    <YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%# CurrentUserID == Convert.ToInt32(Eval("UserID")) ? Eval("FromUserID") : Eval("UserID") %>' />
                    <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Eval("UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# Eval("UserID") %>' />
                </td>
                <td class="post">
                    <%# Eval("RankName") %>
                </td>
                <td class="post">
                    <%# this.Get<IDateTime>().FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %>
                </td>
                <td class="post" align="center">
                    <%# String.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %>
                </td>
                <td class="post">
                    <%# GetStringSafely(YafUserProfile.GetProfile(DataBinder.Eval(Container.DataItem,"Name").ToString()).Location) %>
                </td>
                <td class="post" runat="server" id="tdLastCol">
                    <asp:Panel ID="pnlRemove" runat="server" Visible="false">
                        <asp:LinkButton ID="lnkRemove" runat="server" Text='<%# this.GetText("REMOVEBUDDY") %>'
                           OnLoad="Remove_Load"  CommandName="remove" CommandArgument='<%# Eval("UserID") %>'></asp:LinkButton>
                    </asp:Panel>
                    <asp:Panel ID="pnlPending" runat="server" Visible="false">
                        <asp:LinkButton runat="server" CommandName="approve" CommandArgument='<%# Eval("FromUserID") %>'
                            Text='<%# this.GetText("APPROVE") %>' />
                        |
                        <asp:LinkButton runat="server" OnLoad="Deny_Load" CommandName="deny" CommandArgument='<%# Eval("FromUserID") %>'
                            Text='<%# this.GetText("DENY") %>' />
                        |
                        <asp:LinkButton runat="server" CommandName="approveadd" CommandArgument='<%# Eval("FromUserID") %>'
                            Text='<%# this.GetText("APPROVE_ADD") %>' />
                    </asp:Panel>
                    <asp:Panel ID="pnlRequests" runat="server" Visible="false">
                        <%# this.Get<IDateTime>().FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Requested"]) %>
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr runat="server" id="rptFooter" visible="false">
                <td class="footer1" colspan="6">
                    <asp:Button ID="Button1" OnLoad="ApproveAll_Load" CommandName="approveall" CssClass="pbutton"
                        Text='<%# this.GetText("APPROVE_ALL") %>' runat="server" />
                    <asp:Button ID="Button3" OnLoad="ApproveAddAll_Load" CommandName="approveaddall"
                        CssClass="pbutton" Text='<%# this.GetText("APPROVE_ADD_ALL") %>'
                        runat="server" />
                    <asp:Button ID="Button2" OnLoad="DenyAll_Load" CommandName="denyall" CssClass="pbutton"
                        runat="server" Text='<%# this.GetText("DENY_ALL") %>' />
                </td>
            </tr>
        </FooterTemplate>
    </asp:Repeater>
</table>
<YAF:Pager ID="Pager1" runat="server" LinkedPager="Pager" OnPageChange="Pager_PageChange" />
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
