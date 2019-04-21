﻿<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.cp_editalbumimages" Codebehind="cp_editalbumimages.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>

<YAF:PageLinks runat="server" ID="PageLinks" />


<div class="row">
    <div class="col-xl-12">
        <h2>
            <YAF:LocalizedLabel ID="Title" LocalizedTag="TITLE" runat="server" />
        </h2>
    </div>
</div>

<div class="row">
    <div class="col-3">
        <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
    </div>
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-images fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="title" />
            </div>
            <div class="card-body text-center">
                <asp:PlaceHolder id="TitleRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="txtTitle">
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ALBUM_TITLE" />
                        </asp:Label>
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" MaxLength="255" />
                    </div>
                    <YAF:ThemeButton ID="UpdateTitle" runat="server"
                                     CssClass="mb-3"
                                     OnClick="UpdateTitle_Click" 
                                     TextLocalizedTag="UPDATE"
                                     Type="Secondary" Icon="pen"/>
                    
                </asp:PlaceHolder>
                <asp:Repeater runat="server" ID="List" OnItemCommand="List_ItemCommand">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                    <ItemTemplate>
            
                <li class="list-group-item">
                    <%# this.Eval( "FileName") %>
                
                    (<%# (int)this.Eval("Bytes") / 1024%> Kb)
                
                
                    <YAF:ThemeButton ID="ImageDelete" runat="server" 
                                     ReturnConfirmText='<%# this.GetText("ASK_DELETEIMAGE") %>' 
                                     CommandName="delete" 
                                     CommandArgument='<%# this.Eval( "ImageID") %>'
                                     TextLocalizedTag="DELETE"
                                     Type="Danger"
                                     Icon="trash" />
            </li>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:PlaceHolder id="uploadtitletr" runat="server">
                    <hr/>
                    <h5>
                        <YAF:LocalizedLabel ID="UploadTitle" LocalizedTag="UPLOAD_TITLE" runat="server" />
                    </h5>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="selectfiletr" runat="server">
                    <div class="custom-file mb-3">
                        <input type="file" id="File" class="custom-file-input" runat="server" />
                        <asp:Label runat="server" AssociatedControlID="File" CssClass="custom-file-label">
                            <YAF:LocalizedLabel ID="SelectFile" 
                                                LocalizedTag="SELECT_FILE" 
                                                LocalizedPage="CP_EDITALBUMIMAGES" runat="server" />
                        </asp:Label>
                    </div>
                    <YAF:ThemeButton runat="server" ID="Upload" 
                                     OnClick="Upload_Click" 
                                     TextLocalizedTag="UPLOAD"
                                     CssClass="mb-3"
                                     Type="Success" 
                                     Icon="upload"/>
                </asp:PlaceHolder>
                <YAF:Alert runat="server" Type="info">
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

<div id="DivSmartScroller">
    
</div>
