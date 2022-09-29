<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.BuddyList" Codebehind="BuddyList.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>

<div class="card mb-3">
    <div class="card-header">
        <div class="row justify-content-between align-items-center">
            <div class="col-auto">
                <YAF:IconHeader runat="server"
                                Text="<%# this.GetHeaderText() %>"
                                IconName="user-friends"/>
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
        <asp:Repeater ID="rptBuddy" runat="server" OnItemDataBound="rptBuddy_ItemCreated" OnItemCommand="rptBuddy_ItemCommand">
    <HeaderTemplate>
        <asp:PlaceHolder runat="server" ID="HeaderHolder">
            <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 g-3 mb-2">
        </asp:PlaceHolder>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="col">
            <div class="card shadow-sm">
                <div class="card-body">
                    <div class="d-flex">
                        <div>
                            <asp:Image runat="server" ID="Avatar"
                                       CssClass="img-thumbnail" ImageUrl="<%# this.Get<IAvatars>().GetAvatarUrlForUser(
                                                                                  (Container.DataItem as BuddyUser).UserID,
                                                                                  (Container.DataItem as BuddyUser).Avatar,
                                                                                  (Container.DataItem as BuddyUser).AvatarImage != null) %>"
                                       AlternateText="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? 
                                                              (Container.DataItem as BuddyUser).DisplayName : (Container.DataItem as BuddyUser).Name  %>"/>
                        </div>
                        <div>
                            <YAF:UserLink ID="UserProfileLink" runat="server"
                                          ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as BuddyUser).DisplayName : (Container.DataItem as BuddyUser).Name %>"
                                          Suspended="<%# (Container.DataItem as BuddyUser).Suspended %>"
                                          Style="<%# (Container.DataItem as BuddyUser).UserStyle %>"
                                          UserID="<%#  this.CurrentUserID == (Container.DataItem as BuddyUser).UserID ? (Container.DataItem as BuddyUser).FromUserID: (Container.DataItem as BuddyUser).UserID %>" />
            <div class="btn-group" role="group">
            <asp:PlaceHolder ID="pnlRemove" runat="server" Visible="false">
                <YAF:ThemeButton ID="lnkRemove" runat="server"
                                 TextLocalizedTag="REMOVEBUDDY"
                                 ReturnConfirmTag="NOTIFICATION_REMOVE"
                                 CommandName="remove" CommandArgument="<%# (Container.DataItem as BuddyUser).UserID %>"
                                 Size="Small"
                                 Type="Danger"
                                 Icon="trash"/>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="pnlPending" runat="server" Visible="false">
                <YAF:ThemeButton runat="server"
                                 Size="Small"
                                 CommandName="approve" CommandArgument="<%# (Container.DataItem as BuddyUser).FromUserID %>"
                                 TextLocalizedTag="APPROVE"
                                 Type="Success"
                                 Icon="check"/>
                <YAF:ThemeButton runat="server"
                                 Size="Small"
                                 CommandName="approveadd" CommandArgument="<%# (Container.DataItem as BuddyUser).FromUserID %>"
                                 TextLocalizedTag="APPROVE_ADD"
                                 Type="Success"
                                 Icon="check"/>
                <YAF:ThemeButton runat="server"
                                 Size="Small"
                                 ReturnConfirmTag="NOTIFICATION_DENY"
                                 CommandName="deny" CommandArgument="<%# (Container.DataItem as BuddyUser).FromUserID %>"
                                 TextLocalizedTag="DENY"
                                 Type="Danger"
                                 Icon="times-circle"/>
            </asp:PlaceHolder>
            </div>
            <asp:PlaceHolder ID="pnlRequests" runat="server" Visible="false">
                <%# this.Get<IDateTimeService>().FormatDateLong((Container.DataItem as BuddyUser).Requested) %>
            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </div>
    <asp:Panel CssClass="card-footer" runat="server" ID="Footer" Visible="False">
        <YAF:ThemeButton ID="Button1" runat="server"
                         ReturnConfirmTag="NOTIFICATION_APPROVEALL"
                         CommandName="approveall"
                         TextLocalizedTag="APPROVE_ALL"
                         Type="Secondary"
                         Icon="check-double"/>
        <YAF:ThemeButton ID="Button3" runat="server"
                         ReturnConfirmTag="NOTIFICATION_APPROVEALLADD"
                         CommandName="approveaddall"
                         TextLocalizedTag="APPROVE_ADD_ALL"
                         Type="Secondary"
                         Icon="check-double"/>
        <YAF:ThemeButton ID="Button2" runat="server"
                         ReturnConfirmTag="NOTIFICATION_REMOVE_OLD_UNAPPROVED"
                         CommandName="denyall"
                         TextLocalizedTag="DENY_ALL"
                         Type="Secondary"
                         Icon="times-circle"/>
    </asp:Panel>
    </FooterTemplate>
</asp:Repeater>
<YAF:Pager ID="Pager" runat="server"
           OnPageChange="Pager_PageChange" />
    </div>
    <asp:PlaceHolder runat="server" Visible="<%# this.rptBuddy.Items.Count == 0 %>">
    <div class="card-body">
        <YAF:Alert runat="server" Type="info" ID="Info"></YAF:Alert>
    </div>
</asp:PlaceHolder>
</div>