<%@ Control Language="c#" CodeBehind="../../../pages/postmessage.ascx.cs" AutoEventWireup="True"Inherits="YAF.Pages.postmessage" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../../../controls/PollList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="smileys" Src="../../../controls/smileys.ascx" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../../../controls/LastPosts.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostOptions" Src="../../../controls/PostOptions.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostAttachments" Src="../../../controls/PostAttachments.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AttachmentsUploadDialog" Src="../../../controls/AttachmentsUploadDialog.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:PollList ID="PollList" ShowButtons="true" PollGroupId='<%# GetPollGroupID() %>'
    runat="server" />
<table align="center" cellpadding="4" cellspacing="1" class="content" width="100%">
    <tr>
        <td align="center" class="header1" colspan="2">
            <asp:Label ID="Title" runat="server" />
        </td>
    </tr>
    <tr id="PreviewRow" runat="server" visible="false">
        <td id="PreviewCell" runat="server" class="post" valign="top" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="previewtitle" />
            <br />
            <YAF:MessagePost ID="PreviewMessagePost" runat="server" />
        </td>
    </tr>
    <tr id="SubjectRow" runat="server">
        <td class="postformheader"  width="20%" colspan="2">
            <YAF:LocalizedLabel ID="TopicSubjectLabel" runat="server" LocalizedTag="subject" />
            <br />
            <asp:TextBox ID="TopicSubjectTextBox" runat="server" CssClass="edit" MaxLength="100"
                Width="400" />
        </td>
    </tr>
    <tr id="DescriptionRow" visible="false" runat="server">
		<td class="postformheader"  width="20%" colspan="2">
			<YAF:LocalizedLabel ID="TopicDescriptionLabel" runat="server" LocalizedTag="description" />
            <br />
            <asp:TextBox ID="TopicDescriptionTextBox" runat="server" CssClass="edit" MaxLength="100" Width="400" />
		</td>
	</tr>
    <tr id="BlogRow" runat="server" visible="false">
        <td class="postformheader" width="20%" colspan="2">
            Post to blog?
            <br />
            <asp:CheckBox ID="PostToBlog" runat="server" />
            Blog Password:
            <asp:TextBox ID="BlogPassword" runat="server" TextMode="Password" Width="400" />
            <asp:HiddenField ID="BlogPostID" runat="server" />
        </td>
    </tr>
    <tr id="FromRow" runat="server">
        <td class="postformheader" width="20%" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="from" />
            <br />
            <asp:TextBox ID="From" runat="server" CssClass="edit" Width="400" />
        </td>
    </tr>
    <tr id="StatusRow" visible="false" runat="server">
		<td class="postformheader" width="20%"  colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="Status" />
            <br />
            <asp:DropDownList ID="TopicStatus" runat="server" CssClass="edit" Width="400">
              <asp:ListItem Text="INFORMATIC" Value="informatic"></asp:ListItem>
              <asp:ListItem Text="QUESTION" Value="question"></asp:ListItem>
              <asp:ListItem Text="SOLVED" Value="solved"></asp:ListItem>
              <asp:ListItem Text="ISSUE" Value="issue"></asp:ListItem>
              <asp:ListItem Text="FIXED" Value="fixed"></asp:ListItem>
            </asp:DropDownList>
		</td>
	</tr>	
    <tr id="PriorityRow" runat="server">
        <td class="postformheader" width="20%" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="priority" />
            <br />
            <asp:DropDownList ID="Priority" runat="server" />
        </td>
    </tr>
    <tr id="StyleRow" runat="server">
		<td class="postformheader" width="20%" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="STYLES" />
            <br />
			<asp:TextBox id="TopicStylesTextBox" runat="server" CssClass="edit" Width="400" />
		</td>
	</tr>
    <tr>
        <td id="EditorLine" runat="server" class="post" width="80%" colspan="2">
            <b>
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="message" />
            </b>
            <br />
            <asp:PlaceHolder runat="server" Visible="false" ID="SmilesHolder">
                <YAF:smileys ID="Smileys1" runat="server" OnClick="insertsmiley" />
                <br />
                <YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server" LocalizedTag="MAXNUMBEROF" />
            </asp:PlaceHolder>
            <!-- editor goes here -->
        </td>
    </tr>
    <YAF:PostOptions ID="PostOptions1" runat="server"></YAF:PostOptions>
    <YAF:PostAttachments id="PostAttachments1" runat="server" Visible="False">
    </YAF:PostAttachments>
    <tr id="tr_captcha1" runat="server" visible="false">
        <td class="postformheader" valign="top" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="Captcha_Image" />
            <br />
            <asp:Image ID="imgCaptcha" runat="server" />
        </td>
    </tr>
    <tr id="tr_captcha2" runat="server" visible="false">
        <td class="postformheader" valign="top" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="Captcha_Enter" />
            <br />
            <asp:TextBox ID="tbCaptcha" runat="server" />
        </td>
    </tr>
    <tr id="EditReasonRow" runat="server">
        <td class="postformheader" width="20%" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="EditReason" />
            <br />
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
<asp:PlaceHolder ID="LastPostsHolder" runat="server" Visible="false">
    <YAF:LastPosts ID="LastPosts1" runat="server" Visible="false" />
</asp:PlaceHolder>
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
<YAF:AttachmentsUploadDialog ID="UploadDialog" runat="server" Visible="False"></YAF:AttachmentsUploadDialog>
