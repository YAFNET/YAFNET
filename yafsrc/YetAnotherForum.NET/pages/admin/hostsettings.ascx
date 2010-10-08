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
                            <strong>SQL Server Version:</strong><br />
                            What version of SQL Server is running.
                        </td>
                        <td class="post" width="50%">
                            <asp:Label ID="SQLVersion" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Server Time Zone Correction:</strong><br />
                            Enter a positive or a negative value in minutes between -720 and 720, if the server
                            UTC time value is incorrect: <strong>
                                <%# DateTime.UtcNow %></strong>.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ServerTimeCorrection" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Forum Email:</strong><br />
                            The from address when sending emails to users.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ForumEmail" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Require Email Verification:</strong><br />
                            If unchecked users will not need to verify their email address.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EmailVerification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Use File Table:</strong><br />
                            Uploaded files will be saved in the database instead of the file system.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFileTable" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Abandon Sessions for "Don't Track" Users:</strong><br />
                            Automatically abandon sessions for users who are marked as "Don't Track" such as Search Engines and Bots.
                            Enable if you're having any session issues.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AbandonSessionsForDontTrack" runat="server"></asp:CheckBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="postheader">
                            <strong>Maximum Number of Attachments per Post:</strong><br />
                            Maximum Number of uploaded files per Post. Set to 0 for unlimited.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxNumberOfAttachments" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Max File Size:</strong><br />
                            Maximum size of uploaded files. Leave empty for no limit.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxFileSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Post editing timeout:</strong><br />
                            Number of seconds while post may be modified without showing that to other users
                        </td>
                        <td class="post">
                            <asp:TextBox ID="EditTimeOut" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Web Service Token:</strong><br />
                            Token used to make secure web service calls. Constantly changing until you save
                            your host settings.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="WebServiceToken" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>User Name Max Length:</strong><br />
                            Max Allowed User Name or User Display Name Max Length.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserNameMaxLength" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Max Report Post Chars:</strong><br />
                            Max Allowed Report Post length.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxReportPostChars" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Max Post Size:</strong><br />
                            Maximum size of a post in bytes. Set to 0 for unlimited (not recommended).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxPostSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Post Flood Delay:</strong><br />
                            Number of seconds before another post can be entered. (Does not apply to admins
                            or mods.)
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PostFloodDelay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable Url Referrer Security Check:</strong><br />
                            Validates all POSTs are from the same domain as the referring domain. (No cross
                            domain POSTs.)
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DoUrlReferrerSecurityCheck" runat="server"></asp:CheckBox>
                        </td>
                    </tr>      
                    <tr>
                        <td class="postheader">
                            <strong>Create NNTP user names:</strong><br />
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
                            <strong>Disable New Registrations:</strong><br />
                            New users won't be able to register.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisableRegistrations" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Custom Login Redirect Url:</strong><br />
                            If login is disabled in the AppSettings (AllowLoginAndLogoff needs to be set to "false"),
                            this is the URL users will be redirected to when they need
                            to access the forum. Optionally add "{0}" to the URL to pass the return URL to the
                            custom Url. E.g. "http://mydomain.com/login.aspx?PreviousUrl={0}"
                        </td>
                        <td class="post">
                            <asp:TextBox ID="CustomLoginRedirectUrl" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Require User Login:</strong><br />
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
                            <strong>Image Attachment Display Treshold:</strong><br />
                            Maximum size of picture attachment to display as picture. Pictures over this size
                            will be displayed as links.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PictureAttachmentDisplayTreshold" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable Image Attachment Resize:</strong><br />
                            Attached images will be resized to thumbnails if they are too large.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableImageAttachmentResize" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Image Attachment Resize Max Width:</strong><br />
                            Maximum Width of the resized attachment images.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ImageAttachmentResizeWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Image Attachment Resize Max Height:</strong><br />
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
                            <strong>Add Dynamic Page Meta Tags:</strong><br />
                            If checked, description and keywords meta tags will be created dynamically on the
                            post pages.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AddDynamicPageMetaTags" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="postheader">
                            <strong>Allow Display Gender:</strong><br />
                            If checked, the user gender is displayed in the user data in messages list.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowGenderInUserBox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow User To Hide Himself:</strong><br />
                            If checked, the user who checked it will not be visible in the active users list.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserHideHimself" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <strong>Enable Display Name:</strong><br />
                            If checked, YAF uses an alternative "Display Name" instead of the UserName.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableDisplayName" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Display Name Modification:</strong><br />
                            If checked, and "Enable Display Name" checked, allow modification of Display Name
                            in Edit Profile.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowDisplayNameModification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show User Online/Offline Status:</strong><br />
                            If checked, current user status is displayed in the forum. Hidden users are always
                            displayed as offline.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowUserOnlineStatus" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Users to Thank Posts:</strong><br />
                            If checked users can thank posts they consider useful.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableThanksMod" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable Buddy List:</strong><br />
                            If checked users can add each other as buddies.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableBuddyList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show The Date on Which Users Have Thanked Posts:</strong><br />
                            If checked users can see on which date posts have been thanked. (Thanks Mod must
                            be enabled first.)
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowThanksDate" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Remove Nested Quotes:</strong><br />
                            Automatically remove nested [quote] tags from replies.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RemoveNestedQuotes" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Disable "NoFollow" Tag on Links on Posts Older Than:</strong><br />
                            If "NoFollow" is enabled above, this is disable no follow for links on messages
                            older then X days old (which takes into consideration last edited).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="DisableNoFollowLinksAfterDay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Smilies Display Grid Size:</strong><br />
                            Number of smilies to show by number of rows and columns.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SmiliesPerRow" runat="server"></asp:TextBox><strong>x</strong>
                            <asp:TextBox ID="SmiliesColumns" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Display Points System:</strong><br />
                            If checked, points for posting will be displayed for each user.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisplayPoints" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Days before posts are locked:</strong><br />
                            Number of days until posts are locked and not possible to edit or delete. Set to
                            0 for no limit.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="LockPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <strong>Enable IP Info Service:</strong><br />
                            If checked, we will get info about a registering user from a web service.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableIPInfoService" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>IP Info XML Web Service URL:</strong><br />
                            Set it to get details about user IPs as XML data.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="IPLocatorPath" runat="server"></asp:TextBox>
                        </td>
                    </tr>           
                    <tr>
                        <td class="postheader">
                            <strong>IP Info Page URL:</strong><br />
                            Set it to get details about IPs whereabouts as web page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="IPInfoPageURL" runat="server"></asp:TextBox>
                        </td>
                    </tr>
               
                    <tr>
                        <td class="postheader">
                            <strong>Allow Post to Blog:</strong><br />
                            If checked, post to blog feature is enabled.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPostToBlog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Email Topic:</strong><br />
                            If checked, users will be allowed to email topics.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailTopic" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Quick Answer:</strong><br />
                            Enable or disable display of the Quick Reply Box at the bottom of the Posts page
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowQuickAnswer" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable calendar:</strong><br />
                            Enables/disables calendar in profile, if it causes troubles on servers/for users
                            with different cultures.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableDNACalendar" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Poll Options
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allowed Poll Number:</strong><br />
                            Number of polls, max value no more then 99.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AllowedPollNumber" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allowed Poll Choice Number:</strong><br />
                            Number of a question choices, max value no more then 99.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AllowedPollChoiceNumber" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Poll Votes Dependant on IP:</strong><br />
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
                            <strong>Allow Poll Changes After First Vote:</strong><br />
                            If enabled a poll creator can change choices and question after the first vote was
                            given.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPollChangesAfterFirstVote" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Users Hide Poll Results:</strong><br />
                            If enabled a poll creator can hide results before voting end or if not all polls in a group are voted.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUsersHidePollResults" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Guests View Poll Options:</strong><br />
                            If enabled Guests can see poll choices.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowGuestsViewPollOptions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="header1" colspan="2">
                            Private Messages
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <strong>Allow Private Messages:</strong><br />
                            Allow users to access and send private messages.
                            You should explicitly give permission for each group and/or rank too to enable them for users.                               
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPrivateMessages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Private Message Notifications:</strong><br />
                            Allow users email notifications when new private messages arrive.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPMEmailNotification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Max no. of PM Recipients:</strong><br />
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
                            <strong>Enable Album Feature:</strong><br />
                            If checked, album feature is enabled. You should set allowed number of images and albums for each group and/or rank too, to enable the feature. 
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableAlbum" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Maximum Image Size:</strong><br />
                            Maximum size of image in bytes a user can upload.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AlbumImagesSizeMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Albums Per Page:</strong><br />
                            Number of albums to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AlbumsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Images Per Page:</strong><br />
                            Number of images to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AlbumImagesPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            Syndication Feed Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show RSS Links:</strong><br />
                            Enable or disable display of RSS links throughout the forum.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRSSLink" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Atom Links:</strong><br />
                            Enable or disable display of Atom links throughout the forum.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowAtomLink" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Posts Feeds Access:</strong><br />
                            Restrict display of posts feeds for a topic.
                        </td>              
                        <td class="post">
                              <asp:DropDownList ID="PostsFeedAccess" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Post Latest Feeds Access:</strong><br />
                            Restrict display of posts feeds for latest posts.
                        </td>              
                        <td class="post">
                              <asp:DropDownList ID="PostLatestFeedAccess" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Forum Feeds Access:</strong><br />
                            Restrict display of forum feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList ID="ForumFeedAccess" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Topics Feeds Access:</strong><br />
                            Restrict display of topics feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList ID="TopicsFeedAccess" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Active Topics Feeds Access:</strong><br />
                            Restrict display of active topics feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList ID="ActiveTopicFeedAccess" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Favorite Topics Feeds Access:</strong><br />
                            Restrict display of active topics feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList ID="FavoriteTopicFeedAccess" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                     <tr>
                        <td class="header1" colspan="2">
                            CAPTCHA Settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>CAPTCHA Size:</strong><br />
                            Size (length) of the CAPTCHA random alphanumeric string
                        </td>
                        <td class="post">
                            <asp:TextBox ID="CaptchaSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>reCAPTCHA Public Key:</strong><br />
                            Enter a reCAPTCHA Public Key
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RecaptchaPublicKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>reCAPTCHA Private Key:</strong><br />
                            Enter a reCAPTCHA Private Key
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RecaptchaPrivateKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable reCAPTCHA Multiple Instances:</strong><br />
                            Enable reCAPTCHA Recapture Multiple Instances(shared keys).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RecaptureMultipleInstances" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable CAPTCHA for Guest Posting:</strong><br />
                            Require guest users to enter the CAPTCHA when they post or reply to a forum message
                            (including Quick Reply).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableCaptchaForGuests" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable CAPTCHA for Post a Message:</strong><br />
                            Require users to enter the CAPTCHA when they post or reply to a forum message (including
                            Quick Reply).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableCaptchaForPost" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable CAPTCHA/reCAPTCHA for Register:</strong><br />
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
                            Log settings
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Message history archieve time</strong><br />
                            Number of days to keep message change history.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MessageHistoryDaysToLog" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable Active Location Error Log:</strong><br />
                            If checked, all active location path errors are logged.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableActiveLocationErrorsLog" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Enable Unhandled UserAgent Log:</strong><br />
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
                            <strong>Active Users Time:</strong><br />
                            Number of minutes to display users in Active Users list.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ActiveListTime" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Avatars in Topic Listing:</strong><br />
                            If this is checked, the topic pages will show avatar graphics.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowAvatarsInTopic" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Moved Topics:</strong><br />
                            If this is checked, topics that are moved will leave behind a pointer to the new
                            topic.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowMoved" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Moderator List:</strong><br />
                            If this is checked, the moderator list column is displayed in the forum list.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowModeratorList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                                        <tr>
                        <td class="postheader">
                            <strong>Show Guests In Detailed Active List:</strong><br />
                            If checked, Guests will be displayed In Detailed Active List.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGuestsInDetailedActiveList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Crawlers In Active Lists:</strong><br />
                            If checked, Crawlers will be displayed In Active Lists.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowCrawlersInDetailedActiveList" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Deleted Messages:</strong><br />
                            If this is checked, messsages that are deleted will leave with some notes
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowDeletedMessages" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Deleted Messages to All:</strong><br />
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
                            <strong>Show Links in New Window:</strong><br />
                            If this is checked, links in messages will open in a new window.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="BlankLinks" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show 'no-count' Forum Posts in Active Discussions :</strong><br />
                            If this is checked, posts from no-count forums will be displayed in Active Topics.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="NoCountForumsInActiveDiscussions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Forum Statistics:</strong><br />
                            Enable or disable display of forum statistics on board index page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowForumStatistics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Active Discussions:</strong><br />
                            Enable or disable display of active discussions list on board index page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowActiveDiscussions" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                    
                    <tr>
                        <td class="postheader">
                            <strong>Show Forum Jump Box:</strong><br />
                            Enable or disable display of the Forum Jump Box throughout the forum.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowForumJump" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Shoutbox:</strong><br />
                            Enable or disable display of the Shoutbox (Chat Module) in the forum page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowShoutbox" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Shoutbox Smiles:</strong><br />
                            Enable or disable display of the Shoutbox (Chat Module) smiles.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowShoutboxSmiles" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Groups:</strong><br />
                            Should the groups a user is part of be visible on the posts page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGroups" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Groups in profile:</strong><br />
                            Should the groups a user is part of be visible on the users profile page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowGroupsProfile" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Medals:</strong><br />
                            Should medals of a user be visible on the posts page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowMedals" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Users Browsing:</strong><br />
                            Should users currently browsing forums/topics be displayed at the bottom.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowBrowsingUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Page Generated Time:</strong><br />
                            Enable or disable display of page generation text at the bottom of the page.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowPageGenerationTime" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show YetAnotherForum Version:</strong><br />
                            Enable or disable display of the version/date information the bottom of the page
                            (disable if your concerned about security).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowYAFVersion" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <strong>Show Help:</strong><br />
                            Enable or disable display the Help Link in the Header that Shows the Help Files Pages
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowHelp" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Join Date:</strong><br />
                            If checked, join date will be displayed for each user.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisplayJoinDate" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show "Rules" Before Registration:</strong><br />
                            Require that "rules" are shown and accepted before a new user can register.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRulesForRegistration" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Active Discussions Count:</strong><br />
                            Number of records to display in Active Discussions list on forum index.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ActiveDiscussionsCount" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Use "NoFollow" Tag in Links:</strong><br />
                            If this is checked, all links will have the nofollow tag.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseNoFollowLinks" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Posts Per Page:</strong><br />
                            Number of posts to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="PostsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Topics Per Page:</strong><br />
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
                            <strong>2nd post ad:</strong><br />
                            Place the code that you wish to be displayed in each thread after the 1st post.
                            If you do not want an ad to be displayed, don't put anything in the box.
                        </td>
                        <td class="post">
                            <asp:TextBox TextMode="MultiLine" runat="server" ID="AdPost" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show Ad to "Signed In" Users:</strong><br />
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
                            <strong>Forum Editor:</strong><br />
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
                            <strong>Accepted HTML Tags:</strong><br />
                            Comma seperated list (no spaces) of HTML tags that are allowed in posts using HTML
                            editors.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AcceptedHTML" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Accepted Header HTML Tags:</strong><br />
                            Comma seperated list (no spaces) of HTML tags that are allowed in posts headers for common users
                            editors.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AcceptedHeadersHTML" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Use styled nicks:</strong><br />
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
                            <strong>Allow User Change Theme:</strong><br />
                            Should users be able to choose what theme they want to use?
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserTheme" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow User Change Language:</strong><br />
                            Should users be able to choose what language they want to use?
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserLanguage" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Signatures:</strong><br />
                            Allow users to create signatures. You should set allowed number of characters and BBCodes for each group and/or rank to really enable the feature.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowSignatures" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="postheader">
                            <strong>Allow Email Sending:</strong><br />
                            Allow users to send emails to each other.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailSending" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Email Change:</strong><br />
                            Allow users to change their email address.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Password Change:</strong><br />
                            Allow users to change their passwords.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPasswordChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Moderators View IPs:</strong><br />
                            Allow to view IPs to moderators.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowModeratorsViewIPs" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Notification of All Posts on All topics:</strong><br />
                            Allow users to get individual email notifications on all emails -- tons of email
                            traffic.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowNotificationAllPostsAllTopics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Report Post Permissions:</strong><br />
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
                            <strong>Profile Viewing Permissions:</strong><br />
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
                            <strong>Members List Viewing Permissions:</strong><br />
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
                            <strong>Active Users Viewing Permissions:</strong><br />
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
                            <strong>Max Word Length:</strong><br />
                            Use it to limit number of a word characters in topic names and some other places.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="MaxWordLength" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Use SSL while logging in:</strong><br />
                            Enforce a secure connection for users to log in.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToLogIn" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Use SSL while registering:</strong><br />
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
                            <strong>User box template:</strong><br />
                            Template for rendering user box by user's posts.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Avatar template:</strong><br />
                            Template for rendering avatar.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxAvatar" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Medals template:</strong><br />
                            Template for rendering user's medals.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxMedals" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Rank image template:</strong><br />
                            Template for rendering rank image.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxRankImage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Rank template:</strong><br />
                            Template for rendering user's rank.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxRank" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Groups template:</strong><br />
                            Template for rendering user's groups.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxGroups" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Join date template:</strong><br />
                            Template for rendering user's joine date.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxJoinDate" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Posts template:</strong><br />
                            Template for rendering user's posts.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Points template:</strong><br />
                            Template for rendering user's points.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxPoints" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Location template:</strong><br />
                            Template for rendering user's location.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxLocation" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Gender:</strong><br />
                            Template for rendering user's gender.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxGender" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Thanks From template:</strong><br>
                            Template for rendering user's thanks from.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="UserBoxThanksFrom" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Thanks To template:</strong><br>
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
                            <strong>Allow remote avatars:</strong><br />
                            Can users use avatars from other websites.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarRemote" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow avatar uploading:</strong><br />
                            Can users upload avatars to their profile.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarUpload" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow Gravatars:</strong><br />
                            Automatically use users Gavatars if they exist (note: may require additional processing).
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarGravatar" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Gravatar Rating:</strong><br />
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
                            <strong>Avatar Width:</strong><br />
                            Maximum width for avatars.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AvatarWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Avatar Height:</strong><br />
                            Maximum height for avatars.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="AvatarHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Avatar Size:</strong><br />
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
                            <strong>Forum Statistics Cache Timeout:</strong><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ForumStatisticsCacheTimeout" />
                            <asp:Button ID="ForumStatisticsCacheReset" Text="Clear" runat="server" OnClick="ForumStatisticsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Board User Statistics Cache Timeout:</strong><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="BoardUserStatsCacheTimeout" />
                            <asp:Button ID="BoardUserStatsCacheReset" Text="Clear" runat="server" OnClick="BoardUserStatsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Active Discussions Cache Timeout:</strong><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ActiveDiscussionsCacheTimeout" />
                            <asp:Button ID="ActiveDiscussionsCacheReset" Text="Clear" runat="server" OnClick="ActiveDiscussionsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Board Categories Cache Timeout:</strong><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="BoardCategoriesCacheTimeout" />
                            <asp:Button ID="BoardCategoriesCacheReset" Text="Clear" runat="server" OnClick="BoardCategoriesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Board Moderators Cache Timeout:</strong><br />
                            In minutes
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="BoardModeratorsCacheTimeout" />
                            <asp:Button ID="BoardModeratorsCacheReset" Text="Clear" runat="server" OnClick="BoardModeratorsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Replace Rules Cache Timeout:</strong><br />
                            Smilies, BB code, bad wordsm, etc. (in minutes)
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="ReplaceRulesCacheTimeout" />
                            <asp:Button ID="ReplaceRulesCacheReset" Text="Clear" runat="server" OnClick="ReplaceRulesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>First Post "Title" Cache Timeout:</strong><br />
                            First Post "Title" for SEO Cache Timeout (in minutes)
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="FirstPostCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Online User Status Cache Timeout:</strong><br />
                            You can fine-tune it depending on your site activity (in milliseconds)
                        </td>
                        <td class="post">
                            <asp:TextBox runat="server" ID="OnlineStatusCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>User Lazy Data Cache Timeout:</strong><br />
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
                            <strong>Max Search Results:</strong><br />
                            Maximum number of search results that can be returned. Enter "0" for unlimited (not
                            recommended).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="ReturnSearchMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Use SQL Full Text Search:</strong><br />
                            Toggle use of FULLTEXT SQL Server support on searches.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFullTextSearch" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Search Text Minimal Length:</strong><br />
                            Minimal length of the search string allowed.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchStringMinLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Search Text Maximum Length:</strong><br />
                            Maximum length of the search string allowed.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchStringMaxLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Search Text Pattern:</strong><br />
                            Allowed search text (Regular Expression) pattern.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchStringPattern" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Search Permissions:</strong><br />
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
                            <strong>Search Engine 1:</strong><br />
                            Enter here a search engine pattern.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine1" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Parameters For Search Engine 1:</strong><br />
                            Enter here a search engine parameters.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine1Parameters" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Search Engine 2:</strong><br />
                            Enter here a search engine pattern.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Parameters For Search Engine 2:</strong><br />
                            Enter here a search engine parameters.
                        </td>
                        <td class="post">
                            <asp:TextBox ID="SearchEngine2Parameters" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>External Search Permissions:</strong><br />
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
                            <strong>External Search Results In A New Window:</strong><br />
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
