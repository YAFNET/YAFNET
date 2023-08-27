<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersResetPass"CodeBehind="EditUsersResetPass.ascx.cs" %>

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
<div class="row">
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                       AssociatedControlID="txtNewPassword"
                       LocalizedTag="CHANGE_NEW_PASS" LocalizedPage="ADMIN_EDITUSER" />

        <asp:ValidationSummary ID="Summary1" runat="server" ValidationGroup="passchange" />
        <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" CssClass="form-control" />
        <asp:RequiredFieldValidator ID="PasswordValidator" runat="server" ValidationGroup="passchange"
                                    ControlToValidate="txtNewPassword" SetFocusOnError="true"
                                    Display="None" />
    </div>
    <div class="mb-3 col-md-4">
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

    <div class="mb-3">
        <YAF:HelpLabel ID="HelpLabel3" runat="server" 
                       AssociatedControlID="chkEmailNotify"
                       LocalizedTag="CHANGE_PASS_NOTIFICATION" LocalizedPage="ADMIN_EDITUSER" />
        <div class="form-check form-switch">
            <asp:CheckBox runat="server" ID="chkEmailNotify" />
        </div>
    </div>

<div class="text-center">
    <asp:LinkButton ID="ChangePassword" runat="server"
                    CssClass="btn btn-primary"
                    ValidationGroup="passchange" 
                    OnClick="ChangePassword_Click"
                    CausesValidation="true" />
</div>