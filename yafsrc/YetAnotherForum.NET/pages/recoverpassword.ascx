<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Pages.RecoverPassword" Codebehind="RecoverPassword.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>
<div class="row">
<div class="col-xl-12">
	<asp:PasswordRecovery ID="PasswordRecovery1" runat="server" 
                          OnSendingMail="PasswordRecovery1_SendingMail" 
                          OnVerifyingUser="PasswordRecovery1_VerifyingUser" 
                          OnSendMailError="PasswordRecovery1_SendMailError" 
                          OnVerifyingAnswer="PasswordRecovery1_VerifyingAnswer" 
                          OnAnswerLookupError="PasswordRecovery1_AnswerLookupError" 
                          Width="100%">
		<UserNameTemplate>
                <div class="card">
                    <div class="card-header">
                        <i class="fa fa-key fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
                    </div>
                    <div class="card-body">
                        <h5 class="card-subtitle mb-2 text-muted">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="PAGE1_INSTRUCTIONS" />
                        </h5>
                        <form>
                            <div class="form-group">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="LOGIN" LocalizedTag="USERNAME" />
                                </asp:Label>
                                <asp:TextBox ID="UserName" runat="server" CssClass="form-control"></asp:TextBox>
                                <div class="invalid-feedback d-block">
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server"
                                                                ControlToValidate="UserName"
                                                                ErrorMessage="User Name is required." 
                                                                ToolTip="User Name is required."
                                                                ValidationGroup="PasswordRecovery1"
                                                                CssClass="d-none"
                                                                Visible="False">*</asp:RequiredFieldValidator>
                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server"
                                                           ValidationGroup="PasswordRecovery1" />
                                </div>
                            </div>
                            <asp:Button ID="SubmitButton" runat="server" 
                                        CommandName="Submit" 
                                        CssClass="btn btn-primary" 
                                        Text="Submit" 
                                        ValidationGroup="PasswordRecovery1" />
                        </form>
                    </div>
                </div>
		</UserNameTemplate>
		<QuestionTemplate>
                <div class="card">
                    <div class="card-header">
                        <i class="fa fa-key fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="IDENTITY_CONFIRMATION_TITLE" />
                    </div>
                    <div class="card-body">
                        <h5 class="card-subtitle mb-2 text-muted">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="PAGE2_INSTRUCTIONS" />
                        </h5>
                        <form>
                            <div class="form-group">
                                <label><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="LOGIN" LocalizedTag="USERNAME" /></label>
                                <input type="text" readonly class="form-control-plaintext" value='<asp:Literal ID="UserName" runat="server"></asp:Literal>' />
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server">
                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="REGISTER"
                                                                              LocalizedTag="SECURITY_QUESTION" />
                                </asp:Label>
                                <input type="text" readonly 
                                       class="form-control-plaintext" 
                                       value='<asp:Literal ID="Question" runat="server"></asp:Literal>' />
                                
                            </div>
                            <div class="form-group">
                                <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">
                                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="REGISTER"
                                                        LocalizedTag="SECURITY_ANSWER" />
                                </asp:Label>
                                <asp:TextBox ID="Answer" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" 
                                                            ControlToValidate="Answer"
                                                            ErrorMessage="Answer is required." 
                                                            ToolTip="Answer is required." 
                                                            CssClass="d-none"
                                                            ValidationGroup="PasswordRecovery2">*</asp:RequiredFieldValidator>
                            </div>
                            <div class="invalid-feedback d-block">
                                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="PasswordRecovery2" ShowSummary="True" />
                            </div>
                            <asp:Button ID="SubmitButton" runat="server" 
                                        CommandName="Submit" 
                                        CssClass="btn btn-primary" 
                                        Text="Submit" 
                                        ValidationGroup="PasswordRecovery2" />
                        </form>
                    </div>
                </div>
		</QuestionTemplate>
		<SuccessTemplate>
                <div class="card">
                    <div class="card-header">
                        <i class="fa fa-key fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="TITLE" />
                    </div>
                    <div class="card-body">
                        <p class="card-text">
                            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="PASSWORD_SENT" />
                        </p>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="SubmitButton" runat="server" 
                                         NavigateUrl="<%# BuildLink.GetLink(ForumPages.Login) %>"
                                         Type="Secondary"
                                         TextLocalizedTag="BACK"
                                         Icon="arrow-circle-left"/>
                    </div>
                </div>
            </div>
		</SuccessTemplate>
	</asp:PasswordRecovery>
    </div>
</div>