<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<%@ Control language="c#" Codebehind="index.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.help.index" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:helpmenu runat="server">

<table class="content" width="100%" cellspacing="0" cellpadding="0">
<tr><td class="post" valign="top">
	<table width="100%" cellspacing="0" cellpadding="0">
	<tr><td nowrap class="header2"><b>Search Help Topics</b></td></tr>
	<tr>
		<td nowrap class="post">
			Enter keywords to search for:
			<asp:textbox runat="server" id="search"/>
			<asp:button runat="server" id="DoSearch" text="Search"/>
		</td>
	</tr>
	</table>
</td></tr>
</table>

</YAF:helpmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
