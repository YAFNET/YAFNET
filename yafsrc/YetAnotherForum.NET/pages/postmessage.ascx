<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" Codebehind="postmessage.ascx.cs" %>
<%@ Import Namespace="YAF.Classes.Core" %>
<%@ Register TagPrefix="YAF" TagName="smileys" Src="../controls/smileys.ascx" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostOptions" Src="../controls/PostOptions.ascx" %>
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
	<asp:Repeater ID="ChoiceRepeater" runat="server" Visible="false" >
    <HeaderTemplate>
      </HeaderTemplate>
    <ItemTemplate>
        <tr>
    <td class="postformheader" width="20%">
           <em>
<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="choice" Param0='<%# DataBinder.Eval(Container.DataItem, "ChoiceOrderID") %>' />
</em>
</td>
<td class="post" width="80%">
<asp:HiddenField ID="PollChoiceID"  Value='<%# DataBinder.Eval(Container.DataItem, "ChoiceID") %>' runat="server" />
<asp:TextBox ID="PollChoice" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Choice") %>' CssClass="edit" MaxLength="50" Width="400" />
</td>
</tr>
</ItemTemplate>
<FooterTemplate>
     </FooterTemplate>
</asp:Repeater>
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
	
    <YAF:PostOptions id="PostOptions1" runat="server">
    </YAF:PostOptions>

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
