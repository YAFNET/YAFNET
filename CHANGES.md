YetAnotherForum.NET Changelog
====================

# YetAnotherForum.NET v3.2.17
* [NEW] Extended unit test suite (YAF.Tests, renamed from YAF.Tests.CoreTests) including repository tests against an in-memory SQLite database
* [NEW] UI tests (YAF.Tests.UITests, Playwright): the forum is installed with the install wizard into a LocalDB database and run with IIS Express (no Docker); login, registration, topics, posts, private messages, profile, every page for guests, members and admins, and access checks
* [CHANGES] The client address is taken from X-Forwarded-For / X-Real-IP only for requests of trusted proxies. Proxies in loopback and private networks are trusted by default; a proxy or CDN with public addresses has to be added to the new web.config setting YAF.TrustedProxies (addresses or CIDR networks, separated by comma), otherwise its address is used as the client address
* [FIXED] IP bans could be bypassed: any client could send an X-Forwarded-For header with another address, and the header of several proxies was always read at its first address
* [FIXED] IP bans: a ban matched every address that contained it (e.g. the ban 10.0.0.1 also banned 110.0.0.15 and 10.0.0.12), an invalid mask caused an error; masks are compared exactly, a * matches one part of an ipv4 address
* [FIXED] Login of migrated legacy membership passwords: clear text passwords were compared ignoring the case, base64 hashes were compared ignoring the case, hashes of the ASP.NET membership (salt + password) did not match, a clear text password with a "|" could not be used, unknown formats were compared as clear text
* [FIXED] Deleting an avatar could delete any file of the forum (e.g. /Content/... or a path with ..); only files in the uploads folder are deleted
* [FIXED] Ladder ranks: users were never promoted to the next rank by their number of posts
* [FIXED] User styles: an old style (e.g. of a group the user was removed from or a removed group or rank style) was never removed, the style was not updated when the rank changed, and deleting a group with a style didn't update the styles of its members
* [FIXED] Topic list: the "show topics of the last ..." filter used the wrong period (e.g. "Last Day" showed two days, "Last Six Months" a year) and "Last Year" was missing
* [FIXED] Active users: the crawlers were taken from the wrong flag (members excluded from captchas were shown as crawlers, real crawlers were missing), with guests but without crawlers the crawlers were shown anyway
* [FIXED] Prune topics: 0 or a negative number of days removed all topics, already deleted topics were deleted and counted again, pruning all forums failed with an error when the topics were deleted for good
* [FIXED] Deleting a topic didn't count the posts of its authors again
* [FIXED] Moving posts: the post counts of the topics were wrong when deleted posts were moved, a moved answer stayed the answer of the old topic, and the search index kept the old topic and forum (a post moved to a forum the user can't read was still found)
* [FIXED] Moving a topic: the search index kept the old forum, an invalid number of days for the link of the moved topic caused an error
* [FIXED] Editing a post removed the tags of the topic from the search index
* [FIXED] Tags: a tag entered twice (also with spaces around it) was added twice and the post could not be saved; a tag longer than its column (MySQL) failed
* [FIXED] Admin edit forum: a forum with sub forums could not get a parent; the parent can't be the forum itself or one of its sub forums
* [FIXED] Message history: the comparison of two versions failed with an error for most texts
* [FIXED] Album titles, image captions and the reasons of edited and deleted posts are stored html encoded, saving failed when the encoded text was longer than its column; a changed album title was encoded twice
* [FIXED] Text avatars: a name with "&" created an invalid image, names with several spaces, empty names or names of emoji only failed, a provider key that is no color created an invalid color
* [FIXED] Ignored users: ignoring or removing a user was not shown on the same page, a user could ignore themselves
* [FIXED] Host settings: "Allow private message attachments" could not be saved (the setting was stored under another name)
* [FIXED] Language list: a language file with missing pages or texts caused an error
* [FIXED] Generated passwords could contain three identical characters in a row
* [FIXED] Topic names with " & " became "AandB" in the url instead of "A-and-B"
* [FIXED] Paging: a huge page number in the url caused an error (negative offset)
* [FIXED] Admin -> Database: "Change recovery mode" re-indexed the database instead of changing the recovery mode
* [FIXED] SQLite: the table and column checks used by the database migrations never found existing tables and columns
* [FIXED] MySQL: the current time of the queries was the date only (midnight), e.g. the active users of the day were never removed
* [FIXED] PostgreSQL: the difference of two times in minutes was only the minutes part (2:05 hours were 5 minutes)
* [FIXED] Deleting all rows of a table reported no success when exactly one row was deleted
* [FIXED] Editing a post never approved it (also in forums that are not moderated, or by a moderator), the flags were changed on a copy; the search index kept the old flags
* [FIXED] Editing a post twice in the same second failed on MySQL (the versions of the history have a key of the post and the time)
* [FIXED] Erasing the answer of a topic left the link to the answer (an error), marking a new answer after that failed
* [FIXED] Topic page: the link to a post opened the wrong page when deleted or not approved posts were before it; posts of the same second (MySQL) were listed in a random order
* [FIXED] Deleting a user failed when the user had medals or was ignored by other users (SQL Server, MySQL, PostgreSQL)
* [FIXED] Group e-mails were also sent to disabled and not approved users and users without e-mail address
* [FIXED] Disabled and not approved moderators were listed on the team page and got moderator notifications
* [FIXED] Most active users: the users were not taken by their number of posts
* [FIXED] Signature: the BB codes of the rank were not allowed when the groups allowed none
* [FIXED] Notifications: the number of quotes was the number of received thanks
* [FIXED] Edit settings: the page size (sent by the client) was not checked, it is 1 - 100 now
* [FIXED] Active users of a forum or topic: users that hide their online status were shown, guests were shown as members, members excluded from the captcha as crawlers
* [FIXED] My Topics (watched topics): topics of forums the user can't read anymore were listed, the number of topics for the pager was the number of all topics
* [FIXED] Admin edit forum: the sub forums were not moved with the forum to another category
* [FIXED] Forum statistics: the counts of the parent forum were not updated with a change in a sub forum
* [FIXED] Deleting a forum and moving its topics: the links of moved topics to topics of the new forum stayed, a user with an own access mask in both forums made the delete fail
* [FIXED] Admin medals: saving a medal of another board, saving a medal without a message failed
* [FIXED] Forum jump: the names of the categories were not encoded (html of a category name was shown as html)
* [FIXED] Team page: the names of the moderated forums were shown double encoded in the forum select ("Tom &amp; Jerry")
* [FIXED] Categories were not listed in their sort order
* [FIXED] Admin -> Event log / Spam log: "Delete all" deleted the entries of all boards (the spam log also all other entries), a single entry of another board could be deleted
* [FIXED] Reported posts: the first report of a user was counted as 0 reports
* [FIXED] Message history: the history of a post that was never edited, or of a post that doesn't exist, showed an error
* [FIXED] Event log: the log was never trimmed to the maximum number of entries (only 100 entries were checked)
* [FIXED] Admin -> Private messages: pruning deleted the messages of all boards, 0 days deleted all messages
* [FIXED] A new group created for a role (e.g. of an imported or external user) got the guest access to all forums (also staff forums) instead of the access of the start groups
* [FIXED] Friends: "Approve all" and "Deny all" did nothing (the received requests were never found)
* [FIXED] Thanks: posts of forums the user can't read (and the own posts) could be thanked and their thanks listed, the name of the author was not encoded in the box after removing a thanks, removing a thanks didn't remove its notifications
* [FIXED] Notifications: replies in watched topics and forums didn't update the notification counter of the user until its cache expired
* [FIXED] Multiple boards: an admin could change (and move to the own board) the spam and replace words, access masks, BB codes, categories, ranks and groups of another board with their id
* [FIXED] Admin -> Boards: creating a board changed the culture and language of all boards
* [FIXED] Board statistics: deleted posts, the posts of deleted topics and deleted topics were counted
* [FIXED] Admin -> Boards: deleting a board failed when it had tags, banned user agents or custom profile fields (SQL Server, MySQL, PostgreSQL)
* [FIXED] Registration and e-mail verification: the verification key (an identity token) could be longer than its column (255 characters) and the registration failed on SQL Server, MySQL and PostgreSQL; only a hash of the key is stored now (links sent before the update still work), "resend the verification mail" (login and admin dashboard) sends a new link, on the admin dashboard it could send the mail of another user whose name contains the name of the not approved user
* [FIXED] Watch mails: watchers who can't read the forum (anymore) got the mail with the post, the hidden content of a post ([hide], [hide-reply], [hide-thanks], ...) was in the mail, a mail error failed the post, and the parallel sending removed the request context of the post (errors after the notification)
* [FIXED] Moderator mails (approval and reported posts): a mail error failed the post that needs the approval, an error stopped the mails to the other moderators, and the parallel sending removed the request context of the post
* [FIXED] Mails: the role and suspension mails (sent by an admin) were in the language of the admin, mails to users without an own language were in the language of the current user instead of the board language
* [FIXED] Html mails: topic names, names of users, posts and report reasons were inserted as html into the html mails (html and script of users in the mail), and their brackets were formatted as bb codes (disguised links)
* [FIXED] Mails: every mail error was ignored (e.g. the registration said the verification mail was sent, the SMTP test of the admin always succeeded); when no mail could be sent the error is shown now (registration, admin register user, forgot password, resend verification, admin mail and its test), the notification mails (roles, medals, suspension, welcome, registration and spam bot notifications) log the error without failing the action, the users import continues
* [FIXED] Admin -> Mail: the group mail was sent in parallel without the context of the request (errors of the services), the spam bot notification went to the first group whose name contains "Admin" instead of the admin groups
* [FIXED] The post flood delay was only checked against the last post of the session, a new session (or another browser) could post right away; the delay now uses the last post of the user, for guests the last post from the same address
* [FIXED] "Allow topics with the same name": a new topic was rejected when any topic subject only contained the subject (also deleted topics), editing the subject of a topic was never checked
* [FIXED] Security: Restoring the own post also restored all replies, also the ones deleted by a moderator; only moderators restore the replies too, the restore button of the delete message page checks the access
* [FIXED] The post count of a topic got wrong when a deleted post was deleted again (e.g. the form sent twice) or erased later (Admin > Restore) and when a post or a deleted topic was restored with its replies (counted twice), a topic with a count of 0 was not listed anymore
* [FIXED] Approving or restoring a post did not update the search index, approved posts kept their not approved flags and restored posts were only found again after rebuilding the index
* [FIXED] A too long guest name was replaced with the characters after the first 100 instead of being shortened to 100 characters (post topic, post and edit message)
* [FIXED] Security: The registration showed a warning for a display name that is already used but registered the user anyway (other users could be impersonated), and a display name (registration, profile and admin edit user) was not checked like the user name, so a user could use the guest name, a bad word or a name with a semicolon; the rules are now in UserHelper.IsAllowedName and used everywhere
* [FIXED] Post topic / reply / edit as guest: a guest could post with the user name or display name of a member (the post looked like a post of the member), and a guest who cleared the name field posted with an empty name (the name of the guest user is used now)
* [FIXED] Security: The message history showed the text of deleted posts and of posts waiting for approval to every member who can read the forum, the report post page showed them too; deleted and not approved posts of others could be quoted, and posts of other topics and forums (also without read access) by adding their ids to the multi quote list
* [FIXED] Security: The "block e-mails" option and the ignore list of the users were never checked by the e-mail form, and the user id was only checked when the page was shown (not when sending), so mails could be sent to unapproved and deleted users and users of other boards; a user can send at most 5 such e-mails per hour, and a missing subject or text shows a warning instead of an error
* [FIXED] Security: The e-mail topic form sent the posted subject and text to any address (also for guests), the forum could be used to send spam; only the recipient can be chosen now (the subject is the topic name, the text the link to the topic), guests can't use it and a user can send at most 5 such e-mails per hour
* [FIXED] Security: The password reset page (usable by anybody) sent an email for every request, so the inbox of a user could be flooded; an account gets at most 3 reset emails per hour, further requests show the same message
* [FIXED] New topic / reply: mentioning or quoting a name without user (a guest post, a deleted user, a changed display name) failed with an error after the post was saved; mentioned and quoted users are only notified when they can read the forum, and also for quick replies and posts approved by a moderator
* [FIXED] Custom profile fields: the profile page (and admin edit user) did not check the required fields at all, number fields accepted any text, the value was not limited to the length of the field and a manipulated field id caused an error; the register page showed a missing text (REGISTER.NEED_CUSTOM) without the name of the field and required fields that are not shown on the register page blocked the registration. They are checked against the field definitions of the board now (new text EDIT_PROFILE.NEED_CUSTOM_NUMBER)
* [FIXED] Security: The texts of the script blocks (translations, names, dialog titles and messages, placeholders) were written into the JavaScript strings without encoding, a quote broke the script (e.g. a translation like "l'URL") and a text with </script> could end the script element; the name of the board was shown as html in the title of the notifications. The texts are encoded now
* [FIXED] SQLite: parallel requests failed with "database is locked" (error page); the database is switched to the WAL journal mode once at the start (unless the connection string sets "Journal Mode"), connections wait up to 5 seconds for a lock (unless the connection string sets "BusyTimeout") and are pooled (unless the connection string sets "Pooling"), and an operation that still fails with "database is locked" is tried again (up to 3 times)
* [FIXED] Security: Admin > Edit Board, Languages / Edit Language, Run SQL, ReIndex, Restart App and Test Data had no host admin check and could be granted to the admins of a board (page access), they could create and rename boards, change the language files of all boards (html texts), run any sql and restart the application; these pages are host admin only now, also when an access was stored before
* [FIXED] Security: The name of the board was shown as html in the header of every page, in the login notice for guests and in the cookie notice, the admin of a board could run script for all users (also the host admin); the topic name was written as html into the page title
* [FIXED] Security: The select menus showed the texts of the options as html: the simple selects (e.g. the rank and group filter of the members page), the lists with icons (parent forum of Admin > Edit Forum, the image, avatar and country lists, the forums of the moderators on the team page with their category names) and the tags of the post forms (tag search), a name with html ran script on these pages; a quote in a name broke the list. The texts are encoded now
* [FIXED] Security: The admin and moderator pages showed the names and descriptions of groups, ranks, access masks, medals (also the category and the messages of the awards), profile fields, bb codes and tags, the styles of groups and ranks, the reasons of banned ips, e-mails and names, banned user agents, user names, e-mail addresses and user agents, the deleted topics and posts (Admin > Restore), the forum, category and topic names of the moderation pages and the report texts and reporter names of the reported posts as html; any user who can report posts could run script in the browser of the moderators (stored XSS). They are encoded now
* [FIXED] Security: Display names and user names of the last poster were shown as html in the topic lists, the active discussions and the forum last post column ("by ..."), topic names in the notification and activity lists, forum names in the active discussions, the user name in the print view and the header of the e-mail page; the rank name, the custom profile fields (also their values entered by the users), poll choices, topic tags, attachment file names, group names and the forum names of the similar topics were shown as html too, any member could run script for everybody viewing these pages (stored XSS). They are encoded now
* [FIXED] Security: The print view of a topic and the posts feed published the text of deleted posts when "show deleted messages to all" is enabled (the topic only shows a placeholder for them)
* [FIXED] Security: The search index contained the hidden content of posts (hide, hide-reply, hide-thanks, group-hide, hide-posts): every user could find posts by words of hidden content and see them in the highlighted part of the result. Hidden content is not indexed anymore; run Admin > ReIndex to remove it from an existing index. An updated post (edit, approve, restore) lost the tags of its topic in the index
* [FIXED] Security: The page load took the forum (and so the access rights) from the forum and category ids of the address when both were set, a topic or message id of another forum was not checked against them; e.g. the print page showed a topic of a hidden forum with the ids of a readable forum in the address (PrintTopic?t={hidden topic}&f={readable forum}&c={category}). The forum is now always the forum of the topic/message
* [FIXED] Active users: a visitor with the same browser as an active crawler was never recorded (and not shown as active), crawlers with other addresses were merged into one, a crawler could take over the session of a person with the same address and browser, a parallel request of a new session failed with the unique key of the session and a page load failed when the access rows were cleared at the same time (group change)
* [FIXED] Security: Notifications (page and popup): the notifications showed the topic names of topics the user can't read anymore (moved to a hidden forum, access removed) and of deleted topics, the popup showed the topic names and display names as html (a deleted topic caused an error)
* [FIXED] Security: Polls: a poll id of another forum could be edited with the topic of a forum the user moderates, the choices were updated and deleted by the ids sent with the form (also choices of other polls), the images of the question and the choices, multiple choices and hidden results were saved from a manipulated form although the settings only allow them for admins (and moderators), a poll could be added to any topic (also of other users and topics with a poll) and an invalid expiry value caused an error; poll votes were counted for the choice also when the user had voted already (parallel requests) and for choices of other polls, and every user with vote access could remove the poll
* [FIXED] Security: Topic subjects, forum names and starter names (e.g. the name a guest chooses) were not encoded in the digest e-mail, so html like links could be injected into the digest of other users; the digest contained the hidden content of posts (hide-reply, hide-thanks, ...) as plain text, also for users who do not meet the condition
* [FIXED] Digest: the period of the topics was compared with the local time of the server (the posts are utc), on a server in another time zone than UTC new topics were missing or old topics were sent again
* [FIXED] Security: The admin pages did not check the board of the records in the address, an admin of one board could edit or delete the categories, forums, groups, ranks, medals, access masks and bb codes of other boards; "approve all" and "delete all" of the unapproved users (admin dashboard) approved and deleted the login accounts of all boards, and "delete all topics without posts" (Admin > Restore) deleted the deleted topics of all boards
* [FIXED] Security (hardening): The event log showed the request url of an entry only with html tags removed (an unclosed tag stays), it is now html encoded
* [FIXED] Security: The spam log showed the request url of an entry as html, a visitor could put script into the address of a request that was logged as spam and run it in the browser of the admin (stored XSS)
* [FIXED] Security: Editing a topic did not remove html from the new subject, description and styles (only new topics did), and the delete message page showed the topic title without encoding
* [FIXED] Security: Stored XSS with topic titles: the search results and the similar titles (shown with javascript) used the topic title, description, tags and forum name without html encoding; the search results are now html encoded (ISearch results: Topic, Description, TopicTags, ForumName)
* [FIXED] Security: XSS with the context menu of posts: the selected text of a post was inserted as html and into "javascript:" links (only quotes, semicolons and brackets were removed, which also changed quoted text like "don't"); the menu items are now created as elements with click handlers
* [FIXED] Security: XSS with the mention list of the editors (@name): display names were inserted as html; the guest user was listed
* [FIXED] Security: The admin language editor used the file name of the address as path, so it could read and overwrite files outside the languages folder (e.g. "../web.config"); only existing language files of the folder can be edited now
* [FIXED] Security: Friend requests were not checked on the server (only the button was hidden): requests to users who block friend requests, to the guest account, to unapproved users, users of other boards and with the friends feature disabled are refused now; the names of friends in the notifications (friend request approved, friend removed) were shown as html
* [FIXED] Removing a friend from the menu of a post removed the friend twice and its notification was never shown (the page was redirected before), the notification of a friend request from a post was lost the same way
* [FIXED] Security: The topic lookup of the move/merge dialogs listed the topics of every forum to moderators of any forum (also hidden forums) and the topics of forums of other boards to admins, now only forums of the board the user is admin or moderator of
* [FIXED] Security: Kill user (admin edit user) banned the e-mail, IP and name and deleted all posts before checking that the user is not the admin himself, the guest or a host admin; suspend had no checks at all. The checks now come first, apply to delete and suspend, and also protect admins
* [FIXED] Security: Admin edit user suspend showed the suspend reason as raw HTML and the names of the users in the notification as html
* [FIXED] Admin edit user suspend: the end of the suspension was computed from the local time of the suspended user but saved and shown as UTC (the user was suspended hours too long or too short), a suspension in minutes was added as hours and a count that is not a number caused an error
* [FIXED] Print view and feeds: a post with a spoiler was replaced by the content of the first spoiler (the rest of the post was lost), the posts of the feeds were formatted twice (the html of the first pass was shown as text)
* [FIXED] Security: The good words of the bad word list were inserted as html into posts, topic names and descriptions (the text is html at that point), a replacement with html changed every post; the good words are texts now (IBadWordReplace.ReplaceHtml)
* [FIXED] Links of posts older than "disable nofollow links after days" still got rel=nofollow, the age of the post was computed negative
* [FIXED] Security (hardening): BBCode [url] links allowed control characters in the url (browsers remove them, e.g. in front of "javascript:"), they end the link now
* [FIXED] Code blocks: the language and the highlighted lines of [code=...] were written into html attributes with all characters (e.g. quotes and brackets of a manipulated post broke the markup), and a highlighted block of a mapped language (e.g. [code=cs;2-3]) was not highlighted (the language was mapped before the lines were taken off)
* [FIXED] A plain url (without [url]) in a post or an e-mail removed the text before it on the same line (e.g. "Please see https://..." became only the link), the youtube check consumed the line
* [FIXED] [size=...] had no upper limit (e.g. [size=99999] covered the whole page), larger values are limited to 400% now; [size=9] was 9 pixels (smaller than size 1) instead of the largest size of the list (180%)
* [FIXED] Emoji short codes and ascii smileys inside code blocks were turned into emoji (e.g. a table row "[td]" in code became a flag)
* [FIXED] Security: Quick reply accepted replies to any topic, also without reply access (read-only forums) and to locked topics and forums; quick replies were saved without the preparation of the other post forms (html removed, content of code blocks encoded), markup inside a code block of a quick reply was shown as html to everybody reading the topic (stored XSS)
* [FIXED] Security: The edit page let authors edit their posts without edit access in the forum, and authors could still edit and delete posts locked by a moderator and restore their posts that were deleted by a moderator; every guest could edit and delete the posts of all guests with the edit and delete pages (the address was only checked for the buttons), and a reply with only a message id in the address (quote) did not check the reply access
* [FIXED] Security: The login page and the login box showed "account not approved" (and allowed resending the verification mail) for any password, so the state of an account could be found out with the user name only; accounts disabled by "lock inactive accounts" could still log in (only the forum flags were changed, not the login account)
* [FIXED] Security: Uploaded avatars (stored in the database) were saved and served with the content type sent by the browser (e.g. text/html), now the type of the image is saved and only image types are served
* [FIXED] Security (hardening): Theme names were not checked for path characters, a theme like ".." pointed outside the themes folder
* [FIXED] Security: The album edit page used the album id of the address without checking the owner, any user could rename the albums of other users, upload images into them and delete them with their images; the cover of an album could be set to images of other albums, and the albums and album images were shown without checking the profile view permission
* [FIXED] Security: The user hover card showed the country, region and rank as html and used any home page url (also with quotes and "javascript:") as link, the country of the profile was written unencoded into the flag icon of the profile page; a crafted country sent with the profile form ran script for every visitor (stored XSS). Only web addresses are used as home page link, the values are encoded
* [FIXED] Security: Removing a user from a group, changing a group, changing the access mask of a group in a forum and adding or changing the access mask of a single user in a forum (forum moderators dialog) did not refresh the cached access rights, so revoked access (e.g. a removed moderator) kept working for a while
* [FIXED] Security: With the spam handling "delete and ban user", a spam post of a guest deleted the posts of all guests and the guest account (or failed with an error); the guest account is never deleted and banned now, the spam post is still rejected
* [FIXED] Security: The moderator pages (reported and unapproved posts, forum topics) only checked that the user is a moderator of any forum, moderators of one forum could moderate every forum; moving a topic or a post (also into a new topic, also several topics in the moderator forum view) did not check the target, moderators could move topics and posts into forums they do not moderate or of other boards, and the move topic dialog did not check the moderator access at all
* [FIXED] Security: The moderators of any forum could restore old versions of the posts of every forum (message history), the restore was not checked on the server
* [FIXED] Security: The forgot password page showed the full e-mail address of not yet approved accounts to anybody who entered the user name (as html in the notification), the address is now masked
* [FIXED] Security: Private messages could be sent to users who block private messages or ignore the sender and to unapproved, deleted users and users of other boards; only approved friendships are an exception of the block (new text PMESSAGE.RECIPIENT_NOT_ALLOWED)
* [FIXED] Security: The search api did not check the search permission of the board (the results were still limited to readable forums), and the page size of the search was not limited; the search returned posts that were not approved yet (e.g. spam waiting for moderation) to everybody with read access, now only moderators of the forum find them
* [FIXED] Security: The "delete selected" of the own attachments deleted the attachments of any id sent with the form (the id is a hidden field), also of other users
* [FIXED] Security: The "delete selected" of the private messages deleted the messages of any id sent with the form (the id is a hidden field), also the messages of other users
* [FIXED] Security: The forum access of the edit forum and edit group pages used the group and forum ids of hidden fields without checking the board, admins could change the forum access of groups and forums of other boards
* [FIXED] Security: Logout was a plain link, so any other website could log a forum user out with a link or an image; the logout link has a token of the session now, a request without it does not log out
* [FIXED] Security: A group added for a role (Admin > Groups, roles without group) got the access mask with the id 1 in every forum (a mask of the first board, e.g. the admin access) instead of the access of the start group; check the groups created this way on existing installs
* [FIXED] Security: The attachments of deleted posts and of posts waiting for approval could be downloaded by everybody who can read the forum (e.g. by counting up the id), now only by moderators of the forum (and the author of a not approved post)
* [FIXED] New topic: the tags were only saved for approved topics of users with the activity stream enabled (not for guests, moderated forums and users who turned the activity stream off)
* [FIXED] Move message: the topic list offered deleted topics and the links of moved topics as targets, a message moved there was no longer visible; such topics are no longer listed or accepted
* [FIXED] Attachments: the allowed file extensions setting is read with spaces and dots ("jpg, .png"), before such an entry never matched and the files were rejected as "Invalid File"
* [FIXED] Requests for unknown album images wrote an event log entry each time (the event log could be flooded), they now only return "not found"
* [FIXED] Register: the user name, e-mail and display name were checked trimmed but saved as typed (a space after the user name gave an "invalid user name" error at the login), the password was saved trimmed while the login uses it as typed (a password with a space at the start or end never worked), the same for the passwords of Admin > Register User and Admin > Edit User > Change Password
* [FIXED] RSS/Atom feeds: the times of the posts were wrong by the time zone of the user (the time of the user was marked with the time zone of the server, the update time of the topics feed even got the offset twice); the feeds contain utc times now, the feed reader shows them in its time zone
* [FIXED] Profile > Delete Account > Suspend: the 30 days were counted from the local time of the user instead of UTC, the suspension ended hours too early or too late
* [FIXED] Profile: the birthday was converted like a point in time (from the time zone of the user into the time zone of the viewer), viewers west of the user saw the day before; it is shown as a date now
* [FIXED] Active discussions: guests got the cached list of the page that filled the cache first, the index of a category showed the topics of all categories (or the other way round); the guest cache is kept per category now
* [FIXED] Changed custom profile fields of a user and changed or deleted custom profile definitions were still shown in the posts and the profile (cached data), the cache is cleared now when the profile or a definition changes
* [FIXED] Marking a post as answer wrote the flags of the post as they were when the topic was shown, e.g. a post deleted by a moderator in the meantime was undeleted; the current flags are changed now
* [FIXED] The team page listed all forums of a moderator, also forums the viewer can't read (the names of hidden forums were shown); it now lists only readable forums
* [FIXED] Albums: the page for a new album compared the number of albums with the allowed images per album (instead of the allowed albums), and the limits of albums and images were only checked when the page was shown (not for the upload)
* [FIXED] Reporting the same post again sent the moderators another email each time (a user could flood their inboxes); the moderators are notified once per user and post, further reports are only counted; a report without text (only spaces) was saved
* [FIXED] Profile and admin edit user: a too short (or empty) display name showed the "too long" message
* [FIXED] When the spam check deleted a user (signature, profile homepage), the user stayed logged in and the signature and profile pages still saved the changes
* [FIXED] The topic search of the move/merge dialog removed the current topic from the cached topic list, so that topic was missing from later searches
* [FIXED] Report texts with a question mark were cut off at the first question mark in the reported posts list
* [FIXED] Editing a post of a sticky topic or announcement without priority access reset the topic to a normal topic, editing a post as topic starter removed the topic title styles set by a moderator, and editing a reply (by a user who did not start the topic) removed all tags of the topic; they are only changed when they are shown in the form
* [FIXED] Deleting attachments (own attachments, admin attachments list and edit user, erased posts and deleted users) only deleted the database entry, the files of attachments stored in the file system stayed on the server
* [FIXED] "Lock inactive accounts" (admin users page) also locked host admins, admins and the admin running it, and a value of 0 or less years locked (almost) all users, a value that is not a number failed with an error
* [FIXED] Spam bot auto-bans (and the bans of kill user) saved the bot's name and e-mail as unanchored regular expressions, so later registrations containing that name (e.g. "Joanna" after banning "ann") were treated as bots and banned too (review the banned names and e-mails created by the spam system on existing installs)
* [FIXED] The registration bot check ignored wildcard IP bans (e.g. 192.168.1.*), and an invalid banned name mask cancelled a match on a banned e-mail or IP
* [FIXED] The "watch topic" option of the quick reply was ignored for admins and moderators
* [FIXED] The StopForumSpam and BotScout bot checks sent the user name, e-mail and ip without url encoding (e.g. a + in an e-mail address became a space, an & in a name cut off the request), a StopForumSpam response without a user name caused an error
* [FIXED] The forgot password page said that a new verification e-mail was sent to not yet approved accounts, but none was sent; it is sent now (only while the e-mail verification is pending, accounts waiting for the approval of an admin get no e-mail)
* [FIXED] Creating a category or forum failed when a category or forum with the same name existed on another board, renaming a category or forum to a name that already exists (in the board or the category) caused an error instead of the "name exists" message
* [FIXED] Exporting the selected bb codes failed when an id was not found
* [FIXED] The continue link of the info page used the url parameter without a check (javascript: links and links to other sites), the user name and suspension reason were shown as html
* [FIXED] Disabled (soft deleted) users were shown without the "disabled" badge in the admin users list
* [FIXED] Attachments stored in the database were saved without (or with incomplete) data after the image check had read the upload, resized images were decoded again from the end of the stream and the upload failed
* [FIXED] Missing translations were shown as empty text, translations without a text did not fall back to English
* [FIXED] A new installation on SQL Server, SQLite or PostgreSQL did not create the identity tables (AspNetUsers etc.), they were only created for MySQL
* [FIXED] User claims were stored with the value type instead of the claim type and could not be removed
* [FIXED] The lock objects of the cache were created without synchronization (parallel requests could load the same cache entry at the same time), a key with the hash int.MinValue caused an error
* [FIXED] The "last read" time of the database read tracking ignored the read topics (the forums were read twice)
* [FIXED] Editing a banned IP did not refresh the cached ban list, the old mask stayed banned until the application restarted
* [FIXED] Isolated events (RaiseIssolated, e.g. the application start) were handled twice, and an exception in a handler was not isolated
* [FIXED] Event log entries logged with a Type as source showed "System.RuntimeType"
* [FIXED] One invalid banned e-mail mask disabled all banned e-mail checks (one invalid user agent pattern all user agent bans), banned e-mail, name and user agent masks are now validated as regular expressions
* [FIXED] The ban imports accepted invalid entries, IP imports now accept IP addresses and masks (masks were rejected, incomplete addresses like 1.2.3 accepted), name, e-mail and user agent imports only valid regular expressions
* [FIXED] Admin -> Banned IPs: an invalid address closed the dialog (the message was shown on the page), IPv6 addresses could not be banned and incomplete addresses with empty parts were checked part by part with several messages; the address is checked like the import now
* [FIXED] Awarding a medal a user already has caused an error, the medals of a user who got a new medal stayed cached (the cache of no user was cleared), a user of another board could be selected
* [FIXED] The "days before posts are locked" period started again with every edit, it now starts when the post was written
* [FIXED] Medal names and messages with quotes broke the medal ribbon html of the profile (they were written into the attributes without encoding)
* [FIXED] The breadcrumb of a forum nested more than one level deep did not show all parent forums, links and breadcrumbs with a missing name (e.g. of a deleted user) caused an error
* [FIXED] Avatar paths starting with // loaded the avatar from another site
* [FIXED] Reading a notification could leave the unread notification count too high (the cache was cleared before the update)
* [FIXED] The legacy settings (e.g. "enabletopicdescription") were not removed on case sensitive databases when they were saved in mixed case
* [FIXED] The search index update stored its last run in server time but compared it with UTC post times, so posts were missing from the search on servers east of UTC
* [FIXED] One custom BBCode with an invalid search expression broke the display of all posts, and a custom BBCode whose module class is missing broke the posts using it; such BBCodes are now skipped and logged, the BBCode editor and import validate the search expression
* [FIXED] Admin -> Users -> Import: csv files sent as text/csv and xml files sent as application/xml were rejected as wrong format
* [FIXED] Admin -> Groups: deleting an unknown role (e.g. deleted in the meantime) caused an error
* [FIXED] Search: results were sorted by message id as text ("9" before "10"), so with more hits than "Return Search Max" the newest posts were cut off; the default search (e.g. similar topics) skipped the first 1000 results (the page index started at 1)
* [FIXED] Unticking the multi-quote button of a post did not remove the post from the multi-quote list
* [FIXED] Logged-in users without any forum access got an error page ("Unable to find the Guest User") instead of falling back to the guest permissions
* [FIXED] SimpleReplaceRule with an empty search text never finished
* [FIXED] Admin -> Users -> Import: csv files with an empty last line, rows with fewer or more values than columns or rows without name or e-mail failed the whole import, quoted values kept their quotes, a file without Name/Email column showed a technical error, one failing user stopped the import
* [FIXED] Admin -> Users -> Import: the time zone of the exported users was lost (the import expected a number instead of the time zone name)
* [FIXED] An exception in a run of a background task was not caught in the timer callback (an unhandled exception stops the application), it is logged now and the task runs again
* [FIXED] Emojis with a skin tone (e.g. [raised_hand_tone1], [basketball_player_tone1]) only showed the skin tone color, the emoji itself was missing
* [FIXED] Times shown to guests had no time zone (the text TIMEZONES.NAME_UTC was missing in all languages, every page logged a missing translation); guests now see the offset of the time zone, e.g. " (UTC+02:00)"
* [FIXED] Missing texts were added: members list letter filter, the login buttons of the external providers, the album image button of the editor, the tooltips of the delete post page and the message for moving topics to a category
* [FIXED] Moving topics (move dialog and Moderate -> Forum) with "Leave Pointer" and a number of days below -2 failed with an error instead of the "invalid number of days" message
* [FIXED] A language file of a user or of the board that does not exist (anymore) broke every page (Invalid language file), the board language or english is used instead; a culture of a user shorter than two characters failed the page
* [FIXED] Deleting a forum reported success but did nothing when the previous forum delete task was finished but not yet cleaned up (or still running); a replaced task in the task manager kept the old (disposed) task
* [FIXED] Admin -> Prune: a second prune after a finished one (not yet cleaned up) did nothing but showed "Prune Task Scheduled", a prune while another one is running now shows the running message
* [FIXED] Delete post: a member could open the delete page of the own post of a locked topic (or after the lock period), deleting then just showed the page again; the page and its handlers are denied now with the same rules as the delete and restore buttons of the post
* [FIXED] Saving a name or text longer than its column failed with an error on SQL Server, MySQL and PostgreSQL (not on Sqlite), e.g. the name of an access mask (50 characters): the inputs of the admin pages (access mask, rank, group, forum, board, bb code, banned e-mail, name and ip, replace and spam words), of the poll choices and of "move message" are limited to the length of the column now
* [FIXED] PostgreSQL: deleting a topic (without erasing) failed, the column names of the sql statement that flags its posts as deleted were not quoted
* [FIXED] SQL Server, MySQL, PostgreSQL: a request with a long location parameter, browser, platform or user agent (longer than the columns of the active session) got the error page "Failure Initializing User/Page" instead of the forum; the values are shortened to their columns now
* [FIXED] Albums: an image with the same file name as another image of the album (e.g. phones name every photo "image.jpg") overwrote the file of the other image, both showed the new image and deleting one of them removed the file of the other; the name gets a number now ("image (2).jpg"), the album limit message showed the image limit
* [FIXED] Digest with several boards in one database: the digest task of every application sent the digests of all boards, but with the topics, name and links of its own board, so the users of the other boards got the wrong digest; every application sends the digest of its own board now
* [FIXED] Attachments: since 3.2.14 the attachments of posts (added with [attach]) could only be downloaded by their uploader and the admins, the other members got "insufficient rights" (the larger image and the download); the links shown by the forum are signed now and can be used by everybody with the download permission, any other attachment id is refused
* [FIXED] Security: the id of any attachment (e.g. of another user) could be shown with its download link in an own post, signature or private message; only the attachments of the author are shown now
* [FIXED] Admin -> Ranks: deleting a rank that users have failed with an error on SQL Server, MySQL and PostgreSQL (on SQLite the users kept a deleted rank); a rank in use can't be deleted, like an access mask in use
* [FIXED] Reporting a post: every report of the same user and post is added to the report text, after some reports it was longer than its column (4000 characters) and the report failed with an error on SQL Server, MySQL and PostgreSQL; the oldest reports of the text are removed now
* [FIXED] Edit profile and Admin -> Edit User -> Profile: a real name, location, city, homepage, blog or xmpp longer than 255 characters (the homepage with the added "https://") failed the save with an error on SQL Server, MySQL and PostgreSQL; the fields are checked now
* [FIXED] User import: profile values longer than their columns or a country name instead of the country code (the column has 2 characters) failed the import of the user with an error on SQL Server, MySQL and PostgreSQL; the texts are shortened to the columns and only known countries are taken (also by name)
* [FIXED] Admin -> Users -> Import: a user refused by the login system (e.g. a name with characters that are not allowed, an invalid e-mail) was created as forum user without login and got the registration mail with a password that did not work; such users are skipped and logged now
* [FIXED] MySQL: a custom profile field with a name or default value longer than 255 characters was not saved (Admin -> Profile Definitions), the inputs are limited to the length of their columns
* [FIXED] Admin -> Edit Group and Edit Rank: a new or renamed group or rank with the name of an existing one failed with an error (the name is unique in the board); it is refused with a warning now
* [FIXED] Board index: "Watch all forums" watched nothing (the forums were filtered by an empty category) and "Mark all forums read" only marked the forums loaded so far (the first 20, the others are loaded with "show more"); both use all forums the user can read (of the shown category) now, guests are refused
* [FIXED] Watching an already watched topic or forum again (e.g. from a second tab) failed with an error (the subscription is unique), and guests could watch topics with a sent form; a topic or forum is watched once and guests are refused
* [FIXED] Custom BBCodes with a module: a new, changed or deleted BBCode was only used after the application restarted (its cached list was not cleared)
* [FIXED] Edit settings (also Admin -> Edit User): when the users can't choose a theme or a language the options are not shown, saving the settings reset the theme and the language of the user to the board defaults; they keep their value now, and "hide me" is only changed by users who may hide themselves
* [FIXED] Medals: a medal awarded to a group, a changed medal, a deleted medal and the medals of a group a user joined or left (or a deleted group) were shown in the old way until the cached medals of the users expired
* [FIXED] Host settings: after enabling private messages, friends or albums the counts of the users (unread messages, friend requests, "My albums") were missing until the cached user data expired
* [FIXED] Admin -> Database: "Re-index" failed with an error page on MySQL (and on errors of the database) and "Shrink" showed a database error; on MySQL both run OPTIMIZE TABLE for the forum tables now, other errors are shown as message
* [FIXED] Guests (all are the same user) could add, edit and remove the polls of every guest topic, now only of the guest topics started from the own address (like the buttons of the posts)
* [FIXED] Guests (all are the same user) saw the uploads of all other guests in the attachment list of the editor, the list is empty for guests now
* [FIXED] Unapproved posts: approving an already approved post (another moderator, the button sent twice) counted the post again for the user and sent the notifications again
* [FIXED] Admin -> Boards: the board the forum runs on could be deleted (the context menu always had the delete), the forum was broken afterwards; the delete is not shown for the current board and refused
* [FIXED] Search: the number of results counted the hits the user can not see (forums without read access, not approved or deleted posts), the pager showed empty pages and a search in a hidden forum told the number of hits; all hits were formatted, now only the shown page
* [FIXED] Avatar: the uploaded avatar of a user without uploaded avatar failed with an error, every request (by anyone) wrote it to the event log; it is not found now without an error
* [FIXED] Admin -> Host Settings: an empty number field was saved as 0, and a page size could be 0 (e.g. 0 posts per page failed every topic page); an empty or invalid number keeps the setting now and the page sizes are at least 1
* [FIXED] Admin -> Groups: the administrators group could be deleted, then the board had no admins anymore (only the host admin); it is protected like the guest group now (also on the server)
* [FIXED] Suspension: the mail that the suspension ended was only sent when the user visited the forum before the hourly clean up task lifted the suspension; the task sends it too now, a mail error does not fail the page
* [FIXED] Security: The own e-mail address was changed at once without the password and without confirming the new address, with a taken over session the account could be taken over (change the e-mail, then reset the password); the password is needed now and the address is changed after the confirmation link sent to the new address was opened (an admin still changes it at once)
* [FIXED] Security: Changing the e-mail address (own or by an admin) did not end the other logins of the user and kept the open password reset links valid (the security stamp was not renewed)
* [FIXED] 21 of the 24 genders of the profile (e.g. "Agender", "Non-binary") had no text (they were shown as [GENDER.AGENDER] etc.)
* [FIXED] Register: after the account was created the password check script of the page failed with a JavaScript error (the fields are not shown anymore)
* [FIXED] The context menu script failed with a JavaScript error on pages with list rows without a context menu (e.g. Admin -> Languages and Task Manager)
* [FIXED] Build: the web projects (SQL Server, SQLite, PostgreSQL) still referenced Microsoft.Bcl.AsyncInterfaces 9.0.9, depending on the build order the bin folder got this version and the forum did not start (the binding redirect and YAF.Core use 10.0.13)
* [FIXED] The poll edit page failed with a parser error (the choice inputs got a second MaxLength attribute with the input limits of this version)
* [FIXED] The albums page of a user failed with a parser error (the edit button of the album list had the runat attribute twice)
* [FIXED] Nested quotes with a user name were rendered in the wrong order with swapped attributions
* [FIXED] A quote of a post that does not exist (anymore) showed "Posted by" with a link to the missing post, it shows "wrote" like a quote without post now
* [FIXED] Editing or quoting older HTML posts lost list items, kept formatting inside links as raw HTML, lost the type of notes and turned paragraph ends into flag emojis ([br])
* [FIXED] Listing the users of a role that does not exist caused an error, e.g. the moderator notification of a new post in a forum with a moderator group without role failed
* [FIXED] 18 translations had broken placeholders (e.g. "{0 comments", "{text", "{0} characters}") in Arabic, Finnish, French, Italian, Lithuanian, Polish, Portuguese, Romanian, Slovak and Turkish, showing these texts failed (e.g. the user name length messages of the registration in Polish, the activity texts in Arabic and Turkish); a unit test now checks the placeholders of all translations
* [FIXED] MemoryCache.GetOrSet failed with an error for value types (e.g. a number) that were not cached yet
* [FIXED] New banned user agents (added in the admin page or imported) were not blocked and imported spam words were not checked until the application restarted: inserting rows and deleting rows by a condition did not raise the repository events that clear the cached data
* [FIXED] Admin -> Banned IPs, e-mails and names: the ban date was stored in the time of the server but shown converted from UTC, it was off by the time zone of the server
* [FIXED] The text avatar (letters of the name) was cached by the browser for 30 days, after a display name change the old letters were shown; it is cached for one hour now
* [FIXED] Banned IPs: behind a reverse proxy the address of the proxy was checked instead of the client address (the address saved with the posts), the bans did not work; the client address of the forwarded header is only taken from trusted proxies
* [FIXED] Admin start page: "Approve all" approved and "Delete all" deleted also the disabled accounts (e.g. locked as inactive), which are not listed as not approved users; both only work on the listed users now
* [FIXED] Security: The search index task (it indexes the posts in bulk) still indexed the hidden content of the posts (hide, hide-reply, hide-thanks, ...), every user could find posts by it and see it in the highlighted part of the result; it uses the same index document as a single post now
* [FIXED] Search: a search with the search service that had just written to the index failed (the closed index writer was used), the index readers of earlier searches stayed open, and documents of older versions without flags were never found
* [FIXED] Security: The api of the forum (thanks, album titles and captions, multi quote, notifications, ...) had no protection against cross site requests, a form of another web site could call it with the login cookie of the user (e.g. in Firefox, which sends the cookie with such forms); only the json requests and the requests with an own header of the forum scripts are accepted now
* [FIXED] Private messages: opening a message marked it as read for all recipients, also when the sender opened the sent message and before the access check (any user could mark the messages of others as read); it also overwrote the state of the recipients with the flags of the message (a message deleted from the inbox or outbox came back, the message was archived). Only the message of the recipient is marked as read now
* [FIXED] Private messages: the first message of a conversation did not show the replies to it once the ids of the messages and the user messages differed (e.g. after a message to all users)
* [FIXED] Security: The attachment upload accepted the uploads of forms of other web sites with the login cookie of the user (cross site request), it only accepts the uploads of the upload script of the forum now
* [FIXED] The user card (hover card) and the BBCode list of the editor were cached by the browser 1000 times longer than the online status cache timeout (milliseconds were used as seconds, about 17 hours by default), the user card also in shared caches although it depends on the permissions of the viewer
* [FIXED] Security: The user list of the mentions (@name in the editor) was also served to guests and listed all users for an empty search; it is only for members now, starts with 3 characters, only searches the users of the board and returns 20 users at most
* [FIXED] Security: Every attachment could be opened with the image address of the resource handler and was then shown in the browser with its content type, an uploaded xml file (allowed by default) with scripts ran them in the forum (stored XSS); only images are shown in the page now (no svg), other files are downloaded, the browser must not guess the type (nosniff). A refused download is not counted anymore
* [FIXED] Security: Album images were sent with the content type of the upload (sent by the browser of the uploader, only text types were refused), an image file with the type xhtml or svg ran its scripts in the forum (stored XSS); other types than images are sent as png now
* [FIXED] The guest and admin users of a new board got the join and last visit date in the time of the server (all dates are stored in UTC), and the "popular topic" icon compared the time of the last post (UTC) with the time of the server
* [FIXED] The paged lists of the editor (attachments, album images) and of the notification menu accepted any page size and page number from the client: a negative value failed with an error, a huge page size loaded all rows; the page size is limited to 1 - 100 now
* [FIXED] The [hide-thanks=x] BBCode counted the thanks the reader gave instead of the thanks received (as the BBCode and its message describe), the hidden content was shown to users who just thanked often enough
* [FIXED] [hide-posts=] and [hide-thanks=] with an empty or too big number failed with an error and broke the topic page for every reader; an empty number is 0, a too big one hides the content from all users but the author and the admins
* [FIXED] [attach] and [albumimg] with an id that is a number but no whole number (e.g. 1.5, 1e3, (5) or a too big number) failed with an error and broke the topic page for every reader
* [FIXED] The team page (and the admin page access list) listed disabled and not approved admins
* [FIXED] Security: The return address of the login page (ReturnUrl) was followed after the login whatever it was, a link to the login of the forum could send the user to another site (open redirect, e.g. a fake login page); only addresses of the forum are followed now, the address is no longer html encoded (an address with several parameters was broken)
* [FIXED] Team page: the e-mail button of an admin who blocks e-mails was only shown to admins who are friends (not to all admins and all friends like for the moderators and the private messages)
* [FIXED] Active users (page and admin start page): the list failed with an error when a visitor was at the profile, albums or album of a user that does not exist (anymore, e.g. deleted while visited), the album location took an album id from the user id (any or no album, the page failed) and the own album was shown as the album of another user
* [FIXED] The [group-hide=role] BBCode showed the hidden content to the users with any other role and hid it from the members of the given roles, and [group-hide] without roles hid the content from all members (not only from guests); the content is shown to the members of one of the roles now (without roles to all members)
* [FIXED] Active users: Android phones were shown as Linux and iPods as iPhones (their user agents contain "Linux" and "iPhone OS")
* [FIXED] a number in the address that is too big for an int (e.g. ActiveUsers?v=2147483648) showed an error page
* [FIXED] pages with an unknown forum or topic in the address (e.g. Moderate/Forums, PostTopic, MoveMessage) showed an error page
* [FIXED] the reply page of an unknown topic showed an error page
* [FIXED] the delete page of an unknown message showed an error page
* [FIXED] the album page of an unknown album (or of an album of another user) showed an error page
* [FIXED] the [userlink] example of the BBCode help showed the address of YAF 1.x
* [FIXED] the gender list of the profile showed missing texts (the genders without a text are removed, a profile with one of them shows "other")
* [FIXED] missing help texts of the host settings (SPAM service, SSL) and of the user info of the admin edit user page
* [FIXED] Private messages: a message the recipient deleted disappeared from the outbox of the sender and a message the sender deleted from the outbox disappeared from the inbox of the recipient, the sender could not delete a sent message on the message page, and a message to all users was removed for all recipients when one recipient and the sender deleted it
* [FIXED] Private messages: export all exported only the messages of the current page and export selected could export other messages than the selected ones
* [FIXED] a click on the border of a delete button (not on its icon or text) deleted without the confirmation dialog
* [FIXED] the answer to a post report (private message to the reporter) showed the report text of any post to the moderators of any forum, now only to the moderators of the forum of the post
* [FIXED] Private messages: the mailbox of the recipient was full one message before the limit
* [FIXED] Profile: the region was not saved when the country was selected (or changed) at the same time
* [FIXED] Profile: countries without regions showed an empty region list
* [FIXED] Friends: a removed friend was still shown in the friend list (the approved request of the friend was listed as friendship)
* [FIXED] Members: the filter by number of posts ignored the selected comparison (equal, less or equal), it always used "more or equal"
* [FIXED] Members: a number of posts that is too big for a number showed an error page
* [FIXED] Security: the buttons of the forum (ThemeButton) did not use the event validation, a made up post could click a button that was not shown to the user (e.g. hidden for the user's rights or a turned off feature like the avatar upload)
* [FIXED] Avatar upload: an image file larger than the "Avatar size" setting was saved anyway (the warning was shown afterwards), and the profile upload did not check that the avatar upload is turned on; a too big file is not saved now
* [FIXED] Admin > Edit Medal: an empty, negative or too big sort order (0 - 255) of a user or role medal showed an error page
* [FIXED] Admin > Edit Medal: after opening the role dialog the page had a script error (the scripts of the hidden user dialog were added)
* [FIXED] Admin > Edit Board: a new board whose new admin could not be created (e.g. the name is taken) went back to the board list without a message (and without the board)
* [FIXED] Admin > Boards: the delete button was hidden for the second board of the list (with two boards a board could not be deleted)
* [FIXED] Posting (new topic, reply, quick reply): the message of the flood delay showed only the seconds part of the remaining time (e.g. 30 instead of 90 seconds with a delay over a minute)
* [FIXED] Admin > Edit User > Kill: banning the addresses of the user also banned the own address of the admin (e.g. a user of the same network), the admin was locked out of the forum
* [FIXED] Admin > Event Log / Spam Log: a date filter that is not a date was used as the smallest date (error page with SQL Server)
* [FIXED] Admin > Board Announcement and Admin > Edit User > Suspend: a huge duration showed an error page (date overflow) and a board announcement with 0 or less was saved already expired; the duration is limited to 1 unit - 100 years
* [FIXED] Identity: the user store returned no task for an external login without a user (the first login with a not yet linked login provider failed with an error)
* [FIXED] Identity: HasPasswordAsync of the user store returned the opposite (true for users without a password)
* [FIXED] Admin > Edit Medal: awarding a medal to a role that already has it failed with a database error, the settings are updated now
* [FIXED] Security: suspended users were only sent to the info page by the forum pages, but could still use the api (thank posts, vote, upload files, ...); the api and the file upload refuse their requests now

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
* [FIXED] PM delete IDOR — no ownership check before deleting a UserPMessage by ID.
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
* bug fixes

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
