<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Album" Codebehind="album.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumImageList" Src="../controls/AlbumImageList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellpadding="0">
    <tr>
        <td class="header1">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="Albums_Title" />
        </td>
    </tr>
    <tr>
        <td>
            <YAF:AlbumImageList ID="AlbumImageList1" runat="server"></YAF:AlbumImageList>
        </td>
    </tr>
</table>
