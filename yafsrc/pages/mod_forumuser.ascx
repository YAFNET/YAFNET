<%@ Control Language="c#" Codebehind="mod_forumuser.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.mod_forumuser" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<table class="content" cellspacing="1" cellpadding="0" width="100%" >
    <tr>
        <td class="header1" colspan="2">
            <%= GetText("TITLE") %>
        </td>
    </tr>
    <tr>
        <td class="postheader" width="50%">
            <%= GetText("USER") %>
        </td>
        <td class="post" width="50%">
            <asp:TextBox runat="server" ID="UserName" /><asp:DropDownList runat="server" ID="ToList"
                Visible="false" />
            <asp:Button runat="server" ID="FindUsers" /></td>
    </tr>
    <tr>
        <td class="postheader">
            <%=GetText("ACCESSMASK")%>
        </td>
        <td class="post">
            <asp:DropDownList runat="server" ID="AccessMaskID" /></td>
    </tr>
    <tr class="footer1">
        <td colspan="2" align="center">
            <asp:Button runat="server" ID="Update" />
            <asp:Button runat="server" ID="Cancel" />
        </td>
    </tr>
</table>

<YAF:SmartScroller ID="SmartScroller1" runat="server" />
