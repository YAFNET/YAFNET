<%@ Control Language="c#" Codebehind="register.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.register" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" LoginCreatedUser="False" StartNextButtonText="Agree" StartNextButtonType="Link"
        OnPreviousButtonClick="CreateUserWizard1_PreviousButtonClick" OnCreateUserError="CreateUserWizard1_CreateUserError">
        <WizardSteps>
            <asp:TemplatedWizardStep runat="server" StepType="Start" Title="Agreement" AllowReturn="False">
                <ContentTemplate>
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                        <tr>
                            <td class="header1" colspan="2">
                                Terms and Conditions:</td>
                        </tr>
                        <tr>
                            <td class="post" colspan="2">
                                <p>
                                    It is impossible for owners and operators of this forum to confirm the validity of all posts on this forum.
                                    Posts reflect the views and opinion of the author, but not necessarily of the forum owners and operators. If you feel that
                                    a posted message is questionable, is encouraged to notify an administrator of this forum immediately.</p>                            
                                <p>
                                    You agree not to post any abusive, vulgar, obscene, hateful, slanderous, threatening, sexually-oriented or any other material
                                    that may violate any applicable laws. Doing so may lead to you being permanently banned from this forum.
                                    Note that all IP address are logged and may aid in enforcing these conditions.</p>
                                <p>
                                    You agree that the owners and operators of this forum have the right to remove, edit, move or close any topic
                                    at any time should they see fit. As a user you agree to any information you have entered above being stored in a database. While
                                    this information will not be disclosed to any third party without your consent the owners and operators cannot
                                    be held responsible for any hacking attempt that may lead to the data being compromised.</p>
                                <p>
                                    This forum system uses cookies to store information on your local computer. These cookies do not contain any of the information
                                    you have entered above; they serve only to improve your viewing pleasure. The e-mail address is used only for confirming your
                                    registration details and password (and for sending new passwords should you forget your current one).</p>
                                <p>
                                    You agree not to use any automated tools for registering and/or posting on this bulletin board. By failing to obey this rule
                                    you would be granting us permission to repeatedly query your web server.</p>
                                <p>
                                    By clicking "Agree" below you will to be bound by the terms and conditions for using this forum</p>
                                 
                                <p align="center">
                                <asp:LinkButton ID="StartNextButton" runat="server" CommandName="MoveNext">I Agree to these Terms and Conditions</asp:LinkButton><br /><br />
                                <asp:LinkButton ID="StartDisagreeButton" runat="server" CommandName="MovePrevious">I DO NOT Agree to these Terms and Conditions</asp:LinkButton>
                                </p>
                            </td>
                        </tr>
                        </table>
                </ContentTemplate>
            </asp:TemplatedWizardStep>
            <asp:CreateUserWizardStep runat="server">
                <ContentTemplate>
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                        <tr>
                            <td align="center" class="header1" colspan="2"><%# GetText("TITLE") %></td>
                        </tr>
                        <tr>
                            <td align="center" class="post" colspan="2">
                            <asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" Display="Dynamic"
                                    ErrorMessage="The Password and Confirmation Password must match." ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required."
                                    ToolTip="User Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required."
                                    ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword" ErrorMessage="Confirm Password is required."
                                    ToolTip="Confirm Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                                    ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">Security Question:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Question" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question" ErrorMessage="Security question is required."
                                    ToolTip="Security question is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security Answer:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Answer" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer" ErrorMessage="Security answer is required."
                                    ToolTip="Security answer is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr align="right">
                            <td align="center" colspan="2" class="postfooter">
                                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Create User" ValidationGroup="CreateUserWizard1" />
                            </td>
                        </tr>                        
                    </table>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True" ValidationGroup="CreateUserWizard1" ShowSummary="False" />
                </ContentTemplate>
                <CustomNavigationTemplate>
                    <!-- moved nav to Content Template -->
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep runat="server">
            </asp:CompleteWizardStep>
        </WizardSteps>
        <StepNavigationTemplate>
            <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious" Text="Previous" />
            <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next" />
        </StepNavigationTemplate>
        <StartNavigationTemplate>
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <asp:Button ID="FinishPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious" Text="Previous" />
            <asp:Button ID="FinishButton" runat="server" CommandName="MoveComplete" Text="Finish" />
        </FinishNavigationTemplate>
    </asp:CreateUserWizard>
</div>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
