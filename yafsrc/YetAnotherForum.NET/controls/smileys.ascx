<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.smileys" Codebehind="smileys.ascx.cs" %>

<asp:PlaceHolder ID="SmiliesPlaceholder" runat="server" Visible="true">
  <table class="content" width="100%" cellspacing="0" cellpadding="0" style="margin-top:30px">
    <tr>
      <td class="postheader header2" style="text-align:center">
        <span class="header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SMILIES_HEADER" /></span>
      </td>
    </tr>
    <tr>
      <td class="post smilesBox">
        <div id="SmiliesBox" class="content">
          <table align="center" cellspacing="3" cellpadding="9">
	        <asp:Literal ID="SmileyResults" runat="server" />
	      </table>
        </div>
      </td>
    </tr>
  </table>
</asp:PlaceHolder>
