<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.AlbumList"
    CodeBehind="AlbumList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-images fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="AlbumHeaderLabel" runat="server" 
                                                                                  LocalizedTag="ALBUMS_HEADER_TEXT" />
            </div>
            <div class="card-body">
                <YAF:ThemeButton ID="AddAlbum" runat="server" 
                                 Visible="false" OnClick="AddAlbum_Click"
                                 Type="Primary"
                                 Icon="images"/>

                <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
                <asp:Repeater runat="server" ID="Albums" OnItemCommand="Albums_ItemCommand" OnItemDataBound="Albums_ItemDataBound">
                    <HeaderTemplate>
                        <div class="row">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                    <ItemTemplate>
                        <div class="col-md-4">
                            <div class="card mb-4 shadow-sm" 
                                 style="width:400px">
                                <a href='<%# YafBuildLink.GetLink(ForumPages.album, "u={0}&a={1}", this.Eval("UserID"), this.Eval("AlbumID")) %>'
                                   target="_parent" title='<%# this.HtmlEncode(this.Eval("Title"))%>'>
                                <asp:Image runat="server" ID="coverImage" ImageUrl='<%# "{0}resource.ashx?album={1}&cover={2}".FormatWith(YafForumInfo.ForumClientFileRoot, this.Eval("AlbumID"), (this.Eval("CoverImageID").ToString() == string.Empty ? "0" : this.Eval("CoverImageID"))) %>'
                                           ToolTip='<%# this.HtmlEncode(this.Eval("Title")) %>' runat="server" 
                                           AlternateText='<%# this.Eval("AlbumID") %>'
                                           CssClass="card-img-top"/>
                                <div class="card-body">
                                    <div class="card-text">
                                        <div class="d-flex justify-content-between align-items-center">
                                            <YAF:ThemeButton ID="Edit" CssClass="pbutton" 
                                                             TextLocalizedTag="EDIT"
                                                             TextLocalizedPage="BUTTON"
                                                             Visible='<%#
    this.UserID == this.PageContext.PageUserID %>' runat="server" CommandName="edit"
                                                             CommandArgument='<%# this.Eval("AlbumID") %>'
                                                             Size="Small"
                                                             Type="OutlineSecondary"/>
                                            <small class="text-muted">
                                                <%# this.HtmlEncode(this.Eval("Title"))%>
                                            </small>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        </ItemTemplate>
                    </asp:Repeater>
                <YAF:Pager runat="server" ID="PagerBottom" 
                           LinkedPager="PagerTop" 
                           OnPageChange="Pager_PageChange" />
            </div>
            <div class="card-footer">
                <small class="text-muted">
                    <asp:Label ID="albumsInfo" Visible="false" runat="server"></asp:Label>
                </small>
            </div>
        </div>
    </div>
</div>

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
