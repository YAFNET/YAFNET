<%@ Control Language="c#" CodeFile="im_email.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.im_email" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="400px" border="0" cellpadding="0" cellspacing="1" align="center">
    <tr class="header2">
        <td colspan="2">
            <%=GetText("TITLE")%>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <%=GetText("SUBJECT")%>
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Subject" /></td>
    </tr>
    <tr>
        <td class="postheader" valign='top'>
            <%=GetText("BODY")%>
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Body" TextMode="multiline" Rows='10' Style='width: 100%' />
        </td>
    </tr>
    <tr class="postfooter">
        <td colspan="2" align="center">
            <asp:Button runat="server" ID="Send" OnClick="Send_Click" />
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
