/* Yet Another Forum.NET
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
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Threading;
using YAF.Classes.Data;
using YAF.Classes.Pattern;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Runs Tasks in the background -- controlled by the context.
	/// </summary>
	public class YafTaskModule : System.Web.IHttpModule
	{
		private const string _moduleAppName = "YafTaskModule";

		protected static object _lockObject = new object();
		protected static bool _moduleInitialized = false;
		protected static Dictionary<string,IBackgroundTask> _taskManager = new Dictionary<string, IBackgroundTask>();
		protected static HttpApplication _appInstance = null;

		/// <summary>
		/// Current Page Instance of the Module Manager
		/// </summary>
		public Dictionary<string,IBackgroundTask> TaskManager
		{
			get
			{
				return _taskManager;
			}
		}

		/// <summary>
		/// Start a non-running task -- will set the HttpApplication instance.
		/// </summary>
		/// <param name="instanceName">Unique name of this task</param>
		/// <param name="start">Task to run</param>
		public void StartTask( string instanceName, IBackgroundTask start )
		{
			if ( _moduleInitialized )
			{
				// add and start this module...
				if ( !start.IsRunning && !_taskManager.ContainsKey( instanceName ))
				{
					Debug.WriteLine( String.Format( "Starting Task {0}...", instanceName ) );
					// setup and run...
					start.AppContext = _appInstance;
					start.Run();
					// add it after so that IsRunning is set first...
					_taskManager[instanceName] = start;
				}
			}
		}

		void System.Web.IHttpModule.Dispose()
		{

		}

		void System.Web.IHttpModule.Init( System.Web.HttpApplication httpApplication )
		{
			if ( !_moduleInitialized )
			{
				// create a lock so no other instance can affect the static variable
				lock ( _lockObject )
				{
					if ( !_moduleInitialized )
					{
						_appInstance = httpApplication;

						// create a hook into the application allow YAF to find this handler...
						httpApplication.Application.Add(_moduleAppName, this );

						_moduleInitialized = true;

						// create intermittent cleanup task...
						StartTask( "CleanUpTask", new CleanUpTask() { Module = this } );
					}
				} // now lock is released and the static variable is true..
			}
		}

		public static YafTaskModule Current
		{
			get
			{
				if ( HttpContext.Current.Application[_moduleAppName] != null )
				{
					return HttpContext.Current.Application[_moduleAppName] as YafTaskModule;
				}

				return null;
			}
		}
	}
}
