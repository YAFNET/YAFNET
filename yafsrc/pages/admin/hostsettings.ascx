<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="hostsettings.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.hostsettings" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellSpacing="1" cellPadding="0" align="center">
		<tr>
			<td class="header1" colSpan="2">Forum Settings</td>
		</tr>
		<tr>
			<td class="header2" align="center" colSpan="2">Forum Setup</td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><B>MS SQL Server Version:</B><BR>
				What version of MS SQL Server is running.</td>
			<td class="post" width="50%">
				<asp:label id="SQLVersion" runat="server" cssclass="smallfont"></asp:label></td>
		</tr>
		<tr>
			<td class="postheader"><B>Time Zone:</B><BR>
				The time zone of the web server.</td>
			<td class="post">
				<asp:dropdownlist id="TimeZones" runat="server" DataValueField="Value" DataTextField="Name"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader"><B>Forum Email:</B><BR>
				The from address when sending emails to users.</td>
			<td class="post">
				<asp:TextBox id="ForumEmailEdit" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Require Email Verification:</B><BR>
				If unchecked users will not need to verify their email address.</td>
			<td class="post">
				<asp:checkbox id="EmailVerification" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Show Moved Topics:</B><BR>
				If this is checked, topics that are moved will leave behind a pointer to the 
				new topic.</td>
			<td class="post">
				<asp:checkbox id="ShowMoved" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Links in New Window:</B><BR>
				If this is checked, links in messages will open in a new window.</td>
			<td class="post">
				<asp:checkbox id="BlankLinks" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Show Groups:</B><BR>
				Should the groups a user is part of be visible on the posts page.</td>
			<td class="post">
				<asp:checkbox id="ShowGroupsX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Show Groups in profile:</B><BR>
				Should the groups a user is part of be visible on the users profile page.</td>
			<td class="post">
				<asp:checkbox id="ShowGroupsProfile" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Use File Table:</B><BR>
				Uploaded files will be saved in the database instead of the file system.</td>
			<td class="post">
				<asp:checkbox id="UseFileTableX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Show RSS Links:</B><BR>
				Enable or disable display of RSS links throughout the forum.</td>
			<td class="post">
				<asp:checkbox id="ShowRSSLinkX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Show Page Generated Time:</B><BR>
				Enable or disable display of page generation text at the bottom of the page.</td>
			<td class="post">
				<asp:checkbox id="ShowPageGenerationTime" runat="server"></asp:checkbox></td>
		</tr>		
		<tr>
			<td class="postheader"><B>Show Forum Jump Box:</B><BR>
				Enable or disable display of the Forum Jump Box throughout the forum.</td>
			<td class="post">
				<asp:checkbox id="ShowForumJumpX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Display Points System:</B><BR>
				If checked, points for posting will be displayed for each user.</td>
			<td class="post">
				<asp:checkbox id="DisplayPoints" runat="server"></asp:checkbox></td>
		</tr>		
		<tr>
			<td class="postheader"><B>Remove Nested Quotes:</B><BR>
				Automatically remove nested [quote] tags from replies.</td>
			<td class="post">
				<asp:checkbox id="RemoveNestedQuotesX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Poll Votes Dependant on IP:</B><BR>
			By default, poll voting is tracked via username and client-side cookie. (One vote per username. Cookies are used if guest voting is allowed.)
			If this option is enabled, votes also use IP as a reference providing	the most security against voter fraud.
			</td>
			<td class="post">
				<asp:checkbox id="PollVoteTiedToIPX" runat="server"></asp:checkbox></td>
		</tr>			
		<tr>
			<td class="postheader"><B>Max File Size:</B><BR>
				Maximum size of uploaded files. Leave empty for no limit.</td>
			<td class="post">
				<asp:TextBox id="MaxFileSize" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Smilies Display Grid Size:</B><BR>
				Number of smilies to show by number of rows and columns.</td>
			<td class="post">
				<asp:TextBox id="SmiliesPerRow" runat="server"></asp:TextBox><B>x</B>
				<asp:TextBox id="SmiliesColumns" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Posts Per Page:</B><BR>
				Number of posts to show per page.</td>
			<td class="post">
				<asp:TextBox id="PostsPerPage" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Topics Per Page:</B><BR>
				Number of topics to show per page.</td>
			<td class="post">
				<asp:TextBox id="TopicsPerPage" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Days before posts are locked:</B><BR>
				Number of days until posts are locked and not possible to edit or delete. Set 
				to 0 for no limit.</td>
			<td class="post">
				<asp:textbox id="LockPosts" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Post Flood Delay:</B><BR>
				Number of seconds before another post can be entered. (Does not apply to admins or mods.)</td>
			<td class="post">
				<asp:TextBox id="PostFloodDelay" runat="server"></asp:TextBox></td>
		</tr>		
		<tr>
			<td class="postheader"><B>Date and time format from language file:</B><BR>
				If this is checked, the date and time format will use settings from the 
				language file. Otherwise the browser settings will be used.</td>
			<td class="post">
				<asp:checkbox id="DateFormatFromLanguage" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Create NNTP user names:</B><BR>
				Check to allow users to automatically be created when downloading usenet 
				messages. Only enable this in a test environment, and <EM>NEVER</EM> in a 
				production environment. The main purpose of this option is for performance 
				testing.</td>
			<td class="post">
				<asp:checkbox id="CreateNntpUsers" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
      <td class="header2" align="center" colspan="2">
        Forum Ads</td>
    </tr>
    <tr>
      <td class="postheader">
        <b>2nd post ad:</b><br />
        Place the code that you wish to be displayed in each thread after the 1st post.
        If you do not want an ad to be displayed, don't put anything in the box.
      </td>
      <td class="post">
        <asp:TextBox TextMode="MultiLine" runat="server" ID="AdPost" />
      </td>
    </tr>
    <tr>
      <td class="postheader">
        <b>Show ad from above to signed in users:</b><br />
        If checked, signed in users will see ads.
      </td>
      <td class="post">
        <asp:CheckBox runat="server" ID="ShowAdsToSignedInUsers" />
      </td>
    </tr>
		<tr>
			<td class="header2" align="center" colSpan="2">Editing/Formatting Settings</td>
		</tr>
		<tr>
			<td class="postheader"><B>Forum Editor:</B><BR>
				Select global editor type for your forum. To use the HTML editors (FCK and 
				FreeTextBox) the .bin file must be in the \bin directory and the proper support 
				files must be put in \editors.
			</td>
			<td class="post">
				<asp:dropdownlist id="ForumEditorList" runat="server" DataValueField="Value" DataTextField="Name"></asp:dropdownlist></td>
		</tr>
		<tr>
			<td class="postheader"><B>Accepted HTML Tags:</B><BR>
				Comma seperated list (no spaces) of HTML tags that are allowed in posts using 
				HTML editors.</td>
			<td class="post">
				<asp:TextBox id="AcceptedHTML" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="header2" align="center" colSpan="2">Permissions Settings</td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow User Change Theme:</B><BR>
				Should users be able to choose what theme they want to use?</td>
			<td class="post">
				<asp:checkbox id="AllowUserThemeX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow User Change Language:</B><BR>
				Should users be able to choose what language they want to use?</td>
			<td class="post">
				<asp:checkbox id="AllowUserLanguageX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow Private Messages:</B><BR>
				Allow users to access and send private messages.</td>
			<td class="post">
				<asp:checkbox id="AllowPrivateMessagesX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow Private Message Notifications:</B><BR>
				Allow users email notifications when new private messages arrive.</td>
			<td class="post">
				<asp:checkbox id="AllowPMNotifications" runat="server"></asp:checkbox></td>
		</tr>		
		<tr>
			<td class="postheader"><B>Allow Email Sending:</B><BR>
				Allow users to send emails to each other.</td>
			<td class="post">
				<asp:checkbox id="AllowEmailSendingX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow Signatures:</B><BR>
				Allow users to create signatures.</td>
			<td class="post">
				<asp:checkbox id="AllowSignaturesX" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Disable New Registrations:</B><BR>
				New users won't be able to register.</td>
			<td class="post">
				<asp:checkbox id="DisableRegistrations" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="header2" align="center" colSpan="2">SMTP Server Settings</td>
		</tr>
		<tr>
			<td class="postheader"><B>SMTP Server:</B><BR>
				To be able to send posts you need to enter the name of a valid smtp server.</td>
			<td class="post">
				<asp:TextBox id="ForumSmtpServer" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>SMTP User Name:</B><BR>
				If you need to be authorized to send email.</td>
			<td class="post">
				<asp:TextBox id="ForumSmtpUserName" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader"><B>SMTP Password:</B><BR>
				If you need to be authorized to send email.</td>
			<td class="post">
				<asp:TextBox id="ForumSmtpUserPass" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="header2" align="center" colSpan="2">Avatar Settings</td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow remote avatars:</B><BR>
				Can users use avatars from other websites.</td>
			<td class="post">
				<asp:checkbox id="AvatarRemote" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Allow avatar uploading:</B><BR>
				Can users upload avatars to their profile.</td>
			<td class="post">
				<asp:checkbox id="AvatarUpload" runat="server"></asp:checkbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Avatar Width:</B><BR>
				Maximum width for avatars.</td>
			<td class="post">
				<asp:textbox id="AvatarWidth" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Avatar Height:</B><BR>
				Maximum height for avatars.</td>
			<td class="post">
				<asp:textbox id="AvatarHeight" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader"><B>Avatar Size:</B><BR>
				Maximum size for avatars in bytes.</td>
			<td class="post">
				<asp:textbox id="AvatarSize" runat="server"></asp:textbox></td>
		</tr> <!--tr>
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
			<td class="postfooter" align="center" colSpan="2">
				<asp:Button id="Save" runat="server" Text="Save" onclick="Save_Click"></asp:Button></td>
		</tr>
	</table>
</yaf:adminmenu>
<yaf:SmartScroller id="SmartScroller1" runat="server" />
