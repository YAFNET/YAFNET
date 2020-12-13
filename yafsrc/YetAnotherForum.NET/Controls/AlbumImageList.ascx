<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumImageList"
    CodeBehind="AlbumImageList.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>


<div class="bg-light">
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
                          <a href='<%# "{0}resource.ashx?image={1}".Fmt(BoardInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                             title='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : "{0}&lt;br /&gt; Album IMG Code: [ALBUMIMG]{1}[/ALBUMIMG]".Fmt(this.HtmlEncode(this.Eval("Caption")), this.UserAlbum.ID) %>'
                             data-gallery>
                              <img src='<%# "{0}resource.ashx?imgprv={1}".Fmt(BoardInfo.ForumClientFileRoot, this.Eval("ID")) %>'
                                   class="card-img-top"
                                   alt='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption"))%>' 
                                   title='<%# this.Eval("Caption") == null ? this.HtmlEncode(this.Eval("FileName")) : this.HtmlEncode(this.Eval("Caption"))%>' />
                          </a>
                          <div class="card-body">
                              <asp:Label runat="server" 
                                         Visible="<%# this.UserID != this.PageContext.PageUserID %>"
                                         CssClass="card-text">
                                  <%# this.HtmlEncode(this.Eval("Caption"))%>
                              </asp:Label>
                              <asp:Label runat="server"
                                         Visible="<%# this.UserID == this.PageContext.PageUserID %>"
                                         CssClass="card-text mb-3">
                                  <span id='<%# "spnTitle{0}".Fmt(this.Eval("ID")) %>'
                                        onclick="showTexBox(this.id)"
                                        title='<%# this.Eval("Caption") == null ? this.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : this.GetText("ALBUM_IMAGE_CHANGE_CAPTION2") %>'
                                        data-bs-toggle="tooltip">
                                      <YAF:Icon runat="server" IconName="pen" IconType="text-secondary"/>
                                      <%# this.Eval("Caption").IsNullOrEmptyField() ? this.GetText("ALBUM_IMAGE_CHANGE_CAPTION") : this.HtmlEncode(this.Eval("Caption"))%>
                                  </span>
                                  <input type="text" id='<%# "txtTitle{0}".Fmt(this.Eval("ID")) %>' 
                                         class="form-control"
                                         onkeydown="checkKey(event, this,'<%# this.Eval("ID") %>',false)"
                                         onblur="blurTextBox(this.id, '<%# this.Eval("ID")%>', false)" style="display: none;" />
                              </asp:Label>
                              <div class="d-flex justify-content-between align-items-center">
                                  <div class="btn-group">
                                      <YAF:ThemeButton ID="SetCover" runat="server" 
                                                       CommandArgument='<%# this.Eval("ID") %>'
                                                       Size="Small"
                                                       Type="OutlineSecondary"
                                                       Visible="<%# this.UserID == this.PageContext.PageUserID %>"/>
                                  </div>
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