<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumLastPost.ascx.cs" Inherits="YAF.Controls.ForumLastPost" %>

<%# YafDateTime.FormatDateTimeTopic( DataRow["LastPosted"] ) %>
<br/>
<asp:PlaceHolder ID="TopicInPlaceHolder" runat="server">
	<%# String.Format( PageContext.Localization.GetText( "in" ), String.Empty) %>
	<asp:HyperLink id="topicLink" runat="server"></asp:HyperLink>
	<br/>
</asp:PlaceHolder>

<%# String.Format( PageContext.Localization.GetText( "by" ), String.Empty) %> <YAF:UserLink ID="ProfileUserLink" runat="server" />

<asp:HyperLink id="LastTopicImgLink" runat="server">
	<asp:Image runat="server" id="Icon" />
</asp:HyperLink>