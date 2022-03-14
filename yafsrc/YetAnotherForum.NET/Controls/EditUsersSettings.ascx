<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersSettings" Codebehind="EditUsersSettings.ascx.cs" %>

<h4>
            <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server"
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="FORUM_SETTINGS" />
        </h4>
<asp:PlaceHolder id="HideTr" visible="<%# this.PageBoardContext.BoardSettings.AllowUserHideHimself || this.PageBoardContext.IsAdmin %>" runat="server">
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Activity">
            <YAF:LocalizedLabel ID="LocalizedLabel35" runat="server"
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="ACTIVITY" />
        </asp:Label>
        <div class="form-check form-switch">
            <asp:CheckBox ID="Activity" runat="server"
                          Checked="false" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:Label runat="server" AssociatedControlID="HideMe">
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="EDIT_PROFILE"
                        LocalizedTag="HIDEME" />
</asp:Label>
<div class="form-check form-switch">
    <asp:CheckBox ID="HideMe" runat="server"
                  Checked="false" />
</div>
<div class="mb-3">
    <asp:Label runat="server" AssociatedControlID="TimeZones">
        <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server"
                            LocalizedPage="EDIT_PROFILE"
                            LocalizedTag="TIMEZONE2" />
    </asp:Label>
    <asp:DropDownList runat="server" ID="TimeZones"
                      DataTextField="Name"
                      DataValueField="Value"
                      CssClass="select2-select" />
</div>
    <asp:PlaceHolder runat="server" id="UserThemeRow">
        <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Theme">
            <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server"
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="SELECT_THEME" />
        </asp:Label>
        <asp:DropDownList runat="server" ID="Theme" CssClass="select2-select" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="UserLanguageRow">
        <div class="mb-3">
            <asp:Label runat="server" AssociatedControlID="Culture">
                <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server"
                                    LocalizedPage="EDIT_PROFILE"
                                    LocalizedTag="SELECT_LANGUAGE" />
            </asp:Label>
            <asp:DropDownList runat="server" ID="Culture" CssClass="select2-select" />
        </div>
    </asp:PlaceHolder>
<div class="mb-3">
    <asp:Label runat="server" AssociatedControlID="PageSize">
        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                            LocalizedPage="EDIT_PROFILE"
                            LocalizedTag="SELECT_PAGESIZE" />
    </asp:Label>
    <asp:DropDownList runat="server" ID="PageSize" CssClass="select2-select" />
</div>
<asp:PlaceHolder runat="server" ID="LoginInfo" Visible="false">
        <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server"
                                        LocalizedPage="EDIT_PROFILE"
                                        LocalizedTag="CHANGE_EMAIL" />
            </h4>
        <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Email">
                <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server"
                                    LocalizedPage="EDIT_PROFILE"
                                    LocalizedTag="EMAIL" />
            </asp:Label>
        <asp:TextBox ID="Email" runat="server"
                             CssClass="form-control"
                             OnTextChanged="EmailTextChanged"
                             TextMode="Email" />
            </div>
    </asp:PlaceHolder>

<div class="text-lg-center">
    <YAF:ThemeButton ID="UpdateProfile" runat="server"
                     Type="Primary"
                     CssClass="me-2"
                     OnClick="UpdateProfileClick"
                     Icon="save"
                     TextLocalizedTag="SAVE"
                     TextLocalizedPage="COMMON" />
    <YAF:ThemeButton ID="Cancel" runat="server"
                     Type="Secondary"
                     OnClick="CancelClick"
                     Icon="reply"
                     TextLocalizedTag="CANCEL"
                     TextLocalizedPage="COMMON" />
</div>