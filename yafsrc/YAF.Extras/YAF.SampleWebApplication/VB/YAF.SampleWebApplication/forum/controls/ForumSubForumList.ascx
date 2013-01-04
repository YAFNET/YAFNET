<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumSubForumList" Codebehind="ForumSubForumList.ascx.cs" %>
<asp:Repeater ID="SubforumList" runat="server" OnItemCreated="SubforumList_ItemCreated">
    <HeaderTemplate>        
        <div class="subForumList"><span class="subForumTitle"><YAF:LocalizedLabel ID="SubForums" LocalizedTag="SUBFORUMS" runat="server" />:</span>
 </HeaderTemplate>
    <ItemTemplate>
        <YAF:ThemeImage ID="ThemeSubforumIcon" runat="server" /> <%# GetForumLink((System.Data.DataRow)Container.DataItem) %></ItemTemplate>
    <SeparatorTemplate>, </SeparatorTemplate>
    <FooterTemplate>
				</div>        
    </FooterTemplate>
</asp:Repeater>
