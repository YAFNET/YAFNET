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
using System.Diagnostics;
using System.Text;
using YAF.Classes.Core;

namespace YAF.Modules
{
	[YafModule("Mail Queue Starting Module","Tiny Gecko",1)]
	public class MailSendingModule : IBaseModule
	{
		private const string _keyName = "MailSendTask";

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
			YafContext.Current.AfterInit +=new EventHandler<EventArgs>( Current_AfterInit );

		}

		void Current_AfterInit( object sender, EventArgs e )
		{
			// add the mailing task if it's not already added...
			if ( YafTaskModule.Current != null && !YafTaskModule.Current.TaskManager.ContainsKey( _keyName ) )
			{
				// start it...
				YafTaskModule.Current.StartTask( _keyName, new MailSendTask() );
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
	/// Sends Email in the background.
	/// </summary>
	public class MailSendTask : IntermittentBackgroundTask
	{
		protected int _uniqueId = 0;
		protected YafSendMailThreaded _sendMailThreaded = new YafSendMailThreaded();

		public MailSendTask()
		{
			// set the unique value...
			Random rand = new Random();
			_uniqueId = rand.Next();

			// set interval values...
			this.RunPeriodMs = ( rand.Next( 10 ) + 5 ) * 1000;
			this.StartDelayMs = ( rand.Next( 10 ) + 5 ) * 1000;
		}

		public override void RunOnce()
		{
			Debug.WriteLine( "Running Send Mail Thread..." );
			// send thread handles it's own exception...
			_sendMailThreaded.SendThreaded( _uniqueId );
		}
	}
}
