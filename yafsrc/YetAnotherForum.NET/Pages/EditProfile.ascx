<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.EditProfile" Codebehind="EditProfile.ascx.cs" %>

<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersProfile.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="card">
            <div class="card-header">
                <YAF:Icon runat="server"
                          IconName="user-edit"
                          IconType="text-secondary"></YAF:Icon>
                <YAF:LocalizedLabel runat="server" LocalizedTag="EDIT_PROFILE"></YAF:LocalizedLabel>
            </div>
            <div class="card-body">
                <YAF:ProfileEdit runat="server" ID="ProfileEditor" />
            </div>
        </div>
    </div>
</div>