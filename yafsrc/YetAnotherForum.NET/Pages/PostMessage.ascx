<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.PostMessage" CodeBehind="PostMessage.ascx.cs" %>

<%@ Register TagPrefix="YAF" TagName="LastPosts" Src="../controls/LastPosts.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PostOptions" Src="../controls/PostOptions.ascx" %>
<%@ Register TagPrefix="YAF" TagName="PollList" Src="../controls/PollList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="AttachmentsUploadDialog" Src="../Dialogs/AttachmentsUpload.ascx" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

<YAF:PollList ID="PollList" runat="server" />

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
                          IconName="comment-dots"
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
                <asp:PlaceHolder ID="FromRow" runat="server" Visible="False">
                    <div class="mb-3">
                        <asp:Label runat="server" AssociatedControlID="From">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="from" />
                        </asp:Label>
                        <asp:TextBox ID="From" runat="server" CssClass="form-control" />
                    </div>
                </asp:PlaceHolder>
                <div class="mb-3">
                    <asp:Label runat="server">
                        <YAF:LocalizedLabel runat="server" LocalizedTag="message" />
                    </asp:Label>
                    <asp:PlaceHolder ID="EditorLine" runat="server">
                        <!-- editor goes here -->
                    </asp:PlaceHolder>
                </div>
                <YAF:PostOptions ID="PostOptions1" runat="server"></YAF:PostOptions>

                <asp:PlaceHolder ID="tr_captcha1" runat="server" Visible="false">
                    <div class="mb-3">
                        <asp:Label runat="server">
                <YAF:LocalizedLabel runat="server" LocalizedTag="Captcha_Image" />
                        </asp:Label>
                        <asp:Image ID="imgCaptcha" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="tr_captcha2" runat="server" Visible="false">
                    <div class="mb-3">
                        <asp:Label runat="server" AssociatedControlID="tbCaptcha">
                            <YAF:LocalizedLabel runat="server" 
                                                LocalizedTag="Captcha_Enter" />
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
                                 TextLocalizedTag="SAVE" TitleLocalizedTag="SAVE_TITLE"
                                 Type="Primary" 
                                 Icon="save" />
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

<YAF:LastPosts ID="LastPosts1" runat="server" />
<YAF:AttachmentsUploadDialog ID="UploadDialog" runat="server" Visible="False"></YAF:AttachmentsUploadDialog>
