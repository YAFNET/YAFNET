<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.ForumWelcome" Codebehind="ForumWelcome.ascx.cs" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<div class="alert alert-light float-right d-none d-md-block" role="alert">
    <i class="fa fa-clock"></i>&nbsp;<asp:Label ID="TimeNow" runat="server" />
    <span class="mx-1"></span>
    <i class="fa fa-calendar-alt"></i>&nbsp;<asp:Label ID="TimeLastVisit" runat="server" />
</div>