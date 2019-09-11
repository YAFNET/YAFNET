<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.info" Codebehind="info.ascx.cs" %>
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
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="card text-white bg-secondary mb-3 text-center mx-auto w-50">
    <h5 class="card-header">
        <asp:Label runat="server" ID="Title" Text="Information" />
    </h5>
    <div class="card-body">
        <p class="card-text"><asp:Label runat="server" ID="Info" /></p>
    </div>
    <div class="card-footer">
        <YAF:ThemeButton runat="server" ID="Continue" Type="Danger" />
    </div>
</div>