<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false"
    CodeBehind="yetanotherforum.ascx.cs" Inherits="yaf.MojoPortalModule" %>

<%@ Register TagPrefix="portal" Namespace="mojoPortal.Web" Assembly="mojoPortal.Web" %>
<%@ Register TagPrefix="portal" Namespace="mojoPortal.Web.UI" Assembly="mojoPortal.Web" %>
<%@ Register TagPrefix="mp" Namespace="mojoPortal.Web.Controls" Assembly="mojoPortal.Web.Controls" %>
<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<%@ Register TagPrefix="yc" Namespace="yaf.controls" Assembly="yaf" %>
<mp:CornerRounderTop id="ctop1" runat="server" />
<portal:ModuleTitleControl id="moduleTitle" runat="server" />
<asp:Panel ID="pnlModuleContent" Runat="server" CssClass="modulecontent" >
    <asp:Label id="lblSample" runat="server" />
    <yaf:Forum Visible="true" runat="server" BoardID="1" ID="forum1" />                
    </asp:Panel>
<div class="modulefooter">&nbsp;</div>
<mp:CornerRounderBottom ID="cbottom1" runat="server" />