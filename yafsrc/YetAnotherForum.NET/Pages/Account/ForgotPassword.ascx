<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Pages.Account.ForgotPassword" Codebehind="ForgotPassword.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="row">
    <div class="col">
        <div class="card w-25 mx-auto">
            <asp:Panel runat="server" ID="ContentBody"
                       CssClass="card-body">
                <h5 class="card-title">
                    <YAF:LocalizedLabel runat="server" LocalizedTag="PAGE1_INSTRUCTIONS"></YAF:LocalizedLabel>
                </h5>
                <div class="form-group">
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedPage="LOGIN" LocalizedTag="USERNAME" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="UserName"
                                 CssClass="form-control"
                                 required="required"></asp:TextBox>
                    <YAF:LocalizedRequiredFieldValidator runat="server"
                                                         ControlToValidate="UserName"
                                                         LocalizedTag="NEED_USERNAME"
                                                         CssClass="invalid-feedback"/>
                </div>
                <div class="form-group">
                    <YAF:ThemeButton runat="server" ID="Forgot"
                                     CausesValidation="True"
                                     TextLocalizedTag="SUBMIT"
                                     CssClass="btn-block"
                                     OnClick="ForgotPasswordClick"/> 
                </div>
            </asp:Panel>
        </div>
    </div>
</div>

