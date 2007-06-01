<%@ Control Language="c#" Codebehind="cp_inbox.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_inbox" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
    <tr>
        <td class="header1" colspan="6">
            <%# GetText(IsOutbox ? "sentitems" : "title") %>
        </td>
    </tr>
    <tr class="header2">
        <td>
            &nbsp;</td>
        <td>
            <img runat="server" id="SortSubject" align="absmiddle" />
            <asp:LinkButton runat="server" ID="SubjectLink" /></td>
        <td>
            <img runat="server" id="SortFrom" align="absmiddle" />
            <asp:LinkButton runat="server" ID="FromLink" /></td>
        <td>
            <img runat="server" id="SortDate" align="absmiddle" />
            <asp:LinkButton runat="server" ID="DateLink" /></td>
        <td>
            &nbsp;</td>
    </tr>
    <asp:Repeater ID="Inbox" runat="server">
        <FooterTemplate>
            <tr class="footer1">
                <td colspan="6" align="right">
                    <asp:Button runat="server" OnLoad="DeleteSelected_Load" CommandName="delete" Text='<%# GetText("deleteselected") %>' /></td>
            </tr>
        </FooterTemplate>
        <ItemTemplate>
            <tr class="post">
                <td align="center">
                    <img src='<%# GetImage(Container.DataItem) %>' /></td>
                <td>
                    <a href='<%# YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.cp_message,"pm={0}",Eval("UserPMessageID")) %>'>
                        <%# HtmlEncode(Eval("Subject")) %>
                    </a>
                </td>
                <td>
                    <%# Eval(IsOutbox ? "ToUser" : "FromUser") %>
                </td>
                <td>
                    <%# yaf_DateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Created"] )%>
                </td>
                <td align="center">
                    <asp:CheckBox runat="server" ID="ItemCheck" /></td>
                <asp:Label runat="server" ID="UserPMessageID" Visible="false" Text='<%# Eval("UserPMessageID") %>' />
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</table>