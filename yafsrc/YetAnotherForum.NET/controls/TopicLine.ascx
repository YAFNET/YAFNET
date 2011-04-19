<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicLine.ascx.cs" Inherits="YAF.Controls.TopicLine" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Controls" %>
<%@ Import Namespace="YAF.Types.Constants" %>
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
        %>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, linkParams, this.TopicRow["LinkTopicID"])%>"
            class="post_link" title="<%=this.Get<IFormatMessage>().GetCleanedTopicMessage(this.TopicRow["FirstMessage"], this.TopicRow["LinkTopicID"]).MessageTruncated%>">
            <%=this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Subject"]))%></a>
        <%
            var favoriteCount = this.TopicRow["FavoriteCount"].ToType<int>();
            
            if (favoriteCount > 0)
            {
%>
        <span class="topicFavoriteCount"><a title="<%=this.GetText("FAVORITE_COUNT_TT")%>">[+<%=favoriteCount%>]</a></span>
        <%
            }
%>
        <br />
        <span class="topicStarter">
            <%= new UserLink
        {
          ID = "topicStarterLink",
          UserID = this.TopicRow["UserID"].ToType<int>(),
          ReplaceName = this.TopicRow["Starter"].ToString(),
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

        string strMiniPost = this.Get<ITheme>().GetItem(
          "ICONS",
          (DateTime.Parse(this.TopicRow["LastPosted"].ToString()) > this.Get<IYafSession>().GetTopicRead((int)this.TopicRow["TopicID"]))
            ? "ICON_NEWEST"
            : "ICON_LATEST");
        string strMiniUnreadPost = this.Get<ITheme>().GetItem(
          "ICONS",
          (DateTime.Parse(this.TopicRow["LastPosted"].ToString()) > this.Get<IYafSession>().GetTopicRead((int)this.TopicRow["TopicID"]))
          ? "ICON_NEWEST_UNREAD"
          : "ICON_LATEST_UNREAD");   
                  
        if (string.IsNullOrEmpty(this.AltLastPost))
        {
            this.AltLastPost = this.GetText("DEFAULT", "GO_LAST_POST");
        }
                
        if (string.IsNullOrEmpty(this.AltLastUnreadPost))
        {
            this.AltLastUnreadPost = this.GetText("DEFAULT", "GO_LASTUNREAD_POST");
        }    
                
        %>
        <%=new UserLink { UserID = userID, ReplaceName = this.TopicRow["LastUserName"].ToString(), Style = this.TopicRow["LastUserStyle"].ToString() }.RenderToString() %>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, "m={0}&find=lastpost", this.TopicRow["LastMessageID"]) %>"
            title="<%=this.AltLastPost%>">
            <img src="<%=strMiniPost%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>" />            
        </a>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, "m={0}&find=unread", this.TopicRow["LastMessageID"]) %>"
            title="<%=this.AltLastUnreadPost%>">
        <img src="<%=strMiniUnreadPost%>" visible="<%=this.PageContext.BoardSettings.ShowLastUnreadPost%>" alt="<%=this.AltLastUnreadPost%>" title="<%=this.AltLastUnreadPost%>" />
        </a>
        <br />
        <%=new DisplayDateTime() { Format = DateTimeFormat.BothTopic, DateTime = this.TopicRow["LastPosted"] }.RenderToString()%>
        <%
            }    
        %>
    </td>
</tr>
