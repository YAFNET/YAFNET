<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumSubForumList.ascx.cs"
    Inherits="YAF.Controls.ForumSubForumList" %>
<asp:Repeater ID="SubforumList" runat="server" OnItemCreated="SubforumList_ItemCreated">
    <HeaderTemplate>
        <span class="smallfont subforumlink"><b><YAF:LocalizedLabel ID="SubForums" LocalizedTag="SUBFORUMS" runat="server" /></b>:
    </HeaderTemplate>
    <ItemTemplate>
        <YAF:ThemeImage ID="ThemeSubforumIcon" runat="server" /> <%# GetForumLink((System.Data.DataRow)Container.DataItem) %></ItemTemplate>
    <SeparatorTemplate>, </SeparatorTemplate>
    <FooterTemplate>
        </span>
    </FooterTemplate>
</asp:Repeater>
