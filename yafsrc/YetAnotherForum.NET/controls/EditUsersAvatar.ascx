<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.EditUsersAvatar" Codebehind="EditUsersAvatar.ascx.cs" %>


        <h2>
            <YAF:LocalizedLabel runat="server" LocalizedPage="CP_EDITAVATAR" LocalizedTag="title" />
        </h2>
    <hr />
    <asp:PlaceHolder runat="server" id="AvatarCurrentText">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="CP_EDITAVATAR"
                LocalizedTag="AvatarCurrent" />
        </h4>
        <p>
            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_EDITAVATAR"
                LocalizedTag="AvatarNew" />
        </p>
    <hr />
    </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" id="avatarImageTD">
            <asp:Image ID="AvatarImg" runat="server" Visible="true" AlternateText="Avatar Image" />
            <br />
            <br />
            <asp:Label runat="server" ID="NoAvatar" Visible="false" />
            <asp:LinkButton runat="server" ID="DeleteAvatar" Type="Primary" Visible="false" OnClick="DeleteAvatar_Click" />
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
        <div class="alert alert-info" role="alert">
            <asp:Label id="noteRemote" runat="server"></asp:Label>
        </div>
        <p>
            <asp:LinkButton ID="UpdateRemote" Type="Primary" runat="server" OnClick="RemoteUpdate_Click" />
        </p>
    <hr />
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" id="AvatarUploadRow">
        <h4>
            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="CP_EDITAVATAR"
                LocalizedTag="avatarupload" />
        </h4>
        <p>
            <input type="file" id="File" runat="server" class="form-control-file" />
        </p>
        <div class="alert alert-info" role="alert">
            <asp:Label id="noteLocal" runat="server"></asp:Label>
        </div>
        </asp:PlaceHolder>

                <div class="text-lg-center">
                     <asp:LinkButton ID="UpdateUpload" Type="Primary" runat="server" OnClick="UploadUpdate_Click" />&nbsp;
            <asp:LinkButton ID="Back" Type="Secondary" runat="server" OnClick="Back_Click" />
            </div>
