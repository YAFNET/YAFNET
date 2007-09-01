<%@ Control language="c#" CodeFile="cp_signature.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_signature" %>

<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:SignatureEdit runat="server" id="SignatureEditor" />

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
