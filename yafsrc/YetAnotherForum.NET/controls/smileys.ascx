<%@ Control Language="c#" AutoEventWireup="True" CodeFile="smileys.ascx.cs" Inherits="YAF.Controls.smileys"
	TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<asp:PlaceHolder ID="SmiliesPlaceholder" runat="server" Visible="true">
	<br />
	<br />
	<table class="content" align="center" cellspacing="0" cellpadding="9">
		<tr class="postheader">
			<td class="header" id="AddSmiley" runat="server" align="center">
				<%# PageContext.Localization.GetText("SMILIES_HEADER")%></td>
		</tr>
		<asp:Literal ID="SmileyResults" runat="server" />
		<asp:PlaceHolder ID="MoreSmiliesHolder" runat="server">
		<tr class="postfooter">
			<td class="footer" id="MoreSmiliesCell" align="center" runat="server">
				<asp:HyperLink ID="MoreSmilies" Text="More Smilies..." Target="_blank" runat="server" /></td>
		</tr>
		</asp:PlaceHolder>
	</table>
</asp:PlaceHolder>
