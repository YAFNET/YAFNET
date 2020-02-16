<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Album" Codebehind="Album.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumImageList" Src="../controls/AlbumImageList.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2>
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="Albums_Title" />
        </h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <YAF:AlbumImageList ID="AlbumImageList1" runat="server"></YAF:AlbumImageList>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="Back" 
                                 OnClick="Back_Click"
                                 TextLocalizedTag="BACK_ALBUMS"
                                 Type="Secondary"
                                 Icon="arrow-circle-left"/>
            </div>
        </div>
    </div>
</div>