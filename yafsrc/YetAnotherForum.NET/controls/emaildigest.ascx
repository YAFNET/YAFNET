<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="emaildigest.ascx.cs"
    Inherits="YAF.Controls.emaildigest" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="ServiceStack" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="en">
<head id="YafHead" runat="server">
    <title>Digest Notification</title>
</head>
<body class="bg-light">
<div class="container">
<div class="mx-auto mt-4 mb-3 text-center" 
     style="width:100px;height:40px;background: url('<%= "{0}/{1}/{2}/{3}".Fmt(this.Get<BoardSettings>().BaseUrlMask, YafForumInfo.ForumClientFileRoot, BoardFolders.Current.Logos, this.BoardSettings.ForumLogo) %>') no-repeat;"></div>
  <div class="card mb-4" style="border-top: 5px solid #3761b5;">
    <div class="card-body">
      <h4 class="text-center">
          <%= this.GetText("ACTIVETOPICS") %>
      </h4>
      <h5 class="text-muted text-center">
          <%= this.Get<IDateTime>().FormatDateLong(DateTime.UtcNow) %>
      </h5>
    </div>
  </div>
      
    <% if (this.NewTopics.Any())
                   { %>
                <h4 class="text-center">
                    <%= this.GetText("NEWTOPICS") %></h4>
                <%
                    foreach (var f in this.NewTopics)
                    { %>
                <h5 class="text-center">
                    <%= f.Key.Name %>
                </h5>
        <ul class="list-group text-center">
                <%
                    foreach (var t in f.OrderByDescending(x => x.LastPostDate))
                    { %>
            <li class="list-group-item">
                        <p class="text-center">
                            <a href="<%= YafBuildLink.GetLink(this.BoardSettings, ForumPages.posts, true, "m={0}#post{0}", t.LastMessageID) %>"
                                target="_blank">
                                <i class="fas fa-comment"></i> <%= t.Subject %></a> 
                                 <span class="badge badge-secondary">
                                    <%= string.Format(this.GetText("COMMENTS"), t.Replies) %>
                                 </span>
                        </p>
                        <p class="text-center">
                            <%= this.GetMessageFormattedAndTruncated(t.LastMessage, 200) %>
                        </p>
                        <p class="text-center text-muted">
                            <small><%= string.Format(this.GetText("STARTEDBY"), t.StartedUserName) %></small>
                        </p>
                        <a class="btn btn-primary btn-sm mx-auto mt-2"
                                  href="<%= YafBuildLink.GetLink(ForumPages.posts, true, "m={0}#post{0}", t.LastMessageID) %>"
                                  target="_blank">
                            <%= this.GetText("LINK") %></a>
                    </li>
                <%
                    }%>
        </ul>
                    
        <%
                    }
                   } %>
                <% if (this.ActiveTopics.Any())
                   { %>
                    <%
                    foreach (var f in this.ActiveTopics)
                    { %>
                <h5 class="text-center">
                    <%= f.Key.Name %>
                </h5>
               <ul class="list-group text-center">
                <%
                    foreach (var t in f.OrderBy(x => x.Replies))
                    { %>
                   <li class="list-group-item">
                        <p class="text-center">
                            <a href="<%= YafBuildLink.GetLink(this.BoardSettings, ForumPages.posts, true, "m={0}#post{0}", t.LastMessageID) %>"
                                target="_blank">
                                <i class="fas fa-comment"></i> <%= t.Subject %></a> 
                                <span class="badge badge-secondary">
                                    <%= string.Format(this.GetText("COMMENTS"), t.Replies) %>
                                </span>
                        </p>
                        <p class="text-center">
                            <%= this.GetMessageFormattedAndTruncated(t.LastMessage, 200) %>
                        </p>
                        <p class="text-center text-muted">
                            <small><%= string.Format(this.GetText("STARTEDBY"), t.StartedUserName) %></small>
                        </p>
                            <a class="btn btn-primary btn-sm mx-auto mt-2" 
                               href="<%= YafBuildLink.GetLink(this.BoardSettings, ForumPages.posts, true, "m={0}#post{0}", t.LastMessageID) %>"
                               target="_blank">
                                <%= this.GetText("LINK") %></a>
                    </li>
                <%
                    }%>
                    </ul>
                    
                   <% }
                   } %>

    <div class="text-center text-muted small"> 
      <%= this.GetText("REMOVALTEXT") %>&nbsp;<a href="<%= YafBuildLink.GetLink(this.BoardSettings, ForumPages.cp_subscriptions, true) %>">
          <%= this.GetText("REMOVALLINK") %></a>
  </div>
</div>
</body>
</html>
