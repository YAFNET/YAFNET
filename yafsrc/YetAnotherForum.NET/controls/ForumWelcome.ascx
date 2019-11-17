<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumWelcome" Codebehind="ForumWelcome.ascx.cs" %>
<div class="alert alert-light float-right d-none d-md-block" role="alert">
    <i class="fa fa-clock"></i>&nbsp;<asp:Label ID="TimeNow" runat="server" />
    <asp:PlaceHolder runat="server" ID="LastVisitHolder">
        <span class="mx-1"></span>
        <span class="fa-stack">
            <i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>
            <i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i>
            <i class="fa fa-clock fa-badge text-secondary"></i>
        </span>&nbsp;<asp:Label ID="TimeLastVisit" runat="server" />
    </asp:PlaceHolder>
</div>