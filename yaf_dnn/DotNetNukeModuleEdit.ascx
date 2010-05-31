<%@ Control Language="c#" Inherits="YAF.DotNetNukeModuleEdit" AutoEventWireup="false" Codebehind="DotNetNukeModuleEdit.ascx.cs" %>
<table width="100%" cellspacing="0" cellpadding="2" summary="importantmessagesdev edit design table">
    <tr>
        <td width="50%" class="SubHead">
            Select Board to use in this module.
        </td>
        <td width="50%">
            <asp:DropDownList AutoPostBack="true" CssClass="NormalTextBox" runat="server" ID="BoardID" />
        </td>
    </tr>
    <tr>
        <td width="50%" class="SubHead">
            Select Category to show in this module.
        </td>
        <td width="50%">
            <asp:DropDownList CssClass="NormalTextBox" runat="server" ID="CategoryID" />
        </td>
    </tr>
</table>
<p>
    <asp:LinkButton runat="server" ID="update" CssClass="CommandButton" />
    &nbsp;
    <asp:LinkButton runat="server" ID="cancel" CssClass="CommandButton" />
    &nbsp;
    <asp:LinkButton runat="server" ID="create" CssClass="CommandButton" />
</p>
