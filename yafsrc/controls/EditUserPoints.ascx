<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUserPoints.ascx.cs" Inherits="yaf.controls.EditUserPoints" %>
Current Points: <asp:Literal ID="ltrCurrentPoints" runat="server" />
<br /><br />
Make user's points: <asp:TextBox runat="server" ID="txtUserPoints" ValidationGroup="UserPoints" />
<asp:RangeValidator ID="RangeValidator3" Runat="server" ErrorMessage="Number Please"
    ControlToValidate="txtUserPoints" SetFocusOnError="true" ValidationGroup="UserPoints" MaximumValue="1000000" MinimumValue="0" Type="Integer"
    Display="Dynamic">
</asp:RangeValidator>
<asp:Button runat="server" ID="btnUserPoints" OnClick="SetUserPoints_Click" Text="Go" />
<br /><br />
Add points 
<asp:TextBox runat="server" ID="txtAddPoints" ValidationGroup="Add" />
<asp:RangeValidator ID="RangeValidator1" Runat="server" ErrorMessage="Number Please"
    ControlToValidate="txtAddPoints" SetFocusOnError="true" ValidationGroup="Add" MaximumValue="1000000" MinimumValue="0" Type="Integer"
    Display="Dynamic">
</asp:RangeValidator>
<asp:Button runat="server" ID="btnAddPoints" OnClick="AddPoints_Click" Text="Go" />
<br /><br />
Remove points
<asp:TextBox runat="server" ID="txtRemovePoints" ValidationGroup="Remove" />
<asp:RangeValidator ID="RangeValidator2" Runat="server" ErrorMessage="Number Please"
    ControlToValidate="txtRemovePoints" SetFocusOnError="true" ValidationGroup="Remove" MaximumValue="1000000" MinimumValue="0" Type="Integer"
    Display="Dynamic">
</asp:RangeValidator>
<asp:Button runat="server" ID="Button1" OnClick="RemovePoints_Click" Text="Go" />

