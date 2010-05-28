<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.edituser" Codebehind="edituser.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="QuickEdit" Src="../../controls/EditUsersInfo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GroupsEdit" Src="../../controls/EditUsersGroups.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ProfileEdit" Src="../../controls/EditUsersProfile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendEdit" Src="../../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PointsEdit" Src="../../controls/EditUsersPoints.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AvatarEdit" Src="../../controls/EditUsersAvatar.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ResetPasswordEdit" Src="../../controls/EditUsersResetPass.ascx" %>
<%@ Register TagPrefix="uc1" TagName="KillEdit" Src="../../controls/EditUsersKill.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<DotNetAge:Tabs ID="EditUserTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false"
		AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false"
		EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd=""
		OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove=""
		OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false" Spinner="">
		<Animations>
		</Animations>
		<Views>
			<DotNetAge:View runat="server" ID="View1" Text="User Details" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:QuickEdit ID="QuickEditControl" runat="server" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View2" Text="User Roles" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:GroupsEdit ID="GroupEditControl" runat="server" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View3" Text="User Profile" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:ProfileEdit ID="ProfileEditControl" runat="server" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View4" Text="User Avatar" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:AvatarEdit runat="server" ID="AvatarEditControl" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View5" Text="User Signature" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:SignatureEdit ID="SignatureEditControl" runat="server" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View6" Text="User Password" Visible="<%#!IsGuestUser%>" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:ResetPasswordEdit runat="server" ID="ResetPasswordControl" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View7" Text="User Points" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:PointsEdit runat="server" ID="UserPointsControl" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View8" Text="User Suspend" Visible="<%#!IsGuestUser%>" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:SuspendEdit runat="server" ID="SuspendUserControl" />
			</DotNetAge:View>
			<DotNetAge:View runat="server" ID="View9" Text="User Kill Activity" Visible="<%#!IsGuestUser%>" NavigateUrl="" HeaderCssClass=""
				HeaderStyle="" Target="_blank">
				<uc1:KillEdit runat="server" ID="KillEdit1" />
			</DotNetAge:View>			
		</Views>
	</DotNetAge:Tabs>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
