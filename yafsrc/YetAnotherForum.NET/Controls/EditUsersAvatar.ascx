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
                        <asp:Image ID="AvatarImg" runat="server" 
                                   Visible="true" 
                                   AlternateText="Image"
                                   CssClass="img-thumbnail"/>
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
                    <div class="mb-3">
                        <h4>
                            <asp:Label runat="server">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                    LocalizedPage="EDIT_AVATAR"
                                                    LocalizedTag="ouravatar" />
                            </asp:Label>
                        </h4>
                        <YAF:ImageListBox ID="AvatarGallery" runat="server" CssClass="select2-image-select" /> 
                    </div>
                    <div class="mb-3">
                        <YAF:ThemeButton ID="UpdateGallery" runat="server"
                                         Type="Primary"
                                         OnClick="GalleryUpdateClick"
                                         TextLocalizedTag="UPDATE"
                                         Icon="save" />
                    </div>
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="AvatarUploadRow">
                    <div class="mb-3">
                        <h4>
                            <asp:Label runat="server" AssociatedControlID="File">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedPage="EDIT_AVATAR"
                                                    LocalizedTag="AVATARUPLOAD" />
                            </asp:Label>
                        </h4>
                        <div class="form-file">
                            <input type="file" id="File" runat="server" class="form-file-input" aria-describedby="File" />
                            <label for="<%# this.File.ClientID %>" class="form-file-label">
                                <span class="form-file-text">
                                    <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server"
                                                        LocalizedPage="EDIT_AVATAR"
                                                        LocalizedTag="AVATARUPLOAD" />
                                </span>
                                <span class="form-file-button">
                                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server"
                                                        LocalizedTag="BROWSE" />
                                </span>
                            </label>
                        </div>
                    </div>
                    <YAF:Alert runat="server" Type="info">
                        <YAF:Icon runat="server" IconName="info-circle"></YAF:Icon>
                        <asp:Label ID="noteLocal" runat="server"></asp:Label>
                    </YAF:Alert>
                </asp:PlaceHolder>
                <div class="text-lg-center">
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
</div>
