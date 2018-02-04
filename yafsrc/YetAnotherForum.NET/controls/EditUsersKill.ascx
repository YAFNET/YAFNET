<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersKill" CodeBehind="EditUsersKill.ascx.cs" %>

    <h2 runat="server" id="trHeader">

            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_KILL_USER" LocalizedPage="ADMIN_EDITUSER" />
        </h2>
    <hr />

        <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="IP_ADDRESSES" LocalizedPage="ADMIN_EDITUSER" />
        </h4>
        <p>
            <asp:Literal ID="IpAddresses" runat="server"></asp:Literal>
        </p>
    <hr />

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="BAN_EMAIL_OFUSER" LocalizedPage="ADMIN_EDITUSER" />
            </strong>
        </h4>
        <p>
            <asp:CheckBox CssClass="form-control" ID="BanEmail" runat="server" Checked="true" />
        </p>
    <hr />

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="BAN_IP_OFUSER" LocalizedPage="ADMIN_EDITUSER" />
            </strong>
        </h4>
        <p>
            <asp:CheckBox CssClass="form-control" ID="BanIps" runat="server" Checked="true" />
        </p>
    <hr />

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="BAN_NAME_OFUSER" LocalizedPage="ADMIN_EDITUSER" />
            </strong>
        </h4>
        <p>
            <asp:CheckBox CssClass="form-control" ID="BanName" runat="server" Checked="true" />
        </p>
    <hr />

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="SUSPEND_OR_DELETE_ACCOUNT" LocalizedPage="ADMIN_EDITUSER" />
            </strong>
        </h4>
        <p>
            <asp:RadioButtonList runat="server" ID="SuspendOrDelete"  CssClass="form-control">
            </asp:RadioButtonList>
            <asp:Literal ID="SuspendedTo" runat="server"></asp:Literal>
        </p>
    <hr />

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE_POSTS_USER" LocalizedPage="ADMIN_EDITUSER" />
            </strong>
        </h4>
        <p>
            <strong>
                <asp:Literal ID="PostCount" runat="server"></asp:Literal></strong> (<asp:HyperLink ID="ViewPostsLink" runat="server" Target="_blank">
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="VIEW_ALL" LocalizedPage="ADMIN_EDITUSER" />
                </asp:HyperLink>)
        </p>

    <asp:PlaceHolder runat="server" ID="ReportUserRow">
         <hr />
        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="REPORT_USER" LocalizedPage="ADMIN_EDITUSER" />
            </strong>
        </h4>
        <p>
            <asp:CheckBox CssClass="form-control" ID="ReportUser" runat="server" />
        </p>
        </asp:PlaceHolder>

                <div class="text-lg-center">

            <YAF:ThemeButton runat="server" ID="Kill" Type="Primary" OnClick="Kill_OnClick"
                             Icon="ban" TextLocalizedPage="ADMIN_EDITUSER" TextLocalizedTag="HEAD_KILL_USER"
                             ReturnConfirmText='<%# this.GetText("ADMIN_EDITUSER", "KILL_USER_CONFIRM") %>'>
            </YAF:ThemeButton>
            </div>
