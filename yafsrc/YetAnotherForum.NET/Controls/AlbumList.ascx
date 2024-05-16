<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumList"
    CodeBehind="AlbumList.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Core.Context.Start" %>

<section class="text-center container">
    <div class="row">
        <div class="col-lg-6 col-md-8 mx-auto">
            <h1 class="fw-light-subtle">
                <YAF:LocalizedLabel runat="server" ID="Header"
                                    LocalizedTag="ALBUMS_HEADER_TEXT"
                />
            </h1>
            <p class="lead text-body-secondary">
                <asp:Label ID="albumsInfo" Visible="false" runat="server"></asp:Label>
            </p>
            <p>
                <YAF:ThemeButton ID="AddAlbum" runat="server"
                                 Visible="false" OnClick="AddAlbum_Click"
                                 Type="Primary"
                                 Icon="images"
                                 CssClass="mb-3"/>
            </p>
        </div>
    </div>
</section>

<div class="text-light-emphasis bg-light-subtle">
    <div class="container">
        <asp:Repeater runat="server" ID="Albums"
                      OnItemCommand="Albums_ItemCommand">
            <HeaderTemplate>
                <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 g-3">
            </HeaderTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
            <ItemTemplate>
                <div class="col">
                    <div class="card mb-4 shadow-sm">
                        <a href='<%# this.Get<LinkBuilder>().GetLink(ForumPages.Album, new { u =  this.Eval("UserID"), a = this.Eval("ID") }) %>'
                           target="_parent" title='<%# this.HtmlEncode(this.Eval("Title"))%>' data-bs-toggle="tooltip">
                            <asp:Image runat="server" ID="coverImage"
                                       ImageUrl='<%# "{0}resource.ashx?album={1}&cover={2}".FormatWith(BoardInfo.ForumClientFileRoot, this.Eval("ID"), this.Eval("CoverImageID").ToType<int?>().HasValue ? this.Eval("CoverImageID") : "0") %>'
                                       ToolTip='<%# this.HtmlEncode(this.Eval("Title")) %>'
                                       AlternateText='<%# this.Eval("ID") %>'
                                       CssClass="card-img-top"/>
                        </a>
                        <div class="card-body">
                            <p class="card-text">
                                <asp:Label runat="server"
                                           Visible="<%# this.User.ID != this.PageBoardContext.PageUserID %>"
                                           CssClass="card-text">
                                    <%# this.HtmlEncode(this.Eval("Title"))%>
                                </asp:Label>
                                <asp:Label runat="server"
                                           Visible="<%# this.User.ID == this.PageBoardContext.PageUserID %>"
                                           CssClass="card-text">
                                    <YAF:Icon runat="server" IconName="pen" IconType="text-secondary"/>
                                    <a class="album-caption border-bottom border-danger border-3" data-type="text" 
                                       data-id='<%# this.Eval("ID") %>' 
                                       data-url='<%# "{0}{1}/Album/ChangeAlbumTitle".FormatWith(BoardInfo.ForumClientFileRoot, WebApiConfig.UrlPrefix) %>'
                                       data-title='<%#  this.GetText("ALBUM_CHANGE_TITLE") %>'><%# this.Eval("Title").IsNullOrEmptyField() ? this.GetText("ALBUM_CHANGE_TITLE") : this.HtmlEncode(this.Eval("Title"))%></a>
                                </asp:Label>
                            </p>
                            <div class="d-flex justify-content-between align-items-center">
                                <div class="btn-group">
                                    <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                                     TextLocalizedTag="VIEW"
                                                     NavigateUrl='<%# this.Get<LinkBuilder>().GetLink(ForumPages.Album, new { u =  this.Eval("UserID"), a = this.Eval("ID") }) %>'
                                                     Size="Small"
                                                     Type="OutlineSecondary"/>
                                    <YAF:ThemeButton ID="Edit" runat="server"
                                                     TextLocalizedTag="EDIT"
                                                     TextLocalizedPage="BUTTON"
                                                     Visible="<%# this.User.ID == this.PageBoardContext.PageUserID %>" runat="server"
                                                     CommandName="edit"
                                                     CommandArgument='<%# this.Eval("ID") %>'
                                                     Size="Small"
                                                     Type="OutlineSecondary"/>
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