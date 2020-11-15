<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PrivateMessage" Codebehind="PrivateMessage.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Flags" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>

    <div class="col">
        <asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
            <ItemTemplate>
                <div class="row">
                    <div class="col">
                        <div class="card mt-3">
                            <div class="card-header">
                                <div class="row justify-content-between align-items-center">
                                    <div class="col-auto">
                                        <YAF:Icon runat="server" 
                                                  IconName="envelope-open"
                                                  IconType="text-secondary" />
                                        <%# this.HtmlEncode((Container.DataItem as dynamic).Subject) %>
                                    </div>
                                    <div class="col-auto">
                                        <YAF:Icon runat="server"
                                                  IconName="calendar-day"
                                                  IconType="text-secondary"
                                                  IconNameBadge="clock" 
                                                  IconBadgeType="text-secondary" />
                                        <YAF:DisplayDateTime ID="CreatedDateTime" runat="server"
                                                             DateTime="<%# (Container.DataItem as dynamic).Created %>" />
                                        <span class="fw-bold ml-2">
                                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                LocalizedTag="FROM" />:
                                        </span>
                                        <YAF:UserLink ID="FromUserLink" runat="server"
                                                      ReplaceName="<%# this.PageContext.BoardSettings.EnableDisplayName ? (Container.DataItem as dynamic).FromUserDisplayName : (Container.DataItem as dynamic).FromUser  %>"
                                                      Suspended="<%# (Container.DataItem as dynamic).FromSuspended %>"
                                                      Style="<%#(Container.DataItem as dynamic).FromStyle %>"
                                                      UserID="<%# (Container.DataItem as dynamic).FromUserID %>" />
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <YAF:MessagePost ID="Message" runat="server" 
                                                 MessageFlags="<%# new MessageFlags((Container.DataItem as dynamic).Flags) %>" 
                                                 Message="<%# (Container.DataItem as dynamic).Body%>"
                                                 MessageID="<%# (Container.DataItem as dynamic).UserPMessageID %>" />
                            </div>
                            <div class="card-footer">
                                <div class="row justify-content-between align-items-center">
                                    <div class="col-auto px-0">
                                        <YAF:ThemeButton ID="ReportMessage" runat="server"
                                                         CommandName="report" CommandArgument="<%# (Container.DataItem as dynamic).UserPMessageID %>" 
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
                                                         CommandName="reply" CommandArgument="<%# (Container.DataItem as dynamic).UserPMessageID %>" 
                                                         TextLocalizedTag="BUTTON_REPLY" TitleLocalizedTag="BUTTON_REPLY_TT"
                                                         DataToggle="tooltip"
                                                         Type="Secondary"
                                                         Icon="reply"
                                                         CssClass="mr-1"
                                                         Visible="<%# (Container.DataItem as dynamic).FromUserID != this.PageContext.PageUserID %>"/>
                                        <YAF:ThemeButton ID="QuoteMessage" runat="server"
                                                         CommandName="quote" CommandArgument="<%# (Container.DataItem as dynamic).UserPMessageID %>" 
                                                         TextLocalizedTag="BUTTON_QUOTE" TitleLocalizedTag="BUTTON_QUOTE_TT"
                                                         DataToggle="tooltip"
                                                         Type="Secondary"
                                                         Icon="reply"
                                                         CssClass="mr-1"
                                                         Visible="<%# (Container.DataItem as dynamic).FromUserID != this.PageContext.PageUserID %>"/>
                                        <YAF:ThemeButton ID="DeleteMessage" runat="server"
                                                         CommandName="delete" CommandArgument="<%# (Container.DataItem as dynamic).UserPMessageID %>" 
                                                         DataToggle="tooltip"
                                                         TextLocalizedTag="BUTTON_DELETE" TitleLocalizedTag="BUTTON_DELETE_TT"
                                                         ReturnConfirmText='<%# this.GetText("confirm_deletemessage") %>'
                                                         Type="Danger"
                                                         Icon="trash"/>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>