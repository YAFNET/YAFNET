<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.im_xmpp" Codebehind="im_xmpp.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
<table class="content"  width="600px" border="0" cellpadding="0" cellspacing="1">
  <tr>
        <td class="header1">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
        </td>
  </tr>
  <tr>
        <td class="post">
            <asp:Label ID="NotifyLabel" runat="server"></asp:Label>    
        </td>
  </tr>
</table>  
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
