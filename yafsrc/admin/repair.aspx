<%@ Page language="c#" Codebehind="repair.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.repair" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<table class=content align=center cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td class=header1>Repair</td>
	</tr>
	<tr>
		<td class=header2>Forum Access</td>
	</tr>
	<tr>
		<td class=post>
			<p>
			If the access rights for a forum or user group has been destroyed,
			you can use this option to correct it. When running this function
			all missing access rights will be restored. Existing access rights
			will not be overwritten.
			</p>
			<p>
				<asp:LinkButton id=RepairAccess runat="server">Repair</asp:LinkButton>
			</p>
		</td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
