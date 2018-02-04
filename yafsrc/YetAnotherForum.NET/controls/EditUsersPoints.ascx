<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersPoints" Codebehind="EditUsersPoints.ascx.cs" %>


        <h2>
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="USER_REPUTATION" LocalizedPage="ADMIN_EDITUSER" />
        </h2>
    <hr />

	  <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="CURRENT_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </h4>
        <p>
            <asp:Literal ID="ltrCurrentPoints" runat="server" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SET_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="txtUserPoints" ValidationGroup="UserPoints" CssClass="form-control" TextMode="Number" />
            <asp:RangeValidator ID="RangeValidator4" runat="server" ErrorMessage="Please enter a number" ControlToValidate="txtUserPoints"
                SetFocusOnError="true" ValidationGroup="UserPoints" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:LinkButton runat="server" ID="btnUserPoints" Type="Primary" OnClick="SetUserPoints_Click" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ADD_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="txtAddPoints" ValidationGroup="Add" CssClass="form-control" TextMode="Number" />
            <asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="Number Please" ControlToValidate="txtAddPoints" SetFocusOnError="true"
                ValidationGroup="Add" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:LinkButton runat="server" ID="btnAddPoints" Type="Primary" OnClick="AddPoints_Click" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="REMOVE_POINTS" LocalizedPage="ADMIN_EDITUSER" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="txtRemovePoints" ValidationGroup="Removem" CssClass="form-control" TextMode="Number" />
            <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Number Please" ControlToValidate="txtRemovePoints" SetFocusOnError="true"
                ValidationGroup="Remove" MaximumValue="1000000" MinimumValue="0" Type="Integer" Display="Dynamic">
            </asp:RangeValidator>
            <asp:LinkButton runat="server" ID="Button1" Type="Primary" OnClick="RemovePoints_Click" />
        </p>
