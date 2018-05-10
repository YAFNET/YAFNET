<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicLine.ascx.cs" Inherits="YAF.Controls.TopicLine" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Controls" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<tr class="<%= "{0}{1}".FormatWith(this.IsAlt ? "topicRow_Alt post_alt" : "topicRow post", this.IsStickyOrAnnouncement()) %>">
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
            if (this.Get<YafBoardSettings>().ShowAvatarsInTopic)
            {
                var avatarUrl = this.GetAvatarUrlFromID(this.TopicRow["UserID"].ToType<int>());

                var avatarTitle = this.GetTextFormatted(
                    "USER_AVATAR",
                    this.HtmlEncode(this.TopicRow[this.Get<YafBoardSettings>().EnableDisplayName ? "StarterDisplay" : "Starter"].
                        ToString()));
        %>
        <img src="<%=avatarUrl%>" alt="<%= avatarTitle %>" title="<%= avatarTitle %>"
            class="avatarimage img-rounded" />
        <%}

            string priorityMessage = this.GetPriorityMessage(this.TopicRow);
            if (priorityMessage.IsSet())
            {
        %>
        <span class="post_priority">
            <%=priorityMessage %></span>
        <%
            }

            const string linkParams = "t={0}";
        %>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, linkParams, this.TopicRow["LinkTopicID"])%>"
            class="post_link" title="<%=this.Get<IFormatMessage>().GetCleanedTopicMessage(this.TopicRow["FirstMessage"], this.TopicRow["LinkTopicID"]).MessageTruncated%>">
            <%=this.FormatTopicName() %> 
        </a>
       <%  if (this.TopicRow["Description"].ToString().IsSet())
            {
        %>               
        <span class="description" >
        <br />
            <%=this.Get<IBadWordReplace>().Replace(this.HtmlEncode(this.TopicRow["Description"]))%>
        </span>  
            <%
            }
            %>     
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
          ReplaceName = this.TopicRow[this.Get<YafBoardSettings>().EnableDisplayName ? "StarterDisplay" : "Starter"].ToString(),
          Style = this.TopicRow["StarterStyle"].ToString()
        }.RenderToString() %>
        </span>
        <%    
            if (this.ShowTopicPosted)
            {
        %>
        <span class="topicPosted">,
            <%= new DisplayDateTime { Format = DateTimeFormat.BothTopic, DateTime = this.TopicRow["Posted"] }.RenderToString()%>
        </span>            
        <%
            }
    
            int actualPostCount = this.TopicRow["Replies"].ToType<int>() + 1;

            if (this.Get<YafBoardSettings>().ShowDeletedMessages)
            {
                // add deleted posts not included in replies...
                actualPostCount += this.TopicRow["NumPostsDeleted"].ToType<int>();
            }     

      string tPager = this.CreatePostPager(
        actualPostCount, this.Get<YafBoardSettings>().PostsPerPage, this.TopicRow["LinkTopicID"].ToType<int>());

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

                var lastAvatarTitle = this.GetTextFormatted(
                    "USER_AVATAR",
                    this.HtmlEncode(this.TopicRow[this.Get<YafBoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUserName"].
                        ToString()));

                if (this.Get<YafBoardSettings>().ShowAvatarsInTopic)
                {%>
        <img src="<%=this.GetAvatarUrlFromID(userID)%>" alt="<%=lastAvatarTitle%>" title="<%=lastAvatarTitle%>"
            class="avatarimage img-rounded" />
        <%
            }

            DateTime lastRead =
                this.Get<IReadTrackCurrentUser>().GetForumTopicRead(
                forumId: this.TopicRow["ForumID"].ToType<int>(),
                topicId: this.TopicRow["TopicID"].ToType<int>(),
                forumReadOverride: this.TopicRow["LastForumAccess"].ToType<DateTime?>() ?? YAF.Utils.Helpers.DateTimeHelper.SqlDbMinTime(),
                topicReadOverride: this.TopicRow["LastTopicAccess"].ToType<DateTime?>() ?? YAF.Utils.Helpers.DateTimeHelper.SqlDbMinTime()); 


        string strMiniPost = this.Get<ITheme>().GetItem(
          "ICONS",
          (this.TopicRow["LastPosted"].ToType<DateTime>() > lastRead)
            ? "ICON_NEWEST"
            : "ICON_LATEST");
        string strMiniUnreadPost = this.Get<ITheme>().GetItem(
          "ICONS",
          (this.TopicRow["LastPosted"].ToType<DateTime>() > lastRead)
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
        <%=new UserLink { UserID = userID, 
                          ReplaceName = this.TopicRow[this.Get<YafBoardSettings>().EnableDisplayName ? "LastUserDisplayName" : "LastUserName"].ToString(),
                          Style = this.TopicRow["LastUserStyle"].ToString() }.RenderToString() %>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, "m={0}#post{0}", this.TopicRow["LastMessageID"]) %>"
            title="<%=this.AltLastPost%>">
            <img src="<%=strMiniPost%>" alt="<%=this.AltLastPost%>" title="<%=this.AltLastPost%>" />            
        </a>
        <a href="<%=YafBuildLink.GetLink(ForumPages.posts, "t={0}&find=unread", this.TopicRow["TopicID"]) %>"
            title="<%=this.AltLastUnreadPost%>">
        <img src="<%=strMiniUnreadPost%>" visible="<%=this.Get<YafBoardSettings>().ShowLastUnreadPost%>" alt="<%=this.AltLastUnreadPost%>" title="<%=this.AltLastUnreadPost%>" />
        </a>
        <br />
        <%=new DisplayDateTime { Format = DateTimeFormat.BothTopic, DateTime = this.TopicRow["LastPosted"] }.RenderToString()%>
        <%
            }    
        %>        
    </td>
</tr>
