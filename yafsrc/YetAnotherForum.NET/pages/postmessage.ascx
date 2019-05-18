<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.postmessage" Codebehind="postmessage.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostOptions" Src="../controls/PostOptions.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostAttachments" Src="../controls/PostAttachments.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AttachmentsUploadDialog" Src="../Dialogs/AttachmentsUpload.ascx" %>

<YAF:PageLinks ID="PageLinks" runat="server" />

<YAF:PollList ID="PollList"  ShowButtons="true" PollGroupId='<%# this.GetPollGroupID() %>'  runat="server"/>

<div class="row">
    <div class="col-xl-12">
        <h2></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-comment-dots fa-fw"></i>&nbsp;<asp:Label ID="Title" runat="server" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder id="PreviewRow" runat="server" visible="false">
                    <asp:Label runat="server">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
                    </asp:Label>
                    <asp:PlaceHolder id="PreviewCell" runat="server">
                        <YAF:Alert Type="light" runat="server">
                            <YAF:MessagePost ID="PreviewMessagePost" runat="server" />
                        </YAF:Alert>
                    </asp:PlaceHolder>
	            </asp:PlaceHolder>
	            <asp:PlaceHolder id="SubjectRow" runat="server">
                    <div class="form-group">
		                <asp:Label runat="server" AssociatedControlID="TopicSubjectTextBox">
                            <YAF:LocalizedLabel ID="TopicSubjectLabel" runat="server" LocalizedTag="subject" />
                        </asp:Label>
                        <asp:TextBox ID="TopicSubjectTextBox" runat="server" 
                                     CssClass="form-control" 
                                     MaxLength="100" 
                                     autocomplete="off" />
		            </div>
                    <div id="SearchResultsPlaceholder"  
                         data-url='<%=YafForumInfo.ForumClientFileRoot %>' 
                         data-userid='<%= YafContext.Current.PageUserID %>'>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="DescriptionRow" visible="false" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="TopicDescriptionTextBox">
                            <YAF:LocalizedLabel ID="TopicDescriptionLabel" runat="server" LocalizedTag="description" />
                        </asp:Label>
                        <asp:TextBox ID="TopicDescriptionTextBox" runat="server" 
                                     CssClass="form-control" 
                                     MaxLength="100" 
                                     autocomplete="off" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="FromRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="From">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="from" />
                        </asp:Label>
                        <asp:TextBox ID="From" runat="server" CssClass="form-control" />
		            </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="PriorityRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Priority">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="priority" />
                        </asp:Label>
                        <asp:DropDownList ID="Priority" runat="server" CssClass="standardSelectMenu" />
		            </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="StyleRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="TopicStylesTextBox">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="STYLES" />
                        </asp:Label>
                        <asp:TextBox id="TopicStylesTextBox" runat="server" CssClass="form-control" />
		            </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <asp:Label runat="server">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="message" />
                    </asp:Label>
                    <asp:PlaceHolder id="EditorLine" runat="server">
                        <!-- editor goes here -->
                    </asp:PlaceHolder>
                </div>
                <asp:PlaceHolder runat="server" id="maxCharRow">
            <YAF:Alert runat="server" Type="info">
                <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NOTE" LocalizedPage="COMMON" /></strong>
                <YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server" LocalizedTag="MAXNUMBEROF" />
            </YAF:Alert>
                </asp:PlaceHolder>
                <YAF:PostOptions id="PostOptions1" runat="server">
                </YAF:PostOptions>

    <YAF:PostAttachments id="PostAttachments1" runat="server" Visible="False">
    </YAF:PostAttachments>

                <asp:PlaceHolder id="tr_captcha1" runat="server" visible="false">
        <div class="form-group">
            <asp:Label runat="server">
                <YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
            </asp:Label>
            <asp:Image ID="imgCaptcha" runat="server" />
        </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="tr_captcha2" runat="server" visible="false">
                    <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="tbCaptcha">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Enter" />
                    </asp:Label>
                    <asp:TextBox ID="tbCaptcha" runat="server" CssClass="form-control" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder id="EditReasonRow" runat="server">
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="ReasonEditor">
                <YAF:LocalizedLabel runat="server" LocalizedTag="EditReason" />
            </asp:Label>
			<asp:TextBox ID="ReasonEditor" runat="server" CssClass="form-control" />
		</div>
            </asp:PlaceHolder>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="Preview" runat="server"
                                 OnClick="Preview_Click" 
                                 TextLocalizedTag="PREVIEW" TitleLocalizedTag="PREVIEW_TITLE"
                                 Type="Secondary" Icon="image" />
                <YAF:ThemeButton ID="PostReply"  runat="server"
                                 OnClick="PostReply_Click" 
                                 TextLocalizedTag="SAVE" TitleLocalizedTag="SAVE_TITLE" 
                                 Type="Primary" Icon="save" />
                <YAF:ThemeButton ID="Cancel" runat="server"
                                 OnClick="Cancel_Click"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary" Icon="times" />
            </div>
        </div>
    </div>
</div>


<script type="text/javascript">

	var prm = Sys.WebForms.PageRequestManager.getInstance();

	prm.add_beginRequest(beginRequest);

	function beginRequest() {
		prm._scrollPosition = null;
	}

</script>

<YAF:LastPosts ID="LastPosts1" runat="server" Visible="false" />
<YAF:AttachmentsUploadDialog ID="UploadDialog" runat="server" Visible="False"></YAF:AttachmentsUploadDialog>