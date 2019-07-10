﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Dialogs.LoginBox" CodeBehind="LoginBox.ascx.cs" %>



<asp:UpdatePanel ID="UpdateLoginPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Login ID="Login1" runat="server" RememberMeSet="True" OnLoginError="Login1_LoginError" OnLoggedIn="Login1_LoggedIn"
            OnAuthenticate="Login1_Authenticate" VisibleWhenLoggedIn="True">
            <LayoutTemplate>
                <div id="LoginBox" class="modal fade">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">
                                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="LOGIN" LocalizedTag="title" />
                                </h5>
                                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="form-group">
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="LOGIN" LocalizedTag="username" />
                                    </asp:Label>
                                    <div class="input-group">
                                        <div class="input-group-prepend">
                                            <span class="input-group-text"><i class="fas fa-user fa-fw"></i></span>
                                        </div>
                                        <asp:TextBox ID="UserName" runat="server" CssClass="form-control"></asp:TextBox>
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
                                        <asp:TextBox ID="Password" CssClass="form-control" runat="server" TextMode="Password"></asp:TextBox>
                                    </div>
                                </div>
                                <asp:LinkButton ID="PasswordRecovery" CssClass="btn btn-secondary btn-sm mb-3" runat="server" CausesValidation="false"
                                    OnClick="PasswordRecovery_Click" />

                                <div class="alert alert-danger CapsLockWarning" style="display: none;" role="alert">
                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="CAPS_LOCK" />
                                </div>
                                <div class="form-check pl-0">
                                    <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <asp:Button ID="LoginButton" runat="server"
                                    CommandName="Login" ValidationGroup="Login1"
                                    CssClass="btn btn-primary" />
                                <asp:PlaceHolder ID="FaceBookHolder" runat="server" Visible="false">
                                    <asp:LinkButton runat="server" ID="FacebookRegister"
                                        CssClass="authLogin facebookLogin" Visible="True"
                                        OnClick="FacebookRegisterClick">
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="TwitterHolder" runat="server" Visible="false">
                                    <asp:LinkButton runat="server" ID="TwitterRegister"
                                        CssClass="authLogin twitterLogin" Visible="True"
                                        OnClick="TwitterRegisterClick">
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="GoogleHolder" runat="server" Visible="false">
                                    <asp:LinkButton runat="server" ID="GoogleRegister"
                                        CssClass="authLogin googleLogin" Visible="True"
                                        OnClick="GoogleRegisterClick">
                                    </asp:LinkButton>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </div>
            </LayoutTemplate>
        </asp:Login>
    </ContentTemplate>
</asp:UpdatePanel>
