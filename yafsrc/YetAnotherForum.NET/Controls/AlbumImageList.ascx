<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumImageList"
    CodeBehind="AlbumImageList.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Core.Context.Start" %>
<%@ Import Namespace="YAF.Core.Utilities" %>


<div class="bg-light-subtle">
    <div class="container">
        <asp:Repeater runat="server" ID="AlbumImages"
                  OnItemDataBound="AlbumImages_ItemDataBound"
                  OnItemCommand="AlbumImages_ItemCommand">
            <HeaderTemplate>
                 <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 g-3">
             </HeaderTemplate>
             <FooterTemplate>
                 </div>
             </FooterTemplate>
             <ItemTemplate>
                  <div class="col">
                      <div class="card mb-4 shadow-sm">
                          <a href='<%# "{0}resource.ashx?image={1}".FormatWith(BoardInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                             title='<%#  this.HtmlEncode(this.Eval("Caption")) %>'
                             data-toggle="lightbox" data-gallery='<%# this.UserAlbum.ID %>' data-caption='<%# this.HtmlEncode(this.Eval("Caption"))%>'>
                              <img src='<%# "{0}resource.ashx?imgprv={1}".FormatWith(BoardInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                                   class="card-img-top"
                                   alt='<%# this.HtmlEncode(this.Eval("Caption"))%>'
                                   title='<%# this.HtmlEncode(this.Eval("Caption"))%>' />
                          </a>
                          <div class="card-body">
                              <asp:Label runat="server"
                                         Visible="<%# this.UserID != this.PageBoardContext.PageUserID %>"
                                         CssClass="card-text">
                                  <%# this.HtmlEncode(this.Eval("Caption"))%>
                              </asp:Label>
                              <asp:Label runat="server"
                                         Visible="<%# this.UserID == this.PageBoardContext.PageUserID %>"
                                         CssClass="card-text mb-3">
                                  <YAF:Icon runat="server" IconName="pen" IconType="text-secondary"/>
                                  <a class="album-image-caption border-bottom border-danger border-3" data-type="text" 
                                     id='<%# this.Eval("ID") %>' 
                                     data-id='<%# this.Eval("ID") %>' 
                                     data-url='<%# "{0}{1}/Album/ChangeImageCaption".FormatWith(BoardInfo.ForumClientFileRoot, WebApiConfig.UrlPrefix) %>'
                                     data-title='<%#  this.GetText(this.Eval("Caption") == null ? "ALBUM_IMAGE_CHANGE_CAPTION" : "ALBUM_IMAGE_CHANGE_CAPTION2") %>'><%# this.Eval("Caption").IsNullOrEmptyField() ? this.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : this.HtmlEncode(this.Eval("Caption"))%></a>
                              </asp:Label>
                              <div class="d-flex justify-content-between align-items-center mt-1">
                                  <div class="btn-group">
                                      <YAF:ThemeButton ID="SetCover" runat="server"
                                                       CommandArgument='<%# this.Eval("ID") %>'
                                                       Size="Small"
                                                       Type="OutlineSecondary"
                                                       Visible="<%# this.UserID == this.PageBoardContext.PageUserID %>"/>
                                  </div>
                                  <div class="float-end"><a href="javascript:<%# JavaScriptBlocks.BootBoxPromptJs("Album IMG Code", "", this.GetText("CANCEL"), this.GetText("OK"), "[ALBUMIMG]{0}[/ALBUMIMG]".FormatWith(this.UserAlbum.ID)) %>" role="button" type="button"><YAF:Icon runat="server" IconName="share-from-square" IconType="text-secondary"/></a></div>
                              </div>
                          </div>
                      </div>
                  </div>
             </ItemTemplate>
        </asp:Repeater>
    <div class="row justify-content-end">
        <div class="col-auto">
        <YAF:Pager runat="server" ID="PagerTop"
                   OnPageChange="Pager_PageChange" />
            </div>
        </div>
    </div>
</div>