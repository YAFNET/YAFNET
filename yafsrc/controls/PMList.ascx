<%@ Import namespace="YAF.Classes.Utils"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PMList.ascx.cs" Inherits="YAF.Controls.PMList" EnableTheming="true" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<table class="content" cellspacing="1" cellpadding="0" style="width:98%;">
    <tr>
        <td class="header1">
            <%= GetTitle() %>
        </td>
    </tr>
    <tr>
        <td>
<asp:GridView ID="MessagesView" runat="server" OnRowCreated="MessagesView_RowCreated" DataKeyNames="UserPMessageID" Width="100%" ShowFooter="true" 
 AutoGenerateColumns="false" CellPadding="0" CellSpacing="1" EmptyDataText='<%#GetLocalizedText("NO_MESSAGES") %>'>
    <HeaderStyle CssClass="header2" />
    <RowStyle CssClass="post" />
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate></HeaderTemplate>
            <HeaderStyle CssClass="largefont" />
            <ItemTemplate>
                <img src="<%# GetImage(Container.DataItem) %>" alt="" />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="center" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Image runat="server" ID="SortSubject" />
                <asp:LinkButton runat="server" ID="SubjectLink" OnClick="SubjectLink_Click" Text='<%#GetLocalizedText("SUBJECT") %>' />
            </HeaderTemplate>
            <HeaderStyle CssClass="largefont" />
            <ItemTemplate>
                <a href='<%# GetMessageLink(Eval("UserPMessageID")) %>'>
                    <%# Server.HtmlEncode(Eval("Subject").ToString()) %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Image runat="server" ID="SortFrom" />
                <asp:LinkButton runat="server" ID="FromLink" OnClick="FromLink_Click" Text='<%#GetMessageUserHeader() %>' />
            </HeaderTemplate>
            <HeaderStyle CssClass="largefont" />
            <ItemTemplate>
                <%# GetMessageUser(Container.DataItem)%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Image runat="server" ID="SortDate" />
                <asp:LinkButton runat="server" ID="DateLink" OnClick="DateLink_Click" Text='<%#GetLocalizedText("DATE") %>' />
            </HeaderTemplate>
            <HeaderStyle CssClass="largefont" />
            <ItemTemplate>
                <%# yaf_DateTime.FormatDateTime((DateTime)Eval("Created"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate></HeaderTemplate>
            <HeaderStyle CssClass="largefont" />
            <ItemTemplate><asp:CheckBox runat="server" ID="ItemCheck" /></ItemTemplate>
            <FooterTemplate>
                <span style="line-height:0.5em;"><asp:Button runat="server" ID="ArchiveSelected" Text='<%# GetArchiveSelectedText() %>' OnClick="ArchiveSelected_Click" Visible="<%#this.View == PMView.Inbox %>" /><br /><br /><asp:Button runat="server" ID="DeleteSelected" OnLoad="DeleteSelected_Load" Text='<%# GetDeleteSelectedText() %>' OnClick="DeleteSelected_Click" /></span>
            </FooterTemplate>
            <HeaderStyle Width="150px" />
            <ItemStyle Width="150px" HorizontalAlign="Center" />
            <FooterStyle Width="150px" HorizontalAlign="Center" />
        </asp:TemplateField>
    </Columns>
</asp:GridView>
        </td>
    </tr>
</table>
    <%--
<asp:Repeater ID="MessagesView" runat="server" OnItemCommand="MessagesView_ItemCommand" OnItemCreated="MessagesView_ItemCreated">
    <HeaderTemplate>
        <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="6">
                <%# GetTitle() %>
            </td>
        </tr>
        <tr class="header2">
            <td>&nbsp;</td>
            <td>
                <asp:Image runat="server" ID="SortSubject" />
                <asp:LinkButton runat="server" ID="SubjectLink" OnClick="SubjectLink_Click" Text='<%#GetLocalizedText("SUBJECT") %>' /></td>
            <td>
                <asp:Image runat="server" ID="SortFrom" />
                <asp:LinkButton runat="server" ID="FromLink" OnClick="FromLink_Click" Text='<%#GetMessageUserHeader() %>' /></td>
            <td>
                <asp:Image runat="server" ID="SortDate" />
                <asp:LinkButton runat="server" ID="DateLink" OnClick="DateLink_Click" Text='<%#GetLocalizedText("DATE") %>' /></td>
            <td>&nbsp;</td>
        </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr class="post">
            <td align="center">
                <img src="<%# GetImage(Container.DataItem) %>" alt="" /></td>
            <td>
                <a href='<%# GetMessageLink(Eval("UserPMessageID")) %>'>
                    <%# Server.HtmlEncode(Eval("Subject").ToString()) %>
                </a>
            </td>
            <td>
                <%# GetMessageUser(Container.DataItem)%>
            </td>
            <td>
                <%# yaf_DateTime.FormatDateTime((DateTime)Eval("Created"))%>
            </td>
            <td align="center"><asp:CheckBox runat="server" ID="ItemCheck" /></td>
            <%--<asp:Label runat="server" ID="UserPMessageID" Visible="false" Text='<%# Eval("UserPMessageID") %>' />
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        <tr class="footer1">
            <td colspan="6" align="right">
                <asp:Button runat="server" OnLoad="DeleteSelected_Load" CommandName="delete" Text='<%# GetDeleteSelectedText() %>' /></td>
        </tr>
        </table>
    </FooterTemplate>
</asp:Repeater>    
    --%>
