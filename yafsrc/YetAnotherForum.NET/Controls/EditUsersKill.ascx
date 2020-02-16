<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersKill" CodeBehind="EditUsersKill.ascx.cs" %>

<h2 runat="server" id="trHeader">
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_KILL_USER" LocalizedPage="ADMIN_EDITUSER" />
</h2>
<div class="form-group">
    <asp:Label runat="server">
        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="IP_ADDRESSES" LocalizedPage="ADMIN_EDITUSER" />
    </asp:Label>
    <asp:Literal ID="IpAddresses" runat="server"></asp:Literal>
</div>
<div class="form-row">
    <div class="form-group col-md-4">
        <asp:Label runat="server" AssociatedControlID="BanEmail">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="BAN_EMAIL_OFUSER" LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <div class="custom-control custom-switch">
            <asp:CheckBox Text="&nbsp;" ID="BanEmail" runat="server" Checked="true" />
        </div>
    </div>
    <div class="form-group col-md-4">
        <asp:Label runat="server" AssociatedControlID="BanIps">
            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="BAN_IP_OFUSER" LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <div class="custom-control custom-switch">
            <asp:CheckBox Text="&nbsp;" ID="BanIps" runat="server" Checked="true" />
        </div>
    </div>
    <div class="form-group col-md-4">
        <asp:Label runat="server" AssociatedControlID="BanName">
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="BAN_NAME_OFUSER" LocalizedPage="ADMIN_EDITUSER" />
        </asp:Label>
        <div class="custom-control custom-switch">
            <asp:CheckBox Text="&nbsp;" ID="BanName" runat="server" Checked="true" />
        </div>
    </div>
</div>
<div class="form-group">
    <asp:Label runat="server" AssociatedControlID="SuspendOrDelete">
        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="SUSPEND_OR_DELETE_ACCOUNT" LocalizedPage="ADMIN_EDITUSER" />
    </asp:Label>
    <div class="custom-control custom-radio custom-control-inline">
        <asp:RadioButtonList runat="server" ID="SuspendOrDelete" 
                             RepeatLayout="UnorderedList"
                             CssClass="list-unstyled">
        </asp:RadioButtonList>
    </div>
    </div>
            <asp:Literal ID="SuspendedTo" runat="server"></asp:Literal>
    <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="ViewPostsLink">
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE_POSTS_USER" LocalizedPage="ADMIN_EDITUSER" />
            </asp:Label>
            <strong>
                <asp:Literal ID="PostCount" runat="server"></asp:Literal></strong> (<asp:HyperLink ID="ViewPostsLink" runat="server" Target="_blank">
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="VIEW_ALL" LocalizedPage="ADMIN_EDITUSER" />
                </asp:HyperLink>)
        </div>
    <asp:PlaceHolder runat="server" ID="ReportUserRow">
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="ReportUser">
                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="REPORT_USER" LocalizedPage="ADMIN_EDITUSER" />
            </asp:Label>
            <div class="custom-control custom-switch">
                <asp:CheckBox Text="&nbsp;" ID="ReportUser" runat="server" />
            </div>
        </div>
        </asp:PlaceHolder>
<div class="text-lg-center mt-1">
    <YAF:ThemeButton runat="server" ID="Kill" 
                     Type="Primary" 
                     OnClick="Kill_OnClick"
                     Icon="ban" 
                     TextLocalizedPage="ADMIN_EDITUSER" TextLocalizedTag="HEAD_KILL_USER"
                     ReturnConfirmText='<%# this.GetText("ADMIN_EDITUSER", "KILL_USER_CONFIRM") %>'>
    </YAF:ThemeButton>
</div>