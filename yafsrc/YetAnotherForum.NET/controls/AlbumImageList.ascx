<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumImageList"
    CodeBehind="AlbumImageList.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Core.Extensions" %>

<div class="card-header">
    <asp:Literal ID="ltrTitleOnly" runat="server"></asp:Literal><asp:Literal ID="ltrTitle" runat="server"></asp:Literal>
</div>
<div class="card-body">
    <YAF:ThemeButton runat="server" ID="EditAlbums"
                     OnClick="EditAlbums_Click" 
                     TextLocalizedTag="BUTTON_EDITALBUMIMAGES"
                     Icon="plus"/>

    <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />

    <asp:Repeater runat="server" ID="AlbumImages" 
                  OnItemDataBound="AlbumImages_ItemDataBound"
                  OnItemCommand="AlbumImages_ItemCommand">
        <HeaderTemplate>
            <div class="row mt-3">
        </HeaderTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    <ItemTemplate>
        <div class="col-sm-4">
        <div class="card mb-3">
            <div class="card-body">
                <div class="card-text mb-3">
                    <a href='<%# "{0}resource.ashx?image={1}".Fmt(BoardInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                       title='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : "{0}&lt;br /&gt; Album IMG Code: [ALBUMIMG]{1}[/ALBUMIMG]".Fmt(this.HtmlEncode(this.Eval("Caption")), this.AlbumID) %>'
                       data-gallery>
                        <img src='<%# "{0}resource.ashx?imgprv={1}".Fmt(BoardInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                             class="img-thumbnail"
                             alt='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption"))%>' 
                             title='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption"))%>' />
                    </a>
                </div>
                <span runat="server" id="spnUser" visible='<%#
    this.UserID != this.PageContext.PageUserID %>'>
                    <%# this.HtmlEncode(this.Eval("Caption"))%></span> <span runat="server" id="spnImageOwner"
                                                                             visible='<%#
    this.UserID == this.PageContext.PageUserID %>'><span id='<%# "spnTitle{0}".Fmt(this.Eval("ID")) %>'
                                                         onclick="showTexBox(this.id)" style="display: inline;"><%# this.Eval("Caption") == null ? this.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : this.HtmlEncode(this.Eval("Caption"))%></span>
                    <input type="text" id='<%# "txtTitle{0}".Fmt(this.Eval("ID")) %>' onkeydown="checkKey(event, this,'<%#
    this.Eval("ID") %>',false)"
                           onblur="blurTextBox(this.id, '<%# this.Eval("ID")%>', false)" style="display: none;" />
                    <YAF:ThemeButton ID="SetCover" runat="server" 
                                     CommandArgument='<%# this.Eval("ID") %>'
                                     Size="Small"
                                     Visible='<%# this.UserID == this.PageContext.PageUserID %>'/>
                </span>
            </div>
        </div>
        </div>
        </ItemTemplate>
    </asp:Repeater>

    <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
</div>
