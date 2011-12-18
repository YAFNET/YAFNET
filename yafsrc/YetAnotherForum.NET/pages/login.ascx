<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.login" CodeBehind="login.ascx.cs" %>
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
                                    <tr>
                                        <td align="right" class="postheader">
                                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">
                                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
                                            </asp:Label>
                                        </td>
                                        <td class="post">
                                            <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" class="postheader" style="height: 24px">
                                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PASSWORD" />
                                            </asp:Label>
                                        </td>
                                        <td class="post" style="height: 24px">
                                            <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2" class="post">
                                            <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="2" class="postfooter" style="height: 24px">
                                            <asp:Button ID="LoginButton" runat="server" class="pbutton" CommandName="Login" ValidationGroup="Login1" />
                                            |
                                            <asp:Button ID="PasswordRecovery" runat="server" CausesValidation="false" class="pbutton"
                                                OnClick="PasswordRecovery_Click" />
                                        </td>
                                    </tr>
                                    <tr id="SingleSignOnRow" runat="server" Visible="false">
                                       <td style="text-align:center" colspan="2" class="postfooter">
                                         <asp:PlaceHolder id="FacebookHolder" runat="server" Visible="false">
                                           <fb:login-button onlogin="LoginUser()" perms="email,user_birthday,status_update,publish_stream">
                                             <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="FACEBOOK_LOGIN" />
                                           </fb:login-button>
                                           <div id="fb-root"></div>
                                         </asp:PlaceHolder>
                                         <asp:PlaceHolder id="TwitterHolder" runat="server" Visible="false">
                                           <button id="TwitterLogin" runat="server" style="background:none;border:none;cursor:pointer;padding:3px;">
                                           </button>
                                         </asp:PlaceHolder>
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
