<%@ Control Language="c#" CodeFile="edituser.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.edituser" %>
<%@ Register TagPrefix="uc1" TagName="QuickEdit" Src="../../controls/EditUsersInfo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GroupsEdit" Src="../../controls/EditUsersGroups.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ProfileEdit" Src="../../controls/EditUsersProfile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendEdit" Src="../../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PointsEdit" Src="../../controls/EditUsersPoints.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AvatarEdit" Src="../../controls/EditUsersAvatar.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ResetPasswordEdit" Src="../../controls/EditUsersResetPass.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <ajaxToolkit:TabContainer runat="server" ID="PMTabs" CssClass="ajax__tab_yaf">
        <ajaxToolkit:TabPanel runat="server" ID="QuickEditTab" HeaderText="User Details">
            <ContentTemplate>
                <uc1:QuickEdit ID="QuickEditControl" runat="server" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="GroupEditTab" HeaderText="User Roles">
            <ContentTemplate>
                <uc1:GroupsEdit ID="GroupEditControl" runat="server" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="ProfileEditTab" HeaderText="User Profile">
            <ContentTemplate>
                <uc1:ProfileEdit ID="ProfileEditControl" runat="server" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="AvatarTab" HeaderText="User Avatar">
            <ContentTemplate>
                <uc1:AvatarEdit runat="server" ID="AvatarEditControl" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="SignatureTab" HeaderText="User Signature">
            <ContentTemplate>
                <uc1:SignatureEdit ID="SignatureEditControl" runat="server" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="ResetPasswordTab" HeaderText="Reset Password">
            <ContentTemplate>
                <uc1:ResetPasswordEdit runat="server" ID="ResetPasswordControl" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>        
        <ajaxToolkit:TabPanel runat="server" ID="PointsTab" HeaderText="User Points">
            <ContentTemplate>
                <uc1:PointsEdit runat="server" ID="UserPointsControl" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel runat="server" ID="SuspendTab" HeaderText="Suspend User">
            <ContentTemplate>
                <uc1:SuspendEdit runat="server" ID="SuspendUserControl" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
