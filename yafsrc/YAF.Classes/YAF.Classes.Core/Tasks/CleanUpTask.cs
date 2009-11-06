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
using System.Web;
using System.Linq;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Automatically cleans up the tasks if they are no longer running...
	/// </summary>
	public class CleanUpTask : IntermittentBackgroundTask
	{
		private YafTaskModule _module;
		public YafTaskModule Module
		{
			get
			{
				return _module;
			}
			set
			{
				_module = value;
			}
		}

		public CleanUpTask()
		{
			// set interval values...
			this.RunPeriodMs = 500;
			this.StartDelayMs = 500;
		}

		public override void RunOnce()
		{
			// look for tasks to clean up...
			if ( Module != null )
			{
				foreach ( string instanceName in Module.TaskManager.Keys )
				{
					IBackgroundTask task = Module.TaskManager[instanceName];

					if ( task == null )
					{
						Module.TaskManager.Remove( instanceName );
					}
					else if ( task != null && !task.IsRunning )
					{
						Module.TaskManager.Remove( instanceName );
						task.Dispose();
					}
				}
			}
		}
	}
}
