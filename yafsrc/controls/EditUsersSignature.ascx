<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersSignature.ascx.cs" Inherits="YAF.Controls.EditUsersSignature" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>


<table class="content" width="100%" cellspacing="1" cellpadding="0">
<tr>
	<td class="header1" colspan="2"><%= ForumPage.GetText("CP_SIGNATURE","title")%></td>
</tr>
<tr>
	<td class="postformheader" valign="top"><%= ForumPage.GetText("CP_SIGNATURE","signature")%></td>
	<td class="post" id="EditorLine" runat="server">
		<!-- editor goes here -->
	</td>
</tr>
<tr>
	<td class="footer1" colspan="2" align="center">
		<asp:button id="save" cssclass="pbutton" runat="server"/>
		<asp:button id="cancel" cssclass="pbutton" runat="server"/>
	</td>
</tr>
</table>
