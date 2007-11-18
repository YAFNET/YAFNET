<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumSubForumList.ascx.cs"
    Inherits="YAF.Controls.ForumSubForumList" %>
<asp:Repeater ID="SubforumList" runat="server">
    <HeaderTemplate>
        <span class="smallfont subforumlink"><b><YAF:LocalizedLabel ID="SubForums" LocalizedTag="SUBFORUMS" runat="server" /></b>:
    </HeaderTemplate>
    <ItemTemplate>
        <%# GetSubForumIcon(Container.DataItem) %> <%# GetForumLink((System.Data.DataRow)Container.DataItem) %></ItemTemplate>
    <SeparatorTemplate>, </SeparatorTemplate>
    <FooterTemplate>
        </span>
    </FooterTemplate>
</asp:Repeater>
