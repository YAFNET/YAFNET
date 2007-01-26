<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersSuspend.ascx.cs" Inherits="YAF.Controls.EditUsersSuspend" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2"><%= PageContext.Localization.GetText("TOOLBAR", "admin")%></td>
	</tr>
	<tr runat="server" id="SuspendedRow">
		<td class="postheader">
      <%= PageContext.Localization.GetText("PROFILE", "ENDS") %>
    </td>
    <td class="post">
      <%= GetSuspendedTo() %>
      &nbsp;<asp:Button runat="server" ID="RemoveSuspension" />
    </td>
  </tr>
  <tr id="Tr1" runat="server">
    <td class="postheader">
      Suspend User:</td>
    <td class="post">
      <asp:TextBox runat="server" ID="SuspendCount" Style="width: 60px" />&nbsp;<asp:DropDownList
        runat="server" ID="SuspendUnit" />&nbsp;<asp:Button runat="server" ID="Suspend" />
    </td>
  </tr>
</table>