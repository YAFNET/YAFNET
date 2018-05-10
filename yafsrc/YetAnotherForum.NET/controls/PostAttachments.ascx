<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostAttachments"
    CodeBehind="PostAttachments.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>

<tr class="forumRowCat header2">
        <td colspan="2">
            <YAF:CollapsibleImage ID="CollapsibleImage" runat="server" BorderWidth="0" 
                ImageAlign="Bottom" PanelID="AttachmentsHolder" 
                AttachedControlID="AttachmentsHolder" ToolTip='<%# this.GetText("COMMON", "SHOWHIDE") %>'
                DefaultState="Collapsed"  />
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
            <p style="text-align:center">
                <YAF:LocalizedLabel ID="LoadingText" runat="server" LocalizedTag="LOADING"></YAF:LocalizedLabel>
                <br /><asp:Image ID="LoadingImage" runat="server" />
            </p>
        </div>
        <div id="AttachmentsListBox" class="content">
            <div id="PostAttachmentListPlaceholder" 
                data-url='<%= YafForumInfo.ForumClientFileRoot %>' 
                data-userid='<%= YAF.Core.YafContext.Current.PageUserID %>'
                data-notext='<%= this.GetText("ATTACHMENTS", "NO_ATTACHMENTS") %>' 
                style="clear: both;">
                <ul class="PostAttachmentList">
                </ul>
            </div>
            <span class="UploadNewFileLine">
               <a class="OpenUploadDialog yaflittlebutton">
                   <span>
                       <YAF:LocalizedLabel ID="ThemeButton1" LocalizedTag="UPLOAD_NEW" LocalizedPage="ATTACHMENTS" runat="server" />
                   </span>
               </a>
            </span>
        </div>
    </td>
</tr>
</asp:PlaceHolder>