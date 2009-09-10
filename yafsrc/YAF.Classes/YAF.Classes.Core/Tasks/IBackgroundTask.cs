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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Compilation;
using YAF.Modules;

namespace YAF.Classes.Core
{
	public interface IBackgroundTask : IDisposable
	{
		int BoardID
		{
			set;
		}

		bool IsRunning
		{
			get;
		}


		HttpApplication AppContext
		{
			set;
		}

		void Run();
	}

	public class BaseBackgroundTask : IBackgroundTask
	{
		protected int _boardId = YafControlSettings.Current.BoardID;
		protected bool _isRunning = false;
		protected HttpApplication _appContext = null;
		protected object _lockObject = new object();

		#region IBackgroundTask Members

		virtual public int BoardID
		{
			protected get
			{
				return _boardId;
			}
			set
			{
				_boardId = value;
			}
		}

		virtual public bool IsRunning
		{
			get
			{
				lock ( _lockObject )
				{
					return _isRunning;
				}
			}
			protected set
			{
				lock ( _lockObject )
				{
					_isRunning = value;
				}
			}
		}

		virtual public HttpApplication AppContext
		{
			protected get
			{
				return _appContext;
			}
			set
			{
				_appContext = value;
			}
		}

		virtual public void RunOnce()
		{
			// run once code
		}

		virtual public void Run()
		{
			IsRunning = true;

			RunOnce();

			IsRunning = false;
		}

		#endregion

		#region IDisposable Members

		virtual public void Dispose()
		{
			IsRunning = false;
		}

		#endregion
	}

	public class IntermittentBackgroundTask : BaseBackgroundTask
	{
		protected Timer _intermittentTimer = null;
		protected object _intermittentTimerSemaphore = new object();

		private long _startDelayMs;
		public long StartDelayMs
		{
			get
			{
				return _startDelayMs;
			}
			set
			{
				_startDelayMs = value;
			}
		}

		private long _runPeriodMs;
		public long RunPeriodMs
		{
			get
			{
				return _runPeriodMs;
			}
			set
			{
				_runPeriodMs = value;
			}
		}


		public override void RunOnce()
		{
			base.RunOnce();
		}

		public override void Run()
		{
			if ( !IsRunning )
			{
				// we're running this thread now...
				IsRunning = true;

				// create the timer...
				_intermittentTimer = new Timer( new TimerCallback( TimerCallback ), null, StartDelayMs, RunPeriodMs );
			}
		}

		protected virtual void TimerCallback( object sender )
		{
			lock ( _intermittentTimerSemaphore )
			{
				RunOnce();
			}
		}

		public override void Dispose()
		{
			_intermittentTimer.Dispose();
			base.Dispose();
		}
	}

	public class LongBackgroundTask : IntermittentBackgroundTask
	{
		public LongBackgroundTask()
		{
			StartDelayMs = 50;
			RunPeriodMs = Timeout.Infinite;
		}

		protected override void TimerCallback( object sender )
		{
			lock ( _intermittentTimerSemaphore )
			{
				// we're done with this timer...
				_intermittentTimer.Dispose();
				// run this item once...
				RunOnce();
				// no longer running when we get here...
				IsRunning = false;
			}			
		}
	}
}
