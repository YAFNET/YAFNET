<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersSettings" Codebehind="EditUsersSettings.ascx.cs" %>

<asp:PlaceHolder runat="server" id="ForumSettingsRows">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" 
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="FORUM_SETTINGS" />
        </h4>
</asp:PlaceHolder>
<asp:PlaceHolder id="HideTr" visible="<%# this.Get<BoardSettings>().AllowUserHideHimself || this.PageContext.IsAdmin %>" runat="server">
    <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="Activity">
            <YAF:LocalizedLabel ID="LocalizedLabel35" runat="server" 
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="ACTIVITY" />
        </asp:Label>
        <div class="custom-control custom-switch">
            <asp:CheckBox Text="&nbsp;" ID="Activity" runat="server" Checked="false" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:Label runat="server" AssociatedControlID="HideMe">
    <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="EDIT_PROFILE"
                        LocalizedTag="HIDEME" />
</asp:Label>
<div class="custom-control custom-switch">
    <asp:CheckBox Text="&nbsp;" ID="HideMe" runat="server" Checked="false" />
</div>
<div class="form-group">
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
        <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="Theme">
            <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" 
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="SELECT_THEME" />
        </asp:Label>
        <asp:DropDownList runat="server" ID="Theme" CssClass="select2-select" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="TrTextEditors">
        <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="ForumEditor">
            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedPage="EDIT_PROFILE"
                LocalizedTag="SELECT_TEXTEDITOR" />
        </asp:Label>
        
         <asp:DropDownList ID="ForumEditor" runat="server" 
                           CssClass="select2-select"
                           DataValueField="Value" 
                           DataTextField="Name">
         </asp:DropDownList>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="UserLanguageRow">
        <div class="form-group">
        <asp:Label runat="server" AssociatedControlID="Culture">
            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" 
                                LocalizedPage="EDIT_PROFILE"
                                LocalizedTag="SELECT_LANGUAGE" />
        </asp:Label>
        <asp:DropDownList runat="server" ID="Culture" CssClass="select2-select" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="LoginInfo" Visible="false">
        <h4>
                <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" 
                                        LocalizedPage="EDIT_PROFILE"
                                        LocalizedTag="CHANGE_EMAIL" />
            </h4>
        <div class="form-group">
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
                     OnClick="UpdateProfileClick"
                     Icon="save" 
                     TextLocalizedTag="SAVE" 
                     TextLocalizedPage="COMMON" />
    &nbsp;
    <YAF:ThemeButton ID="Cancel" runat="server" 
                     Type="Secondary" 
                     OnClick="CancelClick"
                     Icon="trash" 
                     TextLocalizedTag="CANCEL" 
                     TextLocalizedPage="COMMON" />
</div>