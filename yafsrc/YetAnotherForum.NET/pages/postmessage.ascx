<%@ Control Language="c#" CodeFile="postmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="smileys" Src="../controls/smileys.ascx" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<table align="center" cellpadding="4" cellspacing="1" class="content" width="100%">
	<tr>
		<td align="center" class="header1" colspan="2">
			<asp:Label ID="Title" runat="server" />
		</td>
	</tr>
	<tr id="PreviewRow" runat="server" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
		</td>
		<td id="PreviewCell" runat="server" class="post" valign="top">
			<YAF:MessagePost ID="PreviewMessagePost" runat="server" />
		</td>
	</tr>
	<tr id="SubjectRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="subject" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="Subject" runat="server" CssClass="edit" MaxLength="100" Width="400" />
		</td>
	</tr>
	<tr id="BlogRow" runat="server" visible="false">
		<td class="postformheader" width="20%">
			Post to blog?
		</td>
		<td class="post" width="80%">
			<asp:CheckBox ID="PostToBlog" runat="server" />
			Blog Password:
			<asp:TextBox ID="BlogPassword" runat="server" TextMode="Password" Width="400" />
			<asp:HiddenField ID="BlogPostID" runat="server" />
		</td>
	</tr>
	<tr id="FromRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="from" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="From" runat="server" CssClass="edit" Width="400" />
		</td>
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
			<YAF:ThemeButton ID="CreatePoll" runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="CreatePoll_Click" TextLocalizedTag="CREATEPOLL" />
		</td>
		<td class="post" width="80%">
			&nbsp;
		</td>
	</tr>
	<tr id="RemovePollRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:ThemeButton ID="RemovePoll" runat="server" CssClass="yafcssbigbutton leftItem"
				OnCommand="RemovePoll_Command" OnLoad="RemovePoll_Load" TextLocalizedTag="REMOVEPOLL" />
		</td>
		<td class="post" width="80%">
			&nbsp;
		</td>
	</tr>
	<tr id="PollRow1" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="pollquestion" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="Question" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow2" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice1" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice1ID" runat="server" />
			<asp:TextBox ID="PollChoice1" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow3" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice2" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice2ID" runat="server" />
			<asp:TextBox ID="PollChoice2" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow4" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice3" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice3ID" runat="server" />
			<asp:TextBox ID="PollChoice3" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow5" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice4" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice4ID" runat="server" />
			<asp:TextBox ID="PollChoice4" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow6" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice5" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice5ID" runat="server" />
			<asp:TextBox ID="PollChoice5" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow7" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice6" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice6ID" runat="server" />
			<asp:TextBox ID="PollChoice6" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow8" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice7" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice7ID" runat="server" />
			<asp:TextBox ID="PollChoice7" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow9" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice8" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice8ID" runat="server" />
			<asp:TextBox ID="PollChoice8" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow10" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice9" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:HiddenField ID="PollChoice9ID" runat="server" />
			<asp:TextBox ID="PollChoice9" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRowExpire" runat="server" visible="false">
		<td class="postformheader" width="20%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire" />
			</em>
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="PollExpire" runat="server" CssClass="edit" MaxLength="10" Width="400" />
			<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire_explain" />
		</td>
	</tr>
	<tr>
		<td class="postformheader" valign="top" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="message" />
			<br />
			<YAF:smileys runat="server" OnClick="insertsmiley" />
			<br />
			<YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server" LocalizedTag="MAXNUMBEROF" />
		</td>
		<td id="EditorLine" runat="server" class="post" width="80%">
			<!-- editor goes here -->
		</td>
	</tr>
	<tr id="NewTopicOptionsRow" runat="server" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
		</td>
		<td class="post">
			<asp:CheckBox ID="TopicWatch" runat="server" />
			<YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" LocalizedTag="TOPICWATCH" />
			<br id="TopicAttachBr" runat="server" />
			<asp:CheckBox ID="TopicAttach" runat="server" Visible="false" />
			<YAF:LocalizedLabel ID="TopicAttachLabel" runat="server" LocalizedTag="TOPICATTACH"
				Visible="false" />
		</td>
	</tr>
	<tr id="tr_captcha1" runat="server" visible="false">
		<td class="postformheader" valign="top">
			<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
		</td>
		<td class="post">
			<asp:Image ID="imgCaptcha" runat="server" />
		</td>
	</tr>
	<tr id="tr_captcha2" runat="server" visible="false">
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
			<asp:TextBox ID="ReasonEditor" runat="server" CssClass="edit" Width="400" />
		</td>
	</tr>
	<tr>
		<td class="footer1">
			&nbsp;
		</td>
		<td class="footer1">
			<YAF:ThemeButton ID="Preview" runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="Preview_Click" TextLocalizedTag="PREVIEW" />
			<YAF:ThemeButton ID="PostReply" runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="PostReply_Click" TextLocalizedTag="SAVE" />
			<YAF:ThemeButton ID="Cancel" runat="server" CssClass="yafcssbigbutton leftItem" OnClick="Cancel_Click"
				TextLocalizedTag="CANCEL" />
		</td>
	</tr>
</table>
<br />

<script type="text/javascript">

	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}

</script>

<YAF:LastPosts ID="LastPosts1" runat="server" Visible="false" />
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
