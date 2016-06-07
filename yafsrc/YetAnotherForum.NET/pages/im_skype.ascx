<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.im_skype" Codebehind="im_skype.ascx.cs" %>
<script type="text/javascript"
src="http://download.skype.com/share/skypebuttons/js/skypeCheck.js">
</script>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div align="center">
    <asp:HyperLink runat="server" ID="Msg">
        <img runat="server" id="Img" border="0" /></asp:HyperLink>
</div>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
