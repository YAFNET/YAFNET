<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Albums" Codebehind="albums.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumList" Src="../controls/AlbumList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellpadding="0">
    <tr>
        <td class="header1">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td>
            <YAF:AlbumList ID="AlbumList1" runat="server"></YAF:AlbumList>
        </td>
    </tr>
</table>
