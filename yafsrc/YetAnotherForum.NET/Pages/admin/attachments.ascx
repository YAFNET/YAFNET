<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.attachments"
    CodeBehind="attachments.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Types.Constants" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="paperclip"
                                    LocalizedPage="ADMIN_ATTACHMENTS"></YAF:IconHeader>
                </div>
                <div class="card-body">
        <asp:Repeater runat="server" ID="List" OnItemCommand="ListItemCommand">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between text-break">
                        <h5 class="mb-1">
                            <%# this.GetPreviewImage(((Tuple<User,Attachment>)Container.DataItem).Item2) %>
                            <%# ((Tuple<User,Attachment>)Container.DataItem).Item2.FileName  %>
                        </h5>
                        <small class="d-none d-md-block">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="USAGES"></YAF:LocalizedLabel>
                            <span class="badge badge-secondary">
                                <%# this.Get<ISearch>().CountHits(string.Format("]{0}[", ((Tuple<User,Attachment>)Container.DataItem).Item2.ID))  %>
                            </span>
                        </small>
                    </div>
                    <p><strong><YAF:LocalizedLabel runat="server" LocalizedTag="FROM" LocalizedPage="DELETEMESSAGE"></YAF:LocalizedLabel></strong>
                        <%# this.UserLink(((Tuple<User,Attachment>)Container.DataItem).Item1) %>
                    </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <a href="<%# BuildLink.GetLink(
                                             ForumPages.Search,
                                             "search={0}",
                                             string.Format("]{0}[", ((Tuple<User,Attachment>)Container.DataItem).Item2.ID)) %>"
                               class="btn btn-info btn-sm mb-1"><YAF:LocalizedLabel runat="server" LocalizedTag="SHOW_TOPICS"></YAF:LocalizedLabel></a>
                            <YAF:ThemeButton runat="server"
                                             Type="Danger"
                                             Size="Small"
                                             CommandName="delete" CommandArgument="<%# ((Tuple<User,Attachment>)Container.DataItem).Item2.ID %>"
                                             Icon="trash"
                                             TextLocalizedTag="DELETE">
                            </YAF:ThemeButton>
                        </div>
                    </small>
                </li>
            </ItemTemplate>
            <FooterTemplate>
               </ul>
            </FooterTemplate>
        </asp:Repeater>
                    <YAF:Alert runat="server" ID="NoInfo"
                               Type="success"
                               Visible="False">
                        <YAF:Icon runat="server" IconName="check" IconType="text-success" />
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NO_ENTRY"></YAF:LocalizedLabel>
                    </YAF:Alert>
                </div>
        </div>
    </div>
</div>
<div class="row justify-content-end">
    <div class="col-auto">
        <YAF:Pager ID="PagerTop" runat="server"
                   OnPageChange="PagerTopPageChange"/>
    </div>
</div>