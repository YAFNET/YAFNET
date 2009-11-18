<%@ Control Language="c#" CodeFile="posts.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.posts" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="DisplayPost" Src="../controls/DisplayPost.ascx" %>
<%@ Register TagPrefix="YAF" TagName="DisplayAd" Src="../controls/DisplayAd.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<a id="top" name="top"></a>
	<asp:Repeater ID="Poll" runat="server" Visible="false">
		<HeaderTemplate>
		<table cellpadding="0" cellspacing="1" class="content" width="100%">

			<tr>
				<td class="header1" colspan="3">
					<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="question" />
					:
					<%# GetPollQuestion() %>
					<%# GetPollIsClosed() %>
				</td>
			</tr>
			<tr>
				<td class="header2">
					<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="choice" />
				</td>
				<td class="header2" align="center" width="10%">
					<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="votes" />
				</td>
				<td class="header2" width="40%">
					<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="statistics" />
				</td>
			</tr>
			</table>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post">
					<YAF:MyLinkButton runat="server" Enabled="<%#CanVote%>" CommandName="vote" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>'
						Text='<%# HtmlEncode(YafServices.BadWordReplace.Replace(Convert.ToString(DataBinder.Eval(Container.DataItem, "Choice")))) %>' />
				</td>
				<td class="post" align="center">
					<%# DataBinder.Eval(Container.DataItem, "Votes") %>
				</td>
				<td class="post">
					<nobr>
					<img alt="" src="<%# GetThemeContents("VOTE","LCAP") %>"><img alt="" src='<%# GetThemeContents("VOTE","BAR") %>'
						height="12" width='<%# VoteWidth(Container.DataItem) %>%'><img alt="" src='<%# GetThemeContents("VOTE","RCAP") %>'></nobr>
					<%# DataBinder.Eval(Container.DataItem,"Stats") %>
					%
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="header2">
					<%= GetText("total") %>
				</td>
				<td class="header2" align="center">
					<%# GetTotal() %>
				</td>
				<td class="header2">
					100%
				</td>
			</tr>
			<br />
		</FooterTemplate>
	</asp:Repeater>

<table class="command" width="100%">
	<tr>
		<td align="left">
			<YAF:Pager ID="Pager" runat="server" UsePostBack="False" />
		</td>
		<td>
			<YAF:ThemeButton ID="MoveTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="MoveTopic_Click" TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT" />
			<YAF:ThemeButton ID="UnlockTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="UnlockTopic_Click" TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT" />
			<YAF:ThemeButton ID="LockTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="LockTopic_Click" TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT" />
			<YAF:ThemeButton ID="DeleteTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="DeleteTopic_Click" OnLoad="DeleteTopic_Load" TextLocalizedTag="BUTTON_DELETETOPIC"
				TitleLocalizedTag="BUTTON_DELETETOPIC_TT" />
			<YAF:ThemeButton ID="NewTopic1" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="NewTopic_Click" TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" />
			<YAF:ThemeButton ID="PostReplyLink1" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="PostReplyLink_Click" TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT" />
		</td>
	</tr>
</table>
<table class="content postHeader" width="100%">
	<tr class="postTitle">
		<td class="header1">
			<div class="leftItem">
				<asp:Label ID="TopicTitle" runat="server" />
			</div>
			<div class="rightItem">
				<asp:HyperLink ID="OptionsLink" runat="server">
					<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="Options" />
				</asp:HyperLink>
				<asp:PlaceHolder ID="ViewOptions" runat="server">
					<asp:HyperLink ID="ViewLink" runat="server">
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="View" />
					</asp:HyperLink>
				</asp:PlaceHolder>
			</div>
		</td>
	</tr>
	<tr class="header2 postNavigation">
		<td class="header2links">
			<asp:LinkButton ID="PrevTopic" runat="server" CssClass="header2link" OnClick="PrevTopic_Click">
				<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="prevtopic" />
			</asp:LinkButton>
			<asp:LinkButton ID="NextTopic" runat="server" CssClass="header2link" OnClick="NextTopic_Click">
				<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="nexttopic" />
			</asp:LinkButton>
			<div runat="server" visible="false">
				<asp:LinkButton ID="TrackTopic" runat="server" CssClass="header2link" OnClick="TrackTopic_Click">
					<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="watchtopic" />
				</asp:LinkButton>
				<asp:LinkButton ID="EmailTopic" runat="server" CssClass="header2link" OnClick="EmailTopic_Click">
					<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="emailtopic" />
				</asp:LinkButton>
				<asp:LinkButton ID="PrintTopic" runat="server" CssClass="header2link" OnClick="PrintTopic_Click">
					<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="printtopic" />
				</asp:LinkButton>
				<asp:HyperLink ID="RssTopic" runat="server" CssClass="header2link">
					<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="rsstopic" />
				</asp:HyperLink>
			</div>
		</td>
	</tr>
</table>
<asp:Repeater ID="MessageList" runat="server" OnItemCreated="MessageList_OnItemCreated">
	<ItemTemplate>
		<table class="content postContainer" width="100%">
			<%# GetThreadedRow(Container.DataItem) %>
			<YAF:DisplayPost ID="DisplayPost1" runat="server" DataRow="<%# Container.DataItem %>"
				Visible="<%#IsCurrentMessage(Container.DataItem)%>" IsThreaded="<%#IsThreaded%>" />
			<YAF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
		</table>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<table class="content postContainer_Alt" width="100%">
			<%# GetThreadedRow(Container.DataItem) %>
			<YAF:DisplayPost ID="DisplayPostAlt" runat="server" DataRow="<%# Container.DataItem %>"
				IsAlt="True" Visible="<%#IsCurrentMessage(Container.DataItem)%>" IsThreaded="<%#IsThreaded%>" />
			<YAF:DisplayAd ID="DisplayAd" runat="server" Visible="False" />
		</table>
	</AlternatingItemTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="QuickReplyPlaceHolder" runat="server">
	<table class="content postQuickReply" width="100%">
		<tr>
			<td colspan="3" class="post" style="padding: 0px;">
				<YAF:DataPanel runat="server" ID="DataPanel1" AllowTitleExpandCollapse="true" TitleStyle-CssClass="header2"
					TitleStyle-Font-Bold="true" Collapsed="true">
					<div class="post quickReplyLine" id="QuickReplyLine" runat="server">
						
						
						
						
						
					</div>
					<div id="CaptchaDiv" align="center" visible="false" runat="server">
						
						
						
						
						
						<br />
						<table class="content">
							<tr>
								<td class="header2">
									<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="Captcha_Image" />
								
								
								
								
								
								</td>
							</tr>
							<tr>
								<td class="post" align="center">
									<asp:Image ID="imgCaptcha" runat="server" AlternateText="Captcha" />
								
								
								
								
								
								</td>
							</tr>
							<tr>
								<td class="post">
									<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="Captcha_Enter" />
								
								
								
								
								
									<asp:TextBox ID="tbCaptcha" runat="server" />
								
								
								
								
								
								</td>
							</tr>
						</table>
						<br />
					</div>
					&nbsp;<div align="center" style="margin: 7px;">
						<asp:Button ID="QuickReply" CssClass="pbutton" runat="server" />
						
						
						
						
						
						&nbsp;</div>
				</YAF:DataPanel>
			</td>
		</tr>
	</table>
</asp:PlaceHolder>
<table class="content postForumUsers" width="100%">
	<YAF:ForumUsers ID="ForumUsers1" runat="server" />
</table>
<table cellpadding="0" cellspacing="0" class="command" width="100%">
	<tr>
		<td align="left">
			<YAF:Pager ID="PagerBottom" runat="server" LinkedPager="Pager" UsePostBack="false" />
		</td>
		<td>
			<YAF:ThemeButton ID="MoveTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="MoveTopic_Click" TextLocalizedTag="BUTTON_MOVETOPIC" TitleLocalizedTag="BUTTON_MOVETOPIC_TT" />
			<YAF:ThemeButton ID="UnlockTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="UnlockTopic_Click" TextLocalizedTag="BUTTON_UNLOCKTOPIC" TitleLocalizedTag="BUTTON_UNLOCKTOPIC_TT" />
			<YAF:ThemeButton ID="LockTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="LockTopic_Click" TextLocalizedTag="BUTTON_LOCKTOPIC" TitleLocalizedTag="BUTTON_LOCKTOPIC_TT" />
			<YAF:ThemeButton ID="DeleteTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="DeleteTopic_Click" OnLoad="DeleteTopic_Load" TextLocalizedTag="BUTTON_DELETETOPIC"
				TitleLocalizedTag="BUTTON_DELETETOPIC_TT" />
			<YAF:ThemeButton ID="NewTopic2" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="NewTopic_Click" TextLocalizedTag="BUTTON_NEWTOPIC" TitleLocalizedTag="BUTTON_NEWTOPIC_TT" />
			<YAF:ThemeButton ID="PostReplyLink2" runat="server" CssClass="yafcssbigbutton rightItem"
				OnClick="PostReplyLink_Click" TextLocalizedTag="BUTTON_POSTREPLY" TitleLocalizedTag="BUTTON_POSTREPLY_TT" />
		</td>
	</tr>
</table>
<YAF:PageLinks ID="PageLinksBottom" runat="server" LinkedPageLinkID="PageLinks" />
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
	<div id="DivForumJump">
		<YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
		&nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
	</div>
</asp:PlaceHolder>
<div id="DivPageAccess" class="smallfont">
	<YAF:PageAccess ID="PageAccess1" runat="server" />
</div>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
<asp:UpdatePanel ID="PopupMenuUpdatePanel" runat="server">
	<ContentTemplate>
		<YAF:PopMenu runat="server" ID="OptionsMenu" Control="OptionsLink" />
		<span id="WatchTopicID" runat="server" visible="false"></span>
	</ContentTemplate>
</asp:UpdatePanel>
<YAF:PopMenu ID="ViewMenu" runat="server" Control="ViewLink" />
