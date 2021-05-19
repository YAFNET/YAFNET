<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailDigest.ascx.cs"
    Inherits="YAF.Controls.EmailDigest" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="ServiceStack" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html lang="en">
<head id="YafHead" runat="server">
    <title>Digest Notification</title>
</head>
<body class="bg-light">
<div class="container">
<div class="mx-auto mt-4 mb-3 text-center"
     style="width:100px;height:40px;background: url('<%= "{0}/{1}/{2}/{3}".Fmt(this.PageContext.BoardSettings.BaseUrlMask, BoardInfo.ForumClientFileRoot, this.Get<BoardFolders>().Logos, this.BoardSettings.ForumLogo) %>') no-repeat;"></div>
  <div class="card mb-4" style="border-top: 5px solid #3761b5;">
    <div class="card-body">
      <h4 class="text-center">
          <YAF:LocalizedLabel runat="server" LocalizedTag="ACTIVETOPICS"></YAF:LocalizedLabel>
      </h4>
      <h5 class="text-muted text-center">
          <%= this.Get<IDateTimeService>().FormatDateLong(DateTime.UtcNow) %>
      </h5>
    </div>
  </div>

    <% if (this.NewTopics.Any())
                   { %>

                <%
                    foreach (var f in this.NewTopics)
                    { %>
               <div class="card text-center mb-3">
                   <div class="card-body">
                       <h5 class="card-title">
                    <%= f.Key.Name %>
                </h5>

                <%
                    foreach (var t in f.OrderByDescending(x => x.LastPostDate))
                    { %>
                           <h6 class="card-subtitle">
                            <a href="<%= this.Get<LinkBuilder>().GetLink(this.BoardSettings, ForumPages.Posts, true, "m={0}&name={1}", t.LastMessageID, t.Subject) %>"
                                target="_blank">
                                <i class="fas fa-comment"></i> <%= t.Subject %></a>
                                 <span class="badge bg-secondary">
                                    <%= string.Format(this.GetText("COMMENTS"), t.Replies) %>
                                 </span>
                           </h6>
                           <p class="text-muted small">
                               <%= string.Format(this.GetText("STARTEDBY"), t.StartedUserName) %>
                           </p>
                        <p class="card-text">
                            <%= this.GetMessageFormattedAndTruncated(t.LastMessage, 200) %>
                        </p>

                        <a class="btn btn-primary btn-sm mx-auto mt-2"
                                  href="<%= this.Get<LinkBuilder>().GetLink(this.BoardSettings, ForumPages.Posts, true, "m={0}&name={1}", t.LastMessageID, t.Subject) %>"
                                  target="_blank">
                            <%= this.GetText("LINK") %></a>

                <%
                    }%>

                       </div>
                    </div>
        <%
                    }
                   } %>
                <% if (this.ActiveTopics.Any())
                   { %>
                    <%
                    foreach (var f in this.ActiveTopics)
                    { %>
            <div class="card text-center mb-3">
                <div class="card-body">
                    <h5 class="card-title">
                        <%= f.Key.Name %>
                    </h5>

                    <%
                        foreach (var t in f.OrderByDescending(x => x.LastPostDate))
                        { %>
                        <h6 class="card-subtitle">
                            <a href="<%= this.Get<LinkBuilder>().GetLink(this.BoardSettings, ForumPages.Posts, true, "m={0}&name={1}", t.LastMessageID, t.Subject) %>"
                               target="_blank">
                                <i class="fas fa-comment"></i> <%= t.Subject %></a>
                            <span class="badge bg-secondary">
                                <%= string.Format(this.GetText("COMMENTS"), t.Replies) %>
                            </span>
                        </h6>
                        <p class="text-muted small">
                            <%= string.Format(this.GetText("STARTEDBY"), t.StartedUserName) %>
                        </p>
                        <p class="card-text">
                            <%= this.GetMessageFormattedAndTruncated(t.LastMessage, 200) %>
                        </p>

                        <a class="btn btn-primary btn-sm mx-auto mt-2"
                           href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Posts, true, "m={0}&name={1}", t.LastMessageID, t.Subject) %>"
                           target="_blank">
                            <%= this.GetText("LINK") %></a>

                    <%
                    }%>

                </div>
            </div>

                   <% }
                   } %>

    <div class="text-center text-muted small">
        <YAF:LocalizedLabel runat="server" LocalizedTag="REMOVALTEXT"></YAF:LocalizedLabel>&nbsp;
        <a href="<%= this.Get<LinkBuilder>().GetLink(this.BoardSettings, ForumPages.Profile_Subscriptions, true) %>">
            <YAF:LocalizedLabel runat="server" LocalizedTag="REMOVALLINK"></YAF:LocalizedLabel>
        </a>
  </div>
</div>
</body>
</html>
