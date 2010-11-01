<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicLine.ascx.cs" Inherits="YAF.Controls.TopicLine" %>
<tr class="<%=this.IsAlt ? "topicRow_Alt post_alt" : "topicRow post" %>">
    <td class="topicImage">
        <%  string imgTitle = string.Empty;
            string imgSrc = this.GetTopicImage(this._row, ref imgTitle);
        %>
        <img src="<%=imgSrc%>" alt="<%=imgTitle%>" title="<%=imgTitle%>" />
    </td>
    <td class="topicMain">
        <%
            if (this.PageContext.BoardSettings.ShowAvatarsInTopic)
            {
                var avatarUrl = this.GetAvatarUrlFromID(Convert.ToInt32(this._row["UserID"]));
        %>
        <img src="<%=avatarUrl%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>"
            class="avatarimage" />
        <%}

            string priorityMessage = this.GetPriorityMessage(this._row);
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
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, linkParams, this._row["LinkTopicID"])%>"
            class="post_link" title="<%=YafFormatMessage.GetCleanedTopicMessage(this._row["FirstMessage"], this._row["LinkTopicID"]).MessageTruncated%>">
            <%=this.Get<YafBadWordReplace>().Replace(Convert.ToString(this.HtmlEncode(this._row["Subject"])))%></a>
        <br />
        <span class="topicStarter">
            <%= new UserLink
        {
          ID = "topicStarterLink",
          UserID = this._row["UserID"].ToType<int>(),
          Style = this._row["StarterStyle"].ToString()
        }.RenderToString() %>
        </span>
        <%    
            if (this.ShowTopicPosted)
            {
        %>
        <span class="topicPosted">,
            <%= new DisplayDateTime() {Format = DateTimeFormat.BothTopic, DateTime = this._row["Posted"]}.RenderToString() %>
        </span>            
        <%
            }
    
            int actualPostCount = this._row["Replies"].ToType<int>() + 1;

            if (this.PageContext.BoardSettings.ShowDeletedMessages)
            {
                // add deleted posts not included in replies...
                actualPostCount += this._row["NumPostsDeleted"].ToType<int>();
            }     

      string tPager = this.CreatePostPager(
        actualPostCount, this.PageContext.BoardSettings.PostsPerPage, this._row["LinkTopicID"].ToType<int>());

      if (tPager != String.Empty)
      {
        %>
        <span class="topicPager smallfont">-
            <%=this.PageContext.Localization.GetText("GOTO_POST_PAGER").FormatWith(tPager) %></span>
        <%
      }      
        %>
    </td>
    <td class="topicReplies">
        <%=this.FormatReplies() %>
    </td>
    <td class="topicViews">
        <%=this.FormatViews()%>
    </td>
    <td class="topicLastPost smallfont">
        <%
            if (!_row["LastMessageID"].IsNullOrEmptyDBField())
            {
                int userID = _row["LastUserID"].ToType<int>();

                if (this.PageContext.BoardSettings.ShowAvatarsInTopic)
                {%>
        <img src="<%=this.GetAvatarUrlFromID(userID)%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>"
            class="avatarimage" />
        <%
            }

        string strMiniPost = this.PageContext.Theme.GetItem(
          "ICONS",
          (DateTime.Parse(_row["LastPosted"].ToString()) > YafContext.Current.Get<YafSession>().GetTopicRead((int)this._row["TopicID"]))
            ? "ICON_NEWEST"
            : "ICON_LATEST");
        if (string.IsNullOrEmpty(this.AltLastPost))
        {
            this.AltLastPost = this.PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST");
        }
                
        %>
        <%=new UserLink { UserID = userID, Style = _row["LastUserStyle"].ToString() }.RenderToString() %>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", _row["LastMessageID"]) %>"
            title="<%=this.AltLastPost%>">
            <img src="<%=strMiniPost%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>" />
        </a>
        <br />
        <%=new DisplayDateTime() { Format = DateTimeFormat.BothTopic, DateTime = this._row["LastPosted"] }.RenderToString()%>
        <%
            }    
        %>
    </td>
</tr>
