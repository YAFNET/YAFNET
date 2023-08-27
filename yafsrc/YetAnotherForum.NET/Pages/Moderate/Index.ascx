<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Moderate.Index" Codebehind="Index.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Objects.Model" %>
<%@ Import Namespace="ServiceStack.Text" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel runat="server" LocalizedTag="MODERATEINDEX_TITLE" /></h2>
    </div>
</div>
<div class="row">
    <asp:Repeater ID="CategoryList" runat="server" OnItemDataBound="CategoryList_OnItemDataBound">
        <ItemTemplate>
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconType="text-secondary"
                                        IconName="folder"
                                        Text='<%# this.Eval( "Name") %>'
                                        LocalizedPage="ADMIN_BOARDS"></YAF:IconHeader>
                    </div>
                    <div class="card-body text-center">
                        <asp:Repeater ID="ForumList" runat="server" 
                                      OnItemCommand="ForumListItemCommand">
                            <ItemTemplate>
                                <div class="list-group list-group-flush small">
                                    <div class="list-group-item list-group-item-action">
                                        <h5 class="fw-bold">
                                            <%# "{0}{1}".Fmt(((ModerateForum)Container.DataItem).ParentID.HasValue ? "--" : "-", ((ModerateForum)Container.DataItem).Name) %>
                                        </h5>
                                        <YAF:ThemeButton ID="ViewUnapprovedPostsBtn" runat="server" 
                                                         CommandName="viewunapprovedposts" 
                                                         CommandArgument="<%# ((ModerateForum)Container.DataItem).ForumID %>" 
                                                         Visible="<%# ((ModerateForum)Container.DataItem).MessageCount > 0 %>" 
                                                         Type="Secondary"
                                                         Size="Small">
                                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                                                LocalizedTag="UNAPPROVED" />
                                            <span class="badge text-bg-light"><%# ((ModerateForum)Container.DataItem).MessageCount %></span>
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="NoUnapprovedInfo" runat="server"
                                                         TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE" 
                                                         Visible="<%# ((ModerateForum)Container.DataItem).MessageCount == 0 %>" 
                                                         Type="Secondary"
                                                         Enabled="False"
                                                         Size="Small">
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="ViewReportedBtn" runat="server" 
                                                         CommandName="viewreportedposts" 
                                                         CommandArgument="<%# ((ModerateForum)Container.DataItem).ForumID %>" 
                                                         Visible="<%# ((ModerateForum)Container.DataItem).ReportedCount > 0 %>" 
                                                         Type="Secondary"
                                                         Size="Small">
                                            <YAF:LocalizedLabel ID="ReportedCountLabel" runat="server" 
                                                                LocalizedTag="REPORTED" /> 
                                            <span class="badge text-bg-light"><%# ((ModerateForum)Container.DataItem).ReportedCount %></span>
                                        </YAF:ThemeButton>
                                        <YAF:ThemeButton ID="NoReportedInfo" runat="server"
                                                         TextLocalizedTag="NO_POSTS" TextLocalizedPage="MODERATE"
                                                         Visible="<%# ((ModerateForum)Container.DataItem).ReportedCount == 0 %>" 
                                                         Type="Secondary"
                                                         Enabled="False"
                                                         Size="Small">
                                        </YAF:ThemeButton>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
<asp:PlaceHolder id="InfoPlaceHolder" runat="server" Visible="false">
    <YAF:Alert runat="server" Dismissing="False" Type="success">
        <i class="fa fa-check fa-fw me-1"></i>
        <YAF:LocalizedLabel ID="NoCountInfo" 
                            LocalizedTag="NOMODERATION" 
                            LocalizedPage="MODERATE" 
                            runat="server">
        </YAF:LocalizedLabel>
    </YAF:Alert>
</asp:PlaceHolder>