<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumWelcome.ascx.cs"
    Inherits="YAF.Controls.ForumWelcome" %>
<div class="yafForumWelcome">
    <div id="divTimeNow">
        <asp:Label ID="TimeNow" runat="server" /></div>
    <div id="divTimeLastVisit">
        <asp:Label ID="TimeLastVisit" runat="server" /></div>
    <div id="divUnreadMsgs">
        <asp:HyperLink runat="server" ID="UnreadMsgs" Visible="false" /></div>
</div>