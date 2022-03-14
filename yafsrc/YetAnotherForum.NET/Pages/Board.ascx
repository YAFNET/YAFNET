<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Board" Codebehind="Board.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ForumWelcome" Src="../controls/ForumWelcome.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumStatsUsers" Src="../controls/ForumStatsUsers.ascx" %>
<%@ Register TagPrefix="YAF" TagName="Statistics" Src="../controls/Statistics.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ActiveDiscussion" Src="../controls/ActiveDiscussion.ascx" %>
<%@ Register TagPrefix="YAF" TagName="CategoryList" Src="../controls/CategoryList.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col">
        <YAF:ForumWelcome runat="server" ID="Welcome" />
    </div>
</div>

<div class="row">
    <div class='<%= this.PageBoardContext.BoardSettings.TwoColumnBoardLayout ? "col-md-8" : "col" %>'>
        <YAF:CategoryList ID="ForumCategoryList" runat="server" />
    </div>
    <%= this.PageBoardContext.BoardSettings.TwoColumnBoardLayout ?  string.Empty : @"</div><div class=""row"">" %>
    <div class='<%= this.PageBoardContext.BoardSettings.TwoColumnBoardLayout ? "col-md-4" : "col" %>'>
        <YAF:ActiveDiscussion ID="ActiveDiscussions" runat="server" />
        <YAF:ForumStatsUsers ID="ForumStats" runat="Server" />
    </div>
</div>
<div class="row">
    <div class="col">
        <YAF:Statistics ID="ForumStatistics" runat="Server" />
    </div>
</div>