<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersResetPass"CodeBehind="EditUsersResetPass.ascx.cs" %>
<asp:UpdatePanel ID="PasswordUpdatePanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        
            
                <h2>
                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_USER_RESETPASS" LocalizedPage="ADMIN_EDITUSER" />
                </h2>
            <hr />
            <asp:PlaceHolder ID="PasswordResetErrorHolder" runat="server" Visible="false">
                
                    <h4>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ERROR_NOTE_PASSRESET" LocalizedPage="ADMIN_EDITUSER" />
                    </h4>
                <hr />
            </asp:PlaceHolder>
            
                <h4>
                    <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASS_OPTION" LocalizedPage="ADMIN_EDITUSER" /></strong>
                </h4>
                <p>
                    <asp:RadioButtonList ID="rblPasswordResetFunction" runat="server" AutoPostBack="true"
                        OnSelectedIndexChanged="rblPasswordResetFunction_SelectedIndexChanged" CssClass="form-control">
                    </asp:RadioButtonList>
                </p>
            <asp:PlaceHolder ID="ChangePasswordHolder" runat="server" Visible="false">
                <hr />
                <h4>
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEAD_USER_CHANGEPASS" LocalizedPage="ADMIN_EDITUSER" />
                </h4>
                <div class="alert alert-warning" role="alert">
                        <strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="PASSWORD_REQUIREMENTS" LocalizedPage="ADMIN_EDITUSER" /></strong>
                        <asp:Label ID="lblPassRequirements" runat="server"></asp:Label>
                          </div>
                <hr />
                
                    <h4>
                        <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CHANGE_NEW_PASS" LocalizedPage="ADMIN_EDITUSER" />
                    </h4>
                      <asp:ValidationSummary ID="Summary1" runat="server" ValidationGroup="passchange" />
                     <p><asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="PasswordValidator" runat="server" ValidationGroup="passchange"
                            ControlToValidate="txtNewPassword" SetFocusOnError="true"
                            Display="None" />
                    </p>
                <hr />
                
                    <h4>
                        <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="CONFIRM_PASS" LocalizedPage="ADMIN_EDITUSER" />
                    </h4>
                    <p>
                        <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="passchange"
                            ControlToValidate="txtConfirmPassword"
                            SetFocusOnError="true" Display="None" />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ValidationGroup="passchange"
                            ControlToCompare="txtNewPassword"
                            ControlToValidate="txtConfirmPassword" SetFocusOnError="true" Display="None" />
                    </p>
                <hr />
                
                    <h4>
                        <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="CHANGE_PASS_NOTIFICATION" LocalizedPage="ADMIN_EDITUSER" />
                    </h4>
                    <p>
                        <asp:CheckBox CssClass="form-control" runat="server" ID="chkEmailNotify" />
                    </p>
      
            </asp:PlaceHolder>
            

    
                <div class="text-lg-center">

                    <asp:LinkButton ID="btnChangePassword" Visible="false" runat="server"
                        Type="Primary" ValidationGroup="passchange" OnClick="btnChangePassword_Click"
                        CausesValidation="true" />
                    &nbsp;
                    <asp:LinkButton ID="btnResetPassword" runat="server" Type="Primary"
                        OnClick="btnResetPassword_Click" CausesValidation="false" />
                    </div>
      
    </ContentTemplate>
</asp:UpdatePanel>