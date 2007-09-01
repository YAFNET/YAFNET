<%@ Control Language="c#" CodeFile="cp_message.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.cp_message" %>




<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="Inbox" runat="server" OnItemCommand="Inbox_ItemCommand">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
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
                <%# Eval("FromUser") %>
            </td>
            <td class="postheader">
                <table cellspacing="0" cellpadding="0" width="100%">
                    <tr>
                        <td>
                            <b>
                                <%# GetText("posted") %>
                            </b>
                            <%# yaf_DateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %>
                        </td>
                        <td align="right">
                            <asp:LinkButton ID="DeleteMessage" OnLoad="DeleteMessage_Load" ToolTip="Delete this message"
                                runat="server" CommandName="delete" CommandArgument='<%# Eval("UserPMessageID") %>'><%# GetThemeContents("BUTTONS","DELETEPOST") %></asp:LinkButton>
                            <asp:LinkButton ID="ReplyMessage" ToolTip="Reply to this message" runat="server"
                                CommandName="reply" CommandArgument='<%# Eval("UserPMessageID") %>'><%# GetThemeContents("BUTTONS","REPLYPM") %></asp:LinkButton>
                            <asp:LinkButton ID="QuoteMessage" ToolTip="Reply with quote" runat="server" CommandName="quote"
                                CommandArgument='<%# Eval("UserPMessageID") %>'><%# GetThemeContents("BUTTONS","QUOTEPOST") %></asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="post">
                &nbsp;</td>
            <td class="post" valign="top">
                <%# FormatMsg.FormatMessage(Eval("Body") as string, Convert.ToInt32(Eval("Flags"))) %>
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
