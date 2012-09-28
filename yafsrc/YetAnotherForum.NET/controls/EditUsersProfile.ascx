<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersProfile" Codebehind="EditUsersProfile.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<table width="100%" class="content EditUserProfileTable" cellspacing="1" cellpadding="4">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel runat="server"  LocalizedPage="CP_EDITPROFILE" LocalizedTag="TITLE" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="aboutyou" />
            </b>
        </td>
    </tr>
    <asp:PlaceHolder ID="DisplayNamePlaceholder" runat="server" Visible="false">
        <tr>
            <td class="postheader" style="width:50%">
                <YAF:LocalizedLabel ID="LocalizedLabel34" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="DISPLAYNAME" />
            </td>
            <td class="post">
                <asp:TextBox ID="DisplayName" runat="server" CssClass="edit" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="postheader" style="width:50%">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="REALNAME2" />
        </td>
        <td class="post">
            <asp:TextBox ID="Realname" runat="server" CssClass="edit" />
        </td>
     </tr>
     <tr id="HideTr" visible="<%# this.Get<YafBoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin %>" runat="server">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel35" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="HIDEME" />
        </td>
        <td class="post">
            <asp:CheckBox ID="HideMe" runat="server" Checked="false" />
        </td>
    </tr>    
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="BirthdayLabel" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="BIRTHDAY" />
        </td>
        <td class="post">
            <asp:TextBox ID="Birthday" runat="server" CssClass="edit"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="OCCUPATION" />
        </td>
        <td class="post">
            <asp:TextBox ID="Occupation" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="INTERESTS" />
        </td>
        <td class="post">
            <asp:TextBox ID="Interests" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="GENDER" />
        </td>
        <td class="post">
            <asp:DropDownList ID="Gender" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="LOCATION" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel40" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="COUNTRY" />
        </td>
        <td class="post">
            <YAF:CountryListBox ID="Country" AutoPostBack="true" OnTextChanged="LookForNewRegions" runat="server" CssClass="edit" />
        </td>
    </tr>
     <tr id="RegionTr" visible="false" runat="server">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel41" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="REGION" />
        </td>
        <td class="post">
            <asp:DropDownList ID="Region" runat="server" CssClass="edit" />            
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel42" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="CITY" />
        </td>
        <td class="post">
            <asp:TextBox ID="City" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="where" />
        </td>
        <td class="post">
            <asp:TextBox ID="Location" runat="server" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="header2">
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
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="HomePage" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="weblog2" />
        </td>
        <td class="post">
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
            </td>
            <td class="post">
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
            </td>
            <td class="post">
                <asp:TextBox runat="server" ID="WeblogID" CssClass="edit" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_USERNAME" />
            </td>
            <td class="post">
                <asp:TextBox runat="server" ID="WeblogUsername" CssClass="edit" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td colspan="2" class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="messenger" />
            </b>
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="MSN" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="MSN" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="YIM" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="YIM" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="AIM" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="AIM" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="ICQ" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="ICQ" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel31" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="Facebook" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Facebook" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel33" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="Twitter" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Twitter" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel37" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="TWITTER_ID" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="TwitterId" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel32" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="xmpp" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Xmpp" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SKYPE" />
        </td>
        <td class="post">
            <asp:TextBox runat="server" ID="Skype" CssClass="edit" />
        </td>
    </tr>
    <tr>
        <td colspan="2" class="header2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="TIMEZONE" />
            </b>
        </td>
    </tr>
    <tr>
		<td class="postheader">
			<YAF:LocalizedLabel ID="DSTLocalizedLabel" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="DST" />
		</td>
		<td class="post">
			<asp:CheckBox runat="server" ID="DSTUser" />
		</td>
	</tr>
    <tr>
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="TIMEZONE2" />
        </td>
        <td class="post">
            <asp:DropDownList runat="server" ID="TimeZones" DataTextField="Name" DataValueField="Value" CssClass="edit" />
        </td>
    </tr>
    <tr runat="server" id="ForumSettingsRows">
        <td colspan="2" class="header2">
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
        </td>
        <td class="post">
            <asp:DropDownList runat="server" ID="Theme" CssClass="edit" />
        </td>
    </tr>
    <tr runat="server" id="TrTextEditors">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_TEXTEDITOR" />
        </td> 
        <td class="post">
         <asp:DropDownList ID="ForumEditor" CssClass="edit" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
        </td>
    </tr>
    <tr runat="server" id="UseMobileThemeRow" visible="false">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="USE_MOBILE_THEME" />
        </td>
        <td class="post">
            <asp:CheckBox ID="UseMobileTheme" runat="server" />
        </td>
    </tr>
    <tr runat="server" id="UserLanguageRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_LANGUAGE" />
        </td>
        <td class="post">
             <asp:DropDownList runat="server" ID="Culture" CssClass="edit" />
        </td>
    </tr>
    <tr runat="server" id="UserLoginRow">
        <td class="postheader">
            <YAF:LocalizedLabel ID="LocalizedLabel36" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="USE_SINGLESIGNON" />
        </td>
        <td class="post">
             <asp:CheckBox id="SingleSignOn" runat="server" />
        </td>
    </tr>
    <asp:PlaceHolder runat="server" ID="LoginInfo" Visible="false">
        <tr>
            <td colspan="2" class="header2">
                <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="CHANGE_EMAIL" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="EMAIL" />
            </td>
            <td class="post">
                <asp:TextBox ID="Email" CssClass="edit" runat="server" OnTextChanged="Email_TextChanged" />
            </td>
        </tr>
    </asp:PlaceHolder>
    <tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="UpdateProfile" CssClass="pbutton" runat="server" OnClick="UpdateProfile_Click" />
            |
            <asp:Button ID="Cancel" CssClass="pbutton" runat="server" OnClick="Cancel_Click" />
        </td>
    </tr>
</table>
