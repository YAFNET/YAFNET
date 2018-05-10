<%@ Control Language="c#" Inherits="YAF.Pages.cp_changepassword" Codebehind="cp_changepassword.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
<div align="center">
    <asp:ChangePassword ID="ChangePassword1" runat="server">
        <ChangePasswordTemplate>
            <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse"
                width="700">
                <tr>
                    <td colspan="2" class="header1">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
                    </td>
                </tr>
                <tr>
                    <td align="right" class="postheader">
                        <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="OLD_PASSWORD" />
                        </asp:Label>
                    </td>
                    <td class="post">
                        <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword"
                            ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="right" class="postheader">
                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NEW_PASSWORD" />
                        </asp:Label>
                    </td>
                    <td class="post">
                        <asp:TextBox ID="NewPassword" runat="server" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword"
                            ErrorMessage="New Password is required." ToolTip="New Password is required."
                            ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td align="right" class="postheader">
                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
                        </asp:Label>
                    </td>
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
                       <asp:CompareValidator ID="NewOldPasswordCompare" ControlToValidate="NewPassword" ControlToCompare="CurrentPassword" 
                               Type="String" Operator="NotEqual" Text="New Password must be different from the old one." ValidationGroup="ctl00$ChangePassword1" Runat="Server" /> 
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
        </ChangePasswordTemplate>
        <SuccessTemplate>
            <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse"
                width="700">
                <tr>
                    <td colspan="2" class="header1">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TITLE" />
                    </td>
                </tr>
                <tr>
                    <td class="post">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="CHANGE_SUCCESS" />
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
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
        ValidationGroup="ctl00$ChangePassword1" ShowSummary="False" />
    <table class="content" border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse" runat="server" ID="SecurityQuestionAndAnswer" Visible="False"
           width="700">
        <tr>
            <td colspan="2" class="header1">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE_SECURITY" />
            </td>
        </tr>
        <tr>
            <td align="right" class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SECURITY_QUESTION_OLD" />
            </td>
            <td class="post">
                <asp:Literal ID="QuestionOld" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td align="right" class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="SECURITY_ANSWER_OLD" /></td>
            <td class="post">
                <asp:TextBox ID="AnswerOld" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="AnswerOld"
                                            ErrorMessage="Answer is required." ToolTip="Answer is required.">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td align="right" class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="SECURITY_QUESTION_NEW" />
            </td>
            <td class="post">
                <asp:TextBox ID="QuestionNew" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td align="right" class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="SECURITY_ANSWER_NEW" /></td>
            <td class="post">
                <asp:TextBox ID="AnswerNew" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="AnswerNew"
                                            ErrorMessage="Answer is required." ToolTip="Answer is required.">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="footer1" align="center">
                <YAF:ThemeButton ID="ChangeSecurityAnswer" runat="server"
                             TextLocalizedTag="TITLE_SECURITY" OnClick="ChangeSecurityAnswerClick" />
            </td>
        </tr>
    </table>
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
