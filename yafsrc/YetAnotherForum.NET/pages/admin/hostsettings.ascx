<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.hostsettings"
    CodeBehind="hostsettings.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel41" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_HOSTSETTINGS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-cog fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel42" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                                    </div>
                <div class="card-body">
  <asp:Panel id="HostSettingsTabs" runat="server">
    <ul class="nav nav-tabs" role="tablist">
        <li class="nav-item"><a href="#View0" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel44" runat="server" LocalizedTag="HEADER_SERVER_INFO" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View1" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
		<li class="nav-item"><a href="#View2" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HOST_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
		<li class="nav-item"><a href="#View3" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HOST_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View4" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HOST_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View5" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HOST_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View6" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="HOST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View7" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="HOST_TEMPLATES" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View8" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="HOST_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View9" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="HOST_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View10" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="HOST_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li class="nav-item"><a href="#View11" class="nav-link" data-toggle="tab" role="tab"><YAF:LocalizedLabel ID="LocalizedLabel34" runat="server" LocalizedTag="HOST_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
	</ul>
      <div class="tab-content">
      <div id="View0" class="tab-pane" role="tabpanel">
                 <h2>
                           <YAF:LocalizedLabel ID="LocalizedLabel37" runat="server" LocalizedTag="HEADER_SERVER_INFO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="SERVER_VERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:Label ID="SQLVersion" runat="server" CssClass="smallfont"></asp:Label>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel231" runat="server" LocalizedTag="APP_OS_NAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:Label ID="AppOSName" runat="server" CssClass="smallfont"></asp:Label>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel232" runat="server" LocalizedTag="APP_RUNTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:Label ID="AppRuntime" runat="server" CssClass="smallfont"></asp:Label>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel233" runat="server" LocalizedTag="APP_CORES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:Label ID="AppCores" runat="server" CssClass="smallfont"></asp:Label>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel234" runat="server" LocalizedTag="APP_MEMORY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:Label ID="AppMemory" runat="server" CssClass="smallfont"></asp:Label>

                    </p>
      </div>
    <div id="View1" class="tab-pane" role="tabpanel">
                        <h2>
                           <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="HEADER_SETUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                        <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="SERVERTIME_CORRECT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control serverTime-Input" ID="ServerTimeCorrection" runat="server"></asp:TextBox>
                        </p>
    <strong><%# DateTime.UtcNow %></strong><hr />

                        <h4>
                           <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="FILE_TABLE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseFileTable" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="ABANDON_TRACKUSR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AbandonSessionsForDontTrack" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="POSTEDIT_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="EditTimeOut" runat="server"></asp:TextBox>
                                            </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="WSERVICE_TOKEN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="WebServiceToken" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel223" runat="server" LocalizedTag="DISPLAYNAME_MIN_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="DisplayNameMinLength" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="NAME_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserNameMaxLength" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="MAX_POST_CHARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="MaxReportPostChars" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="MAX_POST_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="MaxPostSize" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="FLOOT_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PostFloodDelay" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="REFERRER_CHECK" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="DoUrlReferrerSecurityCheck" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="CREATE_NNTPNAMES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="CreateNntpUsers" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel32" runat="server" LocalizedTag="HEADER_SPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                        <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel185" runat="server" LocalizedTag="CHECK_FOR_SPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="SpamServiceType" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel187" runat="server" LocalizedTag="SPAM_MESSAGE_HANDLING" LocalizedPage="ADMIN_HOSTSETTINGS" Suffix=":" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="SpamMessageHandling" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel186" runat="server" LocalizedTag="AKISMET_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <YAF:ThemeButton runat="server" ID="GetRemovalKey" 
                                             NavigateUrl="https://akismet.com/signup/"
                                             CssClass="btn btn-info float-right"
                                             Icon="sign-in-alt"
                                             TextLocalizedTag="AKISMET_KEY_DOWN">
                            </YAF:ThemeButton>
                        </h4>
                        <p>
                            <asp:TextBox ID="AkismetApiKey" CssClass="form-control" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel239" runat="server" LocalizedTag="IGNORE_SPAMCHECK_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox ID="IgnoreSpamWordCheckPostCount" CssClass="form-control" runat="server"></asp:TextBox>

                    </p><hr />
                    <asp:PlaceHolder runat="server" ID="BotRegisterCheck">

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel39" runat="server" LocalizedTag="HEADER_BOTSPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                        <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel224" runat="server" LocalizedTag="CHECK_FOR_BOTSPAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="BotSpamServiceType" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel225" runat="server" LocalizedTag="BOTSCOUT_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <YAF:ThemeButton runat="server" ID="ThemeButton1" 
                                             NavigateUrl="http://botscout.com/getkey.htm"
                                             CssClass="btn btn-info float-right"
                                             Icon="sign-in-alt"
                                             TextLocalizedTag="AKISMET_KEY_DOWN">
                            </YAF:ThemeButton>
                        </h4>
                        <p>
                            <asp:TextBox ID="BotScoutApiKey" CssClass="form-control" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel192" runat="server" LocalizedTag="STOPFORUMSPAM_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <YAF:ThemeButton runat="server" ID="ThemeButton2" 
                                             NavigateUrl="http://stopforumspam.com"
                                             CssClass="btn btn-info float-right"
                                             Icon="sign-in-alt"
                                             TextLocalizedTag="AKISMET_KEY_DOWN">
                            </YAF:ThemeButton>
                        </h4>
                        <p>
                            <asp:TextBox ID="StopForumSpamApiKey" CssClass="form-control" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel226" runat="server" LocalizedTag="BOT_CHECK_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList ID="BotHandlingOnRegister" runat="server" CssClass="custom-select"></asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel227" runat="server" LocalizedTag="BOT_IPBAN_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="BanBotIpOnDetection" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel242" runat="server" LocalizedTag="SPAMCHECK_ALLOWED_URLS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox ID="AllowedNumberOfUrls" CssClass="form-control" runat="server"></asp:TextBox>

                    </p><hr />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="LoginSettings">

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="HEADER_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel17" runat="server" LocalizedTag="DISABLE_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="DisableRegistrations" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="EMAIL_VERIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EmailVerification" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel238" runat="server" LocalizedTag="SHOW_CONNECT_MESSAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowConnectMessageInTopic" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel237" runat="server" LocalizedTag="WELCOME_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                           <asp:DropDownList CssClass="custom-select" ID="SendWelcomeNotificationAfterRegister" runat="server"></asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel18" runat="server" LocalizedTag="LOGIN_REDIR_URL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="CustomLoginRedirectUrl" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel19" runat="server" LocalizedTag="REQUIRE_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="RequireLogin" runat="server" />

                    </p><hr />

                         <h4>
                            <YAF:HelpLabel ID="HelpLabel191" runat="server" LocalizedTag="ENABLE_SSO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowSingleSignOn" runat="server" />

                    </p><hr />

                         <h4>
                            <YAF:HelpLabel ID="HelpLabel20" runat="server" LocalizedTag="MODAL_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseLoginBox" runat="server" />

                    </p><hr />
                    </asp:PlaceHolder>
    <h2>
        <YAF:LocalizedLabel ID="LocalizedLabel43" runat="server" LocalizedTag="HEADER_ATTACHMENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
    </h2>
    <hr />
        
    <h4>
        <YAF:HelpLabel ID="HelpLabel216" runat="server" LocalizedTag="PM_ATTACHMENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
    </h4>
    <p>
        <asp:CheckBox CssClass="form-control" ID="AllowPrivateMessageAttachments" runat="server"></asp:CheckBox>

    </p><hr />

    <h4>
        <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="MAX_FILESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
    </h4>
    <p>
        <asp:TextBox CssClass="form-control" ID="MaxFileSize" runat="server"></asp:TextBox>

    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedTag="HEADER_IMAGE_ATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel21" runat="server" LocalizedTag="DISPLAY_TRESHOLD_IMGATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PictureAttachmentDisplayTreshold" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel22" runat="server" LocalizedTag="IMAGE_ATTACH_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableImageAttachmentResize" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel217" runat="server" LocalizedTag="POSTED_IMAGE_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ResizePostedImages" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel23" runat="server" LocalizedTag="IMAGE_RESIZE_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ImageAttachmentResizeWidth" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel24" runat="server" LocalizedTag="IMAGE_RESIZE_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ImageAttachmentResizeHeight" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel25" runat="server" LocalizedTag="CROP_IMAGE_ATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ImageAttachmentResizeCropped" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel40" runat="server" LocalizedTag="HEADER_CDN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel235" runat="server" LocalizedTag="CDN_SCRIPTMANAGER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ScriptManagerScriptsCDNHosted" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel228" runat="server" LocalizedTag="CDN_JQUERY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="JqueryCDNHosted" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel230" runat="server" LocalizedTag="BOARD_CDN_HOSTED" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="JqueryUIThemeCDNHosted" runat="server" />

                    </p>

	</div>
    <div id="View2" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HEADER_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel193" runat="server" LocalizedTag="USE_READ_TRACKING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseReadTrackingByDatabase" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel86" runat="server" LocalizedTag="USE_FARSI_CALENDER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseFarsiCalender" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel91" runat="server" LocalizedTag="SHOW_RELATIVE_TIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowRelativeTime" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel93" runat="server" LocalizedTag="DYNAMIC_METATAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AddDynamicPageMetaTags" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel94" runat="server" LocalizedTag="ALLOW_DISPLAY_GENDER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowGenderInUserBox" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel202" runat="server" LocalizedTag="ALLOW_DISPLAY_COUNTRY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowCountryInfoInUserBox" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel95" runat="server" LocalizedTag="ALLOW_USER_HIDE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowUserHideHimself" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel96" runat="server" LocalizedTag="ENABLE_DISPLAY_NAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableDisplayName" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel97" runat="server" LocalizedTag="ALLOW_MODIFY_DISPLAYNAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowDisplayNameModification" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel85" runat="server" LocalizedTag="STYLED_NICKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox runat="server" CssClass="form-control" ID="UseStyledNicks" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel209" runat="server" LocalizedTag="STYLED_TOPIC_TITLES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox runat="server" CssClass="form-control" ID="UseStyledTopicTitles" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel98" runat="server" LocalizedTag="MEMBERLIST_PAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="MemberListPageSize" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel139" runat="server" LocalizedTag="MYTOPICSLIST_PAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="MyTopicsListPageSize" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel99" runat="server" LocalizedTag="SHOW_USER_STATUS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowUserOnlineStatus" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel100" runat="server" LocalizedTag="ALLOW_THANKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableThanksMod" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel102" runat="server" LocalizedTag="SHOW_THANK_DATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowThanksDate" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel101" runat="server" LocalizedTag="ENABLE_BUDDYLIST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableBuddyList" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel103" runat="server" LocalizedTag="REMOVE_NESTED_QUOTES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="RemoveNestedQuotes" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel104" runat="server" LocalizedTag="DISABLE_NOFOLLOW_ONOLDERPOSTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="DisableNoFollowLinksAfterDay" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel107" runat="server" LocalizedTag="DAYS_BEFORE_POSTLOCK" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="LockPosts" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel111" runat="server" LocalizedTag="ALLOW_POSTBLOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowPostToBlog" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel113" runat="server" LocalizedTag="ALLOW_QUICK_ANSWER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowQuickAnswer" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel114" runat="server" LocalizedTag="ENABLE_CALENDER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableDNACalendar" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel105" runat="server" LocalizedTag="ALLOW_SHARE_TOPIC" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ShowShareTopicTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel184" runat="server" LocalizedTag="ENABLE_RETWEET_MSG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ShowRetweetMessageTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel173" runat="server" LocalizedTag="TWITTER_USERNAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="TwitterUserName" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel112" runat="server" LocalizedTag="ALLOW_EMAIL_TOPIC" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowEmailTopic" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel188" runat="server" LocalizedTag="ALLOW_TOPIC_DESCRIPTION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableTopicDescription" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel194" runat="server" LocalizedTag="ALLOW_TOPIC_STATUS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableTopicStatus" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel38" runat="server" LocalizedTag="HEADER_HOVERCARD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel218" runat="server" LocalizedTag="ENABLE_HOVERCARDS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableUserInfoHoverCards" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel220" runat="server" LocalizedTag="HOVERCARD_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                             <asp:TextBox CssClass="form-control" ID="HoverCardOpenDelay" runat="server"></asp:TextBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="HEADER_POLL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel115" runat="server" LocalizedTag="MAX_ALLOWED_POLLS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AllowedPollNumber" MaxLength="2" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel116" runat="server" LocalizedTag="MAX_ALLOWED_CHOICES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AllowedPollChoiceNumber" MaxLength="2" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel117" runat="server" LocalizedTag="POLLVOTING_PERIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="PollVoteTiedToIP" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel118" runat="server" LocalizedTag="ALLOW_CHANGE_AFTERVOTE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowPollChangesAfterFirstVote" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel119" runat="server" LocalizedTag="ALLOW_MULTI_VOTING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowMultipleChoices" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel120" runat="server" LocalizedTag="ALLOW_HIDE_POLLRESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowUsersHidePollResults" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel121" runat="server" LocalizedTag="ALLOW_GUESTS_VIEWPOLL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowGuestsViewPollOptions" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel122" runat="server" LocalizedTag="ALLOW_USERS_POLLIMAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowUsersImagedPoll" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel123" runat="server" LocalizedTag="POLL_IMAGE_FILESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PollImageMaxFileSize" MaxLength="4" runat="server"></asp:TextBox>
                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedTag="HEADER_PMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel124" runat="server" LocalizedTag="ALLOW_PMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowPrivateMessages" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel125" runat="server" LocalizedTag="ALLOW_PM_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowPMEmailNotification" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel126" runat="server" LocalizedTag="MAX_PM_RECIPIENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PrivateMessageMaxRecipients" runat="server"></asp:TextBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedTag="HEADER_ALBUM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel127" runat="server" LocalizedTag="ENABLE_ABLBUMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableAlbum" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel128" runat="server" LocalizedTag="MAX_IMAGE_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AlbumImagesSizeMax" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel129" runat="server" LocalizedTag="ALBUMS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AlbumsPerPage" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel130" runat="server" LocalizedTag="IMAGES_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AlbumImagesPerPage" runat="server"></asp:TextBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel31" runat="server" LocalizedTag="HEADER_HOTTOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel197" runat="server" LocalizedTag="POPULAR_VIEWS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PopularTopicViews" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel198" runat="server" LocalizedTag="POPULAR_REPLYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PopularTopicReplys" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel199" runat="server" LocalizedTag="POPULAR_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PopularTopicDays" runat="server"></asp:TextBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedTag="HEADER_SYNDICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel131" runat="server" LocalizedTag="SHOW_RSS_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowRSSLink" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel132" runat="server" LocalizedTag="SHOW_ATOM_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowAtomLink" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="TOPICFEED_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="TopicsFeedItemsCount" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel133" runat="server" LocalizedTag="POSTS_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                              <asp:DropDownList CssClass="custom-select" ID="PostsFeedAccess" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel134" runat="server" LocalizedTag="LASTPOSTS_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                              <asp:DropDownList CssClass="custom-select" ID="PostLatestFeedAccess" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel135" runat="server" LocalizedTag="FORUM_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                              <asp:DropDownList CssClass="custom-select" ID="ForumFeedAccess" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel136" runat="server" LocalizedTag="TOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="TopicsFeedAccess" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel137" runat="server" LocalizedTag="ACTIVETOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                              <asp:DropDownList CssClass="custom-select" ID="ActiveTopicFeedAccess" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel138" runat="server" LocalizedTag="FAVTOPIC_FEEDS_ACCESS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                              <asp:DropDownList CssClass="custom-select" ID="FavoriteTopicFeedAccess" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedTag="HEADER_GEOLOCATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel108" runat="server" LocalizedTag="IP_INFOSERVICE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableIPInfoService" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel109" runat="server" LocalizedTag="IP_INFOSERVICE_XMLURL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="IPLocatorUrlPath" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel195" runat="server" LocalizedTag="IP_INFOSERVICE_DATAMAPPING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="IPLocatorResultsMapping" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel110" runat="server" LocalizedTag="IPINFO_ULRL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="IPInfoPageURL" runat="server"></asp:TextBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel33" runat="server" LocalizedTag="HEADER_REPUTATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel106" runat="server" LocalizedTag="DISPLAY_POINTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="DisplayPoints" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel203" runat="server" LocalizedTag="ENABLE_USERREPUTATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableUserReputation" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel204" runat="server" LocalizedTag="REPUTATION_ALLOWNEGATIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ReputationAllowNegative" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel207" runat="server" LocalizedTag="REPUTATION_MIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ReputationMaxNegative" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel208" runat="server" LocalizedTag="REPUTATION_MAX" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ReputationMaxPositive" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel205" runat="server" LocalizedTag="REPUTATION_MINUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ReputationMinUpVoting" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel206" runat="server" LocalizedTag="REPUTATION_MINDOWN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ReputationMinDownVoting" runat="server"></asp:TextBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedTag="HEADER_CAPTCHA" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel143" runat="server" LocalizedTag="CAPTCHA_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="CaptchaSize" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel144" runat="server" LocalizedTag="RECAPTCHA_PUBLIC_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="RecaptchaPublicKey" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel145" runat="server" LocalizedTag="RECAPTCHA_PRIVATE_KEY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="RecaptchaPrivateKey" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel146" runat="server" LocalizedTag="CAPTCHA_GUEST_POSTING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableCaptchaForGuests" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel147" runat="server" LocalizedTag="ENABLE_CAPTCHA_FORPOST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableCaptchaForPost" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel148" runat="server" LocalizedTag="CAPTCHA_FOR_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="CaptchaTypeRegister" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel35" runat="server" LocalizedTag="HEADER_MESSAGE_NOTIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel214" runat="server" LocalizedTag="NOTIFICATION_DURATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="MessageNotifcationDuration" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel215" runat="server" LocalizedTag="NOTIFICATION_MOBILE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="NotifcationNativeOnMobile" runat="server"></asp:CheckBox>

                    </p>

	</div>
    <div id="View3" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="HEADER_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel152" runat="server" LocalizedTag="ACTIVE_USERTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ActiveListTime" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel153" runat="server" LocalizedTag="SHOW_AVATARS_TOPICLISTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowAvatarsInTopic" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel154" runat="server" LocalizedTag="SHOW_MOVED_TOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowMoved" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel155" runat="server" LocalizedTag="SHOW_MODLIST" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowModeratorList" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel200" runat="server" LocalizedTag="SHOW_MODLIST_ASCOLUMN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowModeratorListAsColumn" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel156" runat="server" LocalizedTag="SHOW_GUESTS_INACTIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowGuestsInDetailedActiveList" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel157" runat="server" LocalizedTag="SHOW_BOTS_INACTIVE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowCrawlersInActiveList" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel158" runat="server" LocalizedTag="SHOW_DEL_MESSAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowDeletedMessages" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel159" runat="server" LocalizedTag="SHOW_DEL_MESSAGES_TOALL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowDeletedMessagesToAll" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="BlankLinksHelpLabel" runat="server" LocalizedTag="SHOW_LINKS_NEWWINDOW" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="BlankLinks" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="ShowLastUnreadPostHelpLabel" runat="server" LocalizedTag="SHOW_UNREAD_LINKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowLastUnreadPost" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel161" runat="server" LocalizedTag="SHOW_NOCOUNT_POSTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="NoCountForumsInActiveDiscussions" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel162" runat="server" LocalizedTag="SHOW_FORUM_STATS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowForumStatistics" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel190" runat="server" LocalizedTag="SHOW_RECENT_USERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowRecentUsers" runat="server"></asp:CheckBox>

                    </p><hr />

                    <h4>
                    <YAF:HelpLabel ID="HelpLabel201" runat="server" LocalizedTag="SHOW_TODAYS_BIRTHDAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                     </h4>
                      <p>
                      <asp:CheckBox CssClass="form-control" ID="ShowTodaysBirthdays" runat="server"></asp:CheckBox>

                      </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel163" runat="server" LocalizedTag="SHOW_ACTIVE_DISCUSSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowActiveDiscussions" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel164" runat="server" LocalizedTag="SHOW_FORUM_JUMP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowForumJump" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel167" runat="server" LocalizedTag="SHOW_GROUPS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowGroups" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel168" runat="server" LocalizedTag="SHOW_GROUPS_INPROFILE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowGroupsProfile" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel169" runat="server" LocalizedTag="SHOW_MEDALS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowMedals" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel170" runat="server" LocalizedTag="SHOW_USERSBROWSING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowBrowsingUsers" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel181" runat="server" LocalizedTag="SHOW_SIMILARTOPICS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowSimilarTopics" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel171" runat="server" LocalizedTag="SHOW_RENDERTIME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowPageGenerationTime" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel172" runat="server" LocalizedTag="SHOW_YAFVERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowYAFVersion" runat="server"></asp:CheckBox>

                    </p><hr />

                       <h4>
                            <YAF:HelpLabel ID="HelpLabel182" runat="server" LocalizedTag="SHOWHELP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ShowHelpTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel183" runat="server" LocalizedTag="SHOWTEAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ShowTeamTo" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel174" runat="server" LocalizedTag="SHOW_JOINDATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="DisplayJoinDate" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel175" runat="server" LocalizedTag="RULES_ONREGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowRulesForRegistration" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel176" runat="server" LocalizedTag="LASTPOST_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ActiveDiscussionsCount" runat="server" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel177" runat="server" LocalizedTag="NOFOLLOW_LINKTAGS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseNoFollowLinks" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel178" runat="server" LocalizedTag="POSTS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="PostsPerPage" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel179" runat="server" LocalizedTag="TOPICS_PER_PAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="TopicsPerPage" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel236" runat="server" LocalizedTag="AMOUNT_OF_SUBFORUMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="SubForumsInForumList" runat="server"></asp:TextBox>

                    </p><hr />


                        <h4>
                            <YAF:HelpLabel ID="HelpLabel240" runat="server" LocalizedTag="SHOW_EDIT_MESSAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowEditedMessage" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel36" runat="server" LocalizedTag="HEADER_SHOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel165" runat="server" LocalizedTag="SHOW_SHOUTBOX" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowShoutbox" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel221" runat="server" LocalizedTag="VIEWSHOUTBOX_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ShoutboxViewPermissions" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel222" runat="server" LocalizedTag="SHOUTBOX_DEFAULTSTATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ShoutboxDefaultState" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel219" runat="server" LocalizedTag="SHOUTBOX_COUNT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ShoutboxShowMessageCount" runat="server"></asp:TextBox>

                    </p>

	</div>
    <div id="View4" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="HEADER_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel26" runat="server" LocalizedTag="POST_AD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox Height="80px" CssClass="form-control" TextMode="MultiLine" runat="server" ID="AdPost" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel27" runat="server" LocalizedTag="SHOWAD_LOGINUSERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox runat="server" CssClass="form-control" ID="ShowAdsToSignedInUsers" />

                    </p>

	</div>
    <div id="View5" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="HEADER_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel88" runat="server" LocalizedTag="FORUM_EDITOR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ForumEditor" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel160" runat="server" LocalizedTag="ALLOW_USERTEXTEDITOR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox runat="server" CssClass="form-control" ID="AllowUsersTextEditor" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel87" runat="server" LocalizedTag="ACCEPT_HTML" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" Height="80px" ID="AcceptedHTML" runat="server" TextMode="MultiLine"></asp:TextBox>

                    </p>

	</div>
    <div id="View6" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="HEADER_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel84" runat="server" LocalizedTag="USER_CHANGE_THEME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowUserTheme" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel83" runat="server" LocalizedTag="USER_CHANGE_LANGUAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowUserLanguage" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel82" runat="server" LocalizedTag="ALLOW_SIGNATURE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowSignatures" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel81" runat="server" LocalizedTag="ALLOW_SENDMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowEmailSending" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel80" runat="server" LocalizedTag="ALLOW_EMAIL_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowEmailChange" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel79" runat="server" LocalizedTag="ALLOW_PASS_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowPasswordChange" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel78" runat="server" LocalizedTag="ALLOW_MOD_VIEWIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowModeratorsViewIPs" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel77" runat="server" LocalizedTag="ALLOW_NOTIFICATION_ONALL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowNotificationAllPostsAllTopics" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel76" runat="server" LocalizedTag="REPORT_POST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ReportPostPermissions" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                         <h4>
                             <YAF:HelpLabel ID="HelpLabel89" runat="server" LocalizedTag="ALLOW_TOPICS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="AllowCreateTopicsSameName" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                             <YAF:HelpLabel ID="HelpLabel90" runat="server" LocalizedTag="ALLOW_FORUMS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                       <p>
                            <asp:CheckBox CssClass="form-control" ID="AllowForumsWithSameName" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel75" runat="server" LocalizedTag="VIEWPROFILE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ProfileViewPermissions" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel74" runat="server" LocalizedTag="VIEWMEMBERLIST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="MembersListViewPermissions" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel73" runat="server" LocalizedTag="VIEWACTIVE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="ActiveUsersViewPermissions" runat="server">
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel72" runat="server" LocalizedTag="MAX_WORD_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="MaxWordLength" MaxLength="2" runat="server"></asp:TextBox>

                    </p><hr />
                    <asp:PlaceHolder runat="server" ID="SSLSettings">

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel71" runat="server" LocalizedTag="SSL_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseSSLToLogIn" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                           <YAF:HelpLabel ID="HelpLabel70" runat="server" LocalizedTag="SSL_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UseSSLToRegister" runat="server"></asp:CheckBox>

                    </p>
                    </asp:PlaceHolder>

	</div>
    <div id="View7" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="HEADER_TEMPLATES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel57" runat="server" LocalizedTag="USERBOX_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBox" TextMode="MultiLine" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel58" runat="server" LocalizedTag="AVATAR_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxAvatar" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel59" runat="server" LocalizedTag="MEDALS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxMedals" runat="server"></asp:TextBox>
                                            </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel60" runat="server" LocalizedTag="RANKIMAGE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxRankImage" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel61" runat="server" LocalizedTag="RANK_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxRank" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel62" runat="server" LocalizedTag="GROUPS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxGroups" runat="server"></asp:TextBox>
                        </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel63" runat="server" LocalizedTag="JOINDATE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxJoinDate" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel64" runat="server" LocalizedTag="POSTS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxPosts" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel65" runat="server" LocalizedTag="REPUTATION_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxReputation" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel196" runat="server" LocalizedTag="COUNTRYIMAGE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxCountryImage" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel66" runat="server" LocalizedTag="LOCATION_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxLocation" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel67" runat="server" LocalizedTag="GENDER_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxGender" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel68" runat="server" LocalizedTag="THANKS_FROM_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxThanksFrom" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel69" runat="server" LocalizedTag="THANKS_TO_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="UserBoxThanksTo" runat="server"></asp:TextBox>

                    </p>

	</div>
    <asp:PlaceHolder runat="server" ID="AvatarsTab">
    <div id="View8" class="tab-pane" role="tabpanel">
		    <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="HEADER_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />
                    <asp:PlaceHolder runat="server" ID="AvatarSettingsHolder">

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel189" runat="server" LocalizedTag="AVATAR_GALLERY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AvatarGallery" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel50" runat="server" LocalizedTag="REMOTE_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AvatarRemote" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel51" runat="server" LocalizedTag="AVATAR_UPLOAD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AvatarUpload" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel52" runat="server" LocalizedTag="ALLOW_GRAVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="AvatarGravatar" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel53" runat="server" LocalizedTag="GRAVATAR_RATING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="GravatarRating" runat="server">
                                <asp:ListItem Value="G"></asp:ListItem>
                                <asp:ListItem Value="PG"></asp:ListItem>
                                <asp:ListItem Value="R"></asp:ListItem>
                                <asp:ListItem Value="X"></asp:ListItem>
                            </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel56" runat="server" LocalizedTag="AVATAR_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AvatarSize" runat="server"></asp:TextBox>
                                            </p><hr />
                    </asp:PlaceHolder>

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel54" runat="server" LocalizedTag="AVATAR_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AvatarWidth" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel55" runat="server" LocalizedTag="AVATAR_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="AvatarHeight" runat="server"></asp:TextBox>

                    </p><hr />

	</div>
    </asp:PlaceHolder>
    <div id="View9" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="HEADER_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel41" runat="server" LocalizedTag="STATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="ForumStatisticsCacheTimeout" />
                            <asp:Button Type="Primary" ID="ForumStatisticsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="ForumStatisticsCacheResetClick" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel42" runat="server" LocalizedTag="USRSTATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="BoardUserStatsCacheTimeout" />
                            <asp:Button Type="Primary" ID="BoardUserStatsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="BoardUserStatsCacheResetClick" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel43" runat="server" LocalizedTag="DISCUSSIONS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="ActiveDiscussionsCacheTimeout" />
                            <asp:Button Type="Primary" ID="ActiveDiscussionsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="ActiveDiscussionsCacheResetClick" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel44" runat="server" LocalizedTag="CAT_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="BoardCategoriesCacheTimeout" />
                            <asp:Button Type="Primary" ID="BoardCategoriesCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="BoardCategoriesCacheResetClick" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel45" runat="server" LocalizedTag="MOD_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="BoardModeratorsCacheTimeout" />
                            <asp:Button Type="Primary" ID="BoardModeratorsCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="BoardModeratorsCacheResetClick" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel46" runat="server" LocalizedTag="REPLACE_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="ReplaceRulesCacheTimeout" />
                            <asp:Button Type="Primary" ID="ReplaceRulesCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>'  runat="server" OnClick="ReplaceRulesCacheResetClick" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel47" runat="server" LocalizedTag="SEO_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="FirstPostCacheTimeout" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel48" runat="server" LocalizedTag="ONLINE_STATUS_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="OnlineStatusCacheTimeout" />

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel49" runat="server" LocalizedTag="LAZY_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" runat="server" ID="ActiveUserLazyDataCacheTimeout" />

                    </p>
                    <p>
                        <asp:Button Type="Primary" ID="ActiveUserLazyDataCacheReset" Text='<%# this.GetText("ADMIN_COMMON", "CLEAR") %>' runat="server" OnClick="UserLazyDataCacheResetClick" />
                    </p>

                        <span class="text-lg-center">
                           <asp:Button  Type="Primary" runat="server" ID="ResetCacheAll" Text='<%# this.GetText("ADMIN_HOSTSETTINGS", "CLEAR_CACHE") %>'  OnClick="ResetCacheAllClick" />
                        </span>

	</div>
    <div id="View10" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="HEADER_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel28" runat="server" LocalizedTag="MAX_SEARCH_RESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="ReturnSearchMax" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel30" runat="server" LocalizedTag="SEARCH_MINLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="SearchStringMinLength" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel33" runat="server" LocalizedTag="SEARCH_PERMISS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:DropDownList CssClass="custom-select" ID="SearchPermissions" runat="server">
                               </asp:DropDownList>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel39" runat="server" LocalizedTag="QUICK_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="ShowQuickSearch" runat="server"></asp:CheckBox>

                    </p><hr />

        </div>
        <div id="View11" class="tab-pane" role="tabpanel">


                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedTag="HEADER_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                    <hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel140" runat="server" LocalizedTag="EVENTLOG_MAX_MESSAGES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="EventLogMaxMessages" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel141" runat="server" LocalizedTag="EVENTLOG_MAX_DAYS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="EventLogMaxDays" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel149" runat="server" LocalizedTag="MESSAGE_CHANGE_HISTORY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="MessageHistoryDaysToLog" runat="server"></asp:TextBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel150" runat="server" LocalizedTag="ENABLE_LOCATIONPATH_ERRORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="EnableActiveLocationErrorsLog" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel151" runat="server" LocalizedTag="UNHANDLED_USERAGENT_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="UserAgentBadLog" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h2>
                            <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedTag="HEADER_LOGSCOPE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h2>
                        <hr />

                        <h4>
                            <YAF:HelpLabel ID="LogErrorLabel" runat="server" LocalizedTag="LOG_ERROR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogError" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel142" runat="server" LocalizedTag="LOG_WARNING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogWarning" runat="server"></asp:CheckBox>

                    </p><hr />



                        <h4>
                            <YAF:HelpLabel ID="HelpLabel180" runat="server" LocalizedTag="LOG_INFORMATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogInformation" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel210" runat="server" LocalizedTag="LOG_VIEWSTATEERROR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogViewStateError" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel211" runat="server" LocalizedTag="LOG_BANNEDIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogBannedIP" runat="server"></asp:CheckBox>
                                            </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel212" runat="server" LocalizedTag="LOG_USERDELETED" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogUserDeleted" runat="server"></asp:CheckBox>

                    </p><hr />

                        <h4>
                            <YAF:HelpLabel ID="HelpLabel213" runat="server" LocalizedTag="LOG_SUSPENDEDANDCONTRA" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </h4>
                        <p>
                            <asp:CheckBox CssClass="form-control" ID="LogUserSuspendedUnsuspended" runat="server"></asp:CheckBox>

                    </p>

        </div>
          </div>
    </asp:Panel>
                </div>
                <div class="card-footer text-lg-center">
                <YAF:ThemeButton ID="Save" runat="server"  Type="Primary" OnClick="SaveClick"
                    Icon="save" TextLocalizedTag="SAVE_SETTINGS" TextLocalizedPage="ADMIN_HOSTSETTINGS">
                </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<asp:HiddenField runat="server" ID="hidLastTab" Value="View1" />
<YAF:SmartScroller ID="SmartScroller1" runat="server" />