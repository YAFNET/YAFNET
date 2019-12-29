<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumWelcome" Codebehind="ForumWelcome.ascx.cs" %>

<div class="alert alert-light float-right d-none d-md-block py-0" role="alert">
    <i class="fa fa-clock"></i>&nbsp;<asp:Label ID="TimeNow" runat="server" />
    <asp:PlaceHolder runat="server" ID="LastVisitHolder">
        <span class="mx-1"></span>
        <YAF:Icon runat="server" 
                  IconName="calendar-day"
                  IconType="text-secondary"
                  IconNameBadge="clock" 
                  IconBadgeType="text-secondary"></YAF:Icon>
        <asp:Label ID="TimeLastVisit" runat="server" />
    </asp:PlaceHolder>
</div>