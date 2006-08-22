<%@ Register TagPrefix="yaf" Namespace="yaf" Assembly="yaf" %>
<%@ Control language="c#" Inherits="yaf_dnn.DotNetNukeModuleEdit" CodeBehind="DotNetNukeModuleEdit.ascx.cs" AutoEventWireup="false" %>

<table width="100%" cellspacing="0" cellpadding="2" summary="importantmessagesdev edit design table">
<tr>
	<td width="50%" class="SubHead">Select Board to use in this module.</td>
	<td width="50%"><asp:dropdownlist autopostback="true" cssclass="NormalTextBox" runat="server" id="BoardID"/></td>
</tr>
<tr>
	<td width="50%" class="SubHead">Select Category to show in this module.</td>
	<td width="50%"><asp:dropdownlist cssclass="NormalTextBox" runat="server" id="CategoryID"/></td>
</tr>
</table>

<p>
	<asp:linkbutton runat="server" id="update" cssclass="CommandButton" />
	&nbsp;
	<asp:linkbutton runat="server" id="cancel" cssclass="CommandButton" />
	&nbsp;
	<asp:linkbutton runat="server" id="create" cssclass="CommandButton" />
</p>
