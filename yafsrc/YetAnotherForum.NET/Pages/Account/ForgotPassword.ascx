<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.Account.ForgotPassword" Codebehind="ForgotPassword.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-auto mx-auto">
        <div class="card">
            <div class="card-body">
                <h5 class="card-title">
                    <YAF:LocalizedLabel runat="server"
                                        LocalizedTag="PAGE1_INSTRUCTIONS"/>
                </h5>
                <div class="mb-3">
                    <asp:Label ID="UserNameLabel" runat="server"
                               AssociatedControlID="UserName">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                            LocalizedPage="LOGIN"
                                            LocalizedTag="USERNAME" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="UserName"
                                 CssClass="form-control"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_USERNAME" />
                    </div>
                </div>
                <div class="mb-3 d-grid gap-2">
                    <YAF:ThemeButton runat="server" ID="Forgot"
                                     CausesValidation="True"
                                     TextLocalizedTag="SUBMIT"
                                     OnClick="ForgotPasswordClick"/>
                </div>
            </div>
        </div>
    </div>
</div>

