<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Album" Codebehind="album.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="AlbumImageList" Src="../controls/AlbumImageList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" style="width:100%;padding:0">
    <tr>
        <td class="header1">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="Albums_Title" />
        </td>
    </tr>
    <tr>
        <td class="post">
            <YAF:AlbumImageList ID="AlbumImageList1" runat="server"></YAF:AlbumImageList>
        </td>
    </tr>
    <tr class="footer1">
		<td colspan="3" style="text-align: center">
			<asp:Button runat="server" CssClass="pbutton" ID="Back" OnClick="Back_Click" />
		</td>
	</tr>
</table>
