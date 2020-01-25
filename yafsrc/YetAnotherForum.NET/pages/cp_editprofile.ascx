<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.cp_editprofile" Codebehind="cp_editprofile.ascx.cs" %>

<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersProfile.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
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