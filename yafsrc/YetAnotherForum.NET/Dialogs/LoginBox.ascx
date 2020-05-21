<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.LoginBox" CodeBehind="LoginBox.ascx.cs" %>

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
                <div class="container-fluid was-validated">
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
                                                                 ControlToValidate="Username" />
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
                                                                 ControlToValidate="Password" />
                            
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
                        <asp:Button ID="LoginButton" runat="server"
                                    CausesValidation="True"
                                    CssClass="btn btn-primary btn-loading btn-block"
                                    OnClick="SignIn"/>
                    </div>
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