<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boardsettings"CodeBehind="boardsettings.ascx.cs" %>

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
                 <i class="fa fa-cogs fa-fw text-secondary"></i>&nbsp;
                 <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BOARD_SETUP"
                                LocalizedPage="ADMIN_BOARDSETTINGS"/>
        </div>
        <div class="card-body">
            <div class="form-row">
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="HelpLabel1" runat="server"
                                   AssociatedControlID="Name"
                                   LocalizedTag="BOARD_NAME" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:TextBox ID="Name" runat="server" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="HelpLabel4" runat="server"
                                   AssociatedControlID="ForumEmail"
                                   LocalizedTag="FORUM_EMAIL" LocalizedPage="ADMIN_HOSTSETTINGS"/>
                    <asp:TextBox ID="ForumEmail" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <YAF:HelpLabel ID="HelpLabel14" runat="server"
                               AssociatedControlID="BoardLogo"
                               LocalizedTag="BOARD_LOGO" LocalizedPage="ADMIN_BOARDSETTINGS" />
                <YAF:ImageListBox ID="BoardLogo" runat="server" CssClass="select2-image-select" />
            </div>
        <div class="form-group">
                <YAF:HelpLabel ID="HelpLabel5" runat="server"
                               AssociatedControlID="ForumBaseUrlMask"
                               LocalizedTag="FORUM_BASEURLMASK" LocalizedPage="ADMIN_HOSTSETTINGS"/>

                <asp:TextBox ID="ForumBaseUrlMask" runat="server" TextMode="Url" CssClass="form-control"></asp:TextBox>
            </div>
            <asp:PlaceHolder ID="CopyrightHolder" runat="server">
                <div class="form-group">
                    <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                   AssociatedControlID="CopyrightHolder"
                                   LocalizedTag="COPYRIGHT_REMOVAL_KEY" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="input-group">
                        <asp:TextBox ID="CopyrightRemovalKey" runat="server" CssClass="form-control"></asp:TextBox>
                        <div class="input-group-append">
                            <YAF:ThemeButton runat="server" ID="GetRemovalKey"
                                             NavigateUrl="https://yetanotherforum.net/purchase.aspx"
                                             Type="Info"
                                             Icon="key"
                                             TextLocalizedTag="COPYRIGHT_REMOVAL_KEY_DOWN">
                            </YAF:ThemeButton>
                        </div>
                    </div>
                </div>

            </asp:PlaceHolder>
            <div class="form-group">
                <YAF:HelpLabel ID="LocalizedLabel5" runat="server"
                               AssociatedControlID="Theme"
                               LocalizedTag="BOARD_THEME" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:DropDownList ID="Theme" runat="server" CssClass="select2-select"></asp:DropDownList>
            </div>
            <div class="form-row">
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel11" runat="server"
                                   AssociatedControlID="ShowTopic"
                                   LocalizedTag="BOARD_TOPIC_DEFAULT"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:DropDownList ID="ShowTopic" runat="server" CssClass="select2-select">
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel10" runat="server"
                                   AssociatedControlID="Culture"
                                   LocalizedTag="BOARD_CULTURE" LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:DropDownList ID="Culture" runat="server" CssClass="select2-select">
                    </asp:DropDownList>
                </div>
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel12" runat="server"
                                   AssociatedControlID="FileExtensionAllow"
                                   LocalizedTag="BOARD_FILE_EXTENSIONS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="custom-control custom-switch">
                        <asp:CheckBox ID="FileExtensionAllow" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
            </div>
            <asp:PlaceHolder ID="PollGroupList" runat="server" Visible="false">
                <div class="form-group">
                    <YAF:HelpLabel ID="PollGroupListLabel" runat="server"
                                   AssociatedControlID="PollGroupList"
                                   LocalizedTag="POLLGROUP_LIST_BOARD"/>
                    <asp:DropDownList ID="PollGroupListDropDown" runat="server" 
                                      CssClass="select2-select" 
                                      placeholder='<%# this.GetText("pollgroup_list") %>'/>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <YAF:HelpLabel ID="LocalizedLabel17" runat="server"
                               AssociatedControlID="DefaultNotificationSetting"
                               LocalizedTag="BOARD_DEFAULT_NOTIFICATION"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:DropDownList ID="DefaultNotificationSetting" runat="server" 
                                  CssClass="select2-select" 
                                  placeholder='<%# this.GetText("BOARD_DEFAULT_NOTIFICATION") %>'>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <YAF:HelpLabel ID="LocalizedLabel13" runat="server"
                               AssociatedControlID="NotificationOnUserRegisterEmailList"
                               LocalizedTag="BOARD_EMAIL_ONREGISTER"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="form-row">
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel14" runat="server"
                                   AssociatedControlID="EmailModeratorsOnModeratedPost"
                                   LocalizedTag="BOARD_EMAIL_MODS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="custom-control custom-switch">
                        <asp:CheckBox ID="EmailModeratorsOnModeratedPost" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="HelpLabel3" runat="server"
                                   AssociatedControlID="EmailModeratorsOnReportedPost"
                                   LocalizedTag="BOARD_EMAIL_REPORTMODS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="custom-control custom-switch">
                        <asp:CheckBox ID="EmailModeratorsOnReportedPost" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel15" runat="server"
                                   AssociatedControlID="AllowDigestEmail"
                                   LocalizedTag="BOARD_ALLOW_DIGEST"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="custom-control custom-switch">
                        <asp:CheckBox ID="AllowDigestEmail" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
                <div class="form-group col-md-6">
                    <YAF:HelpLabel ID="LocalizedLabel16" runat="server"
                                   AssociatedControlID="DefaultSendDigestEmail"
                                   LocalizedTag="BOARD_DIGEST_NEWUSERS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <div class="custom-control custom-switch">
                        <asp:CheckBox ID="DefaultSendDigestEmail" runat="server" Text="&nbsp;"></asp:CheckBox>
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md-4">
                    <YAF:HelpLabel ID="HelpLabelDigest1" runat="server"
                                   AssociatedControlID="DigestSendEveryXHours"
                                   LocalizedTag="BOARD_DIGEST_HOURS"
                                   LocalizedPage="ADMIN_BOARDSETTINGS"/>
                    <asp:TextBox ID="DigestSendEveryXHours" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="form-group">
                <YAF:HelpLabel ID="HelpLabel6" runat="server"
                               AssociatedControlID="CdvVersion"
                               LocalizedTag="CdvVersion"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
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
            <div class="mb-3">
                <YAF:HelpLabel ID="HelpLabel7" runat="server"
                               AssociatedControlID="ForumDefaultAccessMask"
                               LocalizedTag="FORUM_DEFAULT_ACCESSMASK"
                               LocalizedPage="ADMIN_BOARDSETTINGS"/>
                <asp:DropDownList ID="ForumDefaultAccessMask" runat="server" 
                                  CssClass="select2-select" 
                                  placeholder='<%# this.GetText("FORUM_DEFAULT_ACCESSMASK") %>'>
                </asp:DropDownList>
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

<div class="row">
    <div class="col">
        <div class="card">
            <div class="card-header">
                <i class="fa fa-bullhorn fa-fw text-secondary mr-1"></i>
                <YAF:LocalizedLabel runat="server" LocalizedTag="ANNOUNCEMENT_TITLE"></YAF:LocalizedLabel>
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" ID="CurrentAnnouncement" Visible="False">
                    <YAF:Alert runat="server" ID="Current" Type="info">
                        <asp:Label runat="server" ID="CurrentMessage"></asp:Label>
                        <YAF:ThemeButton runat="server"
                                         Type="Danger"
                                         Icon="trash"
                                         OnClick="DeleteClick"></YAF:ThemeButton>
                    </YAF:Alert>
                </asp:PlaceHolder>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="BoardAnnouncement">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="ANNOUNCEMENT_MESSAGE" />
                    </asp:Label>
                    <asp:TextBox ID="BoardAnnouncement" runat="server" 
                                 TextMode="MultiLine" 
                                 CssClass="form-control" 
                                 Rows="3"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="BoardAnnouncementUntil">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedTag="ANNOUNCEMENT_UNTIL" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="BoardAnnouncementUntil" CssClass="form-control" TextMode="Number" />
                    <div class="custom-control custom-radio custom-control-inline mt-1">
                        <asp:RadioButtonList
                            runat="server" ID="BoardAnnouncementUntilUnit" 
                            RepeatLayout="UnorderedList"
                            CssClass="list-unstyled" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="BoardAnnouncementType">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                            LocalizedTag="ANNOUNCEMENT_TYPE" />
                    </asp:Label>
                    <asp:DropDownList runat="server" ID="BoardAnnouncementType" 
                                       CssClass="select2-select">
                        <asp:ListItem Text="primary" Value="primary"></asp:ListItem>
                        <asp:ListItem Text="secondary" Value="secondary"></asp:ListItem>
                        <asp:ListItem Text="success" Value="success"></asp:ListItem>
                        <asp:ListItem Text="danger" Value="danger"></asp:ListItem>
                        <asp:ListItem Text="warning" Value="warning"></asp:ListItem>
                        <asp:ListItem Text="info" Value="info" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="light" Value="light"></asp:ListItem>
                        <asp:ListItem Text="dark" Value="dark"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="SaveAnnouncement" runat="server" 
                                 Type="Primary" 
                                 OnClick="SaveAnnouncementClick"
                                 Icon="save" 
                                 TextLocalizedTag="SAVE">
                </YAF:ThemeButton>
            </div>
        </div>
    </div>
</div>