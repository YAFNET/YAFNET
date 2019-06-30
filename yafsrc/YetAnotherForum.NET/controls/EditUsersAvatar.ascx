<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersAvatar" Codebehind="EditUsersAvatar.ascx.cs" %>


<div class="row">
    <div class="col-xl-12">
        <h2>
            <YAF:LocalizedLabel runat="server" LocalizedPage="CP_EDITAVATAR" LocalizedTag="title" />
        </h2>
    </div>
</div>


<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-tie fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedPage="CP_EDITAVATAR" LocalizedTag="title" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" id="avatarImageTD">
                    <h5 class="card-title">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                            LocalizedPage="CP_EDITAVATAR"
                                            LocalizedTag="AvatarCurrent" />
                    </h5>
                    <p class="card-text">
                        <asp:Image ID="AvatarImg" runat="server" Visible="true" AlternateText="Avatar Image"
                                   CssClass="img-thumbnail"/>
                    </p>
                    <asp:Label runat="server" ID="NoAvatar" Visible="false" />
                    <YAF:ThemeButton runat="server" ID="DeleteAvatar" 
                                     Type="Danger" 
                                     Icon="trash"
                                     TextLocalizedTag="AVATARDELETE"
                                     ReturnConfirmText='<%# this.GetText("CP_EDITAVATAR", "AVATARDELETE") %>'
                                     Visible="false" 
                                     OnClick="DeleteAvatar_Click" />
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" id="AvatarOurs">
                    <h4>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="CP_EDITAVATAR"
                                            LocalizedTag="ouravatar" />
                    </h4>
                    <p>
                        [
                        <asp:HyperLink ID="OurAvatar" runat="server" />
                        ]</p>
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" id="AvatarRemoteRow">
                    <h4>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="CP_EDITAVATAR"
                                            LocalizedTag="avatarremote" />
                    </h4>
                    <p>
                        <asp:TextBox CssClass="form-control" ID="Avatar" runat="server" TextMode="Url" />
                    </p>
                    <YAF:Alert runat="server" Type="info">
                        <asp:Label id="noteRemote" runat="server"></asp:Label>
                    </YAF:Alert>
                    <p>
                        <YAF:ThemeButton ID="UpdateRemote" Type="Primary" runat="server" OnClick="RemoteUpdate_Click" TextLocalizedTag="UPDATE" Icon="save" />
                    </p>
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" id="AvatarUploadRow">
                    <h4>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="CP_EDITAVATAR"
                                            LocalizedTag="avatarupload" />
                    </h4>
                    <div class="custom-file mb-3">
                        <input type="file" id="File" runat="server" class="custom-file-input" />
                        <asp:Label runat="server" AssociatedControlID="File" CssClass="custom-file-label">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_EDITAVATAR"
                                                LocalizedTag="avatarupload" />
                        </asp:Label>
                    </div>
                    <YAF:Alert runat="server" Type="info">
                        <asp:Label id="noteLocal" runat="server"></asp:Label>
                    </YAF:Alert>
                </asp:PlaceHolder>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="UpdateUpload" runat="server" 
                                 OnClick="UploadUpdate_Click" 
                                 Type="Primary"
                                 TextLocalizedTag="SAVE"
                                 Icon="save"/>
                <YAF:ThemeButton ID="Back" 
                                 Type="Secondary" runat="server" 
                                 OnClick="Back_Click"
                                 Icon="reply"
                                 TextLocalizedTag="BACK" />
            </div>
        </div>
    </div>
</div>