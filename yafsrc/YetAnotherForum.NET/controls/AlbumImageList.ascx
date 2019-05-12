<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumImageList"
    CodeBehind="AlbumImageList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

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
                    <a href='<%# "{0}resource.ashx?image={1}".FormatWith(YafForumInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                       title='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption")) + "&lt;br /&gt; Album IMG Code: [ALBUMIMG]" + this.AlbumID + "[/ALBUMIMG]"%>'
                       data-gallery>
                        <img src='<%# "{0}resource.ashx?imgprv={1}".FormatWith(YafForumInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                             class="img-fluid img-thumbnail"
                             alt='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption"))%>' 
                             title='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption"))%>' />
                    </a>
                </div>
                <span runat="server" id="spnUser" visible='<%#
    this.UserID != this.PageContext.PageUserID %>'>
                    <%# this.HtmlEncode(this.Eval("Caption"))%></span> <span runat="server" id="spnImageOwner"
                                                                             visible='<%#
    this.UserID == this.PageContext.PageUserID %>'><span id='<%# "spnTitle" + this.Eval("ID") %>'
                                                         onclick="showTexBox(this.id)" style="display: inline;"><%# this.Eval("Caption") == null ? this.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : this.HtmlEncode(this.Eval("Caption"))%></span>
                    <input type="text" id='<%# "txtTitle" + this.Eval("ID") %>' onkeydown="checkKey(event, this,'<%#
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
    <div id="DivSmartScroller">
       
    </div>
</div>
