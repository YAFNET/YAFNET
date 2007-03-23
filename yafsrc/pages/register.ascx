<%@ Control Language="c#" Codebehind="register.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.register" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" StartNextButtonText="Agree" StartNextButtonType="Link"
        OnPreviousButtonClick="CreateUserWizard1_PreviousButtonClick" OnCreateUserError="CreateUserWizard1_CreateUserError" OnCreatingUser="CreateUserWizard1_CreatingUser" OnNextButtonClick="CreateUserWizard1_NextButtonClick" OnCreatedUser="CreateUserWizard1_CreatedUser">
        <WizardSteps>
            <asp:TemplatedWizardStep runat="server" Title="Agreement" AllowReturn="False" ID="agreement">
                <ContentTemplate>
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                        <tr>
                            <td class="header1" colspan="2" style="height: 19px">
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
                                <asp:LinkButton ID="AgreeLink" runat="server" Text="Agree" CommandName="MoveNext"/><br /><br />
                                <asp:LinkButton ID="DisagreeLink" runat="server" Text="Disagree" CommandName="MovePrevious"/>
                                </p>
                            </td>
                        </tr>
                        </table>
                </ContentTemplate>
                <CustomNavigationTemplate>
                <!-- in the content template -->
                </CustomNavigationTemplate>
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
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><%# GetText("USERNAME") %>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required."
                                    ToolTip="User Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><%# GetText("PASSWORD") %>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required."
                                    ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword"><%# GetText( "CONFIRM_PASSWORD" )%>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword" ErrorMessage="Confirm Password is required."
                                    ToolTip="Confirm Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email"><%# GetText( "EMAIL" )%>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" ErrorMessage="E-mail is required." ToolTip="E-mail is required."
                                    ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question"><%# GetText( "SECURITY_QUESTION" )%>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Question" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question" ErrorMessage="Security question is required."
                                    ToolTip="Security question is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer"><%# GetText( "SECURITY_ANSWER" )%>:</asp:Label></td>
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
                    <!-- in the Content Template -->    
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
            <asp:TemplatedWizardStep runat="server" Title="Profile Information" ID="profile">
                <ContentTemplate>
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                        <tr>
                            <td align="center" class="header1" colspan="2"><%# GetText( "PROFILE" )%></td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="LocationLabel" runat="server" AssociatedControlID="Location"><%# GetText( "LOCATION" )%>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Location" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="postheader">
                                <asp:Label ID="HomepageLabel" runat="server" AssociatedControlID="Homepage"><%# GetText( "HOMEPAGE" )%>:</asp:Label></td>
                            <td class="post">
                                <asp:TextBox ID="Homepage" runat="server"></asp:TextBox>
                            </td>
                        </tr>
	                    <tr>
		                    <td class="header2" colspan="2" align="center"><%# GetText("PREFERENCES") %></td>
	                    </tr>
	                    <tr>
		                    <td class="postheader"><%# GetText("TIMEZONE") %>:</td>
		                    <td class="post"><asp:DropDownList id="TimeZones" runat="server" DataTextField="Name" DataValueField="Value"/></td>
	                    </tr>
                        <tr align="right">
                            <td align="center" colspan="2" class="postfooter">
                                <asp:Button ID="ProfileNextButton" runat="server" CommandName="MoveNext" Text="Next" />
                            </td>
                        </tr>                    
                    </table>
                </ContentTemplate>
                <CustomNavigationTemplate>
                    <!-- in the Content Template -->
                </CustomNavigationTemplate>
            </asp:TemplatedWizardStep>
            <asp:CompleteWizardStep runat="server">
                <ContentTemplate>
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                        <tr>
                            <td align="center" class="header1" colspan="2"><%# GetText("TITLE") %></td>
                        </tr>                
                        <tr>
                            <td align="center" colspan="2" class="post">
                                Your account has been successfully created.</td>
                        </tr>
                        <tr>
                            <td align="right" colspan="2" class="postfooter">
                                <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" CommandName="Finish" Text="Continue" ValidationGroup="CreateUserWizard1" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</div>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
