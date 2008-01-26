/* Yet Another Forum.net
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
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.Threading;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// This class needs a little tweaking, but it works.
	/// No sure how it works when the DB isn't availabe.
	/// </summary>
	public class SendMailThread
	{
		Thread _mailThread = null;
		string _forumEmail = string.Empty;
		System.Web.HttpApplication _appContext;

		public SendMailThread( System.Web.HttpApplication context )
		{
			if ( _mailThread == null )
			{
				_mailThread = new Thread( new ThreadStart( SendMailThreaded ) );
				_mailThread.Priority = ThreadPriority.BelowNormal;
				_forumEmail = YafContext.Current.BoardSettings.ForumEmail;
			}

			_appContext = context;
		}

		public void StartThread()
		{
			if ( _mailThread != null && !_mailThread.IsAlive )
			{
				_mailThread.Start();
			}
		}

		public void StopThread()
		{
			if ( _mailThread != null && _mailThread.IsAlive )
			{
				_mailThread.Abort();
			}
		}

		protected void SendMailThreaded()
		{
			HttpContext.Current = _appContext.Context;

			while ( Thread.CurrentThread.ThreadState != ThreadState.Aborted )
			{
				// wait 5 seconds and start the e-mailing thread again...
				Thread.Sleep( 5000 );

				if ( Thread.CurrentThread.ThreadState == ThreadState.Aborted ) break;

				try
				{
					using ( DataTable dt = YAF.Classes.Data.DB.mail_list() )
					{
						for ( int i = 0; i < dt.Rows.Count; i++ )
						{
							// Build a MailMessage
							if ( dt.Rows [i] ["ToUser"].ToString().Trim() != String.Empty )
							{
								System.Net.Mail.MailAddress fromEmailAddress;
								
								if (dt.Rows [i] ["FromUser"].ToString().Trim() != String.Empty) fromEmailAddress = new System.Net.Mail.MailAddress( dt.Rows [i] ["FromUser"].ToString().Trim() );
								else fromEmailAddress = new System.Net.Mail.MailAddress( YafContext.Current.BoardSettings.ForumEmail, YafContext.Current.BoardSettings.Name );

								General.SendMail( fromEmailAddress, new System.Net.Mail.MailAddress( dt.Rows [i] ["ToUser"].ToString() ), dt.Rows [i] ["Subject"].ToString(), dt.Rows [i] ["Body"].ToString() );
							}
							YAF.Classes.Data.DB.mail_delete( dt.Rows [i] ["MailID"] );
						}
					}
				}
				catch ( Exception x )
				{
					// log the error...
					YAF.Classes.Data.DB.eventlog_create( 1, "SendMailThread", x );
				}
			}
		}
	}
}
