<%@ Control Language="C#" AutoEventWireup="true" CodeFile="cp_editavatar.ascx.cs" Inherits="YAF.Pages.cp_editavatar" %>

<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:ProfileEdit runat="server" id="ProfileEditor" />

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
