<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.hostsettings"
    CodeBehind="hostsettings.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../../controls/PMList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
    <DotNetAge:Tabs ID="HostSettingsTabs" runat="server" ActiveTabEvent="Click" AsyncLoad="false"
        AutoPostBack="false" Collapsible="false" ContentCssClass="" ContentStyle="" Deselectable="false"
        EnabledContentCache="false" HeaderCssClass="" HeaderStyle="" OnClientTabAdd=""
        OnClientTabDisabled="" OnClientTabEnabled="" OnClientTabLoad="" OnClientTabRemove=""
        OnClientTabSelected="" OnClientTabShow="" SelectedIndex="0" Sortable="false"
        Spinner="">
        <Animations>
        </Animations>
        <Views>
            <DotNetAge:View runat="server" ID="View1" Text="Host Settings" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" cellspacing="1" width="100%" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Host Setup
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" width="50%">
                            <b>SQL Server Version:</b><br />
                            What version of SQL Server is running.
                        </td>
                        <td class="post" width="50%">
                            <asp:Label ID="SQLVersion" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Server Time Zone Correction:</b><br />
                            Enter a positive or a negative value in minutes between -720 and 720, if the server
                            UTC time value is incorrect: <b>
                                <%# DateTime.UtcNow %></b>.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ServerTimeCorrection" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Forum Email:</b><br />
                            The from address when sending emails to users.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ForumEmail" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Require Email Verification:</b><br />
                            If unchecked users will not need to verify their email address.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EmailVerification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Use File Table:</b><br />
                            Uploaded files will be saved in the database instead of the file system.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFileTable" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Poll Votes Dependant on IP:</b><br />
                            By default, poll voting is tracked via username and client-side cookie. (One vote
                            per username. Cookies are used if guest voting is allowed.) If this option is enabled,
                            votes also use IP as a reference providing the most security against voter fraud.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="PollVoteTiedToIP" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Maximum Number of Attachments per Post:</b><br />
                            Maximum Number of uploaded files per Post. Set to 0 for unlimited.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxNumberOfAttachments" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Max File Size:</b><br />
                            Maximum size of uploaded files. Leave empty for no limit.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxFileSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Post editing timeout:</b><br />
                            Number of seconds while post may be modified without showing that to other users
                        </td>
                        <td class="post">
                            <asp:TextBox ID="EditTimeOut" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Web Service Token:</b><br />
                            Token used to make secure web service calls. Constantly changing until you save
                            your host settings.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="WebServiceToken" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>User Name Max Length:</b><br />
                            Max Allowed User Name or User Display Name Max Length.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserNameMaxLength" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Max Report Post Chars:</b><br />
                            Max Allowed Report Post length.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxReportPostChars" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Max Post Size:</b><br />
                            Maximum size of a post in bytes. Set to 0 for unlimited (not recommended).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxPostSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Post Flood Delay:</b><br />
                            Number of seconds before another post can be entered. (Does not apply to admins
                            or mods.)
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PostFloodDelay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Url Referrer Security Check:</b><br />
                            Validates all POSTs are from the same domain as the referring domain. (No cross
                            domain POSTs.)
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DoUrlReferrerSecurityCheck" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Date and time format from language file:</b><br />
                            If this is checked, the date and time format will use settings from the language
                            file. Otherwise the browser settings will be used.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DateFormatFromLanguage" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Create NNTP user names:</b><br />
                            Check to allow users to automatically be created when downloading usenet messages.
                            Only enable this in a test environment, and <em>NEVER</em> in a production environment.
                            The main purpose of this option is for performance testing.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="CreateNntpUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Login/Registration Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Disable New Registrations:</b><br />
                            New users won't be able to register.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisableRegistrations" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Custom Login Redirect Url:</b><br />
                            If login is disabled, this is the URL users will be redirected to when they need
                            to access the forum. Optionally add "{0}" to the URL to pass the return URL to the
                            custom Url. E.g. "http://mydomain.com/login.aspx?PreviousUrl={0}"
                        </td>
                        <td class="post">
                            <asp:TextBox ID="CustomLoginRedirectUrl" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Require User Login:</b><br />
                            If checked, users will be required to log in before they can see any content. They'll
                            be redirected straight to login page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RequireLogin" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Image Attachment Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Image Attachment Display Treshold:</b><br />
                            Maximum size of picture attachment to display as picture. Pictures over this size
                            will be displayed as links.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PictureAttachmentDisplayTreshold" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Image Attachment Resize:</b><br />
                            Attached images will be resized to thumbnails if they are too large.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableImageAttachmentResize" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Image Attachment Resize Max Width:</b><br />
                            Maximum Width of the resized attachment images.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ImageAttachmentResizeWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Image Attachment Resize Max Height:</b><br />
                            Maximum Height of the resized attachment images.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ImageAttachmentResizeHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View2" Text="Features" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Features
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Active Users Time:</b><br />
                            Number of minutes to display users in Active Users list.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ActiveListTime" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Guests In Detailed Active List:</b><br />
                            If checked, Guests will be displayed In Detailed Active List.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGuestsInDetailedActiveList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Add Dynamic Page Meta Tags:</b><br />
                            If checked, description and keywords meta tags will be created dynamically on the
                            post pages.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AddDynamicPageMetaTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Display Name:</b><br />
                            If checked, YAF uses an alternative "Display Name" instead of the UserName.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableDisplayName" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Display Gender:</b><br />
                            If checked, the user gender is displayed in the user data in messages list.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowGenderInUserBox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow User To Hide Himself:</b><br />
                            If checked, the user who checked it will not be visible in the active users list.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserHideHimself" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Display Name Modification:</b><br />
                            If checked, and "Enable Display Name" checked, allow modification of Display Name
                            in Edit Profile.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowDisplayNameModification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show User Online/Offline Status:</b><br />
                            If checked, current user status is displayed in the forum. Hidden users are always
                            displayed as offline.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowUserOnlineStatus" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Users to Thank Posts:</b><br />
                            If checked users can thank posts they consider useful.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableThanksMod" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Buddy List:</b><br />
                            If checked users can add each other as buddies.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableBuddyList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show The Date on Which Users Have Thanked Posts:</b><br />
                            If checked users can see on which date posts have been thanked. (Thanks Mod must
                            be enabled first.)
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowThanksDate" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Remove Nested Quotes:</b><br />
                            Automatically remove nested [quote] tags from replies.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RemoveNestedQuotes" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Disable "NoFollow" Tag on Links on Posts Older Than:</b><br />
                            If "NoFollow" is enabled above, this is disable no follow for links on messages
                            older then X days old (which takes into consideration last edited).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="DisableNoFollowLinksAfterDay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Smilies Display Grid Size:</b><br />
                            Number of smilies to show by number of rows and columns.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SmiliesPerRow" runat="server"></asp:TextBox><b>x</b>
                            <asp:TextBox ID="SmiliesColumns" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Display Points System:</b><br />
                            If checked, points for posting will be displayed for each user.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisplayPoints" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Days before posts are locked:</b><br />
                            Number of days until posts are locked and not possible to edit or delete. Set to
                            0 for no limit.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="LockPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>IP Info Page URL:</b><br />
                            Set it to get details about IPs whereabouts.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="IPInfoPageURL" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Post to Blog:</b><br />
                            If checked, post to blog feature is enabled.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPostToBlog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Email Topic:</b><br />
                            If checked, users will be allowed to email topics.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailTopic" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Quick Answer:</b><br />
                            Enable or disable display of the Quick Reply Box at the bottom of the Posts page
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowQuickAnswer" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable calendar:</b><br />
                            Enables/disables calendar in profile, if it causes troubles on servers/for users
                            with different cultures.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableDNACalendar" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            CAPTCHA Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>CAPTCHA Size:</b><br />
                            Size (length) of the CAPTCHA random alphanumeric string
                        </td>
                        <td class="post">
                            <asp:TextBox ID="CaptchaSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>reCAPTCHA Public Key:</b><br />
                            Enter a reCAPTCHA Public Key
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RecaptchaPublicKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>reCAPTCHA Private Key:</b><br />
                            Enter a reCAPTCHA Private Key
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RecaptchaPrivateKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable reCAPTCHA Multiple Instances:</b><br />
                            Enable reCAPTCHA Recapture Multiple Instances(shared keys).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RecaptureMultipleInstances" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable CAPTCHA for Guest Posting:</b><br />
                            Require guest users to enter the CAPTCHA when they post or reply to a forum message
                            (including Quick Reply).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableCaptchaForGuests" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable CAPTCHA for Post a Message:</b><br />
                            Require users to enter the CAPTCHA when they post or reply to a forum message (including
                            Quick Reply).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableCaptchaForPost" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable CAPTCHA/reCAPTCHA for Register:</b><br />
                            Require users to enter the CAPTCHA when they register for the forum.
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="CaptchaTypeRegister" runat="server">
                                <asp:ListItem Value="0" Text="Disabled" />
                                <asp:ListItem Value="1" Text="YafCaptcha" />
                                <asp:ListItem Value="2" Text="ReCaptcha" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Private Messages
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Max no. of PM Recipients:</b><br />
                            Maximum allowed recipients per on PM sent (0 = unlimited)
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PrivateMessageMaxRecipients" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Album Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Album Feature:</b><br />
                            If checked, album feature is enabled.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableAlbum" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Maximum Image Size:</b><br />
                            Maximum size of image in bytes a user can upload.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AlbumImagesSizeMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Albums Per Page:</b><br />
                            Number of albums to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AlbumsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Images Per Page:</b><br />
                            Number of images to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AlbumImagesPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Log settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Message history archieve time</b><br />
                            Number of days to keep message change history.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MessageHistoryDaysToLog" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Active Location Error Log:</b><br />
                            If checked, all active location path errors are logged.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableActiveLocationErrorsLog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Enable Unhandled UserAgent Log:</b><br />
                            If checked, all unhandled UserAgent strings are logged.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UserAgentBadLog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View3" Text="Display" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Display Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Avatars in Topic Listing:</b><br />
                            If this is checked, the topic pages will show avatar graphics.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowAvatarsInTopic" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Moved Topics:</b><br />
                            If this is checked, topics that are moved will leave behind a pointer to the new
                            topic.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowMoved" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Moderator List:</b><br />
                            If this is checked, the moderator list column is displayed in the forum list.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowModeratorList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Deleted Messages:</b><br />
                            If this is checked, messsages that are deleted will leave with some notes
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowDeletedMessages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Deleted Messages to All:</b><br />
                            If Show Deleted Messages is checked above, checking this will force showing the
                            delete message stub to all users.<br />
                            If it remains unchecked, the deleted message stub will only show to administrators,
                            moderators, and the owner of the deleted message.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowDeletedMessagesToAll" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Links in New Window:</b><br />
                            If this is checked, links in messages will open in a new window.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="BlankLinks" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show 'no-count' Forum Posts in Active Discussions :</b><br />
                            If this is checked, posts from no-count forums will be displayed in Active Topics.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="NoCountForumsInActiveDiscussions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Forum Statistics:</b><br />
                            Enable or disable display of forum statistics on board index page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowForumStatistics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Active Discussions:</b><br />
                            Enable or disable display of active discussions list on board index page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowActiveDiscussions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show RSS Links:</b><br />
                            Enable or disable display of RSS links throughout the forum.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRSSLink" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Forum Jump Box:</b><br />
                            Enable or disable display of the Forum Jump Box throughout the forum.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowForumJump" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Shoutbox:</b><br />
                            Enable or disable display of the Shoutbox (Chat Module) in the forum page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowShoutbox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Shoutbox Smiles:</b><br />
                            Enable or disable display of the Shoutbox (Chat Module) smiles.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowShoutboxSmiles" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Groups:</b><br />
                            Should the groups a user is part of be visible on the posts page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGroups" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Groups in profile:</b><br />
                            Should the groups a user is part of be visible on the users profile page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGroupsProfile" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Medals:</b><br />
                            Should medals of a user be visible on the posts page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowMedals" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Users Browsing:</b><br />
                            Should users currently browsing forums/topics be displayed at the bottom.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowBrowsingUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Page Generated Time:</b><br />
                            Enable or disable display of page generation text at the bottom of the page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowPageGenerationTime" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show YetAnotherForum Version:</b><br />
                            Enable or disable display of the version/date information the bottom of the page
                            (disable if your concerned about security).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowYAFVersion" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show Join Date:</b><br />
                            If checked, join date will be displayed for each user.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisplayJoinDate" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Show "Rules" Before Registration:</b><br />
                            Require that "rules" are shown and accepted before a new user can register.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRulesForRegistration" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Active Discussions Count:</b><br />
                            Number of records to display in Active Discussions list on forum index.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ActiveDiscussionsCount" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Use "NoFollow" Tag in Links:</b><br />
                            If this is checked, all links will have the nofollow tag.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseNoFollowLinks" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Posts Per Page:</b><br />
                            Number of posts to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PostsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Topics Per Page:</b><br />
                            Number of topics to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="TopicsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View4" Text="Adverts" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Advert Settings
                        </td>
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
                            <b>Show Ad to "Signed In" Users:</b><br />
                            If checked, signed in users will see ads.
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="ShowAdsToSignedInUsers" />
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View5" Text="Editors" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Editting/Formatting Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Forum Editor:</b><br />
                            Select global editor type for your forum. To use the HTML editors (FCK and FreeTextBox)
                            the .bin file must be in the \bin directory and the proper support files must be
                            put in \editors.
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="ForumEditor" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Accepted HTML Tags:</b><br />
                            Comma seperated list (no spaces) of HTML tags that are allowed in posts using HTML
                            editors.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AcceptedHTML" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Use styled nicks:</b><br />
                            If checked, you can use colors, font size change etc. for active users nicks.
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="UseStyledNicks" />
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View6" Text="Permission" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Permission
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow User Change Theme:</b><br />
                            Should users be able to choose what theme they want to use?
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserTheme" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow User Change Language:</b><br />
                            Should users be able to choose what language they want to use?
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserLanguage" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Signatures:</b><br />
                            Allow users to create signatures.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowSignatures" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Private Messages:</b><br />
                            Allow users to access and send private messages.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPrivateMessages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Private Message Notifications:</b><br />
                            Allow users email notifications when new private messages arrive.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPMEmailNotification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Email Sending:</b><br />
                            Allow users to send emails to each other.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailSending" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Email Change:</b><br />
                            Allow users to change their email address.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Password Change:</b><br />
                            Allow users to change their passwords.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPasswordChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Moderators View IPs:</b><br />
                            Allow to view IPs to moderators.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowModeratorsViewIPs" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Notification of All Posts on All topics:</b><br />
                            Allow users to get individual email notifications on all emails -- tons of email
                            traffic.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowNotificationAllPostsAllTopics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Report Post Permissions:</b><br />
                            Allow reporting posts to:
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="ReportPostPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Profile Viewing Permissions:</b><br />
                            Allow viewing of other users' profiles to:
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="ProfileViewPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Members List Viewing Permissions:</b><br />
                            Allow viewing of members list to:
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="MembersListViewPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Active Users Viewing Permissions:</b><br />
                            Allow viewing of active users list to:
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="ActiveUsersViewPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Max Word Length:</b><br />
                            Use it to limit number of a word characters in topic names and some other places.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxWordLength" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allowed Poll Choice Number:</b><br />
                            Number of a question choices, max value no more then 99.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AllowedPollChoiceNumber" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Use SSL while logging in:</b><br />
                            Enforce a secure connection for users to log in.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToLogIn" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Use SSL while registering:</b><br />
                            Enforce a secure connection for users to register.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToRegister" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View7" Text="Templates" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Template Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>User box template:</b><br />
                            Template for rendering user box by user's posts.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Avatar template:</b><br />
                            Template for rendering avatar.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxAvatar" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Medals template:</b><br />
                            Template for rendering user's medals.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxMedals" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Rank image template:</b><br />
                            Template for rendering rank image.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxRankImage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Rank template:</b><br />
                            Template for rendering user's rank.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxRank" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Groups template:</b><br />
                            Template for rendering user's groups.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxGroups" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Join date template:</b><br />
                            Template for rendering user's joine date.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxJoinDate" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Posts template:</b><br />
                            Template for rendering user's posts.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Points template:</b><br />
                            Template for rendering user's points.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxPoints" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Location template:</b><br />
                            Template for rendering user's location.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxLocation" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Gender:</b><br />
                            Template for rendering user's gender.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxGender" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Thanks From template:</b><br>
                            Template for rendering user's thanks from.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxThanksFrom" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Thanks To template:</b><br>
                            Template for rendering user's thanks to. aLLOW
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxThanksTo" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View8" Text="Avatars" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Avatar Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow remote avatars:</b><br />
                            Can users use avatars from other websites.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarRemote" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow avatar uploading:</b><br />
                            Can users upload avatars to their profile.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarUpload" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Allow Gravatars:</b><br />
                            Automatically use users Gavatars if they exist (note: may require additional processing).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarGravatar" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Gravatar Rating:</b><br />
                            Max rating of Gravatar if allowed.
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="GravatarRating" runat="server">
                                <asp:ListItem Value="G"></asp:ListItem>
                                <asp:ListItem Value="PG"></asp:ListItem>
                                <asp:ListItem Value="R"></asp:ListItem>
                                <asp:ListItem Value="X"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Avatar Width:</b><br />
                            Maximum width for avatars.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AvatarWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Avatar Height:</b><br />
                            Maximum height for avatars.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AvatarHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Avatar Size:</b><br />
                            Maximum size for avatars in bytes.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AvatarSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View9" Text="Cache" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Cache Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Forum Statistics Cache Timeout:</b><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ForumStatisticsCacheTimeout" />
                            <asp:Button ID="ForumStatisticsCacheReset" Text="Clear" runat="server" OnClick="ForumStatisticsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Board User Statistics Cache Timeout:</b><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="BoardUserStatsCacheTimeout" />
                            <asp:Button ID="BoardUserStatsCacheReset" Text="Clear" runat="server" OnClick="BoardUserStatsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Active Discussions Cache Timeout:</b><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ActiveDiscussionsCacheTimeout" />
                            <asp:Button ID="ActiveDiscussionsCacheReset" Text="Clear" runat="server" OnClick="ActiveDiscussionsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Board Categories Cache Timeout:</b><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="BoardCategoriesCacheTimeout" />
                            <asp:Button ID="BoardCategoriesCacheReset" Text="Clear" runat="server" OnClick="BoardCategoriesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Board Moderators Cache Timeout:</b><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="BoardModeratorsCacheTimeout" />
                            <asp:Button ID="BoardModeratorsCacheReset" Text="Clear" runat="server" OnClick="BoardModeratorsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Replace Rules Cache Timeout:</b><br />
                            Smilies, BB code, bad wordsm, etc. (in minutes)
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ReplaceRulesCacheTimeout" />
                            <asp:Button ID="ReplaceRulesCacheReset" Text="Clear" runat="server" OnClick="ReplaceRulesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>First Post "Title" Cache Timeout:</b><br />
                            First Post "Title" for SEO Cache Timeout (in minutes)
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="FirstPostCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Online User Status Cache Timeout:</b><br />
                            You can fine-tune it depending on your site activity (in milliseconds)
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="OnlineStatusCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>User Lazy Data Cache Timeout:</b><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ActiveUserLazyDataCacheTimeout" />
                            <asp:Button ID="ActiveUserLazyDataCacheReset" Text="Clear" runat="server" OnClick="UserLazyDataCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="footer1" colspan="2">
                            <asp:Button runat="server" ID="ResetCacheAll" Text="Clear Cache" OnClick="ResetCacheAll_Click" />
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
            <DotNetAge:View runat="server" ID="View10" Text="Search" NavigateUrl="" HeaderCssClass=""
                HeaderStyle="" Target="_blank">
                <table class="content" width="100%" cellspacing="1" cellpadding="0" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            Search Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Max Search Results:</b><br />
                            Maximum number of search results that can be returned. Enter "0" for unlimited (not
                            recommended).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ReturnSearchMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Use SQL Full Text Search:</b><br />
                            Toggle use of FULLTEXT SQL Server support on searches.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFullTextSearch" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Search Text Minimal Length:</b><br />
                            Minimal length of the search string allowed.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchStringMinLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Search Text Maximum Length:</b><br />
                            Maximum length of the search string allowed.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchStringMaxLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Search Text Pattern:</b><br />
                            Allowed search text (Regular Expression) pattern.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchStringPattern" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Search Permissions:</b><br />
                            Allow search to:
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="SearchPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Search Engine 1:</b><br />
                            Enter here a search engine pattern.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine1" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Parameters For Search Engine 1:</b><br />
                            Enter here a search engine parameters.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine1Parameters" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Search Engine 2:</b><br />
                            Enter here a search engine pattern.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>Parameters For Search Engine 2:</b><br />
                            Enter here a search engine parameters.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine2Parameters" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>External Search Permissions:</b><br />
                            Allow external search to:
                        </td>
                        <td class="post">
                            <asp:DropDownList ID="ExternalSearchPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <b>External Search Results In A New Window:</b><br />
                            Show External Search Results In A New Window.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ExternalSearchInNewWindow" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
            </DotNetAge:View>
        </Views>
    </DotNetAge:Tabs>
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="postfooter" align="center">
                <asp:Button ID="Save" runat="server" Text="Save Settings" CssClass="pbutton" OnClick="Save_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
