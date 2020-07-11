<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Account.Login" CodeBehind="Login.ascx.cs" %>

<%@ Register Src="../../controls/OpenAuthProviders.ascx" TagPrefix="YAF" TagName="OpenAuthProviders" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col">
            <div class="card mx-auto" style="max-width:450px">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="sign-in-alt"></YAF:IconHeader>
                </div>
                <div class="card-body needs-validation">
                    <div class="mb-3">
                            <asp:Label runat="server" 
                                       AssociatedControlID="UserName">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                    LocalizedTag="username" />
                            </asp:Label>
                        <div class="input-group">
                            <span class="input-group-text">
                                <YAF:Icon runat="server"
                                          IconName="user"
                                          IconType="text-secondary"></YAF:Icon>
                            </span>
                            <asp:TextBox runat="server" ID="UserName"
                                         CssClass="form-control"
                                         required="required" />
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="NEED_USERNAME" />
                            </div>
                        </div>
                    </div>
                    </div>
                        <div class="mb-3">
                            <asp:Label runat="server" 
                                       AssociatedControlID="Password">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASSWORD" />
                            </asp:Label>
                        <div class="input-group">
                            <span class="input-group-text">
                                <YAF:Icon runat="server"
                                          IconName="key"
                                          IconType="text-secondary"></YAF:Icon>
                            </span>
                            <asp:TextBox runat="server" ID="Password" 
                                         CssClass="form-control"
                                         TextMode="Password"
                                         required="required"/>
                            <a class="input-group-text" id="PasswordToggle" href="#">
                                    <i class="fa fa-eye-slash" aria-hidden="true"></i>
                                </a>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="NEED_PASSWORD" />
                            </div>
                        </div>
                           
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <div class="form-check">
                                    <asp:CheckBox ID="RememberMe" runat="server"
                                                  Checked="True"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6 text-right">
                                <YAF:ThemeButton ID="PasswordRecovery" runat="server" 
                                                 CausesValidation="False"
                                                 Type="Secondary"
                                                 Size="Small"
                                                 Icon="key"
                                                 OnClick="PasswordRecovery_Click"
                                                 TextLocalizedTag="LOSTPASSWORD" />
                            </div>
                        </div>
                    <div class="mb-3">
                        <YAF:ThemeButton ID="LoginButton" runat="server"
                                         CausesValidation="True"
                                         Icon="sign-in-alt"
                                         Type="Primary"
                                         TextLocalizedTag="FORUM_LOGIN"
                                         CssClass="btn-block btn-login"
                                         OnClick="SignIn"/>
                    </div>
                    <section>
                        <YAF:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
                    </section>
                </div>
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