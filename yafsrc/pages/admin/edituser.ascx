<%@ Control Language="c#" Codebehind="edituser.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.edituser" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="uc1" TagName="QuickEdit" Src="../../controls/EditUsersInfo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GroupsEdit" Src="../../controls/EditUsersGroups.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ProfileEdit" Src="../../controls/EditUsersProfile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendEdit" Src="../../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PointsEdit" Src="../../controls/EditUsersPoints.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AvatarEdit" Src="../../controls/EditUsersAvatar.ascx" %>
<yaf:PageLinks runat="server" ID="PageLinks" />
<yaf:AdminMenu runat="server">
    <table width="100%" cellspacing="1" cellpadding="0" class="content">
        <tbody>
            <tr>
                <td colspan="2" class="header1">
                    Edit User:
                    <asp:Label ID="TitleUserName" runat="server" /></td>
            </tr>
            <tr>
                <td valign="top" class="post" width="150">
                    <ul>
                        <li><asp:LinkButton runat="server" ID="BasicEditLink" OnClick="Edit1_Click" /></li>
                        <li><asp:LinkButton runat="server" ID="GroupLink" OnClick="Edit2_Click" /></li>
                        <li><asp:LinkButton runat="server" ID="ProfileLink" OnClick="Edit3_Click" /></li>
                        <li><asp:LinkButton runat="server" ID="AvatarLink" OnClick="Edit7_Click" /></li>
                        <li><asp:LinkButton runat="server" ID="SignatureLink" OnClick="Edit4_Click" /></li>
                        <li><asp:LinkButton runat="server" ID="SuspendLink" OnClick="Edit5_Click" /></li>
                        <li><asp:LinkButton runat="server" ID="PointsLink" OnClick="Edit6_Click" /></li>
                    </ul>
                </td>
                <td valign="top" class="post">
                    <asp:MultiView ID="UserAdminMultiView" runat="server" ActiveViewIndex="0">
                        <asp:View ID="QuickEditView" runat="server">
                            <uc1:QuickEdit ID="QuickEditControl" runat="server" />
                        </asp:View>
                        <asp:View ID="GroupsEditView" runat="server">
                            <uc1:GroupsEdit ID="GroupEditControl" runat="server" />
                        </asp:View>
                        <asp:View ID="ProfileEditView" runat="server">
                            <uc1:ProfileEdit ID="ProfileEditControl" runat="server" />
                        </asp:View>
                        <asp:View ID="AvatarEditView" runat="server">
                            <uc1:AvatarEdit runat="server" ID="AvatarEditControl" />
                        </asp:View>
                        <asp:View ID="SignatureEditView" runat="server">
                            <uc1:SignatureEdit ID="SignatureEditControl" runat="server" />
                        </asp:View>
                        <asp:View ID="SuspendUserView" runat="server">
                            <uc1:SuspendEdit runat="server" ID="SuspendUserControl" />
                        </asp:View>
                        <asp:View ID="UserPointsView" runat="server">
                            <uc1:PointsEdit runat="server" ID="UserPointsControl" />
                        </asp:View>
                    </asp:MultiView>
                </td>
            </tr>
        </tbody>
    </table>
</yaf:AdminMenu>
<yaf:SmartScroller ID="SmartScroller1" runat="server" />
