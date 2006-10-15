<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersSuspend.ascx.cs" Inherits="yaf.controls.EditUsersSuspend" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2"><%= ForumPage.GetText("TOOLBAR", "admin")%></td>
	</tr>
	<tr runat="server" id="SuspendedRow">
		<td class="postheader">
      <%= ForumPage.GetText("PROFILE", "ENDS") %>
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