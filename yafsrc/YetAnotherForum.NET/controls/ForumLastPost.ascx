<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumLastPost.ascx.cs"
	Inherits="YAF.Controls.ForumLastPost" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
	<asp:Label ID="LastPosted" runat="server" />
	<br />
	<asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
		<%# String.Format( PageContext.Localization.GetText( "in" ), String.Empty) %>
		<asp:HyperLink ID="topicLink" runat="server"></asp:HyperLink>
		<br />
	</asp:PlaceHolder>
	<%# String.Format( PageContext.Localization.GetText( "by" ), String.Empty) %>
	<YAF:UserLink ID="ProfileUserLink" runat="server" />
	<asp:HyperLink ID="LastTopicImgLink" runat="server">
		<asp:Image runat="server" ID="Icon" />
	</asp:HyperLink>
</asp:PlaceHolder>
<asp:PlaceHolder ID="NoPostsHolder" runat="server">
	<%# PageContext.Localization.GetText( "NO_POSTS" )%>
</asp:PlaceHolder>
