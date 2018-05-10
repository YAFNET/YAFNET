<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersResetPass"CodeBehind="EditUsersResetPass.ascx.cs" %>
<asp:UpdatePanel ID="PasswordUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <table width="100%" class="content" cellspacing="1" cellpadding="4">
            <tr>
                <td class="header1" colspan="2">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_USER_RESETPASS" LocalizedPage="ADMIN_EDITUSER" />
                </td>
            </tr>
            <asp:PlaceHolder ID="PasswordResetErrorHolder" runat="server" Visible="false">
                <tr>
                    <td class="header2" colspan="2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ERROR_NOTE_PASSRESET" LocalizedPage="ADMIN_EDITUSER" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="postheader">
                    <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASS_OPTION" LocalizedPage="ADMIN_EDITUSER" /></strong>
                </td>
                <td class="post">
                    <asp:RadioButtonList ID="rblPasswordResetFunction" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="rblPasswordResetFunction_SelectedIndexChanged">
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
                <td class="header2" colspan="2">
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEAD_USER_CHANGEPASS" LocalizedPage="ADMIN_EDITUSER" />
                </td>
                </tr>
                <tr>
                    <td class="postheader">
                        <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CHANGE_NEW_PASS" LocalizedPage="ADMIN_EDITUSER" />
                    </td>
                    <td class="post" width="50%">
                        <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="PASSWORD_REQUIREMENTS" LocalizedPage="ADMIN_EDITUSER" /></strong>
                        <br />
                        <asp:Label ID="lblPassRequirements" runat="server"></asp:Label>
                        <br />
                        <br />
                        <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="PasswordValidator" runat="server" ValidationGroup="passchange"
                            ControlToValidate="txtNewPassword" SetFocusOnError="true"
                            Display="None" />
                    </td>
                </tr>
                <tr>
                    <td class="postheader">
                        <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="CONFIRM_PASS" LocalizedPage="ADMIN_EDITUSER" />
                    </td>
                    <td class="post" width="50%">
                        <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="passchange"
                            ControlToValidate="txtConfirmPassword"
                            SetFocusOnError="true" Display="None" />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="passchange"
                            ControlToCompare="txtNewPassword"
                            ControlToValidate="txtConfirmPassword" SetFocusOnError="true" Display="None" />
                    </td>
                </tr>
                <tr>
                    <td class="postheader">
                        <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="CHANGE_PASS_NOTIFICATION" LocalizedPage="ADMIN_EDITUSER" />
                    </td>
                    <td class="post" width="50%">
                        <asp:CheckBox runat="server" ID="chkEmailNotify" />
                    </td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td class="postfooter" colspan="2" align="center">
                    <asp:Button ID="btnChangePassword" Visible="false" runat="server"
                        CssClass="pbutton" ValidationGroup="passchange" OnClick="btnChangePassword_Click"
                        CausesValidation="true" />
                    <asp:Button ID="btnResetPassword" runat="server" CssClass="pbutton"
                        OnClick="btnResetPassword_Click" CausesValidation="false" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
