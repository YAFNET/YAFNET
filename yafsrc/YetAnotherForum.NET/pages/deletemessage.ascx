<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.DeleteMessage" Codebehind="DeleteMessage.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><asp:Label ID="Title" runat="server" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <span class="fa-stack fa-1x">
                    <i class="fas fa-comment fa-stack-2x"></i>
                    <i class="fas fa-trash fa-stack-1x fa-inverse"></i>
                </span>&nbsp; 
                <YAF:LocalizedLabel runat="server" LocalizedTag="subject" />&nbsp;<asp:Label runat="server" ID="Subject" />
            </div>
            <div class="card-body">
                <form>
                    <asp:PlaceHolder runat="server" id="PreviewRow" visible="false">
                    <div class="form-group">
                        <asp:Label runat="server">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
                        </asp:Label>
                        <div class="card">
                            <div class="card-body">
                                <YAF:MessagePost ID="MessagePreview" runat="server">
                                </YAF:MessagePost>
                            </div>
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" id="DeleteReasonRow">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="ReasonEditor">
                            <% = this.GetReasonText() %>
                        </asp:Label>
                        <asp:TextBox ID="ReasonEditor" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder id="EraseRow" runat="server" visible="false">
                        <div class="form-group">
                            
                            <asp:CheckBox ID="EraseMessage" runat="server" 
                                          Checked="false"
                                          CssClass="custom-control custom-checkbox"/>
                        </div>
                    </asp:PlaceHolder>
                </form>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="Delete" runat="server" 
                                 OnClick="ToogleDeleteStatus_Click"
                                 Type="Danger"
                                 Icon="trash"/>
                <YAF:ThemeButton ID="Cancel" runat="server" 
                                 OnClick="Cancel_Click"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary"
                                 Icon="reply"/>
            </div>
        </div>
    </div>
</div>

<asp:Repeater ID="LinkedPosts" runat="server" Visible="false">
    <HeaderTemplate>
        <div class="row">
        <div class="col">
        <div class="card mb-3">
        <div class="card-header">
            <span class="fa-stack fa-1x">
                <i class="fas fa-comment fa-stack-2x"></i>
                <i class="fas fa-trash fa-stack-1x fa-inverse"></i>
            </span>&nbsp; 
            <asp:CheckBox ID="DeleteAllPosts" runat="server" 
                          CssClass="custom-control custom-checkbox d-inline-block" Text='<%# this.GetText("DELETE_ALL") %>' />
            
        </div>
        <div class="card-body">
            <ul class="list-group">
    </HeaderTemplate>
    <FooterTemplate>
            </ul>
        </div>
        </div>
        </div>
        </div>
    </FooterTemplate>
    <ItemTemplate>
        <li class="list-group-item">
            <p class="pb-1"><span class="font-weight-bold"><a href="<%# BuildLink.GetLink(ForumPages.Profile,"u={0}&name={1}",DataBinder.Eval(Container.DataItem, "UserID"), DataBinder.Eval(Container.DataItem, "UserName")) %>">
                    <%# DataBinder.Eval(Container.DataItem, "UserName") %></a></span>
                <span class="font-weight-bold">
                    <YAF:LocalizedLabel runat="server" LocalizedTag="posted" />
                </span>
                <%# this.Get<IDateTime>().FormatDateTime( ( DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>#
            </p>
                <YAF:MessagePostData ID="MessagePost1" runat="server" 
                                     DataRow="<%# ((System.Data.DataRowView)Container.DataItem).Row %>"
                                     ShowAttachments="false" 
                                     ShowSignature="false">
                </YAF:MessagePostData>
        </li>
    </ItemTemplate>
</asp:Repeater>