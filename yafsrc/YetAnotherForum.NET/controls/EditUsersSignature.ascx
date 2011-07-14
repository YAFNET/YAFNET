<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersSignature" Codebehind="EditUsersSignature.ascx.cs" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr runat="server" id="trHeader">
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel runat="server" LocalizedPage="CP_SIGNATURE" LocalizedTag="title" />
        </td>
    </tr>
    <tr>
        <td class="header2" valign="top">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="signature" />
        </td>
        <td class="header2">
           <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="SIGNATURE_PREVIEW" />
        </td>
        
    </tr>
    <tr>
      <td class="postformheader" valign="top"></td>
      <td class="post" id="PreviewLine" runat="server">
      </td>
    </tr>
    <tr>
       <td class="postformheader" valign="top"></td>
       <td class="header2">
           <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="TITLE" />
        </td>
    </tr>
    <tr>
      <td class="postformheader" valign="top"></td>
      <td class="post" id="EditorLine" runat="server">
            <!-- editor goes here -->
        </td>
    </tr>
    <tr>
        <td class="postformheader" valign="top"></td>
        <td class="header2">
           <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="CP_SIGNATURE"
                LocalizedTag="SIGNATURE_PERMISSIONS" />
        </td>       
    </tr>
    <tr>
        <td class="postformheader" valign="top"></td>
        <td class="post">
            <asp:Label ID="TagsAllowedWarning" runat="server" />
        </td>       
    </tr>
    <tr>
        <td class="footer1" colspan="2" align="center">
            <asp:Button ID="preview" CssClass="pbutton" runat="server" />
            <asp:Button ID="save" CssClass="pbutton" runat="server" />
            <asp:Button ID="cancel" CssClass="pbutton" runat="server" />
        </td>
    </tr>
</table>
