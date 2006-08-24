<%@ Control language="c#" Codebehind="cp_signature.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.cp_signature" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:SignatureEdit runat="server" id="SignatureEditor" />

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
