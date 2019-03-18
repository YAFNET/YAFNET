<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostAttachments"
    CodeBehind="PostAttachments.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>

<tr class="forumRowCat header2">
        <td colspan="2">
           
            <YAF:LocalizedLabel ID="NewPostOptionsLabel" runat="server" LocalizedTag="BUTTON_ATTACH_TT" LocalizedPage="BUTTON" />
        </td>
</tr>
<asp:PlaceHolder ID="AttachmentsHolder" runat="server">
<tr>
    <td>
        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="CURRENT_UPLOADS" LocalizedPage="ATTACHMENTS" />
    </td>
    <td class="post">
        <div id="AttachmentsListPager"></div>
        <div id="PostAttachmentLoader">
            <div style="text-align:center">
                <YAF:LocalizedLabel ID="LoadingText" runat="server" LocalizedTag="LOADING"></YAF:LocalizedLabel>
                <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
            </div>
        </div>
        <div id="AttachmentsListBox" class="content">
            <div id="PostAttachmentListPlaceholder"
                data-url='<%= YafForumInfo.ForumClientFileRoot %>'
                data-userid='<%= YafContext.Current.PageUserID %>'
                data-notext='<%= this.GetText("ATTACHMENTS", "NO_ATTACHMENTS") %>'
                style="clear: both;">
                <ul class="PostAttachmentList">
                </ul>
            </div>
            <span class="UploadNewFileLine">
               <a class="OpenUploadDialog" data-toggle="modal" data-target=".UploadDialog">
                   <span>
                       <YAF:LocalizedLabel ID="ThemeButton1" LocalizedTag="UPLOAD_NEW" LocalizedPage="ATTACHMENTS" runat="server" />
                   </span>
               </a>
            </span>
        </div>
    </td>
</tr>
</asp:PlaceHolder>