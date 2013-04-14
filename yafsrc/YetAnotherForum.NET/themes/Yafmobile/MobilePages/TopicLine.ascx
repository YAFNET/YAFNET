<%@ Control Language="c#" CodeBehind="../../../controls/TopicLine.ascx.cs" AutoEventWireup="True" Inherits="YAF.Controls.TopicLine" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Controls" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<tr class="<%=this.IsAlt ? "topicRow_Alt post_alt" : "topicRow post" %>">
     <asp:PlaceHolder ID="SelectionHolder" runat="server" Visible="false">
        <td>
            <asp:CheckBox ID="chkSelected" runat="server" />
        </td>
    </asp:PlaceHolder>
    <td class="topicImage">
        <%  string imgTitle = string.Empty;
            string imgSrc = this.GetTopicImage(this.TopicRow, ref imgTitle);
        %>
        <img src="<%=imgSrc%>" alt="<%=imgTitle%>" title="<%=imgTitle%>" />
    </td>
    <td class="topicMain">
        <%
            string priorityMessage = this.GetPriorityMessage(this.TopicRow);
            if (priorityMessage.IsSet())
            {
        %>
        <span class="post_priority">
            <%=priorityMessage %></span>
        <%
            }
            
        %>

        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, "m={0}&find=unread", this.TopicRow["LastMessageID"])%>"
            class="post_link" title="<%=this.Get<IFormatMessage>().GetCleanedTopicMessage(this.TopicRow["FirstMessage"], this.TopicRow["LinkTopicID"]).MessageTruncated%>">
            <% if (this.TopicRow["Status"].ToString().IsSet() && this.Get<YafBoardSettings>().EnableTopicStatus)
               {%>
                   <%=string.Format("[{0}] {1}", this.GetText("TOPIC_STATUS", this.TopicRow["Status"].ToString()), this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Subject"])))%>
               <%}
            else
                {%>
                   <%=this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Subject"]))%>
               <%}
            %></a>
        <%
            var favoriteCount = this.TopicRow["FavoriteCount"].ToType<int>();
            
            if (favoriteCount > 0)
            {
        %>
        <span class="topicFavoriteCount">[+<%=favoriteCount%>]</span>
        <%
            }
        
      %>            
        <br />
        <span class="topicStarter">
            <%= new UserLink
        {
          ID = "topicStarterLink",
          UserID = this.TopicRow["UserID"].ToType<int>(),
          ReplaceName = this.Get<YafBoardSettings>().EnableDisplayName ? this.TopicRow["StarterDisplay"].ToString() : this.TopicRow["Starter"].ToString(),
          Style = this.TopicRow["StarterStyle"].ToString()
        }.RenderToString() %>
        </span>
        <%    
            if (this.ShowTopicPosted)
            {
        %>
        <span class="topicPosted">,
            <%= new DisplayDateTime() { Format = DateTimeFormat.BothTopic, DateTime = this.TopicRow["Posted"] }.RenderToString()%>
        </span>            
        <%
            }
    
            int actualPostCount = this.TopicRow["Replies"].ToType<int>() + 1;

            if (this.PageContext.BoardSettings.ShowDeletedMessages)
            {
                // add deleted posts not included in replies...
                actualPostCount += this.TopicRow["NumPostsDeleted"].ToType<int>();
            }     

      string tPager = this.CreatePostPager(
        actualPostCount, this.PageContext.BoardSettings.PostsPerPage, this.TopicRow["LinkTopicID"].ToType<int>());

      if (tPager != String.Empty)
      {
           string altMultipages = this.GetText("GOTO_POST_PAGER").FormatWith(string.Empty);
        %>
        <span class="topicPager smallfont">- <img src="<%=this.Get<ITheme>().GetItem(
          "ICONS","MULTIPAGES_SMALL")%>" alt="<%=altMultipages%>" title="<%=altMultipages%>" />  
            <%=tPager%></span>
        <%
      }      
        %>
    </td>
</tr>