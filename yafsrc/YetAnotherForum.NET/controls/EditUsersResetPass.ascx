<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditUsersResetPass.ascx.cs"
    Inherits="YAF.Controls.EditUsersResetPass" %>
<table width="100%" class="content" cellspacing="1" cellpadding="4">
    <tr>
        <td class="header1" colspan="2">
            Reset User Password
        </td>
    </tr>
    <asp:PlaceHolder ID="PasswordResetErrorHolder" runat="server" Visible="false">
    <tr>
        <td class="header2" colspan="2">
        Error: Current Membership Provider does not support the "Password Reset" function.
        Verify that "EnablePasswordReset" is "true" in your provider configuration settings in the web.config.
        </td>
    </tr>    
    </asp:PlaceHolder>
    <tr>
        <td class="header2" colspan="2">
        <asp:ValidationSummary ID="Summary1" runat="server" ValidationGroup="passreset" />
        </td>
    </tr>
    <tr>    
        <td class="postheader">
            <b>New Password:</b><br />
            New user password.
            </td>
        <td class="post" width="50%">
            <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" />
            <asp:RequiredFieldValidator ID="PasswordValidator" runat="server" ValidationGroup="passreset" ErrorMessage="Must Enter New Password" ControlToValidate="txtNewPassword" SetFocusOnError="true"
             Display="None" />
        </td>
    </tr>
    <tr>    
        <td class="postheader">
            <b>Confirm Password:</b><br />
            Confirm the new password.
            </td>
        <td class="post" width="50%">
            <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" />
            <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="passreset" ErrorMessage="Confirmation password does not match" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword" SetFocusOnError="true"
             Display="None" />
        </td>
    </tr>    
    <tr>
        <td class="postheader">
            <b>Send Email Notification:</b><br />
            Email the user a notification of their new password?</td>
        <td class="post" width="50%">
            <asp:CheckBox runat="server" ID="chkEmailNotify" />
        </td>
    </tr>    
    <tr>
        <td class="postfooter" colspan="2" align="center">
            <asp:Button ID="Reset" runat="server" Text="Reset" ValidationGroup="passreset" OnClick="Reset_Click" CausesValidation="true" />
        </td>
    </tr>
</table>
