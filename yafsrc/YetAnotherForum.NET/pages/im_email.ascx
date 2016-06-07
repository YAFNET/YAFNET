<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.im_email" Codebehind="im_email.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
<table class="content" width="600px" border="0" cellpadding="0" cellspacing="1">
	<tr>
		<td colspan="2" class="header1">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="150px">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SUBJECT" />
		</td>
		<td class="post">
			<asp:TextBox runat="server" ID="Subject" CssClass="edit" /></td>
	</tr>
	<tr>
		<td class="postheader" valign='top' width="150px">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="BODY" />
		</td>
		<td class="post">
			<asp:TextBox runat="server" ID="Body" TextMode="multiline" Rows='10' CssClass="edit" />
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
