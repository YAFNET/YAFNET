<%@ Control Language="c#" CodeFile="register.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.register" %>




<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
    <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" StartNextButtonText="Agree" StartNextButtonType="Link"
        OnPreviousButtonClick="CreateUserWizard1_PreviousButtonClick" OnCreateUserError="CreateUserWizard1_CreateUserError" OnNextButtonClick="CreateUserWizard1_NextButtonClick" OnCreatedUser="CreateUserWizard1_CreatedUser" OnContinueButtonClick="CreateUserWizard1_ContinueButtonClick" OnCreatingUser="CreateUserWizard1_CreatingUser">
        <WizardSteps>
            <asp:TemplatedWizardStep runat="server" Title="Agreement" AllowReturn="False" ID="agreement">
                <ContentTemplate>
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="700">
                        <tr>
                            <td class="header1" colspan="2" align="center">
                                <%# GetText("TERMS_AND_CONDITIONS_TITLE") %></td>
                        </tr>
                        <tr>
                            <td class="post" colspan="2"><asp:Literal ID="TermsAndConditions" runat="server" Text="" />
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
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="700">
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
												<tr runat="server" id="tr_captcha1" visible="false">
														<td align="right" class="postheader" valign="top">
																<%= GetText("Captcha_Image") %>
														</td>
														<td class="post"><asp:Image ID="imgCaptcha" runat="server" /></td>
												</tr>
												<tr runat="server" id="tr_captcha2" visible="false">
														<td align="right" class="postheader" valign="top">
																<%= GetText("Captcha_Enter") %>
														</td>
														<td class="post">
																<asp:TextBox ID="tbCaptcha" runat="server" />
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
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="700">
                        <tr>
                            <td align="center" class="header1" colspan="2"><%# GetText( "TITLE" )%></td>
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
                    <table class="content" cellspacing="1" cellpadding="0" border="0" width="700">
                        <tr>
                            <td align="center" class="header1" colspan="2"><%# GetText( "PROFILE" )%></td>
                        </tr>
                    <tr>
                      <td class="post"><asp:Literal ID="AccountCreated" runat="server" Text="" /></td>
                    </tr>
                    <tr>
                      <td align="right" class="postfooter">
                        <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" CommandName="Continue"
                          Text="Continue" ValidationGroup="CreateUserWizard1" />
                      </td>
                    </tr>
                  </table>
              </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
</div>
<div id="DivSmartScroller">
    <YAF:SmartScroller id="SmartScroller1" runat="server" />
</div>
