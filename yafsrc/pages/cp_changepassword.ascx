<%@ Control Language="c#" Codebehind="cp_changepassword.ascx.cs"
  Inherits="YAF.Pages.cp_changepassword" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
  <asp:ChangePassword ID="ChangePassword1" runat="server">
    <ChangePasswordTemplate>
      <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse"
        width="700">
        <tr>
          <td colspan="2" class="header1">
            <%# GetText("TITLE") %>
          </td>
        </tr>
        <tr>
          <td align="right" class="postheader">
            <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword"><%# GetText( "OLD_PASSWORD" )%></asp:Label></td>
          <td class="post">
            <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
              ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td align="right" class="postheader">
            <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword"><%# GetText( "NEW_PASSWORD" )%></asp:Label></td>
          <td class="post">
            <asp:TextBox ID="NewPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
              ErrorMessage="New Password is required." ToolTip="New Password is required." ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td align="right" class="postheader">
            <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword"><%# GetText( "CONFIRM_PASSWORD" )%></asp:Label></td>
          <td class="post">
            <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword"
              ErrorMessage="Confirm New Password is required." ToolTip="Confirm New Password is required."
              ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <td align="center" colspan="2" style="color: red" class="post">
            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword"
              ControlToValidate="ConfirmNewPassword" Display="Dynamic" ErrorMessage="The Confirm New Password must match the New Password entry."
              ValidationGroup="ctl00$ChangePassword1"></asp:CompareValidator>
            <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
          </td>
        </tr>
        <tr>
          <td align="center" class="footer1" colspan="2">
            <asp:Button ID="ChangePasswordPushButton" runat="server" CommandName="ChangePassword"
              Text="Change Password" ValidationGroup="ctl00$ChangePassword1" />
            <asp:Button ID="CancelPushButton" runat="server" CausesValidation="False" CommandName="Cancel"
              Text="Cancel" OnClick="CancelPushButton_Click" />
          </td>
        </tr>
      </table>
      <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup="ctl00$ChangePassword1" ShowSummary="False" />
    </ChangePasswordTemplate>
    <SuccessTemplate>
      <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse" width="700">
        <tr>
          <td colspan="2" class="header1"><%# GetText("TITLE") %></td>
        </tr>
        <tr>
          <td class="post">
            <%# GetText("CHANGE_SUCCESS") %></td>
        </tr>
        <tr>
          <td colspan="2" class="footer1" align="center">
            <asp:Button ID="ContinuePushButton" runat="server" CausesValidation="False" CommandName="Continue"
              Text="Continue" OnClick="ContinuePushButton_Click" />
          </td>
        </tr>
      </table>
    </SuccessTemplate>
  </asp:ChangePassword>
</div>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
