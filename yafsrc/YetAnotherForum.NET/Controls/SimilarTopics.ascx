<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="SimilarTopics.ascx.cs"
    Inherits="YAF.Controls.SimilarTopics" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Types.Objects" %>

<asp:PlaceHolder id="SimilarTopicsHolder" runat="server" Visible="true">
    <asp:Repeater ID="Topics" runat="server" Visible="true">
        <HeaderTemplate>
            <div class="col-md-6">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:LocalizedLabel runat="server" 
                                        LocalizedPage="POSTS" 
                                        LocalizedTag="SIMILAR_TOPICS">
                    </YAF:LocalizedLabel>
                </div>
            <ul class="list-group list-group-flush">
        </HeaderTemplate>
        <ItemTemplate>
            <li class="list-group-item">
                   <a href="<%# ((SearchMessage)Container.DataItem).TopicUrl%>">
                       <strong><%# this.Get<IBadWordReplace>().Replace(this.HtmlEncode(((SearchMessage)Container.DataItem).Topic)) %></strong>
                   </a> (<a href="<%# ((SearchMessage)Container.DataItem).ForumUrl%>"><%# ((SearchMessage)Container.DataItem).ForumName %></a>)
                <YAF:LocalizedLabel runat="server" LocalizedPage="SEARCH" LocalizedTag="BY">
                   </YAF:LocalizedLabel> 
                    <YAF:UserLink ID="UserName" runat="server" 
                                  UserID="<%#((SearchMessage)Container.DataItem).UserId %>" 
                                  ReplaceName="<%# this.PageContext.BoardSettings.EnableDisplayName ? ((SearchMessage)Container.DataItem).UserDisplayName : ((SearchMessage)Container.DataItem).UserName %>" 
                                  Suspended="<%# ((SearchMessage)Container.DataItem).Suspended %>"

                       Style="<%# ((SearchMessage)Container.DataItem).UserStyle %>">
                      </YAF:UserLink>
                <span class="fa-stack">
                    <i class="fa fa-calendar-day fa-stack-1x text-secondary"></i>
                    <i class="fa fa-circle fa-badge-bg fa-inverse fa-outline-inverse"></i>
                    <i class="fa fa-clock fa-badge text-secondary"></i>
                </span>
                <YAF:DisplayDateTime ID="CreatedDate" runat="server"
                                     Format="BothTopic" DateTime="<%# ((SearchMessage)Container.DataItem).Posted %>">
                </YAF:DisplayDateTime>
            </li>
        </ItemTemplate>
        <FooterTemplate>
                </ul>
            </div>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>