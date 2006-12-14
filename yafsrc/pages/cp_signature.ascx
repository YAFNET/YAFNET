<%@ Control language="c#" Codebehind="cp_signature.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_signature" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:SignatureEdit runat="server" id="SignatureEditor" />

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
