<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.tags"
    CodeBehind="Tags.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Types.Constants" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="tags"
                                    LocalizedPage="ADMIN_TAGS"></YAF:IconHeader>
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
                            <YAF:Icon runat="server" IconName="tag"></YAF:Icon>
                            <%# ((Tag)Container.DataItem).TagName  %>
                        </h5>
                        <small class="d-none d-md-block">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="USAGES"></YAF:LocalizedLabel>
                            <span class="badge badge-secondary">
                                <%# this.GetRepository<TopicTag>().Count(x => x.TagID == ((Tag)Container.DataItem).ID)  %>
                            </span>
                        </small>
                    </div>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <a href="<%# BuildLink.GetLink(
                                             ForumPages.Search,
                                             "tag={0}",
                                             this.Eval("TagName")) %>"
                               class="btn btn-info btn-sm"><YAF:LocalizedLabel runat="server" LocalizedTag="SHOW_TOPICS"></YAF:LocalizedLabel></a>
                            <YAF:ThemeButton runat="server"
                                             Type="Danger"
                                             Size="Small"
                                             CommandName="delete" CommandArgument="<%# ((Tag)Container.DataItem).ID %>"
                                             ReturnConfirmText='<%# this.GetText("ADMIN_EVENTLOG", "CONFIRM_DELETE") %>'
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