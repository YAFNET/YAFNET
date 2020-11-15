<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.EditAlbumImages" Codebehind="EditAlbumImages.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                ID="Header"
                                IconName="images"
                                LocalizedTag="TITLE"
                                LocalizedPage="EDIT_ALBUMIMAGES" />
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="txtTitle">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                            LocalizedTag="ALBUM_TITLE"/>
                    </asp:Label>
                    <div class="input-group">
                        <asp:TextBox ID="txtTitle" runat="server" 
                                     required="required"
                                     CssClass="form-control"
                                     MaxLength="255" />
                        <div class="invalid-feedback">
                            <YAF:LocalizedLabel runat="server"
                                                LocalizedTag="NEED_USERNAME" />
                        </div>
                        <YAF:ThemeButton ID="UpdateTitle" runat="server"
                                         OnClick="UpdateTitle_Click" 
                                         TextLocalizedTag="UPDATE"
                                         Type="Secondary" 
                                         Icon="pen"/>
                    </div>
                </div>
                
                <asp:Repeater runat="server" ID="List" 
                              OnItemCommand="List_ItemCommand">
                    <HeaderTemplate>
                        <div class="mb-3">
                        <asp:Label runat="server">
                            <YAF:LocalizedLabel runat="server" 
                                                LocalizedTag="IMAGES" />
                        </asp:Label>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul></div>
                    </FooterTemplate>
                    <ItemTemplate>
                        <li class="list-group-item">
                            <%# this.Eval( "FileName") %>
                            (<%# (int)this.Eval("Bytes") / 1024%> Kb)
                            <YAF:ThemeButton ID="ImageDelete" runat="server" 
                                             ReturnConfirmText='<%# this.GetText("ASK_DELETEIMAGE") %>' 
                                             CommandName="delete" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TextLocalizedTag="DELETE"
                                             Type="Danger"
                                             Icon="trash" />
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder id="UploadHolder" runat="server">
                    <asp:Label runat="server" AssociatedControlID="File">
                        <YAF:LocalizedLabel ID="UploadTitle" 
                                            LocalizedTag="UPLOAD_TITLE" runat="server" />
                    </asp:Label>
                    <div class="input-group">
                        <div class="mb-3">
                            <label for="<%# this.File.ClientID %>" class="form-label">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" 
                                                    LocalizedTag="SELECT_FILE" 
                                                    LocalizedPage="EDIT_ALBUMIMAGES" runat="server" />
                            </label>
                            <input type="file" id="File" class="form-control" runat="server" />
                        </div>
                        <YAF:ThemeButton runat="server" ID="Upload" 
                                         CausesValidation="True"
                                         OnClick="Upload_Click" 
                                         TextLocalizedTag="UPLOAD"
                                         CssClass="mb-3"
                                         Type="Success" 
                                         Icon="upload"/>
                    </div>
                </asp:PlaceHolder>
                <YAF:Alert runat="server" Type="info">
                    <YAF:Icon runat="server" IconName="info-circle" />
                    <asp:Label ID="imagesInfo" runat="server"></asp:Label>
                </YAF:Alert>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="Delete" OnClick="DeleteAlbum_Click"
                                 TextLocalizedTag="Button_DeleteAlbum"
                                 ReturnConfirmText='<%# this.GetText("ASK_DELETEALBUM") %>'
                                 Type="Danger"
                                 Icon="trash"/>
                <YAF:ThemeButton runat="server" ID="Back" OnClick="Back_Click"
                                 TextLocalizedTag="BACK"
                                 Type="Secondary"
                                 Icon="arrow-circle-left"/>
            </div>
        </div>
    </div>
</div>