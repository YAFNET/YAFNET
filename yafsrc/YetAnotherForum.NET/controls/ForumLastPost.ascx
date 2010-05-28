<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="YAF.Controls.ForumLastPost" Codebehind="ForumLastPost.ascx.cs" %>
<asp:PlaceHolder ID="LastPostedHolder" runat="server">
	<asp:Label ID="LastPosted" runat="server" />
	<br />
	<asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
		<YAF:LocalizedLabel ID="InLabel" runat="server" LocalizedTag="IN" />
		<asp:HyperLink ID="topicLink" runat="server"></asp:HyperLink>
		<br />
	</asp:PlaceHolder>
	<YAF:LocalizedLabel ID="ByLabel" runat="server" LocalizedTag="BY" />
	<YAF:UserLink ID="ProfileUserLink" runat="server" />&nbsp;<asp:HyperLink ID="LastTopicImgLink" runat="server"><YAF:ThemeImage ID="Icon" runat="server" /></asp:HyperLink>
</asp:PlaceHolder>

<YAF:LocalizedLabel ID="NoPostsLabel" runat="server" LocalizedTag="NO_POSTS" />
