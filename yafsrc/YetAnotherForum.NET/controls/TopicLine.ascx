<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicLine.ascx.cs" Inherits="YAF.Controls.TopicLine" %>
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
            if (this.PageContext.BoardSettings.ShowAvatarsInTopic)
            {
                var avatarUrl = this.GetAvatarUrlFromID(Convert.ToInt32(this.TopicRow["UserID"]));
        %>
        <img src="<%=avatarUrl%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>"
            class="avatarimage" />
        <%}

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
            <%=this.Get<YafBadWordReplace>().Replace(Convert.ToString(this.HtmlEncode(this.TopicRow["Subject"])))%></a>
        <%
            var favoriteCount = this.Get<YafFavoriteTopic>().FavoriteTopicCount((int)this.TopicRow["LinkTopicID"]);
            
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
    <td class="topicReplies">
        <%=this.FormatReplies() %>
    </td>
    <td class="topicViews">
        <%=this.FormatViews()%>
    </td>
    <td class="topicLastPost smallfont">
        <%
            if (!this.TopicRow["LastMessageID"].IsNullOrEmptyDBField())
            {
                int userID = this.TopicRow["LastUserID"].ToType<int>();

                if (this.PageContext.BoardSettings.ShowAvatarsInTopic)
                {%>
        <img src="<%=this.GetAvatarUrlFromID(userID)%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>"
            class="avatarimage" />
        <%
            }

        string strMiniPost = this.PageContext.Theme.GetItem(
          "ICONS",
          (DateTime.Parse(this.TopicRow["LastPosted"].ToString()) > YafContext.Current.Get<YafSession>().GetTopicRead((int)this.TopicRow["TopicID"]))
            ? "ICON_NEWEST"
            : "ICON_LATEST");
        if (string.IsNullOrEmpty(this.AltLastPost))
        {
            this.AltLastPost = this.PageContext.Localization.GetText("DEFAULT", "GO_LAST_POST");
        }
                
        %>
        <%=new UserLink { UserID = userID, Style = this.TopicRow["LastUserStyle"].ToString() }.RenderToString() %>
        <a href="<%=YafBuildLink.GetLink(YAF.Classes.ForumPages.posts, "m={0}#post{0}", this.TopicRow["LastMessageID"]) %>"
            title="<%=this.AltLastPost%>">
            <img src="<%=strMiniPost%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>" />
        </a>
        <br />
        <%=new DisplayDateTime() { Format = YAF.Classes.DateTimeFormat.BothTopic, DateTime = this.TopicRow["LastPosted"] }.RenderToString()%>
        <%
            }    
        %>
    </td>
</tr>
