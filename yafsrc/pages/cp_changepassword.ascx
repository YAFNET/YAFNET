<%@ Control language="c#" Codebehind="cp_changepassword.ascx.cs" AutoEventWireup="false" Inherits="YAF.Pages.cp_changepassword" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" id="PageLinks"/>
<div align="center">
<asp:ChangePassword ID="ChangePassword1" runat="server">
  <ChangePasswordTemplate>
          <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse" width="500">
            <tr>
              <td align="center" colspan="2" class="header1">
                Change Your Password</td>
            </tr>
            <tr>
              <td align="right" class="postheader">
                <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">Password:</asp:Label></td>
              <td class="post">
                <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                  ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
              </td>
            </tr>
            <tr>
              <td align="right" class="postheader">
                <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">New Password:</asp:Label></td>
              <td class="post">
                <asp:TextBox ID="NewPassword" runat="server" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                  ErrorMessage="New Password is required." ToolTip="New Password is required." ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
              </td>
            </tr>
            <tr>
              <td align="right" class="postheader">
                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label></td>
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
                  Text="Cancel" />
              </td>
            </tr>
          </table>
  </ChangePasswordTemplate>
  <SuccessTemplate>
          <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse">
            <tr>
              <td align="center" colspan="2">
                Change Password Complete</td>
            </tr>
            <tr>
              <td>
                Your password has been changed!</td>
            </tr>
            <tr>
              <td align="right" colspan="2">
                <asp:Button ID="ContinuePushButton" runat="server" CausesValidation="False" CommandName="Continue"
                  Text="Continue" />
              </td>
            </tr>
          </table>
  </SuccessTemplate>
</asp:ChangePassword>
</div>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
