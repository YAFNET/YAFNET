<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumList"
    CodeBehind="AlbumList.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Configuration" %>

<section class="text-center container">
    <div class="row">
        <div class="col-lg-6 col-md-8 mx-auto">
            <h1 class="fw-light">
                <YAF:LocalizedLabel runat="server" ID="Header"
                                    LocalizedTag="ALBUMS_HEADER_TEXT"
                />
            </h1>
            <p class="lead text-muted">
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

<div class="bg-light">
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
                        <a href='<%# this.Get<LinkBuilder>().GetLink(ForumPages.Album, "u={0}&a={1}", this.Eval("UserID"), this.Eval("ID")) %>'
                           target="_parent" title='<%# this.HtmlEncode(this.Eval("Title"))%>' data-bs-toggle="tooltip">
                            <asp:Image runat="server" ID="coverImage"
                                       ImageUrl='<%# "{0}resource.ashx?album={1}&cover={2}".Fmt(BoardInfo.ForumClientFileRoot, this.Eval("ID"), this.Eval("CoverImageID").ToType<int?>().HasValue ? this.Eval("CoverImageID") : "0") %>'
                                       ToolTip='<%# this.HtmlEncode(this.Eval("Title")) %>'
                                       AlternateText='<%# this.Eval("ID") %>'
                                       CssClass="card-img-top"/>
                        </a>
                        <div class="card-body">
                            <p class="card-text">
                                <%# this.HtmlEncode(this.Eval("Title"))%>
                            </p>
                            <div class="d-flex justify-content-between align-items-center">
                                <div class="btn-group">
                                    <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                                     TextLocalizedTag="VIEW"
                                                     NavigateUrl='<%# this.Get<LinkBuilder>().GetLink(ForumPages.Album, "u={0}&a={1}", this.Eval("UserID"), this.Eval("ID")) %>'
                                                     Size="Small"
                                                     Type="OutlineSecondary"/>
                                    <YAF:ThemeButton ID="Edit" runat="server"
                                                     TextLocalizedTag="EDIT"
                                                     TextLocalizedPage="BUTTON"
                                                     Visible="<%# this.User.ID == this.PageContext.PageUserID %>" runat="server"
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