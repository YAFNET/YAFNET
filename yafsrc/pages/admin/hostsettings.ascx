<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="hostsettings.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.hostsettings" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<TABLE class="content" cellSpacing="1" cellPadding="0" align="center">
		<TR>
			<TD class="header1" colSpan="2">Forum Settings</TD>
		</TR>
		<TR>
			<TD class="header2" colSpan="2" align="center">Forum Setup</TD>
		</TR>
		<TR>
			<TD class="postheader" width="50%"><B>MS SQL Server Version:</B><BR>
				What version of MS SQL Server is running.</TD>
			<TD class="post" width="50%">
				<asp:label id="SQLVersion" runat="server" cssclass="smallfont"></asp:label></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Time Zone:</B><BR>
				The time zone of the web server.</TD>
			<TD class="post">
				<asp:dropdownlist id="TimeZones" runat="server" DataValueField="Value" DataTextField="Name"></asp:dropdownlist></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Forum Email:</B><BR>
				The from address when sending emails to users.</TD>
			<TD class="post">
				<asp:TextBox id="ForumEmailEdit" runat="server"></asp:TextBox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Require Email Verification:</B><BR>
				If unchecked users will not need to verify their email address.</TD>
			<TD class="post">
				<asp:checkbox id="EmailVerification" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Show Moved Topics:</B><BR>
				If this is checked, topics that are moved will leave behind a pointer to the 
				new topic.</TD>
			<TD class="post">
				<asp:checkbox id="ShowMoved" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Links in New Window:</B><BR>
				If this is checked, links in messages will open in a new window.</TD>
			<TD class="post">
				<asp:checkbox id="BlankLinks" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Show Groups:</B><BR>
				Should the groups a user is part of be visible on the posts page.</TD>
			<TD class="post">
				<asp:checkbox id="ShowGroupsX" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Allow Rich Edit:</B><BR>
				If this is checked your users will be able to use a rich edit control when 
				posting messages.</TD>
			<TD class="post">
				<asp:checkbox id="AllowRichEditX" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Allow User Theme:</B><BR>
				Should users be able to choose what theme they want to use?</TD>
			<TD class="post">
				<asp:checkbox id="AllowUserThemeX" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Allow User Language:</B><BR>
				Should users be able to choose what language they want to use?</TD>
			<TD class="post">
				<asp:checkbox id="AllowUserLanguageX" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Use File Table:</B><BR>
				Uploaded files will be saved in the database instead of the file system.</TD>
			<TD class="post">
				<asp:checkbox id="UseFileTableX" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Show RSS Links:</B><BR>
				Enable or disable display of RSS links throught the forum.</TD>
			<TD class="post">
				<asp:checkbox id="ShowRSSLinkX" runat="server"></asp:checkbox></TD>
		</TR>		
		<TR>
			<TD class="postheader"><B>Max File Size:</B><BR>
				Maximum size of uploaded files. Leave empty for no limit.</TD>
			<TD class="post">
				<asp:TextBox id="MaxFileSize" runat="server"></asp:TextBox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Smilies Display Grid Size:</B><BR>
				Number of smilies to show by number of rows and columns.</TD>
			<TD class="post">
				<asp:TextBox id="SmiliesPerRow" runat="server"></asp:TextBox>
				<b>x</b>
				<asp:TextBox id="SmiliesColumns" runat="server"></asp:TextBox>
				</TD>
		</TR>		
		<TR>
			<TD class="header2" colSpan="2" align="center">SMTP Server Settings</TD>
		</TR>
		<TR>
			<TD class="postheader"><B>SMTP Server:</B><BR>
				To be able to send posts you need to enter the name of a valid smtp server.</TD>
			<TD class="post">
				<asp:TextBox id="ForumSmtpServer" runat="server"></asp:TextBox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>SMTP User Name:</B><BR>
				If you need to be authorized to send email.</TD>
			<TD class="post">
				<asp:TextBox id="ForumSmtpUserName" runat="server"></asp:TextBox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>SMTP Password:</B><BR>
				If you need to be authorized to send email.</TD>
			<TD class="post">
				<asp:TextBox id="ForumSmtpUserPass" runat="server"></asp:TextBox></TD>
		</TR>
		<TR>
			<TD class="header2" colSpan="2" align="center">Avatar Settings</TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Allow remote avatars:</B><BR>
				Can users use avatars from other websites.</TD>
			<TD class="post">
				<asp:checkbox id="AvatarRemote" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Allow avatar uploading:</B><BR>
				Can users upload avatars to their profile.</TD>
			<TD class="post">
				<asp:checkbox id="AvatarUpload" runat="server"></asp:checkbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Avatar Width:</B><BR>
				Maximum width for avatars.</TD>
			<TD class="post">
				<asp:textbox id="AvatarWidth" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Avatar Height:</B><BR>
				Maximum height for avatars.</TD>
			<TD class="post">
				<asp:textbox id="AvatarHeight" runat="server"></asp:textbox></TD>
		</TR>
		<TR>
			<TD class="postheader"><B>Avatar Size:</B><BR>
				Maximum size for avatars in bytes.</TD>
			<TD class="post">
				<asp:textbox id="AvatarSize" runat="server"></asp:textbox></TD>
		</TR> <!--tr>
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
		<TR>
			<TD class="postfooter" align="center" colSpan="2">
				<asp:Button id="Save" runat="server" Text="Save"></asp:Button></TD>
		</TR>
	</TABLE>
</yaf:adminmenu>
<yaf:savescrollpos runat="server" id="Savescrollpos1" />
