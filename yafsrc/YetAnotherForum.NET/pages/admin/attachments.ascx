<%@ Control Language="c#" CodeFile="attachments.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.attachments" %>




<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
            <td class="header1" colspan="8">
                Attachments</td>
        </tr>
        <asp:Repeater runat="server" ID="List">
            <HeaderTemplate>
                <tr class="header2">
                    <td>
                        Forum</td>
                    <td>
                        Topic</td>
                    <td>
                        Posted</td>
                    <td>
                        File Name</td>
                    <td align="right">
                        Downloads</td>
                    <td>
                        Content Type</td>
                    <td align="right">
                        Size</td>
                    <td>
                        &nbsp;</td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="post">
                    <td>
                        <a target="_top" href='<%# yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.topics,"f={0}",Eval("ForumID")) %>'>
                            <%# Eval("ForumName") %>
                        </a>
                    </td>
                    <td>
                        <a target="_top" href='<%# yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.posts,"t={0}",Eval("TopicID")) %>'>
                            <%# Eval("TopicName") %>
                        </a>
                    </td>
                    <td>
                        <%# yaf_DateTime.FormatDateTimeShort(Eval( "Posted")) %>
                    </td>
                    <td>
                        <%# Eval( "FileName") %>
                    </td>
                    <td align="right">
                        <%# Eval( "Downloads") %>
                    </td>
                    <td>
                        <%# Eval( "ContentType") %>
                    </td>
                    <td align="right">
                        <%# Eval( "Bytes") %>
                    </td>
                    <td>
                        <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "AttachmentID") %>'>Delete</asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
