<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Controls.AlbumImageList" Codebehind="AlbumImageList.ascx.cs" %>
<asp:Literal ID="ltrTitleOnly" runat="server"></asp:Literal>
<span runat="server" id="spnAlbumOwner" visible='<%# UserID == PageContext.PageUserID %>'>
    <span id='<%= "spnTitle0" + AlbumID %>' class="albumtitle" onclick="showTexBox(this.id)"
        style="display: inline;"><asp:Literal ID="ltrTitle" runat="server"></asp:Literal></span>
    <input type="text" id='<%= "txtTitle0" + AlbumID %>' onkeydown="checkKey(event, this,'<%= AlbumID %>',true)"
        onblur="blurTextBox(this.id, '<%= AlbumID %>', true)" style="display: none;" />
    <asp:Button ID="EditAlbums" CssClass="pbutton" runat="server" OnClick="EditAlbums_Click" />
</span>
<table class="command" cellspacing="0" cellpadding="0" width="100%">
    <tr>
        <td>
            <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
        </td>
    </tr>
</table>
<div class="fileattach">
    <asp:Repeater runat="server" ID="AlbumImages" OnItemDataBound="AlbumImages_ItemDataBound"
        OnItemCommand="AlbumImages_ItemCommand">
        <ItemTemplate>
            <div class="attachedimg" style="display: inline;">
                <table class="albumtable" style="display: inline" width='<%# YafContext.Current.BoardSettings.ImageAttachmentResizeWidth %>'>
                    <tr>
                        <td class="albumimagebox">
                            <a rel='<%# String.Format("lightbox-group{0}", this._attachGroupID) %>' href='<%# String.Format("{0}resource.ashx?image={1}",YafForumInfo.ForumClientFileRoot,Eval("ImageID")) %>'
                                target="_blank" title='<%# HtmlEncode(Eval("FileName")) %>'>
                                <img src='<%# String.Format("{0}resource.ashx?imgprv={1}",YafForumInfo.ForumClientFileRoot,Eval("ImageID")) %>'
                                    alt='<%# HtmlEncode(Eval("FileName")) %>' />
                            </a>
                        </td>
                    </tr>
                    <tr>
                        <td class="albumtitlebox">
                            <span runat="server" id="spnUser" visible='<%# UserID != PageContext.PageUserID %>'><%# HtmlEncode(Eval("Caption"))%></span> 
                            <span runat="server" id="spnImageOwner" visible='<%# UserID == PageContext.PageUserID %>'>
                                    <span class="albumtitle" id='<%# "spnTitle" + Eval("ImageID") %>' onclick="showTexBox(this.id)" style="display: inline;"><%# Eval("Caption").ToString() == string.Empty ? PageContext.Localization.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : HtmlEncode(Eval("Caption"))%></span>
                                    <input type="text" id='<%# "txtTitle" + Eval("ImageID") %>' onkeydown="checkKey(event, this,'<%#Eval("ImageID") %>',false)"
                                        onblur="blurTextBox(this.id, '<%# Eval("ImageID")%>', false)" style="display: none;" />
                                    <asp:Button ID="SetCover" runat="server" CssClass="pbutton" CommandArgument='<%# Eval("ImageID") %>'
                                        Visible='<%# UserID == PageContext.PageUserID %>' />
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
