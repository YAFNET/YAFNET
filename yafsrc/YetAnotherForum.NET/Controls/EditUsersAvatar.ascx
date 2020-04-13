<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersAvatar" CodeBehind="EditUsersAvatar.ascx.cs" %>


<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                IconName="user-tie"
                                LocalizedPage="EDIT_AVATAR" 
                                LocalizedTag="title" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" ID="avatarImageTD">
                    <h5 class="card-title">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server"
                                            LocalizedPage="EDIT_AVATAR"
                                            LocalizedTag="AvatarCurrent" />
                    </h5>
                    <p class="card-text">
                        <asp:Image ID="AvatarImg" runat="server" Visible="true" AlternateText="Image"
                            CssClass="img-thumbnail" />
                    </p>
                    <asp:Label runat="server" ID="NoAvatar" Visible="false" />
                    <YAF:ThemeButton runat="server" ID="DeleteAvatar"
                                     Type="Danger"
                                     Icon="trash"
                                     TextLocalizedTag="AVATARDELETE"
                                     ReturnConfirmText='<%# this.GetText("EDIT_AVATAR", "AVATARDELETE") %>'
                                     Visible="false"
                                     OnClick="DeleteAvatar_Click" />
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="AvatarOurs">
                    <div class="form-group">
                        <h4>
                            <asp:Label runat="server">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                    LocalizedPage="EDIT_AVATAR"
                                                    LocalizedTag="ouravatar" />
                            </asp:Label>
                        </h4>
                        <YAF:ThemeButton runat="server" ID="OurAvatar" 
                                         Type="Primary"
                                         Icon="images"
                                         TextLocalizedTag="OURAVATAR_SELECT"/>
                    </div>

                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="AvatarRemoteRow">
                    <div class="form-group">
                        <h4>
                            <asp:Label runat="server" AssociatedControlID="Avatar">
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server"
                                                    LocalizedPage="EDIT_AVATAR"
                                                    LocalizedTag="avatarremote" />
                            </asp:Label>
                        </h4>
                        <asp:TextBox ID="Avatar" runat="server"
                                     CssClass="form-control" 
                                     TextMode="Url" />
                    </div>
                    <YAF:Alert runat="server" Type="info">
                        <asp:Label ID="noteRemote" runat="server"></asp:Label>
                    </YAF:Alert>
                    <div class="form-group">
                        <YAF:ThemeButton ID="UpdateRemote" runat="server"
                                         Type="Primary"
                                         OnClick="RemoteUpdate_Click"
                                         TextLocalizedTag="UPDATE"
                                         Icon="save" />
                    </div>
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="AvatarUploadRow">
                    <div class="form-group">
                        <h4>
                            <asp:Label runat="server" AssociatedControlID="File">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedPage="EDIT_AVATAR"
                                                    LocalizedTag="AVATARUPLOAD" />
                            </asp:Label>
                        </h4>
                        <div class="input-group">
                            <div class="custom-file mb-3">
                                <input type="file" id="File" runat="server" class="custom-file-input" aria-describedby="File" />
                                <asp:Label runat="server" AssociatedControlID="File" CssClass="custom-file-label">
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                        LocalizedPage="EDIT_AVATAR"
                                                        LocalizedTag="AVATARUPLOAD" />
                                </asp:Label>
                            </div>
                        </div>
                    </div>
                    <YAF:Alert runat="server" Type="info">
                        <asp:Label ID="noteLocal" runat="server"></asp:Label>
                    </YAF:Alert>
                </asp:PlaceHolder>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="UpdateUpload" runat="server"
                                 OnClick="UploadUpdate_Click"
                                 Type="Primary"
                                 TextLocalizedTag="SAVE"
                                 Icon="save" />
                <YAF:ThemeButton ID="Back"
                                 Type="Secondary" runat="server"
                                 OnClick="Back_Click"
                                 Icon="reply"
                                 TextLocalizedTag="BACK" />
            </div>
        </div>
    </div>
</div>
