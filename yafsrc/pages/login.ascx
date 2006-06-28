<%@ Control language="c#" Codebehind="login.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.login" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<asp:Login ID="Login1" runat="server">
</asp:Login>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
