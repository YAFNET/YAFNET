<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.LoginBox" CodeBehind="LoginBox.ascx.cs" %>

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
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <div class="container-fluid">
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
                                         CssClass="btn-loading btn-block"
                                         TextLocalizedTag="FORUM_LOGIN"
                                         OnClick="SignIn"/>
                    </div>
                    <section>
                        <YAF:OpenAuthProviders runat="server" ID="OpenAuthLogin" />
                    </section>
                </div>
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