<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="../../../controls/forumwelcome.ascx.cs"
    Inherits="YAF.Controls.ForumWelcome" %>
<div class="yafForumWelcome">
    <div id="divTimeNow" runat="server" visible="false">
        <asp:Label ID="TimeNow" runat="server" /></div>
    <div id="divTimeLastVisit" runat="server" visible="false">
        <asp:Label ID="TimeLastVisit" runat="server" /></div>
    <div id="divUnreadMsgs">
        <asp:HyperLink runat="server" ID="UnreadMsgs" Visible="false" /></div>
</div>