<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumList" Codebehind="AlbumList.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<div class="imgtitle">
   <YAF:LocalizedLabel ID="AlbumHeaderLabel" runat="server" LocalizedTag="ALBUMS_HEADER_TEXT" Param0="" />
</div>
<br />
<asp:Button ID="AddAlbum" CssClass="pbutton" runat="server" OnClick="AddAlbum_Click" />
<br />
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<div class="fileattach">
    <asp:Repeater runat="server" ID="Albums" OnItemCommand="Albums_ItemCommand">
        <ItemTemplate>
            <div class="attachedimg" style="display: inline;">
                <table class="albumtable" style="display: inline" width='<%# YafContext.Current.BoardSettings.ImageAttachmentResizeWidth %>'>
                    <tr>
                        <td class="albumimagebox">
                            <a href='<%# YafBuildLink.GetLink(ForumPages.album, "u={0}&a={1}", Eval("UserID"), Eval("AlbumID")) %>'
                                target="_parent" title='<%# HtmlEncode(Eval("Title"))%>'>
                                <img src='<%# String.Format("{0}resource.ashx?album={1}&cover={2}",YafForumInfo.ForumClientFileRoot, Eval("AlbumID"), (Eval("CoverImageID").ToString() == string.Empty ? "0" : Eval("CoverImageID")) ) %>'
                                    alt='<%# HtmlEncode(Eval("Title")) %>' />
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td class="albumtitlebox">
                            <span runat="server" id="spnUser" visible='<%# UserID != PageContext.PageUserID %>'>
                                <%# HtmlEncode(Eval("Title"))%></span> <span runat="server" id="spnAlbumOwner" visible='<%# UserID == PageContext.PageUserID %>'>
                                    <span class="albumtitle" id='<%# "spnTitle0" + Eval("AlbumID") %>' onclick="showTexBox(this.id)"
                                        style="display: inline;"><%# Eval("Title").ToString() == string.Empty ? PageContext.Localization.GetText("ALBUM_CHANGE_TITLE") : HtmlEncode(Eval("Title"))%></span>
                                    <input type="text" id='<%# "txtTitle0" + Eval("AlbumID") %>' onkeydown="checkKey(event, this,'<%#Eval("AlbumID") %>',true)"
                                        onblur="blurTextBox(this.id, '<%# Eval("AlbumID")%>', true)" style="display: none;" />
                                    <asp:Button ID="Edit" CssClass="pbutton" Text='<%# PageContext.Localization.GetText("BUTTON","EDIT") %>'
                                        Visible='<%# UserID == PageContext.PageUserID %>' runat="server" CommandName="edit"
                                        CommandArgument='<%# Eval("AlbumID") %>' />
                                </span>
                        </td>
                    </tr>
                </table>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
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
