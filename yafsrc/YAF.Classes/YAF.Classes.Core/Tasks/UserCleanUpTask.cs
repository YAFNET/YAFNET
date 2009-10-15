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
using System.Data;
using System.Text;
using System.Threading;
using System.Web;
using System.Linq;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Modules
{
	[YafModule( "Clean Up User Task Starting Module", "Tiny Gecko", 1 )]
	public class UserCleanUpTaskModule : IBaseModule
	{

		private object _forumControlObj;
		public object ForumControlObj
		{
			get
			{
				return _forumControlObj;
			}
			set
			{
				_forumControlObj = value;
			}
		}

		public void Init()
		{
			// hook the page init for mail sending...
			YafContext.Current.AfterInit += new EventHandler<EventArgs>( Current_AfterInit );

		}

		void Current_AfterInit( object sender, EventArgs e )
		{
			// add the mailing task if it's not already added...
			if ( YafTaskModule.Current != null && !YafTaskModule.Current.TaskManager.ContainsKey( UserCleanUpTask.TaskName ) )
			{
				// start it...
				YafTaskModule.Current.StartTask( UserCleanUpTask.TaskName, new UserCleanUpTask() );
			}
		}

		#region IDisposable Members

		public void Dispose()
		{

		}

		#endregion
	}
}

namespace YAF.Classes.Core
{
	/// <summary>
	/// Does some user clean up tasks such as unsuspending users...
	/// </summary>
	public class UserCleanUpTask : IntermittentBackgroundTask
	{
		private const string _taskName = "UserCleanUpTask";
		public static string TaskName
		{
			get
			{
				return _taskName;
			}
		}

		public UserCleanUpTask()
		{
			// set interval values...
			this.RunPeriodMs = 3600000;
			this.StartDelayMs = 30000;
		}

		public override void RunOnce()
		{
			try
			{
				// get all boards...
				List<int> boardIds = TypeHelper.ConvertDataTableColumnToList<int>( "BoardID", DB.board_list( null ) );

				// go through each board...
				foreach ( int boardId in boardIds)
				{
					// get users for this board...
					List<DataRow> users = DB.user_list( boardId, null, null ).Rows.Cast<DataRow>().ToList();

					// handle unsuspension...
					var suspendedUsers = (from u in users
					                      where (u["Suspended"] != DBNull.Value && (DateTime) u["Suspended"] < DateTime.Now)
					                      select u);

					// unsuspend these users...
					foreach ( var user in suspendedUsers )
					{
						DB.user_suspend( user["UserId"], null );
						// sleep for a quarter of a second so we don't pound the server...
						Thread.Sleep( 250 );
					}

					// sleep for a second...
					Thread.Sleep( 1000 );
				}
			}
			catch ( Exception x )
			{
				DB.eventlog_create( null, TaskName, String.Format( "Exception In {1}: {0}", x.Message, TaskName ) );
			}
		}
	}
}
