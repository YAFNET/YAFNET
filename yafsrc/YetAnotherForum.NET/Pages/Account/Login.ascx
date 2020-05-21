<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Account.Login" CodeBehind="Login.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col">
            <div class="card mx-auto" style="max-width:450px">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="sign-in-alt"></YAF:IconHeader>
                </div>
                <asp:Panel runat="server" ID="ContentBody" 
                           CssClass="card-body">
                    <div class="form-group">
                            <asp:Label runat="server" 
                                       AssociatedControlID="UserName">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                    LocalizedTag="username" />
                            </asp:Label>
                    <div class="input-group mb-3">
                        <div class="input-group-prepend">
                            <span class="input-group-text">
                                <YAF:Icon runat="server"
                                          IconName="user"
                                          IconType="text-secondary"></YAF:Icon>
                            </span>
                        </div>
                            <asp:TextBox runat="server" ID="UserName"
                                         CssClass="form-control"
                                         required="required" />
                        <YAF:LocalizedRequiredFieldValidator runat="server"
                                                             CssClass="invalid-feedback"
                                                             LocalizedTag="NEED_USERNAME" 
                                                             ControlToValidate="Username"/>
                        </div>
                    </div>
                        <div class="form-group">
                            <asp:Label runat="server" 
                                       AssociatedControlID="Password">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASSWORD" />
                            </asp:Label>
                        <div class="input-group mb-3">
                            <div class="input-group-prepend">
                                <span class="input-group-text">
                                    <YAF:Icon runat="server"
                                              IconName="key"
                                              IconType="text-secondary"></YAF:Icon>
                                </span>
                            </div>
                            <asp:TextBox runat="server" ID="Password" 
                                         CssClass="form-control"
                                         TextMode="Password"
                                         required="required"/>
                            <YAF:LocalizedRequiredFieldValidator runat="server"
                                                                 CssClass="invalid-feedback"
                                                                 LocalizedTag="NEED_PASSWORD"
                                                                 ControlToValidate="Password"/>
                            <div class="input-group-append">
                                <a class="input-group-text" id="PasswordToggle" href="#">
                                    <i class="fa fa-eye-slash" aria-hidden="true"></i>
                                </a>
                            </div>
                        </div>
                           
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <div class="custom-control custom-checkbox">
                                    <asp:CheckBox ID="RememberMe" runat="server"
                                                  Checked="True"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="form-group col-md-6 text-right">
                                <YAF:ThemeButton ID="PasswordRecovery" runat="server" 
                                                 CausesValidation="False"
                                                 Type="Secondary"
                                                 Size="Small"
                                                 Icon="key"
                                                 OnClick="PasswordRecovery_Click"
                                                 TextLocalizedTag="LOSTPASSWORD" />
                            </div>
                        </div>
                    <div class="form-group">
                        <YAF:ThemeButton ID="LoginButton" runat="server"
                                         CausesValidation="True"
                                         Icon="sign-in-alt"
                                         Type="Primary"
                                         CssClass="btn-block btn-login"
                                         OnClick="SignIn"/>
                    </div>
                </asp:Panel>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="RegisterLink" runat="server"
                                     CausesValidation="False"
                                     OnClick="RegisterLinkClick"
                                     Type="OutlineSecondary"
                                     Icon="user-plus"
                                     Size="Small"
                                     TextLocalizedTag="REGISTER_INSTEAD" />
                </div>
            </div>
        </div>
    </div>