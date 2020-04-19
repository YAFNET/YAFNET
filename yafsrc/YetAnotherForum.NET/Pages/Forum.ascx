<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.forum" Codebehind="Forum.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ForumWelcome" Src="../controls/ForumWelcome.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumStatsUsers" Src="../controls/ForumStatsUsers.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumStatistics" Src="../controls/ForumStatistics.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumActiveDiscussion" Src="../controls/ForumActiveDiscussion.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumCategoryList" Src="../controls/ForumCategoryList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <YAF:ForumWelcome runat="server" ID="Welcome" />
    </div>
</div>

<YAF:PollList ID="PollList" runat="server"/>

<div class="row">
    <div class="col-md-8">
        <YAF:ForumCategoryList ID="ForumCategoryList" runat="server" />
    </div>
    <div class="col-md-4">
        <YAF:ForumActiveDiscussion ID="ActiveDiscussions" runat="server" />
        <YAF:ForumStatsUsers ID="ForumStats" runat="Server" />
    </div>
</div>
<div class="row">
    <div class="col">
        <YAF:ForumStatistics ID="ForumStatistics" runat="Server" />
    </div>
</div>