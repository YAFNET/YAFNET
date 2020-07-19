<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Album" Codebehind="Album.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumImageList" Src="../controls/AlbumImageList.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<section class="text-center container mb-3">
    <div class="row">
        <div class="col-lg-6 col-md-8 mx-auto">
            <h1 class="font-weight-light">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="Albums_Title" />
            </h1>
            <p>
                <YAF:ThemeButton runat="server" ID="EditAlbums"
                                 OnClick="EditAlbums_Click" 
                                 TextLocalizedTag="BUTTON_EDITALBUMIMAGES"
                                 Icon="plus"/>
                <YAF:ThemeButton runat="server" ID="Back" 
                                 OnClick="Back_Click"
                                 TextLocalizedTag="BACK_ALBUMS"
                                 Type="Secondary"
                                 Icon="arrow-circle-left"/>
            </p>
        </div>
    </div>
</section>

<YAF:AlbumImageList ID="AlbumImageList1" runat="server" />