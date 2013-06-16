<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.forum" Codebehind="forum.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="ForumWelcome" Src="../controls/ForumWelcome.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumIconLegend" Src="../controls/ForumIconLegend.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumStatistics" Src="../controls/ForumStatistics.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumActiveDiscussion" Src="../controls/ForumActiveDiscussion.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumCategoryList" Src="../controls/ForumCategoryList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ShoutBox" Src="../controls/ShoutBox.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:ForumWelcome runat="server" ID="Welcome" />
<div class="DivTopSeparator">
</div>
<YAF:ShoutBox ID="ShoutBox1" runat="server" />
<YAF:PollList ID="PollList" runat="server"/>
<YAF:ForumCategoryList ID="ForumCategoryList" runat="server"></YAF:ForumCategoryList>
<br />
<YAF:ForumActiveDiscussion ID="ActiveDiscussions" runat="server" />
<br />
<YAF:ForumStatistics ID="ForumStats" runat="Server" />
<YAF:ForumIconLegend ID="IconLegend" runat="server" />
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>

