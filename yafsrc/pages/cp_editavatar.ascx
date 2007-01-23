<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="cp_editavatar.ascx.cs" Inherits="YAF.Pages.cp_editavatar" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:ProfileEdit runat="server" id="ProfileEditor" />

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
