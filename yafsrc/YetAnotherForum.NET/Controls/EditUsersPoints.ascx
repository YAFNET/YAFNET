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
    <span class="badge text-bg-secondary"><asp:Literal ID="ltrCurrentPoints" runat="server" /></span>
</h6>

<div class="row">
    <div class="mb-3 col-md-4">
        <asp:Label runat="server" AssociatedControlID="txtUserPoints" CssClass="form-label">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                LocalizedTag="SET_POINTS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <asp:TextBox runat="server" ID="txtUserPoints" 
                     CssClass="form-control" 
                     TextMode="Number" />
        <YAF:ThemeButton runat="server" ID="btnUserPoints" 
                         TextLocalizedTag="GO"
                         Icon="check"
                         Type="Secondary" 
                         OnClick="SetUserPoints_Click"
                         CssClass="mt-1"/>
    </div>
    <div class="mb-3 col-md-4">
        <asp:Label runat="server" AssociatedControlID="txtAddPoints" CssClass="form-label">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                LocalizedTag="ADD_POINTS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <asp:TextBox runat="server" ID="txtAddPoints" 
                     CssClass="form-control" 
                     TextMode="Number" Text="0" />
        <YAF:ThemeButton runat="server" ID="btnAddPoints"
                         TextLocalizedTag="GO"
                         Icon="check"
                         Type="Secondary" 
                         OnClick="AddPoints_Click"
                         CssClass="mt-1" />
    </div>
    <div class="mb-3 col-md-4">
        <asp:Label runat="server" AssociatedControlID="txtRemovePoints" CssClass="form-label">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                LocalizedTag="REMOVE_POINTS" 
                                LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <asp:TextBox runat="server" ID="txtRemovePoints" 
                     CssClass="form-control" 
                     TextMode="Number" 
                     Text="0" />
        <YAF:ThemeButton runat="server" ID="Button1" 
                         Type="Secondary" 
                         TextLocalizedTag="GO"
                         Icon="check"
                         OnClick="RemovePoints_Click"
                         CssClass="mt-1" />
    </div>
</div>