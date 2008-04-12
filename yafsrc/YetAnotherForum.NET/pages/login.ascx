<%@ Control Language="c#" CodeFile="login.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.login" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel ID="UpdateLoginPanel" runat="server">
    <ContentTemplate>
        <div align="center">
            <asp:Login ID="Login1" runat="server" RememberMeSet="True" OnLoginError="Login1_LoginError"
                VisibleWhenLoggedIn="True">
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
                                            </asp:Label></td>
                                        <td class="post">
                                            <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="right" class="postheader" style="height: 24px">
                                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">
                                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="password" />
                                            </asp:Label></td>
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
                                            <asp:HyperLink ID="PasswordRecovery" runat="server" />
                                            |
                                            <asp:Button ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="Login1" />
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
