<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.BuddyList" Codebehind="BuddyList.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<div class="card mb-3">
    <div class="card-header">
        <div class="row justify-content-between align-items-center">
            <div class="col-auto">
                <YAF:IconHeader runat="server"
                                Text="<%# this.GetHeaderText() %>"
                                IconName="user-friends"/>
            </div>
            <div class="col-auto">
                <div class="input-group input-group-sm mr-2" role="group">
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
    <asp:Repeater ID="rptBuddy" runat="server" OnItemCreated="rptBuddy_ItemCreated" OnItemCommand="rptBuddy_ItemCommand">
    <HeaderTemplate>
        <asp:PlaceHolder runat="server" ID="HeaderHolder">
            <ul class="list-group list-group-flush">
        </asp:PlaceHolder>
    </HeaderTemplate>
    <ItemTemplate>
        <li class="list-group-item">
            <YAF:UserLink ID="UserProfileLink" runat="server" 
                          ReplaceName='<%# this.Eval(this.PageContext.BoardSettings.EnableDisplayName ? "DisplayName" : "Name") %>'
                          Suspended='<%# this.Eval("Suspended").ToType<DateTime?>() %>'
                          Style='<%# this.Eval("UserStyle") %>'
                          UserID='<%#  this.Eval(this.CurrentUserID == this.Eval("UserID").ToType<int>() ? "FromUserID": "UserID").ToType<int>() %>' />
            <div class="btn-group" role="group">
            <asp:PlaceHolder ID="pnlRemove" runat="server" Visible="false">
                <YAF:ThemeButton ID="lnkRemove" runat="server"
                                 TextLocalizedTag="REMOVEBUDDY"
                                 ReturnConfirmText='<%# this.GetText("FRIENDS", "NOTIFICATION_REMOVE") %>'
                                 CommandName="remove" CommandArgument='<%# this.Eval("UserID") %>'
                                 Size="Small"
                                 Type="Danger"
                                 Icon="trash"/>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnlPending" runat="server" Visible="false">
                <YAF:ThemeButton runat="server" 
                                 Size="Small"
                                 CommandName="approve" CommandArgument='<%# this.Eval("FromUserID") %>'
                                 TextLocalizedTag="APPROVE"
                                 Type="Success"
                                 Icon="check"/>
                <YAF:ThemeButton runat="server"
                                 Size="Small"
                                 CommandName="approveadd" CommandArgument='<%# this.Eval("FromUserID") %>'
                                 TextLocalizedTag="APPROVE_ADD"
                                 Type="Success"
                                 Icon="check"/>
                <YAF:ThemeButton runat="server"
                                 Size="Small"
                                 ReturnConfirmText='<%# this.GetText("FRIENDS", "NOTIFICATION_DENY") %>'
                                 CommandName="deny" CommandArgument='<%# this.Eval("FromUserID") %>'
                                 TextLocalizedTag="DENY"
                                 Type="Danger"
                                 Icon="times-circle"/>
            </asp:PlaceHolder>
            </div>
            <asp:PlaceHolder ID="pnlRequests" runat="server" Visible="false">
                <%# this.Get<IDateTime>().FormatDateLong((DateTime)((System.Data.DataRowView)Container.DataItem)["Requested"]) %>
            </asp:PlaceHolder>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        <asp:PlaceHolder runat="server" ID="FooterHolder">
            </ul>
            <asp:Panel CssClass="card-footer" runat="server" ID="Footer" Visible="False">
                <YAF:ThemeButton ID="Button1" runat="server" 
                                 ReturnConfirmText='<%# this.GetText("FRIENDS", "NOTIFICATION_APPROVEALL") %>'
                                 CommandName="approveall"
                                 TextLocalizedTag="APPROVE_ALL"
                                 Type="Secondary"
                                 Icon="check-double"/>
                <YAF:ThemeButton ID="Button3" runat="server" 
                                 ReturnConfirmText='<%# this.GetText("FRIENDS", "NOTIFICATION_APPROVEALLADD") %>'
                                 CommandName="approveaddall"
                                 TextLocalizedTag="APPROVE_ADD_ALL"
                                 Type="Secondary"
                                 Icon="check-double"/>
                <YAF:ThemeButton ID="Button2" runat="server" 
                                 ReturnConfirmText='<%# this.GetText("FRIENDS", "NOTIFICATION_REMOVE_OLD_UNAPPROVED")%>'
                                 CommandName="denyall"
                                 TextLocalizedTag="DENY_ALL"
                                 Type="Secondary"
                                 Icon="times-circle"/>
            </asp:Panel>
        </asp:PlaceHolder>
    </FooterTemplate>
</asp:Repeater>
<YAF:Pager ID="Pager" runat="server"
           OnPageChange="Pager_PageChange" />
<asp:PlaceHolder runat="server" Visible="<%# this.rptBuddy.Items.Count == 0 %>">
    <div class="card-body">
        <YAF:Alert runat="server" Type="info" ID="Info"></YAF:Alert>
    </div>
</asp:PlaceHolder>
</div>