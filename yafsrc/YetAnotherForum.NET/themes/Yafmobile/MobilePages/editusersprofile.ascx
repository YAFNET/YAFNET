<%@ Control Language="C#" AutoEventWireup="true" CodeFile="editusersprofile.ascx.cs"
    Inherits="YAF.Controls.EditUsersProfile" %>
<table width="100%" class="content" cellspacing="1" cellpadding="4">
    <tr>
        <td class="header1">
            <YAF:LocalizedLabel runat="server" LocalizedPage="CP_EDITPROFILE" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="aboutyou" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="realname2" />
            <asp:TextBox ID="Realname" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="occupation" />
            <asp:TextBox ID="Occupation" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="interests" />
            <asp:TextBox ID="Interests" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="gender" />
            <asp:DropDownList ID="Gender" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="location" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="where" />
            <asp:TextBox ID="Location" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="homepage" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="homepage2" />
            <asp:TextBox runat="server" ID="HomePage" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="weblog2" />
            <asp:TextBox runat="server" ID="Weblog" CssClass="edit" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="MetaWeblogAPI" Visible="true">
        <tr>
            <td colspan="2" class="header2">
                <b>
                    <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedPage="CP_EDITPROFILE"
                        LocalizedTag="METAWEBLOG_TITLE" />
                </b>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_URL" />
                    <asp:TextBox runat="server" ID="WeblogUrl" CssClass="edit" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_ID" />
                <br />
                <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_ID_INSTRUCTIONS" />
                    <asp:TextBox runat="server" ID="WeblogID" CssClass="edit" />
            </td>

        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_USERNAME" />
                    <asp:TextBox runat="server" ID="WeblogUsername" CssClass="edit" />
            </td>

        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="messenger" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="msn" />
            <asp:TextBox runat="server" ID="MSN" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="yim" />
            <asp:TextBox runat="server" ID="YIM" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="aim" />
            <asp:TextBox runat="server" ID="AIM" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="icq" />
            <asp:TextBox runat="server" ID="ICQ" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SKYPE" />
            <asp:TextBox runat="server" ID="Skype" CssClass="edit" />
        </td>
    </tr>    
    <tr>
        <td class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="timezone" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="timezone2" />
            <asp:DropDownList runat="server" ID="TimeZones" DataTextField="Name" DataValueField="Value" Width="100%" />
        </td>
    </tr>
    <tr runat="server" id="ForumSettingsRows">
        <td class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="FORUM_SETTINGS" />
            </b>
        </td>
    </tr>
    <tr runat="server" id="UserThemeRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_THEME" />
            <asp:DropDownList runat="server" ID="Theme" />
        </td>
    </tr>
    <tr runat="server" id="OverrideForumThemeRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="OVERRIDE_DEFAULT_THEMES" />
            <asp:CheckBox ID="OverrideDefaultThemes" runat="server" />
        </td>
    </tr>
    <tr runat="server" id="UserLanguageRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_LANGUAGE" />
            <asp:DropDownList runat="server" ID="Language" />
        </td>
    </tr>
    <tr runat="server" id="PMNotificationRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="PM_EMAIL_NOTIFICATION" />
            <asp:CheckBox ID="PMNotificationEnabled" runat="server" />
        </td>
    </tr>
     <tr runat="server" id="WatchTopicsRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel31" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="AUTOWATCH_TOPICS_NOTIFICATION" />
            <asp:CheckBox ID="AutoWatchTopicsEnabled" runat="server" />
        </td>
    </tr>      
    <asp:PlaceHolder runat="server" ID="LoginInfo" Visible="false">
        <tr>
            <td colspan="2" class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="change_email" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="email" />
                <asp:TextBox ID="Email" CssClass="edit" runat="server" OnTextChanged="Email_TextChanged" />
                </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="footer1" align="center">
            <asp:Button ID="UpdateProfile" CssClass="pbutton" runat="server" OnClick="UpdateProfile_Click" />
            |
            <asp:Button ID="Cancel" CssClass="pbutton" runat="server" OnClick="Cancel_Click" />
        </td>
    </tr>
</table>
