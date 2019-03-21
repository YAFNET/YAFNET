<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_message" Codebehind="cp_message.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
    <FooterTemplate>
        </div>
        </div>
        </div>
        </div>
    </FooterTemplate>

    <ItemTemplate>
        <div class="row">
            <div class="col-xl-12">
                <h2><%# this.HtmlEncode(this.Eval("Subject")) %></h2>
            </div>
        </div>

        <div class="row">
        <div class="col">
        <YAF:ThemeButton ID="DeleteMessage" runat="server"
                         CommandName="delete" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                         TextLocalizedTag="BUTTON_DELETE" TitleLocalizedTag="BUTTON_DELETE_TT"
                         ReturnConfirmText='<%# this.GetText("confirm_deletemessage") %>'
                         Type="Danger"
                         Icon="trash"/>
        <YAF:ThemeButton ID="ReplyMessage" runat="server"
                         CommandName="reply" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                         TextLocalizedTag="BUTTON_REPLY" TitleLocalizedTag="BUTTON_REPLY_TT"
                         Type="Secondary"
                         Icon="reply"/>
        <YAF:ThemeButton ID="QuoteMessage" runat="server"
                         CommandName="quote" CommandArgument='<%# this.Eval("UserPMessageID") %>' 
                         TextLocalizedTag="BUTTON_QUOTE" TitleLocalizedTag="BUTTON_QUOTE_TT"
                         Type="Secondary"
                         Icon="reply"/>
        <div class="card mt-3">
        <div class="card-header">
            <i class="fa fa-envelope-open fa-fw"></i>&nbsp;<%# this.HtmlEncode(this.Eval("Subject")) %>
            <time class="float-right"><i class="fa fa-calendar-alt fa-fw"></i>&nbsp;
                <YAF:DisplayDateTime ID="CreatedDateTime" runat="server"
                                     DateTime='<%# Container.DataItemToField<DateTime>("Created") %>'></YAF:DisplayDateTime></time>
            
            <span class="float-right">
                <span class="font-weight-bold"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                               LocalizedTag="FROM" />:</span>
            <YAF:UserLink ID="FromUserLink" runat="server" UserID='<%# Convert.ToInt32(this.Eval( "FromUserID" )) %>' />
            </span>
            
            
        </div>
        <div class="card-body">
        
        <tr>
            <td class="postheader">
                <b>
                    
                </b>
            </td>
            <td class="postheader" width="80%">
                <div class="leftItem postedLeft">
                    <b>
                        
                    </b>
                    
                </div>
                <div class="rightItem postedRight">
                    
                </div>
            </td>
        </tr>
        <tr>
            <td class="post">
                &nbsp;
            </td>
            <td class="post" valign="top">
                <YAF:MessagePost ID="Message" runat="server" 
                    MessageFlags='<%# new MessageFlags(this.Eval("Flags")) %>' 
                    Message='<%# this.Eval("Body").ToType<string>()%>' />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
<div id="DivSmartScroller">
    
</div>
