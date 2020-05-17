<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Account.Register" Codebehind="Register.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <div class="card">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="user"/>
            </div>
            <asp:Panel runat="server" ID="Message" CssClass="card-body text-center" Visible="False">
                <YAF:Alert runat="server" Type="success">
                    <asp:Literal ID="AccountCreated" runat="server" />
                </YAF:Alert>
            </asp:Panel>
            <asp:Panel ID="BodyRegister" runat="server" CssClass="card-body">
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                LocalizedTag="USERNAME" />
                            </asp:Label>
                            
                        <asp:TextBox ID="UserName" runat="server"
                                     CssClass="form-control"
                                     required="required"></asp:TextBox>
                        <YAF:LocalizedRequiredFieldValidator ID="UserNameRequired" runat="server" 
                                                             ControlToValidate="UserName"
                                                             LocalizedTag="NEED_USERNAME"
                                                             CssClass="invalid-feedback" />
                    </div>
                    <asp:PlaceHolder runat="server" ID="DisplayNamePlaceHolder" Visible="false">
                        <div class="form-group col-md-6">
                            <asp:Label ID="DisplayNameLabel" runat="server" AssociatedControlID="DisplayName">
                                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="DISPLAYNAME" />
                                </asp:Label>
                            
                            <asp:TextBox ID="DisplayName" runat="server"
                                         CssClass="form-control"></asp:TextBox>
                        </div>
					
                    </asp:PlaceHolder>
                </div>
                <div class="form-group">
                    <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="EMAIL" />
                    </asp:Label>
                    <asp:TextBox ID="Email" runat="server"
                                 TextMode="Email"
                                 CssClass="form-control"
                                 required="required"
                                 placeholder="name@example.com"></asp:TextBox>
                    <YAF:LocalizedRequiredFieldValidator ID="EmailRequired" runat="server" 
                                                         ControlToValidate="Email" 
                                                         LocalizedTag="NEED_EMAIL"
                                                         CssClass="invalid-feedback" />
                    <asp:RegularExpressionValidator ID="EmailValid" runat="server" 
                                                    ControlToValidate="Email"
                                                    ValidationExpression="^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,10}$"
                                                    CssClass="invalid-feedback">
                    </asp:RegularExpressionValidator>
                </div>
                <div class="form-row">
                            <div class="form-group col-md-6">
								<asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
									<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PASSWORD" />
									</asp:Label>
                                <asp:TextBox ID="Password" runat="server" 
                                             TextMode="Password"
                                             CssClass="form-control"
                                             required="required"></asp:TextBox>
                                <div class="d-none" id="passwordStrength">
                                    <small class="form-text text-muted mb-2" id="passwordHelp"></small>
                                    <div class="progress">
                                        <div id="progress-password" class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100"></div>
                                    </div>
                                </div>
                            <YAF:LocalizedRequiredFieldValidator ID="PasswordRequired" runat="server" 
                                                                 ControlToValidate="Password"
                                                                 LocalizedTag="NEED_PASSWORD"
                                                                 CssClass="invalid-feedback"/>
                            </div>
                            <div class="form-group col-md-6">
								<asp:Label ID="ConfirmPasswordLabel" runat="server" AssociatedControlID="ConfirmPassword">
									<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
									</asp:Label>
								<asp:TextBox ID="ConfirmPassword" runat="server" 
                                             TextMode="Password"
                                             CssClass="form-control"
                                             required="required"></asp:TextBox>
                                <YAF:LocalizedRequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" 
                                                                     ControlToValidate="ConfirmPassword"
                                                                     LocalizedTag="RETYPE_PASSWORD"
                                                                     CssClass="invalid-feedback" />
	                             <div class="invalid-feedback" id="PasswordInvalid"></div>
                              
                            </div>
                        </div>
               
						<asp:PlaceHolder runat="server" ID="YafCaptchaHolder" Visible="false">
							<div class="form-group">
								<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="Captcha_Image" />
							
								<asp:Image ID="imgCaptcha" runat="server" />
                                <br />
                                <asp:LinkButton id="RefreshCaptcha" runat="server"></asp:LinkButton>
                            </div>
							<div class="form-group">
								<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="Captcha_Enter" />
							
								<asp:TextBox CssClass="form-control" ID="tbCaptcha" runat="server" />
                            </div>
						</asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="RecaptchaPlaceHolder" Visible="false">
                            <div class="form-group">
                                <YAF:ReCaptchaControl runat="server" ID="Recaptcha1" />
                            </div>
					    </asp:PlaceHolder>
            </asp:Panel>
            <asp:Panel runat="server" ID="Footer" 
                       CssClass="card-footer text-center">
                <YAF:ThemeButton ID="CreateUser" runat="server"
                                 CausesValidation="True"
                                 Icon="user-plus"
                                 Type="Primary"
                                 CssClass="btn-loading"
                                 OnClick="RegisterClick"/>
                <YAF:ThemeButton runat="server" ID="LoginButton"
                                 CausesValidation="False"
                                 Type="OutlineSecondary"
                                 Visible="False"
                                 Icon="sign-in-alt"></YAF:ThemeButton>
            </asp:Panel>
        </div>
    </div>
</div>