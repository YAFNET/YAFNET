<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.Settings" CodeBehind="Settings.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1>
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="HEADER" LocalizedPage="ADMIN_BOARDSETTINGS" />
        </h1>
    </div>
</div>
<div class="row">
    <div class="col-xl-12">
         <div class="card mb-3">
             <div class="card-header">
                 <YAF:IconHeader runat="server"
                                 IconName="cogs"
                                 LocalizedTag="BOARD_SETUP"
                                 LocalizedPage="ADMIN_BOARDSETTINGS"></YAF:IconHeader>
        </div>
        <div class="card-body">
            <div class="row">
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                   AssociatedControlID="Name"
                                   LocalizedTag="BOARD_NAME" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:TextBox ID="Name" runat="server" required="required" 
                                 CssClass="form-control" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server" 
                                            LocalizedTag="MSG_NAME_BOARD" />
                    </div>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                   AssociatedControlID="ForumEmail"
                                   LocalizedTag="FORUM_EMAIL" LocalizedPage="ADMIN_HOSTSETTINGS"/>
                    <asp:TextBox ID="ForumEmail" runat="server" 
                                 TextMode="Email" 
                                 CssClass="form-control" />
                </div>
            </div>
            <div class="mb-3">
                <YAF:HelpLabel ID="HelpLabel14" runat="server"
                               AssociatedControlID="BoardLogo"
                               LocalizedTag="BOARD_LOGO" LocalizedPage="ADMIN_BOARDSETTINGS" />
                <YAF:ImageListBox ID="BoardLogo" runat="server" CssClass="select2-image-select" />
            </div>
        <div class="mb-3">
                <YAF:HelpLabel ID="HelpLabel5" runat="server"
                               AssociatedControlID="ForumBaseUrlMask"
                               LocalizedTag="FORUM_BASEURLMASK" LocalizedPage="ADMIN_HOSTSETTINGS"/>

                <asp:TextBox ID="ForumBaseUrlMask" runat="server" TextMode="Url" CssClass="form-control"></asp:TextBox>
            </div>
            <asp:PlaceHolder ID="CopyrightHolder" runat="server">
                <div class="mb-3">
                    <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                   AssociatedControlID="CopyrightHolder"
                                   LocalizedTag="COPYRIGHT_REMOVAL_KEY" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="input-group">
                        <asp:TextBox ID="CopyrightRemovalKey" runat="server" 
                                     CssClass="form-control"></asp:TextBox>
                        <YAF:ThemeButton runat="server" ID="GetRemovalKey"
                                             NavigateUrl="https://yetanotherforum.net/purchase.aspx"
                                             Type="Info"
                                             Icon="key"
                                             TextLocalizedTag="COPYRIGHT_REMOVAL_KEY_DOWN">
                        </YAF:ThemeButton>
                    </div>
                </div>

            </asp:PlaceHolder>
            <div class="mb-3">
                <YAF:HelpLabel ID="LocalizedLabel5" runat="server"
                               AssociatedControlID="Theme"
                               LocalizedTag="BOARD_THEME" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:DropDownList ID="Theme" runat="server" CssClass="select2-select"></asp:DropDownList>
            </div>
            <div class="row">
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel11" runat="server"
                                   AssociatedControlID="ShowTopic"
                                   LocalizedTag="BOARD_TOPIC_DEFAULT"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:DropDownList ID="ShowTopic" runat="server" CssClass="select2-select">
                    </asp:DropDownList>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel10" runat="server"
                                   AssociatedControlID="Culture"
                                   LocalizedTag="BOARD_CULTURE" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:DropDownList ID="Culture" runat="server" CssClass="select2-select">
                    </asp:DropDownList>
                </div>
            </div>
            <div class="mb-3">
                <YAF:HelpLabel ID="LocalizedLabel17" runat="server"
                               AssociatedControlID="DefaultNotificationSetting"
                               LocalizedTag="BOARD_DEFAULT_NOTIFICATION"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:DropDownList ID="DefaultNotificationSetting" runat="server" 
                                  CssClass="select2-select" 
                                  placeholder='<%# this.GetText("BOARD_DEFAULT_NOTIFICATION") %>'>
                </asp:DropDownList>
            </div>
            <div class="mb-3">
                <YAF:HelpLabel ID="LocalizedLabel13" runat="server"
                               AssociatedControlID="NotificationOnUserRegisterEmailList"
                               LocalizedTag="BOARD_EMAIL_ONREGISTER"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="row">
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel14" runat="server"
                                   AssociatedControlID="EmailModeratorsOnModeratedPost"
                                   LocalizedTag="BOARD_EMAIL_MODS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="EmailModeratorsOnModeratedPost" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                   AssociatedControlID="EmailModeratorsOnReportedPost"
                                   LocalizedTag="BOARD_EMAIL_REPORTMODS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="EmailModeratorsOnReportedPost" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel15" runat="server"
                                   AssociatedControlID="AllowDigestEmail"
                                   LocalizedTag="BOARD_ALLOW_DIGEST"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="AllowDigestEmail" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel16" runat="server"
                                   AssociatedControlID="DefaultSendDigestEmail"
                                   LocalizedTag="BOARD_DIGEST_NEWUSERS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="form-check form-switch">
                        <asp:CheckBox ID="DefaultSendDigestEmail" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="mb-3 col-md-4">
                    <YAF:HelpLabel ID="HelpLabelDigest1" runat="server"
                                   AssociatedControlID="DigestSendEveryXHours"
                                   LocalizedTag="BOARD_DIGEST_HOURS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:TextBox ID="DigestSendEveryXHours" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="mb-3">
                <YAF:HelpLabel ID="HelpLabel6" runat="server"
                               AssociatedControlID="CdvVersion"
                               LocalizedTag="CdvVersion"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <div class="input-group">
                    <asp:TextBox ID="CdvVersion" runat="server" 
                                 CssClass="form-control" 
                                 Enabled="False"></asp:TextBox>
                    <YAF:ThemeButton runat="server"
                                     OnClick="IncreaseVersionOnClick"
                                     CssClass="float-right"
                                     Type="Info"
                                     Icon="file-code"
                                     TextLocalizedTag="CDVVERSION_BUTTON"
                                     TitleLocalizedTag="CDVVERSION_HELP">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
        <div class="card-footer text-center">
            <YAF:ThemeButton ID="Save" Type="Primary" runat="server" OnClick="SaveClick"
                             Icon="save" TextLocalizedTag="SAVE">
            </YAF:ThemeButton>
        </div>
    </div>
    </div>
</div>