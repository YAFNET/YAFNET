<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_message" Codebehind="cp_message.ascx.cs" %>
<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
    <HeaderTemplate>
        <table class="content" width="100%">
    </HeaderTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
    <SeparatorTemplate>
        <tr class="postsep">
            <td colspan="2" style="height: 7px">
            </td>
        </tr>
    </SeparatorTemplate>
    <ItemTemplate>
        <tr>
            <td class="header1" colspan="2">
                <%# HtmlEncode(Eval("Subject")) %>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <b>
                    <YAF:UserLink ID="FromUserLink" runat="server" UserID='<%# Convert.ToInt32(Eval( "FromUserID" )) %>' />
                </b>
            </td>
            <td class="postheader" width="80%">
                <div class="leftItem postedLeft">
                    <b>
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="posted" />
                    </b>
                    <YAF:DisplayDateTime ID="CreatedDateTime" runat="server" DateTime='<%# Container.DataItemToField<DateTime>("Created") %>'></YAF:DisplayDateTime>
                </div>
                <div class="rightItem postedRight">
                    <YAF:ThemeButton ID="DeleteMessage" runat="server" CssClass="yaflittlebutton button-delete" CommandName="delete"
                        CommandArgument='<%# Eval("UserPMessageID") %>' TextLocalizedTag="BUTTON_DELETE"
                        TitleLocalizedTag="BUTTON_DELETE_TT" OnLoad="ThemeButtonDelete_Load" />
                    <YAF:ThemeButton ID="ReplyMessage" runat="server" CssClass="yaflittlebutton button-reply" CommandName="reply"
                        CommandArgument='<%# Eval("UserPMessageID") %>' TextLocalizedTag="BUTTON_REPLY"
                        TitleLocalizedTag="BUTTON_REPLY_TT" />
                    <YAF:ThemeButton ID="QuoteMessage" runat="server" CssClass="yaflittlebutton button-quote" CommandName="quote"
                        CommandArgument='<%# Eval("UserPMessageID") %>' TextLocalizedTag="BUTTON_QUOTE"
                        TitleLocalizedTag="BUTTON_QUOTE_TT" />
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
        <tr class="postfooter">
            <td class="small postTop" colspan='2'>
                <a onclick="ScrollToTop();" class="postTopLink" href="javascript: void(0)">            
                  <YAF:ThemeImage ID="ThemeImage1" LocalizedTitlePage="POSTS" LocalizedTitleTag="TOP"  runat="server" ThemeTag="TOTOPPOST" />
                </a>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
