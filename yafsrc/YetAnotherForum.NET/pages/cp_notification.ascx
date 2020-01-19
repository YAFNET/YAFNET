<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_notification" Codebehind="cp_notification.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="MyNotifications" Src="../controls/MyNotifications.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <YAF:MyNotifications runat="server"></YAF:MyNotifications>
    </div>
</div>
