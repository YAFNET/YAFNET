<%@ Control Language="C#" AutoEventWireup="true" CodeFile="recoverpassword.ascx.cs"
    Inherits="YAF.Pages.recoverpassword" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div align="center">
    <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" OnSendingMail="PasswordRecovery1_SendingMail">
        <UserNameTemplate>
            <table border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse">
                <tr>
                    <td>
                        <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                            <tr>
                                <td colspan="2" align="center" class="header1">
                                    <%# GetText("TITLE") %>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" class="post">
                                    <%# GetText("PAGE1_INSTRUCTIONS") %>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="postheader">
                                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName"><%# GetText("LOGIN","USERNAME") %></asp:Label></td>
                                <td class="post">
                                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                        ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="PasswordRecovery1">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" style="color: red">
                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2" class="postfooter">
                                    <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" ValidationGroup="PasswordRecovery1" />
                                </td>
                            </tr>
                        </table>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                            ValidationGroup="PasswordRecovery1" ShowSummary="False" />
                    </td>
                </tr>
            </table>
        </UserNameTemplate>
        <QuestionTemplate>
            <table border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse">
                <tr>
                    <td>
                        <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                            <tr>
                                <td colspan="2" align="center" class="header1">
                                    <%# GetText( "IDENTITY_CONFIRMATION_TITLE" )%>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" class="post">
                                    <%# GetText("PAGE2_INSTRUCTIONS") %>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="postheader">
                                    <%# GetText("LOGIN","USERNAME") %>
                                </td>
                                <td class="post">
                                    <asp:Literal ID="UserName" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="postheader">
                                    <%# GetText("REGISTER","SECURITY_QUESTION") %>
                                </td>
                                <td class="post">
                                    <asp:Literal ID="Question" runat="server"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" class="postheader">
                                    <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer"><%# GetText("REGISTER","SECURITY_ANSWER") %></asp:Label></td>
                                <td class="post">
                                    <asp:TextBox ID="Answer" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
                                        ErrorMessage="Answer is required." ToolTip="Answer is required." ValidationGroup="PasswordRecovery2">*</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="2" style="color: red">
                                    <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2" class="postfooter">
                                    <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Submit" ValidationGroup="PasswordRecovery2" />
                                </td>
                            </tr>
                        </table>
                        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowMessageBox="True"
                            ValidationGroup="PasswordRecovery2" ShowSummary="False" />
                    </td>
                </tr>
            </table>
        </QuestionTemplate>
        <SuccessTemplate>
            <table border="0" cellpadding="1" cellspacing="0" style="border-collapse: collapse;">
                <tr>
                    <td>
                        <table class="content" cellspacing="1" cellpadding="0" border="0" width="600">
                            <tr>
                                <td colspan="2" align="center" class="header1">
                                    <%# GetText( "TITLE" )%>
                                </td>
                            </tr>
                            <tr>
                                <td class="post">
                                    <%# GetText( "PASSWORD_SENT" )%>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="2" class="postfooter">
                                    <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Continue"
                                        OnClick="SubmitButton_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </SuccessTemplate>
    </asp:PasswordRecovery>
</div>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
