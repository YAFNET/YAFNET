<%@ Control Language="c#" Inherits="yaf.controls.smileys" CodeFile="smileys.ascx.cs" CodeFileBaseClass="yaf.BaseUserControl" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>
<br /><br />

<table class="content" align="center" cellspacing="0" cellpadding="9">
	<tr class="postheader">
		<td class="header" id="AddSmiley" runat="server" align="center"><b>Add Smiley</b></td>
	</tr>
	<asp:Literal id="SmileyResults" Runat="server" />
</table>

<p class="navlinks" align="center"><yaf:pager runat="server" id="pager"/></p>
