/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
using System;
using System.Text;

namespace YAF.Classes
{
	/// <summary>
	/// List of all pages available in the YAF forum
	/// </summary>
	public enum ForumPages
	{
		forum,
		topics,
		posts,
		profile,
		activeusers,
		moderate,
		postmessage,
		deletemessage,
		movemessage,
		mod_forumuser,
		attachments,
		pmessage,
		movetopic,
		emailtopic,
		printtopic,
		members,
		cp_pm,
		cp_profile,
		cp_editprofile,
		cp_editavatar,
		cp_signature,
		cp_subscriptions,
		cp_message,
		cp_changepassword,
		login,
		approve,
		info,
		rules,
		register,
		search,
		active,
		logout,
		moderate_index,
		moderate_reportedposts,
		moderate_unapprovedposts,
        moderate_reportedabuse,
		moderate_reportedspam,
		error,
		shoutbox,
		avatar,
		im_yim,
		im_aim,
		im_icq,
		im_skype,
		im_msn,
		im_email,
		rsstopic,
		help_index,
		help_recover,
		lastposts,
		recoverpassword,
		showsmilies,
		admin_admin,
		admin_hostsettings,
		admin_boards,
		admin_boardsettings,
		admin_forums,
		admin_bannedip,
		admin_smilies,
		admin_accessmasks,
		admin_groups,
		admin_users,
		admin_ranks,
		admin_mail,
		admin_medals,
		admin_prune,
		admin_pm,
		admin_attachments,
		admin_eventlog,
		admin_nntpservers,
		admin_nntpforums,
		admin_nntpretrieve,
		admin_version,
		admin_bannedip_edit,
		admin_editaccessmask,
		admin_editboard,
		admin_editcategory,
		admin_editforum,
		admin_editgroup,
		admin_editmedal,
		admin_editnntpforum,
		admin_editnntpserver,
		admin_editrank,
		admin_edituser,
		admin_reguser,
		admin_smilies_edit,
		admin_smilies_import,
		admin_replacewords,
		admin_replacewords_edit,
		admin_replacewords_import,
		admin_extensions,
		admin_extensions_edit,
		admin_extensions_import,
		admin_bbcode,
		admin_bbcode_edit,
		admin_bbcode_import,
		admin_reindex,
		admin_runsql,
		admin_taskmanager,
		admin_test_data,
		admin_restartapp
	}
}
