<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumImageList"
    CodeBehind="AlbumImageList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<asp:Literal ID="ltrTitleOnly" runat="server"></asp:Literal>
<span runat="server" id="spnAlbumOwner" visible='<%# UserID == PageContext.PageUserID %>'>
    <span id='<%= "spnTitle0" + AlbumID %>' class="albumtitle" onclick="showTexBox(this.id)"
        style="display: inline;">
        <asp:Literal ID="ltrTitle" runat="server"></asp:Literal></span>
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
<asp:Repeater runat="server" ID="AlbumImages" OnItemDataBound="AlbumImages_ItemDataBound"
    OnItemCommand="AlbumImages_ItemCommand">
    <HeaderTemplate>
        <div class="fileattach ceebox">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="attachedimg" style="display: inline;">
            <table class="albumtable" style="display: inline" width='<%# this.Get<YafBoardSettings>().ImageAttachmentResizeWidth %>'>
                <tr>
                    <td class="albumimagebox">
                        <a href='<%# "{0}resource.ashx?image={1}".FormatWith(YafForumInfo.ForumClientFileRoot, this.Eval("ImageID")) %>'
                            title='<%# Eval("Caption").ToString() == string.Empty ? this.HtmlEncode(Eval("FileName")) : this.HtmlEncode(Eval("Caption")) + "&lt;br /&gt; Album IMG Code: [ALBUMIMG]" + AlbumID + "[/ALBUMIMG]"%>' 
                            title='<%# Eval("Caption").ToString() == string.Empty ? this.HtmlEncode(Eval("FileName")) : this.HtmlEncode(Eval("Caption")) + "&lt;br /&gt; Album IMG Code: [ALBUMIMG]" + AlbumID + "[/ALBUMIMG]"%>'>
                            <img src='<%# "{0}resource.ashx?imgprv={1}".FormatWith(YafForumInfo.ForumClientFileRoot, this.Eval("ImageID")) %>'
                                alt='<%# Eval("Caption").ToString() == string.Empty ? this.HtmlEncode(Eval("FileName")) : this.HtmlEncode(Eval("Caption"))%>' title='<%# Eval("Caption").ToString() == string.Empty ? this.HtmlEncode(Eval("FileName")) : this.HtmlEncode(Eval("Caption"))%>' />
                        </a>
                    </td>
                </tr>
                <tr>
                    <td class="albumtitlebox">
                        <span runat="server" id="spnUser" visible='<%# UserID != PageContext.PageUserID %>'>
                            <%# this.HtmlEncode(Eval("Caption"))%></span> <span runat="server" id="spnImageOwner"
                                visible='<%# UserID == PageContext.PageUserID %>'><span class="albumtitle" id='<%# "spnTitle" + Eval("ImageID") %>'
                                    onclick="showTexBox(this.id)" style="display: inline;"><%# Eval("Caption").ToString() == string.Empty ? this.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : this.HtmlEncode(Eval("Caption"))%></span>
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
