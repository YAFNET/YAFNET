<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.cp_editalbumimages" Codebehind="cp_editalbumimages.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="3">
            <YAF:LocalizedLabel ID="Title" LocalizedTag="TITLE" runat="server" />
        </td>
    </tr>
    <tr id="TitleRow" runat="server">
        <td class="postformheader">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ALBUM_TITLE" />
        </td>
        <td class="post">
            <asp:TextBox ID="txtTitle" runat="server" CssClass="edit" MaxLength="255" Width="400" />
            <asp:Button ID="UpdateTitle" runat="server" Text="Update" CssClass="pbutton" OnClick="UpdateTitle_Click" />
        </td>
        <td class="post">
            <asp:Button runat="server" CssClass="pbutton" ID="Delete" OnClick="DeleteAlbum_Click"
                OnLoad="DeleteAlbum_Load" Text='<%# this.GetText("Button_DeleteAlbum") %>' />
        </td>
    </tr>
    <asp:Repeater runat="server" ID="List" OnItemCommand="List_ItemCommand">
        <HeaderTemplate>
            <tr>
                <td class="header2">
                    <YAF:LocalizedLabel ID="Filename" LocalizedTag="FILENAME" runat="server" />
                </td>
                <td class="header2" align="right">
                    <YAF:LocalizedLabel ID="Size" LocalizedTag="SIZE" runat="server" />
                </td>
                <td class="header2">
                    &nbsp;
                </td>
            </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="post">
                    <%# Eval( "FileName") %>
                </td>
                <td class="post" align="right">
                    <%# (int)Eval("Bytes") / 1024%> Kb
                </td>
                <td class="post">
                    <asp:LinkButton ID="ImageDelete" runat="server" CssClass="pbutton" OnLoad="ImageDelete_Load" CommandName="delete"
                        CommandArgument='<%# Eval( "ImageID") %>'><%# this.GetText("DELETE") %></asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
    <tr id="uploadtitletr" runat="server">
        <td class="header2" colspan="3">
            <YAF:LocalizedLabel ID="UploadTitle" LocalizedTag="UPLOAD_TITLE" runat="server" />
        </td>
    </tr>
    <tr id="selectfiletr" runat="server">
        <td class="postheader">
            <YAF:LocalizedLabel ID="SelectFile" LocalizedTag="SELECT_FILE" LocalizedPage="CP_EDITALBUMIMAGES" runat="server" />
        </td>
        <td class="post">
            <input type="file" id="File" class="pbutton" runat="server" />
        </td>
        <td class="post">
            <asp:Button runat="server" CssClass="pbutton" ID="Upload" OnClick="Upload_Click" />
        </td>
    </tr>
    <tr class="footer1">
        <td colspan="3">
            <em><asp:Label ID="imagesInfo" runat="server"></asp:Label></em>
        </td>
    </tr>
    <tr class="footer1">
        <td colspan="3" align="center">
            <asp:Button runat="server" CssClass="pbutton" ID="Back" OnClick="Back_Click" />
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
