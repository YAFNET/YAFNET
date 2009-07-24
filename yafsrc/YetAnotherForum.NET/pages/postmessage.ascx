<%@ Control Language="c#" CodeFile="postmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="4" width="100%" align="center">
	<tr>
		<td class="header1" align="center" colspan="2">
			<asp:Label ID="Title" runat="server" /></td>
	</tr>
	<tr id="PreviewRow" runat="server" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
		</td>
		<td class="post" valign="top" id="PreviewCell" runat="server">
		    <YAF:MessagePost ID="PreviewMessagePost" runat="server" />
		</td>
	</tr>
	<tr id="SubjectRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="subject" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="Subject" runat="server" CssClass="edit" Width="400" MaxLength="100" /></td>
	</tr>
	<tr id="BlogRow" visible="false" runat="server">
		<td class="postformheader" width="20%">
			Post to blog?</td>
		<td class="post" width="80%">
			<asp:CheckBox ID="PostToBlog" runat="server" />
			Blog Password:
			<asp:TextBox ID="BlogPassword" runat="server" TextMode="Password" Width="400" /><asp:HiddenField
				ID="BlogPostID" runat="server" />
		</td>
	</tr>
	<tr id="FromRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="from" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="From" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PriorityRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="priority" />
		</td>
		<td class="post" width="80%">
			<asp:DropDownList ID="Priority" runat="server" />
		</td>
	</tr>
	<tr id="PersistencyRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="PERSISTENCY" />
		</td>
		<td class="post" width="80%">
			<asp:CheckBox ID="Persistency" runat="server" />
			(<YAF:LocalizedLabel runat="server" LocalizedTag="PERSISTENCY_INFO" />
			)
		</td>
	</tr>
	<tr id="CreatePollRow" runat="server">
		<td class="postformheader" width="20%">
		    <YAF:ThemeButton ID="CreatePoll" runat="server" CssClass="yafcssbigbutton leftItem" TextLocalizedTag="CREATEPOLL"
                OnClick="CreatePoll_Click" /></td>
		<td class="post" width="80%">
			&nbsp;</td>
	</tr>
	<tr id="RemovePollRow" runat="server">
		<td class="postformheader" width="20%">
		    <YAF:ThemeButton ID="RemovePoll" runat="server" CssClass="yafcssbigbutton leftItem" TextLocalizedTag="REMOVEPOLL"
                OnCommand="RemovePoll_Command" OnLoad="RemovePoll_Load" /></td>
		<td class="post" width="80%">
			&nbsp;</td>
	</tr>
	<tr id="PollRow1" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="pollquestion" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:TextBox MaxLength="50" ID="Question" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow2" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice1" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice1ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice1" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow3" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice2" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice2ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice2" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow4" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice3" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice3ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice3" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow5" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice4" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice4ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice4" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow6" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice5" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice5ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice5" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow7" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice6" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice6ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice6" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow8" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice7" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice7ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice7" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow9" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice8" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice8ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice8" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRow10" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice9" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice9ID" runat="server" /><asp:TextBox MaxLength="50" ID="PollChoice9" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr id="PollRowExpire" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:TextBox MaxLength="10" ID="PollExpire" runat="server" CssClass="edit" Width="400" />
			<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire_explain" />
		</td>
	</tr>
	<tr>
		<td class="postformheader" width="20%" valign="top">
			<YAF:LocalizedLabel runat="server" LocalizedTag="message" />
			<br />
			<uc1:smileys runat="server" OnClick="insertsmiley" />
		</td>
		<td class="post" id="EditorLine" width="80%" runat="server">
			<!-- editor goes here -->
		</td>
	</tr>
	<tr id="NewTopicOptionsRow" runat="server" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
		</td>
		<td class="post">
			<asp:CheckBox ID="TopicWatch" runat="server" /><YAF:LocalizedLabel ID="TopicWatchLabel"
				runat="server" LocalizedTag="TOPICWATCH" />
			<br id="TopicAttachBr" runat="server" />
			<asp:CheckBox ID="TopicAttach" runat="server" Visible="false" /><YAF:LocalizedLabel
				ID="TopicAttachLabel" runat="server" LocalizedTag="TOPICATTACH" Visible="false" />
		</td>
	</tr>
	<tr runat="server" id="tr_captcha1" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
		</td>
		<td class="post">
			<asp:Image ID="imgCaptcha" runat="server" /></td>
	</tr>
	<tr runat="server" id="tr_captcha2" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Enter" />
		</td>
		<td class="post">
			<asp:TextBox ID="tbCaptcha" runat="server" />
		</td>
	</tr>
	<tr id="EditReasonRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="EditReason" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="ReasonEditor" runat="server" CssClass="edit" Width="400" /></td>
	</tr>
	<tr>
		<td class="footer1">&nbsp;
		</td>	
		<td class="footer1">
                <YAF:ThemeButton ID="Preview" runat="server" CssClass="yafcssbigbutton leftItem" TextLocalizedTag="PREVIEW"
                OnClick="Preview_Click" />	
                <YAF:ThemeButton ID="PostReply" runat="server" CssClass="yafcssbigbutton leftItem" TextLocalizedTag="SAVE"
                OnClick="PostReply_Click" />	    
                <YAF:ThemeButton ID="Cancel" runat="server" CssClass="yafcssbigbutton leftItem" TextLocalizedTag="CANCEL"
                OnClick="Cancel_Click" />                            	
		</td>
	</tr>
</table>
<br />
<asp:Repeater ID="LastPosts" runat="server" Visible="false">
	<HeaderTemplate>
		<table class="content" cellspacing="1" cellpadding="0" width="100%" align="center">
			<tr>
				<td class="header2" align="center" colspan="2">
					<YAF:LocalizedLabel runat="server" LocalizedTag="last10" />
				</td>
			</tr>
	</HeaderTemplate>
	<FooterTemplate>
		</table>
	</FooterTemplate>
	<ItemTemplate>
		<tr class="postheader">
			<td width="140">
				<b><a href="<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",Eval( "UserID")) %>">
					<%# Eval( "UserName") %>
				</a></b>
			</td>
			<td width="80%" class="small" align="left">
				<b>
					<YAF:LocalizedLabel runat="server" LocalizedTag="posted" />
				</b>
				<%# YafDateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
			</td>
		</tr>
		<tr class="post">
			<td>
				&nbsp;</td>
			<td valign="top" class="message">
			    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false" DataRow="<%# Container.DataItem %>"></YAF:MessagePostData>
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="postheader">
			<td width="140">
				<b><a href="<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",Eval( "UserID")) %>">
					<%# Eval( "UserName") %>
				</a></b>
			</td>
			<td width="80%" class="small" align="left">
				<b>
					<YAF:LocalizedLabel runat="server" LocalizedTag="posted" />
				</b>
				<%# YafDateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>
			</td>
		</tr>
		<tr class="post_alt">
			<td>
				&nbsp;</td>
			<td valign="top" class="message">
			    <YAF:MessagePostData ID="MessagePostAlt" runat="server" ShowAttachments="false" DataRow="<%# Container.DataItem %>"></YAF:MessagePostData>
			</td>
		</tr>
	</AlternatingItemTemplate>
</asp:Repeater>
<iframe runat="server" visible="false" id="LastPostsIFrame" name="lastposts" width="100%"
	height="300" frameborder="0" marginheight="2" marginwidth="2" scrolling="yes"></iframe>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
