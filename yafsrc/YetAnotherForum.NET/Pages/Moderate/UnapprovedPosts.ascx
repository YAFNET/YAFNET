<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Moderate.UnapprovedPosts" CodeBehind="UnapprovedPosts.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Models" %>

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
                        <YAF:IconHeader runat="server"
                                        IconName="comment" 
                                        LocalizedTag="Topic"></YAF:IconHeader>
                        <a id="TopicLink"
                           href='<%# BuildLink.GetLink(ForumPages.Posts, "t={0}&name={1}", ((Tuple<Topic, Message, User>)Container.DataItem).Item1.ID, ((Tuple<Topic, Message, User>)Container.DataItem).Item1.TopicName) %>'
                           runat="server" 
                           Visible="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item1.NumPosts > 0 %>"><%# ((Tuple<Topic, Message, User>)Container.DataItem).Item1.TopicName %></a>
                         <asp:Label id="TopicName" 
                                    runat="server" 
                                    Visible="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item1.NumPosts == 0 %>" 
                                    Text="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item1.TopicName %>"></asp:Label>
                        <div class="float-right text-muted">
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
                            </span>
                            <%# this.Get<IDateTime>().FormatDateTimeShort(((Tuple<Topic, Message, User>)Container.DataItem).Item2.Posted)%>
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedTag="POSTEDBY" LocalizedPage="REPORTPOST" />
                            </span>
                            <YAF:UserLink ID="UserName" runat="server" 
                                          Suspended="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item3.Suspended %>"
                                          Style="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item3.UserStyle %>"
                                          UserID="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item2.UserID %>" />
                            <YAF:ThemeButton ID="AdminUserButton" runat="server" 
                                             Size="Small"
                                             Visible="<%# this.PageContext.IsAdmin %>"
                                             TextLocalizedTag="ADMIN_USER" TextLocalizedPage="PROFILE"
                                             Icon="users-cog" 
                                             Type="Danger"
                                             NavigateUrl='<%# BuildLink.GetLinkNotEscaped( ForumPages.Admin_EditUser,"u={0}", ((Tuple<Topic, Message, User>)Container.DataItem).Item2.UserID ) %>'>
                            </YAF:ThemeButton>
                        </div>
                    </div>
                    <div class="card-body">
                        <%# this.FormatMessage((Tuple<Topic, Message, User>)Container.DataItem)%>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="ApproveBtn" runat="server" 
                                         TextLocalizedPage="MODERATE_FORUM"
                                         TextLocalizedTag="APPROVE" 
                                         CommandName="Approve" CommandArgument="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item2.ID %>"
                                         Icon="check" Type="Success"/>
                        <YAF:ThemeButton ID="DeleteBtn" runat="server"
                                         TextLocalizedPage="MODERATE_FORUM" TextLocalizedTag="DELETE" 
                                         CommandName="Delete" CommandArgument="<%# ((Tuple<Topic, Message, User>)Container.DataItem).Item2.ID %>"
                                         ReturnConfirmText='<%# this.GetText("ASK_DELETE") %>'
                                         Icon="trash" Type="Danger" />
                    </div>
                </div>
            </div>
        </div>
    </ItemTemplate>
</asp:Repeater>