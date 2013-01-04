<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.smileys" Codebehind="smileys.ascx.cs" %>

<asp:PlaceHolder ID="SmiliesPlaceholder" runat="server" Visible="true">
  <table class="content smiliesContent" width="100%" cellspacing="0" cellpadding="0" style="margin-top:30px">
    <tr>
      <td class="postheader header2" style="text-align:center">
        <span class="header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="SMILIES_HEADER" /></span>
      </td>
    </tr>
    <tr>
      <td class="post smilesBox">
        <div id="SmiliesBox" class="content">
           <div id="SmiliesPager"></div>
           <br style="clear:both;" />
           <div id="SmiliesPagerResult">
            <p style="text-align:center"><asp:Label ID="LoadingText" runat="server"></asp:Label><br /><asp:Image ID="LoadingImage" runat="server" /></p>
           </div>

            <div id="SmiliesPagerHidden" style="display:none;">
	          <asp:Literal ID="SmileyResults" runat="server" />
            </div>

        </div>
      </td>
    </tr>
  </table>
</asp:PlaceHolder>
