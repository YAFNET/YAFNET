<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersResetPass"CodeBehind="EditUsersResetPass.ascx.cs" %>

<asp:UpdatePanel ID="PasswordUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <h2>
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="HEAD_USER_RESETPASS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </h2>
        <hr />
        <asp:PlaceHolder ID="PasswordResetErrorHolder" runat="server" Visible="false">
            <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                    LocalizedTag="ERROR_NOTE_PASSRESET" 
                                    LocalizedPage="ADMIN_EDITUSER" />
            </h4>
            <hr />
        </asp:PlaceHolder>
        <h4>
            <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                        LocalizedTag="PASS_OPTION" 
                                        LocalizedPage="ADMIN_EDITUSER" /></strong>
        </h4>
        <div class="form-group">
            <div class="custom-control custom-radio custom-control-inline">
                <asp:RadioButtonList ID="rblPasswordResetFunction" runat="server" 
                                     AutoPostBack="true"
                                     OnSelectedIndexChanged="rblPasswordResetFunction_SelectedIndexChanged" 
                                     RepeatLayout="UnorderedList"
                                     CssClass="list-unstyled">
                </asp:RadioButtonList>
            </div>
        </div>
        <asp:PlaceHolder ID="ChangePasswordHolder" runat="server" Visible="false">
                <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                        LocalizedTag="HEAD_USER_CHANGEPASS" 
                                        LocalizedPage="ADMIN_EDITUSER" />
                </h4>
                <YAF:Alert runat="server" Type="warning">
                    <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                LocalizedTag="PASSWORD_REQUIREMENTS" 
                                                LocalizedPage="ADMIN_EDITUSER" />
                    </strong>
                    <asp:Label ID="lblPassRequirements" runat="server"></asp:Label>
                </YAF:Alert>
                <hr />
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                       AssociatedControlID="txtNewPassword"
                                       LocalizedTag="CHANGE_NEW_PASS" LocalizedPage="ADMIN_EDITUSER" />
                    
                        <asp:ValidationSummary ID="Summary1" runat="server" ValidationGroup="passchange" />
                        <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="PasswordValidator" runat="server" ValidationGroup="passchange"
                                                    ControlToValidate="txtNewPassword" SetFocusOnError="true"
                                                    Display="None" />
                    </div>
                    <div class="form-group col-md-6">
                        <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                       AssociatedControlID="txtConfirmPassword"
                                       LocalizedTag="CONFIRM_PASS" LocalizedPage="ADMIN_EDITUSER" />
                        <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="passchange"
                                                    ControlToValidate="txtConfirmPassword"
                                                    SetFocusOnError="true" Display="None" />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="passchange"
                                              ControlToCompare="txtNewPassword"
                                              ControlToValidate="txtConfirmPassword" SetFocusOnError="true" Display="None" />
                    </div>
                </div>
                <div class="form-group">
                    <YAF:HelpLabel ID="HelpLabel3" runat="server" 
                                   AssociatedControlID="chkEmailNotify"
                                   LocalizedTag="CHANGE_PASS_NOTIFICATION" LocalizedPage="ADMIN_EDITUSER" />
                    <div class="custom-control custom-switch">
                        <asp:CheckBox runat="server" ID="chkEmailNotify"
                                      Text="&nbsp;" />
                    </div>
                </div>
            </asp:PlaceHolder>
        <div class="text-center">
            <asp:LinkButton ID="btnChangePassword" runat="server"
                            Visible="false" 
                            CssClass="btn btn-primary"
                            ValidationGroup="passchange" 
                            OnClick="btnChangePassword_Click"
                            CausesValidation="true" />
            <asp:LinkButton ID="btnResetPassword" runat="server"
                            CssClass="btn btn-primary"
                            OnClick="btnResetPassword_Click" 
                            CausesValidation="false" />
        </div>
    </ContentTemplate>
</asp:UpdatePanel>