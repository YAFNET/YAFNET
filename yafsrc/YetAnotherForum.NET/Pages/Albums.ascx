<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Albums" Codebehind="Albums.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumList" Src="../controls/AlbumList.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="title" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <YAF:AlbumList ID="AlbumList1" runat="server"></YAF:AlbumList>
    </div>
</div>
