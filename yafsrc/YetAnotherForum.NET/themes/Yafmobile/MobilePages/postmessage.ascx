<%@ Control Language="c#" CodeFile="postmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<table align="center" cellpadding="4" cellspacing="1" class="content" width="100%">
	<tr>
		<td align="center" class="header1" colspan="2">
			<asp:Label ID="Title" runat="server" />
		</td>
	</tr>
	<tr id="PreviewRow" runat="server" visible="false">
		<td class="postformheader" valign="top" colspan="2">
			<YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
			<YAF:MessagePost ID="PreviewMessagePost" runat="server" />
		</td>
	</tr>
	<tr id="SubjectRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="subject" />
		    <br />
			<asp:TextBox ID="Subject" runat="server" CssClass="edit" MaxLength="100" Width="400" />
		</td>
	</tr>
	<tr id="BlogRow" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			&nbsp;Post to blog?
		    <br />
			<asp:CheckBox ID="PostToBlog" runat="server" />
			Blog Password:
			<asp:TextBox ID="BlogPassword" runat="server" TextMode="Password" Width="400" />
			:<asp:HiddenField ID="BlogPostID" runat="server" />
		</td>
	</tr>
	<tr id="FromRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="from" />
		    <br />
			<asp:TextBox ID="From" runat="server" CssClass="edit" Width="400" />
		</td>
	</tr>
	<tr id="PriorityRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="priority" />
		    <br />
			<asp:DropDownList ID="Priority" runat="server" />
		</td>
	</tr>
	<tr id="PersistencyRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="PERSISTENCY" />
		    <br />
			<asp:CheckBox ID="Persistency" runat="server" />
			(<YAF:LocalizedLabel runat="server" LocalizedTag="PERSISTENCY_INFO" />
			)
		</td>
	</tr>
	<tr id="CreatePollRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:ThemeButton ID="CreatePoll" runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="CreatePoll_Click" TextLocalizedTag="CREATEPOLL" />
			
		</td>
	</tr>
	<tr id="RemovePollRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:ThemeButton ID="RemovePoll" runat="server" CssClass="yafcssbigbutton leftItem"
				OnCommand="RemovePoll_Command" OnLoad="RemovePoll_Load" TextLocalizedTag="REMOVEPOLL" />
			&nbsp;
		</td>
	</tr>
	<tr id="PollRow1" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="pollquestion" />
			</em>
			<asp:TextBox ID="Question" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow2" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice1" />
			</em>
			<asp:HiddenField ID="PollChoice1ID" runat="server" />
			<asp:TextBox ID="PollChoice1" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow3" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice2" />
			</em>
			<asp:HiddenField ID="PollChoice2ID" runat="server" />
			<asp:TextBox ID="PollChoice2" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow4" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice3" />
			</em>
			<asp:HiddenField ID="PollChoice3ID" runat="server" />
			<asp:TextBox ID="PollChoice3" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow5" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice4" />
			</em>
			<asp:HiddenField ID="PollChoice4ID" runat="server" />
			<asp:TextBox ID="PollChoice4" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow6" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice5" />
			</em>
			<asp:HiddenField ID="PollChoice5ID" runat="server" />
			<asp:TextBox ID="PollChoice5" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow7" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice6" />
			</em>
			
			<asp:HiddenField ID="PollChoice6ID" runat="server" />
			<asp:TextBox ID="PollChoice6" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow8" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice7" />
			</em>
			<asp:HiddenField ID="PollChoice7ID" runat="server" />
			<asp:TextBox ID="PollChoice7" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow9" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice8" />
			</em>
			<asp:HiddenField ID="PollChoice8ID" runat="server" />
			<asp:TextBox ID="PollChoice8" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRow10" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="choice9" />
			</em>
			<asp:HiddenField ID="PollChoice9ID" runat="server" />
			<asp:TextBox ID="PollChoice9" runat="server" CssClass="edit" MaxLength="50" Width="400" />
		</td>
	</tr>
	<tr id="PollRowExpire" runat="server" visible="false">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<em>
				<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire" />
			</em>
			<asp:TextBox ID="PollExpire" runat="server" CssClass="edit" MaxLength="10" Width="400" />
			<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire_explain" />
		</td>
	</tr>
	<tr>
		<td id="EditorLine" runat="server" class="post">
			<!-- editor goes here -->
			<YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server" LocalizedTag="MAXNUMBEROF" />
			<br />
			<YAF:LocalizedLabel runat="server" LocalizedTag="message" />
		</td>
	</tr>
	<tr id="NewTopicOptionsRow" runat="server" visible="false">
		<td class="postformheader" valign="top" colspan="2">
			<YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
		    <br />
			<asp:CheckBox ID="TopicWatch" runat="server" />
			<YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" LocalizedTag="TOPICWATCH" />
			<br id="TopicAttachBr" runat="server" />
			<asp:CheckBox ID="TopicAttach" runat="server" Visible="false" />
			<YAF:LocalizedLabel ID="TopicAttachLabel" runat="server" LocalizedTag="TOPICATTACH"
				Visible="false" />
		</td>
	</tr>
	<tr id="tr_captcha1" runat="server" visible="false">
		<td class="postformheader" valign="top" colspan="2">
			<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
			<asp:Image ID="imgCaptcha" runat="server" />
		</td>
	</tr>
	<tr id="tr_captcha2" runat="server" visible="false">
		<td class="postformheader" valign="top" colspan="2">
			<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Enter" />
			<asp:TextBox ID="tbCaptcha" runat="server" />
		</td>
	</tr>
	<tr id="EditReasonRow" runat="server">
		<td class="postformheader" width="20%" colspan="2" style="width: 100%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="EditReason" />
			<asp:TextBox ID="ReasonEditor" runat="server" CssClass="edit" Width="400" />
		</td>
	</tr>
	<tr>
		<td class="footer1" colspan="2">
			<YAF:ThemeButton ID="Preview" runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="Preview_Click" TextLocalizedTag="PREVIEW" />
			<YAF:ThemeButton ID="PostReply" runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="PostReply_Click" TextLocalizedTag="SAVE" />
			<YAF:ThemeButton ID="Cancel" runat="server" CssClass="yafcssbigbutton leftItem" OnClick="Cancel_Click"
				TextLocalizedTag="CANCEL" />
		</td>
	</tr>
</table>

<script type="text/javascript">

	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}

</script>

<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
