<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.PostAttachments"
    CodeBehind="PostAttachments.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>

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
        <div id="AttachmentsListBox" class="content">
            <div id="AttachmentsListPager"></div>
            <br style="clear:both;" />
            <div id="AttachmentsPagerResult">
                <p style="text-align:center"><asp:Label ID="LoadingText" runat="server"></asp:Label><br /><asp:Image ID="LoadingImage" runat="server" /></p>
            </div>

            <div id="AttachmentsPagerHidden" style="display:none;">
                <asp:Literal ID="AttachmentsResults" runat="server" />
            </div>
            <span class="UploadNewFileLine">
                <YAF:ThemeButton ID="UploadNew" TextLocalizedTag="UPLOAD_NEW" TextLocalizedPage="ATTACHMENTS" runat="server"
                    CssClass="OpenUploadDialog yaflittlebutton">
                </YAF:ThemeButton>
            </span>
        </div>
    </td>
</tr>
</asp:PlaceHolder>