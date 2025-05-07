﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.LoginBox" CodeBehind="LoginBox.ascx.cs" %>

<%@ Register Src="../controls/OpenAuthProviders.ascx" TagPrefix="YAF" TagName="OpenAuthProviders" %>

<div id="LoginBox" class="modal fade" role="dialog" aria-labelledby="LocalizedLabel1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                        LocalizedPage="LOGIN"
                                        LocalizedTag="title" />
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                </button>
            </div>
            <div class="modal-body">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <div class="container-fluid">
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
                                         autocomplete="username"
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
                                          IconType="text-secondary"></YAF:Icon>
                            </span>
                            <asp:TextBox runat="server" ID="Password" 
                                         CssClass="form-control"
                                         autocomplete="current-password"
                                         TextMode="Password"
                                         required="required"/>
                            <a class="input-group-text link-offset-2 link-underline link-underline-opacity-0" id="PasswordToggle" href="#">
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
                                         CssClass="btn-loading"
                                         TextLocalizedTag="FORUM_LOGIN"
                                         OnClick="SignIn"/>
                    </div>
                </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
            <div class="modal-footer text-center">
                <YAF:ThemeButton ID="RegisterLink" runat="server"
                                 CausesValidation="False"
                                 Size="Small"
                                 Type="OutlineSecondary"
                                 Icon="user-plus"
                                 OnClick="RegisterLinkClick"
                                 TextLocalizedTag="REGISTER_INSTEAD" />
            </div>
        </div>
    </div>
</div>