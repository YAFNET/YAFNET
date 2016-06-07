<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersPoints" Codebehind="EditUsersPoints.ascx.cs" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="USER_REPUTATION" LocalizedPage="ADMIN_EDITUSER" />
        </td>
    </tr>
    <tr>
			<td class="header2" height="30" colspan="2">
			</td>
		</tr>
	<tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="CURRENT_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:Literal ID="ltrCurrentPoints" runat="server" /></td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SET_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="txtUserPoints" ValidationGroup="UserPoints" />
            <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Please enter a number" ControlToValidate="txtUserPoints"
                SetFocusOnError="true" ValidationGroup="UserPoints" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:Button runat="server" ID="btnUserPoints" CssClass="pbutton" OnClick="SetUserPoints_Click" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ADD_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="txtAddPoints" ValidationGroup="Add" />
            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Number Please" ControlToValidate="txtAddPoints" SetFocusOnError="true"
                ValidationGroup="Add" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:Button runat="server" ID="btnAddPoints" CssClass="pbutton" OnClick="AddPoints_Click" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="REMOVE_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="txtRemovePoints" ValidationGroup="Remove" />
            <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Number Please" ControlToValidate="txtRemovePoints" SetFocusOnError="true"
                ValidationGroup="Remove" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:Button runat="server" ID="Button1" CssClass="pbutton" OnClick="RemovePoints_Click" />
        </td>
    </tr>
    <tr>
        <td class="postfooter" colspan="2" align="center">
            &nbsp;
        </td>
    </tr>
</table>
