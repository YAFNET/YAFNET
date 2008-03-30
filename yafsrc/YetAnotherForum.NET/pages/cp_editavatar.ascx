<%@ Control Language="C#" AutoEventWireup="true" CodeFile="cp_editavatar.ascx.cs"
    Inherits="YAF.Pages.cp_editavatar" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<YAF:ProfileEdit runat="server" ID="ProfileEditor" />

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
