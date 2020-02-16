<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersPoints" Codebehind="EditUsersPoints.ascx.cs" %>

<h2>
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                        LocalizedTag="USER_REPUTATION" 
                        LocalizedPage="ADMIN_EDITUSER" />
</h2>
<h6>
    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                        LocalizedTag="CURRENT_POINTS" 
                        LocalizedPage="ADMIN_EDITUSER" />
    <span class="badge badge-secondary"><asp:Literal ID="ltrCurrentPoints" runat="server" /></span>
</h6>

<div class="form-row">
    <div class="form-group col-md-4">
        <asp:Label runat="server" AssociatedControlID="txtUserPoints">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                LocalizedTag="SET_POINTS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <asp:TextBox runat="server" ID="txtUserPoints" 
                     ValidationGroup="UserPoints" 
                     CssClass="form-control" 
                     TextMode="Number" />
        <asp:RangeValidator ID="RangeValidator4" runat="server" 
                            ErrorMessage="Please enter a number" 
                            ControlToValidate="txtUserPoints"
                            SetFocusOnError="true" 
                            ValidationGroup="UserPoints" 
                            MaximumValue="1000000" 
                            MinimumValue="0" 
                            Type="Integer" 
                            Display="Dynamic">
        </asp:RangeValidator>
        <YAF:ThemeButton runat="server" ID="btnUserPoints" 
                         TextLocalizedTag="GO"
                         Icon="check"
                         Type="Secondary" 
                         OnClick="SetUserPoints_Click"
                         CssClass="mt-1"/>
    </div>
    <div class="form-group col-md-4">
        <asp:Label runat="server" AssociatedControlID="txtAddPoints">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                LocalizedTag="ADD_POINTS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <asp:TextBox runat="server" ID="txtAddPoints" 
                     ValidationGroup="Add" 
                     CssClass="form-control" 
                     TextMode="Number" Text="0" />
        <asp:RangeValidator ID="RangeValidator1" runat="server" 
                            ErrorMessage="Number Please" 
                            ControlToValidate="txtAddPoints" 
                            SetFocusOnError="true"
                            ValidationGroup="Add" 
                            MaximumValue="1000000" 
                            MinimumValue="0" 
                            Type="Integer" 
                            Display="Dynamic">
        </asp:RangeValidator>
        <YAF:ThemeButton runat="server" ID="btnAddPoints"
                         TextLocalizedTag="GO"
                         Icon="check"
                         Type="Secondary" 
                         OnClick="AddPoints_Click"
                         CssClass="mt-1" />
    </div>
    <div class="form-group col-md-4">
        <asp:Label runat="server" AssociatedControlID="txtRemovePoints">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                LocalizedTag="REMOVE_POINTS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <asp:TextBox runat="server" ID="txtRemovePoints" 
                     ValidationGroup="Removem" 
                     CssClass="form-control" 
                     TextMode="Number" 
                     Text="0" />
        <asp:RangeValidator ID="RangeValidator2" runat="server" 
                            ErrorMessage="Number Please" 
                            ControlToValidate="txtRemovePoints" 
                            SetFocusOnError="true"
                            ValidationGroup="Remove" 
                            MaximumValue="1000000" 
                            MinimumValue="0" 
                            Type="Integer" 
                            Display="Dynamic">
        </asp:RangeValidator>
        <YAF:ThemeButton runat="server" ID="Button1" 
                         Type="Secondary" 
                         TextLocalizedTag="GO"
                         Icon="check"
                         OnClick="RemovePoints_Click"
                         CssClass="mt-1" />
    </div>
</div>