<%@ Control language="c#" Codebehind="cp_editprofile.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.cp_editprofile" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" TagName="ProfileEdit" Src="../controls/EditUsersProfile.ascx" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:ProfileEdit runat="server" id="ProfileEditor" />

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
