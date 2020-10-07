<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersProfile" CodeBehind="EditUsersProfile.ascx.cs" %>

<asp:PlaceHolder ID="ProfilePlaceHolder" runat="server">
    <h2>
        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
            LocalizedPage="EDIT_PROFILE"
            LocalizedTag="aboutyou" />
    </h2>
    <asp:PlaceHolder ID="DisplayNamePlaceholder" runat="server" Visible="false">
        <div class="mb-3">
            <asp:Label runat="server" AssociatedControlID="DisplayName">
                <YAF:LocalizedLabel ID="LocalizedLabel34" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="DISPLAYNAME" />
            </asp:Label>
            <asp:TextBox ID="DisplayName" runat="server" CssClass="form-control" />
        </div>
    </asp:PlaceHolder>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Realname">
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="REALNAME2" />
        </asp:Label>
        <asp:TextBox ID="Realname" runat="server" CssClass="form-control" />
    </div>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Birthday">
            <YAF:LocalizedLabel ID="BirthdayLabel" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="BIRTHDAY" />
        </asp:Label>
        <div class='input-group mb-3 date datepickerinput'>
            <button class="btn btn-secondary datepickerbutton" type="button">
                <i class="fa fa-calendar-day fa-fw"></i>
            </button>
            <asp:TextBox ID="Birthday" runat="server"
                CssClass="form-control"></asp:TextBox>
        </div>
    </div>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Occupation">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="OCCUPATION" />
        </asp:Label>
        <asp:TextBox ID="Occupation" runat="server"
            CssClass="form-control" />
    </div>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Gender">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="INTERESTS" />
        </asp:Label>
        <asp:TextBox ID="Interests" runat="server"
            CssClass="form-control"
            TextMode="MultiLine"
            MaxLength="400"
            Rows="4" />
    </div>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Gender">
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="GENDER" />
        </asp:Label>
        <div class="form-check form-check-inline">
            <asp:RadioButtonList ID="Gender" runat="server"
                RepeatLayout="UnorderedList"
                CssClass="list-unstyled" />
        </div>
    </div>
    <h4>
        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
            LocalizedPage="EDIT_PROFILE"
            LocalizedTag="LOCATION" />
    </h4>

    <div class="mb-3">
        <YAF:ThemeButton runat="server" ID="GetLocation"
            Visible="<%# this.PageContext.BoardSettings.EnableIPInfoService %>"
            Icon="location-arrow"
            Type="Secondary"
            TextLocalizedTag="GET_LOCATION"
            OnClick="GetLocationOnClick">
        </YAF:ThemeButton>
    </div>
    <div class="row">
        <div class="mb-3 col-md-4">
            <asp:Label runat="server" AssociatedControlID="Country">
                <YAF:LocalizedLabel ID="LocalizedLabel40" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="COUNTRY" />
            </asp:Label>
            <YAF:CountryImageListBox runat="server" ID="Country"
                AutoPostBack="true"
                OnTextChanged="LookForNewRegions"
                CssClass="select2-image-select" />

        </div>
        <asp:PlaceHolder ID="RegionTr" Visible="false" runat="server">
            <div class="mb-3 col-md-4">
                <asp:Label runat="server" AssociatedControlID="Region">
                    <YAF:LocalizedLabel ID="LocalizedLabel41" runat="server"
                        LocalizedPage="EDIT_PROFILE"
                        LocalizedTag="REGION" />
                </asp:Label>
                <asp:DropDownList ID="Region" runat="server"
                    CssClass="select2-select" />
            </div>
        </asp:PlaceHolder>
        <div class="mb-3 col-md-4">
            <asp:Label runat="server" AssociatedControlID="City">
                <YAF:LocalizedLabel ID="LocalizedLabel42" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="CITY" />
            </asp:Label>
            <asp:TextBox ID="City" runat="server" CssClass="form-control" />
        </div>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Location">
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="where" />
        </asp:Label>
        <asp:TextBox ID="Location" runat="server" CssClass="form-control" />
    </div>
    <h2>
        <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server"
            LocalizedPage="EDIT_PROFILE"
            LocalizedTag="homepage" />
    </h2>
    <div class="row">
        <div class="mb-3 col-md-6">
            <asp:Label runat="server" AssociatedControlID="HomePage">
                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="homepage2" />
            </asp:Label>
            <asp:TextBox runat="server" ID="HomePage"
                CssClass="form-control"
                TextMode="Url" />
        </div>
        <div class="mb-3 col-md-6">
            <asp:Label runat="server" AssociatedControlID="Weblog">
                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="weblog2" />
            </asp:Label>
            <asp:TextBox runat="server" ID="Weblog"
                CssClass="form-control"
                TextMode="Url" />
        </div>
    </div>

</asp:PlaceHolder>
<asp:PlaceHolder ID="IMServicesPlaceHolder" runat="server">

    <h4>
        <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server"
            LocalizedPage="EDIT_PROFILE"
            LocalizedTag="messenger" />
    </h4>
    <div class="row">
        <div class="mb-3 col-md-6">
            <asp:Label runat="server" AssociatedControlID="ICQ">
                <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="ICQ" />
            </asp:Label>
            <asp:TextBox runat="server" ID="ICQ"
                CssClass="form-control"
                TextMode="Number" />
        </div>
        <div class="mb-3 col-md-6">
            <asp:Label runat="server" AssociatedControlID="Twitter">
                <YAF:LocalizedLabel ID="LocalizedLabel33" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="Twitter" />
            </asp:Label>
            <asp:TextBox runat="server" ID="Twitter" CssClass="form-control" />
        </div>
    </div>
    <div class="row">
        <div class="mb-3 col-md-6">
            <asp:Label runat="server" AssociatedControlID="Xmpp">
                <YAF:LocalizedLabel ID="LocalizedLabel32" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="xmpp" />
            </asp:Label>
            <asp:TextBox runat="server" ID="Xmpp" CssClass="form-control" />
        </div>
        <div class="mb-3 col-md-6">
            <asp:Label runat="server" AssociatedControlID="Skype">
                <YAF:LocalizedLabel ID="LocalizedLabel30" runat="server"
                    LocalizedPage="EDIT_PROFILE"
                    LocalizedTag="SKYPE" />
            </asp:Label>
            <asp:TextBox runat="server" ID="Skype" CssClass="form-control" />
        </div>
    </div>


    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Facebook">
            <YAF:LocalizedLabel ID="LocalizedLabel31" runat="server"
                LocalizedPage="EDIT_PROFILE"
                LocalizedTag="Facebook" />
        </asp:Label>
        <asp:TextBox runat="server" ID="Facebook" CssClass="form-control" />
    </div>
</asp:PlaceHolder>
<asp:Repeater runat="server" ID="CustomProfile" Visible="False" OnItemDataBound="CustomProfile_OnItemDataBound">
    <HeaderTemplate>
        <hr/>
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server"
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="OTHER" />
        </h4>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="mb-3">
            <asp:HiddenField runat="server" ID="DefID" />
            <asp:Label runat="server" ID="DefLabel" />
            <asp:TextBox runat="server" ID="DefText" Visible="False" />
            <asp:TextBox runat="server" ID="DefCheck" Visible="False" />
        </div>
    </ItemTemplate>
</asp:Repeater>

<div class="text-lg-center">
    <YAF:ThemeButton ID="UpdateProfile" runat="server"
                     Type="Primary"
                     OnClick="UpdateProfileClick"
                     CssClass="mr-2"
                     Icon="save"
                     TextLocalizedTag="SAVE"
                     TextLocalizedPage="COMMON" />
    <YAF:ThemeButton ID="Cancel" runat="server"
                        Type="Secondary"
                        OnClick="CancelClick"
                        Icon="trash"
                        TextLocalizedTag="CANCEL"
                        TextLocalizedPage="COMMON" />
</div>
