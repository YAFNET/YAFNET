<%@ Control Language="C#" AutoEventWireup="True" EnableViewState="false" CodeBehind="SimilarTopics.ascx.cs"
    Inherits="YAF.Controls.SimilarTopics" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="System.ComponentModel" %>
<%@ Import Namespace="YAF.Core" %>

<asp:PlaceHolder id="SimilarTopicsHolder" runat="server" Visible="true">
    <asp:Repeater ID="Topics" runat="server" Visible="true">
        <HeaderTemplate>
            <table class="content" width="100%">
                <tr>
                    <td class="header2">
                        <YAF:LocalizedLabel runat="server" LocalizedPage="POSTS" LocalizedTag="SIMILAR_TOPICS"></YAF:LocalizedLabel>
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
               <tr>
                <td class="post">
                   <a href="<%# YafBuildLink.GetLink(ForumPages.posts, "t={0}", DataBinder.Eval(Container.DataItem, "TopicID"))%>"
                       class="post_link">
                       <strong><%# this.Get<IBadWordReplace>().Replace(this.HtmlEncode(DataBinder.Eval(Container.DataItem, "Topic"))) %></strong>
                   </a> (<a href="<%# YafBuildLink.GetLink(ForumPages.forum, "f={0}", DataBinder.Eval(Container.DataItem, "ForumID"))%>"><%# DataBinder.Eval(Container.DataItem, "ForumName") %></a>)
                   <br/>
                   <YAF:LocalizedLabel runat="server" LocalizedPage="SEARCH" LocalizedTag="BY"></YAF:LocalizedLabel> <YAF:UserLink ID="UserName" runat="server" 
                       UserID='<%# DataBinder.Eval(Container.DataItem, "UserID") %>' 
                       ReplaceName='<%# DataBinder.Eval(Container.DataItem, this.Get<YafBoardSettings>().EnableDisplayName ? "StarterDisplayName" : "StarterName") %>' 
                       Style='<%# DataBinder.Eval(Container.DataItem, "StarterStyle") %>'>
                      </YAF:UserLink>
                      <YAF:DisplayDateTime ID="CreatedDate" runat="server"
                          Format="BothTopic" DateTime='<%# DataBinder.Eval(Container.DataItem, "Posted") %>'>
                      </YAF:DisplayDateTime>

                   

                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
           </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>