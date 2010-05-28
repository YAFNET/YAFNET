<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.cp_editavatar" Codebehind="cp_editavatar.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator"></div>
<YAF:ProfileEdit runat="server" ID="ProfileEditor" />

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
