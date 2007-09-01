/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Collections.Generic;
using System.Text;

namespace YAF.Classes.Data
{
	public enum SEARCH_FIELD
	{
		sfMESSAGE = 0,
		sfUSER_NAME = 1
	}

	public enum SEARCH_WHAT
	{
		sfALL_WORDS = 0,
		sfANY_WORDS = 1,
		sfEXACT = 2
	}

	public enum ForumFlags : int
	{
		Locked = 1,
		Hidden = 2,
		IsTest = 4,
		Moderated = 8
	}

	public enum GroupFlags : int
	{
		IsAdmin = 1,
		IsGuest = 2,
		IsStart = 4,
		IsModerator = 8
	}

	public enum AccessFlags : int
	{
		ReadAccess = 1,
		PostAccess = 2,
		ReplyAccess = 4,
		PriorityAccess = 8,
		PollAccess = 16,
		VoteAccess = 32,
		ModeratorAccess = 64,
		EditAccess = 128,
		DeleteAccess = 256,
		UploadAccess = 512
	}

	public enum TopicFlags : int
	{
		Locked = 1,
		Deleted = 8
	}

	public enum UserFlags : int
	{
		IsHostAdmin = 1,
		Approved = 2,
		IsGuest = 4
	}

	public enum RankFlags : int
	{
		IsStart = 1,
		IsLadder = 2
	}

	public enum EventLogTypes : int
	{
		Error = 0,
		Warning = 1,
		Information = 2
	}
}
