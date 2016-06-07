<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.im_msn" Codebehind="im_msn.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div align="center">
    <asp:HyperLink runat="server" ID="Msg">
        <img runat="server" id="Img" border="0" height="25" /></asp:HyperLink>
</div>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
