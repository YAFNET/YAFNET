/* YetAnotherForum.NET
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

namespace YAF.Classes.Core
{
	public class LoadMessage
	{
		public LoadMessage()
		{
			YafContext.Current.Unload += new EventHandler<EventArgs>(Current_Unload);
		}

		void Current_Unload(object sender, EventArgs e)
		{
			// clear the load message...
			Clear();
		}

		private string _loadString = "";

		#region Load Message
		public string LoadString
		{
			get
			{
				if (HttpContext.Current.Session["LoadMessage"] != null)
				{
					// get this as the current "loadstring"
					_loadString = HttpContext.Current.Session["LoadMessage"].ToString();
					// session load string no longer needed
					HttpContext.Current.Session["LoadMessage"] = null;
				}
				return _loadString;
			}
		}

		public string StringJavascript
		{
			get
			{
				string message = LoadString;
				message = message.Replace("\\", "\\\\");
				message = message.Replace("'", "\\'");
				message = message.Replace("\r\n", "\\r\\n");
				message = message.Replace("\n", "\\n");
				message = message.Replace("\"", "\\\"");
				return message;
			}
		}

		/// <summary>
		/// AddLoadMessage creates a message that will be returned on the next page load.
		/// </summary>
		/// <param name="message">The message you wish to display.</param>
		public void Add(string message)
		{
			_loadString += message + "\n\n";
		}

		/// <summary>
		/// AddLoadMessageSession creates a message that will be returned on the next page.
		/// </summary>
		/// <param name="message">The message you wish to display.</param>
		public void AddSession(string message)
		{
			HttpContext.Current.Session["LoadMessage"] = message + "\r\n";
		}

		public void Clear()
		{
			string ls = this.LoadString;
			_loadString = string.Empty;
		}
		#endregion
	}
}
