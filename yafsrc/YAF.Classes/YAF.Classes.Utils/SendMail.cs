/* Yet Another Forum.net
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
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Specialized;
using System.Threading;

namespace YAF.Classes.Utils
{
	public class SendMailThread
	{
		public SendMailThread()
		{
			Thread currentThread = null;

			try
			{
				HttpContext.Current.Application.Lock();
				currentThread = (Thread)HttpContext.Current.Application ["sendMailThread"];

				if ( currentThread == null || !currentThread.IsAlive)
				{
					currentThread = new Thread( new ThreadStart( SendMailThreaded ) );
					currentThread.Priority = ThreadPriority.BelowNormal;
					HttpContext.Current.Application ["sendMailThread"] = currentThread;
					HttpContext.Current.Application ["sendMailForumEmail"] = yaf_Context.Current.BoardSettings.ForumEmail;
					currentThread.Start();
				}
			}
			finally
			{
				HttpContext.Current.Application.UnLock();
			}

			/*HttpContext.Current.Application ["sendMailForumEmail"] = yaf_Context.Current.BoardSettings.ForumEmail;
			SendMailThreaded();
			*/
		}

		static protected void SendMailThreaded()
		{
			try
			{
				using ( DataTable dt = YAF.Classes.Data.DB.mail_list() )
				{
					string forumEmail = HttpContext.Current.Application ["sendMailForumEmail"].ToString();

					for ( int i = 0; i < dt.Rows.Count; i++ )
					{
						// Build a MailMessage
						if ( dt.Rows [i] ["ToUser"].ToString().Trim() != String.Empty )
						{
							General.SendMail( forumEmail, ( string ) dt.Rows [i] ["ToUser"], ( string ) dt.Rows [i] ["Subject"], ( string ) dt.Rows [i] ["Body"] );
						}
						YAF.Classes.Data.DB.mail_delete( dt.Rows [i] ["MailID"] );
					}
					//if ( PageContext.IsAdmin ) PageContext.AddAdminMessage( "Sent Mail", String.Format( "Sent {0} mails.", dt.Rows.Count ) );
				}
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( 1, "SendMailThread", x );
				/*if ( PageContext.IsAdmin )
				{
					PageContext.AddAdminMessage( "Error sending emails to users", x.ToString() );
				}*/
			}
		}
	}
}
