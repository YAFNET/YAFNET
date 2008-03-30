<%@ Control Language="c#" CodeFile="cp_signature.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.cp_signature" %>
<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="SignatureUpdatePanel" runat="server">
    <ContentTemplate>
        <YAF:SignatureEdit runat="server" ID="SignatureEditor" />
    </ContentTemplate>
</asp:UpdatePanel>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
