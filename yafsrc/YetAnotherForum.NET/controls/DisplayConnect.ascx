<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.DisplayConnect"
    EnableViewState="false" Codebehind="DisplayConnect.ascx.cs" %>
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

<div class="row">
    <div class="col">
        <YAF:Alert runat="server" Type="warning" Dismissing="True">
            <asp:PlaceHolder runat="server" ID="ConnectHolder"></asp:PlaceHolder>
        </YAF:Alert>
    </div>
</div>
