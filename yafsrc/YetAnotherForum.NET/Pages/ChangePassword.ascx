<%@ Control Language="c#" Inherits="YAF.Pages.ChangePassword" Codebehind="ChangePassword.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
                <ul class="nav nav-tabs" id="myTab" role="tablist">
                    <li class="nav-item">
                        <a class="nav-link active" id="password-tab" 
                           data-toggle="tab" 
                           href="#password" 
                           role="tab" 
                           aria-controls="password" 
                           aria-selected="true">
                            <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                LocalizedTag="TITLE" />
                        </a>
                    </li>
                    <li class="nav-item" id="QuestionLink" runat="server">
                        <a class="nav-link" id="question-tab" 
                           data-toggle="tab" 
                           href="#question" role="tab" 
                           aria-controls="question" 
                           aria-selected="false">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                LocalizedTag="TITLE_SECURITY" />
                        </a>
                    </li>
                </ul>

                <div class="tab-content">
                    <div class="tab-pane active" id="password" role="tabpanel" aria-labelledby="password-tab">
                        <div class="card mb-3">
                            <div class="card-header">
                                <i class="fa fa-lock fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TITLE" />
                            </div>
                            <asp:ChangePassword ID="ChangePassword1" runat="server">
        <ChangePasswordTemplate>
            <div class="card-body">
                <div class="form-group">
                        <asp:Label ID="CurrentPasswordLabel" runat="server" 
                                   AssociatedControlID="CurrentPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="OLD_PASSWORD" />
                        </asp:Label>
                        <asp:TextBox ID="CurrentPassword" runat="server" 
                                     TextMode="Password" 
                                     CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" 
                                                    ControlToValidate="CurrentPassword"
                                                    ErrorMessage="Password is required." 
                                                    ToolTip="Password is required." 
                                                    ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                    </div>
                <div class="form-row">
                    <div class="form-group col-md-6">
                        <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NEW_PASSWORD" />
                        </asp:Label>
                        <asp:TextBox ID="NewPassword" runat="server" 
                                     TextMode="Password" 
                                     CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" 
                                                    ControlToValidate="NewPassword"
                                                    ErrorMessage="New Password is required." 
                                                    ToolTip="New Password is required."
                                                    ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group col-md-6">
                        <asp:Label ID="ConfirmNewPasswordLabel" runat="server" 
                                   AssociatedControlID="ConfirmNewPassword">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="CONFIRM_PASSWORD" />
                        </asp:Label>
                        <asp:TextBox ID="ConfirmNewPassword" runat="server" 
                                     TextMode="Password" 
                                     CssClass="form-control"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" 
                                                    ControlToValidate="ConfirmNewPassword"
                                                    ErrorMessage="Confirm New Password is required." 
                                                    ToolTip="Confirm New Password is required."
                                                    ValidationGroup="ctl00$ChangePassword1">*</asp:RequiredFieldValidator>
                    </div>
                </div>
                <small class="form-text text-muted">
                        <asp:CompareValidator ID="NewPasswordCompare" runat="server" 
                                              ControlToCompare="NewPassword"
                                              ControlToValidate="ConfirmNewPassword" 
                                              Display="Dynamic" 
                                              ErrorMessage="The Confirm New Password must match the New Password entry."
                                              ValidationGroup="ctl00$ChangePassword1"></asp:CompareValidator>
                       <asp:CompareValidator ID="NewOldPasswordCompare" 
                                             ControlToValidate="NewPassword" 
                                             ControlToCompare="CurrentPassword" 
                                             Type="String" 
                                             Operator="NotEqual" 
                                             Text="New Password must be different from the old one." 
                                             ValidationGroup="ctl00$ChangePassword1" Runat="Server" /> 
                        <asp:Literal ID="FailureText" runat="server" 
                                     EnableViewState="False"></asp:Literal>
                    </small>
            </div>
        <div class="card-footer text-center">
            <asp:Button ID="ChangePasswordPushButton" runat="server" 
                        CommandName="ChangePassword" 
                        Text="Change Password" 
                        ValidationGroup="ctl00$ChangePassword1"
                        CssClass="btn btn-primary"/>
            <asp:Button ID="CancelPushButton" runat="server" 
                        CausesValidation="False" 
                        CommandName="Cancel"
                        Text="Cancel" 
                        OnClick="CancelPushButton_Click"
                        CssClass="btn btn-secondary"/>
        </div>
        </ChangePasswordTemplate>
        <SuccessTemplate>
            <div class="card-header">
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                    LocalizedTag="TITLE" />
            </div>
            <div class="card-body">
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                    LocalizedTag="CHANGE_SUCCESS" />
            </div>
        </SuccessTemplate>
    </asp:ChangePassword>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" 
                           ShowMessageBox="True"
                           ValidationGroup="ctl00$ChangePassword1" 
                           ShowSummary="False" />
                    </div>
                           
                        </div>
                    <asp:PlaceHolder runat="server" ID="QuestionTab">
                    <div class="tab-pane" id="question" role="tabpanel" aria-labelledby="question-tab">
                    <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-lock fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="TITLE_SECURITY" />
                    </div>
                    <div class="card-body">
                        <form>
                            <div class="form-group">
                                <asp:Label runat="server" ID="Label1" AssociatedControlID="QuestionOld">
                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                        LocalizedTag="SECURITY_QUESTION_OLD" />
                                </asp:Label>
                                <asp:TextBox runat="server" ID="QuestionOld" 
                                             ReadOnly="True"
                                             CssClass="form-control">
                                </asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" ID="Label2" AssociatedControlID="AnswerOld">
                                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="SECURITY_ANSWER_OLD" />
                                </asp:Label>
                                <asp:TextBox ID="AnswerOld" runat="server" CssClass="form-control"></asp:TextBox>
                                <small class="form-text text-muted">
                                    <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" 
                                                                ControlToValidate="AnswerOld"
                                                                ErrorMessage="Answer is required." 
                                                                ToolTip="Answer is required.">*</asp:RequiredFieldValidator>
                                </small>
                            </div>
                            <div class="form-row">
                                <div class="form-group col-md-6">
                                    <asp:Label runat="server" ID="Label3" AssociatedControlID="QuestionNew">
                                        <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="SECURITY_QUESTION_NEW" />
                                    </asp:Label>
                                    <asp:TextBox ID="QuestionNew" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="form-group col-md-6">
                                    <asp:Label runat="server" ID="Label4" AssociatedControlID="AnswerNew">
                                        <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="SECURITY_ANSWER_NEW" />
                                    </asp:Label>
                                    <asp:TextBox ID="AnswerNew" runat="server" CssClass="form-control"></asp:TextBox>
                                    <small class="form-text text-muted">
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                                                                    ControlToValidate="AnswerNew"
                                                                    ErrorMessage="Answer is required." 
                                                                    ToolTip="Answer is required.">*</asp:RequiredFieldValidator>
                                    </small>
                                </div>
                            </div>
                        </form>
                    </div>            
                        <div class="card-footer text-center">
                            <YAF:ThemeButton ID="ChangeSecurityAnswer" runat="server"
                                             TextLocalizedTag="TITLE_SECURITY"
                                             OnClick="ChangeSecurityAnswerClick"
                                             Icon="exchange-alt" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</div>