<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.im_icq" Codebehind="im_icq.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
<table class="content" width="600" border="0" cellpadding="0" cellspacing="1">
	<tr>
		<td colspan="2" class="header1">
			<img runat="server" id="Status" style="vertical-align: middle" alt="" /><YAF:LocalizedLabel
				ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="NAME" />
		</td>
		<td class="post">
			<asp:TextBox runat="server" ID="From" size="15" MaxLength="40" Style="width: 100%"
				Enabled="false" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="EMAIL" />
		</td>
		<td class="post">
			<asp:TextBox runat="server" ID="Email" size="15" MaxLength="50" Style="width: 100%"
				Enabled="false" />
		</td>
	</tr>
	<tr>
		<td class="postheader" valign='top'>
			<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="BODY" />
		</td>
		<td class="post">
			<asp:TextBox runat="server" ID="Body" TextMode="multiline" Rows='10' Style='width: 100%' />
		</td>
	</tr>
	<tr class="postfooter">
		<td colspan="2" align="center">
			<asp:Button runat="server" ID="Send" CssClass="pbutton" OnClick="Send_Click" />
		</td>
	</tr>
</table>
</div>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
