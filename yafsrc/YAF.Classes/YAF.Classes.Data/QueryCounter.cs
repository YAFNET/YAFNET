/* Yet Another Forum.NET
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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Web;

namespace YAF.Classes.Data
{
	public sealed class QueryCounter : System.IDisposable
	{
		/* Jaben : 8/28/2007 - this code is used even when it's not in DEBUG */

		private System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
		private string _cmd;

		public QueryCounter( string sql )
		{
			_cmd = sql;

			if ( HttpContext.Current.Items ["NumQueries"] == null )
				HttpContext.Current.Items ["NumQueries"] = ( int ) 1;
			else
				HttpContext.Current.Items ["NumQueries"] = 1 + ( int ) HttpContext.Current.Items ["NumQueries"];

			_stopWatch.Start();
		}

		public void Dispose()
		{
			_stopWatch.Stop();

			double duration = ( double ) _stopWatch.ElapsedMilliseconds / 1000.0;

			_cmd = String.Format( "{0}: {1:N3}", _cmd, duration );

			if ( HttpContext.Current.Items ["TimeQueries"] == null )
				HttpContext.Current.Items ["TimeQueries"] = duration;
			else
				HttpContext.Current.Items ["TimeQueries"] = duration + ( double ) HttpContext.Current.Items ["TimeQueries"];

			if ( HttpContext.Current.Items ["CmdQueries"] == null )
				HttpContext.Current.Items ["CmdQueries"] = _cmd;
			else
				HttpContext.Current.Items ["CmdQueries"] += "<br/>" + _cmd;
		}

		static public void Reset()
		{
			HttpContext.Current.Items ["NumQueries"] = 0;
			HttpContext.Current.Items ["TimeQueries"] = ( double ) 0;
			HttpContext.Current.Items ["CmdQueries"] = "";
		}

		static public int Count
		{
			get
			{
				return ( int ) HttpContext.Current.Items ["NumQueries"];
			}
		}
		static public double Duration
		{
			get
			{
				return ( double ) HttpContext.Current.Items ["TimeQueries"];
			}
		}
		static public string Commands
		{
			get
			{
				return ( string ) HttpContext.Current.Items ["CmdQueries"];
			}
		}
	}
}
