<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Albums" Codebehind="Albums.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumList" Src="../controls/AlbumList.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<YAF:AlbumList ID="AlbumList1" runat="server"></YAF:AlbumList>