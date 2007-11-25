<%@ Control language="c#" CodeFile="cp_editprofile.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_editprofile" %>




<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersProfile.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:ProfileEdit runat="server" id="ProfileEditor" />

<div id="DivSmartScroller">
    <YAF:SmartScroller id="SmartScroller1" runat="server" />
</div>
