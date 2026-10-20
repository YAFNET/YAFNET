YetAnotherForum.NET Changelog
====================

# YetAnotherForum.NET v4.0.10
* [NEW] Extended unit test suite including repository tests against an in-memory SQLite database
* [NEW] UI tests for changing the e-mail address, two-factor authentication, deleting the own account, the login of not approved accounts, password reset links, editing posts of other users and the history of deleted posts
* [NEW] Form UI tests: save the edit forms of the admin pages, the user editor and the profile without changes (the values must stay the same, no error, and the stored values must be accepted by the fields), and with changed text fields (the changes must be saved)
* [NEW] Crawl UI test: follows the links of the forum as admin, member, guest, forum moderator (no admin) and a member with an other language and time zone (with polls, albums, moved posts, tags, bb codes with scripts in their values, display names and new forums, categories, groups, ranks, medals and profile fields), calls every kind of page with invalid ids, and checks for server errors, error pages, errors in the event log and script of user input (html with a script in names, titles, tags and profile fields) running on any page
* [CHANGES] The client address behind a reverse proxy is taken from X-Forwarded-For / X-Real-IP only for requests of trusted proxies (ASP.NET Core forwarded headers middleware). Proxies in loopback and private networks (same host, LAN, docker) are trusted by default; a proxy or CDN with public addresses (e.g. Cloudflare) has to be added to the new setting BoardConfiguration:TrustedProxies (addresses or CIDR networks), otherwise its address is used as the client address
* [CHANGES] The BBCode rule engine is now async: IReplaceRule.ReplaceAsync, IProcessReplaceRules.ProcessAsync, IBBCodeService.MakeHtmlAsync and ConvertHtmlToBBCodeForEditAsync replace the synchronous methods (breaking change for custom rules that implement IReplaceRule directly, rules based on BaseReplaceRule keep working)
* [CHANGES] Html.LocalizedText with BBCode is now Html.LocalizedTextAsync
* [CHANGES] TopicRepository Prune is now PruneAsync(boardId, forumId, days, permDelete) and deletes the topics
* [CHANGES] Missing translations are shown as [PAGE.TAG] and logged to the event log
* [CHANGES] The unread private messages popup is shown only once per new message (like the pending friends popup), not on every page
* [CHANGES] The web service requests (StopForumSpam, BotScout, AbuseIPDB, IP locator, latest version check) use the IHttpClientFactory (registered with AddHttpClient) instead of a new HttpClient for every request
* [CHANGES] The login cookie contains the security stamp of the user now, after the update all users have to log in again once
* [CHANGES] Changing the own e-mail address needs the current password and is only done after the new address is confirmed with the link of the confirmation e-mail; suspending or deleting the own account needs the current password (accounts of external logins without a password are not asked)
* [CHANGES] "Email this topic" only lets the user choose the recipient (one address), the subject (topic name) and the text (link to the topic) are fixed, and a user can send at most 5 such emails per hour, so the forum can not be used to send spam
* [FIXED] Security: Admin > Languages / Edit Language are host admin pages now: the language files are used by all boards and their texts are html, the admin of one board could put script into the pages of all boards (also for the host admin); the pages can not be granted to board admins anymore
* [FIXED] Security: The test data generator (Admin > Test Data) was only hidden in release builds, its "create" handler still worked for admins with the page access and created boards, users, categories and private messages in any board; all handlers are refused in release builds now and the page is host admin only (also in debug builds)
* [FIXED] Security: Admin > Edit Board had no host admin check and could be granted to the admins of a board (unlike the list of the boards), they could create boards and rename any board; it is host admin only now
* [FIXED] Security: The select lists with icons (category and parent forum of Admin > Edit Forum, the forums of the moderators on the team page, the image, medal, avatar and country lists) showed the texts of the options as html, a forum or category name with html ran script for every user opening the page (on Edit Forum also for the host admin), a quote in a name broke the list; the texts are encoded now
* [FIXED] Security: The forum jump and the other forum selects (search, move topic/message, moderation, prune, delete forum) showed the names of the categories as html, a category name with html ran script on these pages for every user (the forum names were already encoded)
* [FIXED] Security: The tags of a topic were shown as html in the tags field (edit post, tag search of the post forms), a tag with an unclosed html tag (not removed when the topic is saved) ran script for the moderators editing the post and for the users finding the tag
* [FIXED] Security: The admin pages showed the names and descriptions of groups, ranks, access masks, medals (also the category and the messages of the awards) and profile fields, the styles of groups and ranks and the reasons of banned ips as html (also the group names on Edit Forum and Edit Medal), the admin of a board could run script in the pages of the host admin; they are encoded now
* [FIXED] Security: The name of the board was shown as html in the header of every page, in the login notice for guests, in the cookie notice and as the title of the notifications, the admin of a board could run script for all users (also the host admin)
* [FIXED] Security: The simple select lists (e.g. the rank and group filter of the members page, the groups of Admin > Mail, the access masks of Admin > Settings) showed the texts of the options as html
* [FIXED] Security: The names of the custom profile fields (set by the admin of the board) were shown as html in the messages of invalid or missing values (register page, edit profile, admin edit user), the user name of Admin > Register User, the file type of an album upload and the error of a failed attachment upload were shown as html in the notifications
* [FIXED] Security: With display names turned on, the display name of the last poster was shown as html in the topic lists ("by ..."), and the display name of the user in the header of the e-mail and albums pages, a member could run script for everybody viewing the topic list (display names can contain any character)
* [FIXED] Security: The style of a topic title (set by moderators) was written as html into the last topic link of the forum list, it could add attributes like event handlers (script for every visitor, also admins); it is encoded now and only used with "styled topic titles" enabled
* [FIXED] Security: The search index contained the hidden content of posts (hide, hide-reply, hide-thanks, group-hide, hide-posts): every user could find posts by words of hidden content and see them in the highlighted part of the result. Hidden content is not indexed anymore; run Admin > ReIndex to remove it from an existing index
* [FIXED] Security: The html part of the notification e-mails (topic and forum watch mails, reported posts, approval, medals, welcome...) contained the values of users as html and bb code: a topic name, a display name or the reason of a report could add links, images or forms (also disguised [url] links and html in [code] blocks) to the mails other users and the moderators get. The values are text in the html mail now (html encoded, their brackets are no bb codes), only the template text itself is formatted
* [FIXED] Security: Removing a thanks returned the users who thanked a message (names and avatars) for any message id, also of forums the user can't read
* [FIXED] Security: The page load took the forum (and so the access rights) from the forum and category ids of the address when both were set, a topic or message id of another forum was not checked against them; e.g. the print page showed a topic of a hidden forum with the ids of a readable forum in the address (PrintTopic/{hidden topic}?f={readable forum}&c={category}). The forum is now always the forum of the topic/message
* [FIXED] Security: The api of the admin dialogs (banned ips, e-mails, names, user agents and countries, bb code, spam word and replace word import, medals, profile definitions, user import, group access masks, the user statistics) only checked that the user is an admin, an admin of the board without the access to the admin page (Admin > Page Access) could use it anyway; "Grant all" of the page access could grant the pages of the host admin with a manipulated form
* [FIXED] Moderation pages (forum, unapproved posts, reported posts) failed with a server error for a forum that does not exist (e.g. /Moderate/Forums/0); the admin pages Edit BB Code, Rank, Access Mask, Category, Group and Medal failed for a negative id (it was not checked like the other ids)
* [FIXED] Active users (page and admin dashboard): the page failed when a visitor was at the albums of a user (/Albums/{id}, the name was expected in the address) or at an album that does not exist (anymore); the album link had no address (the user id was missing) and its title was shown the other way round (the id when a title is set); the name of a profile or albums location was the text of the address (any text could be shown), it is the name of the user now
* [FIXED] Profile: the birthday was converted like a point in time (from the time zone of the user into the time zone of the viewer), viewers west of the user saw the day before; it is shown as a date now (new IDateTimeService.FormatCalendarDate)
* [FIXED] RSS/Atom feeds: the times of the posts were wrong by the time zone of the user (the time of the user was marked with the time zone of the server); the feeds contain utc times now, the feed reader shows them in its time zone
* [FIXED] A plain url (without [url] bb code) in a post, signature or e-mail removed the text before it on its line, and of several urls on a line only the last one was shown (the rules to skip youtube urls matched the whole line)
* [FIXED] Push notifications of new topics and posts showed the topic name html encoded (e.g. "&amp;") and without the replaced bad words
* [FIXED] "Users browsing this forum/topic": users who hide themselves ("hide me") were listed for everyone; the topic list read the flags of the user as the flags of the visit, so approved users were treated as guests (the list was not shown) and users excluded from the captcha were shown as crawlers
* [FIXED] Message history: admins got a "restore" button for the current text of the post (no old version), which always ended on the "invalid" info page
* [FIXED] Admin > Edit Medal: removing a medal of a group asked with a missing translation ("Missing Translation For ADMIN_EDITMEDAL.REMOVE MEDAL FROM THIS USER?" in the log), it asks now "Remove medal from this group?" (new text ADMIN_EDITMEDAL.CONFIRM_REMOVE_GROUP); removing a medal of a user did not ask at all
* [FIXED] Error page: the error message and the stack trace were written as html (an exception message with user input could add html, the generic and async names like Task<IActionResult> were swallowed)
* [FIXED] Security: Install: after the installation the install steps could still be posted by anybody (also guests), this put the whole forum into the install mode (all pages redirected to the install), the steps that create the forum and its admin account could be run again and test e-mails could be sent to any address; the install pages only work while an installation is running now
* [FIXED] Security: Password reset: the reset link logged in users with two factor authentication without the code (the e-mail alone was enough), the code is asked now
* [FIXED] Security: Login with two factor authentication: the "remember me" choice of the login was ignored, the login was always kept for a month (also when it was unchecked, e.g. on a public computer)
* [FIXED] Moderated posts and quick replies: the mentioned and quoted users were never notified, they are notified when a moderator approves the post and for a quick reply like for the reply page
* [FIXED] Security: Notifications (page and popup): the notifications showed the topic names of topics the user can't read anymore (moved to a hidden forum, access removed) and of deleted topics
* [FIXED] New topic: the tags were only saved for approved topics of users with the activity stream enabled
* [FIXED] Moderate > Reported Posts: "View" redirected to any post with its topic name in the link (also of hidden forums), an unknown post failed with an error
* [FIXED] My Topics: a "since" value or a list mode that is not in the list (sent by the client) failed with an error
* [FIXED] The post count of a topic got wrong when a deleted post was deleted again (e.g. the form sent twice) or erased later (Admin > Restore), when a deleted post was moved and when a post or a deleted topic was restored with its replies (counted twice), a topic with a count of 0 was not listed anymore; restoring a post counted the posts of test forums for the author
* [FIXED] New topic / reply: quoting or mentioning a name without user (a guest post, a deleted user, a changed display name) failed with an error after the post was saved, mentioned and quoted users are only notified when they can read the forum
* [FIXED] Security: Polls: the images of the question and the choices (loaded from any site), multiple choices and hidden results were saved from a manipulated form although the settings only allow them for admins (and moderators)
* [FIXED] Profile > Edit Settings: a user could hide from the active users list with a manipulated form although "allow users to hide themselves" is disabled (the default)
* [FIXED] Avatars: with the avatar upload disabled (the default) an avatar could still be uploaded with a post (profile and admin edit user), only the form was hidden
* [FIXED] Albums: with the albums disabled the album images could still be uploaded with a post to the edit album page (only the get was denied)
* [FIXED] Security: Team page and help: the post (forum select of the team page, help search) did not check the "show team to" / "show help to" setting, it showed the team (admins and moderators) to everyone
* [FIXED] Security: Topics page: the post handlers (page size, show list, collapse, mark read) did not check the read access, they showed a hidden forum with its name, description, moderators and users
* [FIXED] Admin start page: a page of the active users list after the last one (e.g. a bookmark when fewer users are online) failed with an error, the "showing x to y" of the unverified users used the numbers of the active users list
* [FIXED] Paged lists: a page number of 0 or less (from the url) failed with an error, a huge page number overflowed to a negative offset (an error on SQL Server and PostgreSQL)
* [FIXED] Register: the user name, e-mail and display name were checked trimmed but saved as typed (a space after the user name gave an "invalid user name" error), the password was saved trimmed while the login uses it as typed (a password with a space at the start or end never worked), the same for the new password of Admin > Edit User > Change Password
* [FIXED] Login box: the user name was not trimmed like on the login page (a space added by autocomplete gave "wrong password"), a missing password (manipulated form) failed with an error
* [FIXED] Security: Team page: the forum select (the forum id is sent by the client) redirected to any forum with its name in the link, also hidden forums and forums of other boards, a forum id that is not a number or unknown failed with an error
* [FIXED] Message history: restoring an older version of a message always showed "Invalid", the time of the version was compared exactly with milliseconds while the database stores it more exact (and was read with the culture of the user); the version is now found by its time as shown on the page (a time range in the query also missed versions now and then)
* [FIXED] Profile > Subscriptions: a notification type that is not a number (manipulated form) failed with an error, a number that is not in the list was saved
* [FIXED] Admin > Users: a "since" filter that is not a number or out of range and a huge number of years for "Lock inactive accounts" (manipulated form) failed with an error
* [FIXED] Security: Topic and forum watch e-mails and the digest e-mails contained the hidden content of posts (hide-reply, hide-thanks, hide-posts, group-hide, ...) as plain text, also for users who do not meet the condition; hidden content is removed from the e-mails now
* [FIXED] Active discussions: guests got the cached list of the page that filled the cache first, the index of a category showed the topics of all categories (or the other way round); the guest cache is kept per category now
* [FIXED] Security: My Topics (watched topics) listed the watched topics of forums the user can not read (anymore, e.g. after a group change or a move to a private forum), with the text of the last post
* [CHANGES] A user can send 5 emails per hour to other users (like "email this topic"), and can't send emails to a user who ignores them (like private messages)
* [FIXED] Requests for unknown album images or attachments wrote an event log entry each time (the event log could be flooded), they now only return "not found"
* [FIXED] "Next topic" / "Previous topic" without a further topic (and "Reply" in a locked topic) showed an error page instead of the message, the topic is now shown again with the message
* [CHANGES] Security: Admin > Run SQL and Admin > ReIndex work on the whole database (all boards), they are host admin only now and can no longer be granted to the admins of a board (also not with a manipulated form)
* [CHANGES] Security: Admin > Restart App stops the application of all boards, it is host admin only now and can no longer be granted to the admins of a board
* [FIXED] Attachments: the allowed file extensions setting is read with spaces and dots ("jpg, .png"), before such an entry never matched and the files were rejected as "Invalid File"
* [FIXED] Avatar upload (profile and admin edit user): an image file larger than the "Avatar size" setting was saved anyway and the warning was shown afterwards (in the profile without the avatar gallery); it is not saved now. The "image was resized" message was lost on the profile page, and the admin page went to the own avatar page of the admin after an upload instead of back to the user
* [FIXED] New topic / edit message: a priority that is not in the list (manipulated form) failed with an error (number too big) or was saved as unknown priority, it is limited to normal, sticky and announcement now; Admin start page: a huge "days old" for deleting the not approved users failed with an error
* [FIXED] Admin > Board Announcement and Admin > Edit User > Suspend: a huge duration failed with an error (date overflow), and an announcement with 0 or less was saved already expired; the duration is limited to 1 unit - 100 years now
* [FIXED] Move topic: the days of the moved topic link were not checked on the server, 0 or less left a link that was already expired, a huge value failed with an error; they are limited to 1 - 36500 days now
* [FIXED] Profile > Edit Settings and Admin > Edit User > Settings: a language that is not in the list (sent with a manipulated form) was saved and every page of the user failed with an error; only languages of the list are saved now. The page size (also of all lists, it is sent with every form) is limited to 1 - 100
* [FIXED] Profile > Delete Account > Suspend: the 30 days were counted from the local time of the user instead of UTC, the suspension ended hours too early or too late
* [FIXED] Admin > Groups (multi boards): the provider roles without a group of the board could be deleted, also when they are the group of another board (the roles are shared by all boards), such roles are no longer deleted
* [FIXED] Admin start page: "Approve all" approved the not approved accounts of all boards including the disabled (deleted) accounts, and "Delete all" deleted the not approved users of all boards; both only work on the listed users of the board now (IAspNetUsersHelper.ApproveAllAsync and DeleteAllUnapprovedAsync have a boardId parameter). Approving a forum user without login account failed with an error
* [FIXED] Admin > Event Log / Spam Log: a date filter that is not a date showed no entries (to date) or all entries (since date), the default is kept now
* [FIXED] Admin > Spam Log: "Delete all" deleted the whole event log (all entries of all types and boards), it deletes only the spam entries of the board now; Admin > Event Log: "Delete all" and the delete of an entry only delete entries of the board (the id is sent by the client)
* [FIXED] Security: Admin > Edit Medal: removing an award of a user or group and the add/edit award dialogs did not check that the medal belongs to the board (an unknown medal failed with an error)
* [FIXED] Admin > Edit User > Avatar: an avatar of the gallery could not be selected (the check was inverted, saving without a selection removed the avatar), and the upload form was nested in another form so the file was never sent
* [FIXED] Security: Admin > Edit User > Avatar and Attachments: the buttons did not check the user id of the form, an admin of a board could change the avatar and list or delete the attachments of any user (other boards, host admins)
* [FIXED] Move message: the topic list offered deleted topics and the links of moved topics as targets, a message moved there was no longer visible; such topics are no longer listed or accepted
* [FIXED] Polls: the image of the poll question was never shown (the image tag had no source)
* [FIXED] Admin > Edit Medal: a medal awarded to a group could not be edited (the group field is disabled in the dialog and was not sent, the save was refused), awarding a medal to a group again failed with a database error, and sort orders above 255 failed with a server error
* [FIXED] Changed or deleted custom profile definitions were still shown in the posts and profiles of the users (cached data), the cache is cleared now when a definition changes
* [FIXED] Admin > Replace Words / Spam Words dialogs: saving without a word failed with a server error instead of the "invalid expression" message
* [FIXED] Admin > Custom Profile Definitions: a new definition without a default value could not be saved (database error, the dialog just reloaded the page); an empty name is refused with a message and an unknown data type is saved as text
* [FIXED] Wrong button texts in admin dialogs: the add button of the replace words and spam words dialogs showed "Add new BBCode" (misspelled language page); the profile definition dialog showed "Edit Profile Definition" and "Save" when a new definition was added
* [FIXED] Custom profile fields: the required fields and number fields were checked with the settings sent by the client (a manipulated form could leave out required fields), the profile page did not check them at all and number fields accepted any text; they are checked against the field definitions of the board now (new text EDIT_PROFILE.NEED_CUSTOM_NUMBER). The length of a custom field was not limited on the register and admin pages (the attribute was written as text), and a length of 0 (no limit) blocked any input on the profile page
* [FIXED] A page could fail with an error (NullReferenceException in LoadPageLazyUserData) when the groups or access masks were changed in another request at the same time: the access rows of the user were cleared after they were checked and the page had no access data; they are created again now. A failure while loading the page data is no longer hidden behind a follow-up error
* [FIXED] SQLite: parallel requests (e.g. the tabs of Admin > Edit User loaded at the same time) could fail with "database is locked" although the connection waits for the busy timeout; such an operation is tried again (up to 3 times)
* [FIXED] Admin > Edit Group: the access masks of the forums were saved by script requests that ran while the page was already left (the browser could cancel them, with many forums the next page often showed and saved again the old masks); they are saved with the form now, only for forums and access masks of the board
* [FIXED] Moderate > Forums: the warnings (nothing selected, no forum chosen, pointer days) showed the topic list without topics, and the topic links contained "UrlRewriteHelper.CleanStringForUrl(...)" as text; Admin > Forums lost the category list with the "can not delete" warning
* [FIXED] Admin > Edit Medal without image showed an error page instead of the warning, Admin > Edit Board lost the culture list with a warning, and Admin > Edit Language showed no texts after saving
* [FIXED] Warnings lost the data of the page: Admin > Edit Category with the name of an existing category lost the image list and the entered values, Admin > Users lost the list of the users when deleting yourself, a guest or an admin, Admin > Access Masks lost the list when deleting a mask in use, Admin > Mail lost the groups when the test mail failed, and the e-mail page (rate limit, empty subject), report post (too long text) and Admin > Delete Forum (another delete running) lost the name of the user, author or forum
* [FIXED] Admin > Edit Access Mask: saving an existing access mask created a new access mask with the changed values, the edited mask was not changed (the id of the mask was not sent with the form)
* [FIXED] Admin > Edit BBCode always showed 1 as execution order (the value was fixed in the page), saving a bb code changed its order to 1
* [FIXED] Admin > Edit Category / Edit Forum / Edit Access Mask: a category, forum or access mask with the sort order 0 (e.g. the "No Access" mask, or after the "sort by name" tools, they start with 0) could not be saved, the field required at least 1
* [FIXED] Post topic / reply as guest: a guest who cleared the name field got an error page (the name of the guest user is used now), and the entered name was replaced by "Guest" when the page was shown again with a warning
* [FIXED] Posting in a moderated forum showed an error page ("Could not find a part of the path '/css/themes/yaf/bootstrap-forum.min.css'") after the post was saved: the notification of the moderators read a style file that the mail template does not use, from a wrong path; the moderators were not notified
* [FIXED] Edit poll: after a warning (no question, less than two choices) the poll could not be saved anymore ("access denied", the id of the poll was kept only for one request), and the poll of a new topic in a moderated forum sent the user to the topic, which is not shown before it is approved ("invalid"), instead of the approval info
* [FIXED] Admin > Edit Forum: a forum with a parent and sub forums could not be saved anymore ("cannot be child forum"), the check now only refuses a parent that is the forum itself or one of its sub forums (ForumRepository.IsOwnSubForum)
* [FIXED] Admin > Edit Forum: every warning (no category, parent loop, name exists) showed an error page instead of the message (the lists of the page were missing), and changing the category of an existing forum reset the selection (the forum was loaded again)
* [FIXED] Admin > Edit Forum: the save button posted to ".../Model.Re/Save" (the route needs a forum id before the handler), it now posts the forum id; the board check covers the forum id of the route and of the form
* [FIXED] Deleting a forum and moving its topics failed when a user had an own access mask in both forums (the user keeps the one of the new forum)
* [FIXED] Some buttons of the admin pages did nothing or showed an error page, their handlers did not exist or were not public: deleting old private messages (Admin > Private Messages), removing points of a user (Admin > Edit User > Reputation), "New access mask" in the context menu of the access masks, and the four sorting tools of Admin > Forums (a unit test now checks all handler names of the pages)
* [FIXED] Marking a post as answer wrote the flags of the post as they were when the topic was opened, e.g. a post deleted by a moderator in the meantime was undeleted; the current flags are used now
* [FIXED] Some host settings could not be changed, the form fields had the wrong type and were never saved: "Show team page to", "Show help page to", "Show share topic to", "Days before posts are locked" and "Disable nofollow on posts older than" (a unit test now compares all fields with the board settings)
* [FIXED] Changing the access mask of a group in a forum did not refresh the cached moderator list (up to 24 hours), so new moderators got no emails about reported or unapproved posts and removed moderators still got them
* [FIXED] The team page listed all forums of a moderator, also forums the user can't read (the names of hidden forums were shown); it now lists only readable forums, and the per user list no longer changes the cached moderator list
* [FIXED] The "load more" requests of the topic list, My Topics, My Account and Notifications, and the paged lists of the editor (attachments, album images) and of the notification menu accepted any page size from the client (also negative or huge values); the page size is now limited to 1-100
* [FIXED] The limit of images per album did not work for an upload of several images at once (the limit was raised with every saved image)
* [FIXED] The page for a new album compared the number of albums with the allowed images per album (instead of the allowed albums), and the album limit message showed the image limit
* [FIXED] Reporting the same post again sent the moderators another email each time (a user could flood their inboxes); the moderators are notified once per user and post, further reports are only counted
* [FIXED] Reporting a post without text showed an error page
* [CHANGES] Security: The password reset page gives the same answer for every entry ("If an account with this user name or email address exists, an email has been sent ..."), it no longer tells whether a user name or email address is registered or whether an account is verified (new text RECOVER_PASSWORD.SUCCESS_IF_EXISTS, removed SUCCESS and ACCOUNT_NOT_APPROVED_VERIFICATION)
* [FIXED] Security: The password reset page (usable by anybody) sent an email for every request, so the inbox of a user could be flooded; an account gets at most 3 reset or verification emails per hour, further requests show the same message
* [FIXED] Sending an email to a user without subject or text showed an error instead of the missing field
* [FIXED] Security (hardening): The event log and the spam log showed the request url of an entry only with html tags removed (an unclosed tag stays), it is now html encoded
* [FIXED] The event log and the spam log only showed the user agent of an entry when it had an url
* [FIXED] Security: XSS with the context menu of posts: the selected text of a post was inserted as html and into "javascript:" links (only quotes, semicolons and brackets were removed, which also changed quoted text like "don't"); the menu items are now created as elements with click handlers
* [FIXED] Security: XSS with the mention list of the editor (@name): display names were inserted as html
* [FIXED] The mention list of the editor showed the users of the previous keystroke (the list for the typed name only appeared after typing another character)
* [FIXED] The "quote selected text" item of the context menu was shown for every post (also deleted posts, locked topics and users without reply access), the condition was rendered as text
* [FIXED] Security: Stored XSS with topic titles: the search results and the similar titles (shown with javascript) used the topic title, description and tags without html encoding, and an unclosed tag (e.g. "<img src=x onerror=..." without ">") is not removed when the topic is saved; the search results are now html encoded (ISearch results: Topic, Description, TopicTags)
* [FIXED] Security: Editing a topic did not remove html from the new subject and description (only new topics did), and the delete message page showed the topic title without encoding
* [FIXED] The similar topics below a topic showed user names with special characters encoded twice (e.g. "Tom &amp; Jerry")
* [FIXED] A changed display name (profile and admin edit user) was not checked like at the registration, so a user could rename themselves to the guest name, a bad word or a name with a semicolon; the rules are now in UserHelper.IsAllowedNameAsync and used everywhere
* [FIXED] Saving the profile with an empty display name showed the error page, and a too short display name showed the "too long" message
* [FIXED] Users with post access but without reply access in a forum (e.g. an access mask that only allows new topics) could still reply by opening the reply page directly; the reply page now needs the reply access, like the reply buttons and the quick reply
* [FIXED] Suspended users were only sent to the info page by the forum pages, but could still use the api (thank posts, vote in polls, give reputation, send friend requests, upload files, ...) and send chat messages; these requests are now denied until the suspension has ended
* [FIXED] Security: Logout was a GET request, so any other website could log a forum user out with a link or an image; the logout dialog now posts a form with the antiforgery token (JavaScriptBlocks.LogOutJs takes the id of the logout form instead of the link)
* [FIXED] Security: Every client could choose its IP address with an X-Forwarded-For or X-Real-IP header, so IP bans could be bypassed, logs and spam checks got a wrong address and the IP check of guest posts (edit/delete) could be fooled
* [FIXED] Admin prune topics: 0 or a negative number of days pruned (or with "permanent delete" removed) all topics of the board or forum; at least one day is kept now
* [FIXED] Help pages: an unknown page in the address (e.g. Help/abc, also by guests) logged a missing translation twice and reloaded the language file for every request, and every help search logged the missing translation HELP_INDEX.SEARCHTITLE; unknown pages show "invalid" now and the search results use the title of the help index
* [FIXED] Admin PM maintenance deleted (and counted) the private messages of all boards, not only of the current board, and 0 or a negative number of days deleted all messages; the label said "Delete unread messages older than", but read messages are deleted as well (new text ADMIN_PM.DELETE_OLDER "Delete messages older than:")
* [FIXED] Security: The admin language editor used the file name of the request (also from the posted form) as path, so it could read and overwrite files outside the languages folder (e.g. "../../appsettings.json"); only existing json files of the languages folder are used now
* [FIXED] Security: Friend requests could be sent directly (without the hidden button) to users who block friend requests, to the guest account, to users of other boards and when the friends feature is disabled
* [FIXED] Texts with quotes or apostrophes (e.g. "O'Brien", profile city and interests, forum and topic names, poll questions) were shown with the html codes ("O&#39;Brien") in many places; Html.HtmlEncode returns html content now, so Razor does not encode it a second time
* [FIXED] Album titles and image captions with special characters (e.g. "Tom & Jerry") were shown html encoded ("Tom &amp;amp; Jerry"), and every save of the album title on the edit page encoded it once more
* [FIXED] The tooltips of the delete and restore buttons of the delete message page were missing in all languages (DELETEMESSAGE.DELETE_TT, UNDELETE_TT)
* [FIXED] SQLite: parallel requests (e.g. the tabs of the admin edit user page) failed with "database is locked" (error page); the database is switched to the WAL journal mode once at the start (unless the connection string sets "Journal Mode") and connections wait up to 5 seconds for a lock (unless the connection string sets "BusyTimeout") and are pooled (unless the connection string sets "Pooling"), so the WAL index is not rebuilt for every new connection
* [FIXED] Security: Polls could be edited without their topic in the address, then a moderator of one forum could edit the polls of every forum; more choices than allowed could be posted, and an invalid expiry value caused an error
* [FIXED] Signatures: the BBCodes allowed by the rank were ignored when the groups of the user allow none
* [FIXED] When the spam check deleted a user (signature, profile homepage, posts), the user stayed logged in and the signature and profile pages still saved the changes
* [FIXED] The text avatar (letters of the name) was cached forever, after a display name change the old letters were shown until the application was restarted; it is cached for one hour now
* [FIXED] Security: The topic lookup of the move/merge dialogs listed the topics of forums of other boards to admins
* [FIXED] Changing the page size of the active users list showed "access denied" to all users without upload rights (by default all members)
* [FIXED] The mention list of the editor searched the users of all boards and returned every match; it now searches the current board, needs 3 characters and returns at most 20 users
* [FIXED] Security: The endpoint of a push notification subscription was stored as sent by the browser, so a member could make the forum send requests to any (also internal) address with every notification (SSRF); only https endpoints of the browser push services are accepted now, existing other endpoints are removed, and guests can not subscribe
* [FIXED] Push notifications stopped when the browser renewed its subscription (the new endpoint was ignored), and expired subscriptions were never removed (an error was logged for every notification)
* [FIXED] Security: The message history showed the text of deleted posts and of posts waiting for approval to every member who can read the forum, and the report post page showed posts waiting for approval
* [FIXED] Security: The print view of a topic printed the text of deleted posts when "show deleted messages to all" is enabled
* [FIXED] The "resend verification e-mail" and "register instead" buttons of the login page did nothing when the user name or password field was empty (the form validation blocked them)
* [FIXED] Confirmation dialogs (e.g. of delete buttons) were only shown when the icon or the text of the button was clicked; a click on its border, the keyboard or the enter key in the form ran the action without asking; on the topic and search pages every confirmation was also shown twice (the script was loaded by two bundles)
* [FIXED] Security: Changing or resetting the password (also by an admin), changing the e-mail address or turning 2FA on or off did not end the other logins of the user, and used password reset links stayed valid until they expired (the user store did not support the ASP.NET Identity security stamp)
* [FIXED] Security: The login page and the login box showed "account not approved" (and allowed resending the verification mail) for any password, so the state of an account could be found out with the user name only
* [FIXED] Security: Accounts disabled by "lock inactive accounts" could still log in and stayed logged in (only the forum flags were changed, not the login account)
* [FIXED] Security: Authors could restore their own posts that were deleted by a moderator (undelete button and delete message page), only moderators can restore them now
* [FIXED] Security: Admin edit user info: a non host admin could make any user (also himself) a host admin by posting the hidden host admin setting, and could set a rank of another board; only host admins can change the setting now and the rank must belong to the board
* [FIXED] Security: Admin edit user points could change the points of any user id (also of other boards), only the edited user can be changed now
* [FIXED] Security: Admin edit user suspend showed the suspend reason as raw HTML
* [FIXED] Security: Kill user (admin edit user) banned the e-mail, IP and name and deleted all posts before checking that the user is not the admin himself, the guest or a host admin; suspend had no checks at all. The checks now come first, apply to delete and suspend, and also protect admins
* [FIXED] Security: Users could set any path as gallery avatar, removing or replacing the avatar then deleted that file on the server (e.g. appsettings.json); only gallery images can be selected and only uploaded avatar files are deleted
* [FIXED] Security: BBCode [url] links could still run javascript: by putting a tab or control character into the URL scheme (stored XSS)
* [FIXED] Security: Groups created when syncing a new ASP.NET Identity role received moderator-level access to every forum instead of the access of the start group (check groups created by role sync on existing installs)
* [FIXED] Security: Quick reply accepted replies to any topic, also without reply access (read-only and hidden forums) and to locked topics and forums
* [FIXED] Security: Poll votes could be repeated without limit, counted for choices of other polls and cast without vote access in the topic's forum
* [FIXED] Security: Removing a poll, voting, marking an answer and "mark all notifications as read" could be triggered by GET requests (CSRF), the login box and file uploads did not validate the antiforgery token
* [FIXED] Security: The topic lookup of the move/merge dialogs listed the topics of every forum to moderators of any forum, now only forums the user is admin or moderator of
* [FIXED] Security: The lockout after 10 failed logins was never enforced (the password was checked before the lockout, and the login page did not save it), so passwords could be guessed without limit
* [FIXED] Security: Display names were not encoded in the notification list, a user could inject script into other users' notifications by mentioning, quoting or thanking them (stored XSS)
* [FIXED] Security: Removing a user from a group, changing a group or changing the access mask of a group in a forum did not refresh the cached access rights, so revoked access (e.g. a removed moderator) kept working for a while
* [FIXED] Security: The posts feed published the text of deleted posts when "show deleted messages to all" is enabled
* [FIXED] Security: A profile home page url with quotes or angle brackets was accepted and shown unencoded in the user hover card (stored XSS)
* [FIXED] Security: Reputation votes only checked the rules of the post page, direct requests could vote for own posts, vote when reputation is disabled, vote down without enough points or when negative reputation is disabled, and vote repeatedly within 24 hours
* [FIXED] Security: Adding or changing the access mask of a single user in a forum (forum moderators dialog) did not refresh the cached access rights, so a revoked access kept working for a while
* [FIXED] Security: Uploaded avatars were served with the content type sent by the browser (e.g. text/html), now only image types are used
* [FIXED] Security: The edit page let authors edit their posts without edit access in the forum, and authors could still edit and delete posts locked by a moderator
* [FIXED] Security: Moderators could remove the polls of forums they do not moderate (the moderator access of the requested forum was checked instead of the poll's forum)
* [FIXED] Security: Theme names were not checked for path characters, a theme like ".." pointed outside the themes folder
* [FIXED] Security: Any user could delete the albums and album images of other users
* [FIXED] Security: Album images were served without checking the profile view permission
* [FIXED] Security: Topic subjects, forum names and starter names (e.g. the name a guest chooses) were not encoded in the digest e-mail, so html like links could be injected into the digest of other users
* [FIXED] Security: Banned IPs were matched as regular expressions, so a ban also blocked other addresses (e.g. 10.0.0.1 also blocked 110.0.0.15, 1.2.3.* also 1.2.30.4) and an invalid ban (e.g. from an import) broke every request; bans are now compared exactly, * matches one part of an IPv4 address
* [FIXED] Security: IP bans were checked against the address of the reverse proxy (e.g. nginx, IIS ARR, Cloudflare) instead of the client address, so banned visitors were not blocked behind a proxy
* [FIXED] Security: Passwords migrated from the legacy membership as clear text were not case-sensitive, and passwords with an unknown format were compared as clear text (so the stored value itself was accepted); unknown formats now always fail
* [FIXED] Security: The admin profile definition dialog could change (and move into the own board) the profile definitions of other boards
* [FIXED] Security: The topic of a page was taken from a board-wide cache when the request had no topic id, so on the single post page lock, unlock, delete topic, reply, move and quick reply acted on whichever topic another user had opened last (and the move topic dialog moved the wrong topic or failed); the cache is removed, all links and dialogs send the topic id
* [FIXED] Security: With the spam handling "delete and ban user", a spam post of a guest deleted the posts of all guests (the guest account was "deleted"); the guest account is never deleted and banned now, the spam post is still rejected
* [FIXED] Security: Notifications shown after a redirect (e.g. login errors, friend requests, moderation results) were stored per IP address, so other users with the same address (office, school, mobile network, proxy) saw them; they are stored in the user's session now
* [FIXED] Security: Moving a topic or a post (also into a new topic, also several topics in the moderator forum view) did not check the target, moderators could move topics and posts into forums they do not moderate or of other boards by changing the request; moving a post into its own topic was not prevented
* [FIXED] Security: The access checks of the moderator pages (moderation, reported and unapproved posts, forum topics and forum users) and of the move post page were never run, every registered user could approve, delete and move posts and topics and give users (also himself) access to forums; moderators of one forum could moderate every forum, and the ids of posts and topics sent with a moderator action were not checked against the moderated forum
* [FIXED] Security: Posts of forums without read access could be read with the report post page and by adding their ids to the multi quote list; deleted and not approved posts of others could be quoted
* [FIXED] Security: Every registered user could restore old versions of any post (message history), and could change polls of other users and choices of other polls
* [FIXED] Security: Users could rename albums of other users and upload images into them
* [FIXED] Security: Deleting the own post also deleted all posts of other users that quoted it (the moderator option to delete linked posts was always on), deleting the own account even erased them; restoring the own post also restored the replies deleted by moderators
* [FIXED] Security: A normal admin could change host admins (password, suspension, groups, profile ...) by posting to the edit user tabs, the user was taken from a cache that all admins shared and that was filled before the host admin check
* [FIXED] Security: Deleting an unapproved user on the admin dashboard deleted any user id that was sent, also admins, host admins and the guest user
* [FIXED] Security: The country and region in the user hover card were not encoded, a crafted country sent with the profile form ran script in the hover card of that user (stored XSS)
* [FIXED] Security: The forum access mask selection of the edit group page did not check the board of the group and the access mask, admins could change the forum access of other boards
* [FIXED] Security: The e-mail topic form sent the posted subject and text to any address without any checks (also for guests, without a topic and with "allow e-mail topic" turned off), the forum could be used to send spam
* [FIXED] Security: Users who watch a topic or forum got new posts by e-mail and in the activity stream even without read access to the forum (e.g. after being removed from a group), the topic page actions (watch topic ...) did not check the read access
* [FIXED] Security: Signatures with forbidden BBCodes were saved anyway (only a warning was shown), and signatures could still be changed when signatures are turned off
* [FIXED] Security: Topic names, display names and guest names were shown unencoded on the board index (last post), in the forum last post column, the active discussions, the notification and activity lists and the print view, any user could inject script into these pages (stored XSS)
* [FIXED] Security: The print view showed the stored post text as html instead of the formatted post, posts could run script on the print page (stored XSS)
* [FIXED] Security: Report texts were shown unencoded in the reported posts list, any user who can report posts could run script in the browser of moderators (stored XSS)
* [FIXED] Security: User names, e-mail addresses, attachment file names and deleted topics and posts were shown unencoded on admin and moderator pages (stored XSS)
* [FIXED] Security: Moderators could remove the user access of a forum (moderate forum page), only admins can manage it; the user access dialog and the medal dialogs did not check that the user, forum, group, medal and access mask belong to the board
* [FIXED] Security: Notification messages (toasts) are shown as html, but the names of friends (friend requests, removing friends), of suspended users and the e-mail on the forgot password page were not encoded, a crafted display name ran script for the other user (stored XSS)
* [FIXED] Security: The registration showed a warning for a display name that is already used but registered the user anyway (other users could be impersonated), the display name is also checked like the user name now (no bad words, not the guest name)
* [FIXED] Security: The two-factor code after the login could be guessed without limit, wrong codes now count as failed logins (lockout after 10 attempts); an expired or replaced login session caused an error
* [FIXED] Security: The edit settings page wrote the two-factor secret of the user into the page, and disabling two-factor checked the code against the key sent with the form instead of the stored key (2FA could be disabled without a valid code); an enabled 2FA can no longer be replaced by a new key without disabling it first; enabling two-factor needs the current password (accounts of external logins without a password are not asked), after a wrong code or password the same setup key is shown again
* [FIXED] Security: The forgot password page showed the full e-mail address of not yet approved accounts to anybody who entered the user name, the address is now masked
* [FIXED] Security: The own e-mail address was changed at once without the password and without confirming the new address, with a taken over session the account could be taken over (change the e-mail, then reset the password); the e-mail address could also be changed when "allow e-mail change" is turned off
* [FIXED] Security: Host admins could delete their own account by posting the delete account form (the check was only done when showing the page)
* [FIXED] Security: The sender of a private message could send any text as avatar url, the recipient's browser inserted it unencoded into the message (XSS); the avatar is now taken from the sender's profile
* [FIXED] Security: Private messages could be sent to users who block private messages or ignore the sender, to users of other boards, to the guest user and with private messages turned off, the chat hub was also open for guests (admins and users in the friend list of the recipient are excluded from the blocking, as described on the block options page)
* [FIXED] Security: The "block e-mails" option of the users was never checked, and the e-mail form sent mails to any user id posted (also with "allow e-mail sending" turned off, to unapproved users and users of other boards)
* [FIXED] Security: A friend request that was not approved counted as friendship for the blocking exceptions (private messages and e-mails), now only users in the friend list of the other user are excluded
* [FIXED] Security: Anybody could change the cover of any album and set images of other albums as cover (the album page did not check the owner when saving)
* [FIXED] Security: The admin pages did not check the board of the records sent by id, an admin of one board could edit or delete categories, forums, groups, ranks, medals, access masks, bb codes, bans, spam and replace words, tags, profile definitions, attachments and deleted topics/posts of other boards, approve their users, send mails to their groups and add users to their groups; "delete all topics without posts" (restore page) deleted the topics of all boards
* [FIXED] Security: Saving the edit post form only checked the edit access of the forum, users could change the posts of other users (also of admins) and posts in locked topics by posting the form
* [FIXED] Security: Saving a new topic or reply did not check the post and reply access, topics and replies could be posted into read-only forums and locked topics and forums by posting the form; the priority (sticky, announcement) and the topic title styles were saved without priority access or moderator rights
* [FIXED] Security: The country of the profile was written unencoded into the flag icon of the profile page, a crafted country sent with the profile form ran script for every visitor of the profile (stored XSS); only known countries and regions are saved now
* [FIXED] Security: The search api did not check the search permission of the board (the results were still limited to readable forums), and the page size of the search was not limited
* [FIXED] Security: The "delete selected" of the own attachments and the "unsubscribe" of the own subscriptions deleted the attachments and subscriptions of any id sent with the form, also of other users
* [FIXED] Security: Quick replies were saved without the preparation of the other post forms (html removed, content of code blocks encoded), markup inside a code block of a quick reply was shown as html to everybody reading the topic (stored XSS); the admin signature editor is prepared the same way now
* [FIXED] Security: The search returned posts that were not approved yet (e.g. spam waiting for moderation) to everybody with read access, now only moderators of the forum find them
* [FIXED] Security: The private messages page opened a conversation with any user id of the address and showed the name, avatar, last visit and online status of the user, also of the guest account, of deleted or not approved users and of users of other boards; now only members of the board can be selected (the messages were already refused)
* [FIXED] Admin > Edit User > Signature never worked: saving failed with an error page (the bb code and length limits of the user were not loaded, an empty field was not handled), the check was inverted (a valid signature was not saved, one with forbidden bb codes was saved without being prepared); it now works like the own signature page and an empty field removes the signature
* [FIXED] Admin > Edit Rank: the number of albums was a text field, a text was saved as 0 without a message (now a number field like the other limits)
* [FIXED] Admin > Edit Board: saving or creating a board without a description failed with an error page, the page showed the language of the current board instead of the edited one (every save of another board changed its language), and a culture that is not in the list set "en-US" as language file
* [FIXED] 18 translations had broken placeholders (e.g. "{0 comments", "{text", "{0} characters}") in Arabic, Finnish, French, Hebrew, Italian, Lithuanian, Polish, Portuguese, Romanian, Slovak and Turkish, showing these texts failed (e.g. the user name length messages of the registration in Polish, the thanks and post messages of the activity in Arabic and Turkish); a unit test now checks the placeholders of all translations
* [FIXED] Saving the admin edit user info removed the guest flag of the guest user (the disabled checkbox is not posted) and the host admin flag when saved by a non host admin; it also reset flags changed in the meantime (e.g. an approved user was unapproved again)
* [FIXED] The admin mass mail and the spam bot notification were also sent to disabled and not approved accounts (and tried empty addresses)
* [FIXED] The spam bot registration notification only went to the first group with "Admin" in its name, a renamed admin group got nothing; it goes to all admin groups now (by the admin flag)
* [FIXED] The team page listed disabled and not approved admins and moderators; disabled moderators also got the moderator notifications
* [FIXED] The moderator notifications (post needs approval, post reported) looked up the moderators by name, which can belong to a user of another board; a moderator without login account stopped the report notifications for all other moderators
* [FIXED] Saving the admin edit user settings wrote back the flags of the cached user, which reset changes made in the meantime (e.g. approved, host admin, moderated), saving the unchanged e-mail with different case or spaces was rejected as already used and an empty e-mail caused an error
* [FIXED] The "resend verification e-mail" button on the login page did nothing (the handler was not public)
* [FIXED] Admin edit user approve button showed an empty page instead of returning to the user, and approved any user id
* [FIXED] Admin edit user suspend: minutes were added as hours, the end time was calculated in the time zone of the user but stored as UTC (wrong suspension length), zero or negative values ended the suspension at once, and host admins could be suspended
* [FIXED] The topic search of the move/merge dialog removed the current topic from the cached topic list, so that topic was missing from later searches
* [FIXED] Approving a friend request of an unknown user caused an error and the "approved" message was also shown without a request, the friend list actions worked with the friend list turned off
* [FIXED] Saving an empty signature caused an error
* [FIXED] Report texts with a question mark were cut off at the first question mark in the reported posts list
* [FIXED] Texts with quotes broke the page scripts: the "link back to topic" button did nothing in Danish, French, Spanish and Turkish, a board name with quotes or notification messages with double quotes broke the notifications; all texts in the generated scripts are now encoded
* [FIXED] The GeoIP country lookup of parallel requests used the same data stream, under load users could get the wrong country (banned countries, active users) or the page load failed
* [FIXED] The "watch topic" option of the quick reply was ignored for admins and moderators
* [FIXED] The StopForumSpam and BotScout bot checks sent the user name, e-mail and ip without url encoding (e.g. a + in an e-mail address became a space, an & in a name cut off the request), a StopForumSpam response without a user name caused an error
* [FIXED] The forgot password page said that a new verification e-mail was sent to not yet approved accounts, but none was sent; it is sent now (only while the e-mail verification is pending, accounts waiting for the approval of an admin get no e-mail)
* [FIXED] The sub menu of the user menu (My Settings) always opened to the right and was cut off at the right edge of the screen, it now opens on the side with enough space (below the item on small screens)
* [FIXED] A private message to a user who had another conversation open on the messages page was marked as read without being shown, it is now only delivered to the open conversation with the sender (also in several tabs) and otherwise stays unread
* [FIXED] Creating a category or forum failed when a category or forum with the same name existed on another board, renaming a category or forum to a name that already exists (in the board or the category) caused an error instead of the "name exists" message
* [FIXED] Editing a post of a sticky topic or announcement without priority access reset the topic to a normal topic
* [FIXED] A too long guest name was replaced by the part after the first 100 characters instead of being cut to 100 characters
* [FIXED] Deleting attachments (own attachments, admin attachments list and edit user) only deleted the database entry, the files of attachments stored in the file system stayed on the disk
* [FIXED] Exporting the selected bb codes failed when an id was not found
* [FIXED] After deleting own attachments the next attachments in the list were shown as selected, the page is now reloaded without a selection
* [FIXED] After posting a new topic or reply (also quick reply and edit) in a moderated forum the "your post has to be approved" info page could not be opened (the link was created with a wrong parameter)
* [FIXED] The message parameter of the info page was shown as html (links with markup are already rejected by the AntiXssMiddleware, the message is now also encoded)
* [FIXED] The AntiXssMiddleware answered with status 400 but reported the error code 500 in the response
* [FIXED] [size=9] was shown as 9px instead of the largest size (180%), sizes above 9 (percent) were not limited (now at most 400%)
* [FIXED] Uploaded avatars are always converted to webp, but they were stored with the content type of the uploaded file
* [FIXED] The custom profile fields (register, edit profile, admin edit user) used the field id, data type and length sent with the form, values could be saved for fields of other boards and longer than the defined length; the fields of the board are now taken from the database
* [FIXED] Approving or restoring a post did not update the search index, restored posts were only found again after rebuilding the index
* [FIXED] Editing a reply (by a user who did not start the topic) removed all tags of the topic, the tags are only changed when they are shown in the form (topic starter or moderator)
* [FIXED] Editing a post as topic starter removed the topic title styles set by a moderator
* [FIXED] "Lock inactive accounts" (admin users page) also locked host admins, admins and the admin running it, and a value of 0 or less years locked (almost) all users; admins are never locked now and at least one year is used
* [FIXED] Disabled (soft deleted) users were shown without the "disabled" badge in the admin users list
* [FIXED] Spam bot auto-bans saved the bot's name and e-mail as unanchored regular expressions, so later registrations containing that name (e.g. "Joanna" after banning "ann") were treated as bots and banned too (review the banned names and e-mails created by the spam system on existing installs)
* [FIXED] The registration bot check ignored wildcard IP bans (e.g. 192.168.1.*), and an invalid banned name mask cancelled a match on a banned e-mail or IP
* [FIXED] Album image previews and album covers were also served for files with a text content type
* [FIXED] Removing a thanks deleted the thanks notifications of all other users for that post, thanks could be given for own and unreadable posts
* [FIXED] Resized image attachments were stored with the content type of the original file and could not be downloaded
* [FIXED] Renaming an album or an image caption with an unknown id caused an error, the random album cover never showed the first image
* [FIXED] Comparing two revisions of an edited post on the message history page crashed
* [FIXED] "Go to post" links opened the wrong page in topics with more than two pages
* [FIXED] Restoring a deleted post did not give the author the post count back
* [FIXED] Editing an unapproved post in an unmoderated forum (or with "override approval") did not approve it
* [FIXED] The pager on My Topics -> Watched always showed 0 topics
* [FIXED] Deleting a forum cleared the last post information of an unrelated topic in another forum
* [FIXED] Users were never promoted along a rank ladder
* [FIXED] Reputation votes were not recorded, so the "once every 24 hours" voting limit was never enforced
* [FIXED] "Most active users" showed arbitrary users instead of the top posters
* [FIXED] The "quoted" notification counter showed the number of received thanks
* [FIXED] "Delete all" (event log, spam log, active users) did not clear the cached data when more than one row was deleted
* [FIXED] Nested quotes with a user name were rendered in the wrong order with swapped attributions
* [FIXED] Editing or quoting older HTML posts lost list items, kept formatting inside links as raw HTML, lost the type of notes and turned paragraph ends into flag emojis ([br])
* [FIXED] The order of BBCode rules with the same rank was not deterministic
* [FIXED] "Disable nofollow links after x days" had no effect
* [FIXED] RSS feeds of posts containing a spoiler only showed the spoiler text
* [FIXED] Moderator approval notifications were sent fire-and-forget, so mail errors were lost
* [FIXED] Topic watch notifications could be sent in the wrong language when push notifications are enabled
* [FIXED] Generated passwords never used the first and last character of the allowed set (and could hang when spaces were included)
* [FIXED] FlagsBase.EnumToIndex returned wrong bit indexes, StringValueAttribute could not be applied to enum values
* [FIXED] Admin -> Prune did not delete any topics (it only counted them) and ignored "permanently delete"
* [FIXED] Creating a new board changed the language and culture of all boards without their own language setting
* [FIXED] "Approve all" stopped at the first account without a forum user and left the remaining users unapproved
* [FIXED] Missing translations were shown as empty text, translations without a text did not fall back to English
* [FIXED] The board statistics counted deleted posts and topics (and the admin statistics posts in deleted topics)
* [FIXED] The event log was never trimmed to the maximum number of entries, only by age
* [FIXED] The message history page crashed for posts that were never edited and for unknown messages
* [FIXED] The "reported n times" counter of reported posts was one too low
* [FIXED] Active users: page 2 repeated page 1, "Show crawlers in active list" had no effect and the crawler filter used the wrong flag
* [FIXED] Visitors with the same browser as an active crawler were not listed as online, crawlers from a second IP were not recorded
* [FIXED] Adding a user to a role and some identity store operations (remove login/claim, set password hash) did not wait for the database, errors were lost
* [FIXED] User claims were stored with the value type instead of the claim type and could not be removed
* [FIXED] A failed cache load (e.g. a short database outage) stayed in the cache, so the error was repeated until the cache was cleared
* [FIXED] Read tracking in the session (guests, or with database read tracking off) never found read forums and topics, the "last read" time ignored read topics
* [FIXED] Creating the forum user on the first login was not awaited and always reported success
* [FIXED] Admin -> Groups: deleting a role was not awaited, deleting or listing an unknown role caused an error
* [FIXED] The anti-XSS middleware read the whole body of file uploads into memory although it does not check them
* [FIXED] Editing a banned IP, name or e-mail did not refresh the cached ban list, the old mask stayed banned until the application restarted
* [FIXED] Isolated events (RaiseIsolated / RaiseIsolatedAsync) were handled twice, and an exception in a handler was not isolated
* [FIXED] Users could send a friend request to themselves
* [FIXED] A notification of a deleted topic broke the whole notification list
* [FIXED] The latest posts feed added the time zone offset twice to its update time
* [FIXED] Event log entries logged with a Type as source showed "System.RuntimeType"
* [FIXED] The guest user was suggested in @mentions
* [FIXED] The category list in Admin -> Edit Forum was not sorted
* [FIXED] One invalid banned e-mail mask disabled all banned e-mail checks, banned e-mail and name masks are now validated as regular expressions
* [FIXED] Admin -> Spam Words: an invalid regular expression was not reported
* [FIXED] Awarding a medal a user already has caused an error
* [FIXED] Users could add themselves to their ignore list
* [FIXED] An invalid banned user agent pattern broke every request of the forum, banned user agents are now validated as regular expressions
* [FIXED] Editing a banned user agent did not refresh the cached list
* [FIXED] The digest was not sent to any board when the first board was not due, and not to anyone when one user had no provider key
* [FIXED] The "days before posts are locked" period started again with every edit, it now starts when the post was written
* [FIXED] The default robots.txt (without a robots.txt file) was invalid
* [FIXED] Admin -> Statistics failed for languages with month names shorter than three characters (e.g. Chinese)
* [FIXED] Deleting a forum and moving its topics to another forum did not wait for a running forum delete
* [FIXED] Deleting an album stopped at the first image without a file (e.g. images stored in the database) and left the album and its other images, deleting an unknown image caused an error
* [FIXED] A collapsible panel cookie with an invalid value broke every page that shows the panel
* [FIXED] Medal names and messages with quotes broke the medal ribbon html
* [FIXED] Links and breadcrumbs with a missing name (e.g. of a deleted user) caused an error
* [FIXED] The breadcrumb of a forum nested more than one level deep did not show all parent forums
* [FIXED] The digest period was calculated in server time instead of UTC, so topics were missing or repeated on servers outside UTC
* [FIXED] Avatar paths starting with // loaded the avatar from another site
* [FIXED] The legacy setting "enabletopicdescription" was not removed on case sensitive databases
* [FIXED] Reading a notification could leave the unread notification count too high (the cache was cleared before the update)
* [FIXED] The search index update stored its last run in server time but compared it with UTC post times, so posts were missing from the search on servers east of UTC
* [FIXED] The ban imports accepted invalid entries, IP imports now only accept IP addresses and masks, name and user agent imports only valid regular expressions
* [FIXED] Admin -> Database: "Change recovery mode" re-indexed the database instead of changing the recovery mode
* [FIXED] SQLite: the table and column checks used by the database migrations never found existing tables and columns
* [FIXED] One custom BBCode with an invalid search expression broke the display of all posts, and a custom BBCode whose module class is missing broke the posts using it; such BBCodes are now skipped and logged, the BBCode editor and import validate the search expression
* [FIXED] Topic list age filter: "Last Year" was missing from the options and every option showed the topics of the next longer period ("Last Day" showed two days, "Last Six Months" a year); a year now has 365 days
* [FIXED] The languages page threw an error when a language file was missing pages or texts of the english file; missing texts now count as not translated
* [FIXED] Requests without a remote address (e.g. Kestrel on a Unix socket) failed as soon as an IP ban existed
* [FIXED] Admin -> Banned IPs: an invalid address reloaded the page instead of showing a message in the dialog, IPv6 addresses could not be banned (the import already allowed them) and an empty address caused an error
* [FIXED] Users migrated from the ASP.NET membership (aspnet_Membership) with hashed passwords could not log in, only hashes of the old YAF membership provider were checked; both are now accepted
* [FIXED] Login failed with an error for migrated clear text passwords containing a | or an invalid password format
* [FIXED] Admin -> Users -> Import: csv files sent as text/csv and xml files sent as application/xml were rejected as wrong format
* [FIXED] A database access set on the database provider was discarded at once and the configured provider used instead
* [FIXED] Search: editing a post removed its topic tags from the search index, so the topic was no longer found by its tags
* [FIXED] Search: indexing a post without topic tags failed (null value), and searching with the same search service after it wrote to the index failed because the closed index writer was reused
* [FIXED] Search: results were sorted by message id as text ("9" before "10"), so with more hits than "Return Search Max" the newest posts were cut off
* [FIXED] Search: SearchDefaultAsync skipped the first 1000 results (the page index started at 1)
* [FIXED] Unticking the multi-quote button of a post did not remove the post from the multi-quote list
* [FIXED] Logged-in users without any forum access got an error page instead of falling back to the guest permissions
* [FIXED] The first page of a new session was not recorded in the active users list (forum page was empty)
* [FIXED] MemoryCache GetOrSet failed for value types when the key was missing
* [FIXED] Pages with an invalid id in the url (e.g. ?f=abc or a too large number) showed an error page, invalid values are now ignored
* [FIXED] Push notification (un)subscribe requests were accepted when push notifications are not configured, and devices with special characters in the user agent could not be unsubscribed
* [FIXED] SimpleReplaceRule with an empty search text never finished
* [FIXED] Admin -> Users -> Import: csv files with an empty last line, rows with fewer values than columns or rows without name or e-mail failed the whole import, quoted values kept their quotes, a file without Name/Email column showed a technical error instead of the format message, and import errors showed the message of the banned names import
* [FIXED] Login and login box failed with an error for a not approved account without a forum user
* [FIXED] Redirects outside a page or controller (e.g. from a middleware) lost the page parameters (e.g. the info message)
* [FIXED] Background tasks waited their start delay as seconds between the runs instead of their run period, so expired suspensions were lifted, expired topic links removed and digests sent only about every 8 hours (instead of hourly / every 5 minutes); a failing run no longer stops the task for good, and replaced or disposed tasks stop running
* [FIXED] Admin -> Users -> Import: the time zone of the exported users was lost (the import expected a number instead of the time zone name)
* [FIXED] Deleting a user (admin or spam check) did not delete the albums and album files of the user (album ids were used as image ids)
* [FIXED] Album images were always sent as image/png, now with their own image type (other types, e.g. svg, still as png)
* [FIXED] Account pages (login, register, reset password, ...) did not stop after running their handler: the page checks continued and ran the handler a second time (or redirected a login with "Require Login" back to the login page)
* [FIXED] Emojis with a skin tone (e.g. [raised_hand_tone1], [basketball_player_tone1]) only showed the skin tone color, the emoji itself was missing
* [FIXED] IAspNetUsersHelper.ImportAllMembershipUsers did not wait for the import (fire and forget with request services), it is ImportAllMembershipUsersAsync now
* [FIXED] Times shown to guests ended with [TIMEZONES.NAME_UTC] (the text was missing, every page logged a missing translation); guests now see the offset of the time zone, e.g. " (UTC+02:00)"
* [FIXED] The missing texts EDIT_AVATAR.AVATARMODEL (avatar page) and COMMON.ALBUMIMG_CODE (editor album image button) were added
* [FIXED] Moderate -> Forum: moving the selected topics with "Leave Pointer" was always rejected as invalid number of days
* [FIXED] Missing texts were added: members list letter filter, the info tooltips of Admin -> Edit User -> User Info, the subject of the mail when an admin changes a password, and the message for moving topics to a category
* [FIXED] Edit Profile, Edit Settings and Admin -> Edit User failed with an error for users whose language file does not exist (anymore), the default culture is used then
* [FIXED] Admin -> Banned Countries: changing an entry to a country that is already banned created a duplicate
* [FIXED] Code blocks with highlighted lines (e.g. [code=cs;2-3]) did not use the language name mapping (cs -> csharp, xml -> markup), the language and the lines of a code block only keep allowed characters in the html
* [FIXED] Text avatars were broken for names with "&" (invalid svg), names with several spaces or of emoji only, and users without provider key (no avatar)
* [FIXED] Errors of the banned IP, name and user agent imports were logged as banned e-mail import
* [FIXED] A language file of a user or of the board that does not exist (anymore) broke every page (Invalid language file), the board language or english is used instead
* [FIXED] Deleting a forum reported success but did nothing when the previous forum delete task was finished but not yet cleaned up
* [FIXED] Message history: with the history of two messages open in two tabs, "Compare versions" in the first tab showed the editors, times, edit reasons and ips of the versions of the other message (they were taken from the session, which only holds the list of the last opened history), with fewer versions in the other message the page failed with an error; the versions are loaded again for the compare and the texts are not sent with the form anymore
* [FIXED] Delete post: a member could open the delete page of the own post of a locked topic (or after the lock period), deleting then just showed the page again without the post; the page and its handlers are denied now with the same rules as the delete and restore buttons of the post
* [FIXED] Admin -> Groups: the guest group could be deleted with a manipulated form (the list has no delete button for it)
* [FIXED] Admin -> Digest: after "Force Digest Send" the "Last Digest Send" field was empty until the page was opened again
* [FIXED] Registration and e-mail change failed with an error on SQL Server, MySQL and PostgreSQL (not on Sqlite): the key of the verification link (an identity token, longer since it contains the security stamp of the user) was stored in a column of 255 characters. Only a hash of the key is stored now (links sent before the update still work); "resend the verification mail" (login page and admin dashboard) sends a new link, on the admin dashboard it could send the mail of another user whose name contains the name of the not approved user
* [FIXED] Album titles, image captions and the reasons of edited and deleted posts are stored html encoded, the encoded text (e.g. "&" -> "&amp;", "'" -> "&#39;", "ä" -> "&#228;") could be longer than the column: saving failed with an error on SQL Server, MySQL and PostgreSQL (e.g. a delete reason with umlauts, an image caption with "&"); the text is shortened to fit now. A new album stores its title html encoded like a changed title
* [FIXED] The reasons of edited and deleted posts were shown html encoded (e.g. "Tom&amp;#39;s" instead of "Tom's") in the post and in the message history: they are stored encoded and were encoded again for the output
* [FIXED] Saving a name or text longer than its column failed with an error on SQL Server, MySQL and PostgreSQL (not on Sqlite), e.g. the name of an access mask (50 characters): the inputs of the admin pages (access mask, rank, group, forum, board, bb code, register user, banned e-mail and name, replace and spam words), of the poll and of "move message" are limited to the length of the column now
* [FIXED] PostgreSQL: the conversations of the private messages were not loaded and could not be deleted, deleting topics in the moderation of a forum failed: the column names of a few sql statements were not quoted
* [FIXED] Rarely a page failed with "Object reference not set to an instance of an object" (ForumPage): a background task started by a request (search index, digest, clean up) ran with the context of that request and could replace the board context of the page while it was loaded; the tasks run without the context of the request now
* [FIXED] Rarely the first page of a new session failed with "Failure Initializing User/Page" (unique key of the active session): a parallel request of the same session saved the session after it was checked, the session is updated then now
* [FIXED] MySQL: editing a post a second time in the same second failed with "Duplicate entry" (MySQL saves the time of a version in the message history in whole seconds, the time is part of its key): the new version gets the next second
* [FIXED] PostgreSQL, SQL Server, MySQL: a visitor coming from a page with a long address (referer longer than 500 characters) got the error page "Failure Initializing User/Page" on every forum page instead of the forum: the referer, path, browser and platform saved in the active users list are cut to the length of their columns
* [FIXED] MySQL, SQL Server: browsers with a long user agent (e.g. the in-app browsers of Facebook or Instagram) could not subscribe to push notifications (MySQL creates the device and user agent columns with 255 characters, the device is part of the key): the encoded user agent is cut to 255 characters
* [FIXED] MySQL: a custom profile field with a name or default value longer than 255 characters was not saved (Admin > Profile Definitions), the inputs are limited to the length of their columns
* [FIXED] MySQL: push notifications could not be subscribed with browsers whose push service sends an endpoint longer than 255 characters (e.g. Firefox), the endpoint column is unlimited text now (database version 1005, the upgrade changes the column on MySQL and SQL Server)
* [FIXED] MySQL: a topic tag longer than 255 characters showed the error page "Data too long for column 'TagName'" although the topic was already created (sending it again created the topic twice): the tag name column is unlimited text now (database version 1005)
* [FIXED] MySQL: the database upgrade could not read the length or the type of a column (the lookup searched the schema "dbo", the schema of a MySQL table is its database), a column change depending on it was always or never made
* [FIXED] Admin > BB Codes: a code selected on page 2 (or a later page) exported the code at the same place on page 1, and the selection of a page was shown on the next page; the same selection was taken to the next page of the topics of Moderate > Forums (delete or move of topics that were not selected): the pages are changed by a post of the form, the list showed the posted ids and selection instead of the loaded page
* [FIXED] Admin > Users > Import: a user refused by the login system (e.g. a name with characters that are not allowed, an invalid e-mail) was created as forum user without login and got the registration mail with a password that did not work; such users are skipped now (the reason is logged)
* [FIXED] PostgreSQL: a difference in minutes was only the minutes part of the time (2:05 hours were 5 minutes): users who left the forum were shown in the active users list until the minutes part was larger than the active time, and the online time of the active users list was wrong
* [FIXED] Installation: the search index update (started by the first page) could run before the board was created and failed ("Error In UpdateSearchIndexTask", the settings were saved for the board 0); it waits for the board now
* [FIXED] MySQL: the current time of the database queries was the date without the time (UTC_DATE(), midnight): users who left the forum were shown in the active users list until the end of the day (UTC), the days of the prune of topics, private messages and the message history were counted up to midnight
* [FIXED] My Account > Attachments: the pages of the own attachments could not be changed (the pager was outside of the form) and the page size was not kept
* [FIXED] My Account > Attachments and Subscriptions: the selection of a page was taken to the next page and the selected attachment or subscription of page 2 deleted the one at the same place of page 1 (like the bb code export and Moderate > Forums)
* [FIXED] Active Users: the pages of the list could not be changed (the pager was outside of the form) and the page size was not kept
* [FIXED] Albums: an image with the same file name as another image of the album (e.g. phones name every photo "image.jpg") overwrote the file of the other image, both showed the new image and deleting one of them removed the file of the other; the name gets a number now ("image (2).jpg")
* [FIXED] After a button of a page that shows the page again (e.g. "show posts" of an ignored user, a refused 2FA activation, "send the verification mail again", an album upload), the page stayed at the address of that action and the main button of the page posted to it again: the block options and the settings were not saved, the login sent the verification mail again instead of logging in, "Delete Album" did not delete a new album
* [FIXED] The push notifications of watched topics and forums had the subject in the language of the user, but the text ("... replied to the topic ...") and the buttons in the language of the poster
* [FIXED] Mails sent together (digest, notifications of watched topics and forums): a mail the mail server refused (e.g. an unknown recipient address) stopped all mails after it
* [FIXED] Digest with several boards in one database: the digest task of every application sent the digests of all boards, but with the topics, name and links of its own board (an application serves the board of its configuration), so the users of the other boards got no digest when another application was first; every application sends the digest of its own board now
* [FIXED] Upgrade (Migration1000): the old columns of the private message limits (ranks and groups) were never removed (the default constraint was looked up on the users table), and on MySQL, PostgreSQL and Sqlite also not the old notification setting of the users (a column was only removed with a default constraint, which only SQL Server has); existing databases keep the unused columns
* [FIXED] The mails about added or removed groups and about the suspension of an account (sent when an admin changes the user) were in the language of the admin, they are in the language of the user now (like the medal and private message mails)
* [FIXED] Security: The host pages (host settings, boards, page access, run sql, reindex, languages, restart, test data) can not be granted to the admins of a board, but an access stored before (e.g. by an older version) still opened them (Host Settings only checked the host admin when it was opened, not when it was saved); they are refused for everybody except the host admin now
* [FIXED] Security: Install: when the installation was not completed (the finish button of the last step was not clicked) every page was sent to the install steps and anyone could run them again on the installed forum, also the step that creates an admin; on an installed forum only the last step can be used now, a not completed installation is ended
* [FIXED] Security: The selects of the group medal dialog and the forum user access dialog showed the names of the groups and access masks as html, a name with html ran script in these dialogs; the names are text now
* [FIXED] Security: Search: the search index kept the forum of a moved topic or post (the read access is checked with it), a topic moved to a forum the users can't read (e.g. a staff forum) was still found with its text; the index is updated when a topic or post is moved
* [FIXED] Security: Attachments: any attachment could be downloaded by counting up its id, and the id of any attachment could be shown in an own post; the download links are signed now (only links shown by the forum work, the download permission of the groups still applies) and a post only shows the attachments of its author
* [FIXED] Notification mails to users without an own language (medals, private messages, reported and unapproved posts for moderators, groups, suspension) were in the language of the sender (e.g. the admin), they are in the language of the board now
* [FIXED] Topic tags: a tag entered twice (the tag field allows the same tag again) or with spaces around it was saved as several tags, an existing tag entered twice failed the save of the post (primary key of the topic tags); every tag is trimmed and added once now
* [FIXED] Edit profile and Admin > Edit User > Profile: a real name, location, city, homepage or blog longer than 255 characters (the homepage with the added "https://") failed the save with an error on SQL Server, MySQL and PostgreSQL; the fields have a maximum length and too long values are refused with a warning
* [FIXED] User import (csv): profile values longer than their columns or a country name instead of the country code (the column has 2 characters) failed the import with an error on SQL Server, MySQL and PostgreSQL; the texts are shortened to the columns and only known country and region codes are taken
* [FIXED] Reporting a post: every report of the same user and post is added to the report text, after some reports (or with a higher maximum report length) it was longer than its column (4000 characters) and the report failed with an error on SQL Server, MySQL and PostgreSQL; the report text is unlimited now (Migration1005)
* [FIXED] Album images: when several files were uploaded at once only the files up to the first image were checked, the files after it were saved whatever their type
* [FIXED] User name styles were only set, never removed: a user taken out of a styled group, the members of a deleted group and the users of a group or rank whose style was removed kept the old style; a user who got another rank (promoted or changed by the admin) did not get the style of the new rank
* [FIXED] Admin Ranks: deleting a rank that users have failed with an error on SQL Server, MySQL and PostgreSQL (on SQLite the users kept a deleted rank); a rank in use can't be deleted, like an access mask in use
* [FIXED] Admin Edit Rank: the signature BBCodes field was a number field, BBCodes (e.g. "B,I") could not be entered and saving a rank removed its BBCodes
* [FIXED] Admin Edit Forum: a forum moved to another category left its sub forums in the old category (on their page the old category was shown above the moved parent forum, and the admin forum list showed them in the old category); the sub forums are moved with the forum
* [FIXED] Admin Forums: copying a forum did not create a new forum, the save changed the copied forum to the values of the copy
* [FIXED] Admin Edit Group: a new or renamed group with the name of an existing group failed with an error (the name is unique in the board); it is refused with a warning
* [FIXED] Admin Edit Rank: a new or renamed rank with the name of an existing rank failed with an error (the name is unique in the board); it is refused with a warning
* [FIXED] Topic "Watch this topic": watching an already watched topic again (e.g. from a second tab) failed with an error (the subscription is unique), and guests could watch topics with a sent form; a topic is watched once and guests are refused
* [FIXED] New topic: when topics with the same subject are not allowed, a subject was refused when any topic contained it ("Help" was refused because of "Need help with X") and deleted topics counted; only the same subject (ignoring the case) of a not deleted topic is refused now (new text POSTMESSAGE.SUBJECT_EXISTS instead of SUBJECT_DUPLICATE)
* [FIXED] Board index "Watch all forums" watched nothing (the forums were filtered by an empty category) and only looked at the forums loaded so far; it now watches all forums the user can read (of the shown category), and guests are refused
* [FIXED] Edit Message: the topic subject could be changed to the subject of another topic (the duplicate subject check never ran on edit); a changed subject is checked like for a new topic, the topic itself does not count
* [FIXED] Custom BBCodes: a new or changed custom BBCode was not used in the posts until the application restarted (the cached rule engines, which contain the custom BBCodes, were not cleared); only these and the BBCode lists are cleared now, not the whole cache (that broke the installation)
* [FIXED] Profile Edit Settings: the theme and time zone were saved as sent (a manipulated theme longer than the column failed the save on PostgreSQL/SQL Server/MySQL, a theme was saved even when users can not choose one); only themes and time zones of the lists are saved
* [FIXED] Board index "Mark all forums read" marked only the forums loaded so far (the first 20, the others are loaded with "show more"); it now marks all forums the user can read (of the shown category)
* [FIXED] Admin Edit Medal: a medal without a message (the field is optional) could not be saved (error page, the column is not nullable); a medal without a message is shown with its name only
* [FIXED] Admin Medals: a medal could not be deleted (every post of the page failed with an error, the shown medal list was bound and can not be created)
* [FIXED] Medals: a medal awarded to a group, a changed medal and a deleted medal were shown in the old way for up to 10 minutes (the cached medals of the users were not cleared)
* [FIXED] Admin Forums: a deleted category was still shown in the list (the list of the page is kept in the session and was not reloaded)
* [FIXED] Message history: the button back to the reported posts was never shown (the forum id is in the query string, not in the route)
* [FIXED] Host settings: after enabling private messages, friends or albums the counts of the users (unread messages, friend requests, "My albums") were missing until the cached user data expired
* [FIXED] Admin database maintenance: "Re-index" failed with an error page on MySQL (and on errors of the database), now a message is shown
* [FIXED] Admin database maintenance: "Shrink" on MySQL showed a database error instead of "not supported"
* [FIXED] Guests (all are the same user) could edit and delete every guest post and add or edit the polls of every guest topic by the address of the page, now only the guest posts of the own address (like the buttons of the post)
* [FIXED] Guests (all are the same user) saw the uploads of all other guests in the attachment list of the editor, the list is no longer shown to guests
* [FIXED] Admin database maintenance: "Re-index" and "Shrink" on MySQL run OPTIMIZE TABLE for the forum tables (they were not supported)
* [FIXED] Guests (all are the same user) shared one flood delay: the post of one guest blocked all other guests, the delay of guests is now checked per address
* [FIXED] Members: the filter by number of posts ignored the selected comparison (equal, less or equal), it always used "more or equal"
* [FIXED] Mail: a refused single mail (test mail of the admin and install pages, mail to a user, test digest) was reported as sent, the error is shown again when no mail was sent
* [FIXED] Admin mail to a group: a mail that could not be sent showed an error page and stopped the mails to the other members, all mails are sent in one connection now and an error is shown as message
* [FIXED] Notification mails (watched topics, approval, medals, roles, suspension, admin notices) failed the action with an error page when the mail could not be sent (e.g. the mail server is down): the post was saved, but the mentions and the activity stream were skipped. The error is logged now
* [FIXED] Unapproved posts: approving an already approved post (another moderator, the button sent twice) counted the post again for the user and sent the notifications again
* [FIXED] Register: when the verification mail can not be sent the account is created anyway, the page now tells it and points to "Resend Email Verification" on the login instead of showing an error page
* [FIXED] Admin Register User: when the verification mail could not be sent the user was created, but the default notification settings were not saved and an error page was shown, the settings are saved first now and the mail error is shown as message
* [FIXED] Delete user: the medals and the push subscriptions of the user were not deleted, on PostgreSQL, SQL Server and MySQL the delete of a user with a medal or a push subscription failed (after the login of the user was already deleted). Ignore entries of other users for the deleted user are removed too
* [FIXED] Delete board: the tags, profile definitions, banned countries, banned user agents and push subscriptions of the board were not deleted, on PostgreSQL, SQL Server and MySQL the delete of such a board failed half way
* [FIXED] Admin Boards: the board the forum runs on could be deleted (the context menu always had the delete), the forum was broken afterwards. The delete is not shown for the current board and refused
* [FIXED] Group medals: after a user joined or left a group (or the group was deleted) the medals of the group were still shown or missing for up to 10 minutes
* [FIXED] Search: the number of results counted the hits the user can not see (forums without read access, not approved or deleted posts), the pager showed empty pages and a search in a hidden forum told the number of hits. Only the shown page is formatted now
* [FIXED] Localization: the language file (and the english file for the fallback) was read and parsed again for every request, the cache was overwritten right away. The parsed file is cached now until the file is changed (e.g. by the language editor)
* [FIXED] Logout: the push notification subscription of the browser is removed for the user, the next user of the browser got the notifications of both users
* [FIXED] Admin Edit Medal: after an award was removed from a group or user the other awards of the medal were not listed anymore
* [FIXED] Block options: the options of turned off features (private messages, buddy list, e-mails) are not shown, saving the page reset them (e.g. the blocked e-mails were sent again after the admin turned the e-mails on again); they keep their value now
* [FIXED] Edit settings: when the users can't choose a theme or a language (or hide themselves) the options are not shown, saving the settings reset the theme and the language of the user to the board defaults (and showed the hidden user again); they keep their values now
* [FIXED] Subscriptions: the daily digest option was always shown unchecked, saving the notification settings turned the digest off; and while the digest is turned off (the option is not shown) saving reset it. The option shows the saved value and keeps it now
* [FIXED] Admin Edit User settings: when the users can't change their e-mail address the field is not shown, the empty address was refused and the settings could not be saved; when the users can't choose a theme or a language saving reset them. The hidden values are kept now
* [FIXED] Admin Edit User: the "moderated" setting of a user was only shown to the host admin, when a board admin saved the user info (e.g. the rank) the user was not moderated anymore; all admins see and change the setting now
* [FIXED] Edit message: the persistent option is only shown to admins and moderators, when the author edited a persistent message it was not persistent anymore; it is kept now
* [FIXED] Replace words: the good word was added as html to the messages, topic subjects and descriptions (it is text entered by the admin), html in it ran on the pages; it is encoded now and html is removed when a replace word is saved or imported. The topic subject on the topic page was replaced twice
* [FIXED] Admin Edit Medal: the image of a medal was saved as sent by the form and shown as html on the medals page; only the images of the medals folder are accepted now and the image is encoded
* [FIXED] Avatar: the uploaded avatar of a user without uploaded avatar failed with an error, every request (by anyone) wrote it to the event log; it is not found now without an error
* [FIXED] Answer of a topic: when the answer was erased or moved to another topic the topic kept it, the link to the answer showed an error and no new answer could be marked (the moved post was shown as answer in the other topic)
* [FIXED] Topic page: a link to a post (e.g. a notification, the last post or a search result) opened the wrong page when deleted or not approved posts came before it (the position of the post counted them), the post was not shown
* [FIXED] Admin Host Settings: an empty number field was saved as 0, and a page size could be 0 (e.g. 0 posts per page failed every topic page); an empty or invalid number keeps the setting now and the page sizes are at least 1
* [FIXED] Suspension: the mail that the suspension ended was only sent when the user visited the forum before the hourly clean up task lifted the suspension; the task sends it too now
* [FIXED] Delete forum with moving the topics: the links of moved topics were moved too, a link to a topic of the new forum was shown next to the topic itself; these links are removed now
* [FIXED] Topic page: posts of the same second (MySQL saves whole seconds) were listed in any order and a link to such a post could open the page before it; they are listed in the order they were posted now
* [FIXED] Delete topic: the posts of a deleted topic were still counted for their authors (deleted posts are not counted), e.g. a deleted spam topic still counted for the rank; the post counts are updated now
* [FIXED] Forum statistics: the topic and post counts of a forum include its sub forums, but a new post in a sub forum only updated the counts of the sub forum; the parent forum is updated too now
* [FIXED] Guests can't post with the name (user name or display name) of a member anymore (also in other case), the post looked like a post of the member
* [FIXED] Admin Groups: the administrators group could be deleted, then the board had no admins anymore (only the host admin); it is protected like the guest group now
* [FIXED] Install: the installation waits until the default BBCodes are imported (errors of the import were lost and the forum could be used before the import was finished)
* [FIXED] Push notifications: sending a notification ran in an endless loop until the server was out of memory (keys without base64 padding, like the VAPID keys)
* [FIXED] Push notifications were never sent: creating the VAPID signature failed with the current BouncyCastle version
* [FIXED] Text avatars: all users with a numeric provider key (old membership) got a black avatar
* [FIXED] Upgrade: the upgrade waits until the default BBCodes are imported (errors of the import were lost and the forum could be used before the import was finished)
* [FIXED] Team page: the names of the moderated forums were shown double encoded in the forum select ("Tom &amp; Jerry")
* [FIXED] Admin > Edit Forum: the option "Hide if no access" had no effect (forums without read access were never listed); a forum without read access is listed with "(No Access)" now unless the option is set, new forums have it checked and the upgrade sets it for the existing forums (they stay hidden)
* [FIXED] Moderators: the users added to a forum with a moderator access mask (Moderate > Forum > Add user) were not listed as moderators (forum list, team page) and got no moderator notifications (reported posts, posts waiting for approval), only the moderators of groups
* [FIXED] Admin > Edit User > Kill: banning the addresses of the user also banned the own address of the admin (e.g. a spammer in the same network), the admin locked himself out
* [FIXED] Event log: suspensions (by an admin or of the own account) were always logged, the setting "Allow log when users suspended or un-suspended" was only used for the removal of a suspension
* [FIXED] Admin > Ranks: the start rank of the new users could be deleted when no user had it, then nobody could register (new text ADMIN_RANKS.MSG_START_RANK)
* [FIXED] Admin > Event log, Spam log: with the persian calendar the since date was 1/01/02 (read as 2022) and a value that is no date showed an error page
* [FIXED] Polls: the option to show the results only after the poll closes didn't work, the results of the running poll were shown (and the note was never shown)
* [FIXED] Admin > Edit User > Kill: the reason of the banned addresses was the text "Edits a user" (wrong text call) instead of the user, now a text with the name that fits into the column
* [FIXED] Registration: the mail to the admins about a registered spam bot had the subject [REGISTER.COMMON] (wrong text call)
* [FIXED] Delete account: the title of the page in the browser was a missing text (wrong text call)
* [FIXED] Attachments: the upload dialog stayed open when the upload ended while the dialog was still opening (fast uploads)
* [FIXED] Active users: the location of pages like the profile settings, the login or a new topic was shown as [ACTIVELOCATION.PAGE] (and logged as a missing translation)
* [FIXED] Emoji short codes and ascii smileys inside code blocks of several lines were turned into emoji
* [FIXED] The search results showed the forum name as html (e.g. a forum "Q&A <beta>" lost "<beta>"); the forum name of the search results is html encoded like the topic
* [FIXED] Watch mails: one watcher with a stored address that is no mail address (e.g. an imported user) stopped the mails to all watchers and showed an error after the post was saved; that watcher is skipped now
* [FIXED] Admin -> Banned IPs, e-mails and names: the ban date was stored in the time of the server but shown converted from UTC, it was off by the time zone of the server
* [FIXED] The guest and admin users of a new board got the join and last visit date in the time of the server (all dates are stored in UTC), and the "popular topic" icon compared the time of the last post (UTC) with the time of the server
* [FIXED] The [hide-thanks=x] BBCode counted the thanks the reader gave instead of the thanks received (as the BBCode and its message describe), the hidden content was shown to users who just thanked often enough
* [FIXED] [hide-posts=] and [hide-thanks=] with an empty or too big number failed with an error and broke the topic page for every reader; an empty number is 0, a too big one hides the content from all users but the author and the admins
* [FIXED] [attach] and [albumimg] with an id that is a number but no whole number (e.g. 1.5, 1e3, (5) or a too big number) failed with an error and broke the topic page for every reader
* [FIXED] The [group-hide=role] BBCode showed the hidden content to the users with any other role and hid it from the members of the given roles, and [group-hide] without roles hid the content from all members (not only from guests); the content is shown to the members of one of the roles now (without roles to all members), the description says so
* [FIXED] The team page (and the admin page access list) listed disabled and not approved admins
* [FIXED] The [userlink] example of the BBCode help showed the address of YAF 1.x (yaf_profile1234.aspx, without a slash after the forum address)
* [FIXED] Posting (new topic, reply, quick reply): the message of the flood delay showed only the seconds part of the remaining time (e.g. 30 instead of 90 seconds with a delay over a minute)
* [FIXED] Admin > Edit User > Kill: the suspend option only worked after the cache of the user expired
* [FIXED] Notification "topics I post to" (auto watch): a reply of the reply page did not watch the topic, the quick reply watched it even when the member unchecked the watch option, and the watch option of the quick reply was always unchecked (a quick reply removed the watch of a watched topic); the option is checked for a watched topic and with auto watch, the option decides
* [FIXED] Admin > Delete Forum: "Move all topics" without a selected forum (0) moved the topics to the forum 0 (they were lost); a forum of the board is needed now
* [FIXED] Security (hardening): a stored home page that is no web address (e.g. javascript: of an imported user or of a database of an older version) was shown as link on the profile page and the user card; only web addresses are links now
* [FIXED] Erasing a post and deleting a user only deleted the database entries of their attachments, the files of attachments stored in the file system stayed on the disk
* [FIXED] The banned e-mail import accepted masks that are no valid regular expressions (they never matched); like the name and user agent imports only valid expressions are imported
* [FIXED] New boards created a "Registered" role instead of "Registered Users" (the member group), the upgrade renames an existing "Registered" role (or moves its members to "Registered Users") for all old versions
* [FIXED] Search: the tag search didn't find new topics (the tags were saved after the post was added to the search index, changed tags were not updated), posts waiting for approval were indexed without the tags and the description of the topic
* [FIXED] Search: the user and the tag filter without a search text only searched the tag, a name or tag with a space also found other users and tags
* [FIXED] Search: a failed search kept the loading dialog open and hid the results (also of the next searches)
* [FIXED] BBCode editor: "Remove format" changed the first occurrence of the selected text in the message (not the selection), "Outdent" removed other characters when the selection didn't start and end with the indent tags
* [FIXED] BBCode editor: a cancelled media dialog inserted [media]null[/media]
* [FIXED] BBCode editor: the paste button added the text at the end of the message instead of at the cursor
* [FIXED] BBCode editor: tags with a - (e.g. [hide-thanks=1]) were closed with the wrong tag ([/hide])
* [FIXED] BBCode editor mentions: selecting a user removed the typed letters from the first word that contained them (not after the @)
* [FIXED] Upload dialog: the attachment list didn't show the uploaded files until the page was loaded again, a refused upload (e.g. too large for the server) showed no error
* [FIXED] Context menu of posts: the menu was opened away from the click when a part of the post (e.g. bold text) was clicked or on a long press on phones
* [FIXED] Register: the password strength bar of a weak password kept the color and width of a stronger password typed before
* [FIXED] Wysiwyg editor: a refused or failed drag and drop upload left the upload placeholder in the editor
* [FIXED] Register: a registration with custom profile fields on the register page always failed without a message (the attributes of the database model of the fields were validated), the fields were shown when they were required instead of when "show on register page" was set (a required field could not be hidden from the register page, a field with only "show on register page" was never shown), and the names of the fields were missing after a refused registration

# YetAnotherForum.NET v4.0.9
* [NEW] added context menu to category and forum on the main
* [FIXED #1769] paging for attachments on admin edit user page
* [FIXED] geoipservice can now correctly handle IPV6 Addresses 
* [CHANGES] tweaked the ui to make it more consistend 
* Replaced ImageSharp with SkiaSharp

# YetAnotherForum.NET v4.0.8
* [GHSA-phc5-m6cm-5c9f] security fix

# YetAnotherForum.NET v4.0.7
* [NEW] Setting to enable/disable show similiar topics when creating a new topic
* [NEW] full editor button in quick reply (resolves #1728)
* [FIXED #1724] Language Editor
* [FIXED] install/update bbcode extensions on linux
* [FIXED] Login accepted any password for accounts still using legacy Clear/Encrypted password formats
* [FIXED] Any logged-in user could move or split another user's post via the Move Message page
* [FIXED] Any logged-in user could restore/undelete an arbitrary deleted post
* [FIXED] BBCode [url=...] allowed javascript:/data: links to bypass the URL scheme filter
* [FIXED] X-Forwarded-For parsing could not resolve a client IP behind multiple proxy hops, allowing IP bans to be bypassed
* [FIXED] Reputation/Friends/Ignored User actions were vulnerable to CSRF via simple GET links
* [FIXED] Poll controller allowed unauthenticated access and could throw on invalid poll/topic/forum ids
* [FIXED] Forum delete/cleanup background tasks could report as finished while still running, allowing overlapping runs
* [FIXED] Chat hub connection tracking was not thread-safe, causing intermittent errors under concurrent use
* [FIXED] "Hide my online status" user flag had no effect on SQLite installs
* [FIXED] Active/message/private message cleanup queries never deleted anything on MySQL/PostgreSQL
* [FIXED] Half/quarter-hour timezone offsets (e.g. India, Iran, Nepal) were truncated to whole hours
* [FIXED] Reporting a message for the first time could throw an error under concurrent reports
* [FIXED] A transient database error during posting could silently disable the anti-flood delay
* [FIXED] Navigating away from the install wizard mid-setup returned a blank page instead of redirecting
* bug fixes

# YetAnotherForum.NET v4.0.6
* bug fixes

# YetAnotherForum.NET v4.0.5
* [FIXED #1637] Fix attachment list not loading on EditMessage page - Thanks @mt0321
* [FIXED #1636] Fix sitemap XML: UTF-8 encoding, correct namespace, and element order - Thanks @mt0321
* [FIXED #1635] Fixed new install with postgreSQL
* [FIXED #1628] Fix [list] and [*] BBCode for old posts - Thanks @mt0321
* [FIXED #1620] Update ampersand URL handling to ensure "A & B" in forum/topic names becomes "A-and-B" - Thanks @mt0321
* [FIXED #1619] Return 404 for unrecognized handler names that fall through to Index - Thanks @mt0321
* [FIXED #1599] double slash in sitemap URLs - Thanks @mt0321
* [FIXED #1597] Encode topic name in GeneratePageTitle - Thanks @mt0321
* [FIXED #1595] Encode dynamic fields in admin EventLog and SpamLog - Thanks @mt0321
* [FIXED #1573] forum Image now showing correctly
* [FIXED #1572] load saved Forum image
* [NEW #1622] Add category URL segments (/Category/{id}/{name}) - Thanks @mt0321
* [NEW] include albums on the user profile page
* bug fixes

# YetAnotherForum.NET v4.0.4
* Upgraded to .NET 10.0
* [NEW] Ban Ip Addresses by Country
* [NEW] Allowed UserName Characters can be set via the host settings
* [NEW] Application now is PWA ready
* [NEW] Ability to send Device push notifications (Vapid Keys can be generated here: https://yetanotherforum.net/Vapid-Keys)
* [NEW] Support for WebP image format
* bug fixes

# YetAnotherForum.NET v4.0.2
* [NEW] added task to automatically import black listed ip addresses
* [NEW] syntax highlighting for stack traces in bb code tags
* [FIXED] active location
* [FIXED] paging on the members page
* redesigned the user profile page
* bug fixes

# YetAnotherForum.NET v4.0.1
* [NEW] added support for note bbcode
* [NEW] added host setting to set the enter mode for editor (line break)
* [NEW] show user medals in hover cards
* [NEW] Set meta description for the board via Settings
* [NEW] show country in active users list based on ip address (if geoip database is used)
* [NEW] show stats for Country, Browser, Platform and new registrations on the admin dashboard
* [FIXED] paste text in the editor
* [FIXED] sitemap url
* Migrate old PMs to new YAF v4 format
* redesigned attachment upload/selection dialog
* bug fixes

# YetAnotherForum.NET v4.0.0
* Converted Project to .NET 9 (ASP.NET Core)
* Real-time Notifications for Watched Topics/Forums
* Real-time Notifications for Replies
* Real-time Notifications for Quoting
* Real-time Notifications for Mentioning
* Real-time Notifications for Received Thanks
* Replaced Private Messages with real-time user chat
* Two Factor Authentication

# YetAnotherForum.NET v3.2.16
* bug fixes

# YetAnotherForum.NET v3.2.15
* [GHSA-phc5-m6cm-5c9f] security fix

# YetAnotherForum.NET v3.2.14
* [FIXED] Legacy "Clear" password format lets you log in as anyone with any password
* [FIXED] Account lockout is recorded but never enforced
* [FIXED] Stored XSS via attachment filename
* [FIXED] Stored XSS via [url=...] javascript: scheme bypass using an embedded tab
* [FIXED] Private-message replies leak across users in broadcast threads
* [FIXED] IDOR on attachments
* [FIXED] Private message leak
* [FIXED] PM delete IDOR � no ownership check before deleting a UserPMessage by ID.
* [FIXED] Stored XSS via scheme bypass in bbcode url
* bug fixes

# YetAnotherForum.NET v3.2.13
* bug fixes

# YetAnotherForum.NET v3.2.12
* bug fixes

# YetAnotherForum.NET v3.2.11
* bug fixes

# YetAnotherForum.NET v3.2.10
* bug fixes

# YetAnotherForum.NET v3.2.9
* [NEW] added support for note bbcode
* [NEW] added host setting to set the enter mode for editor (line break)
* redesigned attachment upload/selection dialog
* bug fixes

# YetAnotherForum.NET v3.2.8
* [NEW] Board Setting to set the default page size
* [FIXED #1371] paste from clipboard and drag & drop in the SCEditor
* [CHANGES #1372] increase Interests field to 4000 characters 
* bug fixe

# YetAnotherForum.NET v3.2.7
* [FIXED] New Install
* bug fixes

# YetAnotherForum.NET v3.2.6
* [NEW] log user agent strings for Active users
* [NEW] added support for twitch bbcode embed
* [NEW #1187] reply via ctrl+enter
* [FIXED #1179] switching darkmode
* [FIXED] thanks info for guest
* [FIXED] search in dropdowns
* bug fixes

# YetAnotherForum.NET v3.2.5
* [CHANGES] changed quick reply button
* bug fixes

# YetAnotherForum.NET v3.2.4
* [NEW] Set theme mode via browser or via dropdown
* [FIXED] Load hovercard data only on show
* bug fixes

# YetAnotherForum.NET v3.2.3
* [NEW] New WYSIWYG SCEdtitor
* [CHANGES] updated help menu
* [CHANGES] improved saving of registry
* [CHANGES] removed legacy nntp support
* [FIXED] topics post stats
* [FIXED] send notifications only to approved and not deleted users
* [FIXED] Migrations from older versions
* bug fixes

# YetAnotherForum.NET v3.2.2
* [FIXED] Install wizard
* [FIXED] crawler detection
* [FIXED #1032] admin access to moderate page
* [FIXED] eventlog toggle button
* bug fixes

# YetAnotherForum.NET v3.2.1
* [FIXED] create poll button access
* [FIXED] disable register page
* [FIXED] Issue with AntiXsrfToken
* [FIXED] Install wizard
* bug fixes

# YetAnotherForum.NET v3.2.0
* [NEW] updated quote styling
* [CHANGES] upgraded to .NET Framework v4.8.1
* [CHANGES] hover cards now using bs popovers
* [CHANGES] Digest Email now uses email template
* [CHANGES] updated edit album title & album image caption UI
* [FIXED] Show PM Replies
* [FIXED] Sitemap urls
* removed jquery
* replaced all jquery plugins with vanilla JS
* bug fixes

# YetAnotherForum.NET v3.1.15
* [FIXED #941] Replacing the CKEditor with the (old) improved BBCode Editor
* [FIXED] Upgrade process
* [FIXED #785] Admin can now change user passwords
* [NEW] UserAgents can be banned
* Upgraded to jQuery 3.7.1
* bug fixes

# YetAnotherForum.NET v3.1.14
* [NEW] Added user setting for switching theme to light/dark mode
* [FIXED] Wildcards in Banned IP
* Upgraded to jQuery 3.7.0
* Upgraded to CKEditor 4.22.1
* bug fixes

# YetAnotherForum.NET v3.1.13
* bump to FontAwesome 6.3.0
* Upgraded to jQuery 3.6.4
* Upgraded to CKEditor 4.20.2
* [FIXED] Changing user email
* [FIXED] Cover Image for Albums
* bug fixes

# YetAnotherForum.NET v3.1.12
* [FIXED] Stored XSS in Signature

# YetAnotherForum.NET v3.1.11
* [FIXED] Stored XSS in Inbox (Send a private message)
* Updated Event log ui
* bug fixes

# YetAnotherForum.NET v3.1.10
* [FIXED] Image Gallery close button 
* [FIXED] Code block when quoting a post
* [FIXED] Add New Button on Replace Words and Profile Definitions Page
* [FIXED] Notifications on Admin Pages
* Upgraded to jQuery 3.6.3

# YetAnotherForum.NET v3.1.9
* [FIXED #813] custom profile checkboxes
* [FIXED #810] Save Board Email Address
* [FIXED #807] Enabled Browser based spell checking in the Editor
* [FIXED #806] wrong member count in Active User List
* [FIXED #805] Mark all Forums as Read
* [FIXED #803] empty p tags in bbcode list elements
* [FIXED] showing sticky topics at the top
* [NEW] Support for tables via table bbcodes
* bug fixes

# YetAnotherForum.NET v3.1.8
* [FIXED #761] Delete Board Announcement
* [FIXED #762] deleting posts with Reply
* Upgraded to Bootstrap 5.2.2
* Upgraded to CKEditor 4.20.0
* Bug fixes

# YetAnotherForum.NET v3.1.7
* Upgraded to Bootstrap 5.2.0
* Bug fixes

# YetAnotherForum.NET v3.1.6
* [NEW] Converted language files from xml to json
* [NEW] Auto translated language files via DeepL api and Google translate api
* Upgraded to CKEditor 4.19.0
* [FIXED] Migrating attachments from old dbs
* [FIXED #729] show duplicate topics
* [FIXED #740] delete all messages/topics
* [FIXED] soft message delete
* [FIXED] sending welcome message
* [FIXED #747] moving message
* [FIXED] mobile context menu
* [FIXED] report pm
* [FIXED] saving banned name
* [FIXED] user delete user account
* bug fixes

# YetAnotherForum.NET v3.1.5
* [NEW] Added a Setting to enable/disable Custom Context Menu in Posts
* bug fixes

# YetAnotherForum.NET v3.1.5
* bug fixes

# YetAnotherForum.NET v3.1.4
* [NEW] Active Flag for Categories to enable/disable a Category
* [NEW] Infinite Scrolling on Board Index if there are more then 20 Forums
* [NEW] Infinite Scrolling for all Forum Dropdowns
* [FIXED] New Installations
* [PERFORMANCE] removed various javascript libaries to server side rendering

# YetAnotherForum.NET v3.1.3
* Added new Audio BBCode (thanks to User daprose-dotcom)
* Upgraded to FontAwesome 6.1.1
* Upgraded to CKEditor 4.18.0
* [FIXED] Show user attachments page only if user has attachments and upload access
* [FIXED] wrong forums count in stats (When the board has no posts)
* Category names now have links to the Category
* [NEW] Default PanelState for the Category Collapse Button can be defined in the Board Settings
* [NEW] Added Forums & Categories to Board Stats on the admin index page
* [NEW] Added options to re-order Categories and/or Forums by Name ASC or DESC
* [FIXED] Wrong Background Color and missing border for select2 dropdowns
* [FIXED] Moderator Avatar on the team page
* [FIXED] Duplicates in Moderator List on the team page
* [FIXED] On the team page moderator forums are grouped by category
* Moved "Autosave" Notification in the Editor to the status bar
* bug fixes

# YetAnotherForum.NET v3.1.2
* bug fixes

# YetAnotherForum.NET v3.1.1
* [FIXED] Wrong Role Name after new Install
* [FIXED] saving topic tags

# YetAnotherForum.NET v3.1.0
* [NEW] Auto Upgrader
* [NEW] New Topics or Replies of Wach Forum/Topic appear in Notifications
* Upgraded to FontAwesome 6.0.0
* Attachments are now displayed in Search Results
* [FIXED] Border on Topic Tags Input
* [FIXED] Upgrading from old Versions (older then DB Version 30)
* [BREAKING] switched upload/download access from forum access masks to group access

# YetAnotherForum.NET v3.0.2
* [FIXED #658] Re-Added CTRL+Enter to submit post
* [FIXED #656] The UTC time represented when the offset is applied must be between year 0 and 10,000
* [FIXED] Not Approved users should not be able to log-in
* [NEW] Not Approved Users can re-send the Confirm Email 
* [FIXED] Not Approved users can be edited in the Admin Interface 
* Upgraded to CKEditor 4.17.1
* bug fixes

# YetAnotherForum.NET v3.0.1
* [FIXED #645] Send PM for regular users
* bug fixes
 
# YetAnotherForum.NET v3.0.0
* Minimum .NET Version v4.8
* Upgraded to Bootstrap 5.1.3
* [NEW] Added Support for MySQL
* [NEW] Added Support for SQLite
* [NEW] Added Support for PostgreSQL
* [NEW] Custom Profile Properties
* [NEW] CKEditor is now standard editor
* [NEW] Html Email Template
* [NEW] Standard Text Avatars
* Replaced obsolete Membership/Role/Profile Provider with ASP.NET Identity
* Automatic migration of all old attachments on upgrade
* Completed DAL changes

# YetAnotherForum.NET v2.3.1.15
* [NEW] Added Page Title template option
* [NEW] Added New Admin Page for managing Attachments
* [NEW] Added New Admin Page for managing Topic Tags
 
# YetAnotherForum.NET v2.3.1.14
* added board name to page title
* [FIXED] Add topic tag to new topic
* [FIXED] XSS attack attack on fileupload (thanks to User @sushiwushi)
* [FIXED] XSS on returnurl (thanks to User @sushiwushi)
* [FIXED] XSS in Hovercard (thanks to User @sushiwushi)

* # YetAnotherForum.NET v2.3.1.13
* [NEW] Added Board Setting for Default Access Mask when creating a new forum
* [FIXED] Deleting a Message with an Attachment
* [FIXED #603] subforums are not saved
* [FIXED] lock/unlock topic
* [FIXED] Back Button on Forget Password Page
* [FIXED] Dont Render Crawlers as Regular Users
* [FIXED] IP Address Check for IPv6
* Exclude Favicon inside a Portal (DNN)

# YetAnotherForum.NET v2.3.1.12
* Upgraded to Bootstrap 4.6.0
* [NEW] Success Message after Message Report
* Increased auto save Delay in CKEditor to 60 Seconds
* [FIXED] DNN Search

# YetAnotherForum.NET v2.3.1.11
* [FIXED] Admin User page
* [Fixed] publish solution

# YetAnotherForum.NET v2.3.1.10
* [FIXED] wrong lanuage code for hebrew
* [FIXED] version info
* [FIXED] youtube autoembed
* Install Membership Provider Scripts on Upgrade for Azure if needed
* Upgraded to Bootstrap 4.5.3

# YetAnotherForum.NET v2.3.1.09
* [NEW] added login failure logging
* [SECURITY FIX] for Private Messages
* [FIXED] duplicate topic title check
* [FIXED] Attachments inside PMs
* [FIXED #589] visiblity of profile
* [FIXED] Remove line break after code block
* [FIXED #588] creating of new medals
* FIXED] show info message after shrink db
* Removed Postback before logout dialog appears

# YetAnotherForum.NET v2.3.1.08
* Upgraded to Bootstrap 4.5.2
* [FIXED] Cookie Alert 
* [FIXED] Dropdowns for small devices
* [FIXED] Attachments

# YetAnotherForum.NET v2.3.1.07
* Upgraded to jQuery 3.5.1
* Upgraded to Bootstrap 4.5.0
* [NEW] user attachments can be managed
* [FIXED] font size in quotes
* [FIXED] Pruning of private messages
* [FIXED #569] Instagram BB Code
* [FIXED] Links to Moved topics
* [FIXED #580] wrong DateTime format for Sitemap Urls
* [FIXED #579] Clear cache on user delete

# YetAnotherForum.NET v2.3.1.06
* [FIXED] Emoji Button in CKEditor
* Upgraded to CKEditor 4.14.0
* Bug fixes
 

# YetAnotherForum.NET v2.3.1.05
* [FIXED] sending watch emails + added erro handling
* [FIXED] MultiQuote Button
* Bug fixes


# YetAnotherForum.NET v2.3.1.04
* Avatars are now visible for posts on mobile devices 
* Removed spacing from Quotes
* [FIXED] Display Date Times in Posts
* [FIXED] dont send email notifications after edit post
* Better handling of mass emails


# YetAnotherForum.NET v2.3.1.03
* [NEW #549] Admin option to restore or permanently delete old topics that are deleted
* Language for guests is now based on current browser Language
* Bug fixes


# YetAnotherForum.NET v2.3.1.02
* [FIXED #541] Create Poll
* [FIXED #539] Admin Groups & Ranks Page
* [FIXED] Topics & Post count in Forum List 


# YetAnotherForum.NET v2.3.1.01
* [FIXED] YAF Upgrade Message
* [FIXED] Hide RSS Feed dropdown if rss & atom feeds are disabled


* # YetAnotherForum.NET v2.3.1.00
* [NEW] Sitemap.xml file with all Forums
* [NEW #240] Topic Tags
* [NEW #53] Option to lock In-Active Users
* [NEW #216] Report Private Messages
* [NEW #283] Global Board Announcement
* [NEW #495] send all emails in html format witch forum layout
* [NEW] layout for quote messages 
* [NEW #477] Member based Post Moderating
* [NEW] Context menus for admin pages (open with right click)
* [CHANGES] Moved Forum Icon Legend in to a popover
* [CHANGES] Upgraded to Bootstrap 4.4.1
* [FIXED #249] Watch Topics/Forum Emails are not send in user language 

# YetAnotherForum.NET v2.3.0.07
* [FIXED #528] Topics view
* [FIXED] csv user import commas inside quotes


# YetAnotherForum.NET v2.3.0.06
* Bug fixes

# YetAnotherForum.NET v2.3.0.05
* Bug fixes

# YetAnotherForum.NET v2.3.0.04
* [FIXED] selecting forum image
* [FIXED #501] error after edit moderated post
* [FIXED] sending multiple digest mails
* [FIXED] deleting role
* [FIXED] Similar Topics


# YetAnotherForum.NET v2.3.0.03
* [FIXED #492] relative time tooltips
* [FIXED] deleting of private messages
* [FIXED #494] Add as friend button on profile page
* [FIXED #497] saving forum & category
* [FIXED #499] saving group
* [FIXED #498] removed page refresh on the postmessage page
* [FIXED #500] error on moving topic
* [FIXED] topic urls


# YetAnotherForum.NET v2.3.0.02
* [FIXED] Send Email to Admins on new User Register
* [FIXED] Search Function when Board has no Messages
* [FIXED] Board Delete function
* [FIXED #490] Duplicate Editor Module Id
* [FIXED] Saving Board logo
* [FIXED] forum statistics when active discussion is disabled
* [FIXED #488] Multi Quote Function
* [CHANGES] Spam & Event Log Options has been moved to a filter drop down 
* [CHANGES] Empty Tabs on My Friends page show info message

 
* # YetAnotherForum.NET v2.3.0.01
## Fixed Issues:
* [FIXED] Send Email to Admins on new User Register
* [FIXED] Search Function when Board has no Messages


# YetAnotherForum.NET v2.3.0.0

## New Features:
* [NEW] UI Based on Bootstrap 4
* [NEW] 22 Bootstrap Themes
* [NEW] Added Dropdown on the moderate page to move multiple topics
* [NEW #375] indication in the menu if post needs moderation
* [NEW #365] now you can archive pm's from the outbox
* [NEW #367] Added Badge Topic Starter Badge
* [NEW #379] Mark Message as Answer
* [NEW] Old GIF Emoticons have been replaced by Unicode Emoji
* [NEW #285] Rewritten Search Engine based on Lucene.NET
* [NEW #92] Show similar topic titles
* [NEW #89] Added Buton "Get Location from IP Address" to Retrieve the Country & City from the IP Address
* [NEW #107] Added Button "Watch all Forums"
* [NEW #258] Anti Spam statistics
* [NEW #256] automatically save message draft and also added save button
* [NEW #286] Mentions of users via @Username
* [NEW #287] Onsite Notification for Quoted Posts, Mentions and received Thanks
* [NEW #457] Board Logo can be set in the Board Settings
* Attachments can be uploaded by Drag & Drop in the Editor

## Fixed Issues:
* Fixed Unicode Urls


# YetAnotherForum.NET v2.2.4.20
## Fixed Issues:
* [FIXED] Exclude topics and forum page from noindex


# YetAnotherForum.NET v2.2.4.19
## Fixed Issues:
* [FIXED] Attachments Upload on Private Message

# YetAnotherForum.NET v2.2.4.18
## New Features:
* Upgraded to CKEditor 4.12.1

## Fixed Issues:
* Fixed issue with display of current YAF Version


# YetAnotherForum.NET v2.2.4.17
## New Features:
* You can now use relative urls for remote forum urls

## Fixed Issues:
* Image attachment thumbnails are now always max 200 x 200

# YetAnotherForum.NET v2.2.4.16
## New Features:
* Added Setting to enable/disable cookie consent popup

## Fixed Issues:
* updated twitter bbcode extension
* fixed issue with code blocks on mobile devices
* improved "message_move" and "forum_updatelastpost" performance
* [FIXED] XSS Issue

# YetAnotherForum.NET v2.2.4.15
## New Features:
* Added Instagram BBCode

## Fixed Issues:
* [FIXED] Issue when search box is empty
* updated vimeo bb code extension
* [FIXED] pager on album images selector
* [FIXED] emaildigest
* [FIXED #413] shows username instead of diplay name of deleted user
* Posts Pages with querystring "m" now contains noindex tag
* [FIXED] unread topics

# YetAnotherForum.NET v2.2.4.14
## Fixed Issues:
* [FIXED] Attachments Upload on Private Message
* [FIXED] horizontal scrollbar
* Remove non-breaking spaces from Thanks List
* [FIXED #400] using default prism.js theme
* [FIXED] ceebox script for jquery 3.x


# YetAnotherForum.NET v2.2.4.13
## New Features:

* [NEW] Send Email to user on role (un)assignment
* [NEW] Added GDPR Cookie Consent Check and Page
* Updated the Rule and Privacy Policy with GDPR Info

## Fixed Issues:
* [FIXED] not working attachments upload
* [FIXED] yafeditor script was not automatically minified
* [FIXED] Send Email to User
* [FIXED #402] correctly display local datetime
* Updated recommended web.configs


# YetAnotherForum.NET v2.2.4.12

## Fixed Issues:
* Editor Dropdown with album images now uses sql paging

# YetAnotherForum.NET v2.2.4.11

## Fixed Issues:
* [NEW #392] Security Question & Answer can be changed now


# YetAnotherForum.NET v2.2.4.10

## Fixed Issues:
* [FIXED #394]  Timezone was not correctly loaded on the Edit Profile Page


# YetAnotherForum.NET v2.2.4.9

## Fixed Issues:
* [FIXED] Admin Edit User
* [FIXED] Admin Create New User Page


# YetAnotherForum.NET v2.2.4.8

## Fixed Issues:
* [FIXED] Issue Registering New User
* [FIXED] Database Ugrade Issue when Fulltext is enabled


# YetAnotherForum.NET v2.2.4.7

## Fixed Issues:
* [FIXED #387] Stored XSS via file upload
* [FIXED #386] Hover Card Layout Issue


# YetAnotherForum.NET v2.2.4.6

## Fixed Issues:
* [FIXED] always use forum email as sender address


# YetAnotherForum.NET v2.2.4.5

## Fixed Issues:
* [FIXED #380] DB User Read Tracking without Session tracking
* [FIXED #377] deleting of a banned ip address
* [FIXED] handling of urls with parenthesis
* [FIXED] attachments control was visible when uploads are disabled
* [FIXED] handling of old attachments
* [FIXED] ignore ThreadAbortExceptions when adding a album image
* Extended Logging


# YetAnotherForum.NET v2.2.4.4

## Fixed Issues:
* [FIXED] Remote Avatars
* Removed Editor overriding when using a Mobile device
* Improved BBCode <-> HTML Code Converter


# YetAnotherForum.NET v2.2.4

## New Features:

* [NEW] Impoved spam detection now you can define how many urls are allowed inside a message before flagged a spam
* [NEW] Sitemap Provider for DNN Platform
* [NEW] Redirect Forum Remote URL to new Window
* [NEW #353] Added Spam Check when sending PM's
* [NEW #341] IP lookup also in the banned IP section


## Fixed Issues:

* [FIXED #329] check box to enable topic watching and receive notification in quick reply form
* [FIXED] Unable to Cast to object error during install.
* [FIXED] when converting from html to bbcode preserve paragraphs
* [FIXED] unable to cast object issue
* [FIXED #334] Error on Ban User Page when there is no IP Address
* [FIXED] user style cache was not cleared after changing Rank/Group
* [FIXED] added a check if email address is provided when connect via Facebook
* [FIXED #338] user was not deleted when have attachments
* [FIXED #337] similar topics shows topics with no posts
* [FIXED #342] url encode values when rewriting querystring
* [FIXED] admins are not listed as mods and don't receive mod emails
* [FIXED] board_poststats SP which do not delete readtracking before trying to delete removed topics
* Added Canonical Meta Tag Module
* [FIXED] issue with url search when use the word install or error
* [FIXED] little issue with the finish language
* [FIXED] updated SSO Facebook API to version 2.9
* [FIXED] Issue with IP v6 Addresses
* [FIXED #357] Quoting when user name contains html code


### Full Change log here:
https://github.com/YAFNET/YAFNET/commits/master



# YetAnotherForum.NET v2.2.3

## New Features:

* 

## Fixed Issues:

* [FIXED #300](https://github.com/YAFNET/YAFNET/issues/300) Warning message for duplicate Smiley Codes
* [FIXED] Issue with password check on the Register page
* [FIXED] Back Button on the Profile Modify Avatar page
* [FIXED] Host Setting for the Visibility of the Retweet Button was ignored
* [FIXED] Notification for all posts on all topics
* [FIXED] Register Page
* [FIXED #310] Attachment Render Error
* Moderating of users is now limited to Admin/Host/ and Mini admin. Toavoid security issues.
* tiny fix for the swedish language file
* [FIXED] Shoutbox was not corretly disabled when using the mobile theme
* Upgrades JS Libaries
* Use Mobile Theme option is now always available
* [FIXED] an Issue with the Preview function when using CKEditor
* [FIXED #316] XSS Prevention for the login page
* [FIXED #269] Thanks Button is disabled now when post is deleted
* [FIXED] Moderate Pages where visible for all Users
* [FIXED] Team Page now only lists moderators from the current Board
* All boards are now upgraded on a Upgrade
* [FIXED] Attachments in Multi Board installations
* [FIXED] issue with Attachments with the same name override old attachments
* [FIXED] When creating a new Board the BBCode Extensions and other are correctly installed


### Full Change log here:
https://github.com/YAFNET/YAFNET/commits/master


# YetAnotherForum.NET v2.2.2

## New Features:

* [NEW #204](https://github.com/YAFNET/YAFNET/issues/204): enhancements to the suspension feature
* [NEW] Image Attachments & Album images are now opening in modal window in editor preview pane
* [NEW] Added Search Function to Admin Banned Email/IP/Name Pages
* [NEW] Added new Password strength check to the Register Page
* [NEW] Image Attachments & Album images are now opening in modal window in editor preview pane
* [NEW] after new attachment upload list with "current attachments" is autom. updated
* [NEW] CKEditor now has the same keystrokes as the standard YAF Editor
* Banned IP/Email/Names Admin Pages now are sorted by newest first

## Fixed Issues:

* [FIXED #282](https://github.com/YAFNET/YAFNET/issues/290) Code Highlighting on the post message page inside the last posts
* [FIXED #282](https://github.com/YAFNET/YAFNET/issues/282) Attachment images dimensions where not correctly set
* [FIXED #276](https://github.com/YAFNET/YAFNET/issues/276) Banned IP/Email/Names now are sorted by newest first
* [FIXED #275](https://github.com/YAFNET/YAFNET/issues/275) wrong label text
* [FIXED #277](https://github.com/YAFNET/YAFNET/issues/277) Search form exception
* [FIXED #272](https://github.com/YAFNET/YAFNET/issues/272) Notification sending and board name was not used in all forum emails
* [FIXED] Topic Name where not correctly encoded on the Similar topics control
* [FIXED] Issue with install scripts in case sensitive data bases
* [FIXED] Fixed Caching issues
* [FIXED] Wrong label text on the Add/Edit Album Page
* [FIXED] Issue where New User Email Notification was send after the user was automatically deleted by the spam check system
* [FIXED] Issue with saving timezone
* [FIXED] Provider Scripts where not updated on a Upgrade
* [FIXED] Re-Added VB language for the code Highlighter
* IMG bbcode tags can now handle images from google shared photos


### Full Change log here:
https://github.com/YAFNET/YAFNET/commits/master

# YetAnotherForum.NET v2.2.1

## New Features:

* [NEW #217](https://github.com/YAFNET/YAFNET/issues/239): Selected Quote of Messages
* User online status icon on the profile page now indicates when user is suspended
* [NEW #239](https://github.com/YAFNET/YAFNET/issues/239): Multiple image attachments are now grouped as ceebox gallery
* SQL scripts are not splitted in install/upgrade for faster install/upgrade
* Admin Users page now shows if user is suspended and until which date
* On the Admin Users page you can now filter by suspended users to show only suspended users
* Admin Users Page now shows an Twitter/Facebook/Google icon if the users is logged in via that SSO Service
* It is now possible to select multiple attachments for deleting
* New Bootstraped Error Page

## Fixed Issues:
* [FIXED #269](https://github.com/YAFNET/YAFNET/issues/269) Some buttons where visible when post is soft deleted
* [FIXED #268](https://github.com/YAFNET/YAFNET/issues/268) If current culture is RTL page automatically switches to RTL mode
* [FIXED #265](https://github.com/YAFNET/YAFNET/issues/265) Event/Spam Log Date Time is now displayed in user time
* [FIXED #262](https://github.com/YAFNET/YAFNET/issues/262) Pagination on search after navigating back to the search results 
* [FIXED #261](https://github.com/YAFNET/YAFNET/issues/261) Change Password page Enter key and Submit event 
* [FIXED #248](https://github.com/YAFNET/YAFNET/issues/248) issue with access rights checking
* [FIXED #250](https://github.com/YAFNET/YAFNET/issues/250) File Attach Button in the Standard Editor was not rendered after postback
* [FIXED #247](https://github.com/YAFNET/YAFNET/issues/247): Notification on all posts and topics without read access check
* [FIXED #244](https://github.com/YAFNET/YAFNET/issues/244): Fixed not Working Upload Dialog in editors other then the Standard BB Code Editor
* [FIXED #243](https://github.com/YAFNET/YAFNET/issues/243): a few tweaks to the attachments upload dialog
* [FIXED] tinyMCE is now longer included by default because of license incompatibility
* [FIXED] dateTime Issue with the Most Recent users feature
* [FIXED] Exception caused by bots on the posts page
* [FIXED] duplicate loading of the jQuery-UI theme CSS
* [FIXED] Issue with reCaptcha on the register page
* [FIXED] timeAgo loading Script
* [FIXED] Wrong Encoding in some places
* [FIXED] Issue with Unicode Chars from the Quick Search
* [FIXED] emails inside BBCode Code tags
* [FIXED] not working confirm dialogs on some Delete Buttons
* [FIXED] User messages where not deleted on user delete and ban
* Removed not working report message to Akismet
* Attachments can now dropped anywhere inside the upload dialog
* Better error descriptions for SSO (Facebook,Google,Twitter)
* Attachment lists are now loaded paged via AJAX to avoid long loading times

### Full Change log here:
https://github.com/YAFNET/YAFNET/commits/master

# YetAnotherForum.NET v2.2.0

## New Features:

* [#18](https://github.com/YAFNET/YAFNET/issues/18): Added Sorting/Paging for Active Users Page 
* [#18](https://github.com/YAFNET/YAFNET/issues/18): Added Sorting/Paging for Active Users and Unverified Users on the Admin Page
* [#229](https://github.com/YAFNET/YAFNET/issues/229): Topics List on the MoveMessage Page are now loaded paged via ajax
* [#191](https://github.com/YAFNET/YAFNET/issues/191): YAF.NET is now compatible with Azure
* [#112](https://github.com/YAFNET/YAFNET/issues/112): Intuitive Posting of Photos
* [#230](https://github.com/YAFNET/YAFNET/issues/230): Thousand Separator in Active Users
* [#198](https://github.com/YAFNET/YAFNET/issues/198): Display of user's medal in his profile
* [#196](https://github.com/YAFNET/YAFNET/issues/196): Spam email address import needs additional check
* [#181](https://github.com/YAFNET/YAFNET/issues/181): Restore Previous Message Button on the Message History Page
* [#168](https://github.com/YAFNET/YAFNET/issues/168): added Ability to send replies with Ctrl-Enter
* [#163](https://github.com/YAFNET/YAFNET/issues/163): Add a "resend confirmation email" functionality
* [#158](https://github.com/YAFNET/YAFNET/issues/158): Optional Connect Message is now shown after the first message
* [#154](https://github.com/YAFNET/YAFNET/issues/154): added new spam event log page to the spam protection section of the admin area
* [#146](https://github.com/YAFNET/YAFNET/issues/146): SEO improvements for meta title,description,keywords
* [#133](https://github.com/YAFNET/YAFNET/issues/133): forum description is now optional
* [#129](https://github.com/YAFNET/YAFNET/issues/154): number of shown sub forum can be defined in the host settings
* [#123](https://github.com/YAFNET/YAFNET/issues/123): Save Avatars in Physical folder
* [#105](https://github.com/YAFNET/YAFNET/issues/105): Automatic YouTube link recognition
* [#103](https://github.com/YAFNET/YAFNET/issues/103): Welcome mail to new members upon successfully verified registration
* [#102](https://github.com/YAFNET/YAFNET/issues/102): Display similar threads and topics
* [#100](https://github.com/YAFNET/YAFNET/issues/100): View topic started by xxxx" instead of just "View topic" in Active Discussions list
* added new event log filter for "detected a banned ip"
* added ability to search for post titles only
* existing BBCode extensions can now be upgraded during an upgrade
* moved to no Captcha reCAPTCHA
* Re-designed install wizard
* CSS Files are now combined
* JS Files are now combined
* Switched from syntax highlighter to Prism.js

##Fixed Issues:
* [#215](https://github.com/YAFNET/YAFNET/issues/215): added missing Board Name to mass emails
* [#214](https://github.com/YAFNET/YAFNET/issues/214): added missing {databaseOwner}
* [#209](https://github.com/YAFNET/YAFNET/issues/209): Fixed group name parameter length to match the length of database field
* [#202](https://github.com/YAFNET/YAFNET/issues/202): Spam check now also checks for google/facebook accounts
* [#196](https://github.com/YAFNET/YAFNET/issues/196): added check if import email address is an email
* [#195](https://github.com/YAFNET/YAFNET/issues/195): Unapproved Posts go to RSS
* [#182](https://github.com/YAFNET/YAFNET/issues/182): twitter hover card not working
* [#180](https://github.com/YAFNET/YAFNET/issues/180): rss fixes
* [#179](https://github.com/YAFNET/YAFNET/issues/179): Multiple email notifications on the same event
* [#173](https://github.com/YAFNET/YAFNET/issues/173): "kill user" fails for users with no IP
* [#172](https://github.com/YAFNET/YAFNET/issues/172): Country list not sorted by country name
* [#170](https://github.com/YAFNET/YAFNET/issues/170): updated time zone names for Russian cities
* [#169](https://github.com/YAFNET/YAFNET/issues/169): Focus on text controls for search
* [#165](https://github.com/YAFNET/YAFNET/issues/165): deleting a topic
* [#161](https://github.com/YAFNET/YAFNET/issues/161): Country list null object in Event Log
* [#157](https://github.com/YAFNET/YAFNET/issues/157): Img tag links get extra // in IE
* [#155](https://github.com/YAFNET/YAFNET/issues/155): fix for banned ip when banned ip is "::1"
* [#152](https://github.com/YAFNET/YAFNET/issues/152): Cannot log out of Moderator from IE11
* [#148](https://github.com/YAFNET/YAFNET/issues/148): Tweaked the Meta Content
* [#78](https://github.com/YAFNET/YAFNET/issues/78): Editing Medal's throws error and logs me out
* fixed nvarchar length in role name parameters
* fixed page title set on the popup page
* removed not working spell button
* fixed issue with the postid anchor
* timeago loading issue fixed
* Security fix rss topic feeds showed wrong forum topics
* Register link on the login page now goes to the roles page if enabled
* new index to increase sql performance
* Fixed message converting when user quotes a html message with an bbcode editor
* Security XSS Preventing Fixes
* fixed an issue where the user was automatically logged out when the logout url was used inside an img bbcode
* fixed some exceptions caused by spam bots
* Posts in Forum RSS Feed are now correctly sorted
* Wrong email template for Message Approval Mail

###Full Change log here:
https://github.com/YAFNET/YAFNET/commits/master
