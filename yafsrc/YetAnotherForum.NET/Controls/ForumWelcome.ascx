<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumWelcome" Codebehind="ForumWelcome.ascx.cs" %>

<YAF:Alert runat="server" Type="light" CssClass="float-right d-none d-md-block py-0">
    <YAF:Icon runat="server" 
              IconName="clock"></YAF:Icon>
    <asp:Label ID="TimeNow" runat="server" />
    <asp:PlaceHolder runat="server" ID="LastVisitHolder">
        <span class="mx-1"></span>
        <YAF:Icon runat="server" 
                  IconName="calendar-day"
                  IconNameBadge="clock"></YAF:Icon>
        <asp:Label ID="TimeLastVisit" runat="server" />
    </asp:PlaceHolder>
</YAF:Alert>