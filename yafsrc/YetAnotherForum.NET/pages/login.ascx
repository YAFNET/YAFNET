<%@ Control language="c#" CodeFile="login.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.login" %>
<YAF:PageLinks runat="server" id="PageLinks"/>

<div align="center">		
    <asp:Login ID="Login1" runat="server" RememberMeSet="True" OnLoginError="Login1_LoginError" VisibleWhenLoggedIn="False">
        <LayoutTemplate>
            <table align="center" width="100%" border="0" cellpadding="0" cellspacing="0" style="border-collapse: collapse">
                <tr>
                    <td>
                        <table border="0" cellpadding="0" class="content" width="400">
                            <tr>
                                <td align="center" colspan="2" class="header1"><%# GetText("title") %></td>
                            </tr>
                            <tr>
                                <td align="right" class="postheader">
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><%# GetText("username") %></asp:Label></td>
                                <td class="post">
                                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" ErrorMessage="User Name is required."
                                        ToolTip="User Name is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="postheader">
                                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password"><%# GetText("password") %></asp:Label></td>
                                <td class="post">
                                    <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" ErrorMessage="Password is required."
                                        ToolTip="Password is required." ValidationGroup="Login1">*</asp:RequiredFieldValidator>                                    
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" class="post">
                                    <asp:CheckBox ID="RememberMe" runat="server"></asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" class="postfooter" style="height: 24px">   
																		<asp:HyperLink ID="PasswordRecovery" runat="server" /> |                              
                                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="Login1" />
                                </td>
                            </tr>
                        </table>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="False" ValidationGroup="Login1" ShowSummary="False" />
                    </td>
                </tr>
            </table>
        </LayoutTemplate>
    </asp:Login>
 </div>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
