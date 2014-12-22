<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boardsettings"CodeBehind="boardsettings.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
        </tr>
        <tr>
            <td class="header2" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BOARD_SETUP"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
        </tr>
        <tr>
            <td class="postheader" style="width: 50%">
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="BOARD_NAME" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post" style="width: 50%">
                <asp:TextBox ID="Name" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="FORUM_EMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
            </td>
            <td class="post">
                <asp:TextBox ID="ForumEmail" runat="server" Width="400" type="email"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="FORUM_BASEURLMASK" LocalizedPage="ADMIN_HOSTSETTINGS" />
            </td>
            <td class="post">
                <asp:TextBox ID="ForumBaseUrlMask" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>
        <asp:PlaceHolder ID="CopyrightHolder" runat="server">
        <tr>
            <td class="postheader" style="width: 50%">
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="COPYRIGHT_REMOVAL_KEY" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post" style="width: 50%">
                <asp:TextBox ID="CopyrightRemovalKey" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>   
        </asp:PlaceHolder>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="BOARD_THREADED"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:CheckBox ID="AllowThreaded" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="BOARD_THEME" LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="Theme" runat="server" Width="400" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel6" runat="server" LocalizedTag="BOARD_MOBILE_THEME"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="MobileTheme" runat="server" Width="400" CssClass="standardSelectMenu">
                    <asp:ListItem Text="[None Selected]" Value=""></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel7" runat="server" LocalizedTag="BOARD_THEME_LOGO"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:CheckBox ID="AllowThemedLogo" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel8" runat="server" LocalizedTag="BOARD_JQ_THEME"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="JqueryUITheme" runat="server" Width="400" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel10" runat="server" LocalizedTag="BOARD_CULTURE"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="Culture" runat="server" Width="400" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel11" runat="server" LocalizedTag="BOARD_TOPIC_DEFAULT"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="ShowTopic" runat="server" Width="400" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel12" runat="server" LocalizedTag="BOARD_FILE_EXTENSIONS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="FileExtensionAllow" runat="server" Width="400" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="PollGroupList" runat="server" visible="false">
            <td class="postheader" style="width: 20%">
                <YAF:HelpLabel ID="PollGroupListLabel" runat="server" LocalizedTag="pollgroup_list" />
            </td>
            <td class="post" style="width: 80%">
                <asp:DropDownList ID="PollGroupListDropDown" runat="server" CssClass="standardSelectMenu" Width="400" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel13" runat="server" LocalizedTag="BOARD_EMAIL_ONREGISTER"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:TextBox ID="NotificationOnUserRegisterEmailList" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel14" runat="server" LocalizedTag="BOARD_EMAIL_MODS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:CheckBox ID="EmailModeratorsOnModeratedPost" runat="server"></asp:CheckBox>
            </td>
        </tr>
         <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="BOARD_EMAIL_REPORTMODS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:CheckBox ID="EmailModeratorsOnReportedPost" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel15" runat="server" LocalizedTag="BOARD_ALLOW_DIGEST"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:CheckBox ID="AllowDigestEmail" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabelDigest1" runat="server" LocalizedTag="BOARD_DIGEST_HOURS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:TextBox ID="DigestSendEveryXHours" runat="server" Width="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel16" runat="server" LocalizedTag="BOARD_DIGEST_NEWUSERS"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:CheckBox ID="DefaultSendDigestEmail" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel17" runat="server" LocalizedTag="BOARD_DEFAULT_NOTIFICATION"
                    LocalizedPage="ADMIN_BOARDSETTINGS" />
            </td>
            <td class="post">
                <asp:DropDownList ID="DefaultNotificationSetting" runat="server" Width="400" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postfooter" align="center" colspan="2">
                <asp:Button ID="Save" CssClass="pbutton" runat="server" Text="Save" OnClick="Save_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
