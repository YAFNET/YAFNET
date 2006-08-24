<%@ Control language="c#" Codebehind="edituser.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.edituser" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="uc1" TagName="QuickEdit" Src="../../controls/EditUsersInfo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GroupsEdit" Src="../../controls/EditUsersGroups.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ProfileEdit" Src="../../controls/EditUsersProfile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../../controls/EditUsersSignature.ascx" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table width="100%" cellspacing="1" cellpadding="0" class="content" >
  <tbody>
	<tr>
		<td colspan="2" class="header1">Edit User: <asp:label id="TitleUserName" runat="server"/></td>
	</tr>
	<tr>
		<td valign="top" class="post" width="150">
		    <asp:LinkButton runat="server" id="BasicEditLink" onclick="Edit1_Click" /> <br />
		    <asp:LinkButton runat="server" id="GroupLink" onclick="Edit2_Click" /> <br />
		    <asp:LinkButton runat="server" id="ProfileLink" onclick="Edit3_Click" /> <br />
		    <asp:LinkButton runat="server" id="SignatureLink" onclick="Edit4_Click" /> <br />
    </td>
    <td valign="top" class="post">
        <asp:MultiView ID="UserAdminMultiView" runat="server" ActiveViewIndex="0">
            <asp:View ID="QuickEditView" runat="server">
               <uc1:QuickEdit id="QuickEditControl" runat="server"></uc1:QuickEdit>
            </asp:View>
            <asp:View ID="GroupsEditView" runat="server">
                <uc1:GroupsEdit id="GroupEditControl" runat="server"></uc1:GroupsEdit>
            </asp:View>
            <asp:View ID="ProfileEditView" runat="server">
                <uc1:ProfileEdit id="ProfileEditControl" runat="server"></uc1:ProfileEdit>
            </asp:View>
            <asp:View ID="SignatureEditView" runat="server">
                <uc1:SignatureEdit id="SignatureEditControl" runat="server"></uc1:SignatureEdit>
            </asp:View>
        </asp:MultiView>
     </td>
  </tr>
  </tbody>
</table>
</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
