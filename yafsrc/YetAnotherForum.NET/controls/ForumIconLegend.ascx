<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false"
	Inherits="YAF.Controls.ForumIconLegend" Codebehind="ForumIconLegend.ascx.cs" %>
<table style="padding: 2px; margin: 2px" width="100%">
	<tr>
		<td>
			<YAF:ThemeImage ID="ForumNewImage" Style="vertical-align: middle" runat="server" LocalizedTitlePage="ICONLEGEND"
				LocalizedTitleTag="New_Posts" ThemeTag="FORUM_NEW" />
			<YAF:LocalizedLabel ID="NewPostsLabel" runat="server" LocalizedPage="ICONLEGEND"
				LocalizedTag="New_Posts" />
			<YAF:ThemeImage ID="ForumRegularImage" Style="vertical-align: middle" runat="server"
				ThemeTag="FORUM" LocalizedTitlePage="ICONLEGEND" LocalizedTitleTag="No_New_Posts" />
			<YAF:LocalizedLabel ID="NoNewPostsLabel" runat="server" LocalizedPage="ICONLEGEND"
				LocalizedTag="No_New_Posts" />
			<YAF:ThemeImage ID="ForumLockedImage" Style="vertical-align: middle" runat="server"
				ThemeTag="FORUM_LOCKED" LocalizedTitlePage="ICONLEGEND" LocalizedTitleTag="Forum_Locked" />
			<YAF:LocalizedLabel ID="ForumLockedLabel" runat="server" LocalizedPage="ICONLEGEND"
				LocalizedTag="Forum_Locked" />
		</td>
	</tr>
</table>
