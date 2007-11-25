<%@ Control Language="c#" CodeFile="im_icq.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.im_icq" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="400px" border="0" cellpadding="0" cellspacing="1" align="center">
    <tr class="header2">
        <td colspan="2">
            <img runat="server" id="Status" style="vertical-align: middle" /><%=GetText("TITLE")%></td>
    </tr>
    <tr>
        <td class="postheader">
            <%=GetText("NAME")%>
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="From" size="15" MaxLength="40" Style="width: 100%"
                Enabled="false" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <%=GetText("EMAIL")%>
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Email" size="15" MaxLength="50" Style="width: 100%"
                Enabled="false" />
        </td>
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
            <asp:Button runat="server" ID="Send" />
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller id="SmartScroller1" runat="server" />
</div>
