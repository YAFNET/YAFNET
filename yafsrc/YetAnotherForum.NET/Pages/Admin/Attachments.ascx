﻿<%@ Control Language="c#" AutoEventWireup="True" EnableViewState="true" Inherits="YAF.Pages.Admin.Attachments"
    CodeBehind="Attachments.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
                <div class="card-header">
                    <div class="row justify-content-between align-items-center">
                        <div class="col-auto">
                            <YAF:IconHeader runat="server"
                                            IconType="text-secondary"
                                            IconName="paperclip"
                                            LocalizedPage="ADMIN_ATTACHMENTS"></YAF:IconHeader>
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
                            <%# this.GetPreviewImage(((Tuple<User,Attachment>)Container.DataItem).Item2) %>
                            <%# ((Tuple<User,Attachment>)Container.DataItem).Item2.FileName  %>
                        </h5>
                        <small class="d-none d-md-block">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="USAGES"></YAF:LocalizedLabel>
                            <span class="badge text-bg-secondary">
                                <%# this.Get<ISearch>().CountHits(string.Format("]{0}[", ((Tuple<User,Attachment>)Container.DataItem).Item2.ID))  %>
                            </span>
                        </small>
                    </div>
                    <p><strong><YAF:LocalizedLabel runat="server" LocalizedTag="FROM" LocalizedPage="DELETEMESSAGE"></YAF:LocalizedLabel></strong>
                        <%# this.UserLink(((Tuple<User,Attachment>)Container.DataItem).Item1) %>
                    </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <a href="<%# this.Get<LinkBuilder>().GetLink(
                                             ForumPages.Search,
                                             new { search = string.Format("]{0}[", ((Tuple<User,Attachment>)Container.DataItem).Item2.ID) }) %>"
                               class="btn btn-info"><YAF:LocalizedLabel runat="server" LocalizedTag="SHOW_TOPICS"></YAF:LocalizedLabel></a>
                            <YAF:ThemeButton runat="server"
                                             Type="Danger"
                                             Size="Small"
                                             CommandName="delete" CommandArgument="<%# ((Tuple<User,Attachment>)Container.DataItem).Item2.ID %>"
                                             Icon="trash"
                                             ReturnConfirmTag="CONFIRM_DELETE"
                                             TextLocalizedTag="DELETE">
                            </YAF:ThemeButton>
                        </div>
                    </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <a href="<%# this.Get<LinkBuilder>().GetLink(
                                         ForumPages.Search,
                                         new { search = string.Format("]{0}[", ((Tuple<User,Attachment>)Container.DataItem).Item2.ID) }) %>"
                           class="dropdown-item"><YAF:LocalizedLabel runat="server" LocalizedTag="SHOW_TOPICS"></YAF:LocalizedLabel></a>
                        <YAF:ThemeButton runat="server"
                                         Type="None"
                                         CssClass="dropdown-item"
                                         CommandName="delete" CommandArgument="<%# ((Tuple<User,Attachment>)Container.DataItem).Item2.ID %>"
                                         ReturnConfirmTag="CONFIRM_DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE">
                        </YAF:ThemeButton>
                    </div>
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