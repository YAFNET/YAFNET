<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.cp_signature" Codebehind="cp_signature.ascx.cs" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>
<%@ Register TagPrefix="YAF" TagName="SignatureEdit" Src="../controls/EditUsersSignature.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
<div class="col-sm-auto">
    <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
</div>
<div class="col">
<asp:UpdatePanel ID="SignatureUpdatePanel" runat="server">
    <ContentTemplate>
        <YAF:SignatureEdit runat="server" ID="SignatureEditor" />
    </ContentTemplate>
</asp:UpdatePanel>
    </div>
</div>