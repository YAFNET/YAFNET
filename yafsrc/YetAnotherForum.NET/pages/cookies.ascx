<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cookies" Codebehind="cookies.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td class="header1" align="center">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
		</td>
	</tr>
	<tr>
		<td>
			<YAF:LocalizedLabel runat="server" LocalizedTag="COOKIES_TEXT" EnableBBCode="true" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
