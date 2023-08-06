<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmailDigest.ascx.cs"
    Inherits="YAF.Controls.EmailDigest" %>

<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Objects" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html>
<head id="YafHead" runat="server">
    <title>Digest Notification</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
</head>
<body class="bg-light">
<div class="container">
    <div class="mx-auto mt-4 mb-3 text-center" style="width: 100px; height: 40px;">
        <asp:Image ID="Logo" AlternateText="logo" runat="server" />
    </div>
    <div class="p-3 mx-auto text-center">
        <div class="card-body">
            <h1 class="display-4 fw-normal">
                <YAF:LocalizedLabel runat="server" LocalizedTag="ACTIVETOPICS"></YAF:LocalizedLabel>
            </h1>
            <p class="fs-5 text-body-secondary">
                <%= this.Get<IDateTimeService>().FormatDateLong(DateTime.UtcNow) %>
            </p>
        </div>
    </div>

<asp:Repeater runat="server" ID="NewTopicsForumsRepeater" OnItemDataBound="NewTopicsForumsRepeater_OnItemDataBound">
        <ItemTemplate>
            <div class="card text-center mb-3">
                <div class="card-body">
                    <h5 class="card-title">
                        <%# ((IGrouping<SimpleForum, SimpleTopic>)Container.DataItem).Key.Name %>
                    </h5>
                    <asp:Repeater runat="server" ID="NewTopicsRepeater">
                        <ItemTemplate>
                            <h6 class="card-subtitle">
                                <a href="<%# this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Posts, new { m = ((SimpleTopic)Container.DataItem).LastMessageID, name = ((SimpleTopic)Container.DataItem).Subject }) %>"
                                   target="_blank">
                                    <i class="fas fa-comment"></i> <%# ((SimpleTopic)Container.DataItem).Subject %></a>
                                <span class="badge text-bg-secondary">
                                    <%# string.Format(this.GetText("COMMENTS"), ((SimpleTopic)Container.DataItem).Replies) %>
                                </span>
                            </h6>
                            <p class="text-body-secondary small">
                                <%# string.Format(this.GetText("STARTEDBY"), ((SimpleTopic)Container.DataItem).StartedUserName) %>
                            </p>
                            <p class="card-text">
                                <%# this.GetMessageFormattedAndTruncated(((SimpleTopic)Container.DataItem).LastMessage, 200) %>
                            </p>

                            <a class="btn btn-primary btn-sm mx-auto mt-2"
                               href="<%# this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Posts, new { m = ((SimpleTopic)Container.DataItem).LastMessageID, name = ((SimpleTopic)Container.DataItem).Subject }) %>"
                               target="_blank">
                                <%# this.GetText("LINK") %></a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Repeater runat="server" ID="ActiveTopicsForumsRepeater" OnItemDataBound="ActiveTopicsForumsRepeater_OnItemDataBound">
        <ItemTemplate>
            <div class="card text-center mb-3">
                <div class="card-body">
                    <h5 class="card-title">
                        <%# ((IGrouping<SimpleForum, SimpleTopic>)Container.DataItem).Key.Name %>
                    </h5>
                    <asp:Repeater runat="server" ID="ActiveTopicsRepeater">
                        <ItemTemplate>
                            <h6 class="card-subtitle">
                                <a href="<%# this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Posts, new { m = ((SimpleTopic)Container.DataItem).LastMessageID, name = ((SimpleTopic)Container.DataItem).Subject }) %>"
                                   target="_blank">
                                    <i class="fas fa-comment"></i> <%# ((SimpleTopic)Container.DataItem).Subject %></a>
                                <span class="badge text-bg-secondary">
                                    <%# string.Format(this.GetText("COMMENTS"), ((SimpleTopic)Container.DataItem).Replies) %>
                                </span>
                            </h6>
                            <p class="text-body-secondary small">
                                <%# string.Format(this.GetText("STARTEDBY"), ((SimpleTopic)Container.DataItem).StartedUserName) %>
                            </p> 
                            <p class="card-text">
                                <%# this.GetMessageFormattedAndTruncated(((SimpleTopic)Container.DataItem).LastMessage, 200) %>
                            </p>

                            <a class="btn btn-primary btn-sm mx-auto mt-2"
                               href="<%# this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Posts, new { m = ((SimpleTopic)Container.DataItem).LastMessageID, name = ((SimpleTopic)Container.DataItem).Subject }) %>"
                               target="_blank">
                                <%# this.GetText("LINK") %></a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </ItemTemplate>
    </asp:Repeater>

<div class="text-center text-body-secondary small">
    <YAF:LocalizedLabel runat="server" LocalizedTag="REMOVALTEXT"></YAF:LocalizedLabel>&nbsp;
    <a href="<%= this.Get<LinkBuilder>().GetAbsoluteLink(ForumPages.Profile_Subscriptions) %>">
        <YAF:LocalizedLabel runat="server" LocalizedTag="REMOVALLINK"></YAF:LocalizedLabel>
    </a>
</div>

</div>

</body>
</html>