<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.CookieConsent" Codebehind="CookieConsent.ascx.cs" %>


<div class="alert alert-dismissible text-center cookiealert" role="alert">
    <div class="cookiealert-container">
        <strong><YAF:LocalizedLabel runat="server" LocalizedPage="COMMON" LocalizedTag="COOKIE_HEADER"></YAF:LocalizedLabel></strong>
        <YAF:LocalizedLabel runat="server" LocalizedPage="COMMON" LocalizedTag="COOKIE" ID="Lable1"></YAF:LocalizedLabel>
        
        <YAF:ThemeButton runat="server" ID="MoreDetails" 
                         CssClass="yaflittlebutton" 
                         TextLocalizedTag="COOKIE_DETAILS"></YAF:ThemeButton>
        <YAF:ThemeButton runat="server" ID="close" 
                         CssClass="yaflittlebutton acceptcookies" 
                         TextLocalizedTag="COOKIE_AGREE"></YAF:ThemeButton>
    </div>
</div>