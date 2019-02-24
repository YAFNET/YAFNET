YetAnotherForum.NET Changelog
====================

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
* [FIXED #3]Stored XSS via file upload
* [FIXED #386] Hover Card Layout Issue


# YetAnotherForum.NET v2.2.4.6

## Fixed Issues:
* [FIXED] always use forum email as sender address


* # YetAnotherForum.NET v2.2.4.5

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
ch

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
