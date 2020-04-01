<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.restore" CodeBehind="restore.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Types.Extensions" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1>
            <YAF:HelpLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="TITLE" 
                                LocalizedPage="ADMIN_RESTORE" />
        </h1>
    </div>
</div>

<div class="row">
    <div class="col-xl-12">
        <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
        <div class="card mb-3">
            <div class="card-header">
                <YAF:Icon runat="server"
                          IconName="trash-restore pr-1"
                          IconType="text-secondary"></YAF:Icon>
                <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                               LocalizedTag="TITLE" 
                               LocalizedPage="ADMIN_RESTORE" />
                <div class="float-right">
                    <YAF:ThemeButton runat="server"
                                     CssClass="dropdown-toggle"
                                     DataToggle="dropdown"
                                     Type="Secondary"
                                     Icon="filter"
                                     TextLocalizedTag="FILTER_DROPDOWN"
                                     TextLocalizedPage="ADMIN_USERS"></YAF:ThemeButton>
                    <div class="dropdown-menu">
                        <div class="px-3 py-1">
                            <div class="form-group">
                                <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                               AssociatedControlID="SearchInput"
                                               LocalizedTag="FILTER" LocalizedPage="ADMIN_RESTORE" />
                                <asp:TextBox runat="server" ID="Filter"
                                             CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <YAF:ThemeButton runat="server" 
                                                 Icon="sync-alt"
                                                 Type="Primary"
                                                 TextLocalizedTag="SEARCH"
                                                 TextLocalizedPage="ADMIN_RESTORE"
                                                 CssClass="btn-block"
                                                 OnClick="RefreshClick"></YAF:ThemeButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="card-body">
                <asp:Repeater runat="server" ID="DeletedTopics" OnItemCommand="List_ItemCommand">
                    <HeaderTemplate>
                        <ul class="list-group">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li class="list-group-item list-group-item-action">
                            <asp:HiddenField ID="hiddenID" runat="server" Value='<%# this.Eval("ID") %>' />
                            <div class="d-flex w-100 justify-content-between">
                                <h5 class="mb-1">
                                    <%# this.Eval("TopicName") %>
                                    <YAF:ThemeButton runat="server" ID="ThemeButton1"
                                                     Type="Link"
                                                     Icon="external-link-alt"
                                                     Visible='<%# this.Eval("NumPosts").ToType<int>() > 0 %>'
                                                     NavigateUrl='<%# BuildLink.GetLink(ForumPages.Posts, "t={0}", this.Eval("ID")) %>'>
                                    </YAF:ThemeButton>
                                </h5>
                                <small><%# "{0} {1}".Fmt(this.Eval("NumPosts"), this.GetText("POSTS")) %></small>
                            </div>
                            <small>
                                <div class="btn-group">
                                    <YAF:ThemeButton runat="server" ID="Restore"
                                                     TextLocalizedTag="RESTORE_TOPIC"
                                                     Icon="trash-restore"
                                                     Type="Success"
                                                     Visible='<%# this.Eval("NumPosts").ToType<int>() > 0 %>'
                                                     CommandName="restore"
                                                     CommandArgument='<%# this.Eval("ID") %>'></YAF:ThemeButton>
                                    <YAF:ThemeButton runat="server" ID="Delete"
                                                     TextLocalizedTag="DELETE"
                                                     Type="Danger"
                                                     Icon="trash"
                                                     CommandName="delete"
                                                     CommandArgument='<%# this.Eval("ID") %>'>
                                    </YAF:ThemeButton>
                                </div>
                            </small>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                        </div>
                        <div class="card-footer text-center">
                            <YAF:ThemeButton runat="server" 
                                             CommandName="delete_all" 
                                             ID="Linkbutton4" 
                                             Type="Danger"
                                             Icon="dumpster" 
                                             TextLocalizedTag="DELETE_ALL"
                                             TextLocalizedPage="ADMIN_EVENTLOG">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton runat="server" 
                                             CommandName="delete_zero" 
                                             ID="ThemeButton2" 
                                             Type="Danger"
                                             Icon="dumpster" 
                                             TextLocalizedTag="DELETE_ALL_ZERO"
                                             TextLocalizedPage="ADMIN_RESTORE">
                            </YAF:ThemeButton>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                    </div>
            <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
            
    </div>
</div>