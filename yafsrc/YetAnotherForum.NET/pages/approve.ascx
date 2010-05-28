<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.approve" Codebehind="approve.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator"></div>
<div align="center">
	<table class="content" width="600" cellspacing="1" cellpadding="0" id="approved"
		runat="server" visible="false">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
			</td>
		</tr>
		<tr>
			<td class="post" colspan="2" align="center">
				<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="email_verified" />
			</td>
		</tr>
	</table>
	<table class="content" width="600" cellspacing="1" cellpadding="0" id="error" runat="server"
		visible="false">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="title" />
			</td>
		</tr>
		<tr>
			<td class="header2" colspan="2" align="center">
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="email_verify_failed" />
			</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="enter_key" />
				:</td>
			<td class="post" width="50%">
				<asp:TextBox Style="width: 300px" ID="key" runat="server" /></td>
		</tr>
		<tr>
			<td class="postfooter" colspan="2" align="center">
				<asp:Button ID="ValidateKey" runat="server" OnClick="ValidateKey_Click" />
			</td>
		</tr>
	</table>
</div>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
