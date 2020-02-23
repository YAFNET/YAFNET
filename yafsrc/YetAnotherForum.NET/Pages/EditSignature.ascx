<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.EditSignature" Codebehind="EditSignature.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
<div class="col">
<asp:UpdatePanel ID="SignatureUpdatePanel" runat="server">
    <ContentTemplate>
        <YAF:SignatureEdit runat="server" ID="SignatureEditor" />
    </ContentTemplate>
</asp:UpdatePanel>
    </div>
</div>