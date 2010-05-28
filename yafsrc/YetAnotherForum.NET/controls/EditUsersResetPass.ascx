<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersResetPass" Codebehind="EditUsersResetPass.ascx.cs" %>
<asp:UpdatePanel ID="PasswordUpdatePanel" runat="server" UpdateMode="Conditional">
<ContentTemplate>
<table width="100%" class="content" cellspacing="1" cellpadding="4">
    <tr>
        <td class="header1" colspan="2">
            Reset/Change User Password
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
			<td class="postheader">
			<b>Option:</b></td>
			<td class="post">
				<asp:RadioButtonList ID="rblPasswordResetFunction" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblPasswordResetFunction_SelectedIndexChanged">
					<asp:ListItem Selected="True" Text="Reset Password and Email New Password to User" Value="reset" />
					<asp:ListItem Text="Manually Change Password and Optionally Email New Password to User" value="change" />
				</asp:RadioButtonList>
			</td>
    </tr>
    <tr>
        <td class="header2" colspan="2">
        <asp:ValidationSummary ID="Summary1" runat="server" ValidationGroup="passchange" />
        </td>
    </tr>    
    <asp:PlaceHolder ID="ChangePasswordHolder" runat="server" Visible="false">
    <tr>    
        <td class="postheader">
            <b>New Password:</b><br />
            New user password.
            </td>
        <td class="post" width="50%">
            <b>Membership Password Requirements:</b>
            <br /><asp:Label ID="lblPassRequirements" runat="server"></asp:Label>  
            <br /><br />         
            <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" />
         
            <asp:RequiredFieldValidator ID="PasswordValidator" runat="server" ValidationGroup="passchange" ErrorMessage="Must Enter New Password" ControlToValidate="txtNewPassword" SetFocusOnError="true"
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
						<asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="passchange" ErrorMessage="Must Enter a Confirm Password" ControlToValidate="txtConfirmPassword" SetFocusOnError="true"
             Display="None" />            
            <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="passchange" ErrorMessage="Confirmation password does not match" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword" SetFocusOnError="true"
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
    </asp:PlaceHolder>    
    <tr>
        <td class="postfooter" colspan="2" align="center">
            <asp:Button ID="btnChangePassword" Visible="false" runat="server" Text="Change Password" CssClass="pbutton" ValidationGroup="passchange" OnClick="btnChangePassword_Click" CausesValidation="true" />
            <asp:Button ID="btnResetPassword" runat="server" Text="Reset" CssClass="pbutton" OnClick="btnResetPassword_Click" CausesValidation="false" />
        </td>
    </tr>
</table>
</ContentTemplate>
</asp:UpdatePanel>