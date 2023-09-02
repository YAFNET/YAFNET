<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Account.Login" CodeBehind="Login.ascx.cs" %>

<%@ Register Src="../../controls/OpenAuthProviders.ascx" TagPrefix="YAF" TagName="OpenAuthProviders" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <div class="card mx-auto" style="max-width:450px">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconType="text-secondary"
                                LocalizedPage="LOGIN"
                                LocalizedTag="TITLE"
                                IconName="sign-in-alt"></YAF:IconHeader>
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" ID="NotApprovedHolder" Visible="False">
                    <div class="mb-3">
                        <YAF:Alert runat="server" ID="NotApprovedInfo" Type="warning">
                            <YAF:LocalizedLabel runat="server" LocalizedPage="LOGIN" LocalizedTag="ACCOUNT_NOT_APPROVED"></YAF:LocalizedLabel>
                            <YAF:ThemeButton runat="server" ID="ResendConfirm" 
                                             Type="None"
                                             CssClass="alert-link"
                                             OnClick="ResendConfirmClick"
                                             TextLocalizedTag="ADMIN_RESEND_EMAIL"></YAF:ThemeButton>
                        </YAF:Alert>
                    </div>
                </asp:PlaceHolder>
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
                        <div class="mb-3">
                            <asp:Label runat="server"
                                       AssociatedControlID="Password">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASSWORD" />
                            </asp:Label>
                        <div class="input-group">
                            <span class="input-group-text">
                                <YAF:Icon runat="server"
                                          IconName="key"
                                          IconType="text-secondary"/>
                            </span>
                            <asp:TextBox runat="server" ID="Password"
                                         autocomplete="current-password"
                                         CssClass="form-control"
                                         TextMode="Password"
                                         required="required"/>
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server"
                                                    LocalizedTag="NEED_PASSWORD" />
                            </div>
                            <span class="input-group-text link-offset-2 link-underline link-underline-opacity-0" id="PasswordToggle">
                                <i class="fa fa-eye-slash" aria-hidden="true"></i>
                            </span>
                        </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <div class="form-check">
                                    <asp:CheckBox ID="RememberMe" runat="server"
                                                  Checked="True"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6 text-end">
                                <YAF:ThemeButton ID="PasswordRecovery" runat="server"
                                                 CausesValidation="False"
                                                 Type="Secondary"
                                                 Size="Small"
                                                 Icon="key"
                                                 OnClick="PasswordRecovery_Click"
                                                 TextLocalizedTag="LOSTPASSWORD" />
                            </div>
                        </div>
                    <div class="mb-3 d-grid gap-2">
                        <YAF:ThemeButton ID="LoginButton" runat="server"
                                         CausesValidation="True"
                                         Icon="sign-in-alt"
                                         Type="Primary"
                                         TextLocalizedTag="FORUM_LOGIN"
                                         CssClass="btn-login"
                                         OnClick="SignIn"/>
                    </div>
                <asp:PlaceHolder runat="server" ID="OpenAuthProvidersHolder" Visible="False">
                    <section>
                        <YAF:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
                    </section>
                </asp:PlaceHolder>
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