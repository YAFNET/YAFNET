<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.HostSettings"
    CodeBehind="HostSettings.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col flex-grow-1 ms-md-3 d-md-none">
        <div class="nav d-grid gap-2" id="v-pills-tab-dropdown" role="tablist" aria-orientation="vertical">
             <div class="dropdown d-grid gap-2">
            <YAF:ThemeButton runat="server"
                             CssClass="dropdown-toggle"
                             DataToggle="dropdown"
                             Type="Secondary"
                             TextLocalizedTag="TITLE"
                             TextLocalizedPage="ADMIN_HOSTSETTINGS"></YAF:ThemeButton>
            <div class="dropdown-menu scrollable-dropdown w-100" aria-labelledby="dropdownMenuButton">
               <a href="#View0" class="nav-link" data-bs-toggle="pill" role="tab">
                            <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HEADER_SERVER_INFO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </a>
                <a href="#View1" class="nav-link" data-bs-toggle="pill" role="tab">
                    <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server"
                                        LocalizedTag="HEADER_SETUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View2" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server"
                                LocalizedTag="HOST_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View3" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server"
                                LocalizedTag="HOST_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View4" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server"
                                LocalizedTag="HOST_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
                <a href="#View5" class="nav-link" data-bs-toggle="pill" role="tab">
                    <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server"
                                        LocalizedTag="HOST_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                </a>
                <a href="#View6" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server"
                                LocalizedTag="HOST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
                <a href="#View8" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server"
                                LocalizedTag="HOST_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View9" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server"
                                LocalizedTag="HOST_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View10" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server"
                                LocalizedTag="HOST_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View11" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server"
                                LocalizedTag="HOST_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
            </div>
        </div>
        </div>
       
    </div>
    <div class="col-md-3 d-none d-md-block mt-3">
        <div class="nav flex-column nav-pills" id="v-pills-tab" role="tablist" aria-orientation="vertical">
            <a href="#View0" class="nav-link" data-bs-toggle="pill" role="tab">
                            <YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedTag="HEADER_SERVER_INFO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </a>
                        <a href="#View1" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel36" runat="server"
                                LocalizedTag="HEADER_SETUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View2" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel45" runat="server"
                                LocalizedTag="HOST_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View3" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel46" runat="server"
                                LocalizedTag="HOST_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View4" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel47" runat="server"
                                LocalizedTag="HOST_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
            <a href="#View5" class="nav-link" data-bs-toggle="pill" role="tab">
                <YAF:LocalizedLabel ID="LocalizedLabel48" runat="server"
                                    LocalizedTag="HOST_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
            </a>
            <a href="#View6" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel49" runat="server"
                                LocalizedTag="HOST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
            <a href="#View8" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel51" runat="server"
                                LocalizedTag="HOST_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View9" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel52" runat="server"
                                LocalizedTag="HOST_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View10" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel53" runat="server"
                                LocalizedTag="HOST_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        <a href="#View11" class="nav-link" data-bs-toggle="pill" role="tab">
            <YAF:LocalizedLabel ID="LocalizedLabel54" runat="server"
                                LocalizedTag="HOST_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
        </a>
        </div>
    </div>
    <div class="col-md-9">
    <asp:Panel runat="server" ID="HostSettingsTabs" CssClass="tab-content">

            <div class="tab-pane fade" id="View0" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel56" runat="server" LocalizedTag="HEADER_SERVER_INFO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                           AssociatedControlID="SQLVersion"
                                           LocalizedTag="SERVER_VERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="SQLVersion" runat="server"
                                         CssClass="form-control"
                                         TextMode="MultiLine"
                                         Height="100"
                                         Enabled="False"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel231" runat="server"
                                           AssociatedControlID="AppOsName"
                                           LocalizedTag="APP_OS_NAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="AppOSName" runat="server"
                                         CssClass="form-control"
                                         Enabled="False"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel232" runat="server"
                                           AssociatedControlID="AppRuntime"
                                           LocalizedTag="APP_RUNTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="AppRuntime" runat="server"
                                         CssClass="form-control"
                                         Enabled="False"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel233" runat="server"
                                           AssociatedControlID="AppCores"
                                           LocalizedTag="APP_CORES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="AppCores" runat="server"
                                         CssClass="form-control"
                                         Enabled="False"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel234" runat="server"
                                           AssociatedControlID="AppMemory"
                                           LocalizedTag="APP_MEMORY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="AppMemory" runat="server"
                                         CssClass="form-control"
                                         Enabled="False"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="tab-pane fade" id="View1" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel58" runat="server"
                                              LocalizedTag="HEADER_SETUP"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                    <ul class="nav nav-tabs" id="tabSetup" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="setup-tab" data-bs-toggle="tab" href="#setup" role="tab" aria-controls="setup" aria-selected="true">
                                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                                    LocalizedTag="HEADER_SETUP"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="password-tab" data-bs-toggle="tab" href="#password" role="tab" aria-controls="password" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server"
                                                    LocalizedTag="HEADER_PASSWORD"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="spam-tab" data-bs-toggle="tab" href="#spam" role="tab" aria-controls="spam" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel32" runat="server"
                                                    LocalizedTag="HEADER_SPAM"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="bot-tab" data-bs-toggle="tab" href="#bot" role="tab" aria-controls="bot" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel39" runat="server"
                                                    LocalizedTag="HEADER_BOTSPAM"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="login-tab" data-bs-toggle="tab" href="#login" role="tab" aria-controls="login" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server"
                                                    LocalizedTag="HEADER_LOGIN"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="attachments-tab" data-bs-toggle="tab" href="#attachments" role="tab" aria-controls="attachments" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel43" runat="server"
                                                    LocalizedTag="HEADER_ATTACHMENTS"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="albums-tab" data-bs-toggle="tab" href="#albums" role="tab" aria-controls="albums" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server"
                                                    LocalizedTag="HEADER_ALBUM"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="image-tab" data-bs-toggle="tab" href="#image" role="tab" aria-controls="image" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server"
                                                    LocalizedTag="HEADER_IMAGE_ATTACH"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="cdn-tab" data-bs-toggle="tab" href="#cdn" role="tab" aria-controls="cdn" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel40" runat="server"
                                                    LocalizedTag="HEADER_CDN"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                    </ul>
                    <div class="tab-content" id="setupTabContent">
                        <div class="tab-pane fade show active" id="setup" role="tabpanel" aria-labelledby="setup-tab">
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                               AssociatedControlID="ServerTimeCorrection"
                                               LocalizedTag="SERVERTIME_CORRECT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="input-group">
                                    <asp:TextBox CssClass="form-control serverTime-Input" ID="ServerTimeCorrection" runat="server" TextMode="Number"></asp:TextBox>
                                </div>
                                <small class="form-text text-body-secondary"><%# DateTime.UtcNow %></small>
                            </div>
                            <asp:PlaceHolder runat="server" ID="SSLSettings">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel71" runat="server"
                                                   AssociatedControlID="RequireSSL"
                                                   LocalizedTag="SSL_REQUIRE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="RequireSSL" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel5" runat="server"
                                                   AssociatedControlID="UseFileTable"
                                                   LocalizedTag="FILE_TABLE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="UseFileTable" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel7" runat="server"
                                                   AssociatedControlID="ShowCookieConsent"
                                                   LocalizedTag="SHOW_COOKIECONSET" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="ShowCookieConsent" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel6" runat="server"
                                               AssociatedControlID="AbandonSessionsForDontTrack"
                                               LocalizedTag="ABANDON_TRACKUSR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AbandonSessionsForDontTrack" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel9" runat="server"
                                               AssociatedControlID="EditTimeOut"
                                               LocalizedTag="POSTEDIT_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="EditTimeOut" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel16" runat="server"
                                               AssociatedControlID="CreateNntpUsers"
                                               LocalizedTag="CREATE_NNTPNAMES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="CreateNntpUsers" runat="server"></asp:CheckBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel223" runat="server"
                                                   AssociatedControlID="DisplayNameMinLength"
                                                   LocalizedTag="DISPLAYNAME_MIN_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" runat="server" ID="DisplayNameMinLength" />
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel11" runat="server"
                                                   AssociatedControlID="UserNameMaxLength"
                                                   LocalizedTag="NAME_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="UserNameMaxLength" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel12" runat="server"
                                                   AssociatedControlID="MaxReportPostChars"
                                                   LocalizedTag="MAX_POST_CHARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="MaxReportPostChars" runat="server" />
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel13" runat="server"
                                                   AssociatedControlID="MaxPostSize"
                                                   LocalizedTag="MAX_POST_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="MaxPostSize" runat="server"></asp:TextBox>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel14" runat="server"
                                                   AssociatedControlID="PostFloodDelay"
                                                   LocalizedTag="FLOOT_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="PostFloodDelay" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    <div class="tab-pane fade" id="password" role="tabpanel" aria-labelledby="password-tab">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel52" runat="server"
                                           AssociatedControlID="MinRequiredPasswordLength"
                                           LocalizedTag="PASSWORD_MIN_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="MinRequiredPasswordLength"
                                         CssClass="form-control"
                                         TextMode="Number"
                                         runat="server"></asp:TextBox>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel44" runat="server"
                                               AssociatedControlID="PasswordRequireNonLetterOrDigit"
                                               LocalizedTag="PASSWORD_REQUIRE_NON_LETTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="PasswordRequireNonLetterOrDigit" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel46" runat="server"
                                               AssociatedControlID="PasswordRequireDigit"
                                               LocalizedTag="PASSWORD_REQUIRE_DIGIT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="PasswordRequireDigit" runat="server" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel50" runat="server"
                                               AssociatedControlID="PasswordRequireLowercase"
                                               LocalizedTag="PASSWORD_LOWER_CASE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="PasswordRequireLowercase" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel53" runat="server"
                                               AssociatedControlID="PasswordRequireUppercase"
                                               LocalizedTag="PASSWORD_UPPER_CASE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="PasswordRequireUppercase" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                        <div class="tab-pane fade" id="spam" role="tabpanel" aria-labelledby="spam-tab">
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel185" runat="server"
                                               AssociatedControlID="SpamServiceType"
                                               LocalizedTag="CHECK_FOR_SPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />


                                <asp:DropDownList CssClass="form-select" ID="SpamServiceType" runat="server">
                                </asp:DropDownList>
                            </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel187" runat="server"
                                           AssociatedControlID="SpamMessageHandling"
                                           LocalizedTag="SPAM_MESSAGE_HANDLING" LocalizedPage="ADMIN_HOSTSETTINGS" Suffix=":" />


                            <asp:DropDownList CssClass="form-select" ID="SpamMessageHandling" runat="server">
                            </asp:DropDownList>
                        </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel239" runat="server"
                                               AssociatedControlID="IgnoreSpamWordCheckPostCount"
                                               LocalizedTag="IGNORE_SPAMCHECK_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox ID="IgnoreSpamWordCheckPostCount" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel242" runat="server"
                                               AssociatedControlID="AllowedNumberOfUrls"
                                               LocalizedTag="SPAMCHECK_ALLOWED_URLS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox ID="AllowedNumberOfUrls" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <asp:PlaceHolder runat="server" ID="BotRegisterCheck">
                        <div class="tab-pane fade" id="bot" role="tabpanel" aria-labelledby="bot-tab">
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel224" runat="server"
                                               AssociatedControlID="BotSpamServiceType"
                                               LocalizedTag="CHECK_FOR_BOTSPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:DropDownList CssClass="form-select" ID="BotSpamServiceType" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel225" runat="server"
                                               AssociatedControlID="BotScoutApiKey"
                                               LocalizedTag="BOTSCOUT_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="input-group">
                                    <asp:TextBox ID="BotScoutApiKey" CssClass="form-control" runat="server"></asp:TextBox>
                                    <YAF:ThemeButton runat="server" ID="ThemeButton1"
                                                     NavigateUrl="http://botscout.com/getkey.htm"
                                                     Type="Info"
                                                     Icon="sign-in-alt"
                                                     TextLocalizedTag="AKISMET_KEY_DOWN" />
                                </div>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel192" runat="server"
                                               AssociatedControlID="StopForumSpamApiKey"
                                               LocalizedTag="STOPFORUMSPAM_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="input-group">
                                    <asp:TextBox ID="StopForumSpamApiKey" CssClass="form-control" runat="server"></asp:TextBox>
                                    <YAF:ThemeButton runat="server" ID="ThemeButton2"
                                                     NavigateUrl="http://stopforumspam.com"
                                                     Type="Info"
                                                     Icon="sign-in-alt"
                                                     TextLocalizedTag="AKISMET_KEY_DOWN" />
                                </div>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel226" runat="server"
                                               AssociatedControlID="BotHandlingOnRegister"
                                               LocalizedTag="BOT_CHECK_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:DropDownList ID="BotHandlingOnRegister" runat="server"
                                                  CssClass="form-select"></asp:DropDownList>
                            </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel227" runat="server"
                                           AssociatedControlID="BanBotIpOnDetection"
                                           LocalizedTag="BOT_IPBAN_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="BanBotIpOnDetection" runat="server" />
                            </div>
                            </div>
                        </div>
                        </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="LoginSettings">
                        <div class="tab-pane fade" id="login" role="tabpanel" aria-labelledby="login-tab">
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel17" runat="server"
                                                   AssociatedControlID="DisableRegistrations"
                                                   LocalizedTag="DISABLE_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="DisableRegistrations" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel19" runat="server"
                                                   AssociatedControlID="RequireLogin"
                                                   LocalizedTag="REQUIRE_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="RequireLogin" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel238" runat="server"
                                               AssociatedControlID="ShowConnectMessageInTopic"
                                               LocalizedTag="SHOW_CONNECT_MESSAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowConnectMessageInTopic" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel237" runat="server"
                                               AssociatedControlID="SendWelcomeNotificationAfterRegister"
                                               LocalizedTag="WELCOME_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:DropDownList CssClass="form-select" ID="SendWelcomeNotificationAfterRegister" runat="server"></asp:DropDownList>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel18" runat="server"
                                               AssociatedControlID="CustomLoginRedirectUrl"
                                               LocalizedTag="LOGIN_REDIR_URL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="CustomLoginRedirectUrl" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel175" runat="server"
                                               AssociatedControlID="ShowRulesForRegistration"
                                               LocalizedTag="RULES_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowRulesForRegistration" runat="server" />
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="tab-pane fade" id="attachments" role="tabpanel" aria-labelledby="attachments-tab">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel216" runat="server"
                                           AssociatedControlID="AllowPrivateMessageAttachments"
                                           LocalizedTag="PM_ATTACHMENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="AllowPrivateMessageAttachments" runat="server"></asp:CheckBox>

                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel8" runat="server"
                                           AssociatedControlID="MaxFileSize"
                                           LocalizedTag="MAX_FILESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="MaxFileSize" runat="server"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel31" runat="server"
                                           AssociatedControlID="AllowedFileExtensions"
                                           LocalizedTag="ALLOWED_EXTENSIONS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="AllowedFileExtensions" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="albums" role="tabpanel" aria-labelledby="albums-tab">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel32" runat="server"
                                           AssociatedControlID="EnableAlbum"
                                           LocalizedTag="ENABLE_ABLBUMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="EnableAlbum" runat="server"></asp:CheckBox>

                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel34" runat="server"
                                           AssociatedControlID="AlbumImagesSizeMax"
                                           LocalizedTag="ALBUM_IMAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="AlbumImagesSizeMax" TextMode="Number" runat="server"></asp:TextBox>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel35" runat="server"
                                               AssociatedControlID="AlbumsPerPage"
                                               LocalizedTag="ALBUMS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="AlbumsPerPage" TextMode="Number" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel36" runat="server"
                                               AssociatedControlID="AlbumImagesPerPage"
                                               LocalizedTag="ALBUM_IMAGES_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="AlbumImagesPerPage" TextMode="Number" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                        <div class="tab-pane fade" id="image" role="tabpanel" aria-labelledby="image-tab">
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel21" runat="server"
                                               AssociatedControlID="PictureAttachmentDisplayTreshold"
                                               LocalizedTag="DISPLAY_TRESHOLD_IMGATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="PictureAttachmentDisplayTreshold" runat="server"></asp:TextBox>
                            </div>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel22" runat="server"
                                                   AssociatedControlID="EnableImageAttachmentResize"
                                                   LocalizedTag="IMAGE_ATTACH_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="EnableImageAttachmentResize" runat="server" />
                                    </div>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel217" runat="server"
                                                   AssociatedControlID="ResizePostedImages"
                                                   LocalizedTag="POSTED_IMAGE_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="ResizePostedImages" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel20" runat="server"
                                                   AssociatedControlID="ImageThumbnailMaxWidth"
                                                   LocalizedTag="IMAGE_THUMB_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="ImageThumbnailMaxWidth" runat="server"></asp:TextBox>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel29" runat="server"
                                                   AssociatedControlID="ImageThumbnailMaxHeight"
                                                   LocalizedTag="IMAGE_THUMB_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="ImageThumbnailMaxHeight" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel23" runat="server"
                                                   AssociatedControlID="ImageAttachmentResizeWidth"
                                                   LocalizedTag="IMAGE_RESIZE_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="ImageAttachmentResizeWidth" runat="server"></asp:TextBox>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel24" runat="server"
                                                   AssociatedControlID="ImageAttachmentResizeHeight"
                                                   LocalizedTag="IMAGE_RESIZE_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="ImageAttachmentResizeHeight" runat="server"></asp:TextBox>
                                </div>
                            </div>
                    </div>
                    <div class="tab-pane fade" id="cdn" role="tabpanel" aria-labelledby="cdn-tab">
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel235" runat="server"
                                               AssociatedControlID="ScriptManagerScriptsCDNHosted"
                                               LocalizedTag="CDN_SCRIPTMANAGER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ScriptManagerScriptsCDNHosted" runat="server" />
                                </div>
                            </div>
                        </div>
                    </div>
                    </div>


                    </div>
                <div class="card-footer text-lg-center">
                <YAF:ThemeButton ID="Save" runat="server"  Type="Primary" OnClick="SaveClick"
                             Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                </YAF:ThemeButton>
            </div>
                </div>
            </div>
            <div class="tab-pane fade" id="View2" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel60" runat="server"
                                              LocalizedTag="HEADER_SERVER_INFO"
                                              LocalizedPage="HEADER_FEATURES" />
                    </div>
                    <div class="card-body">
                         <ul class="nav nav-tabs" id="featuresTab" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="display-tab" data-bs-toggle="tab" href="#display" role="tab" aria-controls="display" aria-selected="true">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                    LocalizedTag="HEADER_DISPLAY"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="hover-tab" data-bs-toggle="tab" href="#hover" role="tab" aria-controls="hover" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                                                    LocalizedTag="HEADER_HOVERCARD"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="poll-tab" data-bs-toggle="tab" href="#poll" role="tab" aria-controls="poll" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                                    LocalizedTag="HEADER_POLL"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="pms-tab" data-bs-toggle="tab" href="#pms" role="tab" aria-controls="pms" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                                                    LocalizedTag="HEADER_PMS"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="hot-tab" data-bs-toggle="tab" href="#hot" role="tab" aria-controls="hot" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                    LocalizedTag="HEADER_HOTTOPICS"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="syndication-tab" data-bs-toggle="tab" href="#syndication" role="tab" aria-controls="syndication" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                                                    LocalizedTag="HEADER_SYNDICATION"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="geo-tab" data-bs-toggle="tab" href="#geo" role="tab" aria-controls="geo" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server"
                                                    LocalizedTag="HEADER_GEOLOCATION"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="reputation-tab" data-bs-toggle="tab" href="#reputation" role="tab" aria-controls="reputation" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server"
                                                    LocalizedTag="HEADER_REPUTATION"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="notification-tab" data-bs-toggle="tab" href="#notification" role="tab" aria-controls="notification" aria-selected="false">
                                <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server"
                                                    LocalizedTag="HEADER_MESSAGE_NOTIFICATION"
                                                    LocalizedPage="ADMIN_HOSTSETTINGS" />
                            </a>
                        </li>
                    </ul>
                    <div class="tab-content" id="featuresTabContent">
                        <div class="tab-pane fade show active" id="display" role="tabpanel" aria-labelledby="display-tab">
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel193" runat="server"
                                                   AssociatedControlID="UseReadTrackingByDatabase"
                                                   LocalizedTag="USE_READ_TRACKING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="UseReadTrackingByDatabase" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel93" runat="server"
                                                   AssociatedControlID="AddDynamicPageMetaTags"
                                                   LocalizedTag="DYNAMIC_METATAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="AddDynamicPageMetaTags" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                            </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel91" runat="server"
                                               AssociatedControlID="ShowRelativeTime"
                                               LocalizedTag="SHOW_RELATIVE_TIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowRelativeTime" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel86" runat="server"
                                               AssociatedControlID="UseFarsiCalender"
                                               LocalizedTag="USE_FARSI_CALENDER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="UseFarsiCalender" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel95" runat="server"
                                               AssociatedControlID="AllowUserHideHimself"
                                               LocalizedTag="ALLOW_USER_HIDE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowUserHideHimself" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel99" runat="server"
                                               AssociatedControlID="ShowUserOnlineStatus"
                                               LocalizedTag="SHOW_USER_STATUS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowUserOnlineStatus" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel96" runat="server"
                                               AssociatedControlID="EnableDisplayName"
                                               LocalizedTag="ENABLE_DISPLAY_NAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="EnableDisplayName" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel97" runat="server"
                                               AssociatedControlID="AllowDisplayNameModification"
                                               LocalizedTag="ALLOW_MODIFY_DISPLAYNAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowDisplayNameModification" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel85" runat="server"
                                               AssociatedControlID="UseStyledNicks"
                                               LocalizedTag="STYLED_NICKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="UseStyledNicks"
                                                  Text="&nbsp;"/>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel209" runat="server"
                                               AssociatedControlID="UseStyledTopicTitles"
                                               LocalizedTag="STYLED_TOPIC_TITLES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox runat="server" ID="UseStyledTopicTitles"
                                                  Text="&nbsp;"/>
                                </div>
                            </div>
                        </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel102" runat="server"
                                               AssociatedControlID="ShowThanksDate"
                                               LocalizedTag="SHOW_THANK_DATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowThanksDate" runat="server"></asp:CheckBox>
                                </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel101" runat="server"
                                           AssociatedControlID="EnableBuddyList"
                                           LocalizedTag="ENABLE_BUDDYLIST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="EnableBuddyList" runat="server"></asp:CheckBox>
                            </div>
                        </div>
                       <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel103" runat="server"
                                           AssociatedControlID="RemoveNestedQuotes"
                                           LocalizedTag="REMOVE_NESTED_QUOTES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="RemoveNestedQuotes" runat="server"></asp:CheckBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel113" runat="server"
                                               AssociatedControlID="ShowQuickAnswer"
                                               LocalizedTag="ALLOW_QUICK_ANSWER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowQuickAnswer" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel112" runat="server"
                                               AssociatedControlID="AllowEmailTopic"
                                               LocalizedTag="ALLOW_EMAIL_TOPIC" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowEmailTopic" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel104" runat="server"
                                               AssociatedControlID="DisableNoFollowLinksAfterDay"
                                               LocalizedTag="DISABLE_NOFOLLOW_ONOLDERPOSTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="DisableNoFollowLinksAfterDay" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel107" runat="server"
                                               AssociatedControlID="LockPosts"
                                               LocalizedTag="DAYS_BEFORE_POSTLOCK" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="LockPosts" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel105" runat="server"
                                           AssociatedControlID="ShowShareTopicTo"
                                           LocalizedTag="ALLOW_SHARE_TOPIC" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:DropDownList CssClass="form-select" ID="ShowShareTopicTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </div>
                        </div>
                        <div class="tab-pane fade" id="hover" role="tabpanel" aria-labelledby="hover-tab">
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel218" runat="server"
                                                   AssociatedControlID="EnableUserInfoHoverCards"
                                                   LocalizedTag="ENABLE_HOVERCARDS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="EnableUserInfoHoverCards" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel220" runat="server"
                                                   AssociatedControlID="HoverCardOpenDelay"
                                                   LocalizedTag="HOVERCARD_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <asp:TextBox CssClass="form-control" ID="HoverCardOpenDelay" runat="server"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="poll" role="tabpanel" aria-labelledby="poll-tab">
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel116" runat="server"
                                               AssociatedControlID="AllowedPollChoiceNumber"
                                               LocalizedTag="MAX_ALLOWED_CHOICES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="AllowedPollChoiceNumber" MaxLength="2" runat="server"></asp:TextBox>
                            </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel118" runat="server"
                                               AssociatedControlID="AllowPollChangesAfterFirstVote"
                                               LocalizedTag="ALLOW_CHANGE_AFTERVOTE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowPollChangesAfterFirstVote" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel119" runat="server"
                                               AssociatedControlID="AllowMultipleChoices"
                                               LocalizedTag="ALLOW_MULTI_VOTING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowMultipleChoices" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                            <div class="row">
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel120" runat="server"
                                                   AssociatedControlID="AllowUsersHidePollResults"
                                                   LocalizedTag="ALLOW_HIDE_POLLRESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="AllowUsersHidePollResults" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                                <div class="mb-3 col-md-6">
                                    <YAF:HelpLabel ID="HelpLabel122" runat="server"
                                                   AssociatedControlID="AllowUsersImagedPoll"
                                                   LocalizedTag="ALLOW_USERS_POLLIMAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    <div class="form-check form-switch">
                                        <asp:CheckBox Text="&nbsp;" ID="AllowUsersImagedPoll" runat="server"></asp:CheckBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    <div class="tab-pane fade" id="pms" role="tabpanel" aria-labelledby="pms-tab">
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel124" runat="server"
                                               AssociatedControlID="AllowPrivateMessages"
                                               LocalizedTag="ALLOW_PMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowPrivateMessages" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel125" runat="server"
                                               AssociatedControlID="AllowPMEmailNotification"
                                               LocalizedTag="ALLOW_PM_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="AllowPMEmailNotification" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel126" runat="server"
                                           AssociatedControlID="PrivateMessageMaxRecipients"
                                           LocalizedTag="MAX_PM_RECIPIENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="PrivateMessageMaxRecipients" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="hot" role="tabpanel" aria-labelledby="hot-tab">
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel197" runat="server"
                                               AssociatedControlID="PopularTopicViews"
                                               LocalizedTag="POPULAR_VIEWS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="PopularTopicViews" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel198" runat="server"
                                               AssociatedControlID="PopularTopicReplys"
                                               LocalizedTag="POPULAR_REPLYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="PopularTopicReplys" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel199" runat="server"
                                               AssociatedControlID="PopularTopicDays"
                                               LocalizedTag="POPULAR_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="PopularTopicDays" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="syndication" role="tabpanel" aria-labelledby="syndication-tab">
                        <div class="row">
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel132" runat="server"
                                               AssociatedControlID="ShowAtomLink"
                                               LocalizedTag="SHOW_ATOM_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ShowAtomLink" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                           AssociatedControlID="TopicsFeedItemsCount"
                                           LocalizedTag="TOPICFEED_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="TopicsFeedItemsCount" runat="server"></asp:TextBox>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel133" runat="server"
                                               AssociatedControlID="PostsFeedAccess"
                                               LocalizedTag="POSTS_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:DropDownList CssClass="form-select" ID="PostsFeedAccess" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel134" runat="server"
                                               AssociatedControlID="PostLatestFeedAccess"
                                               LocalizedTag="LASTPOSTS_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:DropDownList CssClass="form-select" ID="PostLatestFeedAccess" runat="server">
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel136" runat="server"
                                           AssociatedControlID="TopicsFeedAccess"
                                           LocalizedTag="TOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:DropDownList CssClass="form-select" ID="TopicsFeedAccess" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="geo" role="tabpanel" aria-labelledby="geo-tab">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel108" runat="server"
                                           AssociatedControlID="EnableIPInfoService"
                                           LocalizedTag="IP_INFOSERVICE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="EnableIPInfoService" runat="server"></asp:CheckBox>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel109" runat="server"
                                           AssociatedControlID="IPLocatorUrlPath"
                                           LocalizedTag="IP_INFOSERVICE_XMLURL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="IPLocatorUrlPath" runat="server"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel195" runat="server"
                                           AssociatedControlID="IPLocatorResultsMapping"
                                           LocalizedTag="IP_INFOSERVICE_DATAMAPPING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="IPLocatorResultsMapping" runat="server"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel110" runat="server"
                                           AssociatedControlID="IPInfoPageURL"
                                           LocalizedTag="IPINFO_ULRL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="IPInfoPageURL" runat="server"></asp:TextBox>
                            </div>
                    </div>
                    <div class="tab-pane fade" id="reputation" role="tabpanel" aria-labelledby="reputation-tab">
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel203" runat="server"
                                               AssociatedControlID="EnableUserReputation"
                                               LocalizedTag="ENABLE_USERREPUTATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="EnableUserReputation" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel204" runat="server"
                                               AssociatedControlID="ReputationAllowNegative"
                                               LocalizedTag="REPUTATION_ALLOWNEGATIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <div class="form-check form-switch">
                                    <asp:CheckBox Text="&nbsp;" ID="ReputationAllowNegative" runat="server"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel207" runat="server"
                                               AssociatedControlID="ReputationMaxNegative"
                                               LocalizedTag="REPUTATION_MIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="ReputationMaxNegative" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel208" runat="server"
                                               AssociatedControlID="ReputationMaxPositive"
                                               LocalizedTag="REPUTATION_MAX" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="ReputationMaxPositive" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel205" runat="server"
                                               AssociatedControlID="ReputationMinUpVoting"
                                               LocalizedTag="REPUTATION_MINUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="ReputationMinUpVoting" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel206" runat="server"
                                               AssociatedControlID="ReputationMinDownVoting"
                                               LocalizedTag="REPUTATION_MINDOWN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="ReputationMinDownVoting" runat="server"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="tab-pane fade" id="notification" role="tabpanel" aria-labelledby="notification-tab">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel255" runat="server"
                                           AssociatedControlID="MessageNotifcationDuration"
                                           LocalizedTag="NOTIFICATION_DURATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="MessageNotifcationDuration" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    </div>
                    </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="ThemeButton3" runat="server"  Type="Primary" OnClick="SaveClick"
                                     Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                    </YAF:ThemeButton>
                </div>
                </div>
            </div>
            <div class="tab-pane fade" id="View3" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel62" runat="server"
                                              LocalizedTag="HEADER_DISPLAY"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
        <YAF:HelpLabel ID="HelpLabel152" runat="server"
                       AssociatedControlID="ActiveListTime"
                       LocalizedTag="ACTIVE_USERTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
        <asp:TextBox CssClass="form-control" ID="ActiveListTime" runat="server" />
    </div>
    <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel176" runat="server"
                               AssociatedControlID="ActiveDiscussionsCount"
                               LocalizedTag="LASTPOST_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <asp:TextBox CssClass="form-control" ID="ActiveDiscussionsCount" runat="server" />

            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel156" runat="server"
                               AssociatedControlID="ShowGuestsInDetailedActiveList"
                               LocalizedTag="SHOW_GUESTS_INACTIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowGuestsInDetailedActiveList" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel157" runat="server"
                               AssociatedControlID="ShowCrawlersInActiveList"
                               LocalizedTag="SHOW_BOTS_INACTIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowCrawlersInActiveList" runat="server"></asp:CheckBox>

                </div>
            </div>
        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel158" runat="server"
                               AssociatedControlID="ShowDeletedMessages"
                               LocalizedTag="SHOW_DEL_MESSAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowDeletedMessages" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel159" runat="server"
                               AssociatedControlID="ShowDeletedMessagesToAll"
                               LocalizedTag="SHOW_DEL_MESSAGES_TOALL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowDeletedMessagesToAll" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel240" runat="server"
                               AssociatedControlID="ShowEditedMessage"
                               LocalizedTag="SHOW_EDIT_MESSAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowEditedMessage" runat="server"></asp:CheckBox>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="BlankLinksHelpLabel" runat="server"
                               AssociatedControlID="BlankLinks"
                               LocalizedTag="SHOW_LINKS_NEWWINDOW" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="BlankLinks" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel177" runat="server"
                               AssociatedControlID="UseNoFollowLinks"
                               LocalizedTag="NOFOLLOW_LINKTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="UseNoFollowLinks" runat="server"></asp:CheckBox>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel161" runat="server"
                               AssociatedControlID="NoCountForumsInActiveDiscussions"
                               LocalizedTag="SHOW_NOCOUNT_POSTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="NoCountForumsInActiveDiscussions" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel163" runat="server"
                               AssociatedControlID="ShowActiveDiscussions"
                               LocalizedTag="SHOW_ACTIVE_DISCUSSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowActiveDiscussions" runat="server"></asp:CheckBox>

                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel190" runat="server"
                               AssociatedControlID="ShowRecentUsers"
                               LocalizedTag="SHOW_RECENT_USERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowRecentUsers" runat="server"></asp:CheckBox>

                </div>
            </div>
        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel162" runat="server"
                               AssociatedControlID="ShowForumStatistics"
                               LocalizedTag="SHOW_FORUM_STATS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowForumStatistics" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel171" runat="server"
                               AssociatedControlID="ShowPageGenerationTime"
                               LocalizedTag="SHOW_RENDERTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowPageGenerationTime" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel172" runat="server"
                               AssociatedControlID="ShowYAFVersion"
                               LocalizedTag="SHOW_YAFVERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="ShowYAFVersion" runat="server"></asp:CheckBox>

                </div>
            </div>
        </div>
                    <div class="row">
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel168" runat="server"
                           AssociatedControlID="ShowGroupsProfile"
                           LocalizedTag="SHOW_GROUPS_INPROFILE" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowGroupsProfile" runat="server"></asp:CheckBox>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel170" runat="server"
                           AssociatedControlID="ShowBrowsingUsers"
                           LocalizedTag="SHOW_USERSBROWSING" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowBrowsingUsers" runat="server"></asp:CheckBox>
            </div>
        </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel181" runat="server"
                           AssociatedControlID="ShowSimilarTopics"
                           LocalizedTag="SHOW_SIMILARTOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowSimilarTopics" runat="server"></asp:CheckBox>
            </div>
        </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel164" runat="server"
                           AssociatedControlID="ShowForumJump"
                           LocalizedTag="SHOW_FORUM_JUMP" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowForumJump" runat="server"></asp:CheckBox>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel169" runat="server"
                           AssociatedControlID="ShowMedals"
                           LocalizedTag="SHOW_MEDALS" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowMedals" runat="server"></asp:CheckBox>
            </div>
        </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel154" runat="server"
                           AssociatedControlID="ShowMoved"
                           LocalizedTag="SHOW_MOVED_TOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowMoved" runat="server"></asp:CheckBox>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel182" runat="server"
                           AssociatedControlID="ShowHelpTo"
                           LocalizedTag="SHOWHELP" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <asp:DropDownList CssClass="form-select" ID="ShowHelpTo" runat="server" DataValueField="Value" DataTextField="Name">
            </asp:DropDownList>
        </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel183" runat="server"
                           AssociatedControlID="ShowTeamTo"
                           LocalizedTag="SHOWTEAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <asp:DropDownList CssClass="form-select" ID="ShowTeamTo" runat="server" DataValueField="Value" DataTextField="Name">
            </asp:DropDownList>
        </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel155" runat="server"
                           AssociatedControlID="ShowModeratorList"
                           LocalizedTag="SHOW_MODLIST" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <div class="form-check form-switch">
                <asp:CheckBox Text="&nbsp;" ID="ShowModeratorList" runat="server"></asp:CheckBox>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel178" runat="server"
                           AssociatedControlID="PostsPerPage"
                           LocalizedTag="POSTS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <asp:TextBox CssClass="form-control" ID="PostsPerPage" runat="server"></asp:TextBox>
        </div>
        <div class="mb-3 col-md-6">
            <YAF:HelpLabel ID="HelpLabel236" runat="server"
                           AssociatedControlID="SubForumsInForumList"
                           LocalizedTag="AMOUNT_OF_SUBFORUMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <asp:TextBox CssClass="form-control" ID="SubForumsInForumList" runat="server"></asp:TextBox>
        </div>
    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel37" runat="server"
                                       AssociatedControlID="ShowScrollBackToTopButton"
                                       LocalizedTag="SHOW_SCROLL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        <div class="form-check form-switch">
                            <asp:CheckBox Text="&nbsp;" ID="ShowScrollBackToTopButton" runat="server"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel25" runat="server"
                                       AssociatedControlID="ScrollToPost"
                                       LocalizedTag="SCROLL_TO_POST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        <div class="form-check form-switch">
                            <asp:CheckBox Text="&nbsp;" ID="ScrollToPost" runat="server"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel38" runat="server"
                                       AssociatedControlID="TwoColumnBoardLayout"
                                       LocalizedTag="TWOCOLUMN_BOARD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        <div class="form-check form-switch">
                            <asp:CheckBox Text="&nbsp;" ID="TwoColumnBoardLayout" runat="server"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                           AssociatedControlID="TitleTemplate"
                                           LocalizedTag="TEMPLATE_TITLE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="TitleTemplate" runat="server"></asp:TextBox>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel40" runat="server"
                                           AssociatedControlID="PagingTitleTemplate"
                                           LocalizedTag="TEMPLATE_TITLE_PAGER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox CssClass="form-control" ID="PagingTitleTemplate" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel15" runat="server"
                                       AssociatedControlID="UseCustomContextMenu"
                                       LocalizedTag="USE_CUSTOM_CONTEXTMENU" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        <div class="form-check form-switch">
                            <asp:CheckBox Text="&nbsp;" ID="UseCustomContextMenu" runat="server"></asp:CheckBox>
                        </div>
                    </div>
                    </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="ThemeButton4" runat="server"  Type="Primary" OnClick="SaveClick"
                                     Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                    </YAF:ThemeButton>
                </div>
                </div>
            </div>
            <div class="tab-pane fade" id="View4" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel64" runat="server"
                                              LocalizedTag="HEADER_ADVERTS"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel26" runat="server"
                                           AssociatedControlID="AdPost"
                                           LocalizedTag="POST_AD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox ID="AdPost" runat="server"
                                         Height="80px"
                                         CssClass="form-control"
                                         TextMode="MultiLine" />
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel27" runat="server"
                                           AssociatedControlID="ShowAdsToSignedInUsers"
                                           LocalizedTag="SHOWAD_LOGINUSERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox runat="server" ID="ShowAdsToSignedInUsers"
                                              Text="&nbsp;"/>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-lg-center">
                        <YAF:ThemeButton ID="ThemeButton5" runat="server"  Type="Primary" OnClick="SaveClick"
                                         Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                        </YAF:ThemeButton>
                    </div>
                </div>
            </div><div class="tab-pane fade" id="View5" role="tabpanel">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconType="text-secondary"
                                IconName="cog"
                                LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                - <YAF:LocalizedLabel ID="LocalizedLabel66" runat="server"
                                      LocalizedTag="HEADER_EDITORS"
                                      LocalizedPage="ADMIN_HOSTSETTINGS" />
            </div>
            <div class="card-body">
            </div>
            <div class="card-footer text-lg-center">
                <YAF:ThemeButton ID="ThemeButton6" runat="server"  Type="Primary" OnClick="SaveClick"
                                 Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                </YAF:ThemeButton>
            </div>
        </div>
            </div><div class="tab-pane fade" id="View6" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel68" runat="server"
                                              LocalizedTag="HEADER_PERMISSION"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel84" runat="server"
                               AssociatedControlID="AllowUserTheme"
                               LocalizedTag="USER_CHANGE_THEME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="AllowUserTheme" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel83" runat="server"
                               AssociatedControlID="AllowUserLanguage"
                               LocalizedTag="USER_CHANGE_LANGUAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="AllowUserLanguage" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel82" runat="server"
                               AssociatedControlID="AllowSignatures"
                               LocalizedTag="ALLOW_SIGNATURE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="AllowSignatures" runat="server"></asp:CheckBox>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel81" runat="server"
                               AssociatedControlID="AllowEmailSending"
                               LocalizedTag="ALLOW_SENDMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="AllowEmailSending" runat="server"></asp:CheckBox>
                </div>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel80" runat="server"
                               AssociatedControlID="AllowEmailChange"
                               LocalizedTag="ALLOW_EMAIL_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="AllowEmailChange" runat="server"></asp:CheckBox>

                </div>
            </div>
        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel78" runat="server"
                                           AssociatedControlID="AllowModeratorsViewIPs"
                                           LocalizedTag="ALLOW_MOD_VIEWIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="AllowModeratorsViewIPs" runat="server"></asp:CheckBox>

                            </div>
                        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel89" runat="server"
                               AssociatedControlID="AllowCreateTopicsSameName"
                               LocalizedTag="ALLOW_TOPICS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <asp:DropDownList CssClass="form-select" ID="AllowCreateTopicsSameName" runat="server">
                </asp:DropDownList>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel90" runat="server"
                               AssociatedControlID="AllowForumsWithSameName"
                               LocalizedTag="ALLOW_FORUMS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <div class="form-check form-switch">
                    <asp:CheckBox Text="&nbsp;" ID="AllowForumsWithSameName" runat="server"></asp:CheckBox>

                </div>
            </div>
        </div>
        <div class="mb-3">
            <YAF:HelpLabel ID="HelpLabel76" runat="server"
                           AssociatedControlID="ReportPostPermissions"
                           LocalizedTag="REPORT_POST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
            <asp:DropDownList CssClass="form-select" ID="ReportPostPermissions" runat="server">
            </asp:DropDownList>
        </div>
        <div class="row">
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel73" runat="server"
                               AssociatedControlID="ActiveUsersViewPermissions"
                               LocalizedTag="VIEWACTIVE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <asp:DropDownList CssClass="form-select" ID="ActiveUsersViewPermissions" runat="server">
                </asp:DropDownList>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel75" runat="server"
                               AssociatedControlID="ProfileViewPermissions"
                               LocalizedTag="VIEWPROFILE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <asp:DropDownList CssClass="form-select" ID="ProfileViewPermissions" runat="server">
                </asp:DropDownList>
            </div>
            <div class="mb-3 col-md-6">
                <YAF:HelpLabel ID="HelpLabel74" runat="server"
                               AssociatedControlID="MembersListViewPermissions"
                               LocalizedTag="VIEWMEMBERLIST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                <asp:DropDownList CssClass="form-select" ID="MembersListViewPermissions" runat="server">
                </asp:DropDownList>
            </div>
        </div>
                    </div>
                    <div class="card-footer text-lg-center">
                        <YAF:ThemeButton ID="ThemeButton7" runat="server"  Type="Primary" OnClick="SaveClick"
                                         Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                        </YAF:ThemeButton>
                    </div>
                </div>
            </div>
        <asp:PlaceHolder runat="server" ID="AvatarsTab">
        <div class="tab-pane fade" id="View8" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel72" runat="server"
                                              LocalizedTag="HEADER_AVATARS"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <asp:PlaceHolder runat="server" ID="AvatarSettingsHolder">
            <div class="row">
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel189" runat="server"
                                   AssociatedControlID="AvatarGallery"
                                   LocalizedTag="AVATAR_GALLERY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                    <div class="form-check form-switch">
                        <asp:CheckBox Text="&nbsp;" ID="AvatarGallery" runat="server"></asp:CheckBox>
                    </div>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel51" runat="server"
                                   AssociatedControlID="AvatarUpload"
                                   LocalizedTag="AVATAR_UPLOAD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                    <div class="form-check form-switch">
                        <asp:CheckBox Text="&nbsp;" ID="AvatarUpload" runat="server"></asp:CheckBox>
                    </div>
                </div>
            </div>
                            <div class="mb-3">
                                <YAF:HelpLabel ID="HelpLabel56" runat="server"
                                               AssociatedControlID="AvatarSize"
                                               LocalizedTag="AVATAR_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="AvatarSize" runat="server"></asp:TextBox>
                            </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel54" runat="server"
                                               AssociatedControlID="AvatarWidth"
                                               LocalizedTag="AVATAR_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="AvatarWidth" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel55" runat="server"
                                               AssociatedControlID="AvatarHeight"
                                               LocalizedTag="AVATAR_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="AvatarHeight" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        </asp:PlaceHolder>
                    </div>
                    <div class="card-footer text-lg-center">
                        <YAF:ThemeButton ID="ThemeButton9" runat="server"  Type="Primary" OnClick="SaveClick"
                                         Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                        </YAF:ThemeButton>
                    </div>
                    </div>
                </div>
        </asp:PlaceHolder>
        <div class="tab-pane fade" id="View9" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel74" runat="server"
                                              LocalizedTag="HEADER_CACHE"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel41" runat="server"
                                           AssociatedControlID="ForumStatisticsCacheTimeout"
                                           LocalizedTag="STATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="input-group">
                                <asp:TextBox CssClass="form-control" runat="server" ID="ForumStatisticsCacheTimeout" />
                                <YAF:ThemeButton ID="ForumStatisticsCacheReset" runat="server"
                                                 Type="Primary"
                                                 TextLocalizedTag="CLEAR"
                                                 Icon="trash"
                                                 OnClick="ForumStatisticsCacheResetClick" />
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel42" runat="server"
                                           LocalizedTag="USRSTATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="input-group">
                                <asp:TextBox CssClass="form-control" runat="server" ID="BoardUserStatsCacheTimeout" />
                                <YAF:ThemeButton Type="Primary" ID="BoardUserStatsCacheReset"
                                                 TextLocalizedTag="CLEAR"  runat="server"
                                                 Icon="trash"
                                                 OnClick="BoardUserStatsCacheResetClick" />
                            </div>
                        </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel43" runat="server"
                                       LocalizedTag="DISCUSSIONS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        <div class="input-group">
                            <asp:TextBox CssClass="form-control" runat="server" ID="ActiveDiscussionsCacheTimeout" />
                            <YAF:ThemeButton Type="Primary" ID="ActiveDiscussionsCacheReset"
                                             TextLocalizedTag="CLEAR"
                                             runat="server"
                                             Icon="trash"
                                             OnClick="ActiveDiscussionsCacheResetClick" />
                        </div>
                    </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel45" runat="server"
                                           LocalizedTag="MOD_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="input-group">
                                <asp:TextBox CssClass="form-control" runat="server" ID="BoardModeratorsCacheTimeout" />
                                <YAF:ThemeButton Type="Primary" ID="BoardModeratorsCacheReset"
                                                 TextLocalizedTag="CLEAR"  runat="server"
                                                 Icon="trash"
                                                 OnClick="BoardModeratorsCacheResetClick" />
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel48" runat="server"
                                           LocalizedTag="ONLINE_STATUS_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:TextBox runat="server" ID="OnlineStatusCacheTimeout"
                                         CssClass="form-control" />
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel49" runat="server"
                                           LocalizedTag="LAZY_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="ActiveUserLazyDataCacheTimeout"
                                             CssClass="form-control" />

                                <YAF:ThemeButton Type="Primary" ID="ActiveUserLazyDataCacheReset"
                                                 TextLocalizedTag="CLEAR" runat="server"
                                                 Icon="trash"
                                                 OnClick="UserLazyDataCacheResetClick" />
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-lg-center">
                        <YAF:ThemeButton  Type="Primary" runat="server" ID="ResetCacheAll"
                                          TextLocalizedTag="CLEAR_CACHE"
                                          Icon="trash"
                                          OnClick="ResetCacheAllClick" />
                    </div>
                </div>
            </div>
        <div class="tab-pane fade" id="View10" role="tabpanel">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="cog"
                                        LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                        - <YAF:LocalizedLabel ID="LocalizedLabel76" runat="server"
                                              LocalizedTag="HEADER_SEARCH"
                                              LocalizedPage="ADMIN_HOSTSETTINGS" />
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel28" runat="server"
                                               LocalizedTag="MAX_SEARCH_RESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="ReturnSearchMax" runat="server"></asp:TextBox>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel30" runat="server"
                                               LocalizedTag="SEARCH_MINLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                <asp:TextBox CssClass="form-control" ID="SearchStringMinLength" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel33" runat="server"
                                           LocalizedTag="SEARCH_PERMISS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <asp:DropDownList CssClass="form-select" ID="SearchPermissions" runat="server">
                            </asp:DropDownList>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel39" runat="server"
                                           LocalizedTag="QUICK_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <div class="form-check form-switch">
                                <asp:CheckBox Text="&nbsp;" ID="ShowQuickSearch" runat="server"></asp:CheckBox>
                            </div>
                        </div>
                        <div class="mb-3">
                            <YAF:HelpLabel runat="server"
                                           LocalizedTag="INDEX_SEARCH"></YAF:HelpLabel>
                            <YAF:ThemeButton runat="server" ID="IndexSearch"
                                             TextLocalizedTag="INDEX_SEARCH"
                                             TitleLocalizedTag="INDEX_SEARCH_HELP"
                                             Icon="sync"
                                             Type="Danger"
                                             OnClick="IndexSearch_OnClick"></YAF:ThemeButton>
                        </div>
                    </div>
                    <div class="card-footer text-lg-center">
                        <YAF:ThemeButton ID="ThemeButton10" runat="server"  Type="Primary" OnClick="SaveClick"
                                         Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                        </YAF:ThemeButton>
                    </div>
                </div>
            </div>
        <div class="tab-pane fade" id="View11" role="tabpanel">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconType="text-secondary"
                                    IconName="cog"
                                    LocalizedPage="ADMIN_HOSTSETTINGS"></YAF:IconHeader>
                    - <YAF:LocalizedLabel ID="LocalizedLabel78" runat="server"
                                          LocalizedTag="HEADER_LOG"
                                          LocalizedPage="ADMIN_HOSTSETTINGS" />
                </div>
                <div class="card-body">
                        <div class="row">
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel140" runat="server"
                                   LocalizedTag="EVENTLOG_MAX_MESSAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                    <asp:TextBox CssClass="form-control" ID="EventLogMaxMessages" runat="server"></asp:TextBox>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel141" runat="server"
                                   LocalizedTag="EVENTLOG_MAX_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                    <asp:TextBox CssClass="form-control" ID="EventLogMaxDays" runat="server"></asp:TextBox>
                </div>
                <div class="mb-3 col-md-6">
                    <YAF:HelpLabel ID="HelpLabel149" runat="server"
                                   LocalizedTag="MESSAGE_CHANGE_HISTORY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                    <asp:TextBox CssClass="form-control" ID="MessageHistoryDaysToLog" runat="server"></asp:TextBox>
                </div>
            </div>
                    <h2>
                <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server"
                                                LocalizedTag="HEADER_LOGSCOPE" LocalizedPage="ADMIN_HOSTSETTINGS" />
            </h2>
            <hr />
            <div class="row">
                <div class="mb-3 col-md-6">
                               <YAF:HelpLabel ID="LogErrorLabel" runat="server"
                                              LocalizedTag="LOG_ERROR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                               <div class="form-check form-switch">
                                   <asp:CheckBox Text="&nbsp;" ID="LogError" runat="server"></asp:CheckBox>
                               </div>
                           </div>
                <div class="mb-3 col-md-6">
                               <YAF:HelpLabel ID="HelpLabel142" runat="server"
                                              LocalizedTag="LOG_WARNING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                               <div class="form-check form-switch">
                                   <asp:CheckBox Text="&nbsp;" ID="LogWarning" runat="server"></asp:CheckBox>
                               </div>
                           </div>
                           <div class="mb-3 col-md-6">
                               <YAF:HelpLabel ID="HelpLabel180" runat="server"
                                              LocalizedTag="LOG_INFORMATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                               <div class="form-check form-switch">
                                   <asp:CheckBox Text="&nbsp;" ID="LogInformation" runat="server"></asp:CheckBox>
                               </div>
                           </div>
                </div>
                <div class="row">
                    <div class="mb-3 col-md-6">
                               <YAF:HelpLabel ID="HelpLabel210" runat="server"
                                              LocalizedTag="LOG_VIEWSTATEERROR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                               <div class="form-check form-switch">
                                   <asp:CheckBox Text="&nbsp;" ID="LogViewStateError" runat="server"></asp:CheckBox>
                               </div>
                           </div>
                           <div class="mb-3 col-md-6">
                               <YAF:HelpLabel ID="HelpLabel211" runat="server"
                                              LocalizedTag="LOG_BANNEDIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                               <div class="form-check form-switch">
                                   <asp:CheckBox Text="&nbsp;" ID="LogBannedIP" runat="server"></asp:CheckBox>
                               </div>
                           </div>
                       </div>
                      <div class="row">
                          <div class="mb-3 col-md-6">
                              <YAF:HelpLabel ID="HelpLabel212" runat="server"
                                             LocalizedTag="LOG_USERDELETED" LocalizedPage="ADMIN_HOSTSETTINGS" />
                              <div class="form-check form-switch">
                                  <asp:CheckBox Text="&nbsp;" ID="LogUserDeleted" runat="server"></asp:CheckBox>
                              </div>
                          </div>
                          <div class="mb-3 col-md-6">
                              <YAF:HelpLabel ID="HelpLabel213" runat="server"
                                             LocalizedTag="LOG_SUSPENDEDANDCONTRA" LocalizedPage="ADMIN_HOSTSETTINGS" />
                              <div class="form-check form-switch">
                                  <asp:CheckBox Text="&nbsp;" ID="LogUserSuspendedUnsuspended" runat="server"></asp:CheckBox>
                              </div>
                          </div>
                      </div>
            </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="ThemeButton11" runat="server"  Type="Primary" OnClick="SaveClick"
                                     Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                    </YAF:ThemeButton>
                </div>
                </div>
            </div>
    </asp:Panel>
        </div>
    </div>

<asp:HiddenField runat="server" ID="hidLastTab" Value="View1" />