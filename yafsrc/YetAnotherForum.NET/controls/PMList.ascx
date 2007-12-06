<%@ Import namespace="YAF.Classes.Utils"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PMList.ascx.cs" Inherits="YAF.Controls.PMList" EnableTheming="true" %>

<YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
<asp:GridView ID="MessagesView" runat="server" OnRowCreated="MessagesView_RowCreated" DataKeyNames="UserPMessageID" Width="99%" GridLines="None" Cellspacing="1" ShowFooter="true" 
 AutoGenerateColumns="false" CssClass="content"  EmptyDataText='<%#GetLocalizedText("NO_MESSAGES") %>' EmptyDataRowStyle-CssClass="post">
    <HeaderStyle CssClass="header2" />
    <RowStyle CssClass="post" />
    <AlternatingRowStyle CssClass="post_alt" />
    <FooterStyle CssClass="footer1" />
    <Columns>
        <asp:TemplateField>
            <HeaderTemplate>&nbsp;</HeaderTemplate>
            <ItemTemplate>
                <img src="<%# GetImage(Container.DataItem) %>" alt="" />
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Image runat="server" ID="SortFrom" />
                <asp:LinkButton runat="server" ID="FromLink" OnClick="FromLink_Click" Text='<%#GetMessageUserHeader() %>' />
            </HeaderTemplate>
            <ItemTemplate>
                <YAF:UserLink ID="UserLink1" runat="server"
                    UserID='<%# Convert.ToInt32(( View == PMView.Outbox ) ? Eval("ToUserID") : Eval("FromUserID" )) %>'
                    UserName='<%# Convert.ToString(( View == PMView.Outbox ) ? Eval( "ToUser" ) : Eval( "FromUser" ))%>' />
            </ItemTemplate>
        </asp:TemplateField>        
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Image runat="server" ID="SortSubject" />
                <asp:LinkButton runat="server" ID="SubjectLink" OnClick="SubjectLink_Click" Text='<%#GetLocalizedText("SUBJECT") %>' />
            </HeaderTemplate>
            <HeaderStyle width="40%" />
            <ItemTemplate>
                <a href='<%# GetMessageLink(Eval("UserPMessageID")) %>'>
                    <%# HtmlEncode(Eval("Subject")) %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>
                <asp:Image runat="server" ID="SortDate" />
                <asp:LinkButton runat="server" ID="DateLink" OnClick="DateLink_Click" Text='<%#GetLocalizedText("DATE") %>' />
            </HeaderTemplate>
            <ItemTemplate>
                <%# YafDateTime.FormatDateTime((DateTime)Eval("Created"))%>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <HeaderTemplate>&nbsp;</HeaderTemplate>
            <ItemTemplate><asp:CheckBox runat="server" ID="ItemCheck" /></ItemTemplate>
            <FooterTemplate>
                <asp:Button runat="server" ID="ArchiveSelected" Text='<%# GetArchiveSelectedText() %>' OnClick="ArchiveSelected_Click" Visible="<%#this.View == PMView.Inbox %>" />
                <asp:Button runat="server" ID="DeleteSelected" OnLoad="DeleteSelected_Load" Text='<%# GetDeleteSelectedText() %>' OnClick="DeleteSelected_Click" />
            </FooterTemplate>
            <HeaderStyle Width="125px" />
            <ItemStyle Width="125px" HorizontalAlign="Center" />
            <FooterStyle Width="125px" HorizontalAlign="Center" />
        </asp:TemplateField>          
    </Columns>
</asp:GridView>
<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
