<%@ Control Language="c#" Inherits="YAF.Pages.Profile.ChangePassword" CodeBehind="ChangePassword.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                LocalizedTag="TITLE"
                                LocalizedPage="CHANGE_PASSWORD"
                                IconName="lock"></YAF:IconHeader>
            </div>
            <asp:Panel runat="server" ID="ContentBody"
                       CssClass="card-body">
                <div class="form-group">
                    <asp:Label ID="CurrentPasswordLabel" runat="server"
                               AssociatedControlID="CurrentPassword">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="OLD_PASSWORD" />
                    </asp:Label>
                    <asp:TextBox ID="CurrentPassword" runat="server"
                                 TextMode="Password"
                                 CssClass="form-control"
                                 required="required"/>
                    <YAF:LocalizedRequiredFieldValidator ID="CurrentPasswordRequired" runat="server"
                                                         ControlToValidate="CurrentPassword"
                                                         LocalizedTag="NEED_OLD_PASSWORD"
                                                         ValidationGroup="ctl00$ChangePassword1" 
                                                         CssClass="invalid-feedback" />
                </div>
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NEW_PASSWORD" />
                        </asp:Label>
                        <asp:TextBox ID="NewPassword" runat="server"
                                     TextMode="Password"
                                     CssClass="form-control"
                                     required="required" />
                        <YAF:LocalizedRequiredFieldValidator ID="NewPasswordRequired" runat="server"
                                                             ControlToValidate="NewPassword"
                                                             LocalizedTag="NEED_NEW_PASSWORD"
                                                             ValidationGroup="ctl00$ChangePassword1" 
                                                             CssClass="invalid-feedback" />
                    </div>
                    <div class="form-group col-md-6">
                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server"
                                   AssociatedControlID="ConfirmNewPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
                        </asp:Label>
                        <asp:TextBox ID="ConfirmNewPassword" runat="server"
                                     TextMode="Password"
                                     CssClass="form-control"
                                     required="required"/>
                        <YAF:LocalizedRequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server"
                                                             ControlToValidate="ConfirmNewPassword"
                                                             LocalizedTag="NEED_NEW_CONFIRM_PASSWORD"
                                                             ValidationGroup="ctl00$ChangePassword1" CssClass="invalid-feedback" />
                    </div>
                </div>
                <div class="form-row">
                    <asp:CompareValidator ID="NewPasswordCompare" runat="server"
                                          ControlToCompare="NewPassword"
                                          ControlToValidate="ConfirmNewPassword"
                                          Display="Dynamic"
                                          ValidationGroup="ctl00$ChangePassword1" CssClass="invalid-feedback"></asp:CompareValidator>
                    <asp:CompareValidator ID="NewOldPasswordCompare"
                                          ControlToValidate="NewPassword"
                                          ControlToCompare="CurrentPassword"
                                          Display="Dynamic"
                                          Type="String"
                                          Operator="NotEqual"
                                          ValidationGroup="ctl00$ChangePassword1" runat="Server" CssClass="invalid-feedback" />
                </div>
            </asp:Panel>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="ChangePasswordPushButton" runat="server"
                                 CausesValidation="True"
                                 Type="Primary"
                                 TextLocalizedTag="CHANGE_BUTTON"
                                 ValidationGroup="ctl00$ChangePassword1"
                                 Icon="key"
                                 OnClick="ChangePasswordClick"/>
                <YAF:ThemeButton ID="CancelPushButton" runat="server"
                                 CausesValidation="False"
                                 TextLocalizedTag="CANCEL"
                                 Icon="times"
                                 OnClick="CancelPushButtonClick"
                                 Type="Secondary" />
            </div>
        </div>
    </div>
</div>
