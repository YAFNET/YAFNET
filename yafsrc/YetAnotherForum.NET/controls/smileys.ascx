<%@ Control Language="c#" AutoEventWireup="True" CodeFile="smileys.ascx.cs" Inherits="YAF.Controls.smileys"
  TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:PlaceHolder ID="SmiliesPlaceholder" runat="server" Visible="true">
  <br />
  <br />
  <table class="content" align="center" cellspacing="0" cellpadding="9">
    <tr class="postheader">
      <td class="header" id="AddSmiley" runat="server" align="center">
        <b>Add Smiley</b></td>
    </tr>
    <asp:Literal ID="SmileyResults" runat="server" />
  </table>
  <p class="navlinks" align="center">
    <YAF:Pager runat="server" ID="pager" OnPageChange="pager_PageChange" />
  </p>
</asp:PlaceHolder>
