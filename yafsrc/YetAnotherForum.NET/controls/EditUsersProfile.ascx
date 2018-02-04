<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersProfile" Codebehind="EditUsersProfile.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
    <asp:PlaceHolder ID="ProfilePlaceHolder" runat="server">

        <h2>
            <YAF:LocalizedLabel runat="server"  LocalizedPage="CP_EDITPROFILE" LocalizedTag="TITLE" />
        <small>
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="aboutyou" />
        </small></h2>

    <hr />
    <asp:PlaceHolder ID="DisplayNamePlaceholder" runat="server" Visible="false">

            <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel34" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="DISPLAYNAME" />
            </h4>
            <p>
                <asp:TextBox ID="DisplayName" runat="server" CssClass="form-control" />
            </p>
        <hr />
    </asp:PlaceHolder>

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="REALNAME2" />
        </h4>
        <p>
            <asp:TextBox ID="Realname" runat="server" CssClass="form-control" />
        </p>
     <hr />
     <asp:PlaceHolder id="HideTr" visible="<%# this.Get<YafBoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin %>" runat="server">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel35" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="HIDEME" />
        </h4>
        <p>
            <asp:CheckBox CssClass="form-control" ID="HideMe" runat="server" Checked="false" />
        </p>
    <hr />
    </asp:PlaceHolder>
        <h4>
            <YAF:LocalizedLabel ID="BirthdayLabel" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="BIRTHDAY" />
        </h4>
        <div class='input-group mb-3 date datepickerinput'>
            <span class="input-group-prepend">
                <button class="btn btn-secondary datepickerbutton" type="button">
                    <i class="fa fa-calendar fa-fw"></i>
                </button>
            </span>
            <asp:TextBox ID="Birthday" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="OCCUPATION" />
        </h4>
        <p>
            <asp:TextBox ID="Occupation" runat="server" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="INTERESTS" />
        </h4>
        <p>
            <asp:TextBox ID="Interests" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="400" Rows="5" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="GENDER" />
        </h4>
        <p>
            <asp:RadioButtonList ID="Gender" runat="server" CssClass="form-control" RepeatDirection="Horizontal" />
        </p>
    <hr />

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="LOCATION" />
            </strong>
        </h4>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel40" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="COUNTRY" />
        </h4>
        <p>
            <YAF:ImageListBox ID="Country" AutoPostBack="true" OnTextChanged="LookForNewRegions" runat="server"
                CssClass="selectpicker custom-select" />
        </p>
    <hr />
     <asp:PlaceHolder id="RegionTr" visible="false" runat="server">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel41" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="REGION" />
        </h4>
        <p>
            <asp:DropDownList ID="Region" runat="server" CssClass="standardSelectMenu custom-select" />
        </p>
         </asp:PlaceHolder>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel42" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="CITY" />
        </h4>
        <p>
            <asp:TextBox ID="City" runat="server" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="where" />
        </h4>
        <p>
            <asp:TextBox ID="Location" runat="server" CssClass="form-control" />
        </p>
    <hr />

        <h2>
             <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="homepage" />
        </h2>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="homepage2" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="HomePage" CssClass="form-control" TextMode="Url" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="weblog2" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="Weblog" CssClass="form-control" TextMode="Url" />
        </p>
    <hr />
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="MetaWeblogAPI" Visible="true">

            <h4>
                <strong>
                    <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedPage="CP_EDITPROFILE"
                        LocalizedTag="METAWEBLOG_TITLE" />
                </strong>
            </h4>
        <hr />

            <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_URL" />
            </h4>
            <p>
                <asp:TextBox runat="server" ID="WeblogUrl" CssClass="form-control" TextMode="Url" />
            </p>
        <hr />

            <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_ID" />
                <br />
                <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_ID_INSTRUCTIONS" />
            </h4>
            <p>
                <asp:TextBox runat="server" ID="WeblogID" CssClass="form-control" />
            </p>
        <hr />

            <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="METAWEBLOG_API_USERNAME" />
            </h4>
            <p>
                <asp:TextBox runat="server" ID="WeblogUsername" CssClass="form-control" />
            </p>
        <hr />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="IMServicesPlaceHolder" runat="server">

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="messenger" />
            </strong>
        </h4>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="MSN" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="MSN" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="YIM" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="YIM" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="AIM" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="AIM" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="ICQ" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="ICQ" CssClass="form-control" TextMode="Number" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel31" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="Facebook" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="Facebook" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel33" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="Twitter" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="Twitter" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel36" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="Google" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="Google" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel32" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="xmpp" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="Xmpp" CssClass="form-control" />
        </p>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel30" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SKYPE" />
        </h4>
        <p>
            <asp:TextBox runat="server" ID="Skype" CssClass="form-control" />
        </p>
    <hr />
    </asp:PlaceHolder>

        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="TIMEZONE" />
            </strong>
        </h4>
    <hr />

        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="TIMEZONE2" />
        </h4>
        <p>
            <asp:DropDownList runat="server" ID="TimeZones" DataTextField="Name" DataValueField="Value" CssClass="standardSelectMenu custom-select" />
        </p>
    <hr />
    <asp:PlaceHolder runat="server" id="ForumSettingsRows">
        <h4>
            <strong>
                <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="FORUM_SETTINGS" />
            </strong>
        </h4>
    <hr />
        </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="UserThemeRow">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_THEME" />
        </h4>
        <p>
            <asp:DropDownList runat="server" ID="Theme" CssClass="standardSelectMenu custom-select" />
        </p>
    <hr />
        </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="TrTextEditors">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_TEXTEDITOR" />
        </h4>
        <p>
         <asp:DropDownList ID="ForumEditor" CssClass="standardSelectMenu custom-select" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
        </p>
    <hr />
            </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="UseMobileThemeRow" visible="false">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="USE_MOBILE_THEME" />
        </h4>
        <p>
            <asp:CheckBox CssClass="form-control" ID="UseMobileTheme" runat="server" />
        </p>
    <hr />
            </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="UserLanguageRow">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedPage="CP_EDITPROFILE"
                LocalizedTag="SELECT_LANGUAGE" />
        </h4>
        <h2>
             <asp:DropDownList runat="server" ID="Culture" CssClass="standardSelectMenu custom-select" />
        </h2>
    <hr />
        </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="LoginInfo" Visible="false">

            <h2>
                <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="CHANGE_EMAIL" />
            </h2>
        <hr />

            <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedPage="CP_EDITPROFILE"
                    LocalizedTag="EMAIL" />
            </h4>
            <p>
                <asp:TextBox ID="Email" CssClass="form-control" runat="server" OnTextChanged="Email_TextChanged" TextMode="Email" />
            </p>
    </asp:PlaceHolder>

                <div class="text-lg-center">

            <YAF:ThemeButton ID="UpdateProfile" Type="Primary" runat="server" OnClick="UpdateProfile_Click"
                             Icon="save" TextLocalizedTag="SAVE" TextLocalizedPage="COMMON" />
            &nbsp;
            <YAF:ThemeButton ID="Cancel" Type="Secondary" runat="server" OnClick="Cancel_Click"
                             Icon="trash" TextLocalizedTag="CANCEL" TextLocalizedPage="COMMON" />
                    &nbsp;
            </div>
