<%@ Control Language="c#" CodeBehind="../../../controls/TopicLine.ascx.cs" AutoEventWireup="True" Inherits="YAF.Controls.TopicLine" %>
<tr class="<%=this.IsAlt ? "topicRow_Alt post_alt" : "topicRow post" %>">
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

            string linkParams = "t={0}";
            if (this.FindUnread)
            {
                linkParams += "&find=unread";
            }
        %>
        <a href="<%=YafBuildLink.GetLink(YAF.Classes.ForumPages.posts, linkParams, this.TopicRow["LinkTopicID"])%>"
            class="post_link" title="<%=YafFormatMessage.GetCleanedTopicMessage(this.TopicRow["FirstMessage"], this.TopicRow["LinkTopicID"]).MessageTruncated%>">
            <%=this.Get<IBadWordReplace>().Replace(Convert.ToString(this.HtmlEncode(this.TopicRow["Subject"])))%></a>
        <%
            var favoriteCount = this.Get<IFavoriteTopic>().FavoriteTopicCount((int)this.TopicRow["LinkTopicID"]);
            
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
          Style = this.TopicRow["StarterStyle"].ToString()
        }.RenderToString() %>
        </span>
        <%    
            if (this.ShowTopicPosted)
            {
        %>
        <span class="topicPosted">,
            <%= new DisplayDateTime() { Format = YAF.Classes.DateTimeFormat.BothTopic, DateTime = this.TopicRow["Posted"] }.RenderToString()%>
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
        %>
        <span class="topicPager smallfont">-
            <%=this.PageContext.Localization.GetText("GOTO_POST_PAGER").FormatWith(tPager) %></span>
        <%
      }      
        %>
    </td>
</tr>