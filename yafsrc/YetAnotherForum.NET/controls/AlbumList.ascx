<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumList"
    CodeBehind="AlbumList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<div class="imgtitle">
    <YAF:LocalizedLabel ID="AlbumHeaderLabel" runat="server" LocalizedTag="ALBUMS_HEADER_TEXT"
        Param0="" />
</div>
<br />
<p>
    <em>
        <asp:Label ID="albumsInfo" Visible="false" runat="server"></asp:Label></em></p>
<div>
    <asp:Button ID="AddAlbum" CssClass="pbutton" runat="server" Visible="false" OnClick="AddAlbum_Click" />
</div>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<asp:Repeater runat="server" ID="Albums" OnItemCommand="Albums_ItemCommand" OnItemDataBound="Albums_ItemDataBound">
    <HeaderTemplate>
        <div class="fileattach">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="attachedimg" style="display: inline;">
            <table class="albumtable" style="display: inline" width='<%# YafContext.Current.BoardSettings.ImageAttachmentResizeWidth %>'>
                <tr>
                    <td class="albumimagebox">
                        <a href='<%# YafBuildLink.GetLink(ForumPages.album, "u={0}&a={1}", Eval("UserID"), Eval("AlbumID")) %>'
                            target="_parent" title='<%# this.HtmlEncode(Eval("Title"))%>'>
                            <asp:Image runat="server" ID="coverImage" ImageUrl='<%# "{0}resource.ashx?album={1}&cover={2}".FormatWith(YafForumInfo.ForumClientFileRoot, this.Eval("AlbumID"), (this.Eval("CoverImageID").ToString() == string.Empty ? "0" : this.Eval("CoverImageID"))) %>'
                                ToolTip='<%# this.HtmlEncode(Eval("Title")) %>' runat="server" AlternateText='<%# Eval("AlbumID") %>' />
                    </td>
                </tr>
                <tr>
                    <td class="albumtitlebox">
                        <span runat="server" id="spnUser" visible='<%# UserID != PageContext.PageUserID %>'>
                            <%# this.HtmlEncode(Eval("Title"))%></span> <span runat="server" id="spnAlbumOwner"
                                visible='<%# UserID == PageContext.PageUserID %>'><span class="albumtitle" id='<%# "spnTitle0" + Eval("AlbumID") %>'
                                    onclick="showTexBox(this.id)" style="display: inline;"><%# Eval("Title").ToString() == string.Empty ? this.GetText("ALBUM_CHANGE_TITLE") : this.HtmlEncode(Eval("Title"))%></span>
                                <input type="text" id='<%# "txtTitle0" + Eval("AlbumID") %>' onkeydown="checkKey(event, this,'<%#Eval("AlbumID") %>',true)"
                                    onblur="blurTextBox(this.id, '<%# Eval("AlbumID")%>', true)" style="display: none;" />
                                <asp:Button ID="Edit" CssClass="pbutton" Text='<%# this.GetText("BUTTON","EDIT") %>'
                                    Visible='<%# UserID == PageContext.PageUserID %>' runat="server" CommandName="edit"
                                    CommandArgument='<%# Eval("AlbumID") %>' />
                            </span>
                    </td>
                </tr>
            </table>
        </div>
    </ItemTemplate>
    <FooterTemplate>
        </div></FooterTemplate>
</asp:Repeater>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
