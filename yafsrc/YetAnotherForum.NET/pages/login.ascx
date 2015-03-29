<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.login" CodeBehind="login.ascx.cs" %>
<%@ Import Namespace="YAF.Core.Services" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="UpdateLoginPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div align="center">
            <asp:Login ID="Login1" runat="server" RememberMeSet="True" OnLoginError="Login1_LoginError" OnLoggedIn="Login1_LoggedIn"
             OnAuthenticate="Login1_Authenticate" VisibleWhenLoggedIn="True">
                <LayoutTemplate>
                    <table align="center" width="100%" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse">
                        <tr>
                            <td>
                                <table border="0" cellpadding="0" class="content" width="400">
                                    <tr>
                                        <td align="center" colspan="2" class="header1">
                                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
                                        </td>
                                    </tr>
                                    <tr runat="server" id="SingleSignOnOptionsRow" Visible="False">
                                        <td colspan="2" class="post">
                                            <asp:RadioButtonList runat="server" id="SingleSignOnOptions" AutoPostBack="true"
                                                OnSelectedIndexChanged="SingleSignOnOptionsChanged">
                                            </asp:RadioButtonList>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="UserNameRow">
                                        <td align="right" class="postheader">
                                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
                                            </asp:Label>
                                        </td>
                                        <td class="post">
                                            <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr runat="server" id="PasswordRow">
                                        <td align="right" class="postheader" style="height: 24px">
                                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASSWORD" />
                                            </asp:Label>
                                        </td>
                                        <td class="post" style="height: 24px">
                                            <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox><br/>
                                        </td>
                                    </tr>
                                    <tr class="CapsLockWarning" style="display: none;">
                                        <td colspan="2" class="post">
                                            <div class="ui-widget">
                                                <div class="ui-state-error ui-corner-all" style="padding: 0 .7em;">
                                                    <p><span class="ui-icon ui-icon-alert" style="float: left; margin-right: .3em;"></span>
                                                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="CAPS_LOCK" />
                                                    </p>
                                                 </div>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="post">
                                            <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="postfooter" style="text-align: center;height: 24px">
                                            <asp:Button ID="LoginButton" runat="server" CssClass="pbutton" CommandName="Login" ValidationGroup="Login1" />
                                            <asp:Button ID="PasswordRecovery" runat="server" CausesValidation="false" class="pbutton"
                                                OnClick="PasswordRecovery_Click" />
                                            <asp:PlaceHolder ID="RegisterLinkPlaceHolder" runat="server" Visible="false">
                                                <div>
                                                    <u><asp:LinkButton ID="RegisterLink" runat="server" OnClick="RegisterLinkClick"></asp:LinkButton></u>
                                                </div>
                                            </asp:PlaceHolder>
                                        </td>
                                    </tr>
                                    <tr id="SingleSignOnRow" runat="server" Visible="false">
                                       <td style="text-align:center" colspan="2" class="postfooter">
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
                                       </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </LayoutTemplate>
            </asp:Login>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
