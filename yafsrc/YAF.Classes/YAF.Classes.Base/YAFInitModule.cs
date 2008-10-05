/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Threading;
using System.Web.Configuration;
using YAF.Classes.Utils;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Runs    
	/// </summary>
	public class YafInitModule : System.Web.IHttpModule
	{

		/// <summary>
		/// Gets ID of board to serve
		/// </summary>
		protected int BoardID
		{
			get
			{
				int boardID;

				// retrieve board id from config, otherwise default is 1
				try { boardID = int.Parse( YAF.Classes.Config.BoardID ); }
				catch { boardID = 1; }

				return boardID;
			}
		}


		#region IHttpModule Members

		void System.Web.IHttpModule.Dispose()
		{
			/* Dispose doesn't work too well on IIS6
			if ( _mailTimer != null )
			{
				System.Diagnostics.Debug.WriteLine( "Disposing timer" );
				_mailTimer.Dispose();
				_mailTimer = null;
			}
			*/
		}


		void System.Web.IHttpModule.Init( System.Web.HttpApplication context )
		{
			try
			{
				// attempt to sync roles. Assumes a perfect world in which this version is completely up to date... which might not be the case.
				YAF.Classes.Utils.RoleMembershipHelper.SyncRoles( BoardID );
			}
			catch
			{
				// do nothing here--upgrading/DB connectivity issues will be handled in ForumPage.cs
			}

			if ( _mailTimer == null )
			{
				System.Diagnostics.Debug.WriteLine( "Initializing mail timer" );
				// get a random ID that will identify this process...
				Random rand = new Random();
				_uniqueId = rand.Next();
				int syncTime = (rand.Next( 10 ) + 5) * 1000;
				// create the mail timer...
				_mailTimer = new System.Threading.Timer( new TimerCallback( MailTimerCallback ), context.Context.Cache, syncTime, syncTime );
			}
		}

		#endregion

		#region "Mail Timer"
		protected static System.Threading.Timer _mailTimer = null;
		protected static object _mailTimerSemaphore = new object();
		protected int _uniqueId = 0;

		private void MailTimerCallback( object sender )
		{
			lock ( _mailTimerSemaphore )
			{
				System.Diagnostics.Debug.WriteLine( "MailTimerCallback Invoked" );
				SendMailThread.SendMailThreaded( _uniqueId );
			}
		}
		#endregion
	}
}
