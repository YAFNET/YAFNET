<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_message" Codebehind="cp_message.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
    <HeaderTemplate>
        <div class="row">
            <div class="col-xl-12">
                <h2><%# this.GetText("TITLE") %></h2>
            </div>
        </div>
    </HeaderTemplate>

    <ItemTemplate>
        <div class="row">
            <div class="col">
                <div class="card mt-3">
                    <div class="card-header">
            <i class="fa fa-envelope-open fa-fw"></i>&nbsp;<%# this.HtmlEncode(this.Eval("Subject")) %>
            <span class="float-right">
                <time><span class="fa-stack">
                        <i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>
                        <i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i>
                        <i class="fa fa-clock fa-badge text-secondary"></i>
                    </span>&nbsp;
                    <YAF:DisplayDateTime ID="CreatedDateTime" runat="server"
                                         DateTime='<%# Container.DataItemToField<DateTime>("Created") %>'></YAF:DisplayDateTime></time>

                <span class="font-weight-bold"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                   LocalizedTag="FROM" />:</span>
                <YAF:UserLink ID="FromUserLink" runat="server" UserID='<%# this.Eval("FromUserID").ToType<int>() %>' />
            </span>
                    </div>
                    <div class="card-body">
            <YAF:MessagePost ID="Message" runat="server" 
            MessageFlags='<%# new MessageFlags(this.Eval("Flags")) %>' 
            Message='<%# this.Eval("Body").ToType<string>()%>' />
                    </div>
                <div class="card-footer">
                    <YAF:ThemeButton ID="ReplyMessage" runat="server"
                                     CommandName="reply" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                                     TextLocalizedTag="BUTTON_REPLY" TitleLocalizedTag="BUTTON_REPLY_TT"
                                     Type="Secondary"
                                     Icon="reply"
                                     Visible='<%# this.Eval("FromUserID").ToType<int>() != this.PageContext.PageUserID %>'/>
                    <YAF:ThemeButton ID="QuoteMessage" runat="server"
                                     CommandName="quote" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                                     TextLocalizedTag="BUTTON_QUOTE" TitleLocalizedTag="BUTTON_QUOTE_TT"
                                     Type="Secondary"
                                     Icon="reply"
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
    </ItemTemplate>
</asp:Repeater>