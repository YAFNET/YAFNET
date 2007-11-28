<%@ Control Language="c#" CodeFile="hostsettings.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.hostsettings" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../../controls/PMList.ascx" %>

<!-- This style is dealing with tabs rendering issues in IE - should be removed once YAF is fully XHTML 1.0 compliant -->
<style type="text/css">
.ajax__tab_default .ajax__tab_inner {height : 100%;} .ajax__tab_default .ajax__tab_tab {height : 100%;} .ajax__tab_xp .ajax__tab_hover .ajax__tab_tab {height : 100%;} .ajax__tab_xp .ajax__tab_active .ajax__tab_tab {height : 100%;} .ajax__tab_xp .ajax__tab_inner {height : 100%;} .ajax__tab_xp .ajax__tab_tab {height:100%} .ajax__tab_xp .ajax__tab_hover .ajax__tab_inner {height : 100%;} .ajax__tab_xp .ajax__tab_active .ajax__tab_inner {height : 100%;} 
</style>

<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="True" />
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:UpdatePanel runat="server" ID="PMUpdatePanel">
    <ContentTemplate>
        <YAF:AdminMenu runat="server" ID="Adminmenu1">
            <ajaxToolkit:TabContainer runat="server" ID="PMTabs" AutoPostBack="true">
                <ajaxToolkit:TabPanel runat="server" ID="SettingsTab">
                    <ContentTemplate>
                        <table class="content" cellspacing="1" width="100%" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Host Setup</td>
                            </tr>
                            <tr>
                                <td class="postheader" width="50%">
                                    <b>MS SQL Server Version:</b><br>
                                    What version of MS SQL Server is running.</td>
                                <td class="post" width="50%">
                                    <asp:Label ID="SQLVersion" runat="server" CssClass="smallfont"></asp:Label></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Time Zone:</b><br>
                                    The time zone of the web server.</td>
                                <td class="post">
                                    <asp:DropDownList ID="TimeZones" runat="server" DataValueField="Value" DataTextField="Name">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Forum Email:</b><br>
                                    The from address when sending emails to users.</td>
                                <td class="post">
                                    <asp:TextBox ID="ForumEmailEdit" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Require Email Verification:</b><br>
                                    If unchecked users will not need to verify their email address.</td>
                                <td class="post">
                                    <asp:CheckBox ID="EmailVerification" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Disable New Registrations:</b><br>
                                    New users won't be able to register.</td>
                                <td class="post">
                                    <asp:CheckBox ID="DisableRegistrations" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Use File Table:</b><br>
                                    Uploaded files will be saved in the database instead of the file system.</td>
                                <td class="post">
                                    <asp:CheckBox ID="UseFileTableX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Poll Votes Dependant on IP:</b><br>
                                    By default, poll voting is tracked via username and client-side cookie. (One vote
                                    per username. Cookies are used if guest voting is allowed.) If this option is enabled,
                                    votes also use IP as a reference providing the most security against voter fraud.
                                </td>
                                <td class="post">
                                    <asp:CheckBox ID="PollVoteTiedToIPX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Max File Size:</b><br>
                                    Maximum size of uploaded files. Leave empty for no limit.</td>
                                <td class="post">
                                    <asp:TextBox ID="MaxFileSize" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Post editing timeout:</b><br>
                                    Number of seconds while post may be modified without showing that to other users</td>
                                <td class="post">
                                    <asp:TextBox ID="EditTimeOut" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Post Flood Delay:</b><br>
                                    Number of seconds before another post can be entered. (Does not apply to admins
                                    or mods.)</td>
                                <td class="post">
                                    <asp:TextBox ID="PostFloodDelay" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Max Search Results:</b><br>
                                    Maximum number of search results that can be returned. Enter "0" for unlimited (not recommended).</td>
                                <td class="post">
                                    <asp:TextBox ID="ReturnSearchMax" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                   <b>Use SQL Full Text Search:</b><br>
                                   Toggle use of FULLTEXT SQL Server support on searches.</td>
                                <td class="post">
                                    <asp:CheckBox ID="UseFullTextSearch" runat="server"></asp:CheckBox></td>
                            </tr>                              
                            <tr>
                                <td class="postheader">
                                    <b>Date and time format from language file:</b><br>
                                    If this is checked, the date and time format will use settings from the language
                                    file. Otherwise the browser settings will be used.</td>
                                <td class="post">
                                    <asp:CheckBox ID="DateFormatFromLanguage" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Create NNTP user names:</b><br>
                                    Check to allow users to automatically be created when downloading usenet messages.
                                    Only enable this in a test environment, and <em>NEVER</em> in a production environment.
                                    The main purpose of this option is for performance testing.</td>
                                <td class="post">
                                    <asp:CheckBox ID="CreateNntpUsers" runat="server"></asp:CheckBox></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="FeaturesTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Features</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show RSS Links:</b><br>
                                    Enable or disable display of RSS links throughout the forum.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowRSSLinkX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Forum Jump Box:</b><br>
                                    Enable or disable display of the Forum Jump Box throughout the forum.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowForumJumpX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Display Points System:</b><br>
                                    If checked, points for posting will be displayed for each user.</td>
                                <td class="post">
                                    <asp:CheckBox ID="DisplayPoints" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Days before posts are locked:</b><br>
                                    Number of days until posts are locked and not possible to edit or delete. Set to
                                    0 for no limit.</td>
                                <td class="post">
                                    <asp:TextBox ID="LockPosts" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Post to Blog:</b><br>
                                    If checked, post to blog feature is enabled.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowPostToBlog" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow "Report Abuse" post:</b><br>
                                    If checked, report feature is enabled.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowReportAbuse" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow "Report Spam" post:</b><br>
                                    If checked, report feature is enabled.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowReportSpam" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Email Topic:</b><br>
                                    If checked, users will be allowed to email topics.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowEmailTopic" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Quick Answer:</b><br>
                                    Enable or disable display of the Quick Reply Box at the bottom of the Posts page</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowQuickAnswerX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="header1" colspan="2">
                                    CAPTCHA Settings</td>
                            </tr>                            
                            <tr>
                                <td class="postheader">
                                    <b>CAPTCHA Size:</b><br>
                                    Size (length) of the CAPTCHA random alphanumeric string</td>
                                <td class="post">
                                    <asp:TextBox ID="CaptchaSize" runat="server"></asp:TextBox></td>
                            </tr>     
                            <tr>
                                <td class="postheader">
                                    <b>Enable CAPTCHA for Post a Message:</b><br/>
                                    Require users enter the CAPTCHA when they post or reply to a forum message (including Quick Reply).</td>
                                <td class="post">
                                    <asp:CheckBox ID="EnableCaptchaForPost" runat="server"></asp:CheckBox></td>
                            </tr>     
                            <tr>
                                <td class="postheader">
                                    <b>Enable CAPTCHA for Register:</b><br/>
                                    Require users enter the CAPTCHA when they register for the forum.</td>
                                <td class="post">
                                    <asp:CheckBox ID="EnableCaptchaForRegister" runat="server"></asp:CheckBox></td>
                            </tr>                                                                              
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="DisplayTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Display Settings</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Moved Topics:</b><br>
                                    If this is checked, topics that are moved will leave behind a pointer to the new
                                    topic.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowMoved" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Deleted Messages:</b><br>
                                    If this is checked, messsages that are deleted will leave with some notes</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowDeletedMessages" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Links in New Window:</b><br>
                                    If this is checked, links in messages will open in a new window.</td>
                                <td class="post">
                                    <asp:CheckBox ID="BlankLinks" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Groups:</b><br>
                                    Should the groups a user is part of be visible on the posts page.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowGroupsX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Groups in profile:</b><br>
                                    Should the groups a user is part of be visible on the users profile page.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowGroupsProfile" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Badges:</b><br>
                                    Should the badges of a user be visible on the posts page.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowBadges" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Users Browsing:</b><br>
                                    Should users currently browsing forums/topics be displayed at the bottom.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowBrowsingUsers" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show Page Generated Time:</b><br>
                                    Enable or disable display of page generation text at the bottom of the page.</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowPageGenerationTime" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Show YetAnotherForum Version:</b><br>
                                    Enable or disable display of the version/date information the bottom of the page (disable if your concerned about security).</td>
                                <td class="post">
                                    <asp:CheckBox ID="ShowYAFVersion" runat="server"></asp:CheckBox></td>
                            </tr>                            
                            <tr>
                                <td class="postheader">
                                    <b>Show Join Date:</b><br>
                                    If checked, join date will be displayed for each user.</td>
                                <td class="post">
                                    <asp:CheckBox ID="DisplayJoinDate" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Remove Nested Quotes:</b><br>
                                    Automatically remove nested [quote] tags from replies.</td>
                                <td class="post">
                                    <asp:CheckBox ID="RemoveNestedQuotesX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Smilies Display Grid Size:</b><br>
                                    Number of smilies to show by number of rows and columns.</td>
                                <td class="post">
                                    <asp:TextBox ID="SmiliesPerRow" runat="server"></asp:TextBox><b>x</b>
                                    <asp:TextBox ID="SmiliesColumns" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Posts Per Page:</b><br>
                                    Number of posts to show per page.</td>
                                <td class="post">
                                    <asp:TextBox ID="PostsPerPage" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Topics Per Page:</b><br>
                                    Number of topics to show per page.</td>
                                <td class="post">
                                    <asp:TextBox ID="TopicsPerPage" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="AdvertsTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Advert Settings</td>
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
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="EditorTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Editting/Formatting Settings</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Forum Editor:</b><br>
                                    Select global editor type for your forum. To use the HTML editors (FCK and FreeTextBox)
                                    the .bin file must be in the \bin directory and the proper support files must be
                                    put in \editors.
                                </td>
                                <td class="post">
                                    <asp:DropDownList ID="ForumEditorList" runat="server" DataValueField="Value" DataTextField="Name">
                                    </asp:DropDownList></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Accepted HTML Tags:</b><br>
                                    Comma seperated list (no spaces) of HTML tags that are allowed in posts using HTML
                                    editors.</td>
                                <td class="post">
                                    <asp:TextBox ID="AcceptedHTML" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="PermissionTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Permission</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow User Change Theme:</b><br>
                                    Should users be able to choose what theme they want to use?</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowUserThemeX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow User Change Language:</b><br>
                                    Should users be able to choose what language they want to use?</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowUserLanguageX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Private Messages:</b><br>
                                    Allow users to access and send private messages.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowPrivateMessagesX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Private Message Notifications:</b><br>
                                    Allow users email notifications when new private messages arrive.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowPMNotifications" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Email Sending:</b><br>
                                    Allow users to send emails to each other.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowEmailSendingX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow Signatures:</b><br>
                                    Allow users to create signatures.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AllowSignaturesX" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Profile Viewing Permissions:</b><br>
                                    Allow viewing of other users' profiles to:</td>
                                <td class="post">
																		<asp:DropDownList ID="ProfileViewPermissions" runat="server">
																			<asp:ListItem Value="0" Text="Forbidden" />
																			<asp:ListItem Value="1" Text="Registered Users" />
																			<asp:ListItem Value="2" Text="All Users" />
																		</asp:DropDownList>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="SMPTTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    SMTP Settings</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>SMTP Server:</b><br>
                                    To be able to send posts you need to enter the name of a valid smtp server.</td>
                                <td class="post">
                                    <asp:TextBox ID="ForumSmtpServer" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>SMTP Server Port:</b><br>
                                    Leave blank to use default SMTP port.</td>
                                <td class="post">
                                    <asp:TextBox ID="ForumSmtpServerPort" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Use SSL:</b><br>
                                    Determines whether conntection with SMTP server is done over SSL encrypted connection.</td>
                                <td class="post">
																		<asp:CheckBox ID="ForumSmtpServerSsl" runat="server" />
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>SMTP User Name:</b><br>
                                    If you need to be authorized to send email.</td>
                                <td class="post">
                                    <asp:TextBox ID="ForumSmtpUserName" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>SMTP Password:</b><br>
                                    If you need to be authorized to send email.</td>
                                <td class="post">
                                    <asp:TextBox ID="ForumSmtpUserPass" runat="server" TextMode="Password"></asp:TextBox></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="TemplatesTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Template Settings</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>User box template:</b><br>
                                    Template for rendering user box by user's posts.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBox" TextMode="MultiLine" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Avatar template:</b><br>
                                    Template for rendering avatar.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxAvatar" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Badges template:</b><br>
                                    Template for rendering user's badges.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxBadges" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Rank image template:</b><br>
                                    Template for rendering rank image.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxRankImage" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Rank template:</b><br>
                                    Template for rendering user's rank.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxRank" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Groups template:</b><br>
                                    Template for rendering user's groups.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxGroups" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Join date template:</b><br>
                                    Template for rendering user's joine date.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxJoinDate" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Posts template:</b><br>
                                    Template for rendering user's posts.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxPosts" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Points template:</b><br>
                                    Template for rendering user's points.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxPoints" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Location template:</b><br>
                                    Template for rendering user's location.</td>
                                <td class="post">
                                    <asp:TextBox ID="UserBoxLocation" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <ajaxToolkit:TabPanel runat="server" ID="AvatarsTab">
                    <ContentTemplate>
                        <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                            <tr>
                                <td class="header1" colspan="2">
                                    Avatar Settings</td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow remote avatars:</b><br>
                                    Can users use avatars from other websites.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AvatarRemote" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Allow avatar uploading:</b><br>
                                    Can users upload avatars to their profile.</td>
                                <td class="post">
                                    <asp:CheckBox ID="AvatarUpload" runat="server"></asp:CheckBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Avatar Width:</b><br>
                                    Maximum width for avatars.</td>
                                <td class="post">
                                    <asp:TextBox ID="AvatarWidth" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Avatar Height:</b><br>
                                    Maximum height for avatars.</td>
                                <td class="post">
                                    <asp:TextBox ID="AvatarHeight" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="postheader">
                                    <b>Avatar Size:</b><br>
                                    Maximum size for avatars in bytes.</td>
                                <td class="post">
                                    <asp:TextBox ID="AvatarSize" runat="server"></asp:TextBox></td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
            <table class="content" cellspacing="1" cellpadding="0" width="100%">
                <tr>
                    <td class="postfooter" align="center">
                        <asp:Button ID="Save" runat="server" Text="Save Settings" OnClick="Save_Click"></asp:Button></td>
                </tr>
            </table>
        </YAF:AdminMenu>
    </ContentTemplate>
</asp:UpdatePanel>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
