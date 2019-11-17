<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
    Inherits="YAF.Controls.CookieConsent" Codebehind="CookieConsent.ascx.cs" %>


<div class="alert alert-dismissible text-center cookiealert" role="alert">
    <div class="cookiealert-container">
        <h4 class="alert-heading">
            <YAF:LocalizedLabel runat="server" 
                                LocalizedPage="COMMON" 
                                LocalizedTag="COOKIE_HEADER">
            </YAF:LocalizedLabel>
        </h4>
        <p>
            <YAF:LocalizedLabel runat="server" ID="Label1"
                                LocalizedPage="COMMON" 
                                LocalizedTag="COOKIE">
            </YAF:LocalizedLabel>
            <YAF:ThemeButton runat="server" ID="MoreDetails" 
                             Type="Link" 
                             TextLocalizedTag="COOKIE_DETAILS">
            </YAF:ThemeButton>
            <YAF:ThemeButton runat="server" ID="close" 
                             CssClass="acceptcookies" 
                             Type="Primary" 
                             TextLocalizedTag="COOKIE_AGREE" 
                             Icon="check-square">
            </YAF:ThemeButton>
        </p>
    </div>
</div>