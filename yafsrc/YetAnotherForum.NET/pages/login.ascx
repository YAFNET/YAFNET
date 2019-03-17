<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.login" CodeBehind="login.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="UpdateLoginPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div align="center">
            <asp:Login ID="Login1" runat="server" RememberMeSet="True" OnLoginError="Login1_LoginError" OnLoggedIn="Login1_LoggedIn"
             OnAuthenticate="Login1_Authenticate" VisibleWhenLoggedIn="True">
                <LayoutTemplate>
                    <div class="row">
                        <div class="col-xl-12">
                            <h2><YAF:LocalizedLabel runat="server" LocalizedTag="TITLE" /></h2>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <div class="card mb-3">
                                <div class="card-header">
                                    <i class="fa fa-sign-in-alt fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
                                </div>
                                <div class="card-body">
                                    <form>
                                        <asp:PlaceHolder runat="server" id="SingleSignOnOptionsRow" Visible="False">
                                            <div class="form-group">
                                                <asp:RadioButtonList runat="server" id="SingleSignOnOptions"
                                                                     AutoPostBack="true"
                                                                     CssClass="form-control"
                                                                     OnSelectedIndexChanged="SingleSignOnOptionsChanged">
                                                </asp:RadioButtonList>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" id="UserNameRow">
                                            <div class="form-group">
                                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
                                                </asp:Label>
                                                <asp:TextBox ID="UserName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder runat="server" id="PasswordRow">
                                            <div class="form-group">
                                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASSWORD" />
                                                </asp:Label>
                                                <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox><br/>
                                                <asp:Button ID="PasswordRecovery" runat="server" CausesValidation="false" class="btn btn-secondary btn-sm"
                                                            OnClick="PasswordRecovery_Click" />
                                                <div class="alert alert-danger CapsLockWarning" style="display: none;" role="alert">
                                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="CAPS_LOCK" />
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                        <div class="form-group form-check">
                                            <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                        </div>
                                    </form>
                                </div>
                                <div class="card-footer">
                                    <asp:Button ID="LoginButton" runat="server" CssClass="btn btn-primary" 
                                                CommandName="Login" ValidationGroup="Login1" />
                                    <asp:PlaceHolder ID="RegisterLinkPlaceHolder" runat="server" Visible="false">
                                       
                                            <asp:LinkButton ID="RegisterLink" runat="server" 
                                                            OnClick="RegisterLinkClick" 
                                                            CssClass="btn btn-secondary"></asp:LinkButton>
                                    </asp:PlaceHolder>
                                     <asp:PlaceHolder id="SingleSignOnRow" runat="server" Visible="false">
                                           <asp:LinkButton runat="server" ID="FacebookRegister" CssClass="authLogin facebookLogin" Visible="False" OnClick="FacebookFormClick"></asp:LinkButton>
                                           <asp:LinkButton runat="server" ID="TwitterRegister" CssClass="authLogin twitterLogin" Visible="False" OnClick="TwitterFormClick"></asp:LinkButton>
                                           <asp:LinkButton runat="server" ID="GoogleRegister" CssClass="authLogin googleLogin" Visible="False" OnClick="GoogleFormClick"></asp:LinkButton>
                                          
                                           <asp:PlaceHolder id="FacebookHolder" runat="server" Visible="false">
                                              <a id="FacebookLogin" runat="server" class="authLogin facebookLogin">
                                                   <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="FACEBOOK_LOGIN" />
                                               </a>
                                           </asp:PlaceHolder>
                                           <asp:PlaceHolder id="TwitterHolder" runat="server" Visible="false">
                                               <a id="TwitterLogin" runat="server" class="authLogin twitterLogin">
                                                   <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TWITTER_LOGIN" />
                                               </a>
                                           </asp:PlaceHolder>
                                           <asp:PlaceHolder id="GoogleHolder" runat="server" Visible="false">
                                              <a id="GoogleLogin" runat="server" class="authLogin googleLogin">
                                                   <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="GOOGLE_LOGIN" />
                                               </a>
                                           </asp:PlaceHolder>
                                           <asp:Button runat="server" ID="Cancel" CssClass="pbutton" Visible="False" OnClick="CancelAuthLoginClick" />
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                </LayoutTemplate>
            </asp:Login>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
