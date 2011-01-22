<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.hostsettings"
    CodeBehind="hostsettings.ascx.cs" %>
<%@ Register TagPrefix="YAF" TagName="PMList" Src="../../controls/PMList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
  <asp:Panel id="HostSettingsTabs" runat="server">
    <ul>
		<li><a href="#View1"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
		<li><a href="#View2"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HOST_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
		<li><a href="#View3"><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HOST_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View4"><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HOST_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View5"><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="HOST_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View6"><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="HOST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View7"><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="HOST_TEMPLATES" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View8"><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="HOST_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View9"><YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="HOST_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
        <li><a href="#View10"><YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="HOST_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" /></a></li>
	</ul>
    <div id="View1">
		<table class="content" cellspacing="2" width="100%" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                           <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="HEADER_SETUP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" width="50%">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="SERVER_VERSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post" width="50%">
                            <asp:Label ID="SQLVersion" runat="server" CssClass="smallfont"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="SERVERTIME_CORRECT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <strong><%# DateTime.UtcNow %></strong>.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ServerTimeCorrection" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="FORUM_EMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ForumEmail" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="EMAIL_VERIFICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EmailVerification" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="FILE_TABLE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFileTable" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="ABANDON_TRACKUSR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AbandonSessionsForDontTrack" runat="server"></asp:CheckBox>
                        </td>
                    </tr>

                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="MAX_ATTACHMENTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxNumberOfAttachments" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="MAX_FILESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxFileSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="POSTEDIT_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="EditTimeOut" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="WSERVICE_TOKEN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="WebServiceToken" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="NAME_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserNameMaxLength" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="MAX_POST_CHARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxReportPostChars" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="MAX_POST_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxPostSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="FLOOT_DELAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PostFloodDelay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="REFERRER_CHECK" LocalizedPage="ADMIN_HOSTSETTINGS" />
                            <strong></strong><br />
                            
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DoUrlReferrerSecurityCheck" runat="server"></asp:CheckBox>
                        </td>
                    </tr>      
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="CREATE_NNTPNAMES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="CreateNntpUsers" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel21" runat="server" LocalizedTag="HEADER_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel17" runat="server" LocalizedTag="DISABLE_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="DisableRegistrations" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel18" runat="server" LocalizedTag="LOGIN_REDIR_URL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="CustomLoginRedirectUrl" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel19" runat="server" LocalizedTag="REQUIRE_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="RequireLogin" runat="server" />
                        </td>
                    </tr>
                    <tr>
                         <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel20" runat="server" LocalizedTag="MODAL_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseLoginBox" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel22" runat="server" LocalizedTag="HEADER_IMAGE_ATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel21" runat="server" LocalizedTag="DISPLAY_TRESHOLD_IMGATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PictureAttachmentDisplayTreshold" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel22" runat="server" LocalizedTag="IMAGE_ATTACH_RESIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableImageAttachmentResize" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel23" runat="server" LocalizedTag="IMAGE_RESIZE_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ImageAttachmentResizeWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel24" runat="server" LocalizedTag="IMAGE_RESIZE_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ImageAttachmentResizeHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel25" runat="server" LocalizedTag="CROP_IMAGE_ATTACH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ImageAttachmentResizeCropped" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View2">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HEADER_FEATURES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>          
                    <tr>
                        <td class="postheader" style="width:50%">
                            <strong>Show Relative Time:</strong><br />
                            If checked, client-side "Time Ago" library will be used to show "relative times" to users
                            instead of UTC and or imperfect server-side times.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowRelativeTime" runat="server"></asp:CheckBox>
                        </td>
                    </tr> 
                    <tr>
                        <td class="postheader" style="width:50%">
                            <strong>Relative Time Refresh Rate:</strong><br />
                            This allows to set how often the "Time Ago" libary should refresh the relative time (In Milliseconds, 60000 ms = 60 seconds = 1 minute).
                        </td>
                        <td class="post">
                            <asp:TextBox ID="RelativeTimeRefreshTime" Style="width:350px" runat="server"></asp:TextBox>
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
                            <strong>Member List Page Size:</strong><br />
                            Number entries on a page in the members list. 
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="MemberListPageSize" />                          
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
                            <asp:TextBox Style="width:350px" ID="DisableNoFollowLinksAfterDay" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Smilies Display per Row:</strong><br />
                            Number of smilies to show per row.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="SmiliesPerRow" runat="server"></asp:TextBox>
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
                            <asp:TextBox Style="width:350px" ID="LockPosts" runat="server"></asp:TextBox>
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
                            <asp:TextBox Style="width:350px" ID="IPLocatorPath" runat="server"></asp:TextBox>
                        </td>
                    </tr>           
                    <tr>
                        <td class="postheader">
                            <strong>IP Info Page URL:</strong><br />
                            Set it to get details about IPs whereabouts as web page.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="IPInfoPageURL" runat="server"></asp:TextBox>
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
                            <YAF:LocalizedLabel ID="LocalizedLabel23" runat="server" LocalizedTag="HEADER_POLL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allowed Poll Number:</strong><br />
                            Number of polls, max value no more then 99.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AllowedPollNumber" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allowed Poll Choice Number:</strong><br />
                            Number of a question choices, max value no more then 99.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AllowedPollChoiceNumber" MaxLength="2" runat="server"></asp:TextBox>
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
                            <strong>Allow Multiple Choices Voting:</strong><br />
                            If enabled users can create poll questions allowing multiple choices voting.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowMultipleChoices" runat="server"></asp:CheckBox>
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
                        <td class="postheader">
                            <strong>Allow Users Poll Images:</strong><br />
                            If enabled Users can add images as poll options.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUsersImagedPoll" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                   <tr>
                        <td class="postheader">
                            <strong>Poll Image FileSize:</strong><br />
                            Max file size for poll images in KB.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="PollImageMaxFileSize" MaxLength="4" runat="server"></asp:TextBox>
                        </td>
                    </tr>                  
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel24" runat="server" LocalizedTag="HEADER_PMS" LocalizedPage="ADMIN_HOSTSETTINGS" />
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
                            <asp:TextBox Style="width:350px" ID="PrivateMessageMaxRecipients" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel25" runat="server" LocalizedTag="HEADER_ALBUM" LocalizedPage="ADMIN_HOSTSETTINGS" />
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
                            <asp:TextBox Style="width:350px" ID="AlbumImagesSizeMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Albums Per Page:</strong><br />
                            Number of albums to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AlbumsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Images Per Page:</strong><br />
                            Number of images to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AlbumImagesPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel26" runat="server" LocalizedTag="HEADER_SYNDICATION" LocalizedPage="ADMIN_HOSTSETTINGS" />
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
                              <asp:DropDownList Style="width:350px" ID="PostsFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Post Latest Feeds Access:</strong><br />
                            Restrict display of posts feeds for latest posts.
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="PostLatestFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Forum Feeds Access:</strong><br />
                            Restrict display of forum feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="ForumFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Topics Feeds Access:</strong><br />
                            Restrict display of topics feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="TopicsFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Active Topics Feeds Access:</strong><br />
                            Restrict display of active topics feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="ActiveTopicFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Favorite Topics Feeds Access:</strong><br />
                            Restrict display of active topics feeds.
                        </td>              
                        <td class="post">
                              <asp:DropDownList Style="width:350px" ID="FavoriteTopicFeedAccess" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel27" runat="server" LocalizedTag="HEADER_IRKOO" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                     <tr>
                        <td class="postheader">
                            <strong>Use Irkoo Reputation Service:</strong><br />
                            Irkoo is a free service that adds user reputations to your site 
                            Visit <strong>http://www.Irkoo.com</strong> to get the site ID and secret key for your website.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="EnableIrkoo" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Irkoo site ID:</strong><br />
                            Enter your Irkoo site ID.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="IrkooSiteID" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Irkoo secret key:</strong><br />
                            Enter your Irkoo secret key.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="IrkooSecretKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Show user reputations in user links only in topic pages:</strong><br />
                            If checked, users' reputation will be displayed only in topic pages.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowIrkooRepOnlyInTopics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Allow guests to view users' reputations:</strong><br />
                            If checked, guests can view members' reputations.
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowGuestsViewReputation" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel28" runat="server" LocalizedTag="HEADER_CAPTCHA" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>CAPTCHA Size:</strong><br />
                            Size (length) of the CAPTCHA random alphanumeric string
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="CaptchaSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>reCAPTCHA Public Key:</strong><br />
                            Enter a reCAPTCHA Public Key
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="RecaptchaPublicKey" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>reCAPTCHA Private Key:</strong><br />
                            Enter a reCAPTCHA Private Key
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="RecaptchaPrivateKey" runat="server"></asp:TextBox>
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
                            <asp:DropDownList Style="width:350px" ID="CaptchaTypeRegister" runat="server">
                                <asp:ListItem Value="0" Text="Disabled" />
                                <asp:ListItem Value="1" Text="YafCaptcha" />
                                <asp:ListItem Value="2" Text="ReCaptcha" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel29" runat="server" LocalizedTag="HEADER_LOG" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Message history archieve time</strong><br />
                            Number of days to keep message change history.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MessageHistoryDaysToLog" runat="server"></asp:TextBox>
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
	</div>
    <div id="View3">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="HEADER_DISPLAY" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Active Users Time:</strong><br />
                            Number of minutes to display users in Active Users list.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="ActiveListTime" runat="server" />
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
                            <asp:CheckBox ID="ShowCrawlersInActiveList" runat="server"></asp:CheckBox>
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
                            If this is checked, posts from 'no-count' forums will be displayed in Active Discussions.
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
                            <asp:TextBox Style="width:350px" ID="ActiveDiscussionsCount" runat="server" />
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
                            <asp:TextBox Style="width:350px" ID="PostsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <strong>Topics Per Page:</strong><br />
                            Number of topics to show per page.
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="TopicsPerPage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View4">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="HEADER_ADVERTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:450px">
                            <YAF:HelpLabel ID="HelpLabel26" runat="server" LocalizedTag="POST_AD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px;" TextMode="MultiLine" runat="server" ID="AdPost" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel27" runat="server" LocalizedTag="SHOWAD_LOGINUSERS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="ShowAdsToSignedInUsers" />
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View5">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="HEADER_EDITORS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:450px">
                            <YAF:HelpLabel ID="HelpLabel88" runat="server" LocalizedTag="FORUM_EDITOR" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ForumEditor" runat="server" DataValueField="Value" DataTextField="Name">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel87" runat="server" LocalizedTag="ACCEPT_HTML" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px;" ID="AcceptedHTML" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel86" runat="server" LocalizedTag="ACCEPT_HEADER_HTML" LocalizedPage="ADMIN_HOSTSETTINGS" />
                           <strong></strong><br />
                            
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px;" ID="AcceptedHeadersHTML" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel85" runat="server" LocalizedTag="STYLED_NICKS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox runat="server" ID="UseStyledNicks" />
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View6">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel16" runat="server" LocalizedTag="HEADER_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel84" runat="server" LocalizedTag="USER_CHANGE_THEME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserTheme" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel83" runat="server" LocalizedTag="USER_CHANGE_LANGUAGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowUserLanguage" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel82" runat="server" LocalizedTag="ALLOW_SIGNATURE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowSignatures" runat="server"></asp:CheckBox>
                        </td>
                    </tr>                   
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel81" runat="server" LocalizedTag="ALLOW_SENDMAIL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailSending" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel80" runat="server" LocalizedTag="ALLOW_EMAIL_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowEmailChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel79" runat="server" LocalizedTag="ALLOW_PASS_CHANGE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowPasswordChange" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel78" runat="server" LocalizedTag="ALLOW_MOD_VIEWIP" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowModeratorsViewIPs" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel77" runat="server" LocalizedTag="ALLOW_NOTIFICATION_ONALL" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AllowNotificationAllPostsAllTopics" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel76" runat="server" LocalizedTag="REPORT_POST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ReportPostPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                         <td class="postheader">
                             <YAF:HelpLabel ID="HelpLabel89" runat="server" LocalizedTag="ALLOW_TOPICS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="AllowCreateTopicsSameName" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                             <YAF:HelpLabel ID="HelpLabel90" runat="server" LocalizedTag="ALLOW_FORUMS_DUPLICATENAME" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                       <td class="post">
                            <asp:CheckBox ID="AllowForumsWithSameName" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel75" runat="server" LocalizedTag="VIEWPROFILE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ProfileViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel74" runat="server" LocalizedTag="VIEWMEMBERLIST_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="MembersListViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel73" runat="server" LocalizedTag="VIEWACTIVE_PERMISSION" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="ActiveUsersViewPermissions" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel72" runat="server" LocalizedTag="MAX_WORD_LENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="MaxWordLength" MaxLength="2" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel71" runat="server" LocalizedTag="SSL_LOGIN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToLogIn" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                           <YAF:HelpLabel ID="HelpLabel70" runat="server" LocalizedTag="SSL_REGISTER" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseSSLToRegister" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View7">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel17" runat="server" LocalizedTag="HEADER_TEMPLATES" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel57" runat="server" LocalizedTag="USERBOX_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBox" TextMode="MultiLine" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel58" runat="server" LocalizedTag="AVATAR_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxAvatar" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel59" runat="server" LocalizedTag="MEDALS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxMedals" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel60" runat="server" LocalizedTag="RANKIMAGE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxRankImage" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel61" runat="server" LocalizedTag="RANK_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxRank" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel62" runat="server" LocalizedTag="GROUPS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxGroups" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel63" runat="server" LocalizedTag="JOINDATE_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxJoinDate" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel64" runat="server" LocalizedTag="POSTS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxPosts" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel65" runat="server" LocalizedTag="POINTS_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxPoints" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel66" runat="server" LocalizedTag="LOCATION_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxLocation" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel67" runat="server" LocalizedTag="GENDER_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxGender" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel68" runat="server" LocalizedTag="THANKS_FROM_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxThanksFrom" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel69" runat="server" LocalizedTag="THANKS_TO_TEMPLATE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="UserBoxThanksTo" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View8">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel18" runat="server" LocalizedTag="HEADER_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel50" runat="server" LocalizedTag="REMOTE_AVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarRemote" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel51" runat="server" LocalizedTag="AVATAR_UPLOAD" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarUpload" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel52" runat="server" LocalizedTag="ALLOW_GRAVATARS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="AvatarGravatar" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel53" runat="server" LocalizedTag="GRAVATAR_RATING" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:350px" ID="GravatarRating" runat="server">
                                <asp:ListItem Value="G"></asp:ListItem>
                                <asp:ListItem Value="PG"></asp:ListItem>
                                <asp:ListItem Value="R"></asp:ListItem>
                                <asp:ListItem Value="X"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel54" runat="server" LocalizedTag="AVATAR_WIDTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AvatarWidth" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel55" runat="server" LocalizedTag="AVATAR_HEIGHT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AvatarHeight" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel56" runat="server" LocalizedTag="AVATAR_SIZE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" ID="AvatarSize" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View9">
		<table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel19" runat="server" LocalizedTag="HEADER_CACHE" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel41" runat="server" LocalizedTag="STATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ForumStatisticsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ForumStatisticsCacheReset" Text="Clear" runat="server" OnClick="ForumStatisticsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel42" runat="server" LocalizedTag="USRSTATS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="BoardUserStatsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="BoardUserStatsCacheReset" Text="Clear" runat="server" OnClick="BoardUserStatsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel43" runat="server" LocalizedTag="DISCUSSIONS_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ActiveDiscussionsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ActiveDiscussionsCacheReset" Text="Clear" runat="server" OnClick="ActiveDiscussionsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel44" runat="server" LocalizedTag="CAT_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="BoardCategoriesCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="BoardCategoriesCacheReset" Text="Clear" runat="server" OnClick="BoardCategoriesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel45" runat="server" LocalizedTag="MOD_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="BoardModeratorsCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="BoardModeratorsCacheReset" Text="Clear" runat="server" OnClick="BoardModeratorsCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel46" runat="server" LocalizedTag="REPLACE_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ReplaceRulesCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ReplaceRulesCacheReset" Text="Clear" runat="server" OnClick="ReplaceRulesCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel47" runat="server" LocalizedTag="SEO_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="FirstPostCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel48" runat="server" LocalizedTag="ONLINE_STATUS_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="OnlineStatusCacheTimeout" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel49" runat="server" LocalizedTag="LAZY_CACHE_TIMEOUT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:350px" runat="server" ID="ActiveUserLazyDataCacheTimeout" />
                            <asp:Button CssClass="pbutton" ID="ActiveUserLazyDataCacheReset" Text="Clear" runat="server" OnClick="UserLazyDataCacheReset_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td class="footer1" colspan="2" style="text-align:center">
                            <asp:Button CssClass="pbutton" runat="server" ID="ResetCacheAll" Text="Clear Cache" OnClick="ResetCacheAll_Click" />
                        </td>
                    </tr>
                </table>
	</div>
    <div id="View10">
                <table class="content" width="100%" cellspacing="2" cellpadding="2" align="center">
                    <tr>
                        <td class="header1" colspan="2">
                            <YAF:LocalizedLabel ID="LocalizedLabel20" runat="server" LocalizedTag="HEADER_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader" style="width:450px">
                            <YAF:HelpLabel ID="HelpLabel28" runat="server" LocalizedTag="MAX_SEARCH_RESULTS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="ReturnSearchMax" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel29" runat="server" LocalizedTag="SQL_FULLTEXT" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="UseFullTextSearch" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel30" runat="server" LocalizedTag="SEARCH_MINLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="SearchStringMinLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel31" runat="server" LocalizedTag="SEARCH_MAXLENGTH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="SearchStringMaxLength" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel32" runat="server" LocalizedTag="SEARCH_PATTERN" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%" ID="SearchStringPattern" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel33" runat="server" LocalizedTag="SEARCH_PERMISS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:99%" ID="SearchPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel34" runat="server" LocalizedTag="SEARCH_ENGINE1" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine1" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel35" runat="server" LocalizedTag="SEARCH_ENGINE1_PARAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine1Parameters" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel36" runat="server" LocalizedTag="SEARCH_ENGINE2" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine2" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel40" runat="server" LocalizedTag="SEARCH_ENGINE2_PARAM" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:TextBox Style="width:99%;height:80px" ID="SearchEngine2Parameters" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel37" runat="server" LocalizedTag="EXTERN_SEARCH_PERMISS" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:DropDownList Style="width:99%" ID="ExternalSearchPermissions" runat="server">
                                <asp:ListItem Value="0" Text="Forbidden" />
                                <asp:ListItem Value="1" Text="Registered Users" />
                                <asp:ListItem Value="2" Text="All Users" />
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel38" runat="server" LocalizedTag="EXTERN_NEWWINDOW" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ExternalSearchInNewWindow" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="postheader">
                            <YAF:HelpLabel ID="HelpLabel39" runat="server" LocalizedTag="QUICK_SEARCH" LocalizedPage="ADMIN_HOSTSETTINGS" />
                        </td>
                        <td class="post">
                            <asp:CheckBox ID="ShowQuickSearch" runat="server"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
        </div>
    </asp:Panel>
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="postfooter" align="center">
                <asp:Button ID="Save" runat="server" Text="Save Settings" CssClass="pbutton" OnClick="Save_Click">
                </asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
