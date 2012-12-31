<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.register" Codebehind="register.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
	<asp:CreateUserWizard ID="CreateUserWizard1" runat="server" StartNextButtonText="Agree"
		StartNextButtonType="Link" OnPreviousButtonClick="CreateUserWizard1_PreviousButtonClick"
		OnCreateUserError="CreateUserWizard1_CreateUserError" OnNextButtonClick="CreateUserWizard1_NextButtonClick"
		OnCreatedUser="CreateUserWizard1_CreatedUser" OnContinueButtonClick="CreateUserWizard1_ContinueButtonClick"
		OnCreatingUser="CreateUserWizard1_CreatingUser">
		<WizardSteps>
			<asp:CreateUserWizardStep runat="server">
				<ContentTemplate>
					<table class="content" cellspacing="1" cellpadding="0" border="0" width="700">
						<tr>
							<td align="center" class="header1" colspan="2">
								<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" />
							</td>
						</tr>
						<tr>
							<td align="center" class="post" colspan="2">
								<asp:CompareValidator ID="PasswordCompare" runat="server" ControlToCompare="Password"
									ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage="The Password and Confirmation Password must match."
									ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
									<YAF:LocalizedLabel ID="LocalizedLabelRequirementsTitle" runat="server" LocalizedTag="PASSWORD_REQUIREMENTS_TITLE"></YAF:LocalizedLabel>:
									<YAF:LocalizedLabel ID="LocalizedLabelRequirementsText" runat="server" LocalizedTag="PASSWORD_REQUIREMENTS_WARN"></YAF:LocalizedLabel>
							</br>
							<YAF:LocalizedLabel ID="LocalizedLabelLohgUserNameWarnText" runat="server" LocalizedTag="USERNAME_LENGTH_WARN"></YAF:LocalizedLabel>
							</td>
						</tr>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
									<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USERNAME" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="UserName" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
									ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
							</td>
						</tr>
						<asp:PlaceHolder runat="server" ID="DisplayNamePlaceHolder" Visible="false"><tr>
							<td align="right" class="postheader">
								<asp:Label ID="DisplayNameLabel" runat="server" AssociatedControlID="DisplayName">
									<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="DISPLAYNAME" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="DisplayName" runat="server"></asp:TextBox>
								<YAF:LocalizedRequiredFieldValidator ID="DisplayNameRequired" runat="server" Enabled="false" ControlToValidate="DisplayName" LocalizedTag="NEED_DISPLAYNAME" ValidationGroup="CreateUserWizard1">*</YAF:LocalizedRequiredFieldValidator>
							</td>
						</tr>
						</asp:PlaceHolder>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
									<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PASSWORD" />
									:</asp:Label>
									<br />

									</td>
							<td class="post">
								<asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
									ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">
									<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
									ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
									ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
									<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="EMAIL" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="Email" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email" 
									ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="EmailValid" runat="server" ValidationGroup="CreateUserWizard1" ControlToValidate="Email"
                                     ErrorMessage="E-mail adress is not valid." ValidationExpression="^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$">*
                                 </asp:RegularExpressionValidator>
							</td>
						</tr>
						<asp:PlaceHolder runat="server" ID="QuestionAnswerPlaceHolder"><tr>
							<td align="right" class="postheader">
								<asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">
									<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SECURITY_QUESTION" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="Question" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question"
									ErrorMessage="Security question is required." ToolTip="Security question is required."
									ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
							</td>
						</tr>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">
									<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="SECURITY_ANSWER" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="Answer" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
									ErrorMessage="Security answer is required." ToolTip="Security answer is required."
									ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
							</td>
						</tr></asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="YafCaptchaHolder" Visible="false"><tr>
							<td align="right" class="postheader" valign="top">
								<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="Captcha_Image" />
							</td>
							<td class="post">
								<asp:Image ID="imgCaptcha" runat="server" />
                                <br />
                                <asp:LinkButton id="RefreshCaptcha" runat="server"></asp:LinkButton>
                            </td>
						</tr>
						<tr>
							<td align="right" class="postheader" valign="top">
								<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="Captcha_Enter" />
							</td>
							<td class="post">
								<asp:TextBox ID="tbCaptcha" runat="server" />
							</td>
						</tr>
						</asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="RecaptchaPlaceHolder" Visible="false">  

						<tr>
							<td align="right" class="postheader" valign="top">
							<YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="Captcha_Image" />
							</td>
							<td class="post">
						    <asp:PlaceHolder runat="server" ID="RecaptchaControl" Visible="false"/>
					    </td>
					    </tr>
					    </asp:PlaceHolder>
						<tr align="right">
							<td align="center" colspan="2" class="postfooter">
								<asp:Button ID="StepNextButton" runat="server" CssClass="pbutton" CommandName="MoveNext" Text="Create User"
									ValidationGroup="CreateUserWizard1" />
							</td>
						</tr>
					</table>
					<asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
						ValidationGroup="CreateUserWizard1" ShowSummary="False" />
				</ContentTemplate>
				<CustomNavigationTemplate>
					<!-- in the Content Template -->
				</CustomNavigationTemplate>
			</asp:CreateUserWizardStep>
			<asp:TemplatedWizardStep runat="server" Title="Profile Information" ID="profile">
				<ContentTemplate>
					<table class="content" cellspacing="1" cellpadding="0" border="0" width="700">
						<tr>
							<td align="center" class="header1" colspan="2">
								<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="TITLE" />
							</td>
						</tr>
                        <tr>
							<td align="right" class="postheader">
								<YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="COUNTRY" />
								:</td>
							<td class="post">
								<YAF:CountryListBox ID="Country" runat="server" DataTextField="Name" DataValueField="Value" /></td>
						</tr>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="LocationLabel" runat="server" AssociatedControlID="Location">
									<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="LOCATION" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="Location" runat="server"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td align="right" class="postheader">
								<asp:Label ID="HomepageLabel" runat="server" AssociatedControlID="Homepage">
									<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="HOMEPAGE" />
									:</asp:Label></td>
							<td class="post">
								<asp:TextBox ID="Homepage" runat="server"></asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="header2" colspan="2" align="center">
								<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="PREFERENCES" />
							</td>
						</tr>
						<tr>
							<td class="postheader">
								<YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="TIMEZONE" />
								:</td>
							<td class="post">
								<asp:DropDownList ID="TimeZones" runat="server" DataTextField="Name" DataValueField="Value" /></td>
						</tr>
                        <tr>
                            <td class="postheader">
                                <YAF:LocalizedLabel ID="DSTLocalizedLabel" runat="server" LocalizedPage="CP_EDITPROFILE"
                                  LocalizedTag="DST" />
                           </td>
                           <td class="post">
                                 <asp:CheckBox runat="server" ID="DSTUser" />
                           </td>
                        </tr>
						<tr align="right">
							<td align="center" colspan="2" class="postfooter">
								<asp:Button ID="ProfileNextButton" runat="server" CssClass="pbutton" CommandName="MoveNext" Text="Next" />
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
							<td align="center" class="header1" colspan="2">
								<YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="PROFILE" />
							</td>
						</tr>
						<tr>
							<td class="post">
								<asp:Literal ID="AccountCreated" runat="server" Text="" /></td>
						</tr>
						<tr>
							<td align="right" class="postfooter">
								<asp:Button ID="ContinueButton" runat="server" CssClass="pbutton" CausesValidation="False" CommandName="Continue"
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
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
