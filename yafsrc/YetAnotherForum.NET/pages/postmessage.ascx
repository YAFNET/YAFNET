<%@ Control Language="c#" CodeFile="postmessage.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="smileys" Src="../controls/smileys.ascx" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<table align="center" cellpadding="4" cellspacing="1" class="content" width="100%">
	<tr>
		<td align="center" class="header1" colspan="2">
		<asp:Label id="Title" runat="server" />
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
		<asp:TextBox id="Subject" runat="server" cssclass="edit" maxlength="100" width="400" />
		</td>
	</tr>
	<tr id="BlogRow" runat="server" visible="false">
		<td class="postformheader" width="20%">Post to blog? </td>
		<td class="post" width="80%">
		<asp:CheckBox id="PostToBlog" runat="server" />
		Blog Password:
		<asp:TextBox id="BlogPassword" runat="server" textmode="Password" width="400" />
		<asp:HiddenField id="BlogPostID" runat="server" />
		</td>
	</tr>
	<tr id="FromRow" runat="server">
		<td class="postformheader" width="20%">
		<YAF:LocalizedLabel runat="server" LocalizedTag="from" />
		</td>
		<td class="post" width="80%">
		<asp:TextBox id="From" runat="server" cssclass="edit" width="400" />
		</td>
	</tr>
	<tr id="PriorityRow" runat="server">
		<td class="postformheader" width="20%">
		<YAF:LocalizedLabel runat="server" LocalizedTag="priority" />
		</td>
		<td class="post" width="80%">
		<asp:DropDownList id="Priority" runat="server" />
		</td>
	</tr>
	<tr id="PersistencyRow" runat="server">
		<td class="postformheader" width="20%">
		<YAF:LocalizedLabel runat="server" LocalizedTag="PERSISTENCY" />
		</td>
		<td class="post" width="80%">
		<asp:CheckBox id="Persistency" runat="server" />
		(<YAF:LocalizedLabel runat="server" LocalizedTag="PERSISTENCY_INFO" />)
		</td>
	</tr>
	<tr id="CreatePollRow" runat="server">
		<td class="postformheader" width="20%">
		<YAF:ThemeButton ID="CreatePoll" runat="server" CssClass="yafcssbigbutton leftItem" OnClick="CreatePoll_Click" TextLocalizedTag="CREATEPOLL" />
		</td>
		<td class="post" width="80%">&nbsp; </td>
	</tr>
	<tr id="RemovePollRow" runat="server">
		<td class="postformheader" width="20%">
		<YAF:ThemeButton ID="RemovePoll" runat="server" CssClass="yafcssbigbutton leftItem" OnCommand="RemovePoll_Command" OnLoad="RemovePoll_Load" TextLocalizedTag="REMOVEPOLL" />
		</td>
		<td class="post" width="80%">&nbsp; </td>
	</tr>
	<tr id="PollRow1" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="pollquestion" />
		</em></td>
		<td class="post" width="80%">
		<asp:TextBox id="Question" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow2" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice1" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice1ID" runat="server" />
		<asp:TextBox id="PollChoice1" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow3" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice2" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice2ID" runat="server" />
		<asp:TextBox id="PollChoice2" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow4" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice3" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice3ID" runat="server" />
		<asp:TextBox id="PollChoice3" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow5" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice4" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice4ID" runat="server" />
		<asp:TextBox id="PollChoice4" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow6" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice5" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice5ID" runat="server" />
		<asp:TextBox id="PollChoice5" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow7" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice6" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice6ID" runat="server" />
		<asp:TextBox id="PollChoice6" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow8" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice7" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice7ID" runat="server" />
		<asp:TextBox id="PollChoice7" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow9" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice8" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice8ID" runat="server" />
		<asp:TextBox id="PollChoice8" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRow10" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="choice9" />
		</em></td>
		<td class="post" width="80%">
		<asp:HiddenField id="PollChoice9ID" runat="server" />
		<asp:TextBox id="PollChoice9" runat="server" cssclass="edit" maxlength="50" width="400" />
		</td>
	</tr>
	<tr id="PollRowExpire" runat="server" visible="false">
		<td class="postformheader" width="20%"><em>
		<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire" />
		</em></td>
		<td class="post" width="80%">
		<asp:TextBox id="PollExpire" runat="server" cssclass="edit" maxlength="10" width="400" />
		<YAF:LocalizedLabel runat="server" LocalizedTag="poll_expire_explain" />
		</td>
	</tr>
	<tr>
		<td class="postformheader" valign="top" width="20%">
		<YAF:LocalizedLabel runat="server" LocalizedTag="message" />
		<br />
		<YAF:smileys runat="server" onclick="insertsmiley" />
		<br />
		<YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server" LocalizedTag="MAXNUMBEROF">
            	
			
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 	
			&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </YAF:LocalizedLabel>
		</td>
		<td id="EditorLine" runat="server" class="post" width="80%">
		<!-- editor goes here --></td>
	</tr>
	<tr id="NewTopicOptionsRow" runat="server" visible="false">
		<td class="postformheader" valign="top">
		<YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="NEWPOSTOPTIONS" />
		</td>
		<td class="post">
		<asp:CheckBox id="TopicWatch" runat="server" />
		<YAF:LocalizedLabel ID="TopicWatchLabel" runat="server" LocalizedTag="TOPICWATCH" />
		<br id="TopicAttachBr" runat="server" />
		<asp:CheckBox id="TopicAttach" runat="server" visible="false" />
		<YAF:LocalizedLabel ID="TopicAttachLabel" runat="server" LocalizedTag="TOPICATTACH" Visible="false" />
		</td>
	</tr>
	<tr id="tr_captcha1" runat="server" visible="false">
		<td class="postformheader" valign="top">
		<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
		</td>
		<td class="post">
		<asp:Image id="imgCaptcha" runat="server" />
		</td>
	</tr>
	<tr id="tr_captcha2" runat="server" visible="false">
		<td class="postformheader" valign="top">
		<YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Enter" />
		</td>
		<td class="post">
		<asp:TextBox id="tbCaptcha" runat="server" />
		</td>
	</tr>
	<tr id="EditReasonRow" runat="server">
		<td class="postformheader" width="20%">
		<YAF:LocalizedLabel runat="server" LocalizedTag="EditReason" />
		</td>
		<td class="post" width="80%">
		<asp:TextBox id="ReasonEditor" runat="server" cssclass="edit" width="400" />
		</td>
	</tr>
	<tr>
		<td class="footer1">&nbsp; </td>
		<td class="footer1">
		<YAF:ThemeButton ID="Preview" runat="server" CssClass="yafcssbigbutton leftItem" OnClick="Preview_Click" TextLocalizedTag="PREVIEW" />
		<YAF:ThemeButton ID="PostReply" runat="server" CssClass="yafcssbigbutton leftItem" OnClick="PostReply_Click" TextLocalizedTag="SAVE" />
		<YAF:ThemeButton ID="Cancel" runat="server" CssClass="yafcssbigbutton leftItem" OnClick="Cancel_Click" TextLocalizedTag="CANCEL" />
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
<YAF:LastPosts id="LastPosts1" runat="server" visible="false" />
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
