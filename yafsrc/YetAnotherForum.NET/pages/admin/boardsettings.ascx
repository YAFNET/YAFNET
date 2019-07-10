﻿<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boardsettings"CodeBehind="boardsettings.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BOARDSETTINGS" /></h1>
    </div>
</div>
<div class="row">
                <div class="col-xl-12">
                    <div class="card mb-3">
                        <div class="card-header">
                             <i class="fa fa-cogs fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BOARD_SETUP"
                                 LocalizedPage="ADMIN_BOARDSETTINGS" />
                        </div>
                        <div class="card-body">
                                 
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="BOARD_NAME" LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:TextBox ID="Name" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
             
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="FORUM_EMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
             
            <p>
                <asp:TextBox ID="ForumEmail" runat="server" TextMode="Email"  CssClass="form-control"></asp:TextBox>
            </p><hr />
             
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="FORUM_BASEURLMASK" LocalizedPage="ADMIN_HOSTSETTINGS" />
             
            <p>
                <asp:TextBox ID="ForumBaseUrlMask" runat="server" TextMode="Url" CssClass="form-control"></asp:TextBox>
            </p><hr />
           <asp:PlaceHolder ID="CopyrightHolder" runat="server">
             
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="COPYRIGHT_REMOVAL_KEY" LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="input-group">
                <asp:TextBox ID="CopyrightRemovalKey" runat="server" CssClass="form-control"></asp:TextBox>
                <div class="input-group-append">
                    <YAF:ThemeButton runat="server" ID="GetRemovalKey" 
                                     NavigateUrl="http://yetanotherforum.net/purchase.aspx"
                                     Type="Info"
                                     Icon="key"
                                     TextLocalizedTag="COPYRIGHT_REMOVAL_KEY_DOWN">
                    </YAF:ThemeButton>
                </div>
            </div><hr />

        </asp:PlaceHolder>

             
                <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="BOARD_THREADED"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="AllowThreaded" runat="server" Text="&nbsp;"></asp:CheckBox>
            </div><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="BOARD_THEME" LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:DropDownList ID="Theme" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel7" runat="server" LocalizedTag="BOARD_THEME_LOGO"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="AllowThemedLogo" runat="server" Text="&nbsp;"></asp:CheckBox>
            </div><hr />

             
                <YAF:HelpLabel ID="LocalizedLabel10" runat="server" LocalizedTag="BOARD_CULTURE"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:DropDownList ID="Culture" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel11" runat="server" LocalizedTag="BOARD_TOPIC_DEFAULT"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:DropDownList ID="ShowTopic" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel12" runat="server" LocalizedTag="BOARD_FILE_EXTENSIONS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
                            
                <div class="custom-control custom-switch">
                    <asp:CheckBox ID="FileExtensionAllow" runat="server" Text="&nbsp;"></asp:CheckBox>
                </div>
            <hr />

        <asp:PlaceHolder ID="PollGroupList" runat="server" Visible="false">
             
                <YAF:HelpLabel ID="PollGroupListLabel" runat="server" LocalizedTag="pollgroup_list" />
             
            <p>
                <asp:DropDownList ID="PollGroupListDropDown" runat="server" CssClass="custom-select" placeholder='<%# this.GetText("pollgroup_list") %>' />
            </p><hr />
        </asp:PlaceHolder>

             
                <YAF:HelpLabel ID="LocalizedLabel13" runat="server" LocalizedTag="BOARD_EMAIL_ONREGISTER"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel14" runat="server" LocalizedTag="BOARD_EMAIL_MODS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="EmailModeratorsOnModeratedPost" runat="server" Text="&nbsp;"></asp:CheckBox>
            </div><hr />


             
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="BOARD_EMAIL_REPORTMODS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="EmailModeratorsOnReportedPost" runat="server" Text="&nbsp;"></asp:CheckBox>
            </div><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel15" runat="server" LocalizedTag="BOARD_ALLOW_DIGEST"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="AllowDigestEmail" runat="server" Text="&nbsp;"></asp:CheckBox>
            </div><hr />
             
                <YAF:HelpLabel ID="HelpLabelDigest1" runat="server" LocalizedTag="BOARD_DIGEST_HOURS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:TextBox ID="DigestSendEveryXHours" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
            </p><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel16" runat="server" LocalizedTag="BOARD_DIGEST_NEWUSERS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <div class="custom-control custom-switch">
                <asp:CheckBox ID="DefaultSendDigestEmail" runat="server" Text="&nbsp;"></asp:CheckBox>
            </div><hr />


             
                <YAF:HelpLabel ID="LocalizedLabel17" runat="server" LocalizedTag="BOARD_DEFAULT_NOTIFICATION"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
             
            <p>
                <asp:DropDownList ID="DefaultNotificationSetting" runat="server" CssClass="custom-select">
                </asp:DropDownList>
            </p><hr />
             
                <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="CdvVersion"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
                            <div class="input-group">
                                <asp:TextBox ID="CdvVersion" runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>
                                <div class="input-group-append">
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


