<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.EditSettings" Codebehind="EditSettings.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileSettings" Src="../controls/EditUsersSettings.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="card">
            <div class="card-header">
                <YAF:Icon runat="server"
                          IconName="user-cog"
                          IconType="text-secondary"></YAF:Icon>
                <YAF:LocalizedLabel runat="server" LocalizedTag="EDIT_SETTINGS"></YAF:LocalizedLabel>
            </div>
            <div class="card-body">
                <YAF:ProfileSettings runat="server" ID="ProfileSettings" />
            </div>
        </div>
    </div>
</div>