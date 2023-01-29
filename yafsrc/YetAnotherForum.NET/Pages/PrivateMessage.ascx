<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PrivateMessage" Codebehind="PrivateMessage.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<%@ Import Namespace="YAF.Core.Helpers" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>

    <div class="col">
        <div class="my-3 p-3 bg-body rounded shadow-sm">
            <h5><YAF:Icon runat="server"
                          IconName="envelope-open"
                          IconType="text-secondary" />
                <asp:Label runat="server" ID="MessageTitle"></asp:Label></h5>
        <asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
            <ItemTemplate>
                <div class="<%# string.Format("row rounded mb-3 {0}", (Container.DataItem as PagedPm).FromUserID == this.PageBoardContext.PageUserID ? "bg-light" : "border border-secondary") %>">
                    <div class="row">
                        <div class="col d-flex mt-1 p-0">
                            <asp:PlaceHolder runat="server" Visible="<%# (Container.DataItem as PagedPm).FromUserID != this.PageBoardContext.PageUserID  %>">
                                <div class="me-1">
                                    <asp:Image runat="server" ID="Avatar"
                                               CssClass="img-avatar-sm mx-2" ImageUrl="<%# this.Get<IAvatars>().GetAvatarUrlForUser(
                                                                                                             (Container.DataItem as PagedPm).FromUserID,
                                                                                                             (Container.DataItem as PagedPm).FromAvatar,
                                                                                                             (Container.DataItem as PagedPm).FromHasAvatarImage) %>"
                                               AlternateText="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as PagedPm).FromUserDisplayName : (Container.DataItem as PagedPm).FromUser %>"/>
                                </div>
                                <div>
                                    <YAF:UserLink ID="FromUserLink" runat="server"
                                                  ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as PagedPm).FromUserDisplayName : (Container.DataItem as PagedPm).FromUser  %>"
                                                  Suspended="<%# (Container.DataItem as PagedPm).FromSuspended %>"
                                                  Style="<%#(Container.DataItem as PagedPm).FromStyle %>"
                                                  UserID="<%# (Container.DataItem as PagedPm).FromUserID %>" />
                                </div>
                            </asp:PlaceHolder>
                            <div class="<%# (Container.DataItem as PagedPm).FromUserID == this.PageBoardContext.PageUserID ? "me-auto" : "ms-auto" %>">
                                <YAF:Icon runat="server"
                                          IconName="calendar-day"
                                          IconType="text-secondary"
                                          IconNameBadge="clock"
                                          IconBadgeType="text-secondary" />
                                <YAF:DisplayDateTime ID="CreatedDateTime" runat="server"
                                                     DateTime="<%# (Container.DataItem as PagedPm).Created %>" />
                            </div>
                            <asp:PlaceHolder runat="server" Visible="<%# (Container.DataItem as PagedPm).FromUserID == this.PageBoardContext.PageUserID  %>">
                                <div>
                                    <YAF:UserLink ID="UserLink1" runat="server"
                                                  ReplaceName="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as PagedPm).FromUserDisplayName : (Container.DataItem as PagedPm).FromUser  %>"
                                                  Suspended="<%# (Container.DataItem as PagedPm).FromSuspended %>"
                                                  Style="<%#(Container.DataItem as PagedPm).FromStyle %>"
                                                  UserID="<%# (Container.DataItem as PagedPm).FromUserID %>" />
                                </div>
                                <div class="ms-1">
                                    <asp:Image runat="server" ID="Image1"
                                               CssClass="img-avatar-sm" ImageUrl="<%# this.Get<IAvatars>().GetAvatarUrlForUser(
                                                                                                             (Container.DataItem as PagedPm).FromUserID,
                                                                                                             (Container.DataItem as PagedPm).FromAvatar,
                                                                                                             (Container.DataItem as PagedPm).FromHasAvatarImage) %>"
                                               AlternateText="<%# this.PageBoardContext.BoardSettings.EnableDisplayName ? (Container.DataItem as PagedPm).FromUserDisplayName : (Container.DataItem as PagedPm).FromUser %>"/>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                    <div class="row">
                            <div class="col mt-3">
                                <YAF:MessagePost ID="Message" runat="server"
                                                 MessageFlags="<%# new MessageFlags((Container.DataItem as PagedPm).Flags) %>"
                                                 Message="<%# HtmlTagHelper.StripHtml((Container.DataItem as PagedPm).Body)%>"
                                                 MessageID="<%# (Container.DataItem as PagedPm).UserPMessageID %>" />
                            </div>
                        </div>
                        
                        <div class="row justify-content-between align-items-center">
                                    <div class="col-auto px-0">
                                        <YAF:ThemeButton ID="ReportMessage" runat="server"
                                                         CommandName="report" CommandArgument="<%# (Container.DataItem as PagedPm).UserPMessageID %>"
                                                         TextLocalizedTag="REPORTPOST"
                                                         TitleLocalizedTag="REPORTPOST_TITLE"
                                                         DataToggle="tooltip"
                                                         TextLocalizedPage="POSTS"
                                                         Type="Link"
                                                         Icon="exclamation-triangle"
                                                         IconColor="text-danger"/>
                                    </div>
                                    <div class="col-auto px-0 d-flex flex-wrap">
                                        <YAF:ThemeButton ID="ReplyMessage" runat="server"
                                                         CommandName="reply" CommandArgument="<%# (Container.DataItem as PagedPm).UserPMessageID %>"
                                                         TextLocalizedTag="BUTTON_REPLY" TitleLocalizedTag="BUTTON_REPLY_TT"
                                                         DataToggle="tooltip"
                                                         Type="Secondary"
                                                         Icon="reply"
                                                         CssClass="me-1 mb-1"
                                                         Visible="<%# (Container.DataItem as PagedPm).FromUserID != this.PageBoardContext.PageUserID %>"/>
                                        <YAF:ThemeButton ID="QuoteMessage" runat="server"
                                                         CommandName="quote" CommandArgument="<%# (Container.DataItem as PagedPm).UserPMessageID %>"
                                                         TextLocalizedTag="BUTTON_QUOTE_TT" TitleLocalizedTag="BUTTON_QUOTE_TT"
                                                         DataToggle="tooltip"
                                                         Type="Secondary"
                                                         Icon="reply"
                                                         CssClass="me-1 mb-1"
                                                         Visible="<%# (Container.DataItem as PagedPm).FromUserID != this.PageBoardContext.PageUserID %>"/>
                                        <YAF:ThemeButton ID="DeleteMessage" runat="server"
                                                         CommandName="delete" CommandArgument="<%# (Container.DataItem as PagedPm).UserPMessageID %>"
                                                         DataToggle="tooltip"
                                                         TextLocalizedTag="BUTTON_DELETE" TitleLocalizedTag="BUTTON_DELETE_TT"
                                                         ReturnConfirmTag="confirm_deletemessage"
                                                         CssClass="mb-1"
                                                         Type="Danger"
                                                         Icon="trash"/>
                                    </div>
                                </div>
                    </div>
            </ItemTemplate>
        </asp:Repeater>
        </div>
    </div>
</div>