<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.LoginBox" CodeBehind="LoginBox.ascx.cs" %>

<div id="LoginBox">  
<asp:UpdatePanel ID="UpdateLoginPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div>
            <asp:Login ID="Login1" runat="server" RememberMeSet="True" OnLoginError="Login1_LoginError" OnLoggedIn="Login1_LoggedIn"
                OnAuthenticate="Login1_Authenticate" VisibleWhenLoggedIn="True">
                <LayoutTemplate>
                    <table width="100%" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse">
                        <tr>
                            <td align="left">
                              <div class="header">
                                <h3><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="LOGIN" LocalizedTag="title" /></h3>
                              </div>
                              <div>
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="LOGIN" LocalizedTag="username" />
                                </asp:Label>
                                <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                              </div>
                              <div>
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="password" />
                                            </asp:Label>
                                <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                              </div>
                              <div>
                                <asp:Button ID="LoginButton" runat="server" CssClass="LoginButton" CommandName="Login" ValidationGroup="Login1" />
                                <asp:CheckBox ID="RememberMe" CssClass="RembemberMe" runat="server"></asp:CheckBox>
                              </div>
                              <hr />
                              <div style="margin:5px 0;">
                                 <asp:LinkButton ID="PasswordRecovery" CssClass="RecoveryButtton" runat="server" CausesValidation="false"
                                                OnClick="PasswordRecovery_Click" />
                                 <asp:PlaceHolder ID="FaceBookHolder" runat="server" Visible="false">
                                    <asp:LinkButton runat="server" ID="FacebookRegister" 
                                        CssClass="authLogin facebookLogin" Visible="False" 
                                        OnClick="FacebookRegisterClick">
                                    </asp:LinkButton>
                                 </asp:PlaceHolder>
                                 <asp:PlaceHolder id="TwitterHolder" runat="server" Visible="false">
                                   <asp:LinkButton runat="server" ID="TwitterRegister" 
                                        CssClass="authLogin twitterLogin" Visible="False" 
                                        OnClick="TwitterRegisterClick">
                                    </asp:LinkButton>
                                 </asp:PlaceHolder>
                                 <asp:PlaceHolder id="GoogleHolder" runat="server" Visible="false">
                                     <asp:LinkButton runat="server" ID="GoogleRegister" 
                                        CssClass="authLogin googleLogin" Visible="False" 
                                        OnClick="GoogleRegisterClick">
                                     </asp:LinkButton>
                                 </asp:PlaceHolder>
                               </div>
                                
                            </td>
                        </tr>
                    </table>
                </LayoutTemplate>
            </asp:Login>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
</div>