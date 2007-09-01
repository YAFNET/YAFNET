<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditUsersSuspend.ascx.cs" Inherits="YAF.Controls.EditUsersSuspend" %>





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