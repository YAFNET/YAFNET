<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" Codebehind="postmessage.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Register TagPrefix="YAF" TagName="smileys" Src="../controls/smileys.ascx" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostOptions" Src="../controls/PostOptions.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostAttachments" Src="../controls/PostAttachments.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AttachmentsUploadDialog" Src="../controls/AttachmentsUploadDialog.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:PollList ID="PollList"  ShowButtons="true" PollGroupId='<%# GetPollGroupID() %>'  runat="server"/>
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
		<td id="PreviewCell" runat="server" class="post previewPostContent ceebox" valign="top">
			<YAF:MessagePost ID="PreviewMessagePost" runat="server" />
		</td>
	</tr>
	<tr id="SubjectRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel ID="TopicSubjectLabel" runat="server" LocalizedTag="subject" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="TopicSubjectTextBox" runat="server" CssClass="edit" MaxLength="100" Width="400" autocomplete="off" />
		</td>
	</tr>
    <tr id="DescriptionRow" visible="false" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel ID="TopicDescriptionLabel" runat="server" LocalizedTag="description" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox ID="TopicDescriptionTextBox" runat="server" CssClass="edit" MaxLength="100" Width="400" autocomplete="off" />
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
    <tr id="StatusRow" visible="false" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="Status" />
		</td>
		<td class="post" width="80%">
			<asp:DropDownList ID="TopicStatus" runat="server" CssClass="standardSelectMenu" Width="400">
            </asp:DropDownList>
		</td>
	</tr>	
	<tr id="PriorityRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel runat="server" LocalizedTag="priority" />
		</td>
		<td class="post" width="80%">
			<asp:DropDownList ID="Priority" runat="server" CssClass="standardSelectMenu" Width="400" />
		</td>
	</tr>
    <tr id="StyleRow" runat="server">
		<td class="postformheader" width="20%">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="STYLES" />
		</td>
		<td class="post" width="80%">
			<asp:TextBox id="TopicStylesTextBox" runat="server" CssClass="edit" Width="400" />
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
    
    <YAF:PostOptions id="PostOptions1" runat="server">
    </YAF:PostOptions>
    
    <YAF:PostAttachments id="PostAttachments1" runat="server" Visible="False">
    </YAF:PostAttachments>

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
				OnClick="Preview_Click" TitleLocalizedTag="PREVIEW_TITLE"  TextLocalizedTag="PREVIEW" />
			<YAF:ThemeButton ID="PostReply" TitleLocalizedTag="SAVE_TITLE"  runat="server" CssClass="yafcssbigbutton leftItem"
				OnClick="PostReply_Click" TextLocalizedTag="SAVE" />
			<YAF:ThemeButton ID="Cancel" TitleLocalizedTag="CANCEL_TITLE"  runat="server" CssClass="yafcssbigbutton leftItem" OnClick="Cancel_Click"
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
<YAF:AttachmentsUploadDialog ID="UploadDialog" runat="server" Visible="False"></YAF:AttachmentsUploadDialog>