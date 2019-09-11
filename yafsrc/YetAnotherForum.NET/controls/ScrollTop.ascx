<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" Inherits="YAF.Controls.ScrollTop" Codebehind="ScrollTop.ascx.cs" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>


<div class="scroll-top">
    <YAF:ThemeButton runat="server" ID="ScrollButton" 
                     CssClass="btn-scroll"
                     Icon="angle-double-up"
                     Type="OutlineSecondary">
    </YAF:ThemeButton>
</div>