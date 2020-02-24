<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Moderate.UnapprovedPosts" CodeBehind="UnapprovedPosts.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Extensions" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel runat="server" LocalizedTag="UNAPPROVED" /></h1>
    </div>
</div>
<asp:Repeater ID="List" runat="server">
    <ItemTemplate>
        <div class="row">
            <div class="col-xl-12">
                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-comment fa-fw text-secondary pr-1"></i>
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel
                                ID="TopicLabel" runat="server"
                                LocalizedTag="TOPIC" />
                        </span>
                        <a id="TopicLink"
                           href='<%# BuildLink.GetLink(ForumPages.Posts, "t={0}", this.Eval("TopicID")) %>'
                           runat="server" 
                           Visible='<%# this.Eval("MessageCount").ToType<int>() > 0 %>'><%# this.Eval("Topic") %></a>
                         <asp:Label id="TopicName" 
                                    runat="server" 
                                    Visible='<%# this.Eval("MessageCount").ToType<int>() == 0 %>' 
                                    Text='<%# this.Eval("Topic") %>'></asp:Label>
                        <div class="float-right text-muted">
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                            </span>
                            <%# this.Get<IDateTime>().FormatDateTimeShort(this.Eval("Posted").ToType<DateTime>())%>
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedTag="POSTEDBY" LocalizedPage="REPORTPOST" />
                            </span>
                            <YAF:UserLink ID="UserName" runat="server" UserID='<%# this.Eval("UserID").ToType<int>() %>' />
                            <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                             Size="Small"
                                             Visible="<%# this.PageContext.IsAdmin %>"
                                             TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE"
                                             Icon="users-cog" 
                                             Type="Danger"
                                             NavigateUrl='<%# BuildLink.GetLinkNotEscaped( ForumPages.admin_edituser,"u={0}", this.Eval("UserID").ToType<int>() ) %>'>
                            </YAF:ThemeButton>
                        </div>
                    </div>
                    <div class="card-body">
                        <%#this.FormatMessage((System.Data.DataRowView)Container.DataItem)%>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="ApproveBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM"
                                         TextLocalizedTag="APPROVE" 
                                         CommandName="Approve" CommandArgument='<%# this.Eval("MessageID") %>'
                                         Icon="check" Type="Success"/>
                        <YAF:ThemeButton ID="DeleteBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="DELETE" 
                                         CommandName="Delete" CommandArgument='<%# this.Eval("MessageID") %>'
                                         ReturnConfirmText='<%# this.GetText("ASK_DELETE") %>'
                                         Icon="trash" Type="Danger" />
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>