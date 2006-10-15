<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="cp_editavatar.ascx.cs" Inherits="yaf.pages.cp_editavatar" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" TagName="ProfileEdit" Src="../controls/EditUsersAvatar.ascx" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:ProfileEdit runat="server" id="ProfileEditor" />

<yaf:SmartScroller id="SmartScroller1" runat = "server" />