<%@ Control language="c#" Codebehind="hostsettings.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.hostsettings" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:adminmenu runat="server">

<table class=content cellspacing=1 cellpadding=0 align=center>
	<tr>
		<td colspan=2 class=header1>Forum Settings</td>
	</tr>
	<tr>
		<td class="header2" colspan="2">Forum Setup</td>
	</tr>
	<tr>
		<td width="50%" class=postheader><b>MS SQL Server Version:</b><br>What version of MS SQL Server is running.</td>
		<td width="50%" class=post><asp:label id=SQLVersion cssclass=smallfont runat=server></asp:label></td>
	</tr>
	<tr>
		<td class=postheader><b>Time Zone:</b><br>The time zone of the web server.</td>
		<td class=post><asp:dropdownlist id=TimeZones runat=server DataTextField="Name" DataValueField="Value"></asp:dropdownlist></td>
	</tr>
	<tr>
		<td class=postheader><b>Forum Email:</b><br/>The from address when sending emails to users.</td>
		<td class=post><asp:TextBox style="width:300px" id=ForumEmailEdit runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Require Email Verification:</b><br/>If unchecked users will not need to verify their email address.</td>
		<td class=post><asp:checkbox id="EmailVerification" runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Show Moved Topics:</b><br/>If this is checked, topics that are moved will leave behind a pointer to the new topic.</td>
		<td class=post><asp:checkbox id="ShowMoved" runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Links in New Window:</b><br/>If this is checked, links in messages will open in a new window.</td>
		<td class=post><asp:checkbox id="BlankLinks" runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Show Groups:</b><br/>Should the groups a user is part of be visible on the posts page.</td>
		<td class=post><asp:checkbox id="ShowGroupsX" runat="server"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Allow Rich Edit:</b><br/>If this is checked your users will be able to use a rich edit control when posting messages.</td>
		<td class="post"><asp:checkbox runat="server" id="AllowRichEditX"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Allow User Theme:</b><br/>Should users be able to choose what theme they want to use?</td>
		<td class="post"><asp:checkbox runat="server" id="AllowUserThemeX"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Allow User Language:</b><br/>Should users be able to choose what language they want to use?</td>
		<td class="post"><asp:checkbox runat="server" id="AllowUserLanguageX"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Use File Table:</b><br/>Uploaded files will be saved in the database instead of the file system.</td>
		<td class="post"><asp:checkbox runat="server" id="UseFileTableX"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Max File Size:</b><br/>Maximum size of uploaded files. Leave empty for no limit.</td>
		<td class=post><asp:TextBox id=MaxFileSize runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan="2">SMTP Server Settings</td>
	</tr>
	<tr>
		<td class=postheader><b>SMTP Server:</b><br>To be able to send posts you need to enter the name of a valid smtp server.</td>
		<td class=post><asp:TextBox style="width:300px" id=ForumSmtpServer runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>SMTP User Name:</b><br>If you need to be authorized to send email.</td>
		<td class=post><asp:TextBox style="width:300px" id=ForumSmtpUserName runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>SMTP Password:</b><br>If you need to be authorized to send email.</td>
		<td class=post><asp:TextBox style="width:300px" id=ForumSmtpUserPass runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan="2">Avatar Settings</td>
	</tr>
	<tr>
		<td class="postheader"><b>Allow remote avatars:</b><br/>Can users use avatars from other websites.</td>
		<td class="post"><asp:checkbox runat="server" id="AvatarRemote"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Allow avatar uploading:</b><br/>Can users upload avatars to their profile.</td>
		<td class="post"><asp:checkbox runat="server" id="AvatarUpload"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Avatar Width:</b><br/>Maximum width for avatars.</td>
		<td class=post><asp:textbox id="AvatarWidth" runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Avatar Height:</b><br/>Maximum height for avatars.</td>
		<td class=post><asp:textbox id="AvatarHeight" runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><b>Avatar Size:</b><br/>Maximum size for avatars in bytes.</td>
		<td class=post><asp:textbox id="AvatarSize" runat="server"/></td>
	</tr>
	<!--tr>
		<td class="header2" colspan="2">Forum Moderator Access</td>
	</tr>
	<tr>
		<td class="postheader"><b>Groups and Users:</b><br/>Forum moderators can access groups and users administration.</td>
		<td class="post">...</td>
	</tr>
	<tr>
		<td class="postheader"><b>Forum:</b><br/>Forum moderators can access forum management.</td>
		<td class="post">...</td>
	</tr>
	<tr>
		<td class="postheader"><b>...</b><br/>...</td>
		<td class="post">...</td>
	</tr-->

	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=Save runat="server" Text="Save"/></td>
	</tr>
</table>

</yaf:adminmenu>

<yaf:savescrollpos runat="server"/>
