<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Login" CodeBehind="Login.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="UpdateLoginPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Login ID="Login1" runat="server" 
                   RememberMeSet="True" 
                   OnLoginError="Login1_LoginError" 
                   OnLoggedIn="Login1_LoggedIn"
                   OnAuthenticate="Login1_Authenticate" 
                   VisibleWhenLoggedIn="True"
                   CssClass="mx-auto">
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
                                    <i class="fa fa-sign-in-alt fa-fw text-secondary pr-1"></i>
                                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
                                </div>
                                <div class="card-body">
                                    <asp:PlaceHolder runat="server" id="SingleSignOnOptionsRow" Visible="False">
                                            <div class="custom-control custom-radio custom-control-inline">
                                                <asp:RadioButtonList runat="server" id="SingleSignOnOptions"
                                                                     AutoPostBack="true"
                                                                     OnSelectedIndexChanged="SingleSignOnOptionsChanged"
                                                                     RepeatLayout="UnorderedList"
                                                                     CssClass="list-unstyled">
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
                                                
                                                <div class="alert alert-danger CapsLockWarning" style="display: none;" role="alert">
                                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="CAPS_LOCK" />
                                                </div>
                                                
                                            </div>
                                        </asp:PlaceHolder>
                                        <div class="form-row">
                                            <div class="form-group col-md-6">
                                                <div class="custom-control custom-checkbox">
                                                    <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                                </div>
                                            </div>
                                            <div class="form-group col-md-6 text-right">
                                                <asp:Button ID="LoginButton" runat="server" 
                                                            CssClass="btn btn-primary" 
                                                            CommandName="Login" 
                                                            ValidationGroup="Login1" />
                                            </div>
                                        </div>
                                </div>
                                <div class="card-footer">
                                    <asp:Button ID="PasswordRecovery" runat="server" 
                                                CausesValidation="false" 
                                                CssClass="btn btn-secondary btn-sm"
                                                OnClick="PasswordRecovery_Click" />
                                    <asp:PlaceHolder ID="RegisterLinkPlaceHolder" runat="server" Visible="false">
                                        <YAF:ThemeButton ID="RegisterLink" runat="server"
                                                         OnClick="RegisterLinkClick"
                                                         Type="OutlineSecondary"
                                                         Size="Small">
                                            </YAF:ThemeButton>
                                    </asp:PlaceHolder>
                                     <asp:PlaceHolder id="SingleSignOnRow" runat="server" 
                                                      Visible="false">
                                           <YAF:ThemeButton runat="server" ID="FacebookRegister" 
                                                            Type="None"
                                                            Size="Small"
                                                            CssClass="btn btn-social btn-facebook mr-2"
                                                            Icon="facebook"
                                                            IconCssClass="fab"
                                                            Visible="False" 
                                                            OnClick="FacebookFormClick"></YAF:ThemeButton>
                                           <YAF:ThemeButton runat="server" ID="TwitterRegister" 
                                                            Type="None"
                                                            Size="Small"
                                                            Icon="twitter"
                                                            IconCssClass="fab"
                                                            CssClass="btn btn-social btn-twitter mr-2" 
                                                            Visible="False" 
                                                            OnClick="TwitterFormClick"></YAF:ThemeButton>
                                           <YAF:ThemeButton runat="server" ID="GoogleRegister" 
                                                            Type="None"
                                                            Size="Small"
                                                            Icon="google"
                                                            IconCssClass="fab"
                                                            CssClass="btn btn-social btn-google mr-2" 
                                                            Visible="False" 
                                                            OnClick="GoogleFormClick"></YAF:ThemeButton>
                                          
                                           <asp:PlaceHolder id="FacebookHolder" runat="server" 
                                                            Visible="false">
                                               <YAF:ThemeButton id="FacebookLogin" runat="server"
                                                                Type="None"
                                                                Size="Small"
                                                                Icon="facebook"
                                                                IconCssClass="fab"
                                                                CssClass="btn-social btn-facebook mr-2"
                                                                TextLocalizedTag="FACEBOOK_LOGIN" />
                                           </asp:PlaceHolder>
                                           <asp:PlaceHolder id="TwitterHolder" runat="server" 
                                                            Visible="false">
                                               <YAF:ThemeButton id="TwitterLogin" runat="server" 
                                                                Type="None"
                                                                Size="Small"
                                                                Icon="twitter"
                                                                IconCssClass="fab"
                                                                CssClass="btn btn-social btn-twitter mr-2"
                                                                TextLocalizedTag="TWITTER_LOGIN" />
                                           </asp:PlaceHolder>
                                           <asp:PlaceHolder id="GoogleHolder" runat="server" 
                                                            Visible="false">
                                              <YAF:ThemeButton id="GoogleLogin" runat="server" 
                                                               Type="None"
                                                               Size="Small"
                                                               Icon="google"
                                                               IconCssClass="fab"
                                                               CssClass="btn btn-social btn-google mr-2"
                                                               TextLocalizedTag="GOOGLE_LOGIN" />
                                           </asp:PlaceHolder>
                                           <YAF:ThemeButton runat="server" ID="Cancel" 
                                                            Visible="False" 
                                                            OnClick="CancelAuthLoginClick"
                                                            TextLocalizedTag="CANCEL" 
                                                            Type="Secondary" 
                                                            Icon="ban"
                                                            CssClass="ml-1"/>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                </LayoutTemplate>
            </asp:Login>
    </ContentTemplate>
</asp:UpdatePanel>