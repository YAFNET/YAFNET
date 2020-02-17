<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostAttachments"
    CodeBehind="PostAttachments.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>

<YAF:ThemeButton runat="server"
                 Type="Secondary"
                 DataToggle="collapse"
                 DataTarget="Attachments"
                 CssClass="mt-3 mb-3"
                 Icon="paperclip"
                 TextLocalizedTag="BUTTON_ATTACH_TT"
></YAF:ThemeButton>
<asp:PlaceHolder ID="AttachmentsHolder" runat="server">
    <div class="collapse" id="Attachments">
        <div class="card">
            <div class="card-header">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="CURRENT_UPLOADS" LocalizedPage="ATTACHMENTS" />
            </div>
            <div class="card-body">
        <div id="AttachmentsListPager"></div>
        <div id="PostAttachmentLoader">
            <div style="text-align:center">
                <YAF:LocalizedLabel ID="LoadingText" runat="server" LocalizedTag="LOADING"></YAF:LocalizedLabel>
                <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
            </div>
        </div>
        <div id="AttachmentsListBox" class="content">
            <div id="PostAttachmentListPlaceholder"
                data-url="<%= BoardInfo.ForumClientFileRoot %>"
                data-userid="<%= BoardContext.Current.PageUserID %>"
                data-notext="<%= this.GetText("ATTACHMENTS", "NO_ATTACHMENTS") %>"
                style="clear: both;">
                <ul class="PostAttachmentList">
                </ul>
            </div>
            <span class="UploadNewFileLine">
               <a class="OpenUploadDialog" data-toggle="modal" data-target="#UploadDialog">
                   <span class="btn btn-primary">
                       <i class="fa fa-file-upload fa-fw"></i>&nbsp;
                       <YAF:LocalizedLabel ID="ThemeButton1" LocalizedTag="UPLOAD_NEW" LocalizedPage="ATTACHMENTS" runat="server" />
                   </span>
               </a>
            </span>
        </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>