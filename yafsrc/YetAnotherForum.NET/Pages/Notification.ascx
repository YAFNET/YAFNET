<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Notification" Codebehind="Notification.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="MyNotifications" Src="../controls/MyNotifications.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>

    <div class="col">
        <YAF:MyNotifications runat="server"></YAF:MyNotifications>
    </div>
</div>
