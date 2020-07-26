<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PrivateMessage" Codebehind="PrivateMessage.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

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
                                        <%# this.HtmlEncode(this.Eval("Subject")) %>
                                    </div>
                                    <div class="col-auto">
                                        <YAF:Icon runat="server"
                                                  IconName="calendar-day"
                                                  IconType="text-secondary"
                                                  IconNameBadge="clock" 
                                                  IconBadgeType="text-secondary" />
                                        <YAF:DisplayDateTime ID="CreatedDateTime" runat="server"
                                                             DateTime='<%# Container.DataItemToField<DateTime>("Created") %>' />
                                        <span class="font-weight-bold ml-2">
                                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                LocalizedTag="FROM" />:
                                        </span>
                                        <YAF:UserLink ID="FromUserLink" runat="server" 
                                                      Suspended='<%# this.Eval("FromSuspended").ToType<DateTime?>() %>'
                                                      Style='<%# this.Eval("FromStyle") %>'
                                                      UserID='<%# this.Eval("FromUserID").ToType<int>() %>' />
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <YAF:MessagePost ID="Message" runat="server" 
                                                 MessageFlags='<%# new MessageFlags(this.Eval("Flags")) %>' 
                                                 Message='<%# this.Eval("Body").ToType<string>()%>'
                                                 MessageID='<%# this.Eval("UserPMessageID") %>' />
                            </div>
                            <div class="card-footer">
                                <div class="row justify-content-between align-items-center">
                                    <div class="col-auto px-0">
                                        <YAF:ThemeButton ID="ReportMessage" runat="server"
                                                         CommandName="report" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                                                         TextLocalizedTag="REPORTPOST"
                                                         TextLocalizedPage="POSTS"
                                                         Type="Link"
                                                         Icon="exclamation-triangle"
                                                         IconColor="text-danger"/>
                                    </div>
                                    <div class="col-auto px-0 d-flex flex-wrap">
                                        <YAF:ThemeButton ID="ReplyMessage" runat="server"
                                                         CommandName="reply" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                                                         TextLocalizedTag="BUTTON_REPLY" TitleLocalizedTag="BUTTON_REPLY_TT"
                                                         Type="Secondary"
                                                         Icon="reply"
                                                         CssClass="mr-1"
                                                         Visible='<%# this.Eval("FromUserID").ToType<int>() != this.PageContext.PageUserID %>'/>
                                        <YAF:ThemeButton ID="QuoteMessage" runat="server"
                                                         CommandName="quote" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                                                         TextLocalizedTag="BUTTON_QUOTE" TitleLocalizedTag="BUTTON_QUOTE_TT"
                                                         Type="Secondary"
                                                         Icon="reply"
                                                         CssClass="mr-1"
                                                         Visible='<%# this.Eval("FromUserID").ToType<int>() != this.PageContext.PageUserID %>'/>
                                        <YAF:ThemeButton ID="DeleteMessage" runat="server"
                                                         CommandName="delete" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
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