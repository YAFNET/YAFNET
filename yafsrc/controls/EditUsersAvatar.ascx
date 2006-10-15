<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersAvatar.ascx.cs" Inherits="yaf.controls.EditUsersAvatar" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="editor" Namespace="yaf.editor" Assembly="yaf" %>

<table width="100%" class="content" cellspacing="1" cellpadding="4">
    <tr>
        <td class="header1" colspan="4">
            <%= ForumPage.GetText( "CP_EDITAVATAR", "title" )%>
        </td>
    </tr>
	<tr runat="server" id="AvatarCurrentText">
		<td class="header2"><%= ForumPage.GetText( "CP_EDITAVATAR", "AvatarCurrent" )%></td>
		<td class="header2" colspan="3"><%= ForumPage.GetText( "CP_EDITAVATAR", "AvatarNew" )%></td>
	</tr>
	<tr>
		<td class="post" align="center" rowspan="4" runat="server" id="avatarImageTD"><asp:image id="AvatarImg" runat="server" visible="true"/>		
		<br /><br />
		<asp:Label runat="server" ID="NoAvatar" Visible="false" />
		<asp:button runat="server" id="DeleteAvatar" visible="false" OnClick="DeleteAvatar_Click"/></td>
	</tr>
	<tr runat="server" id="AvatarOurs">
		<td class="postheader"><%= ForumPage.GetText( "CP_EDITAVATAR", "ouravatar" )%></td>
		<td class="post" colspan="2">[ <asp:HyperLink id="OurAvatar" runat="server" /> ]</td>
	</tr>
	<tr runat="server" id="AvatarRemoteRow">
		<td class="postheader"><%= ForumPage.GetText( "CP_EDITAVATAR", "avatarremote" )%></td>
		<td class="post"><asp:textbox cssclass="edit" id="Avatar" runat="server"/></td>
		<td class="post"><asp:Button ID="UpdateRemote" CssClass="pbutton" runat="server" OnClick="RemoteUpdate_Click" /></td>
	</tr>	
	<tr runat="server" id="AvatarUploadRow">
		<td class="postheader"><%= ForumPage.GetText( "CP_EDITAVATAR", "avatarupload" )%></td>
		<td class="post"><input type="file" id="File" runat="server"/></td>
		<td class="post"><asp:Button ID="UpdateUpload" CssClass="pbutton" runat="server" OnClick="UploadUpdate_Click" /></td>
	</tr>
    <tr>
        <td class="footer1" colspan="4" align="center">
            <asp:Button ID="Back" CssClass="pbutton" runat="server" OnClick="Back_Click" />
        </td>
    </tr>
</table>	