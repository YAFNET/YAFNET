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
    <asp:Panel id="EditUserTabs" runat="server">
               <ul>
                 <li><a href="#View1"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="USER_DETAILS" LocalizedPage="ADMIN_EDITUSER" /></a></li>
                 <li><a href="#View2"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER_ROLES" LocalizedPage="ADMIN_EDITUSER" /></a></li>
                 <li><a href="#View3"><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USER_PROFILE" LocalizedPage="ADMIN_EDITUSER" /></a></li>
                 <li><a href="#View4"><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="USER_AVATAR" LocalizedPage="ADMIN_EDITUSER" /></a></li>
                 <li><a href="#View5"><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="USER_SIG" LocalizedPage="ADMIN_EDITUSER" /></a></li>		        
                 <li runat="server" id="View6Li" Visible="<%#!IsGuestUser%>"><a href='#<%# this.View6.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="USER_PASS" LocalizedPage="ADMIN_EDITUSER" /></a></li>	
                 <li><a href="#View7"><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="USER_REPUTATION" LocalizedPage="ADMIN_EDITUSER" /></a></li>
                 <li runat="server" id="View8Li" Visible="<%#!IsGuestUser%>"><a href='#<%# this.View8.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="USER_SUSPEND" LocalizedPage="ADMIN_EDITUSER" /></a></li>
                 <li runat="server" id="View9Li" Visible="<%#!IsGuestUser%>"><a href='#<%# this.View9.ClientID %>'><YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="USER_KILL" LocalizedPage="ADMIN_EDITUSER" /></a></li>
               </ul>
                <div id="View1">
                   <uc1:QuickEdit ID="QuickEditControl" runat="server" />
                </div>
                <div id="View2">
                  <uc1:GroupsEdit ID="GroupEditControl" runat="server" />
                </div>
                <div id="View3">
                  <uc1:ProfileEdit ID="ProfileEditControl" runat="server" />
                </div>
                <div id="View4">
                   <uc1:AvatarEdit runat="server" ID="AvatarEditControl" />
                </div>
                <div id="View5">
                  <uc1:SignatureEdit ID="SignatureEditControl" runat="server" />
                </div>
                <div id="View6" runat="server" Visible="<%#!IsGuestUser%>">
                  <uc1:ResetPasswordEdit runat="server" ID="ResetPasswordControl" />
                </div>
                <div id="View7">
                  <uc1:PointsEdit runat="server" ID="UserPointsControl" />
                </div>
                <div id="View8" runat="server" Visible="<%#!IsGuestUser%>">
                  <uc1:SuspendEdit runat="server" ID="SuspendUserControl" />
                </div>
                <div id="View9" runat="server" Visible="<%#!IsGuestUser%>">
                   <uc1:KillEdit runat="server" ID="KillEdit1" />
                </div>
             </asp:Panel>
    <asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
