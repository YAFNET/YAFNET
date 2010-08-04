<%@ Control Language="C#" AutoEventWireup="true" CodeFile="buddylist.ascx.cs" Inherits="YAF.Controls.BuddyListMobile" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:Pager ID="Pager" runat="server" OnPageChange="Pager_PageChange" />
<table cellpadding="0" cellspacing="1" class="content" width="100%">
	<tr>
		<td class="header2">
		<img id="SortUserName" runat="server" alt="Sort User Name" style="vertical-align: middle" />
		<asp:LinkButton id="UserName" runat="server" onclick="UserName_Click" />
		</td>
		<td id="tdLastColHdr" runat="server" class="header2">
		<asp:LinkButton id="LastColumn" runat="server" onclick="Requested_Click" />
		</td>
	</tr>
	<asp:Repeater ID="rptBuddy" runat="server" OnItemCommand="rptBuddy_ItemCommand" OnItemCreated="rptBuddy_ItemCreated">
        <ItemTemplate>
            <tr>
                <td class="post">
                    <YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>'
                        UserName='<%# Eval("Name") %>' />
                    <YAF:OnlineStatusImage ID="OnlineStatusImage" runat="server" Visible='<%# PageContext.BoardSettings.ShowUserOnlineStatus && !UserMembershipHelper.IsGuestUser( Eval("UserID") )%>'
                        Style="vertical-align: bottom" UserID='<%# Eval("UserID") %>' />
                </td>

                <td class="post" runat="server" id="tdLastCol">
                    <asp:Panel ID="pnlRemove" runat="server" Visible="false">
                        <asp:LinkButton ID="lnkRemove" runat="server" Text='<%# PageContext.Localization.GetText("REMOVEBUDDY") %>'
                           OnLoad="Remove_Load"  CommandName="remove" CommandArgument='<%# Eval("UserID") %>'></asp:LinkButton>
                    </asp:Panel>
                    <asp:Panel ID="pnlPending" runat="server" Visible="false">
                        <asp:LinkButton runat="server" CommandName="approve" CommandArgument='<%# Eval("FromUserID") %>'
                            Text='<%# PageContext.Localization.GetText("APPROVE") %>' />
                        |
                        <asp:LinkButton runat="server" OnLoad="Deny_Load" CommandName="deny" CommandArgument='<%# Eval("FromUserID") %>'
                            Text='<%# PageContext.Localization.GetText("DENY") %>' />
                        |
                        <asp:LinkButton runat="server" CommandName="approveadd" CommandArgument='<%# Eval("FromUserID") %>'
                            Text='<%# PageContext.Localization.GetText("APPROVE_ADD") %>' />
                    </asp:Panel>
                    <asp:Panel ID="pnlRequests" runat="server" Visible="false">
                        <%# YafServices.DateTime.FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Requested"]) %>
                    </asp:Panel>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr runat="server" id="rptFooter" visible="false">
                <td class="footer1" colspan="6">
                    <asp:Button ID="Button1" OnLoad="ApproveAll_Load" CommandName="approveall" CssClass="pbutton"
                        Text='<%# PageContext.Localization.GetText("APPROVE_ALL") %>' runat="server" />
                    <asp:Button ID="Button3" OnLoad="ApproveAddAll_Load" CommandName="approveaddall"
                        CssClass="pbutton" Text='<%# PageContext.Localization.GetText("APPROVE_ADD_ALL") %>'
                        runat="server" />
                    <asp:Button ID="Button2" OnLoad="DenyAll_Load" CommandName="denyall" CssClass="pbutton"
                        runat="server" Text='<%# PageContext.Localization.GetText("DENY_ALL") %>' />
                </td>
            </tr>
        </FooterTemplate>
    </asp:Repeater>
</table>
<YAF:Pager ID="Pager1" runat="server" LinkedPager="Pager" OnPageChange="Pager_PageChange" />
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
