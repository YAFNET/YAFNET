<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.error" Codebehind="error.ascx.cs" %>
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

<YAF:Alert runat="server" ID="Alert" Type="danger">
    <h4 class="alert-heading">Error</h4>
    <asp:Label runat="server" ID="ErrorMessageHolder">
    </asp:Label>
</YAF:Alert>