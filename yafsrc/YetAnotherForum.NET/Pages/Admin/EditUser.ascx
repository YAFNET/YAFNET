<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.EditUser" Codebehind="EditUser.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="QuickEdit" Src="../../controls/EditUsersInfo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="GroupsEdit" Src="../../controls/EditUsersGroups.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ProfileEdit" Src="../../controls/EditUsersProfile.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ProfileSettings" Src="../../controls/EditUsersSettings.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Attachments" Src="../../controls/EditUsersAttachments.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SignatureEdit" Src="../../controls/EditUsersSignature.ascx" %>
<%@ Register TagPrefix="uc1" TagName="SuspendEdit" Src="../../controls/EditUsersSuspend.ascx" %>
<%@ Register TagPrefix="uc1" TagName="PointsEdit" Src="../../controls/EditUsersPoints.ascx" %>
<%@ Register TagPrefix="uc1" TagName="AvatarEdit" Src="../../controls/EditUsersAvatar.ascx" %>
<%@ Register TagPrefix="uc1" TagName="KillEdit" Src="../../controls/EditUsersKill.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server" ID="IconHeader"
                                    IconName="user-edit"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <asp:Panel id="EditUserTabs" runat="server">
                        <ul class="nav nav-tabs" role="tablist">
                            <li class="nav-item">
                                <a class="nav-link" href="#View1" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="USER_DETAILS" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View2" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER_ROLES" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View3" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USER_PROFILE" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View10" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="USER_SETTINGS" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View4" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="USER_AVATAR" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View11" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" LocalizedPage="ATTACHMENTS" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View5" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="USER_SIG" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View6" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="USER_REPUTATION" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <asp:PlaceHolder runat="server" Visible="<%#!this.EditBoardUser.Item1.UserFlags.IsGuest%>">
                                <li class="nav-item">
                                <a class="nav-link" href="#View8" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="USER_SUSPEND" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            <li class="nav-item">
                                <a class="nav-link" href="#View9" data-bs-toggle="tab" role="tab">
                                    <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="USER_KILL" LocalizedPage="ADMIN_EDITUSER" />
                                </a>
                            </li>
                            </asp:PlaceHolder>
                        </ul>
                        <div class="tab-content">
                            <div class="tab-pane" id="View1" role="tabpanel">
                                <uc1:QuickEdit ID="QuickEditControl" User="<%# this.EditBoardUser %>" runat="server" />
                            </div>
                            <div class="tab-pane" id="View2" role="tabpanel">
                                <uc1:GroupsEdit ID="GroupEditControl" runat="server" Visible="<%# !YAF.Configuration.Config.IsDotNetNuke %>" User="<%# this.EditBoardUser %>" />
                            </div>
                            <div class="tab-pane" id="View3" role="tabpanel">
                                <uc1:ProfileEdit ID="ProfileEditControl" runat="server" User="<%# this.EditBoardUser %>" />
                            </div>
                            <div class="tab-pane" id="View10" role="tabpanel">
                                <uc1:ProfileSettings ID="ProfileSettings" runat="server" User="<%# this.EditBoardUser.Item1 %>" />
                            </div>
                            <div class="tab-pane" id="View11" role="tabpanel">
                                <uc1:Attachments ID="UserAttachments" runat="server" />
                            </div>
                            <div class="tab-pane" id="View4" role="tabpanel">
                                <uc1:AvatarEdit runat="server" ID="AvatarEditControl" />
                            </div>
                            <div class="tab-pane" id="View5" role="tabpanel">
                                <uc1:SignatureEdit ID="SignatureEditControl" runat="server" />
                            </div>
                            <div class="tab-pane" id="View6" role="tabpanel">
                                <uc1:PointsEdit runat="server" ID="UserPointsControl" User="<%# this.EditBoardUser.Item1 %>" />
                            </div>
                            <asp:PlaceHolder runat="server" ID="FielsNoGuests" Visible="<%#!this.EditBoardUser.Item1.UserFlags.IsGuest%>">
                                <div class="tab-pane" id="View8" role="tabpanel">
                                    <uc1:SuspendEdit runat="server" ID="SuspendUserControl" />
                                </div>
                                <div class="tab-pane" id="View9" role="tabpanel">
                                    <uc1:KillEdit runat="server" ID="KillEdit1" User="<%# this.EditBoardUser %>" />
                                </div>
                            </asp:PlaceHolder>
                        </div>

                    </asp:Panel>
                    <asp:HiddenField runat="server" ID="hidLastTab" Value="View1" />

                </div>
               </div>
            </div>
        </div>


