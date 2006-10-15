<%@ Control Language="C#" AutoEventWireup="true" Codebehind="EditUsersPoints.ascx.cs" Inherits="yaf.controls.EditUsersPoints" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            User Points</td>
    </tr>
    <tr>
        <td class="postheader">
            Current Points:</td>
        <td class="post">
            <asp:Literal ID="ltrCurrentPoints" runat="server" /></td>
    </tr>
    <tr>
        <td class="postheader">
            Set Points:</td>
        <td class="post">
            <asp:TextBox runat="server" ID="txtUserPoints" ValidationGroup="UserPoints" />
            <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Please enter a number" ControlToValidate="txtUserPoints"
                SetFocusOnError="true" ValidationGroup="UserPoints" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:Button runat="server" ID="btnUserPoints" OnClick="SetUserPoints_Click" Text="Go" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            Add Points:</td>
        <td class="post">
            <asp:TextBox runat="server" ID="txtAddPoints" ValidationGroup="Add" />
            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Number Please" ControlToValidate="txtAddPoints" SetFocusOnError="true"
                ValidationGroup="Add" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:Button runat="server" ID="btnAddPoints" OnClick="AddPoints_Click" Text="Go" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            Remove Points:</td>
        <td class="post">
            <asp:TextBox runat="server" ID="txtRemovePoints" ValidationGroup="Remove" />
            <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Number Please" ControlToValidate="txtRemovePoints" SetFocusOnError="true"
                ValidationGroup="Remove" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:Button runat="server" ID="Button1" OnClick="RemovePoints_Click" Text="Go" />
        </td>
    </tr>
    <tr>
        <td class="postfooter" colspan="2" align="center">
            &nbsp;
        </td>
    </tr>
</table>
