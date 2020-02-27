<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.LoginBox" CodeBehind="LoginBox.ascx.cs" %>

<asp:UpdatePanel ID="UpdateLoginPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Login ID="Login1" runat="server"
            RememberMeSet="True"
            OnLoginError="Login1_LoginError"
            OnLoggedIn="Login1_LoggedIn"
            OnAuthenticate="Login1_Authenticate"
            VisibleWhenLoggedIn="True">
            <LayoutTemplate>
                <div id="LoginBox" class="modal fade" role="dialog">
                    <div class="modal-dialog modal-lg" role="document">
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
                                <div class="container-fluid">
                                    <div class="row">
                                        <div class="col-md-12 col-lg-auto flex-grow-1">
                                            <div class="form-group">
                                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                        LocalizedPage="LOGIN"
                                                        LocalizedTag="username" />
                                                </asp:Label>
                                                <div class="input-group">
                                                    <div class="input-group-prepend">
                                                        <span class="input-group-text"><i class="fas fa-user fa-fw"></i></span>
                                                    </div>
                                                    <asp:TextBox ID="UserName" runat="server"
                                                        placeholder='<%# this.GetText("USERNAME") %>'
                                                        CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="password" />
                                                </asp:Label>
                                                <div class="input-group">
                                                    <div class="input-group-prepend">
                                                        <span class="input-group-text"><i class="fas fa-lock fa-fw"></i></span>
                                                    </div>
                                                    <asp:TextBox ID="Password" runat="server"
                                                        CssClass="form-control"
                                                        placeholder='<%# this.GetText("PASSWORD") %>'
                                                        TextMode="Password"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="alert alert-danger CapsLockWarning" style="display: none;" role="alert">
                                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="CAPS_LOCK" />
                                            </div>
                                            <div class="form-row">
                                                <div class="form-group col-md-6">
                                                    <div class="custom-control custom-checkbox">
                                                        <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                                    </div>
                                                </div>
                                                <div class="form-group col-md-6 text-right">
                                                    <asp:LinkButton ID="PasswordRecovery" runat="server"
                                                        CssClass="btn btn-secondary btn-sm mb-3"
                                                        CausesValidation="false"
                                                        OnClick="PasswordRecovery_Click" />
                                                </div>
                                            </div>
                                        </div>
                                        <asp:PlaceHolder ID="SingleSignOnHolder" runat="server" Visible="true">
                                            <div class="col-md-12 col-lg-5">
                                                <div class="d-flex h-100 d-inline-block">
                                                    <div class="d-none d-lg-block border-left mr-4"></div>
                                                    <div class="w-100">
                                                        <asp:PlaceHolder ID="FaceBookHolder" runat="server" Visible="false">
                                                            <YAF:ThemeButton runat="server" ID="FacebookRegister"
                                                                Visible="True"
                                                                Type="None"
                                                                Size="Small"
                                                                CssClass="btn btn-sm btn-block btn-social btn-facebook mr-2 mb-1"
                                                                Icon="facebook-f"
                                                                IconCssClass="fab"
                                                                OnClick="FacebookRegisterClick">
                                                            </YAF:ThemeButton>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="TwitterHolder" runat="server" Visible="false">
                                                            <YAF:ThemeButton runat="server" ID="TwitterRegister"
                                                                Visible="True"
                                                                Type="None"
                                                                Size="Small"
                                                                CssClass="btn btn-sm btn-block btn-social btn-twitter mr-2 mb-1"
                                                                Icon="twitter"
                                                                IconCssClass="fab"
                                                                OnClick="TwitterRegisterClick">
                                                            </YAF:ThemeButton>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="GoogleHolder" runat="server" Visible="false">
                                                            <YAF:ThemeButton runat="server" ID="GoogleRegister"
                                                                Visible="True"
                                                                Type="None"
                                                                Size="Small"
                                                                CssClass="btn btn-sm btn-block btn-social btn-google mr-2 mb-1"
                                                                Icon="google"
                                                                IconCssClass="fab"
                                                                OnClick="GoogleRegisterClick">
                                                            </YAF:ThemeButton>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer clearfix">
                                <asp:LinkButton ID="RegisterLink" runat="server"
                                    CssClass="btn btn-secondary mr-auto"
                                    CausesValidation="false"
                                    OnClick="RegisterLink_Click" />

                                <asp:Button ID="LoginButton" runat="server"
                                    CommandName="Login"
                                    ValidationGroup="Login1"
                                    CssClass="btn btn-primary" />
                            </div>
                        </div>
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>
    </ContentTemplate>
</asp:UpdatePanel>
