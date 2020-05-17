<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Pages.Account.ResetPassword" Codebehind="ResetPassword.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>
<div class="row">
    <div class="col">
        <div class="card w-50 mx-auto">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                LocalizedTag="TITLE"
                                LocalizedPage="RECOVER_PASSWORD"
                                IconName="key"/>
            </div>
            <asp:Panel runat="server" ID="ContentBody" 
                       CssClass="card-body">
                <div class="form-group">
                    <asp:Label runat="server" 
                               AssociatedControlID="Email">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="EMAIL" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="Email" 
                                 CssClass="form-control" 
                                 TextMode="Email"
                                 required="required"/>
                    <YAF:LocalizedRequiredFieldValidator ID="EmailRequired" runat="server" 
                                                         ControlToValidate="Email" 
                                                         LocalizedTag="NEED_EMAIL"
                                                         CssClass="invalid-feedback" />
                </div>
                <div class="form-group">
                    <asp:Label runat="server" 
                               AssociatedControlID="Password">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PASSWORD" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="Password" 
                                 TextMode="Password" 
                                 CssClass="form-control"
                                 required="required" />
                    <YAF:LocalizedRequiredFieldValidator ID="PasswordRequired" runat="server" 
                                                         ControlToValidate="Password"
                                                         LocalizedTag="NEED_PASSWORD"
                                                         CssClass="invalid-feedback"/>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" 
                               AssociatedControlID="ConfirmPassword">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="ConfirmPassword" 
                                 TextMode="Password" 
                                 CssClass="form-control"
                                 required="required" />
                    <YAF:LocalizedRequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" 
                                                         ControlToValidate="ConfirmPassword"
                                                         LocalizedTag="RETYPE_PASSWORD"
                                                         CssClass="invalid-feedback" />
                    <asp:CompareValidator runat="server" 
                                          ControlToCompare="Password" 
                                          ControlToValidate="ConfirmPassword"
                                          CssClass="invalid-feedback" 
                                          Display="Dynamic" 
                                          ErrorMessage='<%# this.GetText("INVALID_MATCH") %>' />
                    
                </div>
                <div class="form-group text-center">
                    <YAF:ThemeButton runat="server" ID="Forgot"
                                     CausesValidation="True"
                                     Icon="key"
                                     TextLocalizedTag="TITLE"
                                     OnClick="ResetClick" />
                </div>
            </asp:Panel>
        </div>
    </div>
</div>