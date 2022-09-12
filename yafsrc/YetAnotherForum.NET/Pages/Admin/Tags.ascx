<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.Tags"
    CodeBehind="Tags.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Constants" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <div class="row justify-content-between align-items-center">
                        <div class="col-auto">
                            <YAF:IconHeader runat="server"
                                            IconName="tags"
                                            LocalizedPage="ADMIN_TAGS"></YAF:IconHeader>
                            </div>
                            <div class="col-auto">
                                <div class="btn-toolbar" role="toolbar">
                                    <div class="input-group input-group-sm me-2" role="group">
                                        <div class="input-group-text">
                                            <YAF:LocalizedLabel ID="HelpLabel2" runat="server" LocalizedTag="SHOW" />:
                                        </div>
                                        <asp:DropDownList runat="server" ID="PageSize"
                                                          AutoPostBack="True"
                                                          OnSelectedIndexChanged="PageSizeSelectedIndexChanged"
                                                          CssClass="form-select">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                        </div>
            </div>
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
                            <span class="badge bg-secondary">
                                <%# this.GetRepository<TopicTag>().Count(x => x.TagID == ((Tag)Container.DataItem).ID)  %>
                            </span>
                        </small>
                    </div>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <a href="<%# this.Get<LinkBuilder>().GetLink(
                                             ForumPages.Search,
                                             new {tag = this.Eval("TagName")}) %>"
                               class="btn btn-info"><YAF:LocalizedLabel runat="server" LocalizedTag="SHOW_TOPICS"></YAF:LocalizedLabel></a>
                            <YAF:ThemeButton runat="server"
                                             Type="Danger"
                                             Size="Small"
                                             CommandName="delete" CommandArgument="<%# ((Tag)Container.DataItem).ID %>"
                                             ReturnConfirmTag="CONFIRM_DELETE"
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
                        <YAF:Icon runat="server" IconName="check" />
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