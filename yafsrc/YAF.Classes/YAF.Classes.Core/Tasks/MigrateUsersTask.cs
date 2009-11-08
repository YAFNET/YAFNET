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
using YAF.Classes.Data;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Run when we want to do migration of users in the background...
	/// </summary>
	public class MigrateUsersTask : LongBackgroundTask
	{
		private const string _taskName = "MigrateUsersTask";
		public static string TaskName
		{
			get
			{
				return _taskName;
			}
		}

		public MigrateUsersTask()
		{
			
		}

		static public bool Start(int boardId)
		{
			if ( YafTaskModule.Current == null ) return false;

			if ( !YafTaskModule.Current.TaskExists( TaskName ) )
			{
				MigrateUsersTask task = new MigrateUsersTask {BoardID = boardId};
				YafTaskModule.Current.StartTask( TaskName, task );
			}

			return true;
		}

		public override void RunOnce()
		{
			try
			{
				// attempt to run the migration code...
				RoleMembershipHelper.SyncUsers( BoardID );
			}
			catch(Exception x)
			{
				DB.eventlog_create( null, TaskName, String.Format( "Error In MigrateUsers Task: {0}", x ) );
			}
		}
	}
}
