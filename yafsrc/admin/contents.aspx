<%@ Page language="c#" Codebehind="contents.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.contents" %>

<p></p>

<table class="content" width="100%" cellspacing=0 cellpadding=0><tr><td class="post">
<table class="_content" width="100%" cellspacing=0 cellpadding=0>
	<tr><td class="header2"><b>Admin</b></td></tr>
	<tr><td class="post"><asp:hyperlink runat=server id=HomeLink Target="_top"/></td></tr>
	<tr><td class=post><a target="main" href="main.aspx">Admin Index</a></td></tr>
	<tr><td class=post><a target="main" href="settings.aspx">Settings</a></td></tr>
	<tr><td class=post><a target="main" href="forums.aspx">Forums</a></td></tr>
	<tr><td class=post><a target="main" href="bannedip.aspx">Banned IP</a></td></tr>
	<tr><td class=post><a target="main" href="repair.aspx">Repair</a></td></tr>
	<tr><td class=post><a target="main" href="smilies.aspx">Smilies</a></td></tr>

	<tr><td class="header2"><b>Groups and Users</b></td></tr>
	<tr><td class=post><a target="main" href="accessmasks.aspx">Access Masks</a></td></tr>
	<tr><td class=post><a target="main" href="groups.aspx">Groups (Roles)</a></td></tr>
	<tr><td class=post><a target="main" href="users.aspx">Users</a></td></tr>
	<tr><td class=post><a target="main" href="ranks.aspx">Ranks</a></td></tr>
	<tr><td class=post><a target="main" href="mail.aspx">Mail</a></td></tr>

	<tr><td class="header2"><b>Maintenance</b></td></tr>
	<tr><td class=post><a target="main" href="prune.aspx">Prune Topics</a></td></tr>
	<tr><td class=post><a target="main" href="pm.aspx">Private Messages</a></td></tr>
	<tr><td class=post><a target="main" href="attachments.aspx">Attachments</a></td></tr>

	<asp:placeholder runat="server" visible="true">
	<tr><td class="header2"><b>NNTP</b></td></tr>
	<tr><td class=post><a target="main" href="nntpservers.aspx">NNTP Servers</a></td></tr>
	<tr><td class=post><a target="main" href="nntpforums.aspx">NNTP Forums</a></td></tr>
	<tr><td class=post><a target="main" href="nntpretrieve.aspx">Retrieve Articles</a></td></tr>
	</asp:placeholder>

	<tr><td class="header2"><b>Upgrade</b></td></tr>
	<tr><td class=post><a target="_top" href="../install.aspx">Install</a></td></tr>
</table>
</td></tr></table>
