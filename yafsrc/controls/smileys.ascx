<%@ Control Language="c#" AutoEventWireup="True" Codebehind="smileys.ascx.cs" Inherits="YAF.Controls.smileys" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<br /><br />

<table class="content" align="center" cellspacing="0" cellpadding="9">
	<tr class="postheader">
		<td class="header" id="AddSmiley" runat="server" align="center"><b>Add Smiley</b></td>
	</tr>
	<asp:Literal id="SmileyResults" Runat="server" />
</table>

<p class="navlinks" align="center"><YAF:pager runat="server" id="pager"/></p>
