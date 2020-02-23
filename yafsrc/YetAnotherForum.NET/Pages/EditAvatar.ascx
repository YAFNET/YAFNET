<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.EditAvatar" Codebehind="EditAvatar.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <YAF:ProfileEdit runat="server" ID="ProfileEditor" />
    </div>
</div>