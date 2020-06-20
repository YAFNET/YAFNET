
<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Moderate.ReportedPosts"CodeBehind="ReportedPosts.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="ServiceStack" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<%@ Register TagPrefix="YAF" TagName="ReportedPosts" Src="../../controls/ReportedPosts.ascx" %>


<div class="row">
    <div class="col-xl-12">
        <h1><%# this.PageContext.PageForumName %> - <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="REPORTED" /></h1>
    </div>
</div>
<asp:Repeater ID="List" runat="server">
    <ItemTemplate>
        <div class="row">
            <div class="col-xl-12">
                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-comment fa-fw text-secondary pr-1"></i>
                        <span class="font-weight-bold"><YAF:LocalizedLabel ID="TopicLabel" runat="server"
                                            LocalizedTag="TOPIC" />
                        </span>
                        <a id="TopicLink" 
                           href='<%# BuildLink.GetLink(ForumPages.Posts, "t={0}", this.Eval("TopicID")) %>'
                           runat="server"><%# this.Eval("Topic") %></a>
                        <div class="float-right text-muted">
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                            </span>
                            <%# this.Get<IDateTime>().FormatDateShort((DateTime) DataBinder.Eval(Container.DataItem, "[\"Posted\"]")) %>
                            <span class="font-weight-bold pl-3">
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NUMBERREPORTED" />
                            </span>
                            <%# DataBinder.Eval(Container.DataItem, "[\"NumberOfReports\"]") %>
                            <span class="font-weight-bold pl-3">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedTag="POSTEDBY" LocalizedPage="REPORTPOST" />
                            </span>
                            <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# this.Eval("UserID").ToType<int>() %>' />
                            <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                             Size="Small"
                                             Visible="<%# this.PageContext.IsAdmin %>"
                                             TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE"
                                             NavigateUrl='<%# BuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.Eval("UserID").ToType<int>() ) %>'
                                             Icon="users-cog" 
                                             Type="Danger">
                            </YAF:ThemeButton>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row">
                            <div class="col">
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="ORIGINALMESSAGE" />
                                </h6>
                                <span id="Label1" runat="server" 
                                      class="badge badge-warning" 
                                      visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'>
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MODIFIED" />
                                </span>
                                <div class="mb-3">
                                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" 
                                                         DataRow="<%# ((System.Data.DataRowView)Container.DataItem).Row %>"
                                                         ShowAttachments="false" 
                                                         ShowEditMessage="False" ShowSignature="False">
                                    </YAF:MessagePostData>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col">
                                <YAF:ReportedPosts ID="ReportersList" runat="server" 
                                                   MessageID='<%# DataBinder.Eval(Container.DataItem, "[\"MessageID\"]") %>'
                                                   ResolvedBy='<%# DataBinder.Eval(Container.DataItem, "[\"ResolvedBy\"]") %>' 
                                                   Resolved='<%# DataBinder.Eval(Container.DataItem, "[\"Resolved\"]") %>'
                                                   ResolvedDate='<%# DataBinder.Eval(Container.DataItem, "[\"ResolvedDate\"]") %>' />
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="CopyOverBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="COPYOVER" 
                                         CommandName="CopyOver" CommandArgument='<%# this.Eval("MessageID") %>'
                                         Visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'
                                         Icon="copy" 
                                         Type="Secondary" />
                        <YAF:ThemeButton ID="DeleteBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="DELETE" 
                                         CommandName="Delete" CommandArgument='<%# this.Eval("MessageID") %>'
                                         ReturnConfirmText='<%# this.GetText("ASK_DELETE") %>'
                                         Icon="trash" 
                                         Type="Danger"/>
                        <YAF:ThemeButton ID="ResolveBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="RESOLVED" 
                                         CommandName="Resolved" CommandArgument='<%# this.Eval("MessageID") %>'
                                         Icon="check" Type="Success" />
                        <YAF:ThemeButton ID="ViewBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="VIEW" 
                                         CommandName="View" CommandArgument='<%# this.Eval("MessageID") %>'
                                         Icon="eye" 
                                         Type="Secondary" />
                        <YAF:ThemeButton ID="ViewHistoryBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="HISTORY" 
                                         Visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'
                                         CommandName="ViewHistory" CommandArgument='<%# "{0},{1}".Fmt(this.PageContext.PageForumID, this.Eval("MessageID")) %>'
                                         Icon="history" 
                                         Type="Secondary" />
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>

