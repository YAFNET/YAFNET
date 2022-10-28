<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersInfo"
    CodeBehind="EditUsersInfo.ascx.cs" %>

<h2>
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_USER_DETAILS" LocalizedPage="ADMIN_EDITUSER" />
</h2>
<hr />

<div class="row">
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                       AssociatedControlID="Name"
                       LocalizedTag="USERINFO_NAME" LocalizedPage="ADMIN_EDITUSER" />
        <asp:TextBox CssClass="form-control" ID="Name" runat="server" 
                     Enabled="false" />
    </div>
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                       AssociatedControlID="DisplayName"
                       LocalizedTag="USERINFO_DISPLAYNAME" LocalizedPage="ADMIN_EDITUSER" />
        <asp:TextBox CssClass="form-control" ID="DisplayName" runat="server" />
    </div>
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel3" runat="server" 
                       AssociatedControlID="Email"
                       LocalizedTag="EMAIL" LocalizedPage="PROFILE" />
        <asp:TextBox ID="Email" runat="server" 
                     TextMode="Email"
                     CssClass="form-control" />
    </div>
</div>
<div class="mb-3">
    <YAF:HelpLabel ID="HelpLabel4" runat="server" 
                   AssociatedControlID="RankID"
                   LocalizedTag="RANK" LocalizedPage="ADMIN_USERS" />
    <asp:DropDownList ID="RankID" runat="server" 
                      CssClass="form-select" />
</div>

<asp:PlaceHolder runat="server" id="IsHostAdminRow">
    <div class="row">
    <div class="mb-3 col-md-6">
        <YAF:HelpLabel ID="HelpLabel15" runat="server" 
                       AssociatedControlID="Moderated"
                       LocalizedTag="MODERATE" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="Moderated" />
        </div>
    </div>
    <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel5" runat="server" 
                           AssociatedControlID="IsHostAdminX"
                           LocalizedTag="USERINFO_HOST" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsHostAdminX" />
        </div>
    </div>
    </div>
    </asp:PlaceHolder>
    <div class="row">
    <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel6" runat="server" 
                           AssociatedControlID="IsCaptchaExcluded"
                           LocalizedTag="USERINFO_EX_CAPTCHA" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsCaptchaExcluded" />
        </div>
    </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel7" runat="server" 
                           AssociatedControlID="IsExcludedFromActiveUsers"
                           LocalizedTag="USERINFO_EX_ACTIVE" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsExcludedFromActiveUsers" />
        </div>
    </div>
    </div>
<div class="mb-3">
            <YAF:HelpLabel ID="HelpLabel8" runat="server" 
                           AssociatedControlID="IsApproved"
                           LocalizedTag="USERINFO_APPROVED" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsApproved" Enabled="False" />
        </div>
    </div>
<div class="row">
    <asp:PlaceHolder runat="server" ID="ApproveHolder" Visible="False">
        <div class="mb-3 col-md-6">
            <YAF:ThemeButton runat="server" ID="ApproveUser" 
                             Type="Secondary"
                             Icon="check"
                             OnClick="ApproveUserClick"
                             TextLocalizedTag="ADMIN_APPROVE"></YAF:ThemeButton>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="DisabledHolder" runat="server" Visible="False">
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel16" runat="server" 
                           AssociatedControlID="UnDisableUser"
                           LocalizedTag="DISABLED" LocalizedPage="ADMIN_EDITUSER"/>
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" runat="server" ID="UnDisableUser" Enabled="False" Checked="True" />
            </div>
        </div>
    </asp:PlaceHolder>
</div>
<!-- Easy to enable it if there is major issues (i.e. Guest being deleted). -->
    <asp:PlaceHolder runat="server" id="IsGuestRow" visible="false">
        <div class="mb-3">
            <YAF:HelpLabel ID="HelpLabel9" runat="server" 
                           AssociatedControlID="IsGuestX"
                           LocalizedTag="USERINFO_GUEST" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsGuestX" />
        </div>
    </div>
    </asp:PlaceHolder>
<div class="row">
    <div class="mb-3 col-md-6">
        <YAF:HelpLabel ID="HelpLabel10" runat="server" 
                       AssociatedControlID="Joined"
                       LocalizedTag="JOINED" LocalizedPage="PROFILE" />
        <asp:TextBox CssClass="form-control" ID="Joined" runat="server" Enabled="False" />
    </div>
    <div class="mb-3 col-md-6">
        <YAF:HelpLabel ID="HelpLabel11" runat="server" 
                       AssociatedControlID="LastVisit"
                       LocalizedTag="LASTVISIT" LocalizedPage="PROFILE" />
        <asp:TextBox CssClass="form-control" ID="LastVisit" runat="server" Enabled="False" />
    </div>
</div>
<div class="row">
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel12" runat="server" 
                       AssociatedControlID="IsFacebookUser"
                       LocalizedTag="FACEBOOK_USER" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsFacebookUser" Enabled="false" />
        </div>
    </div>
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel13" runat="server" 
                       AssociatedControlID="IsTwitterUser"
                       LocalizedTag="TWITTER_USER" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsTwitterUser" Enabled="false" />
        </div>
    </div>
    <div class="mb-3 col-md-4">
        <YAF:HelpLabel ID="HelpLabel14" runat="server" 
                       AssociatedControlID="IsGoogleUser"
                       LocalizedTag="Google_USER" LocalizedPage="ADMIN_EDITUSER" />
         
        <div class="form-check form-switch">
            <asp:CheckBox Text="&nbsp;" runat="server" ID="IsGoogleUser" Enabled="false" />
        </div>
    </div>
</div>
<div class="text-lg-center">
    <YAF:ThemeButton ID="Save" runat="server" 
                     Type="Primary"
                     Icon="save" 
                     TextLocalizedTag="SAVE"
                     OnClick="Save_Click" />
</div>