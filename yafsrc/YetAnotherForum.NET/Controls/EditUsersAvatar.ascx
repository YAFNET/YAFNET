<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersAvatar" Codebehind="EditUsersAvatar.ascx.cs" %>


<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-user-tie fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedPage="EDIT_AVATAR" LocalizedTag="title" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder runat="server" id="avatarImageTD">
                    <h5 class="card-title">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                            LocalizedPage="EDIT_AVATAR"
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
                                     ReturnConfirmText='<%# this.GetText("EDIT_AVATAR", "AVATARDELETE") %>'
                                     Visible="false" 
                                     OnClick="DeleteAvatar_Click" />
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" id="AvatarOurs">
                    <div class="form-group">
                        <h4><asp:Label runat="server">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="EDIT_AVATAR"
                                                LocalizedTag="ouravatar" />
                        </asp:Label>
                        </h4>
                        <p>
                            [
                            <asp:HyperLink ID="OurAvatar" runat="server" />
                            ]</p>
                    </div>
                    
                    <hr />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" id="AvatarRemoteRow">
                    <div class="form-group">
                        <h4><asp:Label runat="server" AssociatedControlID="Avatar">
                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                LocalizedPage="EDIT_AVATAR"
                                                LocalizedTag="avatarremote" />
                        </asp:Label>
                        </h4>
                        <asp:TextBox CssClass="form-control" ID="Avatar" runat="server" 
                                     TextMode="Url" />
                    </div>
                    <YAF:Alert runat="server" Type="info">
                        <asp:Label id="noteRemote" runat="server"></asp:Label>
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
                <asp:PlaceHolder runat="server" id="AvatarUploadRow">
                    <div class="form-group">
                        <h4><asp:Label runat="server" AssociatedControlID="File">
                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                LocalizedPage="EDIT_AVATAR"
                                                LocalizedTag="avatarupload" />
                        </asp:Label>
                        </h4>
                        <div class="custom-file mb-3">
                            <input type="file" id="File" runat="server" class="custom-file-input" />
                            <asp:Label runat="server" AssociatedControlID="File" CssClass="custom-file-label">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                    LocalizedPage="EDIT_AVATAR"
                                                    LocalizedTag="avatarupload" />
                            </asp:Label>
                        </div>
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