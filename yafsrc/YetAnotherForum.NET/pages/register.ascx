<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Register" Codebehind="Register.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
<div class="col">
<asp:CreateUserWizard ID="CreateUserWizard1" runat="server" 
                      StartNextButtonText="Agree" StartNextButtonType="Link" 
                      OnPreviousButtonClick="CreateUserWizard1_PreviousButtonClick"
                      OnCreateUserError="CreateUserWizard1_CreateUserError" 
                      OnNextButtonClick="CreateUserWizard1_NextButtonClick"
                      OnCreatedUser="CreateUserWizard1_CreatedUser" 
                      OnContinueButtonClick="CreateUserWizard1_ContinueButtonClick"
                      OnCreatingUser="CreateUserWizard1_CreatingUser"
                      Width="100%">
<WizardSteps>
<asp:CreateUserWizardStep runat="server">
    <ContentTemplate>

                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-user fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                                          LocalizedTag="TITLE" />
                    </div>
                    <div class="card-body">
                        <h5 class="card-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="BASIC_ACCOUNT" />
                        </h5>
                        <div class="form-row">
                        <div class="form-group col-md-6">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
									<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USERNAME" />
									:</asp:Label>
                            
								<asp:TextBox CssClass="form-control" ID="UserName" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
									ErrorMessage="User Name is required." ToolTip="User Name is required." 
                                                            ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
						</div>
						<asp:PlaceHolder runat="server" ID="DisplayNamePlaceHolder" Visible="false">
							<div class="form-group col-md-6">
								<asp:Label ID="DisplayNameLabel" runat="server" AssociatedControlID="DisplayName">
									<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="DISPLAYNAME" />
									:</asp:Label>
                            
								<asp:TextBox CssClass="form-control" ID="DisplayName" runat="server"></asp:TextBox>
								<YAF:LocalizedRequiredFieldValidator ID="DisplayNameRequired" runat="server" 
                                                                     Enabled="false" 
                                                                     ControlToValidate="DisplayName"
                                                                     LocalizedTag="NEED_DISPLAYNAME" 
                                                                     ValidationGroup="CreateUserWizard1">*</YAF:LocalizedRequiredFieldValidator>
                            </div>
					
						</asp:PlaceHolder>
                            </div>
                       
							<div class="form-group">
								<asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
									<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="EMAIL" />
									:</asp:Label>
							
								<asp:TextBox CssClass="form-control" ID="Email" runat="server"
                                             TextMode="Email"></asp:TextBox>
								<asp:RequiredFieldValidator ID="EmailRequired" runat="server" 
                                                            ControlToValidate="Email" 
									ErrorMessage="E-mail is required." ToolTip="E-mail is required." 
                                                            ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="EmailValid" runat="server" 
                                                                ValidationGroup="CreateUserWizard1" 
                                                                ControlToValidate="Email"
                                     ErrorMessage="E-mail address is not valid." 
                                                                ValidationExpression="^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,10}$">*
                                 </asp:RegularExpressionValidator>
                            </div>
                        <h5 class="card-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="PASSWORD" />
                        </h5>
                        
                        <span style="display: none"><asp:CompareValidator ID="PasswordCompare" runat="server" 
                                                                          ControlToCompare="Password"
                                                                          ControlToValidate="ConfirmPassword" 
                                                                          ErrorMessage="The Password and Confirmation Password must match."
                                                                          ValidationGroup="CreateUserWizard1"></asp:CompareValidator></span>
                       
                            <div id="passwordStrength">
                            </div>
                       
                        <div class="form-row">
                        <div class="form-group col-md-6">
								<asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
									<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PASSWORD" />
									:</asp:Label>
									
							
								<asp:TextBox CssClass="form-control" ID="Password" runat="server" TextMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                                            ControlToValidate="Password"
									ErrorMessage="Password is required." 
                                                            ToolTip="Password is required." 
                                                            ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
						</div>
			
							<div class="form-group col-md-6">
								<asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">
									<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
									:</asp:Label>
								<asp:TextBox CssClass="form-control" ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
								<asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
									ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
									ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </div>
						</div>
						<asp:PlaceHolder runat="server" ID="QuestionAnswerPlaceHolder">
                            <div class="form-row">
                            							<div class="form-group col-md-6">
								<asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">
									<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SECURITY_QUESTION" />
									:</asp:Label>
								<asp:TextBox CssClass="form-control" ID="Question" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question"
									ErrorMessage="Security question is required." ToolTip="Security question is required."
									ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </div>
							<div class="form-group col-md-6">
								<asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">
									<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="SECURITY_ANSWER" />
									:</asp:Label>
								<asp:TextBox CssClass="form-control" ID="Answer" runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
									ErrorMessage="Security answer is required." ToolTip="Security answer is required."
									ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </div>
                            </div>
                            </asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="YafCaptchaHolder" Visible="false">
							<div class="form-group">
								<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="Captcha_Image" />
							
								<asp:Image ID="imgCaptcha" runat="server" CssClass="form-control w-25" />
                                <br />
                                <asp:LinkButton id="RefreshCaptcha" runat="server"></asp:LinkButton>
                            </div>
							<div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="tbCaptcha">
                                    <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="Captcha_Enter" />
                                </asp:Label>
                                <asp:TextBox CssClass="form-control" ID="tbCaptcha" runat="server" />
                            </div>
						</asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="RecaptchaPlaceHolder" Visible="false">  
					
							<div class="form-group">
							
						    <YAF:RecaptchaControl runat="server" ID="Recaptcha1" 
                                Visible='<%# this.PageContext.BoardSettings.CaptchaTypeRegister.Equals(2) %>' />
                            </div>
					    </asp:PlaceHolder>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                                               ValidationGroup="CreateUserWizard1" ShowSummary="False" />
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button ID="StepNextButton" runat="server" 
                                    CssClass="btn btn-success mb-1" 
                                    CommandName="MoveNext"
                                    ValidationGroup="CreateUserWizard1" />
                        <YAF:ThemeButton runat="server" ID="LoginButton"
                                         Visible="False"
                                         Type="Primary"
                                         Icon="user-plus"></YAF:ThemeButton>
                        <asp:PlaceHolder runat="server" Visible="False" ID="AuthPanel">
                            <YAF:ThemeButton runat="server" ID="FacebookRegister"
                                             Type="None"
                                             Size="Small"
                                             CssClass="btn btn-social btn-facebook mr-2"
                                             Icon="facebook"
                                             IconCssClass="fab"
                                             Visible="False" 
                                             OnClick="FacebookRegisterClick">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton runat="server" ID="TwitterRegister"
                                             Type="None"
                                             Size="Small"
                                             CssClass="btn btn-social btn-twitter mr-2"
                                             Icon="twitter"
                                             IconCssClass="fab"
                                             Visible="False" 
                                             OnClick="TwitterRegisterClick">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton runat="server" ID="GoogleRegister"
                                             Type="None"
                                             Size="Small"
                                             CssClass="btn btn-social btn-google mr-2"
                                             Icon="google"
                                             IconCssClass="fab"
                                             Visible="False" 
                                             OnClick="GoogleRegisterClick">
                            </YAF:ThemeButton>
                        </asp:PlaceHolder>
                    </div>
                </div>	
				</ContentTemplate>
				<CustomNavigationTemplate>
					<!-- in the Content Template -->
				</CustomNavigationTemplate>
			</asp:CreateUserWizardStep>
            <asp:TemplatedWizardStep runat="server" Title="Profile Information" ID="profile">
				<ContentTemplate>
                    <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-user fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                                                  LocalizedTag="PROFILE" />
                    </div>
                    <div class="card-body">
                    <h5 class="card-title">
                        <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="PROFILE" />
                    </h5>
							<div class="form-group">
								<YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="COUNTRY" />
								<YAF:CountryImageListBox ID="Country" runat="server" DataTextField="Name" DataValueField="Value" 
                                                  CssClass="select2-image-select" />
						</div>

							<div class="form-group">
								<asp:Label ID="LocationLabel" runat="server" AssociatedControlID="Location">
									<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="LOCATION" />
									:</asp:Label>
								<asp:TextBox CssClass="form-control" ID="Location" runat="server"></asp:TextBox>
						</div>
							<div class="form-group">
								<asp:Label ID="HomepageLabel" runat="server" AssociatedControlID="Homepage">
									<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="HOMEPAGE" />
									:</asp:Label>
                            
								<asp:TextBox CssClass="form-control" ID="Homepage" runat="server"></asp:TextBox>
                            </div>
                    <h5 class="card-title">
								<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="PREFERENCES" />
					</h5>
                    <div class="form-group">
								<YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="TIMEZONE" />
								:
								<asp:DropDownList ID="TimeZones" runat="server" 
                                                  DataTextField="Name" 
                                                  DataValueField="Value" 
                                                  CssClass="select2-select" />
                        </div>
                    <div class="form-group">
                                 <asp:CheckBox runat="server" ID="DSTUser" Visible="False" />
                     </div>
                       </div>
                    <div class="card-footer text-center">
							<YAF:ThemeButton ID="ProfileNextButton" runat="server" 
                                             CommandName="MoveNext" TextLocalizedTag="SAVE"
                                             Type="Success"
                                             Icon="save"/>
                        </div>
				</ContentTemplate>
				<CustomNavigationTemplate>
					<!-- in the Content Template -->
				</CustomNavigationTemplate>
			</asp:TemplatedWizardStep>
			<asp:CompleteWizardStep runat="server">
				<ContentTemplate>
                    <div class="row">
                        <div class="col">
                            <div class="card mb-3">
                                <div class="card-header">
                                    <i class="fa fa-user-check fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
                                </div>
                                <div class="card-body text-center">
                                    <YAF:Alert runat="server" Type="success">
                                        <asp:Literal ID="AccountCreated" runat="server" Text="" />
                                    </YAF:Alert>
                                </div>
                                <div class="card-footer text-center">
                                    <asp:Button ID="ContinueButton" runat="server" CssClass="btn btn-success" 
                                                CommandName="Continue"
                                                Text="Continue" ValidationGroup="CreateUserWizard1" />
                                </div>
                            </div>
                        </div>
                    </div>
				</ContentTemplate>
			</asp:CompleteWizardStep>
</WizardSteps>
</asp:CreateUserWizard>
</div>
</div>