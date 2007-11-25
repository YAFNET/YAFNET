<%@ Control Language="c#" CodeFile="im_yim.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.im_yim" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
    <asp:HyperLink runat="server" ID="Msg">
        <img runat="server" id="Img" border="0" /></asp:HyperLink>
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
