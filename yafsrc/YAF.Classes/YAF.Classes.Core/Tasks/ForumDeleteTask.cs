/* Yet Another Forum.net
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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using YAF.Classes.Data;

namespace YAF.Classes.Core
{
	public class ForumDeleteTask : LongBackgroundTask
	{
		private const string _taskName = "ForumDeleteTask";
		public static string TaskName
		{
			get
			{
				return _taskName;
			}
		}

		private int _forumId;
		public int ForumId
		{
			get { return _forumId; }
			set { _forumId = value; }
		}

		public ForumDeleteTask()
		{
			
		}

		static public bool Start(int boardId,int forumId)
		{
			if ( YafTaskModule.Current == null ) return false;

			if ( !YafTaskModule.Current.TaskManager.ContainsKey( TaskName ) )
			{
				ForumDeleteTask task = new ForumDeleteTask
				                      	{
				                      		BoardID = boardId,
				                      		ForumId = forumId
				                      	};
				YafTaskModule.Current.StartTask( TaskName, task );
			}

			return true;
		}

		public override void RunOnce()
		{
			try
			{
				DB.forum_delete( ForumId );
				DB.eventlog_create( null, TaskName, String.Format( "Forum (ID: {0}) Delete Task Complete.", ForumId ), 2 );
			}
			catch(Exception x)
			{
				DB.eventlog_create( null, TaskName, String.Format( "Error In Forum (ID: {0}) Delete Task: {1}", ForumId, x.Message ) );
			}
		}
	}
}
