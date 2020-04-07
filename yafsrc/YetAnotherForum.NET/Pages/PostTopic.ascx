<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PostTopic" CodeBehind="PostTopic.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostOptions" Src="../controls/PostOptions.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostAttachments" Src="../controls/PostAttachments.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AttachmentsUploadDialog" Src="../Dialogs/AttachmentsUpload.ascx" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="row">
    <div class="col-xl-12">
        <h2></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:Icon runat="server"
                          IconName="comment-medical"
                          IconType="text-secondary">
                </YAF:Icon>
                <asp:Label ID="Title" runat="server" />
            </div>
            <div class="card-body">
                <asp:PlaceHolder ID="PreviewRow" runat="server" Visible="false">
                    <asp:Label runat="server">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
                    </asp:Label>
                    <asp:PlaceHolder ID="PreviewCell" runat="server">
                        <YAF:Alert Type="light" runat="server">
                            <YAF:MessagePost ID="PreviewMessagePost" runat="server"/>
                        </YAF:Alert>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="SubjectRow" runat="server">
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
                        data-url="<%=BoardInfo.ForumClientFileRoot %>"
                        data-userid="<%= BoardContext.Current.PageUserID %>">
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="DescriptionRow" Visible="false" runat="server">
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
                <asp:PlaceHolder ID="FromRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="From">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="from" />
                        </asp:Label>
                        <asp:TextBox ID="From" runat="server" CssClass="form-control" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="PriorityRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Priority">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="priority" />
                        </asp:Label>
                        <asp:DropDownList ID="Priority" runat="server" CssClass="select2-image-select" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="StyleRow" runat="server">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="TopicStylesTextBox">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="STYLES" />
                        </asp:Label>
                        <asp:TextBox ID="TopicStylesTextBox" runat="server" CssClass="form-control" />
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Tags">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="TAGS" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="Tags" 
                                 CssClass="form-control">
                    </asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:Label runat="server">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="message" />
                    </asp:Label>
                    <asp:PlaceHolder ID="EditorLine" runat="server">
                        <!-- editor goes here -->
                    </asp:PlaceHolder>
                </div>
                <asp:PlaceHolder runat="server" ID="maxCharRow">
                    <div class="d-sm-none d-md-block">
                    <YAF:Alert runat="server" Type="info">
                        <strong>
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NOTE" LocalizedPage="COMMON" />
                        </strong>
                        <YAF:LocalizedLabel ID="LocalizedLblMaxNumberOfPost" runat="server" 
                                            LocalizedTag="MAXNUMBEROF"/>
                    </YAF:Alert>
                    </div>
                </asp:PlaceHolder>
                <YAF:PostOptions ID="PostOptions1" runat="server"></YAF:PostOptions>

                <YAF:PostAttachments ID="PostAttachments1" runat="server" Visible="False"></YAF:PostAttachments>

                <asp:PlaceHolder ID="tr_captcha1" runat="server" Visible="false">
                    <div class="form-group">
                        <asp:Label runat="server">
                <YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
                        </asp:Label>
                        <asp:Image ID="imgCaptcha" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="tr_captcha2" runat="server" Visible="false">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="tbCaptcha">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Enter" />
                        </asp:Label>
                        <asp:TextBox ID="tbCaptcha" runat="server" CssClass="form-control" />
                    </div>
                </asp:PlaceHolder>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="Preview" runat="server"
                                 CssClass="mt-1"
                                 OnClick="Preview_Click"
                                 TextLocalizedTag="PREVIEW" TitleLocalizedTag="PREVIEW_TITLE"
                                 Type="Secondary" 
                                 Icon="image" />
                <YAF:ThemeButton ID="PostReply" runat="server"
                                 CssClass="mt-1"
                                 OnClick="PostReply_Click"
                                 TextLocalizedTag="NEWTOPIC"
                                 Type="Primary" 
                                 Icon="comment-medical" />
                <YAF:ThemeButton ID="Cancel" runat="server"
                                 CssClass="mt-1"
                                 OnClick="Cancel_Click"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary" 
                                 Icon="times" />
            </div>
        </div>
    </div>
</div>

<YAF:AttachmentsUploadDialog ID="UploadDialog" runat="server" Visible="False"></YAF:AttachmentsUploadDialog>
