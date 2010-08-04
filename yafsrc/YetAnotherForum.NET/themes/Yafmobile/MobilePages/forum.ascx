<%@ Control Language="c#" CodeFile="forum.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.forum" %>
<%@ Register TagPrefix="YAF" TagName="ForumWelcome" Src="forumwelcome.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumActiveDiscussion" Src="forumactivediscussion.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumCategoryList" Src="forumcategorylist.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ShoutBox" Src="../../../controls/ShoutBox.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:ForumWelcome runat="server" ID="Welcome" />
<div class="DivTopSeparator">
</div>
<YAF:ShoutBox ID="ShoutBox1" runat="server" />
<YAF:ForumCategoryList ID="ForumCategoryList" runat="server"></YAF:ForumCategoryList>
<br />
<YAF:ForumActiveDiscussion ID="ActiveDiscussions" runat="server" />
<br />
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
